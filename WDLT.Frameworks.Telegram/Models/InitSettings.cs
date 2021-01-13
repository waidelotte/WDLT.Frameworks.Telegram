using Telegram.Bot.Types.Enums;

namespace WDLT.Frameworks.Telegram.Models
{
    public class InitSettings
    {
        public string Token { get; set; }
        public string WebHookUrl { get; set; }
        public long SuperAdminId { get; set; }
        public string BotName { get; set; }
        public bool IsCommandsEnabled { get; set; }
        public string ErrorMessage { get; set; }
        public ParseMode ParseMode { get; set; }
    }
}