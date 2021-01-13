using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using WDLT.Utils.Extensions;

namespace WDLT.Frameworks.Telegram
{
    public class TelegramMessageBuilder
    {
        private List<List<InlineKeyboardButton>> _buttons;
        private List<List<KeyboardButton>> _keyboard;
        private readonly StringBuilder _massage;

        public TelegramMessageBuilder()
        {
            _massage = new StringBuilder();
        }

        public void AddButtons(List<List<InlineKeyboardButton>> buttons)
        {
            _buttons = buttons;
        }

        public void AddButtons(List<InlineKeyboardButton> buttons)
        {
            if (_buttons == null)
            {
                _buttons = new List<List<InlineKeyboardButton>>
                {
                    buttons
                };
            }
            else
            {
                _buttons.Add(buttons);
            }
        }

        public void AddButton(InlineKeyboardButton button)
        {
            if (_buttons == null)
            {
                _buttons = new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                        button
                    }
                };
            }
            else
            {
                _buttons.Add(new List<InlineKeyboardButton>{ button });
            }
        }

        public void AddButtonToEnd(InlineKeyboardButton button)
        {
            if (_buttons == null)
            {
                _buttons = new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                        button
                    }
                };
            }
            else
            {
                if (_buttons.Last().Count >= 8)
                {
                    _buttons.Add(new List<InlineKeyboardButton>{ button });
                }
                else
                {
                    _buttons.Last().Add(button);
                }
            }
        }

        public void AddKeyboard(List<List<KeyboardButton>> buttons)
        {
            _keyboard = buttons;
        }

        public void AddKeyboard(List<KeyboardButton> buttons)
        {
            if (_keyboard == null)
            {
                _keyboard = new List<List<KeyboardButton>>
                {
                    buttons
                };
            }
            else
            {
                _keyboard.Add(buttons);
            }
        }

        public void AddKeyboard(KeyboardButton button)
        {
            if (_keyboard == null)
            {
                _keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        button
                    }
                };
            }
            else
            {
                _keyboard.Add(new List<KeyboardButton> { button });
            }
        }

        public void AddKeyboardToEnd(KeyboardButton button)
        {
            if (_keyboard == null)
            {
                _keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        button
                    }
                };
            }
            else
            {
                if (_keyboard.Last().Count >= 8)
                {
                    _keyboard.Add(new List<KeyboardButton> { button });
                }
                else
                {
                    _keyboard.Last().Add(button);
                }
            }
        }

        public void AddLine()
        {
            _massage.AppendLine();
        }

        public void AddLine(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _massage.AppendLine();
            }
            else
            {
                _massage.AppendLine(value);
            }
        }

        public IReplyMarkup GetMarkup()
        {
            if(_buttons != null && _keyboard != null) throw new ArgumentException("Must be one: Inline Buttons or Keyboard");

            if(_buttons != null)
            {
                return new InlineKeyboardMarkup(_buttons);
            }
            else if(_keyboard != null)
            {
                return new ReplyKeyboardMarkup(_keyboard, true);
            }
            else
            {
                return null;
            }
        }

        public InlineKeyboardMarkup GetButtons()
        {
            return _buttons != null ? new InlineKeyboardMarkup(_buttons) : null;
        }

        public string ToString(int maxLenght = 4096)
        {
            return _massage.ToString().TruncateAtWord(maxLenght);
        }
    }
}