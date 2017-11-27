using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Microsoft.Teams.TemplateBotCSharp.Utility
{
    /// <summary>
    /// This method is used to Get the local from incoming acitvity payload
    /// </summary>
    public static class TemplateUtility
    {
        public static string GetLocale(Activity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            //Get the local from activity
            if (activity.Entities != null)
            {
                foreach(var entity in activity.Entities)
                {
                    if (string.Equals(entity.Type.ToString().ToLower(), "clientinfo"))
                    {
                        var locale = entity.Properties["locale"];
                        if (locale != null)
                        {
                            return locale.ToString();
                        }
                    }
                }
            }
            return activity.Locale;
        }

        public static ComposeExtensionAttachment CreateComposeExtensionCardsAttachments(WikiHelperSearchResult wikiResult, string state)
        {
            return GetComposeExtensionMainResultAttachment(wikiResult, state).ToComposeExtensionAttachment(GetComposeExtensionPreviewAttachment(wikiResult, state));
        }

        public static Attachment GetComposeExtensionMainResultAttachment(WikiHelperSearchResult wikiResult, string state)
        {
            var tapAction = new CardAction("invoke", value: "{ \"" + "imageUrl" + "\": \"" + wikiResult.imageUrl + "\", \"" + "highlightedTitle" + "\": \"" + wikiResult.highlightedTitle + "\"}");

            if (string.Equals(state.ToLower(), "hero"))
            {
                return new HeroCard()
                {
                    Title = wikiResult.highlightedTitle,
                    Tap = tapAction,
                    Text = wikiResult.text,
                    Images =
                    {
                        new CardImage(wikiResult.imageUrl)
                    },
                }.ToAttachment();
            }
            else
            {
                return new ThumbnailCard()
                {
                    Title = wikiResult.highlightedTitle,
                    Tap = tapAction,
                    Text = wikiResult.text,
                    Images =
                    {
                        new CardImage(wikiResult.imageUrl)
                    },
                }.ToAttachment();
            }
        }

        public static Attachment GetComposeExtensionPreviewAttachment(WikiHelperSearchResult wikiResult, string state)
        {
            string quoted = cleanForJSON(wikiResult.text);
            var tapAction = new CardAction("invoke", value: "{ \"" + "imageUrl" + "\": \"" + wikiResult.imageUrl + "\",\"" + "text" + "\": \"" + quoted + "\", \"" + "highlightedTitle" + "\": \"" + wikiResult.highlightedTitle + "\"}");

            if (string.Equals(state.ToLower(), "hero"))
            {
                return new HeroCard()
                {
                    Title = wikiResult.highlightedTitle,
                    Tap = tapAction,
                    Images =
                    {
                        new CardImage(wikiResult.imageUrl)
                    },
                }.ToAttachment();
            }
            else
            {
                return new ThumbnailCard()
                {
                    Title = wikiResult.highlightedTitle,
                    Tap = tapAction,
                    Images =
                    {
                        new CardImage(wikiResult.imageUrl)
                    },
                }.ToAttachment();
            }
        }

        /// <summary>
        /// Purpose of this method is to parse the invoke request json and returned the invoke value
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string ParseInvokeRequestJson(string inputString)
        {
            JObject invokeObjects = JObject.Parse(inputString);

            if (invokeObjects.Count > 0)
            {
                return invokeObjects[Strings.InvokeRequestJsonKey].Value<string>();
            }

            return null;
        }

        public static string cleanForJSON(string s)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        public static BotData GetBotDataObject(Activity activity)
        {
            StateClient stateClient = activity.GetStateClient();
            return stateClient.BotState.GetUserData(activity.ChannelId, activity.From.Id);
        }
    }
}