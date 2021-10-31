using chanosBot.Actions;
using chanosBot.Core;
using chanosBot.Model;
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
    public class TelegramBot : IDisposable
    {
        #region Fields
        private const int LIMIT_LENGTH = 4096; 

        private bool disposedValue;
        #endregion

        #region Properties
        private TelegramBotClient Bot { get; set; }
        private ActionController ActionController { get; set; } 
        private AutoCommandHelper AutoCommandHelper { get; set; }
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

            AutoCommandHelper = new AutoCommandHelper();
            AutoCommandHelper.EventSendMessage += AutoCommandHelper_EventSendMessage;

            Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
                                              .WriteTo.Console()  
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

            Log.Logger.Information($"Initialize {me.FirstName} Bot.");

            IsRead = false;
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        { 
            var message = e.Message;

            if (message is null ||
                message.Type != MessageType.Text ||
                !message.Text.StartsWith("/"))
            {
                Log.Logger.Warning($"Not support this command ({message.Text})");
                return;
            } 

            try
            {
                var botResponse = ActionController.GetExecuteMessage(message.Text);

                if (botResponse.Message.Length > LIMIT_LENGTH)
                {
                    Log.Logger.Error($"Not support length of message than 4096.\n{message.Text}");
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

                    Log.Logger.Information($"Send Photo : ({botResponse.File.FileType})");
                }

                // Auto Command Options 처리
                if (botResponse.AutoCommand != null)
                {
                    botResponse.AutoCommand.ChatID = message.Chat.Id;
                    botResponse.AutoCommand.UserID = message.From.Id;

                    AutoCommandHelper.AddAutoCommand(botResponse.AutoCommand);
                    await Bot.SendTextMessageAsync(message.Chat.Id, "자동설정 되었습니다.");
                }

                Log.Logger.Information($"Send Message : ({botResponse.Message})");
            }
            catch (ArgumentException ae)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, ae.Message);
                Log.Logger.Fatal(ae, this.ToString());
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, this.ToString());
            }
        }
        #endregion

        #region Events
        private async void AutoCommandHelper_EventSendMessage(AutoCommand autoCommand)
        {
            var botResponse = autoCommand.Command.Execute(autoCommand.Options);

            await Bot.SendTextMessageAsync(autoCommand.ChatID, botResponse.Message);
            Log.Logger.Information($"Send Message : ({botResponse.Message})");
            // 파일 처리
            if (botResponse.HasFile)
            {
                switch (botResponse.File.FileType)
                {
                    case FileType.Url:
                        var input = botResponse.File as InputOnlineFile;
                        await Bot.SendPhotoAsync(autoCommand.ChatID, input, input.FileName);
                        break;
                }

                Log.Logger.Information($"Send Photo : ({botResponse.File.FileType})");
            }
        }
        #endregion


        public void Run()
        { 
            while(IsRead)
            {
                Thread.Sleep(500);
            }

            Log.Logger.Information("Run Bot.StartReceiving();");
            Bot.StartReceiving();

            AutoCommandHelper.Run();
        }

        public override string ToString() => "TelegramBot";


        #region Disposable Pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) { }

                Log.CloseAndFlush();
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
