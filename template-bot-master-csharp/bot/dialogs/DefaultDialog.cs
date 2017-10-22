using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace Microsoft.Teams.Tutorial.CSharp
{
    [Serializable]
    public class DefaultDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Send message
            await context.PostAsync("I'm sorry, but I didn't understand.");

            // Return back to the RootDialog
            context.Done<object>(null);
        }
    }
}