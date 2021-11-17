using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace chanosBot.Model
{
    public class BotResponse : IDisposable
    { 
        private bool disposedValue;

        internal string Message { get; set; } = string.Empty;

        internal bool HasFile => File != null;        

        /// <summary>
        /// 전송 이미지
        /// </summary>
        internal IInputFile File { get; set; }

        /// <summary>
        /// 커맨드 자동설정
        /// </summary>
        internal AutoCommand AutoCommand { get; set; }

        /// <summary>
        /// 커맨드 버튼
        /// </summary>
        internal InlineKeyboardMarkup Keyboard { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) { }

                if (File is InputFileStream fileStream)
                    fileStream?.Content?.Dispose();

                disposedValue = true;
            }
        }

        ~BotResponse()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
