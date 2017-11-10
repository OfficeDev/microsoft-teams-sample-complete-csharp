using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Connector Card V2 Dialog Class. Main purpose of this class is to display the Connector Card moderate information example
    /// </summary>

    [Serializable]
    public class ConnectorCardV2Dialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogConnectorCardV2Dialog);

            var message = context.MakeMessage();
            var attachment = GetO365ConnectorCardV2();
            message.Attachments.Add(attachment);

            await context.PostAsync((message));

            context.Done<object>(null);
        }

        public static Attachment GetO365ConnectorCardV2()
        {
            var section = new O365ConnectorCardSection
            {
                Title = Strings.O365V2Title,
                ActivityTitle = Strings.O365V2ActivityTitle,
                Facts = new List<O365ConnectorCardFact>
                {
                    new O365ConnectorCardFact(Strings.O365V2Fact1Key,Strings.O365V2Fact1Value),
                    new O365ConnectorCardFact(Strings.O365V2Fact2Key,Strings.O365V2Fact2Value),
                    new O365ConnectorCardFact(Strings.O365V2Fact3Key,Strings.O365V2Fact3Value),
                    new O365ConnectorCardFact(Strings.O365V2Fact4Key,Strings.O365V2Fact4Value)
                }
            };

            var o365connector = new O365ConnectorCard
            {
                ThemeColor = Strings.O365V2themecolor,
                Sections = new List<O365ConnectorCardSection> { section },
            };

            return o365connector.ToAttachment();
        }
    }
}