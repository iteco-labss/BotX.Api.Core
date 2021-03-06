﻿using BotX.Api.JsonModel.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BotX.Api.StateMachine
{
	/// <summary>
	/// Реализует конечный автомат для ведения диалога с пользователем
	/// </summary>
	public abstract class BaseStateMachine
	{
		protected readonly IServiceScopeFactory ScopeFactory;
		private BaseState state;

		/// <summary>
		/// Создаёт конечный автомат с начальным состоянием, указанным в качестве аргументоа
		/// </summary>
		/// <param name="messageSender">Отправщик сообщений Express</param>
		protected BaseStateMachine(IBotMessageSender messageSender, IServiceScopeFactory scopeFactory)
		{
			MessageSender = messageSender;
			ScopeFactory = scopeFactory;
		}

		[JsonProperty]
		internal BaseState State
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
		internal BaseState firstStep;

		[JsonProperty]
		internal bool isFinished;

		[JsonProperty]
		public dynamic Model { get; set; } = new ExpandoObject();

		//TODO: сделать init
		//public static T Create<T>() where T : BaseStateMachine
		//{
		//	var messageSender = ExpressBotService.Configuration.ServiceProvider.GetService<BotMessageSender>();
		//	var 
		//}

		/// <summary>
		/// Загружает конечный автомат из ранее сериализованного в json состояния
		/// </summary>
		/// <param name="value">JSON-строка</param>
		/// <param name="messageSender">Отправщик сообщений Express</param>
		/// <returns></returns>
		public T FromJson<T>(string value, IBotMessageSender messageSender, UserMessage userMessage) where T : BaseStateMachine
		{
			var restoredObject = JsonConvert.DeserializeObject(value, serializerSettings);
			if (!(restoredObject is T restoredSm))
				return null;

			using var scope = ScopeFactory.CreateScope();
			var diSm = scope.ServiceProvider.GetService<T>();
			if (diSm != null)
			{
				diSm.firstStep = restoredSm.firstStep;
				diSm.isFinished = restoredSm.isFinished;
				diSm.MessageSender = messageSender;
				diSm.Model = restoredSm.Model;
				diSm.state = restoredSm.state;
				diSm.state.StateMachine = diSm;
				diSm.UserMessage = userMessage;
				return diSm;
			}

			restoredSm.MessageSender = messageSender;
			restoredSm.UserMessage = userMessage;
			return restoredSm;
		}

		/// <summary>
		/// Отправщик сообщений в чат
		/// </summary>
		[JsonIgnore]
		public IBotMessageSender MessageSender { get; set; }

		/// <summary>
		/// Входящее сообщение от пользователя
		/// </summary>
		[JsonIgnore]
		public UserMessage UserMessage { get; internal set; }

		[JsonConstructor]
		internal BaseStateMachine()
		{
		}



		/// <summary>
		/// Вызывается при старте с возможностью задать через <see cref="TransitionToAsync{TState}"/> начальное состояние
		/// </summary>
		/// <returns></returns>
		public abstract Task OnStartedAsync();

		/// <summary>
		/// Реализует логику обработки события завершения конечного автомата
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public abstract Task OnFinishedAsync();

		/// <summary>
		/// Переводит конечный автомат в новое состояние
		/// </summary>
		/// <returns></returns>
		public async Task TransitionToAsync<TState>() where TState : BaseState
		{
			if (isFinished)
				return;

			using var scope = ScopeFactory.CreateScope();
			var newState = scope.ServiceProvider.GetService(typeof(TState)) as BaseState;
			State = newState;
			if (firstStep == null)
				firstStep = newState;
			await State.StartAsync(UserMessage);
			SaveState(); // TODO узнать почему при переходе в новое состояние, мы не вызываем SaveSate
		}

		/// <summary>
		/// Возвращает конечный автомат в исходное состояние
		/// </summary>
		/// <returns></returns>
		public async Task ResetAsync()
		{
			State = firstStep;
			State.ResetState();
			isFinished = false;
			Model = new ExpandoObject();
			await State.StartAsync(UserMessage);
			SaveState(); // TODO узнать почему при переходе в новое состояние, мы не вызываем SaveSate
		}

		/// <summary>
		/// Завершает конечный автомат
		/// </summary>
		public void Finish()
		{
			isFinished = true;
			OnFinishedAsync();
			SaveState();
		}

		/// <summary>
		/// Отменяет стеймашину, не вызывая событий завершения
		/// </summary>
		public void Cancel()
		{
			isFinished = true;
			SaveState();
		}

		/// <summary>
		/// Выполняет логику актуального состояния для конечного автомата
		/// </summary>
		/// <param name="userMessage">Входящее сообщение от пользователя</param>
		/// <returns></returns>
		public async Task EnterAsync(UserMessage userMessage)
		{
			UserMessage = userMessage;
			if (!isFinished)
			{
				if (State == null)
					await OnStartedAsync();
				else
					await State.StartAsync(userMessage);

				this.SaveState();
			}
		}

		/// <summary>
		/// Сериализует состояние автомата в Json
		/// </summary>
		/// <returns></returns>
		public string ToJson()
		{
			return JsonConvert.SerializeObject((BaseStateMachine)this, serializerSettings);
		}

		private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.All,
			MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
		};


		/// <summary>
		/// Вызывается при инициализации конечного автомата и ожидает восстановление состояния из ранее сохранённого
		/// <code>В данном методе необходимо восстановить состояние конечного автомата используя метод
		/// <see cref="BaseStateMachine.FromJson(string, BotMessageSender)"/></code>
		/// </summary>
		/// <returns></returns>
		public abstract BaseStateMachine RestoreState();

		/// <summary>
		/// Вызывется при завершении обработки текущего состояния конечного автомта и ожидает сохранение его состояния
		/// <code>В данном методе необходимо сохранить состояние конечнго автомата, используя <see cref="BaseStateMachine.SaveState(BaseStateMachine)"/></code>
		/// </summary>
		public abstract void SaveState();
	}
}
