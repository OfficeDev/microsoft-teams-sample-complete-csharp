using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Azure;
using System.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace Microsoft.Teams.TemplateBotCSharp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var store = new InMemoryDataStore();

            Conversation.UpdateContainer(
             builder =>
             {
                 //var store = new InMemoryDataStore();
                 builder.Register(c => store)
                           .Keyed<IBotDataStore<BotData>>(FiberModule.Key_DoNotSerialize)
                           .AsSelf()
                           .SingleInstance();
             });
        }
    }
}
