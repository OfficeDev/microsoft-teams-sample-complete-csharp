using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Teams.TemplateBotCSharp.Properties;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Fetch Roster Dialog Class. Main purpose of this dialog class is to Call the Roster Api and Post the 
    /// members information (Name and Id) in Teams. This Dialog is using Thumbnail Card to show the member information in teams.
    /// </summary>
    [Serializable]
    public class ProactiveMsgTo1to1Dialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogSend1on1Dialog);

            var userId = context.Activity.From.Id;
            var botId = context.Activity.Recipient.Id;
            var botName = context.Activity.Recipient.Name;

            string tenantId = null;
            string channelId = null;

            if (context.Activity.ChannelData != null)
            {
                if (context.Activity.ChannelData["tenant"] != null)
                {
                    tenantId = context.Activity.ChannelData["tenant"]["id"];
                }
                channelId = context.Activity.ChannelData["teamsChannelId"];
            }

            var connectorClient = new ConnectorClient(new Uri(context.Activity.ServiceUrl));
            var parameters = new ConversationParameters
            {
                Bot = new ChannelAccount(botId, botName),
                Members = new ChannelAccount[] { new ChannelAccount(userId) },
                ChannelData = new ChannelData { Tenant = new Tenant { tenantId = tenantId } }
            };
            try
            {
                var conversationResource = await connectorClient.Conversations.CreateConversationAsync(parameters);
                IMessageActivity message = null;

                if (conversationResource != null)
                {
                    message = Activity.CreateMessageActivity();
                    message.From = new ChannelAccount(botId, botName);
                    message.Conversation = new ConversationAccount(id: conversationResource.Id.ToString());
                    message.Text = Strings.Send1on1Prompt;
                }

                await connectorClient.Conversations.SendToConversationAsync((Activity)message);
            }
            catch (Exception ex)
            {

            }

            context.Done<object>(null);
        }
    }

    public class ChannelData
    {
        public string teamsChannelId { get; set; }
        public string TeamsTeamId { get; set; }
        public Tenant Tenant { get; set; }
    }

    public class Tenant
    {
        public string tenantId { get; set; }
    }

}