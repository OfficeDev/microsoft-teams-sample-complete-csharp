using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace Microsoft.Teams.Tutorial.CSharp
{
    [Serializable]
    public class HelloDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            // Send the message 'Hello!'
            await context.PostAsync("Hello!");

            // Return back to the RootDialog
            context.Done<object>(null);
        }
    }
}