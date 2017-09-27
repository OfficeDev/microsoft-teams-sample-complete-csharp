using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Teams.TemplateBotCSharp.Properties;
namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Begin Dialog Class. Main purpose of this class is to notify users that Child dialog has been called 
    /// and its a Basic example to call Child dialog from Root Dialog.
    /// </summary>

    [Serializable]
    public class GetLastDialogUsedDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string dialogName = string.Empty;

            if (context.UserData.TryGetValue(Strings.LastDialogKey, out dialogName))
            {
                await context.PostAsync(Strings.LastDialogPromptMsg + dialogName);
            }
            else
            {
                //Set the Last Dialog in Conversation Data
                context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogFetchDiaog);

                await context.PostAsync(Strings.LastDialogErrorMsg);
            }

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogFetchDiaog);

            context.Done<object>(null);
        }
    }
}