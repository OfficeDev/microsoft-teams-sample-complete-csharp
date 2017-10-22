using System;
using Microsoft.Bot.Connector;

namespace Microsoft.Teams.Tutorial.CSharp
{
    public static class StripBotAtMentions
    {
        public static IMessageActivity StripAtMentionText(IMessageActivity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            Mention[] m = activity.GetMentions();
            for (int i = 0; i < m.Length; i++)
            {
                if (m[i].Mentioned.Id == activity.Recipient.Id)
                {
                    //Bot is in the @mention list.  
                    //The below example will strip the bot name out of the message, so you can parse it as if it wasn't included.
                    // Note that the Text object will contain the full bot name, if applicable.
                    if (m[i].Text != null)
                        activity.Text = activity.Text.Replace(m[i].Text, "").Trim();
                }
            }

            //Convert input command in lower case for 1To1 and Channel users
            if (activity.Text != null)
            {
                activity.Text = activity.Text.ToLower();
            }

            return activity;
        }
    }
}