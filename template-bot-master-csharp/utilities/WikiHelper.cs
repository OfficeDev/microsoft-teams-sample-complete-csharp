using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Microsoft.Teams.Tutorial.CSharp
{
    public static class WikiHelper
    {
        const string searchApiUrlFormat = "https://en.wikipedia.org/w/api.php?action=query&list=search&srsearch=[keyword]&srlimit=[limit]&sroffset=[offset]&format=json";
        const string imageApiUrlFormat = "https://en.wikipedia.org/w/api.php?action=query&formatversion=2&format=json&prop=pageimages&piprop=thumbnail&pithumbsize=250&titles=[title]";

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

        public static ComposeExtensionResponse GetConfig()
        {
            string configUrl = ConfigurationManager.AppSettings["BaseUri"].ToString() + "/composeExtensionSettings.html";
            CardAction configExp = new CardAction(ActionTypes.OpenUrl, "Config", null, configUrl);
            List<CardAction> lstCardAction = new List<CardAction>();
            lstCardAction.Add(configExp);
            var composeExtensionResponse = new ComposeExtensionResponse();
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
            // highlight matched keyword
            string highlightedTitle = title;

            if (queryParameter != null)
            {
                var regex = new Regex(queryParameter, RegexOptions.IgnoreCase);
                highlightedTitle = regex.Replace(highlightedTitle, "<b>" + queryParameter + "</b>");
            }

            // make title into a link
            string linkTitle = "<a href=\"" + "https://en.wikipedia.org/wiki/" + title + "\" target=\"_blank\">" + highlightedTitle + "</a>";

            return linkTitle;
        }

        public static ComposeExtensionResponse GetComposeExtenionQueryResult(List<ComposeExtensionAttachment> lstComposeExtensionAttachment)
        {
            var composeExtensionResponse = new ComposeExtensionResponse();
            ComposeExtensionResult composeExtensionResult = new ComposeExtensionResult();
            composeExtensionResult.Type = "result";
            composeExtensionResult.Attachments = lstComposeExtensionAttachment;
            composeExtensionResult.AttachmentLayout = "list";
            composeExtensionResponse.ComposeExtension = composeExtensionResult;

            return composeExtensionResponse;
        }
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