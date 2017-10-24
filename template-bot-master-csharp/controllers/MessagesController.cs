using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Diagnostics;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using System.Collections.Generic;
using Microsoft.Teams.TemplateBotCSharp.Properties;

namespace Microsoft.Teams.Tutorial.CSharp
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
            // Confirmation check - if activity is null - do nothing
            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            if (activity.Type == ActivityTypes.Message)
            {
                // This is used for removing the '@botName' from the incoming message so it can
                // be parsed correctly
                var messageActivity = StripBotAtMentions.StripAtMentionText(activity);

                try
                {
                    // This sends all messages to the bot/dialogs/RootDialog.cs for parsing
                    await Conversation.SendAsync(messageActivity, () => new RootDialog());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            else if (activity.Type == ActivityTypes.Invoke) // Received an invoke
            {
                if (activity.IsO365ConnectorCardActionQuery())
                {
                    return await HandleO365ConnectorCardActionQuery(activity);
                }
                else if (activity.IsComposeExtensionQuery())
                {
                    if (activity.Name == "composeExtension/query")
                    {
                        var invokeResponse = this.GetComposeExtensionResponse(activity);
                        return Request.CreateResponse<ComposeExtensionResponse>(HttpStatusCode.OK, invokeResponse);
                    }
                    else if (activity.Name == "composeExtension/querySettingUrl")
                    {
                        var invokeConfigResponse = WikiHelper.GetConfig();
                        return Request.CreateResponse<ComposeExtensionResponse>(HttpStatusCode.OK, invokeConfigResponse);
                    }
                    else if (activity.Name == "composeExtension/setting")
                    {
                        var composeExtensionQuery = activity.GetComposeExtensionQueryData();
                        if (!string.IsNullOrEmpty(composeExtensionQuery.State))
                        {
                            StateClient stateClient = activity.GetStateClient();
                            BotData userData = stateClient.BotState.GetUserData(activity.ChannelId, activity.From.Id);
                            WikiHelper.ParseSettingsAndSave(composeExtensionQuery.State, userData, stateClient, activity);
                            return Request.CreateResponse<ComposeExtensionResponse>(HttpStatusCode.OK, null);
                        }
                    }
                    else
                    {
                        // Unknown compose extension type
                        return Request.CreateResponse<ComposeExtensionResponse>(HttpStatusCode.OK, null);
                    }
                }
            }
            else
            {
                // This is used to handle many other (some unsupported) types of messages
                HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
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

        private ComposeExtensionResponse GetComposeExtensionResponse(Activity activity)
        {
            StateClient stateClient = activity.GetStateClient();
            BotData userData = stateClient.BotState.GetUserData(activity.ChannelId, activity.From.Id);
            var composeExtensionQuery = activity.GetComposeExtensionQueryData();

            if (composeExtensionQuery.CommandId == null || composeExtensionQuery.Parameters == null)
            {
                return null;
            }

            var initialRunParameter = WikiHelper.GetQueryParameterByName(composeExtensionQuery, Strings.manifestInitialRun);
            var queryParameter = WikiHelper.GetQueryParameterByName(composeExtensionQuery, Strings.manifestParameterName);

            if (userData == null)
            {
                var composeExtensionResponse = new ComposeExtensionResponse();
                string message = Strings.ComposeExtensionNoUserData;
                composeExtensionResponse.ComposeExtension = WikiHelper.GetMessageResponseResult(message);
                return composeExtensionResponse;
            }

            /**
            * Below are the checks for various states that may occur
            * Note that the order of many of these blocks of code is important
            */

            // Situation where the incoming payload was received from the config popup

            if (!string.IsNullOrEmpty(composeExtensionQuery.State))
            {
                WikiHelper.ParseSettingsAndSave(composeExtensionQuery.State, userData, stateClient, activity);

                // Need to keep going to return a response so do not return here

                queryParameter = "";
                initialRunParameter = "true";
            }

            // This is a sitaution where the user's preferences have not been set up yet
            if (string.IsNullOrEmpty(userData.GetProperty<string>(Strings.ComposeExtensionCardTypeKeyword)))
            {
                var composeExtensionResponse = WikiHelper.GetConfig();
                return composeExtensionResponse;
            }

            // This is the situation where the user has entered the word 'reset' and wants
            // to clear his/her settings
            // resetKeyword for English is "reset"
            // **************** NOTE!!!!!!! - it is not recommended that you have a reset
            // keyword like this in your real app - this is simply meant for testing purposes

            if (string.Equals(queryParameter.ToLower(), Strings.ComposeExtensionResetKeyword))
            {
                // Reset the user's state
                userData.SetProperty<string>("composeExtensionCardType", null);
                stateClient.BotState.SetUserData(activity.ChannelId, activity.From.Id, userData);
                var composeExtensionResponse = new ComposeExtensionResponse();
                composeExtensionResponse.ComposeExtension = WikiHelper.GetMessageResponseResult(Strings.ComposeExtensionResetText);
                return composeExtensionResponse;
            }


            // This is the situation where the user in on the initial run of the compose extension
            // e.g. when the user first goes to the compose extension and the search bar is still blank
            // in order to get the compose extension to run the initial run, the setting "initialRun": true
            // must be set in the manifest for the compose extension

            if (initialRunParameter == "true")
            {
                // Signin Experience, uncomment below code for Signin Experience
                // var composeExtensionResponse = WikiHelper.GetSignin(composeExtensionResponse);
                // return composeExtensionResponse;

                var composeExtensionResponse = new ComposeExtensionResponse();
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
            List<ComposeExtensionAttachment> composeExtensionAttachments = new List<ComposeExtensionAttachment>();

            // enumerate search results and build Promises for cards for response
            foreach (var searchResult in wikiResult.query.search)
            {
                //Get the Image result on the basis of Image Title one by one
                ImageResult imageResult = WikiHelper.SearchWikiImage(searchResult);

                //Get the Image Url from imageResult
                string imageUrl = WikiHelper.GetImageURL(imageResult);

                //Set the Highlighter title
                string highlightedTitle = WikiHelper.GetHighLightedTitle(searchResult.title, queryParameter);

                string cardText = searchResult.snippet + " ...";

                // create the card itself and the preview card based upon the information
                // check user preference for which type of card to create

                composeExtensionAttachments.Add(TemplateUtility.CreateComposeExtensionCardsAttachments(highlightedTitle, cardText, imageUrl, userData.GetProperty<string>(Strings.ComposeExtensionCardTypeKeyword)));
            }

            var wikiComposeExtensionResponse = WikiHelper.GetComposeExtenionQueryResult(composeExtensionAttachments);

            return wikiComposeExtensionResponse;
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
    }
}