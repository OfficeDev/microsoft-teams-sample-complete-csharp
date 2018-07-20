using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;

namespace Microsoft.Teams.TemplateBotCSharp.Utility
{
    public static partial class Middleware
    {
        public static string TenantFilterSettingAny = "#ANY#";
        public static string AdaptiveCardActionKey = "dialog";

        /// <summary>
        /// Here are below scenarios - 
        ///     #Scenario 1 - Reject the Bot If Tenant is configured in web.config and doesn't match with Incoming request tenant
        ///     #Scenario 2 - Allow Bot for every Tenant if Tenant is not configured in web.config file and default value is #ANY#             
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="currentTenant"></param>
        /// <returns></returns>
        public static bool RejectMessageBasedOnTenant(IMessageActivity activity, string currentTenant)
        {
            if (!String.Equals(ConfigurationManager.AppSettings["OFFICE_365_TENANT_FILTER"], TenantFilterSettingAny))
            {
                //#Scenario 1
                return !string.Equals(ConfigurationManager.AppSettings["OFFICE_365_TENANT_FILTER"], currentTenant);
            }
            else
            {
                //Scenario 2
                return false;
            }
        }

        public static Activity ConvertActivityTextToLower(Activity activity)
        {
            //Convert input command in lower case for 1To1 and Channel users
            if (activity.Text != null)
            {
                activity.Text = activity.Text.ToLower();
            }

            return activity;
        }

        /// <summary>
        /// Set activity text to "adaptive card", if request is from an adaptive card
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static Activity SetSubmitActivityFromExampleCard(Activity activity)
        {
            // if activity text is blank, activity.ReplyToId is null and if defined key in adaptive card DataJson is present in incoming activity value 
            // to check if this is submit activity from an adaptive card then set activity text "adaptive card" to trigger Adaptive Card dialog, it's a work around that will be cleaned up later
            if (string.IsNullOrEmpty(activity.Text) && activity.ReplyToId != null && activity?.Value != null)
            {
                JObject jsonObject = (JObject)(activity.Value);
                JToken jtokenVal;

                if (jsonObject.Count > 0)
                {
                    if (jsonObject.TryGetValue(AdaptiveCardActionKey, out jtokenVal))
                    {
                        // set activity text "adaptive card"
                        activity.Text = DialogMatches.AdaptiveCard;
                    }
                }
            }

            return activity;
        }
    }
}