using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Extensions
{
	internal static class HttpClientExtensions
	{
		public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
			this HttpClient httpClient, string url, T data)
		{
			var dataAsString = JsonConvert.SerializeObject(data);
			var content = new StringContent(dataAsString);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			return httpClient.PostAsync(url, content);
		}
		public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
			this HttpClient httpClient, Uri uri, T data)
		{
			if (uri == null)
				throw new ArgumentNullException(nameof(uri));

			if (data == null)
				throw new ArgumentNullException(nameof(data));

			return PostAsJsonAsync(httpClient, uri.ToString(), data);
		}

		public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
		{
			var dataAsString = await content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(dataAsString);
		}
	}
}
