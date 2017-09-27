using System;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using Microsoft.Teams.TemplateBotCSharp.Properties;

namespace Microsoft.Teams.TemplateBotCSharp.Utility
{
    public static class ParseInvokeActivityRequest
    {
        public static IMessageActivity ParseInvokeRequest(IMessageActivity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            activity.Text = TemplateUtility.ParseJson(activity.Value.ToString());

            //Change the Type of Activity to work in exisiting Root Dialog Architecture
            activity.Type = Strings.MessageActivity;

            return activity;
        }

        public static IMessageActivity ParseO365ConnectorCardInvokeRequest(IMessageActivity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            activity.Text = "actionablecard";

            //Change the Type of Activity to work in exisiting Root Dialog Architecture
            activity.Type = Strings.MessageActivity;

            return activity;
        }
    }
}