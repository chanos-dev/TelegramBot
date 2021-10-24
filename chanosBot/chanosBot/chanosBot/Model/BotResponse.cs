using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace chanosBot.Model
{
    internal class BotResponse : IDisposable
    {
        private bool disposedValue;

        internal string Message { get; set; }

        internal bool HasFile => File != null;

        internal IInputFile File { get; set; }

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
