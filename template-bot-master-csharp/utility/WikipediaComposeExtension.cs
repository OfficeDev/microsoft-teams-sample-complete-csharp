using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Microsoft.Teams.TemplateBotCSharp.Utility
{
    public static class WikipediaComposeExtension
    {
        const string searchApiUrlFormat = "https://en.wikipedia.org/w/api.php?action=query&list=search&srsearch=[keyword]&srlimit=[limit]&sroffset=[offset]&format=json";
        const string imageApiUrlFormat = "https://en.wikipedia.org/w/api.php?action=query&formatversion=2&format=json&prop=pageimages&piprop=thumbnail&pithumbsize=250&titles=[title]";

        public static ComposeExtensionResponse GetComposeExtensionResponse(Activity activity)
        {
            ComposeExtensionResponse composeExtensionResponse = null;
            ImageResult imageResult = null;
            List<ComposeExtensionAttachment> lstComposeExtensionAttachment = new List<ComposeExtensionAttachment>();
            StateClient stateClient = activity.GetStateClient();
            BotData userData = stateClient.BotState.GetUserData(activity.ChannelId, activity.From.Id);

            var translationHistory = new List<WikiHelperSearchResult>();

            bool IsSettingUrl = false;

            var composeExtensionQuery = activity.GetComposeExtensionQueryData();
            if (string.Equals(activity.Name.ToLower(), Strings.ComposeExtensionQuerySettingUrl))
            {
                IsSettingUrl = true;
            }


            if (composeExtensionQuery.CommandId == null || composeExtensionQuery.Parameters == null)
            {
                return null;
            }

            var initialRunParameter = GetQueryParameterByName(composeExtensionQuery, Strings.manifestInitialRun);
            var queryParameter = GetQueryParameterByName(composeExtensionQuery, Strings.manifestParameterName);

            if (userData == null)
            {
                composeExtensionResponse = new ComposeExtensionResponse();
                string message = Strings.ComposeExtensionNoUserData;
                composeExtensionResponse.ComposeExtension = GetMessageResponseResult(message);
                return composeExtensionResponse;
            }

            /**
                * Below are the checks for various states that may occur
                * Note that the order of many of these blocks of code do matter
             */

            // situation where the incoming payload was received from the config popup

            if (!string.IsNullOrEmpty(composeExtensionQuery.State))
            {
                ParseSettingsAndSave(composeExtensionQuery.State, userData, stateClient, activity);
                /**
                //// need to keep going to return a response so do not return here
                //// these variables are changed so if the word 'setting' kicked off the compose extension,
                //// then the word setting will not retrigger the config experience
                **/

                queryParameter = "";
                initialRunParameter = "true";
            }

            // this is a sitaution where the user's preferences have not been set up yet
            if (string.IsNullOrEmpty(userData.GetProperty<string>(Strings.ComposeExtensionCardTypeKeyword)))
            {
                composeExtensionResponse = GetConfig(composeExtensionResponse);
                return composeExtensionResponse;
            }

            /**
            // this is the situation where the user has entered the word 'reset' and wants
            // to clear his/her settings
            // resetKeyword for English is "reset"
            **/

            if (string.Equals(queryParameter.ToLower(), Strings.ComposeExtensionResetKeyword))
            {
                //make the userData null
                userData = null;
                composeExtensionResponse = new ComposeExtensionResponse();
                composeExtensionResponse.ComposeExtension = GetMessageResponseResult(Strings.ComposeExtensionResetText);
                return composeExtensionResponse;
            }

            /**
            // this is the situation where the user has entered "setting" or "settings" in order
            // to repromt the config experience
            // keywords for English are "setting" and "settings"
            **/

            if ((string.Equals(queryParameter.ToLower(), Strings.ComposeExtensionSettingKeyword) || string.Equals(queryParameter.ToLower(), Strings.ComposeExtensionSettingsKeyword)) || (IsSettingUrl))
            {
                composeExtensionResponse = GetConfig(composeExtensionResponse);
                return composeExtensionResponse;
            }


            /**
            // this is the situation where the user in on the initial run of the compose extension
            // e.g. when the user first goes to the compose extension and the search bar is still blank
            // in order to get the compose extension to run the initial run, the setting "initialRun": true
            // must be set in the manifest for the compose extension
            **/

            if (initialRunParameter == "true")
            {
                //Signin Experience, please uncomment below code for Signin Experience
                //composeExtensionResponse = GetSignin(composeExtensionResponse);
                //return composeExtensionResponse;

                composeExtensionResponse = new ComposeExtensionResponse();

                var userSession = userData.GetProperty<List<WikiHelperSearchResult>>("ComposeExtensionSelectedResults");
                if (userSession != null)
                {
                    translationHistory = userSession;

                    foreach (var searchResult in translationHistory)
                    {
                        WikiHelperSearchResult wikiSearchResult = SetWikiSearchResult(searchResult.imageUrl, searchResult.highlightedTitle, searchResult.text);

                        // create the card itself and the preview card based upon the information
                        // check user preference for which type of card to create
                        lstComposeExtensionAttachment.Add(TemplateUtility.CreateComposeExtensionCardsAttachments(wikiSearchResult, userData.GetProperty<string>(Strings.ComposeExtensionCardTypeKeyword)));
                    }

                    composeExtensionResponse = GetComposeExtenionQueryResult(composeExtensionResponse, lstComposeExtensionAttachment);
                }
                else
                {
                    composeExtensionResponse.ComposeExtension = GetMessageResponseResult(Strings.ComposeExtensionInitialRunText);
                }

                return composeExtensionResponse;
            }


            /**

            * Below here is simply the logic to call the Wikipedia API and create the response for

            * a query; the general flow is to call the Wikipedia API for the query and then call the

            * Wikipedia API for each entry for the query to see if that entry has an image; in order

            * to get the asynchronous sections handled, an array of Promises for cards is used; each

            * Promise is resolved when it is discovered if an image exists for that entry; once all

            * of the Promises are resolved, the response is sent back to Teams

            */

            WikiResult wikiResult = SearchWiki(queryParameter, composeExtensionQuery);

            // enumerate search results and build Promises for cards for response
            foreach (var searchResult in wikiResult.query.search)
            {
                //Get the Image result on the basis of Image Title one by one
                imageResult = SearchWikiImage(searchResult);

                //Get the Image Url from imageResult
                string imageUrl = GetImageURL(imageResult);

                //Set the Highlighter title
                string highlightedTitle = GetHighLightedTitle(searchResult.title, queryParameter);

                string cardText = searchResult.snippet + " ...";

                WikiHelperSearchResult wikiSearchResult = SetWikiSearchResult(imageUrl, highlightedTitle, cardText);

                // create the card itself and the preview card based upon the information
                // check user preference for which type of card to create

                lstComposeExtensionAttachment.Add(TemplateUtility.CreateComposeExtensionCardsAttachments(wikiSearchResult, userData.GetProperty<string>(Strings.ComposeExtensionCardTypeKeyword)));
            }

            composeExtensionResponse = GetComposeExtenionQueryResult(composeExtensionResponse, lstComposeExtensionAttachment);

            return composeExtensionResponse;
        }

        /// <summary>
        /// Purpose of this method is to Keep the history of selected item and return the response Compose Extension
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static ComposeExtensionResponse HandleComposeExtensionSelectedItem(Activity activity)
        {
            WikiHelperSearchResult selectedItem = new WikiHelperSearchResult();
            var translationHistory = new List<WikiHelperSearchResult>();

            BotData userData = TemplateUtility.GetBotDataObject(activity);

            //Get the Max number of History items from config file
            int maxTranslationHistory = Convert.ToInt32(ConfigurationManager.AppSettings["MaxTranslationHistoryCount"]);

            selectedItem = JsonConvert.DeserializeObject<WikiHelperSearchResult>(activity.Value.ToString());

            var userSession = userData.GetProperty<List<WikiHelperSearchResult>>("ComposeExtensionSelectedResults");

            translationHistory = userSession != null ? userSession : translationHistory;

            if (translationHistory != null && translationHistory.Count > 0)
            {
                int index = 0;
                while (index < translationHistory.Count)
                {
                    if (string.Equals(translationHistory[index].highlightedTitle.ToLower(), selectedItem.highlightedTitle.ToLower()))
                    {
                        translationHistory.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            //Add new item in list
            translationHistory.Insert(0, selectedItem);

            //Restrict the transaction History with Max Items.
            if (translationHistory.Count > maxTranslationHistory)
            {
                translationHistory = translationHistory.GetRange(0, maxTranslationHistory);
            }

            //Save the history Items in user Data
            userData.SetProperty<List<WikiHelperSearchResult>>("ComposeExtensionSelectedResults", translationHistory);
            activity.GetStateClient().BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);


            ComposeExtensionResponse composeExtensionResponse = new ComposeExtensionResponse();
            List<ComposeExtensionAttachment> lstComposeExtensionAttachment = new List<ComposeExtensionAttachment>();

            if (selectedItem != null)
            {
                // create the card itself and the preview card based upon the information
                // check user preference for which type of card to create
                lstComposeExtensionAttachment.Add(TemplateUtility.CreateComposeExtensionCardsAttachments(selectedItem, userData.GetProperty<string>(Strings.ComposeExtensionCardTypeKeyword)));
                composeExtensionResponse = GetComposeExtenionQueryResult(composeExtensionResponse, lstComposeExtensionAttachment);
            }
            else
            {
                composeExtensionResponse.ComposeExtension = GetMessageResponseResult(Strings.ComposeExtensionInitialRunText);
            }

            return composeExtensionResponse;
        }

        public static WikiHelperSearchResult SetWikiSearchResult(string imageUrl, string highlightedTitle, string cardText)
        {
            WikiHelperSearchResult objWikiHelperSearchResult = new WikiHelperSearchResult();
            objWikiHelperSearchResult.imageUrl = imageUrl;
            objWikiHelperSearchResult.highlightedTitle = highlightedTitle;
            objWikiHelperSearchResult.text = cardText;
            return objWikiHelperSearchResult;
        }

        // return the value of the specified query parameter
        public static string GetQueryParameterByName(ComposeExtensionQuery query, string name)
        {
            if (query.Parameters[0].Name == name)
            {
                return query.Parameters[0].Value.ToString();
            }
            else
            {
                return "";
            }
        }

        // used to parse the user preferences from the state and save them for later use
        public static void ParseSettingsAndSave(string state, BotData userData, StateClient stateClient, Activity activity)
        {
            userData.SetProperty<string>("composeExtensionCardType", state);
            stateClient.BotState.SetUserData(activity.ChannelId, activity.From.Id, userData);
        }

        public static ComposeExtensionResponse GetConfig(ComposeExtensionResponse composeExtensionResponse)
        {
            string configUrl = ConfigurationManager.AppSettings["BaseUri"].ToString() + "/composeExtensionSettings.html";
            CardAction configExp = new CardAction(ActionTypes.OpenUrl, "Config", null, configUrl);
            List<CardAction> lstCardAction = new List<CardAction>();
            lstCardAction.Add(configExp);
            composeExtensionResponse = new ComposeExtensionResponse();
            ComposeExtensionResult composeExtensionResult = new ComposeExtensionResult();

            ComposeExtensionSuggestedAction objSuggestedAction = new ComposeExtensionSuggestedAction();
            objSuggestedAction.Actions = lstCardAction;

            composeExtensionResult.SuggestedActions = objSuggestedAction;
            composeExtensionResult.Type = "config";
            composeExtensionResponse.ComposeExtension = composeExtensionResult;

            return composeExtensionResponse;
        }

        public static ComposeExtensionResponse GetSignin(ComposeExtensionResponse composeExtensionResponse)
        {
            string configUrl = ConfigurationManager.AppSettings["BaseUri"].ToString() + "/composeExtensionSettings.html";
            CardAction configExp = new CardAction(ActionTypes.OpenUrl, "Config", null, configUrl);
            List<CardAction> lstCardAction = new List<CardAction>();
            lstCardAction.Add(configExp);
            composeExtensionResponse = new ComposeExtensionResponse();
            ComposeExtensionResult composeExtensionResult = new ComposeExtensionResult();

            ComposeExtensionSuggestedAction objSuggestedAction = new ComposeExtensionSuggestedAction();
            objSuggestedAction.Actions = lstCardAction;

            composeExtensionResult.SuggestedActions = objSuggestedAction;
            composeExtensionResult.Type = "auth";
            composeExtensionResponse.ComposeExtension = composeExtensionResult;

            return composeExtensionResponse;
        }

        public static ComposeExtensionResult GetMessageResponseResult(string message)
        {
            ComposeExtensionResult composeExtensionResult = new ComposeExtensionResult();
            composeExtensionResult.Type = "message";
            composeExtensionResult.Text = message;
            return composeExtensionResult;
        }

        public static WikiResult SearchWiki(string queryParameter, ComposeExtensionQuery composeExtensionQuery)
        {
            string searchApiUrl = searchApiUrlFormat.Replace("[keyword]", queryParameter);
            searchApiUrl = searchApiUrl.Replace("[limit]", composeExtensionQuery.QueryOptions.Count + "");
            searchApiUrl = searchApiUrl.Replace("[offset]", composeExtensionQuery.QueryOptions.Skip + "");
            WikiResult wikiResult = null;

            // call Wikipedia API to search
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(searchApiUrl);
                using (HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse())
                {
                    string ResponseText;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        ResponseText = reader.ReadToEnd();
                        wikiResult = JsonConvert.DeserializeObject<WikiResult>(ResponseText);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return wikiResult;
        }

        public static ImageResult SearchWikiImage(Search wikiSearch)
        {
            ImageResult imageResult = null;
            // a separate API call to Wikipedia is needed to fetch the page image, if it exists
            string imageApiUrl = imageApiUrlFormat.Replace("[title]", wikiSearch.title);

            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(imageApiUrl);
                using (HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse())
                {
                    string ResponseText;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        ResponseText = reader.ReadToEnd();
                        imageResult = JsonConvert.DeserializeObject<ImageResult>(ResponseText);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return imageResult;
        }

        public static string GetImageURL(ImageResult imageResult)
        {
            string imageUrl = string.Empty;

            if (imageResult != null && imageResult.query.pages != null && imageResult.query.pages.Count > 0 && imageResult.query.pages[0].thumbnail != null)
            {
                imageUrl = imageResult.query.pages[0].thumbnail.source;
            }
            else
            {
                // no image so use default Wikipedia image
                imageUrl = "https://upload.wikimedia.org/wikipedia/commons/d/de/Wikipedia_Logo_1.0.png";
            }

            return imageUrl;
        }

        public static string GetHighLightedTitle(string title, string queryParameter)
        {
            // make title into a link
            string originalTitle = "<a href=\"" + "https://en.wikipedia.org/wiki/" + title + "\" target=\"_blank\">" + title + "</a>";

            // highlight matched keyword
            string highlightedTitle = title;

            if (queryParameter != null)
            {
                Match matches = new Regex(queryParameter).Match("gi");

                if (matches != null && matches.Length > 0)
                {
                    highlightedTitle = highlightedTitle.Replace(new Regex(queryParameter).Match("gi").ToString(), "<b>" + matches.Value + "</b>");
                }
            }

            return highlightedTitle;
        }

        public static ComposeExtensionResponse GetComposeExtenionQueryResult(ComposeExtensionResponse composeExtensionResponse, List<ComposeExtensionAttachment> lstComposeExtensionAttachment)
        {
            composeExtensionResponse = new ComposeExtensionResponse();
            ComposeExtensionResult composeExtensionResult = new ComposeExtensionResult();
            composeExtensionResult.Type = "result";
            composeExtensionResult.Attachments = lstComposeExtensionAttachment;
            composeExtensionResult.AttachmentLayout = "list";
            composeExtensionResponse.ComposeExtension = composeExtensionResult;

            return composeExtensionResponse;
        }
    }

    public class WikiHelperSearchResult
    {
        public string imageUrl { get; set; }
        public string highlightedTitle { get; set; }
        public string text { get; set; }
    }

    //Wiki Json Result Object Classes
    public class Continue
    {
        public int sroffset { get; set; }
        public string @continue { get; set; }
    }

    public class Searchinfo
    {
        public int totalhits { get; set; }
    }

    public class Search
    {
        public int ns { get; set; }
        public string title { get; set; }
        public int pageid { get; set; }
        public int size { get; set; }
        public int wordcount { get; set; }
        public string snippet { get; set; }
        public string timestamp { get; set; }
    }

    public class Query
    {
        public Searchinfo searchinfo { get; set; }
        public List<Search> search { get; set; }
    }

    public class WikiResult
    {
        public string batchcomplete { get; set; }
        public Continue @continue { get; set; }
        public Query query { get; set; }
    }

    /// <summary>
    /// Image Json Object Classes
    /// </summary>
    public class Normalized
    {
        public bool fromencoded { get; set; }
        public string from { get; set; }
        public string to { get; set; }
    }

    public class Thumbnail
    {
        public string source { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Page
    {
        public int pageid { get; set; }
        public int ns { get; set; }
        public string title { get; set; }
        public Thumbnail thumbnail { get; set; }
    }

    public class QueryImage
    {
        public List<Normalized> normalized { get; set; }
        public List<Page> pages { get; set; }
    }

    public class ImageResult
    {
        public bool batchcomplete { get; set; }
        public QueryImage query { get; set; }
    }
}