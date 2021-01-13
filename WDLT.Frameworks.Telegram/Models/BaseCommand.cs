using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WDLT.Frameworks.Telegram.Models
{
    public abstract class BaseCommand: ITelegramCommand
    {
        public List<string> Triggers { get; set; }
        public string CallbackTrigger { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public bool IsVisible { get; set; }

        protected TelegramFramework Framework;

        protected BaseCommand()
        {
            IsPublic = true;
            IsVisible = true;
            Triggers = new List<string>();
        }

        public void SetFramework(TelegramFramework framework)
        {
            Framework = framework;
        }

        protected virtual Task ExecuteCommandAsync(Message message, string textWithoutCommand)
        {
            return Task.CompletedTask;
        }

        protected virtual Task ExecuteCallbackAsync(Dictionary<string, string> data)
        {
            return Task.CompletedTask;
        }

        private bool CanExecute(int fromId, ChatType type)
        {
            if (fromId == Framework.Settings.SuperAdminId) return true;
            if (!Framework.Settings.IsCommandsEnabled) return false;
            if (TelegramFramework.IsFromPublic(type) && !IsPublic) return false;

            return true;
        }

        public Task InvokeAsync(Update update, string textWithoutCommand = null)
        {
            return !CanExecute(update.Message.From.Id, update.Message.Chat.Type) ? Task.CompletedTask : ExecuteCommandAsync(update.Message, textWithoutCommand);
        }

        public Task InvokeCallbackAsync(CallbackQuery callbackQuery, Dictionary<string, string> data)
        {
            return !CanExecute(callbackQuery.Message.From.Id, callbackQuery.Message.Chat.Type) ? Task.CompletedTask : ExecuteCallbackAsync(data);
        }
    }
}