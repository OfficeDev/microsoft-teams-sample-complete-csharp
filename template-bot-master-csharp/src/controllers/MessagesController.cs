using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Microsoft.Teams.TemplateBotCSharp.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

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

                //Strip At mention from incoming request text
                var messageActivity = Middleware.StripAtMentionText(activity);

                //Convert incoming activity text to lower case, to matches the intent irrespective of incoming text case
                messageActivity = Middleware.ConvertActivityTextToLower(activity);

                //Set the OFFICE_365_TENANT_FILTER key in web.config file with Tenant Information
                //Validate bot for specific teams tenant if any
                if (Middleware.RejectBotBasedOnTenant(activity, activity.GetTenantId()))
                {
                    var connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl));

                    Activity replyActivity = activity.CreateReply();
                    replyActivity.Text = Strings.TenantLevelDeniedAccess;

                    await connectorClient.Conversations.ReplyToActivityAsync(replyActivity);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

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
                    // this will handle the compose extension request
                    var invokeResponse = WikipediaComposeExtension.GetComposeExtensionResponse(activity);
                    return Request.CreateResponse<ComposeExtensionResponse>(HttpStatusCode.OK, invokeResponse);
                }
                //Actionable Message
                else if (activity.IsO365ConnectorCardActionQuery())
                {
                    // this will handle the request coming any action on Actionable messages
                    return await HandleO365ConnectorCardActionQuery(activity);
                }
                //PopUp SignIn
                else if (activity.Name == "signin/verifyState")
                {
                    // this will handle the request coming from PopUp SignIn 
                    return await PopUpSignInHandler(activity);
                }
                // Handle rest of the invoke request
                else
                {
                    var messageActivity = (IMessageActivity)null;

                    //this will parse the invoke value and change the message activity as well
                    messageActivity = InvokeHandler.HandleInvokeRequest(activity);

                    await Conversation.SendAsync(messageActivity, () => new Dialogs.RootDialog());
                    
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
            var connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl));

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

        /// <summary>
        /// Purpose of this method is to handle the PopUp SignIn requests
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        private static async Task<HttpResponseMessage> PopUpSignInHandler(Activity activity)
        {
            var connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl));

            Activity replyActivity = activity.CreateReply();

            replyActivity.Text = $@"Authentication Successful";

            await connectorClient.Conversations.ReplyToActivityWithRetriesAsync(replyActivity);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}