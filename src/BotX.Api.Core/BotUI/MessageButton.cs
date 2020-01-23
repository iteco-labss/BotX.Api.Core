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
		public string Title { get; }

		/// <summary>
		/// Команда, которая будет отправлена в чат, при нажатии этой кнопки
		/// </summary>
		public string Command { get; }

		internal Data Data { get; set; }

		internal bool IsSilent { get; set; }

		internal string InternalCommand { get; }

		/// <summary>
		/// Создаёт кнопку, привязанную к действию
		/// </summary>
		/// <param name="title">Текст на кнопке</param>
		/// <param name="event">Событие, выполняемое при нажатии</param>
		/// <param name="payload"></param>
		/// <param name="isSilent"></param>
		internal MessageButton(string title, BotEventHandler @event, Payload payload, bool isSilent)
		{
			Title = title;
			Data = new Data();
			IsSilent = isSilent;
			var pair = ActionExecutor.actionEvents.SingleOrDefault(x => x.Value.Event == @event.GetMethodInfo());
			if (!pair.Equals(default(KeyValuePair<string, EventData>)))
			{
				Data.EventType = pair.Key;
				Data.Payload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings()
				{
					TypeNameHandling = TypeNameHandling.All,
					MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
				});
			}
			InternalCommand = title;
		}
	}
}
