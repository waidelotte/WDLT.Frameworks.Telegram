using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WDLT.Frameworks.Telegram.Models;

namespace WDLT.Frameworks.Telegram
{
    public class TelegramFramework
    {
        public readonly InitSettings Settings;

        private readonly TelegramBotClient _client;
        private readonly List<ITelegramCommand> _commands;

        public static readonly ChatType[] PublicChatTypes = {
            ChatType.Channel,
            ChatType.Supergroup,
            ChatType.Group
        };

        public TelegramFramework(InitSettings settings)
        {
            Settings = settings;
            _client = new TelegramBotClient(settings.Token);
            _commands = new List<ITelegramCommand>();
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery.Message.From.Id == _client.BotId)
            {
                await EchoCallback(update.CallbackQuery);
                return;
            }

            if (update.Message != null && IsCommand(update))
            {
                try
                {
                    await EchoCommand(update);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    await _client.SendTextMessageAsync(update.Message.Chat.Id, Settings.ErrorMessage);
                }
            }
        }

        private Task EchoCommand(Update update)
        {
            var textRaw = update.Message.Text;
            var argsRaw = textRaw.Split(new[] { ' '}, 2, StringSplitOptions.RemoveEmptyEntries);

            var botData = argsRaw[0].Trim().Replace("/", "").Split("@", StringSplitOptions.RemoveEmptyEntries);
            var forBot = botData.ElementAtOrDefault(1);
            var trigger = botData.First();

            if (forBot != null && !string.Equals(botData.ElementAt(1), Settings.BotName, StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;
            
            var command = _commands.FirstOrDefault(f => f.Triggers.Any(a => string.Equals(a, trigger, StringComparison.OrdinalIgnoreCase)));
            return command == null ? Task.CompletedTask : command.InvokeAsync(update, argsRaw.ElementAtOrDefault(1)?.Trim());
        }

        public Task EchoCallback(CallbackQuery callback)
        {
            if (string.IsNullOrWhiteSpace(callback.Data)) return Task.CompletedTask;

            var split = callback.Data.Split("|", StringSplitOptions.RemoveEmptyEntries);
            var data = split
                .Skip(1)
                .Select(s => s.Split("::", 2, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(d => d[0], d => d.ElementAtOrDefault(1));

            var trigger = split.ElementAtOrDefault(0);

            var command = _commands.FirstOrDefault(f => string.Equals(f.CallbackTrigger, trigger, StringComparison.OrdinalIgnoreCase));
            if (command != null)
            {
                return command.InvokeCallbackAsync(callback, data);
            }

            return Task.CompletedTask;
        }

        public void AddCommand(ITelegramCommand command)
        {
            command.SetFramework(this);
            _commands.Add(command);
        }

        public Task SetWebHookAsync(string url)
        {
            return _client.SetWebhookAsync(url);
        }

        public Task SetWebHookAsync()
        {
            return _client.SetWebhookAsync(Settings.WebHookUrl);
        }

        public Task SetBotCommands()
        {
            return _client.SetMyCommandsAsync(_commands.Where(w => !string.IsNullOrWhiteSpace(w.Description) && w.IsVisible && w.Triggers.Any()).Select(s => new BotCommand
            {
                Command = s.Triggers.First(),
                Description = s.Description
            }));
        }

        public Task SetBotCommands(IEnumerable<BotCommand> list)
        {
            return _client.SetMyCommandsAsync(list);
        }

        public static bool IsFromPublic(ChatType type)
        {
            return PublicChatTypes.Contains(type);
        }

        private static bool IsTextMessage(Update update)
        {
            return update.Message?.Type == MessageType.Text;
        }

        private static bool IsCommand(Update update)
        {
            if (string.IsNullOrEmpty(update.Message?.Text)) return false;
            return update.Message.Entities?.FirstOrDefault()?.Type == MessageEntityType.BotCommand;
        }

        public Task AnswerCallbackQueryAsync(CallbackQuery callback, string text = "⏳", int cacheTimeSec = 3)
        {
            return _client.AnswerCallbackQueryAsync(callback.Id, text, false, cacheTime: cacheTimeSec);
        }

        public static string CreateCallbackData(Dictionary<string, string> data, string trigger)
        {
            var stringData = "|" + string.Join("|", data.Select(s =>
            {
                var value = string.IsNullOrWhiteSpace(s.Value) ? "" : $"::{s.Value}";
                return $"{s.Key}{value}";
            }));
            return $"{trigger}{stringData}";
        }

        public Task<Message> SendTextAsync(TelegramMessageBuilder message, ChatId chatId)
        {
            return SendTextAsync(message.ToString(), chatId, message.GetMarkup());
        }

        public Task<Message> SendTextAsync(string message, ChatId chatId, IReplyMarkup markup = null)
        {
            return _client.SendTextMessageAsync(chatId, message, Settings.ParseMode, replyMarkup: markup);
        }
    }
}