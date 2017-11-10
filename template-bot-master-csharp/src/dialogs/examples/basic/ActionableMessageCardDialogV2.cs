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
    /// This is Actionable Card Dialog Class. Main purpose of this class is to show example of Actionable feature sample like 
    /// MultiChoice, Date, Dropdown and Text
    /// </summary>

    [Serializable]
    public class ActionableMessageCardDialogV2 : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogActionableMessageDialog);

            //Get the Config Base Uri

            string baseUri = Convert.ToString(ConfigurationManager.AppSettings["BaseUri"]);

            var message = context.MakeMessage();
            var attachment = CreateSampleO365ConnectorCard(baseUri);
            message.Attachments.Add(attachment);

            await context.PostAsync((message));

            context.Done<object>(null);
        }

        /// <summary>
        /// Create a sample O365 connector card.
        /// </summary>
        /// <returns>The result card with actions.</returns>
        /// 
        public static Attachment CreateSampleO365ConnectorCard(string baseUri)
        {
            string imageUrl = baseUri + "/public/assets/ActionableCardIconImage.png";
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
                        true)
                },

                  new List<O365ConnectorCardActionBase>
                  {
                   new O365ConnectorCardHttpPOST
                   (
                        O365ConnectorCardHttpPOST.Type,
                        "Send",
                        "multichoice",
                        @"{""CardsType"":""{{CardsType.value}}"", ""Teams"":""{{Teams.value}}""}")
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

            #region Date Card
            var dateCard = new O365ConnectorCardActionCard(
                O365ConnectorCardActionCard.Type,
                "Date Input",
                "Date Card",
                new List<O365ConnectorCardInputBase>
                {
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
                        @"{""date1"":""{{date-1.value}}""")
                });
            #endregion

            var section = new O365ConnectorCardSection(
                "",
                "",
                "Actionable Message",
                "",
                "This is an actionable message card. You can add operations within the card.",
                imageUrl,
                true,
                null,
                null);

            O365ConnectorCard card = new O365ConnectorCard()
            {
                Summary = "O365 card summary",
                ThemeColor = "#E67A9E",
                Sections = new List<O365ConnectorCardSection> { section },

                PotentialAction = new List<O365ConnectorCardActionBase>
                {
                    multichoiceCard,
                    inputCard,
                    dateCard
                }
            };

            return card.ToAttachment();
        }
    }
}