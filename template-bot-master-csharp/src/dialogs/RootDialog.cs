using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Root Dialog, its a triggring point for every Child dialog based on the RexEx Match with user input command
    /// </summary>

    [Serializable]
    public class RootDialog : DispatchDialog
    {
        #region Fetch Roster Api Payload Pattern

        [RegexPattern(DialogMatches.FetchRosterPayloadMatch)]
        [ScorableGroup(1)]
        public async Task FetchRosterPayLoadDetails(IDialogContext context, IActivity activity)
        {
            context.Call(new FetchRosterDialog(), this.EndDialog);
        }

        #endregion
        
        #region Fetch Roster Api Pattern

        [RegexPattern(DialogMatches.FetchRosterApiMatch)]
        [ScorableGroup(1)]
        public async Task FetchRoster(IDialogContext context, IActivity activity)
        {
            context.Call(new ListNamesDialog(), this.EndDialog);
        }

        #endregion

        #region Play Quiz

        [RegexPattern(DialogMatches.RunQuizQuestionsMatch)]
        [ScorableGroup(1)]
        public async Task RunQuiz(IDialogContext context, IActivity activity)
        {
            await this.SendWelcomeMessageQuizAsync(context,activity);
        }
        private async Task SendWelcomeMessageQuizAsync(IDialogContext context, IActivity activity)
        {
            await context.PostAsync(Strings.QuizTitleWelcomeMsg);
            context.Call(new QuizFullDialog(), this.EndDialog);
        }

        #endregion

        #region Prompt Flow Game Dialog Api Pattern

        [RegexPattern(DialogMatches.PromptFlowGameMatch)]
        [ScorableGroup(1)]
        public async Task FlowGame(IDialogContext context, IActivity activity)
        {
            context.Call(new PromptDialogExample(), this.ResumeAfterFlowGame);
        }

        public async Task ResumeAfterFlowGame(IDialogContext context, IAwaitable<bool> result)
        {
            if (result == null)
            {
                throw new InvalidOperationException((nameof(result)) + Strings.NullException);
            }

            var resultedValue = await result;

            if(Convert.ToBoolean(resultedValue))
            {
                await context.PostAsync(Strings.PlayGameThanksMsg);
            }
            else
            {
                await context.PostAsync(Strings.PlayGameFailMsg);
            }

            context.Done<object>(null);
        }

        #endregion

        #region Dialog Flow

        [RegexPattern(DialogMatches.DialogFlowMatch)]
        [ScorableGroup(1)]
        public async Task RunDialogFlow(IDialogContext context, IActivity activity)
        {
            await context.PostAsync(Strings.DialogFlowStep1);
            await this.SendStep1MsgAsync(context, activity);
        }

        private async Task SendStep1MsgAsync(IDialogContext context, IActivity activity)
        {
            await context.PostAsync(Strings.DialogFlowStep2);
            context.Call(new BeginDialogExampleDialog(), this.ResumeAfterDialogFlow);
        }

        public async Task ResumeAfterDialogFlow(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(Strings.DialogFlowStep3);
            context.Done<object>(null);
        }
        #endregion

        #region Hello Dialog

        [RegexPattern(DialogMatches.HelloDialogMatch1)]
        [RegexPattern(DialogMatches.HelloDialogMatch2)]
        [ScorableGroup(1)]
        public async Task RunHelloDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new HelloDialog(), this.EndDialog);
        }

        #endregion

        #region Run at Mention Api Pattern

        [RegexPattern(DialogMatches.AtMentionMatch1)]
        [RegexPattern(DialogMatches.AtMentionMatch2)]
        [RegexPattern(DialogMatches.AtMentionMatch3)]
        [ScorableGroup(1)]
        public async Task AtMentionMatchUser(IDialogContext context, IActivity activity)
        {
            context.Call(new AtMentionDialog(), this.EndDialog);
        }

        #endregion

        #region Multi Dialog1
        [RegexPattern(DialogMatches.MultiDialog1Match1)]
        [ScorableGroup(1)]
        public async Task MultiDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new MultiDialog1(), this.EndDialog);
        }

        #endregion

        #region Multi Dialog2
        [RegexPattern(DialogMatches.MultiDialog2Match)]
        [ScorableGroup(1)]
        public async Task MultiDialog2(IDialogContext context, IActivity activity)
        {
            context.Call(new MultiDialog2(), this.EndDialog);
        }

        #endregion

        #region Help Dialog

        [RegexPattern(DialogMatches.Help)]
        [ScorableGroup(1)]
        public async Task Help(IDialogContext context, IActivity activity)
        {
            await this.Default(context, activity);
        }

        [MethodBind]
        [ScorableGroup(2)]
        public async Task Default(IDialogContext context, IActivity activity)
        {
            context.Call(new HelpDialog(), this.EndDialog);
        }

        public async Task EndDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(null);
        }

        #endregion

        #region Fetch Last Exceuted Dialog

        [RegexPattern(DialogMatches.FecthLastExecutedDialogMatch)]
        [ScorableGroup(1)]
        public async Task FetchLastExecutedDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new GetLastDialogUsedDialog(), this.EndDialog);
        }

        #endregion

        #region Send 1:1 Bot Conversation

        [RegexPattern(DialogMatches.Send1to1Conversation)]
        [ScorableGroup(1)]
        public async Task SendOneToOneConversation(IDialogContext context, IActivity activity)
        {
            await context.PostAsync(Strings.Send1on1ConfirmMsg);
            context.Call(new ProactiveMsgTo1to1Dialog(), this.EndDialog);
        }

        #endregion

        #region Set Up Text Message

        [RegexPattern(DialogMatches.SetUpTextMsg)]
        [ScorableGroup(1)]
        public async Task SetUpTextMessage(IDialogContext context, IActivity activity)
        {
            context.Call(new UpdateTextMsgSetupDialog(), this.EndDialog);
        }

        #endregion

        #region Update Last Setup Text Message

        [RegexPattern(DialogMatches.UpdateLastSetupTextMsg)]
        [ScorableGroup(1)]
        public async Task UpdateLastSetUpTextMessage(IDialogContext context, IActivity activity)
        {
            context.Call(new UpdateTextMsgDialog(), this.EndDialog);
        }

        #endregion

        #region Set Up & Update Card

        [RegexPattern(DialogMatches.SetUpNUpdateCard)]
        [ScorableGroup(1)]
        public async Task SetUpNUpdateCardMessage(IDialogContext context, IActivity activity)
        {
            context.Call(new UpdateCardMsgSetupDialog(), this.EndDialog);
        }

        #endregion

        #region Load Different Types of Cards

        [RegexPattern(DialogMatches.DisplayCards)]
        [ScorableGroup(1)]
        public async Task DisplayCards(IDialogContext context, IActivity activity)
        {
            context.Call(new DisplayCardsDialog(), this.EndDialog);
        }

        [RegexPattern(DialogMatches.StopShowingCards)]
        [ScorableGroup(1)]
        public async Task LoadNone(IDialogContext context, IActivity activity)
        {
            await context.PostAsync(Strings.DisplayCardsThanksMsg);
        }

        #endregion

        #region MessageBack Dialog

        [RegexPattern(DialogMatches.MessageBack)]
        [ScorableGroup(1)]
        public async Task RunMessageBackDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new MessagebackDialog(), this.EndDialog);
        }

        #endregion

        #region LocalTime

        [RegexPattern(DialogMatches.LocalTime)]
        [ScorableGroup(1)]
        public async Task GetLocalTimeZone(IDialogContext context, IActivity activity)
        {
            await context.PostAsync(Strings.UTCTimeZonePrompt + activity.Timestamp);
            await context.PostAsync(Strings.LocalTimeZonePrompt + activity.LocalTimestamp);
        }

        #endregion

        #region Deeplink Dialog

        [RegexPattern(DialogMatches.DeepLinkTabCard)]
        [ScorableGroup(1)]
        public async Task DeeplinkDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new DeepLinkStaticTabDialog(), this.EndDialog);
        }

        #endregion

        #region Authentication Dialog

        [RegexPattern(DialogMatches.AuthSample)]
        [ScorableGroup(1)]
        public async Task AuthSample(IDialogContext context, IActivity activity)
        {
            var message = CreateAuthSampleMessage(context);
            await context.PostAsync(message);
        }

        #region Create Auth Message Card
        private IMessageActivity CreateAuthSampleMessage(IDialogContext context)
        {
            var message = context.MakeMessage();
            var attachment = CreateAuthSampleCard();
            message.Attachments.Add(attachment);
            return message;
        }

        private Attachment CreateAuthSampleCard()
        {
            return new HeroCard
            {
                Title = Strings.AuthSampleCardTitle,
                Buttons = new List<CardAction>
                {
                   new CardAction(ActionTypes.ImBack, Strings.FBAuthCardCaption, value: Strings.FBAuthCardValue),
                   new CardAction(ActionTypes.ImBack, Strings.VSTSAuthCardCaption, value: Strings.VSTSAuthCardValue)
                }
            }.ToAttachment();
        }
        #endregion

        #endregion

        #region Facebook Authentication Exmaple Dialog

        [RegexPattern(DialogMatches.Facebooklogin)]
        [ScorableGroup(1)]
        public async Task SimpleFacebookAuthLoginDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new SimpleFacebookAuthDialog(), this.EndDialog);
        }

        [RegexPattern(DialogMatches.Facebooklogout)]
        [ScorableGroup(1)]
        public async Task SimpleFacebookAuthLogoutDialog(IDialogContext context, IActivity activity)
        {
            context.PrivateConversationData.RemoveValue(SimpleFacebookAuthDialog.AuthTokenKey);
            context.PrivateConversationData.RemoveValue("persistedCookie");
            context.UserData.RemoveValue("name");
            await context.PostAsync(Strings.FBSuccessfulLogoutPrompt);
            await context.PostAsync(Strings.FBSuccessfulLogoutLoginPrompt);
        }

        #endregion

        #region VSTS Authentication Exmaple Dialog

        [RegexPattern(DialogMatches.VSTSlogin)]
        [ScorableGroup(1)]
        public async Task VSTSAuthLoginDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new VSTSAPICallDialog(), this.EndDialog);
        }

        [RegexPattern(DialogMatches.VSTSlogout)]
        [ScorableGroup(1)]
        public async Task VSTSAuthLogoutDialog(IDialogContext context, IActivity activity)
        {
            context.UserData.RemoveValue(VSTSAPICallDialog.VSTSAuthTokenKey);
            context.UserData.RemoveValue("persistedCookieVSTS");
            context.UserData.RemoveValue("name");
            await context.PostAsync(Strings.VSTSSuccessfulLogoutPrompt);
            await context.PostAsync(Strings.VSTSSuccessfulLogoutLoginPrompt);
        }

        #endregion

        #region VSTS Get Work Item Dialog

        [RegexPattern(DialogMatches.VSTSApi)]
        [ScorableGroup(1)]
        public async Task VSTSAuthGetWorkItemDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new VSTSGetworkItemDialog(), this.EndDialog);
        }

        #endregion

        #region Load Hero Card Type

        [RegexPattern(DialogMatches.HeroCard)]
        [ScorableGroup(1)]
        public async Task HeroCard(IDialogContext context, IActivity activity)
        {
            context.Call(new HeroCardDialog(), this.EndDialog);
        }

        #endregion

        #region Load Thumbnail Card Type

        [RegexPattern(DialogMatches.ThumbnailCard)]
        [ScorableGroup(1)]
        public async Task ThumbnailCard(IDialogContext context, IActivity activity)
        {
            context.Call(new ThumbnailcardDialog(), this.EndDialog);
        }

        #endregion

        #region Load Connector Card V1

        [RegexPattern(DialogMatches.ConnectorCardV1)]
        [ScorableGroup(1)]
        public async Task O365ConnectorCardV1(IDialogContext context, IActivity activity)
        {
            context.Call(new ConnectorCardV1Dialog(), this.EndDialog);
        }

        #endregion

        #region Load Connector Card V2

        [RegexPattern(DialogMatches.ConnectorCardV2)]
        [ScorableGroup(1)]
        public async Task O365ConnectorCardV2(IDialogContext context, IActivity activity)
        {
            context.Call(new ConnectorCardV2Dialog(), this.EndDialog);
        }

        #endregion

        #region Load Connector Card V3

        [RegexPattern(DialogMatches.ConnectorCardV3)]
        [ScorableGroup(1)]
        public async Task O365ConnectorCardV3(IDialogContext context, IActivity activity)
        {
            context.Call(new ConnectorCardV3Dialog(), this.EndDialog);
        }

        #endregion

        #region Load Actionable Message Card

        [RegexPattern(DialogMatches.ActionableCard)]
        [ScorableGroup(1)]
        public async Task O365ConnectorCardActionableMessage(IDialogContext context, IActivity activity)
        {
            context.Call(new ActionableMessageCardDialog(), this.EndDialog);
        }

        #endregion

        #region Load Actionable Message Card V2

        [RegexPattern(DialogMatches.ActionableCardV2)]
        [ScorableGroup(1)]
        public async Task O365ConnectorCardActionableMessageV2(IDialogContext context, IActivity activity)
        {
            context.Call(new ActionableMessageCardDialogV2(), this.EndDialog);
        }

        #endregion

        #region PopUp SignIn

        [RegexPattern(DialogMatches.PopUpSignIn)]
        [ScorableGroup(1)]
        public async Task PopUpSignIn(IDialogContext context, IActivity activity)
        {
            context.Call(new PopupSigninCardDialog(), this.EndDialog);
        }

        #endregion
    }
}