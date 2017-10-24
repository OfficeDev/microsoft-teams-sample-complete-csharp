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