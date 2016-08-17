using System;
using System.Collections.Generic;
using Windows.UI.Popups;

namespace Sebastian.Toolkit.Util
{
    public class DialogDispatcher
    {
        private static readonly Queue<MessageDialog> MessageDialogs = new Queue<MessageDialog>();
        private static MessageDialog _current = null;

        public static void Show(string message)
        {
            ShowDialog(message, string.Empty);
        }

        public static void Show(string message, string title)
        {
            ShowDialog(message, title);
        }

        public static void Show(string message, string title, string debugErrorInfo)
        {
#if DEBUG
            message = $"{message}\n\nDebug error info:\n{debugErrorInfo}";
#endif
            ShowDialog(message, title);
        }

        public static void ShowDebugError(string additionalInfo)
        {
#if DEBUG
            Show("There was an error.", "Debug info", additionalInfo);
#endif
        }

        private static void ShowDialog(string text, string title)
        {
            var newDialog = new MessageDialog(text, title);
            MessageDialogs.Enqueue(newDialog);

            ShowNext();
        }

        private static async void ShowNext()
        {
            while (_current == null && MessageDialogs.Count > 0)
            {
                _current = MessageDialogs.Dequeue();
                await _current.ShowAsync();
                _current = null;
            }
        }
    }
}
