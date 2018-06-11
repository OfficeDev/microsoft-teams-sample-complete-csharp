using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using System;
using System.Threading.Tasks;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Fetch Teams Info Dialog Class main purpose of this dialog class is to display Team Name, TeamId and AAD GroupId.
    /// </summary>
    [Serializable]
    public class FetchTeamsInfoDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var connectorClient = new ConnectorClient(new Uri(context.Activity.ServiceUrl));

            if (context.Activity.GetChannelData<TeamsChannelData>().Team == null)
            {
                // Handle for 1 to 1 bot conversation
                await context.PostAsync(Strings.AADId1To1ConversationError);
            }
            else
            {
                // Handle for channel conversation, aad group id only exists within channel
                TeamDetails teamDetails = await connectorClient.GetTeamsConnectorClient().Teams.FetchTeamDetailsAsync(context.Activity.GetChannelData<TeamsChannelData>().Team.Id);

                var message = context.MakeMessage();
                message.Text = GenerateTable(teamDetails);

                await context.PostAsync(message);
            }

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogFetchTeamInfoDialog);

            context.Done<object>(null);
        }

        /// <summary>
        /// Generate HTML dynamically to show TeamId, TeamName and AAD GroupId in table format 
        /// </summary>
        /// <param name="teamDetails"></param>
        /// <returns></returns>
        private string GenerateTable(TeamDetails teamDetails)
        {
            if (teamDetails == null)
            {
                return string.Empty;
            }

            string tableHtml = "<html><table border='1'><tbody><tr style='font-weight:bold'><td> TeamId </td><td> Team Name </td><td> AAD Group Id </td><tr>";

            tableHtml += "<tr><td>" + teamDetails.Id + "</td><td>" + teamDetails.Name + "</td><td>" + teamDetails.AadGroupId + "</td></tr>";

            tableHtml += "</tbody></table></html>";

            return tableHtml;
        }
    }
}