using BotX.Api;
using BotX.Api.Attributes;
using BotX.Api.BotUI;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using BotX.Api.StateMachine;
using Example.StateMachine.States;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.StateMachine
{
	public class DemoStateMachine : BaseStateMachine
	{
		private static Dictionary<Guid, string> statesStorage = new Dictionary<Guid, string>();

		public DemoStateMachine(IBotMessageSender messageSender, IServiceScopeFactory scopeFactory) : base(messageSender, scopeFactory)
		{
		}

		public override async Task OnFinishedAsync()
		{
			await MessageSender.ReplyTextMessageAsync(UserMessage, "Thank you!!");
			var buttons = new MessageButtonsGrid();
			var row = buttons.AddRow();
			row.AddSilentButton("Again", "again");
			await MessageSender.ReplyTextMessageAsync(UserMessage, $"result: {JsonConvert.SerializeObject(Model)}", buttons);
		}

		public override async Task OnStartedAsync()
		{
			await TransitionToAsync<GenderState>();
		}

		public override BaseStateMachine RestoreState()
		{
			if (statesStorage.ContainsKey(UserMessage.From.Huid))
				return FromJson<DemoStateMachine>(statesStorage[UserMessage.From.Huid], MessageSender, UserMessage);
			return null;	
		}

		public override void SaveState()
		{
			statesStorage[UserMessage.From.Huid] = ToJson();
		}

		[BotButtonEvent("Again")]
		public async Task AgainClick(UserMessage message, Payload payload)
		{
			await ResetAsync();
		}

		[BotButtonEvent("GenderSelect")]
		public async Task GenderSelectClick(UserMessage userMessage, GenderPayload payload)
		{
			var data = payload;
			Model.gender = data.Gender;
			await TransitionToAsync<RecommendState>();
		}
	}

	public class GenderPayload : Payload
	{
		public string Gender { get; set; }
	}
}
