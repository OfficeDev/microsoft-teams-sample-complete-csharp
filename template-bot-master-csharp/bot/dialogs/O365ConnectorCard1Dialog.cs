using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Teams.Tutorial.CSharp
{
    /// <summary>
    /// This is Begin Dialog Class. Main purpose of this class is to notify users that Child dialog has been called 
    /// and its a Basic example to call Child dialog from Root Dialog.
    /// </summary>

    [Serializable]
    public class O365ConnectorCard1Dialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var message = context.MakeMessage();
            var attachment = this.GetO365ConnectorCardV1();
            message.Attachments.Add(attachment);

            await context.PostAsync((message)); 

            context.Done<object>(null);
        }

        private Attachment GetO365ConnectorCardV1()
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