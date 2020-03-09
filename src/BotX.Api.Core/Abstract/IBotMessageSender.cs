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
		/// <summary>
		/// Отправляет ответ в виде текстового сообщения
		/// </summary>
		/// <param name="syncId">Идентификатор чата</param>
		/// <param name="to">huid получателей события</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <returns>Идентификатор сообщения(необходим для его редактирования)</returns>
		Task<Guid> ReplyTextMessageAsync(Guid syncId, Guid[] to, string messageText);

		/// <summary>
		/// Отправляет текствое сообщение с кнопками в ответ пользователю
		/// </summary>
		/// <param name="syncId">Идентификатор чата</param>
		/// <param name="to">huid получателей события</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns>Идентификатор сообщения(необходим для его редактирования)</returns>
		Task<Guid> ReplyTextMessageAsync(Guid syncId, Guid[] to, string messageText, MessageButtonsGrid buttons);

		/// <summary>
		/// Отправляет текстовое сообщение в ответ пользователю
		/// </summary>
		/// <param name="requestMessage">Сообщение от пользователя</param>
		/// <param name="messageText">Текст ответа</param>
		/// <returns>Идентификатор сообщения(необходим для его редактирования)</returns>
		Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, string messageText);

		/// <summary>
		/// Отправляет текстовое сообщение в ответ пользователю
		/// </summary>
		/// <param name="requestMessage">Сообщение от пользователя</param>
		/// <param name="messageText">Текст ответа</param>
		/// <param name="mentionHuid">Идентификатор пользователя, которого нужно упомянуть</param>
		/// <returns>Идентификатор сообщения(необходим для его редактирования)</returns>
		Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, string messageText, Guid mentionHuid);

		/// <summary>
		/// Отправка текстового сообщения с кнопками (действиями) в ответ пользователю
		/// </summary>
		/// <param name="requestMessage">Сообщение пользователя</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns>Идентификатор сообщения(необходим для его редактирования)</returns>
		Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, string messageText, MessageButtonsGrid buttons);

		/// <summary>
		/// Отправка файла в чат
		/// </summary>
		/// <param name="syncId">Идентификатор чата</param>
		/// <param name="fileName">Имя файла</param>
		/// <param name="data">Данные файла</param>
		/// <returns></returns>
		Task SendFileAsync(Guid syncId, string fileName, byte[] data);

		/// <summary>
		/// Отправляет файл в ответ на пользовательское сообщение
		/// </summary>
		/// <param name="requestMessage">Сообщение от пользователя</param>
		/// <param name="fileName">Имя файла</param>
		/// <param name="data">Данные файла</param>
		/// <returns></returns>	
		Task SendFileAsync(UserMessage requestMessage, string fileName, byte[] data);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) всем пользователям, которые писали боту или чатам, в которых есть бот
		/// </summary>
		/// <param name="messageText">Текст сообщения</param>
		/// <returns></returns>
		Task SendTextMessageAsync(string messageText);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) всем пользователям, которые писали боту или чатам, в которых есть бот
		/// </summary>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		Task SendTextMessageAsync(string messageText, MessageButtonsGrid buttons);

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

		/// <summary>
		/// Редактирует сообщение отправленное пользователю
		/// </summary>
		/// <param name="syncId">Идентификатор сообщения</param>
		/// <param name="messageText">Новый текст сообщения</param>
		/// <returns></returns>
		Task EditMessageAsync(Guid syncId, string messageText);

		/// <summary>
		/// Редактирует сообщение отправленное пользователю
		/// </summary>
		/// <param name="syncId">Идентификатор сообщения</param>
		/// <param name="messageText">Новый текст сообщения</param>
		/// <param name="buttons">Новые кнопки</param>
		/// <returns></returns>
		Task EditMessageAsync(Guid syncId, string messageText, MessageButtonsGrid buttons);

		/// <summary>
		/// Редактирует сообщение отправленное пользователю
		/// </summary>
		/// <param name="syncId">Идентификатор сообщения</param>
		/// <param name="messageText">Новый текст сообщения</param>
		/// <param name="mentionHuid">Идентификатор пользователя, которого нужно упомянуть</param>
		/// <returns></returns>
		Task EditMessageAsync(Guid syncId, string messageText, Guid mentionHuid);
	}
}