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