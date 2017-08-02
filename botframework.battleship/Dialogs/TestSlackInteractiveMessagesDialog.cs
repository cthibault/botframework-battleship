using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace botframework.battleship.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;

            await this.SendQuestionAsync(context);
        }

        private async Task SendQuestionAsync(IDialogContext context)
        {
            await context.PostAsync("Initiating interactive message demo with multi-dialog configuration...");

            context.Call(new SlackInteractiveMessageDialog(), this.SlackInteractiveMessageDialogResumeAfter);
        }

        private async Task SlackInteractiveMessageDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;

            await context.PostAsync(message);
        }
    }

    [Serializable]
    public class SlackInteractiveMessageDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var msg = context.MakeMessage();
            msg.ChannelData = this.BuildMessage();

            await context.PostAsync(msg);

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var response = await result;

            await context.PostAsync($"Response contained channel data: {response.ChannelData != null}");

            context.Done("SlackInteractiveMessageDialog is done...");
        }

        private SlackMessage BuildMessage()
        {
            return new SlackMessage
            {
                Text = "Interactive Message Test...",
                Attachments = new Attachment[]
                {
                    new Attachment
                    {
                        Text = "So, is it a Yes or a No?",
                        CallbackId = "test-interactive-msg",
                        Color = "#3AA3E3",
                        AttachmentType = "default",
                        Actions = new Action[]
                        {
                            new Action
                            {
                                Name = "accept",
                                Text = "Yes",
                                Type = "button",
                                Value = "yes"
                            },
                            new Action
                            {
                                Name = "accept",
                                Text = "No",
                                Type = "button",
                                Value = "no"
                            },
                        }
                    }
                }
            };
        }
    }


    [Serializable]
    public class TestSlackInteractiveMessagesDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var reply = context.MakeMessage();

            var channelData = new SlackMessage
            {
	            Text = $"I received the following: `{activity.Text}`",
	            Attachments = new Attachment[]
	            {
		            new Attachment
		            {
			            Text = "Did you send this message to me?",
			            CallbackId = "acknowledge",
			            Color = "#3AA3E3",
			            AttachmentType = "default",
			            Actions = new Action[]
			            {
				            new Action
				            {
					            Name = "accept",
					            Text = "Yes",
					            Type = "button",
					            Value = "yes"
				            },
				            new Action
				            {
					            Name = "accept",
					            Text = "No",
					            Type = "button",
					            Value = "no"
				            },
			            }
		            }
	            }
            };          

            context.Wait(MessageReceivedAsync);
        }
    }

    class SlackMessage
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("attachments")]
        public Attachment[] Attachments { get; set; }
    }

    class Attachment
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("attachment_type")]
        public string AttachmentType { get; set; }

        [JsonProperty("actions")]
        public Action[] Actions { get; set; }
    }

    class Action
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
/*
{
    "text": "@PlayerA has challenged you to a friendly game of battleship.",
    "attachments": [
        {
            "text": "Do you accept their challenge?",
            "callback_id": "accept-challenge",
            "color": "#3AA3E3",
            "attachment_type": "default",
            "actions": [
                {
                    "name": "accept",
                    "text": "Yes",
                    "type": "button",
                    "value": "yes"
                },
                {
                    "name": "accept",
                    "text": "No",
                    "type": "button",
                    "value": "no"
                }
            ]
        }
    ]
}
*/
}