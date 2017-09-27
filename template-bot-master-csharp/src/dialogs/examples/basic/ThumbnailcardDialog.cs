using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using Microsoft.Teams.TemplateBotCSharp.Utility;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Begin Dialog Class. Main purpose of this class is to notify users that Child dialog has been called 
    /// and its a Basic example to call Child dialog from Root Dialog.
    /// </summary>

    [Serializable]
    public class ThumbnailcardDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogThumbnailCard);

            var message = context.MakeMessage();
            var attachment = GetThumbnailCard();

            message.Attachments.Add(attachment);
            message.Attachments.Add(TemplateUtility.GetChoiceOptionCard());

            await context.PostAsync((message));

            context.Done<object>(null);
        }

        private static Attachment GetThumbnailCard()
        {
            var thumbnailCard = new ThumbnailCard
            {
                Title = Strings.ThumbnailCardTitle,
                Subtitle = Strings.ThumbnailCardSubTitle,
                Text = Strings.ThumbnailCardTextMsg,
                Images = new List<CardImage> { new CardImage(Strings.ThumbnailCardImageUrl) },
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.OpenUrl, Strings.ThumbnailCardButtonCaption, value: "https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-add-rich-card-attachments"),
                    new CardAction(ActionTypes.MessageBack, Strings.MessageBackCardButtonCaption, value: "{\"" + Strings.cmdValueMessageBack + "\": \"" + Strings.cmdValueMessageBack+ "\"}", text:Strings.cmdValueMessageBack, displayText:Strings.MessageBackDisplayedText)
                }
            };

            return thumbnailCard.ToAttachment();
        }
    }
}