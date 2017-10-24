using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Microsoft.Teams.Tutorial.CSharp
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

        public static Attachment GetChoiceOptionCard()
        {
            return new HeroCard(Strings.DisplayCardsPromptChoiceTitle)
            {
                Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, Strings.OptionYes, value: Strings.cmdDisplayCards),
                        new CardAction(ActionTypes.ImBack, Strings.OptionNo, value: Strings.cmdNoDisplayCards)
                    }
            }.ToAttachment();
        }

        public static ComposeExtensionAttachment CreateComposeExtensionCardsAttachments(string title,string text,string imageUrl, string state)
        {
            return GetComposeExtensionMainResultAttachment(title, text, imageUrl, state).ToComposeExtensionAttachment(GetComposeExtensionPreviewAttachment(title, text, imageUrl, state));
        }

        public static Attachment GetComposeExtensionMainResultAttachment(string title,string text,string imageUrl, string state)
        {
            if (string.Equals(state.ToLower(), "hero"))
            {
                return new HeroCard()
                {
                    Title = title,
                    Text = text,
                    Images =
                    {
                        new CardImage(imageUrl)
                    },
                }.ToAttachment();
            }
            else
            {
                return new ThumbnailCard()
                {
                    Title = title,
                    Text= text,
                    Images =
                    {
                        new CardImage(imageUrl)
                    },
                }.ToAttachment();
            }
        }

        public static Attachment GetComposeExtensionPreviewAttachment(string title,string text, string imageUrl, string state)
        {
            if (string.Equals(state.ToLower(), "hero"))
            {
                return new HeroCard()
                {
                    Title = title,
                    Images =
                    {
                        new CardImage(imageUrl)
                    },
                }.ToAttachment();
            }
            else
            {
                return new ThumbnailCard()
                {
                    Title = title,
                    Images =
                    {
                        new CardImage(imageUrl)
                    },
                }.ToAttachment();
            }
        }

        public static string ParseJson(string inputString)
        {
            JObject invokeObjects = JObject.Parse(inputString);
            if (invokeObjects.Count > 0)
            {
                foreach (var item in invokeObjects)
                {
                   return Convert.ToString(item.Value);
                }
            }

            return null;
        }
    }
}