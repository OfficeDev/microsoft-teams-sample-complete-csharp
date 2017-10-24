using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Microsoft.Bot.Connector.Teams.Models;

namespace Microsoft.Teams.Tutorial.CSharp
{
    /// <summary>
    /// This is Fetch Roster Payload Dialog Class. Main purpose of this dialog class is to Call the Roster Api and Post the 
    /// full JSON Payload in Teams returned by Roster Api.
    /// </summary>
    [Serializable]
    public class FetchRosterDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var connectorClient = new ConnectorClient(new Uri(context.Activity.ServiceUrl));
            var channelData  = context.Activity.GetChannelData<TeamsChannelData>();
            string output = "";
            if (channelData?.Team?.Id != null)
            {
                // This means we are in a team - use team.id to get roster
                var response = await connectorClient.Conversations.GetConversationMembersAsync(channelData.Team.Id);
                output = JsonConvert.SerializeObject(response);
            }
            else
            {
                // This means we are in a 1:1 chat - use the conversation.id to get roster
                var response = await connectorClient.Conversations.GetConversationMembersAsync(context.Activity.Conversation.Id);
                output = JsonConvert.SerializeObject(response);
            }

            var message = context.MakeMessage();
            message.Text = output;

            await context.PostAsync(message);

            context.Done<object>(null);
        }
    }
}