using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System.Web.Http;

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
                 builder.Register(c => store)
                           .Keyed<IBotDataStore<BotData>>(FiberModule.Key_DoNotSerialize)
                           .AsSelf()
                           .SingleInstance();
             });
        }
    }
}
