using BotX.Api;
using BotX.Api.Delegates;
using BotX.Api.JsonModel.Request;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Example.ChatProcessing
{
	public class HelloBotMiddleware
	{
		private readonly BotMiddlewareHandler next;
		public HelloBotMiddleware(BotMiddlewareHandler next, ILogger<HelloBotMiddleware> logger)
		{
			this.next = next;
		}

		public async Task InvokeAsync(UserMessage message, IBotMessageSender sender)
		{
			if (message.Command.Body.ToLower() == "hello")
				await sender.ReplyTextMessageAsync(message, "hi!");
			else
				await next(message);
		}
	}
}
