using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace WDLT.Frameworks.Telegram
{
    public interface ITelegramCommand
    {
        List<string> Triggers { get; set; }
        string CallbackTrigger { get; set; }
        string Description { get; set; }
        bool IsPublic { get; set; }
        bool IsVisible { get; set; }

        Task InvokeAsync(Update update, string textWithoutCommand = null);
        Task InvokeCallbackAsync(CallbackQuery callbackQuery, Dictionary<string, string> data);
        void SetFramework(TelegramFramework framework);
    }
}