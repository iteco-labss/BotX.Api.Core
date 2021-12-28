using System;
using System.Threading.Tasks;
using BotX.Api.BotUI;
using BotX.Api.JsonModel.Api.Request;
using BotX.Api.JsonModel.Api.Response;
using BotX.Api.JsonModel.Request;

namespace BotX.Api
{
	/// <summary>
	/// Клиент, реализующий отправку сообщений, используя BotX Api
	/// </summary>
	public interface IBotMessageSender
	{
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
		/// Отправляет файл в ответ на пользовательское сообщение
		/// </summary>
		/// <param name="requestMessage">Сообщение от пользователя</param>
		/// <param name="fileName">Имя файла</param>
		/// <param name="data">Данные файла</param>
		/// <returns></returns>	
		Task SendFileAsync(UserMessage requestMessage, string fileName, byte[] data);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) пользователю
		/// </summary>
		/// <param name="botId"></param>
		/// <param name="chatId">Идентификатор чата, куда будет отправлено сообщение</param>
		/// <param name="huid">Идентификатор получателя (пользователя) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <returns></returns>
		Task SendTextMessageAsync(Guid botId, Guid chatId, Guid huid, string messageText);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) с кнопками пользователю
		/// </summary>
		/// <param name="botId"></param>
		/// <param name="chatId">Идентификатор чата, куда будет отправлено сообщение</param>
		/// <param name="huid">Идентификатор получателя (пользователя) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		Task SendTextMessageAsync(Guid botId, Guid chatId, Guid huid, string messageText, MessageButtonsGrid buttons);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) пользователю
		/// </summary>
		/// <param name="botId"></param>
		/// <param name="chatId">Идентификатор чата, куда будет отправлено сообщение</param>
		/// <param name="huid">Идентификатор получателя (пользователя) сообщения</param>
		/// <param name="messageSyncId">Идентификатор сообщения в Express (по которому его можно редактировать)</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <returns></returns>
		Task SendTextMessageAsync(Guid botId, Guid chatId, Guid huid, Guid messageSyncId, string messageText);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) с кнопками пользователю
		/// </summary>
		/// <param name="botId"></param>
		/// <param name="chatId">Идентификатор чата, куда будет отправлено сообщение</param>
		/// <param name="huid">Идентификатор получателя (пользователя) сообщения</param>
		/// <param name="messageSyncId">Идентификатор сообщения в Express (по которому его можно редактировать)</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		Task SendTextMessageAsync(Guid botId, Guid chatId, Guid huid, Guid messageSyncId, string messageText, MessageButtonsGrid buttons);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) пользователю
		/// </summary>
		/// <param name="chatId"></param>
		/// <param name="recipients">Идентификаторы получателей (пользователей) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="botId"></param>
		/// <returns></returns>	
		Task SendTextMessageAsync(Guid botId, Guid chatId, Guid[] recipients, string messageText);

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) с кнопками пользователю
		/// </summary>
		/// <param name="botId"></param>
		/// <param name="chatId">Идентификаторы чатов, куда будет отправлено сообщение</param>
		/// <param name="recipients">Идентификаторы получателей (пользователей) сообщения</param>
		/// <param name="messageSyncId">Идентификатор сообщения в Express (по которому его можно редактировать)</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		Task SendTextMessageAsync(Guid botId, Guid chatId, Guid[] recipients, Guid? messageSyncId, string messageText, MessageButtonsGrid buttons);

		/// <summary>
		/// Редактирует сообщение ранее отправленное пользователю
		/// </summary>
		/// <param name="botId">Идентификатор бота</param>
		/// <param name="syncId">Идентификатор сообщения</param>
		/// <param name="messageText">Новый текст сообщения</param>
		/// <param name="mentionHuid">Идентификатор пользователя, которого нужно упомянуть</param>
		/// <returns></returns>
		Task EditMessageAsync(Guid botId, Guid syncId, string messageText, Guid mentionHuid);

		/// <summary>
		/// Редактирует сообщение ранее отправленное пользователю
		/// </summary>
		/// <param name="requestMessage">Входящее сообщение от пользователя</param>
		/// <param name="syncId">Идентификатор редактируемого сообщения</param>
		/// <param name="messageText">Новый текст сообщения</param>
		/// <returns></returns>
		Task EditMessageAsync(UserMessage requestMessage, Guid syncId, string messageText);

		/// <summary>
		/// Редактирует сообщение ранее отправленное пользователю
		/// </summary>
		/// <param name="requestMessage">Входящее сообщение от пользователя</param>
		/// <param name="syncId">Идентификатор редактируемого сообщения</param>
		/// <param name="messageText">Новый текст сообщения</param>
		/// <param name="buttons">Новые кнопки в сообщении</param>
		/// <returns></returns>
		Task EditMessageAsync(UserMessage requestMessage, Guid syncId, string messageText, MessageButtonsGrid buttons);

		/// <summary>
		/// Отправляет файл в указанный чат
		/// </summary>
		/// <param name="botId"></param>
		/// <param name="chatId"></param>
		/// <param name="huid"></param>
		/// <param name="fileName"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		Task SendFileAsync(Guid botId, Guid chatId, Guid huid, string fileName, byte[] data);

		/// <summary>
		/// Отправляет файл в указанный чат с указанным заголовком
		/// </summary>
		/// <param name="botId">Идентификатор бота</param>
		/// <param name="chatId">Идентификатор чата</param>
		/// <param name="huid">Идентификатор пользователя (получателя)</param>
		/// <param name="caption">Заголовок</param>
		/// <param name="fileName">Имя файла с расширением</param>
		/// <param name="data">Данные файла</param>
		/// <returns></returns>
		Task SendFileAsync(Guid botId, Guid chatId, Guid huid, string caption, string fileName, byte[] data);

		/// <summary>
		/// Загружает файл в указанный чат с указанной метаинфой
		/// </summary>
		/// <param name="requestMessage">Сообщение пользователя</param>
		/// <param name="fileName">Имя файла с расширением</param>
		/// <param name="data">Данные файла</param>
		/// <param name="meta">Метаданные файла</param>
		/// <returns>Информация о сохраненном файле</returns>
		Task<FileMetadata> UploadFileAsync(UserMessage requestMessage, string fileName, byte[] data, FileMetaInfo meta = null);
	}
}