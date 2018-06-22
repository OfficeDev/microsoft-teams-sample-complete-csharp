using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Update Card Dialog Class. Main purpose of this class is to Setup the Card and Update the Card in Bot example
    /// </summary>
    [Serializable]
    public class UpdateCardMsgSetupDialog : IDialog<object>
    {
        public static int updateCounter = 1;
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var message = SetupMessage(context);

            ConnectorClient client = new ConnectorClient(new Uri(context.Activity.ServiceUrl));
            ResourceResponse resp = await client.Conversations.ReplyToActivityAsync((Activity)message);

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogSetupUpdateCard);

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if (result == null)
            {
                throw new InvalidOperationException((nameof(result)) + Strings.NullException);
            }

            var activity = await result;

            if (!string.IsNullOrEmpty(context.Activity.ReplyToId))
            {
                var updatedMessage = UpdateMessage(context);

                ConnectorClient client = new ConnectorClient(new Uri(context.Activity.ServiceUrl));
                ResourceResponse resp = await client.Conversations.UpdateActivityAsync(context.Activity.Conversation.Id, context.Activity.ReplyToId, (Activity)updatedMessage);

                await context.PostAsync(Strings.UpdateCardMessageConfirmation);
            }
            else
            {
                await context.PostAsync(Strings.ErrorTextMessageUpdate);
            }

            context.Done<object>(null);
        }

        #region Create Message to Setup Card
        private IMessageActivity SetupMessage(IDialogContext context)
        {
            var message = context.MakeMessage();
            var attachment = CreateCard();
            message.Attachments.Add(attachment);
            return message;
        }

        private Attachment CreateCard()
        {
            return new HeroCard
            {
                Title = Strings.UpdateCardTitle,
                Subtitle = Strings.UpdateCardSubTitle,
                Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                Buttons = new List<CardAction>
                {
                   new CardAction(ActionTypes.MessageBack, Strings.UpdateCardButtonCaption, value: "Update")
                }
            }.ToAttachment();
        }
        #endregion

        #region Create Updated Card Message
        private IMessageActivity UpdateMessage(IDialogContext context)
        {
            var message = context.MakeMessage();
            var attachment = UpdateCard();
            message.Attachments.Add(attachment);
            return message;
        }

        private Attachment UpdateCard()
        {
            return new HeroCard
            {
                Title = Strings.UpdatedCardTitle,
                Subtitle = Strings.UpdatedCardSubTitle,
                Images = new List<CardImage> { new CardImage(ConfigurationManager.AppSettings["BaseUri"].ToString() + "/public/assets/computer_person.jpg") },
                Buttons = new List<CardAction>
                {
                   new CardAction(ActionTypes.OpenUrl, Strings.UpdatedCardButtonCaption, value: "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bot-conversations/bots-conversations#updating-messages"),
                }
            }.ToAttachment();
        }
        #endregion
    }
}