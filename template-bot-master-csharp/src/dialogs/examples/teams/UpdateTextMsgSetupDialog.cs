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
    public class UpdateTextMsgSetupDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            IMessageActivity reply = context.MakeMessage();
            reply.Text = Strings.SetupMessagePrompt;

            ConnectorClient client = new ConnectorClient(new Uri(context.Activity.ServiceUrl));
            ResourceResponse resp = await client.Conversations.ReplyToActivityAsync((Activity)reply);

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogSetupMessasge);

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.SetUpMsgKey, resp.Id.ToString());

            context.Done<object>(null);
        }
    }
}