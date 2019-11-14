using BotX.Api.JsonModel.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.StateMachine
{
    /// <summary>
    /// Реализует конечный автомат для ведения диалога с пользователем
    /// </summary>
    public class BaseStateMachine
    {
        private BaseState state;

        [JsonProperty]
        private BaseState State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
                if (value != null)
                    state.SetContext(this);
            }
        }

        [JsonProperty]
        private BaseState firstStep;

        [JsonProperty]
        internal bool isFinished;

        [JsonProperty]
        internal dynamic model = new object();

        /// <summary>
        /// Срабатывает при переходе в завершающее состояние конечного автомата
        /// </summary>
        public event EventHandler<FinishedEventArgs> OnFinished;

        /// <summary>
        /// Загружает конечный автомат из ранее сериализованного в json состояния
        /// </summary>
        /// <param name="value">JSON-строка</param>
        /// <param name="messageSender">Отправщик сообщений Express</param>
        /// <returns></returns>
        public static BaseStateMachine FromJson(string value, BotMessageSender messageSender)
        {
            var restoredStage = JsonConvert.DeserializeObject<BaseStateMachine>(value, serializerSettings);
            restoredStage.MessageSender = messageSender;
            return restoredStage;
        }

        [JsonIgnore]
        public BotMessageSender MessageSender { get; set; }

        [JsonConstructor]
        private BaseStateMachine()
        {
        }

        /// <summary>
        /// Создаёт конечный автомат с начальным состоянием, указанным в качестве аргументоа
        /// </summary>
        /// <param name="initialState">Начальное состояние автомата</param>
        /// <param name="messageSender">Отправщик сообщений Express</param>
        public BaseStateMachine(BaseState initialState, BotMessageSender messageSender)
        {
            firstStep = initialState;
            MessageSender = messageSender;
            State = initialState;
        }

        /// <summary>
        /// Переводит конечный автомат в новое состояние
        /// </summary>
        /// <param name="newState"></param>
        /// <param name="userMessage"></param>
        /// <returns></returns>
        public async Task TransitionToAsync(BaseState newState, UserMessage userMessage)
        {
            State = newState;
            await State.StartAsync(userMessage, model);
        }

        /// <summary>
        /// Возвращает конечный автомат в исходное состояние
        /// </summary>
        /// <param name="userMessage"></param>
        /// <returns></returns>
        public async Task ResetAsync(UserMessage userMessage)
        {
            await TransitionToAsync(firstStep, userMessage);
        }

        /// <summary>
        /// Завершает конечный автомат
        /// </summary>
        public void Finish(UserMessage userMessage)
        {
            isFinished = true;
            OnFinished?.Invoke(this,
                new FinishedEventArgs 
                { 
                    Model = model,
                    Message = userMessage
                });
        }

        /// <summary>
        /// Выполняет логику актуального состояния для конечного автомата
        /// </summary>
        /// <param name="userMessage">Входящее сообщение от пользователя</param>
        /// <returns></returns>
        public async Task EnterAsync(UserMessage userMessage)
        {
            if (!isFinished)
                await State.StartAsync(userMessage, model);
        }

        /// <summary>
        /// Сериализует состояние автомата в Json
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, serializerSettings);
        }

        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        };
    }
}
