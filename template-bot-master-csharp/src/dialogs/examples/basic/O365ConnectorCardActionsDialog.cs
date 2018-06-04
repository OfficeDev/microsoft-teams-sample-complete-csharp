using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Actionable Card Dialog Class. Main purpose of this class is to show example of O365connector card actionable feature sample like 
    /// multi choice, date/time, input text and multiple sections examples
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

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogActionableMessageDialog);

            var message = context.MakeMessage();

            string baseUri = Convert.ToString(ConfigurationManager.AppSettings["BaseUri"]);

            // get the input number for the example to show if the user passed it into the command - e.g. 'show connector card 2'
            var activity = (IMessageActivity)context.Activity;

            string inputNumber = activity.Text.Substring(activity.Text.Length - 1, 1).Trim();
            Attachment attachment = null;

            switch (inputNumber)
            {
                // Actionable cards can have multiple sections, each with its own set of actions.
                // If a section contains only 1 card action, that is automatically expanded
                case "2":
                    attachment = O365ActionableCardMultipleSection(baseUri);
                    break;

                // this is the default example's content
                // multiple choice (compact & expanded), text input, date and placing images in card
                case "1":
                default:
                    attachment = O365ActionableCardDeafult();
                    break;
            }

            message.Attachments.Add(attachment);
            await context.PostAsync((message));

            context.Done<object>(null);
        }

        /// <summary>
        /// this is the default example's content
        /// multiple choice (compact & expanded), text input, date and placing images in card
        /// </summary>
        /// <returns>The result card with actions.</returns>
        /// 
        public static Attachment O365ActionableCardDeafult()
        {
            #region multi choice examples
           
            var multichoice = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Multiple Choice",
                "Multiple Choice Card",
                new List<O365ConnectorCardInputBase>
                {
                     // multiple choice control with required, multiselect, expanded style
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
                    // multiple choice control with required, multiselect, compact style
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
                    // multiple choice control with single item select, expanded style
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
                    // multiple choice control with single item select, compact style
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

            #region text input examples
            var inputCard = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Text Input",
                "Input Card",
                new List<O365ConnectorCardInputBase>
                {
                    // text input control with multiline
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-1",
                        false,
                        "multiline, no maxLength",
                        null,
                        true,
                        null),
                    // text input control without multiline
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-2",
                        false,
                        "single line, no maxLength",
                        null,
                        false,
                        null),
                    // text input control with multiline, reuired,
                    // and control the length of input box
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-3",
                        true,
                        "multiline, max len = 10, isRequired",
                        null,
                        true,
                        10),
                    // text input control without multiline, reuired,
                    // and control the length of input box
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

            #region date/time input examples
            var dateCard = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Date Input",
                "Date Card",
                new List<O365ConnectorCardInputBase>
                {
                    // date input control, with date and time, required
                    new O365ConnectorCardDateInput(
                        O365ConnectorCardDateInput.Type,
                        "date-1",
                        true,
                        "date with time",
                        null,
                        true),
                    // date input control, only date, no time, not required
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

            var section = new O365ConnectorCardSection
            {
                Title = "**section title**",
                Text = "section text",
                ActivityTitle = "activity title",
                ActivitySubtitle = "activity subtitle",
                ActivityText = "activity text",
                ActivityImage = "http://connectorsdemo.azurewebsites.net/images/MSC12_Oscar_002.jpg",
                ActivityImageType = null,
                Markdown = true,
                Facts = new List<O365ConnectorCardFact>
                {
                    new O365ConnectorCardFact("Fact name 1", "Fact value 1"),
                    new O365ConnectorCardFact("Fact name 2", "Fact value 2"),
                },
                Images = new List<O365ConnectorCardImage>
                {
                    new O365ConnectorCardImage
                    {
                        Image = "http://connectorsdemo.azurewebsites.net/images/MicrosoftSurface_024_Cafe_OH-06315_VS_R1c.jpg",
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
                }
            };

            O365ConnectorCard card = new O365ConnectorCard()
            {
                Summary = "O365 card summary",
                ThemeColor = "#E67A9E",
                Title = "card title",
                Text = "card text",
                Sections = new List<O365ConnectorCardSection> { section },
                PotentialAction = new List<O365ConnectorCardActionBase>
                {
                    multichoice,
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

        /// <summary>
        /// Actionable cards can have multiple sections, each with its own set of actions.
        /// If a section contains only 1 card action, that is automatically expanded
        /// </summary>
        /// <returns>The result card with actions.</returns>
        /// 
        public static Attachment O365ActionableCardMultipleSection(string baseUri)
        {
            #region Section1
            #region Multichoice Card
            // multiple choice control with required, multiselect, compact style
            var multichoiceCardSection1 = new O365ConnectorCardActionCard(
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
                        "compact",
                        true)
                },

                new List<O365ConnectorCardActionBase>
                {
                new O365ConnectorCardHttpPOST
                (
                    O365ConnectorCardHttpPOST.Type,
                    "Send",
                    "multichoice",
                    @"{""CardsType"":""{{CardsType.value}}""")
                });

            #endregion

            var potentialActionSection1 = new List<O365ConnectorCardActionBase>
            {
                 multichoiceCardSection1
            };

            var section1 = new O365ConnectorCardSection
            {
                Title = "Section Title 1",
                Text = "",
                ActivityTitle = "",
                ActivitySubtitle = "",
                ActivityText = "",
                ActivityImage = null,
                ActivityImageType = null,
                Markdown = true,
                Facts = null,
                Images = null,
                PotentialAction = potentialActionSection1
            };
            #endregion 

            #region Section2

            #region Input Card
            // text input examples
            var inputCard = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Text Input",
                "Input Card",
                // text input control with multiline
                new List<O365ConnectorCardInputBase>
                {
                    new O365ConnectorCardTextInput(
                        O365ConnectorCardTextInput.Type,
                        "text-1",
                        false,
                        "This is the title of text box",
                        null,
                        true,
                        null)
                },
                new List<O365ConnectorCardActionBase>
                {
                    new O365ConnectorCardHttpPOST(
                        O365ConnectorCardHttpPOST.Type,
                        "Send",
                        "inputText",
                        @"{""text1"":""{{text-1.value}}""}")
                });
            #endregion

            #region Multichoice Card For Section2
            // multiple choice control with not required, multiselect, compact style
            var multichoiceCardSection2 = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Multiple Choice",
                "Multiple Choice Card",
                new List<O365ConnectorCardInputBase>
                {
                    new O365ConnectorCardMultichoiceInput(
                        O365ConnectorCardMultichoiceInput.Type,
                        "CardsTypesection1", //please make sure that id of the control must be unique across card to work properly
                        false,
                        "This is a title of combo box",
                        "",
                        new List<O365ConnectorCardMultichoiceInputChoice>
                        {
                            new O365ConnectorCardMultichoiceInputChoice("Hero Card", "Hero Card"),
                            new O365ConnectorCardMultichoiceInputChoice("Thumbnail Card", "Thumbnail Card"),
                            new O365ConnectorCardMultichoiceInputChoice("O365 Connector Card", "O365 Connector Card")
                        },
                        "compact",
                        true)
                },
                new List<O365ConnectorCardActionBase>
                {
                    new O365ConnectorCardHttpPOST
                    (
                        O365ConnectorCardHttpPOST.Type,
                        "Send",
                        "multichoice",
                        @"{""CardsTypesection1"":""{{CardsTypesection1.value}}""")
                });

            #endregion

            // please always attach new potential action to individual sections
            var potentialActionSection2 = new List<O365ConnectorCardActionBase>
            {
                 inputCard,
                 multichoiceCardSection2
            };

            var section2 = new O365ConnectorCardSection
            {
                Title = "Section Title 2",
                Text = "",
                ActivityTitle = "",
                ActivitySubtitle = "",
                ActivityText = "",
                ActivityImage = null,
                ActivityImageType = null,
                Markdown = true,
                Facts = null,
                Images = null,
                PotentialAction = potentialActionSection2
            };
            #endregion

            O365ConnectorCard card = new O365ConnectorCard()
            {
                Title = "This is Actionable Card Title",
                Summary = "O365 card summary",
                ThemeColor = "#E67A9E",
                Sections = new List<O365ConnectorCardSection> { section1, section2 },
            };

            return card.ToAttachment();
        }
    }
}