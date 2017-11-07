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