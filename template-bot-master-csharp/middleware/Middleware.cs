using System;
using Microsoft.Bot.Connector;
using System.Configuration;
using Microsoft.Bot.Connector.Teams.Models;

namespace Microsoft.Teams.TemplateBotCSharp.Utility
{
    public static class Middleware
    {
        public static bool RestrictBotForTenant(IMessageActivity activity)
        {
            string targetTenant = ConfigurationManager.AppSettings["OFFICE_365_TENANT_FILTER"] != null ? ConfigurationManager.AppSettings["OFFICE_365_TENANT_FILTER"].ToString() : null;
            string currentTanent = (activity != null && activity.ChannelData != null && activity.ChannelData["tenant"] != null) ? Convert.ToString(activity.ChannelData["tenant"]["id"]) : null;

            if (string.Equals(targetTenant, currentTanent))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}