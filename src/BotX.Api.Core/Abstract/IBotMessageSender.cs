using System;
using System.Threading.Tasks;
using BotX.Api.BotUI;
using BotX.Api.JsonModel.Request;

namespace BotX.Api
{
	/// <summary>
	/// Клиент, реализующий отправку сообщений, используя BotX Api
	/// </summary>
	public interface IBotMessageSender
	{
		Task ReplyTextMessageAsync(Guid syncId, Guid to, string messageText);
		Task ReplyTextMessageAsync(Guid syncId, Guid to, string messageText, MessageButtonsGrid buttons);
		Task ReplyTextMessageAsync(UserMessage requestMessage, string messageText);
		Task ReplyTextMessageAsync(UserMessage requestMessage, string messageText, Guid mentionHuid);
		Task ReplyTextMessageAsync(UserMessage requestMessage, string messageText, MessageButtonsGrid buttons);
		Task SendFileAsync(Guid syncId, string fileName, byte[] data);
		Task SendFileAsync(UserMessage requestMessage, string fileName, byte[] data);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) пользователю
		/// </summary>
		/// <param name="chatId">Идентификатор чата, куда будет отправлено сообщение</param>
		/// <param name="huid">Идентификатор получателя (пользователя) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <returns></returns>
		Task SendTextMessageAsync(Guid chatId, Guid huid, string messageText);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) с кнопками пользователю
		/// </summary>
		/// <param name="chatId">Идентификатор чата, куда будет отправлено сообщение</param>
		/// <param name="huid">Идентификатор получателя (пользователя) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		Task SendTextMessageAsync(Guid chatId, Guid huid, string messageText, MessageButtonsGrid buttons);
		
		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) пользователю
		/// </summary>
		/// <param name="chatIds">Идентификаторы чатов, куда будет отправлено сообщение</param>
		/// <param name="recipients">Идентификаторы получателей (пользователей) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <returns></returns>	
		Task SendTextMessageAsync(Guid[] chatIds, Guid[] recipients, string messageText);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) с кнопками пользователю
		/// </summary>
		/// <param name="chatIds">Идентификаторы чатов, куда будет отправлено сообщение</param>
		/// <param name="recipients">Идентификаторы получателей (пользователей) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		Task SendTextMessageAsync(Guid[] chatIds, Guid[] recipients, string messageText, MessageButtonsGrid buttons);
	}
}