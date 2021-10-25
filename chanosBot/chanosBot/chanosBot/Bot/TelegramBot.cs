using chanosBot.Actions;
using chanosBot.Core;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace chanosBot.Bot
{
    internal class TelegramBot : IDisposable
    {
        #region Fields
        private const int LIMIT_LENGTH = 4096; 

        private bool disposedValue;
        #endregion

        #region Properties
        private Logger Logger { get; set; }
        private TelegramBotClient Bot { get; set; }
        private ActionController ActionController { get; set; } 
        private string Token { get; set; }
        private bool IsRead { get; set; } = true;

        private string LogPath => $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Bot";
        private string LogFileName => "TelegramBot-chan.log";
        #endregion

        #region Constructor
        public TelegramBot(string token)
        {
            this.Token = token;

            Bot = new TelegramBotClient(token);

            ActionController = new ActionController();

            Logger = new LoggerConfiguration().MinimumLevel.Information()
                                              .WriteTo.File(Path.Combine(LogPath, LogFileName),
                                              rollingInterval: RollingInterval.Hour,
                                              rollOnFileSizeLimit: true,
                                              fileSizeLimitBytes: ByteHelper.GetMBToByte(10))
                                              .CreateLogger();

            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);

            Initialize();
        }
        #endregion

        #region Initialize
        private async void Initialize()
        {
            var me = await Bot.GetMeAsync(); 

            Bot.OnMessage += Bot_OnMessage;

            Logger.Information($"Initialize {me.FirstName} Bot.");

            IsRead = false;
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        { 
            var message = e.Message;

            if (message is null ||
                message.Type != MessageType.Text ||
                !message.Text.StartsWith("/"))
            {
                Logger.Warning($"Not support this command ({message.Text})");
                return;
            } 

            try
            {
                var botResponse = ActionController.GetExecuteMessage(message.Text);

                if (botResponse.Message.Length > LIMIT_LENGTH)
                {
                    Logger.Error($"Not support length of message than 4096.\n{message.Text}");
                    await Bot.SendTextMessageAsync(message.Chat.Id, "지원되지 않는 텍스트 길이 입니다.");
                    return;
                }

                await Bot.SendTextMessageAsync(message.Chat.Id, botResponse.Message);

                // 파일 처리
                if (botResponse.HasFile)
                {
                    switch (botResponse.File.FileType)
                    {
                        case FileType.Url:
                            var input = botResponse.File as InputOnlineFile;
                            await Bot.SendPhotoAsync(message.Chat.Id, input, input.FileName);
                            break;
                    }                    
                }

                // Auto Command Options 처리
                if (botResponse.AutoCommand != null)
                {

                }


                Logger.Information($"Send Message : ({botResponse.Message})");
            }
            catch (ArgumentException ae)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, ae.Message);
                Logger.Fatal(ae, this.ToString());
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, this.ToString());
            }
        }
        #endregion


        internal void Run()
        { 
            while(IsRead)
            {
                Thread.Sleep(500);
            }

            Logger.Information("Run Bot.StartReceiving();");
            Bot.StartReceiving();
        }

        public override string ToString() => "TelegramBot";


        #region Disposable Pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) { }

                Logger?.Dispose();
                disposedValue = true;
            }
        }

        ~TelegramBot()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
