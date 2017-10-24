using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace Microsoft.Teams.Tutorial.CSharp
{
    /// <summary>
    /// This is Begin Dialog Class. Main purpose of this class is to notify users that Child dialog has been called 
    /// and its a Basic example to call Child dialog from Root Dialog.
    /// </summary>

    [Serializable]
    public class ThumbnailCardDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var message = context.MakeMessage();
            var attachment = GetThumbnailCard();

            message.Attachments.Add(attachment);

            await context.PostAsync((message));

            context.Done<object>(null);
        }

        private static Attachment GetThumbnailCard()
        {
            var baseUri = ConfigurationManager.AppSettings["BaseUri"].ToString();
            var thumbnailCard = new ThumbnailCard
            {
                Title = Strings.ThumbnailCardTitle,
                Subtitle = Strings.ThumbnailCardSubTitle,
                Text = Strings.ThumbnailCardTextMsg,
                Images = new List<CardImage> { new CardImage(baseUri + "/public/assets/computer.jpg") },
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.OpenUrl, Strings.ThumbnailCardButtonCaption, value: "https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-add-rich-card-attachments"),
                    new CardAction(ActionTypes.MessageBack, Strings.MessageBackCardButtonCaption, value: "{\"" + Strings.cmdValueMessageBack + "\": \"" + Strings.cmdValueMessageBack+ "\"}", text:"hello", displayText:"I clicked the button to send 'hello'")
                }
            };

            return thumbnailCard.ToAttachment();
        }
    }
}