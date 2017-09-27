using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.TemplateBotCSharp.Utility;
using System.Collections.Generic;
using Microsoft.Teams.TemplateBotCSharp.Properties;

namespace Microsoft.Teams.TemplateBotCSharp
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                //Set the Locale for Bot
                activity.Locale = TemplateUtility.GetLocale(activity);

                var messageActivity = StripBotAtMentions.StripAtMentionText(activity);
                try
                {
                    await Conversation.SendAsync(messageActivity, () => new Dialogs.RootDialog());
                }
                catch (Exception ex)
                {

                }
            }
            else if (activity.Type == ActivityTypes.Invoke) // Received an invoke
            {
                // Handle ComposeExtension query
                if (activity.IsComposeExtensionQuery())
                {
                    var invokeResponse = this.GetComposeExtensionResponse(activity);
                    return Request.CreateResponse<ComposeExtensionResponse>(HttpStatusCode.OK, invokeResponse);
                }
                else if (activity.IsO365ConnectorCardActionQuery())
                {
                    return await HandleO365ConnectorCardActionQuery(activity);
                }
                else
                {
                    var messageActivity = (IMessageActivity)null;

                    if (activity.Name == "actionableMessage/executeAction")
                    {
                        messageActivity = ParseInvokeActivityRequest.ParseO365ConnectorCardInvokeRequest(activity);
                    }
                    else
                    {
                        messageActivity = ParseInvokeActivityRequest.ParseInvokeRequest(activity);
                    }

                    await Conversation.SendAsync(messageActivity, () => new Dialogs.RootDialog());
                    // Handle other types of invoke
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private ComposeExtensionResponse GetComposeExtensionResponse(Activity activity)
        {
            ComposeExtensionResponse composeExtensionResponse = null;
            ImageResult imageResult = null;
            List<ComposeExtensionAttachment> lstComposeExtensionAttachment = new List<ComposeExtensionAttachment>();
            StateClient stateClient = activity.GetStateClient();
            BotData userData = stateClient.BotState.GetUserData(activity.ChannelId, activity.From.Id);

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

            var initialRunParameter = WikiHelper.GetQueryParameterByName(composeExtensionQuery, Strings.manifestInitialRun);
            var queryParameter = WikiHelper.GetQueryParameterByName(composeExtensionQuery, Strings.manifestParameterName);

            if (userData == null)
            {
                composeExtensionResponse = new ComposeExtensionResponse();
                string message = Strings.ComposeExtensionNoUserData;
                composeExtensionResponse.ComposeExtension = WikiHelper.GetMessageResponseResult(message);
                return composeExtensionResponse;
            }

            /**
                * Below are the checks for various states that may occur
                * Note that the order of many of these blocks of code do matter
             */

            // situation where the incoming payload was received from the config popup

            if (!string.IsNullOrEmpty(composeExtensionQuery.State))
            {
                WikiHelper.ParseSettingsAndSave(composeExtensionQuery.State, userData, stateClient, activity);
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
                composeExtensionResponse = WikiHelper.GetConfig(composeExtensionResponse);
                return composeExtensionResponse;
            }

            /**
            // this is the situation where the user has entered the word 'reset' and wants
            // to clear his/her settings
            // resetKeyword for English is "reset"
            **/

            if (string.Equals(queryParameter.ToLower(),Strings.ComposeExtensionResetKeyword))
            {
                //make the userData null
                userData = null;
                composeExtensionResponse = new ComposeExtensionResponse();
                composeExtensionResponse.ComposeExtension = WikiHelper.GetMessageResponseResult(Strings.ComposeExtensionResetText);
                return composeExtensionResponse;
            }

            /**
            // this is the situation where the user has entered "setting" or "settings" in order
            // to repromt the config experience
            // keywords for English are "setting" and "settings"
            **/

            if ((string.Equals(queryParameter.ToLower(), Strings.ComposeExtensionSettingKeyword) || string.Equals(queryParameter.ToLower(),Strings.ComposeExtensionSettingsKeyword)) || (IsSettingUrl))
            {
                composeExtensionResponse = WikiHelper.GetConfig(composeExtensionResponse);
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
                //composeExtensionResponse = WikiHelper.GetSignin(composeExtensionResponse);
                //return composeExtensionResponse;

                composeExtensionResponse = new ComposeExtensionResponse();
                composeExtensionResponse.ComposeExtension = WikiHelper.GetMessageResponseResult(Strings.ComposeExtensionInitialRunText);
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

            WikiResult wikiResult = WikiHelper.SearchWiki(queryParameter, composeExtensionQuery);
            
            // enumerate search results and build Promises for cards for response
            foreach (var searchResult in wikiResult.query.search)
            {
                //Get the Image result on the basis of Image Title one by one
                imageResult = WikiHelper.SearchWikiImage(searchResult);

                //Get the Image Url from imageResult
                string imageUrl = WikiHelper.GetImageURL(imageResult);

                //Set the Highlighter title
                string highlightedTitle = WikiHelper.GetHighLightedTitle(searchResult.title, queryParameter);

                string cardText = searchResult.snippet + " ...";

                // create the card itself and the preview card based upon the information
                // check user preference for which type of card to create

                lstComposeExtensionAttachment.Add(TemplateUtility.CreateComposeExtensionCardsAttachments(highlightedTitle, cardText, imageUrl, userData.GetProperty<string>(Strings.ComposeExtensionCardTypeKeyword)));
            }

            composeExtensionResponse = WikiHelper.GetComposeExtenionQueryResult(composeExtensionResponse, lstComposeExtensionAttachment);

            return composeExtensionResponse;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                //uncomment the below line to handle cnversation update messages
                //TeamEventBase eventData = message.GetConversationUpdateData();

                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        /// <summary>
        /// Handles O365 connector card action queries.
        /// </summary>
        /// <param name="activity">Incoming request from Bot Framework.</param>
        /// <param name="connectorClient">Connector client instance for posting to Bot Framework.</param>
        /// <returns>Task tracking operation.</returns>

        private static async Task<HttpResponseMessage> HandleO365ConnectorCardActionQuery(Activity activity)
        {
            var connectorClient = new ConnectorClient(new Uri("https://smba.trafficmanager.net/amer-client-ss.msg/"));

            // Get O365 connector card query data.
            O365ConnectorCardActionQuery o365CardQuery = activity.GetO365ConnectorCardActionQueryData();

            Activity replyActivity = activity.CreateReply();

            replyActivity.TextFormat = "xml";

            replyActivity.Text = $@"

            <h2>Thanks, {activity.From.Name}</h2><br/>


            <h3>Your input action ID:</h3><br/>

            <pre>{o365CardQuery.ActionId}</pre><br/>

            <h3>Your input body:</h3><br/>

            <pre>{o365CardQuery.Body}</pre>

        ";

            await connectorClient.Conversations.ReplyToActivityWithRetriesAsync(replyActivity);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}