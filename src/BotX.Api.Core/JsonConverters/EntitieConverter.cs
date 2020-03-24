using System;
using System.Collections.Generic;
using System.Text;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BotX.Api.JsonConverters
{
	public class EntitieConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException("Мы не умеем сериализовать Mention");
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jsonObject = JObject.Load(reader);
			if (jsonObject.GetValue("type")?.ToString() == "mention")
			{
				Entitie result = new Entitie();
				result.Type = "mention";
				var mention = jsonObject.GetValue("data");
				result.Data = serializer.Deserialize<Mention>(mention?.CreateReader());
				return result;
			}

			return null;
		}

		public override bool CanWrite { get => false; }

		public override bool CanConvert(Type objectType) => objectType == typeof(Entitie);
	}
}
