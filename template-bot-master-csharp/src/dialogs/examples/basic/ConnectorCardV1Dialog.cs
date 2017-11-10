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
    /// This is Connector Card Dialog Class. Main purpose of this class is to display the Connector Card basic example
    /// </summary>

    [Serializable]
    public class ConnectorCardV1Dialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogConnectorCardV1Dialog);

            var message = context.MakeMessage();
            var attachment = GetO365ConnectorCardV1Attachment();
            message.Attachments.Add(attachment);

            await context.PostAsync((message));

            context.Done<object>(null);
        }

        public static Attachment GetO365ConnectorCardV1Attachment()
        {
            var o365connector = new O365ConnectorCard
            {
                Title = Strings.O365V1Title,
                Sections = new List<O365ConnectorCardSection>
                {
                    new O365ConnectorCardSection{ Text= Strings.O365V1Section1 },
                    new O365ConnectorCardSection{ Text= Strings.O365V1Section2 }
                },
            };

            return o365connector.ToAttachment();
        }
    }
}