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
    public class O365ConnectorCard2Dialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var message = context.MakeMessage();
            var attachment = GetO365ConnectorCardV2();
            message.Attachments.Add(attachment);

            await context.PostAsync((message)); 

            context.Done<object>(null);
        }

        private static Attachment GetO365ConnectorCardV2()
        {
            var section = new O365ConnectorCardSection
            {
                ActivityTitle = Strings.O365V3ActivityTitle,
                ActivitySubtitle = Strings.O365V3ActivitySubtitle,
                ActivityImage = Strings.O365V3ImageUrl,
                Facts = new List<O365ConnectorCardFact>
                {
                    new O365ConnectorCardFact(Strings.O365V3Fact1Key,Strings.O365V3Fact1Value),
                    new O365ConnectorCardFact(Strings.O365V3Fact2Key,Strings.O365V3Fact2Value),
                }
            };

            var o365connector = new O365ConnectorCard
            {
                ThemeColor = Strings.O365V3ThemeColor,
                Summary = Strings.O365V3Summary,
                Title = Strings.O365V3Title,
                Sections = new List<O365ConnectorCardSection> { section },
                Text = Strings.O365V3Text
            };

            return o365connector.ToAttachment();
        }
    }
}