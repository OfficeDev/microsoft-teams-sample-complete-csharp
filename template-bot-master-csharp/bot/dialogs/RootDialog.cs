using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using System.Threading;
using System.Collections.Generic;

namespace Microsoft.Teams.Tutorial.CSharp
{
    [Serializable]
    public class RootDialog : DispatchDialog
    {
        [RegexPattern("hello")]
        [RegexPattern("hi")]
        [ScorableGroup(1)]
        public async Task RunHelloDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new HelloDialog(), this.EndDialog);
        }

        [RegexPattern("thumbnail card")]
        [ScorableGroup(1)]
        public async Task RunThumbnailCardDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new ThumbnailCardDialog(), this.EndDialog);
        }

        [RegexPattern("hero card")]
        [ScorableGroup(1)]
        public async Task RunHeroCardDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new HeroCardDialog(), this.EndDialog);
        }

        [RegexPattern("connector card")]
        [ScorableGroup(1)]
        public async Task RunO365ConnectorCard1Dialog(IDialogContext context, IActivity activity)
        {
            context.Call(new O365ConnectorCard1Dialog(), this.EndDialog);
        }

        [RegexPattern("connector card 2")]
        [ScorableGroup(1)]
        public async Task RunO365ConnectorCard2Dialog(IDialogContext context, IActivity activity)
        {
            context.Call(new O365ConnectorCard2Dialog(), this.EndDialog);
        }

        [RegexPattern("connector card actions")]
        [ScorableGroup(1)]
        public async Task RunO365ConnectorCardActionsDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new O365ConnectorCardActionsDialog(), this.EndDialog);
        }

        // Must be called in a channel
        [RegexPattern("at-mention")]
        [ScorableGroup(1)]
        public async Task RunAtMentionDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new AtMentionDialog(), this.EndDialog);
        }

        [RegexPattern("send message to 1:1")]
        [ScorableGroup(1)]
        public async Task RunProactiveMsgTo1to1Dialog(IDialogContext context, IActivity activity)
        {
            context.Call(new ProactiveMsgTo1to1Dialog(), this.EndDialog);
        }

        [RegexPattern("fetch roster")]
        [ScorableGroup(1)]
        public async Task RunFetchRosterDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new FetchRosterDialog(), this.EndDialog);
        }

        [RegexPattern("setup text message")]
        [ScorableGroup(1)]
        public async Task RunUpdateTextMsgSetupDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new UpdateTextMsgSetupDialog(), this.EndDialog);
        }

        [RegexPattern("update text message")]
        [ScorableGroup(1)]
        public async Task RunUpdateTextMsgDialog(IDialogContext context, IActivity activity)
        {
            context.Call(new UpdateTextMsgDialog(), this.EndDialog);
        }

        [MethodBind]
        [ScorableGroup(2)]
        public async Task Default(IDialogContext context, IActivity activity)
        {
            context.Call(new DefaultDialog(), this.EndDialog);
        }

        public async Task EndDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(null);
        }
    }
}