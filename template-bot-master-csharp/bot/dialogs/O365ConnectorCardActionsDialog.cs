using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Teams.Tutorial.CSharp
{
    /// <summary>
    /// This is Begin Dialog Class. Main purpose of this class is to notify users that Child dialog has been called 
    /// and its a Basic example to call Child dialog from Root Dialog.
    /// </summary>

    [Serializable]
    public class O365ConnectorCardActionsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var message = context.MakeMessage();
            var attachment = GetO365ConnectorCardActions();
            message.Attachments.Add(attachment);

            await context.PostAsync((message)); 

            context.Done<object>(null);
        }

        /// <summary>
        /// Create a sample O365 connector card.
        /// </summary>
        /// <returns>The result card with actions.</returns>
        /// 
        private static Attachment GetO365ConnectorCardActions()
        {
            #region Multichoice Card
            var multichoiceCard = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Multiple Choice",
                "Multiple Choice Card",
                new List<O365ConnectorCardInputBase>
                {
                    new O365ConnectorCardMultichoiceInput(
                        O365ConnectorCardMultichoiceInput.Type,
                        "CardsType",
                        true,
                        "Pick multiple options",
                        null,
                        new List<O365ConnectorCardMultichoiceInputChoice>
                        {
                            new O365ConnectorCardMultichoiceInputChoice("Hero Card", "Hero Card"),
                            new O365ConnectorCardMultichoiceInputChoice("Thumbnail Card", "Thumbnail Card"),
                            new O365ConnectorCardMultichoiceInputChoice("O365 Connector Card", "O365 Connector Card")
                        },
                        "expanded",
                        true),
                    new O365ConnectorCardMultichoiceInput(
                        O365ConnectorCardMultichoiceInput.Type,
                        "Teams",
                        true,
                        "Pick multiple options",
                        null,
                        new List<O365ConnectorCardMultichoiceInputChoice>
                        {
                            new O365ConnectorCardMultichoiceInputChoice("Bot", "Bot"),
                            new O365ConnectorCardMultichoiceInputChoice("Tab", "Tab"),
                            new O365ConnectorCardMultichoiceInputChoice("Connector", "Connector"),
                            new O365ConnectorCardMultichoiceInputChoice("Compose Extension", "Compose Extension")
                        },
                        "compact",
                        true),
                    new O365ConnectorCardMultichoiceInput(
                        O365ConnectorCardMultichoiceInput.Type,
                        "Apps",
                        false,
                        "Pick an App",
                        null,
                        new List<O365ConnectorCardMultichoiceInputChoice>
                        {
                            new O365ConnectorCardMultichoiceInputChoice("VSTS", "VSTS"),
                            new O365ConnectorCardMultichoiceInputChoice("Wiki", "Wiki"),
                            new O365ConnectorCardMultichoiceInputChoice("Github", "Github")
                        },
                        "expanded",
                        false),
                    new O365ConnectorCardMultichoiceInput(
                        O365ConnectorCardMultichoiceInput.Type,
                        "OfficeProduct",
                        false,
                        "Pick an Office Product",
                        null,
                        new List<O365ConnectorCardMultichoiceInputChoice>
                        {
                            new O365ConnectorCardMultichoiceInputChoice("Outlook", "Outlook"),
                            new O365ConnectorCardMultichoiceInputChoice("MS Teams", "MS Teams"),
                            new O365ConnectorCardMultichoiceInputChoice("Skype", "Skype")
                        },
                        "compact",
                        false)
            },

            new List<O365ConnectorCardActionBase>
                  {
                   new O365ConnectorCardHttpPOST(
                        O365ConnectorCardHttpPOST.Type,
                        "Send",
                        "multichoice",
                        @"{""CardsType"":""{{CardsType.value}}"", ""Teams"":""{{Teams.value}}"", ""Apps"":""{{Apps.value}}"", ""OfficeProduct"":""{{OfficeProduct.value}}""}")
                 });

            #endregion

            #region Input Card
            var inputCard = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Text Input",
                "Input Card",
                new List<O365ConnectorCardInputBase>
                {
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-1",
                        false,
                        "multiline, no maxLength",
                        null,
                        true,
                        null),
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-2",
                        false,
                        "single line, no maxLength",
                        null,
                        false,
                        null),
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-3",
                        true,
                        "multiline, max len = 10, isRequired",
                        null,
                        true,
                        10),
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-4",
                        true,
                        "single line, max len = 10, isRequired",
                        null,
                        false,
                        10)
                },
                new List<O365ConnectorCardActionBase>
                {
                    new O365ConnectorCardHttpPOST(
                        O365ConnectorCardHttpPOST.Type,
                        "Send",
                        "inputText",
                        @"{""text1"":""{{text-1.value}}"", ""text2"":""{{text-2.value}}"", ""text3"":""{{text-3.value}}"", ""text4"":""{{text-4.value}}""}")
                });
            #endregion

            #region Date Card
            var dateCard = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Date Input",
                "Date Card",
                new List<O365ConnectorCardInputBase>
                {
                    new O365ConnectorCardDateInput(
                        O365ConnectorCardDateInput.Type,
                        "date-1",
                        true,
                        "date with time",
                        null,
                        true),
                    new O365ConnectorCardDateInput(
                        O365ConnectorCardDateInput.Type,
                        "date-2",
                        false,
                        "date only",
                        null,
                        false)
                },
                new List<O365ConnectorCardActionBase>
                {
                    new O365ConnectorCardHttpPOST(
                        O365ConnectorCardHttpPOST.Type,
                        "Send",
                        "dateInput",
                        @"{""date1"":""{{date-1.value}}"", ""date2"":""{{date-2.value}}""}")
                });
            #endregion

            var section = new O365ConnectorCardSection(
                "**section title**",
                "section text",
                "activity title",
                "activity subtitle",
                "activity text",
                "http://connectorsdemo.azurewebsites.net/images/MSC12_Oscar_002.jpg",
                true,
                new List<O365ConnectorCardFact>
                {
                    new O365ConnectorCardFact("Fact name 1", "Fact value 1"),
                    new O365ConnectorCardFact("Fact name 2", "Fact value 2"),
                },
                new List<O365ConnectorCardImage>
                {
                    new O365ConnectorCardImage
                    {
                        Image = "http://connectorsdemo.azurewebsites.net/images/WIN12_Anthony_02.jpg",
                        Title = "image 1"
                    },
                    new O365ConnectorCardImage
                    {
                        Image = "http://connectorsdemo.azurewebsites.net/images/WIN12_Scene_01.jpg",
                        Title = "image 2"
                    },
                    new O365ConnectorCardImage
                    {
                        Image = "http://connectorsdemo.azurewebsites.net/images/WIN12_Anthony_02.jpg",
                        Title = "image 3"
                    }
                });

            O365ConnectorCard card = new O365ConnectorCard()
            {
                Summary = "O365 card summary",
                ThemeColor = "#E67A9E",
                Title = "card title",
                Text = "card text",
                Sections = new List<O365ConnectorCardSection> { section },
                PotentialAction = new List<O365ConnectorCardActionBase>
                 {
                    multichoiceCard,
                    inputCard,
                    dateCard,
                    new O365ConnectorCardViewAction(
                        O365ConnectorCardViewAction.Type,
                        "View Action",
                        null,
                        new List<string>
                        {
                            "http://microsoft.com"
                        }),
                    new O365ConnectorCardOpenUri(
                        O365ConnectorCardOpenUri.Type,
                        "Open Uri",
                        "open-uri",
                        new List<O365ConnectorCardOpenUriTarget>
                        {
                            new O365ConnectorCardOpenUriTarget
                            {
                                Os = "default",
                                Uri = "http://microsoft.com"
                            },
                            new O365ConnectorCardOpenUriTarget
                            {
                                Os = "iOS",
                                Uri = "http://microsoft.com"
                            },
                            new O365ConnectorCardOpenUriTarget
                            {
                                Os = "android",
                                Uri = "http://microsoft.com"
                            },
                            new O365ConnectorCardOpenUriTarget
                            {
                                Os = "windows",
                                Uri = "http://microsoft.com"
                            }
                        })
                  }
            };

            return card.ToAttachment();
        }
    }
}