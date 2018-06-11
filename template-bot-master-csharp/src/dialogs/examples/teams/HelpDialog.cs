﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Help Dialog Class. Main purpose of this dialog class is to post the help commands in Teams.
    /// These are Actionable help commands for easy to use.
    /// </summary>
    [Serializable]
    public class HelpDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var message = context.MakeMessage();

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogHelpDialog);

            // This will create Interactive Card with help command buttons
            message.Attachments = new List<Attachment>
            {
                new HeroCard(Strings.HelpTitle)
                {
                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionRunQuiz, value: Strings.cmdRunQuiz),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionFetchRoster, value: Strings.cmdFetchRoster),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionPlayGame, value: Strings.cmdPrompt),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionRosterPayload, value: Strings.cmdRosterPayload),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionDialogFlow, value: Strings.cmdDialogFlow),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionHelloDialog, value: Strings.cmdHelloDialog),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionAtMention, value: Strings.cmdRunAtMentionDialog),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionMultiDialog1, value: Strings.cmdMultiDialog1),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionMultiDialog2, value: Strings.cmdMultiDialog2),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionFetchLastDialog, value: Strings.cmdFetchLastDialog),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionSetupMessage, value: Strings.cmdSetupMessage),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionUpdateMessage, value: Strings.cmdUpdateMessage),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionSend1on1Conversation, value: Strings.cmdSend1on1ConversationDialog),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionUpdateCard, value: Strings.cmdUpdateCard),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionDisplayCards, value: Strings.cmdDisplayCards),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionDeepLink, value: Strings.cmdDeepLink),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionAuthSample, value: Strings.cmdAuthSampleDialog),
                        new CardAction(ActionTypes.ImBack, Strings.HelpLocalTimeZone, value: Strings.cmdLocalTime),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionMessageBack, value: Strings.cmdMessageBack),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionPopUpSignIn, value: Strings.cmdPopUpSignIn),
                        new CardAction(ActionTypes.ImBack, Strings.HelpCaptionGetAADId, value: Strings.cmdGetAADId)
                    }
                }.ToAttachment()
            };

            await context.PostAsync(message);
            context.Done<object>(null);
        }
    }
}