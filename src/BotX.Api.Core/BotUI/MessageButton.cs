using BotX.Api.Delegates;
using BotX.Api.Executors;
using BotX.Api.JsonModel.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BotX.Api.BotUI
{
	/// <summary>
	/// Кнопка, добавляемая в сообщение бота
	/// </summary>
	public class MessageButton
	{
		/// <summary>
		/// Текст на кнопке
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Команда, которая будет отправлена в чат, при нажатии этой кнопки
		/// </summary>
		public string Command { get; set; }

		internal Data Data { get; set; }

		internal bool IsSilent { get; set; }

		internal string InternalCommand { get; set; }
		
		internal static MessageButton Create<T>(string title, string command, BotEventHandler<T> @event, T payload, bool isSilent) where T : Payload
		{
			MessageButton res = new MessageButton()
			{
				Title = title,
				Data = new Data(),
				IsSilent = isSilent,
				InternalCommand = string.IsNullOrEmpty(command) ? title : command
			};

			if (@event != null)
			{
				var pair = ActionExecutor.actionEvents.SingleOrDefault(x => x.Value.Event == @event.GetMethodInfo());
				if (!pair.Equals(default(KeyValuePair<string, EventData>)))
				{
					res.Data.EventType = pair.Key;
					res.Data.Payload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings()
					{
						TypeNameHandling = TypeNameHandling.All,
						MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
					});
				}
			}
			return res;
		}
	}
}
