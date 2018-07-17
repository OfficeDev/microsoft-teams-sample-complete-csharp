using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Teams.TemplateBotCSharp.Properties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Teams.TemplateBotCSharp.Dialogs
{
    /// <summary>
    /// This is Adaptive Card Dialog Class. Main purpose of this class is to display the Adaptive Card example
    /// </summary>

    [Serializable]
    public class AdaptiveCardDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //Set the Last Dialog in Conversation Data
            context.UserData.SetValue(Strings.LastDialogKey, Strings.LastDialogAdaptiveCard);

            var message = context.MakeMessage();
            var attachment = GetAdaptiveCardAttachment();

            message.Attachments.Add(attachment);

            await context.PostAsync((message));

            context.Done<object>(null);
        }

        // Here is the example of adaptive card having image set, text block,
        // input box, date input, time input, toggle input, choice set(dropdown), choice set(dropdown) with multiselect etc.
        public static Attachment GetAdaptiveCardAttachment()
        {
            var card = new AdaptiveCard()
            {
                Version = "1.0", // version is mandatory field
                Body = new List<CardElement>()
                {
                    new Container()
                    {
                        Items = new List<CardElement>()
                        {
                            // TextBlock Item allows for the inclusion of text, with various font sizes, weight and color,
                            new TextBlock()
                            {
                                Text = "Adaptive Card!",
                                Speak = "Adaptive card!",
                                Weight = TextWeight.Bolder, // set the weight of text e.g. Bolder, Light, Normal
                                Size = TextSize.Large, // set the size of text e.g. Extra Large, Large, Medium, Normal, Small
                            },
                            // FactSet item makes it simple to display a series of facts (e.g. name/value pairs) in a tabular form
                            new FactSet
                            {
                                Separation = SeparationStyle.Default,
                                Facts =
                                {
                                    // Describes a fact in a FactSet as a key/value pair
                                    new AdaptiveCards.Fact
                                    {
                                        Title = "Board:",
                                        Value = "Adaptive Card"
                                    },
                                    new AdaptiveCards.Fact
                                    {
                                        Title = "List:",
                                        Value = "Backlog"
                                    },
                                    new AdaptiveCards.Fact
                                    {
                                        Title = "Assigned to:",
                                        Value = "Matt Hidinger"
                                    },
                                    new AdaptiveCards.Fact
                                    {
                                        Title = "Due date:",
                                        Value = "Not set"
                                    }
                                }
                            },
                            // ImageSet allows for the inclusion of a collection images like a photogallery
                            new ImageSet
                            {
                                ImageSize = ImageSize.Medium,
                                Images =
                                {
                                    // Image Item allows for the inclusion of images
                                    new Image
                                    {
                                        Url = "http://contososcubabot.azurewebsites.net/assets/steak.jpg"
                                    },
                                    new Image
                                    {
                                        Url = "http://contososcubabot.azurewebsites.net/assets/chicken.jpg"
                                    },
                                    new Image
                                    {
                                        Url = "http://contososcubabot.azurewebsites.net/assets/tofu.jpg"
                                    },
                                }
                            },// wrap the text in textblock
                            new TextBlock()
                            {
                                // mardown example for bold text
                                Text = "'**Matt H. said** \"I'm compelled to give this place 5 stars due to the number of times I've chosen to eat here this past year!\',", 
                                Wrap = true, // True if text is allowed to wrap
                            },
                            new TextBlock()
                            {
                                Text = "Place your text here:"
                            },
                            // text input collects text from the user
                            new TextInput()
                            {
                                Id = "textInputId",
                                Speak = "<s>Please enter your text here</s>",
                                Placeholder = "Text Input",
                                Style = TextInputStyle.Text // set the type of input box e.g Text, Tel, Email, Url
                            },
                            new TextBlock()
                            {
                                Text = "Please select Date here?"
                            },
                            // date input collects Date from the user
                            new DateInput()
                            {
                                Id = "dateInput",
                                Speak = "<s>Please select Date here?</s>",
                            },
                            new TextBlock()
                            {
                                Text = "Please enter time here?"
                            },
                            // time input collects time from the user
                            new TimeInput()
                            {
                                Id = "timeInput"                                
                            },
                            new TextBlock()
                            {
                                Separation = SeparationStyle.Default,
                                Text = "Please select your choice here? (Compact Dropdown)"
                            },
                            // Shows an array of Choice objects
                            new ChoiceSet()
                            {
                               Id = "choiceSetCompact",
                               Value = "1", // please set default value here
                               Style = ChoiceInputStyle.Compact, // set the style of Choice set to compact
                               Choices =
                               {
                                  // describes a choice input. the value should be a simple string without a ","
                                  new Choice
                                  {
                                      Title = "Red",
                                      Value = "1" // do not use a “,” in the value, since MultiSelect ChoiceSet returns a comma-delimited string of choice values
                                  },
                                  new Choice
                                  {
                                      Title = "Green",
                                      Value = "2"
                                  },
                                  new Choice
                                  {
                                      Title = "Blue",
                                      Value = "3"
                                  },
                                  new Choice
                                  {
                                      Title = "White",
                                      Value = "4"
                                  }
                                }
                            },
                            new TextBlock()
                            {
                                Separation = SeparationStyle.Default,
                                Text = "Please select your choice here? (Expanded Dropdown)"
                            },
                            // Shows an array of Choice objects
                            new ChoiceSet()
                            {
                               Id= "choiceSetExpandedRequired",
                               Value = "1", // please set default value here
                               Style = ChoiceInputStyle.Expanded, // set the style of Choice set to expanded
                               IsRequired = true, // set required value here
                               Choices =
                               {
                                    new Choice
                                    {
                                        Title = "Red",
                                        Value = "1"
                                    },
                                    new Choice
                                    {
                                        Title = "Green",
                                        Value = "2"
                                    },
                                    new Choice
                                    {
                                        Title = "Blue",
                                        Value = "3"
                                    },
                                    new Choice
                                    {
                                        Title = "White",
                                        Value = "4"
                                    }
                               }
                            },
                            new TextBlock()
                            {
                                Text = "Please select multiple items here? (Multiselect Dropdown)"
                            },
                            // Shows an array of Choice objects (Multichoice)
                            new ChoiceSet()
                            {
                               Id = "choiceSetExpanded",
                               Value = "1,2", // The initial choice (or set of choices) that should be selected. For multi-select, specifcy a comma-separated string of values
                               Style = ChoiceInputStyle.Expanded, // // set the style of Choice set to expanded
                               IsMultiSelect = true, // allow multiple choices to be selected
                               Choices =
                               {
                                    new Choice
                                    {
                                        Title = "Red",
                                        Value = "1"
                                    },
                                    new Choice
                                    {
                                        Title = "Green",
                                        Value = "2"
                                    },
                                    new Choice
                                    {
                                        Title = "Blue",
                                        Value = "3"
                                    },
                                    new Choice
                                    {
                                        Title = "White",
                                        Value = "4"
                                    }
                               }
                            },                            
                            // column set divides a region into Column's allowing elements to sit side-by-side
                            new ColumnSet()
                            {
                                Columns = new List<Column>()
                                {
                                    // defines a container that is part of a column set
                                    new Column()
                                    {
                                        Size = ColumnSize.Auto, // “auto”, “stretch”, or a number representing relative width of the column in the column group
                                        Items = new List<CardElement>()
                                        {
                                            new Image()
                                            {
                                                Url = "https://placeholdit.imgix.net/~text?txtsize=65&txt=Adaptive+Cards&w=300&h=300",
                                                Size = ImageSize.Medium,
                                                Style = ImageStyle.Person
                                            }
                                        }
                                    },
                                    new Column()
                                    {
                                        Size = ColumnSize.Stretch, // “auto”, “stretch”, or a number representing relative width of the column in the column group
                                        Items = new List<CardElement>()
                                        {
                                            new TextBlock()
                                            {
                                                Text =  "Hello!",
                                                Weight = TextWeight.Bolder,
                                                IsSubtle = true
                                            },
                                            new TextBlock()
                                            {
                                                Text = "Are you looking for a Tab or Bot?",
                                                Wrap = true
                                            }
                                        }
                                    }
                                }
                            },
                            //  input toggle collects a true/false response from the user
                            new ToggleInput
                            {
                                Id = "AcceptsTerms",
                                Title = "I accept the terms and conditions (True/False)",
                                ValueOff = "false", // the value when toggle is off (default: false)
                                ValueOn = "true"  // the value when toggle is on (default: true)
                            },
                        }
                    }
                },
                Actions = new List<ActionBase>()
                {
                    // submit action gathers up input fields, merges with optional data field and generates event to client asking for data to be submitted
                    new SubmitAction()
                    {
                        Title = "Submit",
                        Speak = "<s>Search</s>"
                    },
                    // show action defines an inline AdaptiveCard which is shown to the user when it is clicked
                    new ShowCardAction()
                    {
                        Card = new AdaptiveCard()
                        {
                            Version = "1.0",
                            Body = new List<CardElement>()
                            {
                                new Container()
                                {
                                    Items = new List<CardElement>()
                                    {
                                        new TextInput()
                                        {
                                            Id = "Text",
                                            Speak = "<s>Please enter your text here?</s>",
                                            Placeholder = "text here",
                                            Style = TextInputStyle.Text
                                        },
                                    }
                                }
                            },
                            Actions = new List<ActionBase>()
                            {
                                new SubmitAction()
                                {
                                    Title = "Submit",
                                    Speak = "<s>Search</s>"
                                },
                            }
                        }
                    },
                    // open url show the given url, either by launching it to an external web browser
                    new OpenUrlAction()
                    {
                        Title = "Open Url",
                        Url = "http://adaptivecards.io/explorer/Action.OpenUrl.html"
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            return attachment;
        }
    }
}