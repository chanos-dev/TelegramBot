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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
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

        private string LogPath => $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Bot\Log";
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
            Bot.OnUpdate += Bot_OnUpdate;

            Log.Logger.Information($"Initialize {me.FirstName} Bot.");

            IsRead = false;
        }

        private async void Bot_OnUpdate(object sender, UpdateEventArgs e)
        {
            var update = e.Update;

            // reply data
            if (update.CallbackQuery != null)
            {
                var message = update.CallbackQuery.Message;

                try
                { 
                    await SendMessageToTelegram(ActionController.ReplyExecuteMessage(update.CallbackQuery.Data), message, isEdit: true);
                    Log.Logger.Information($"Reply CallBack Data : ({update.CallbackQuery.Data})");
                }
                catch (ArgumentException ae)
                {
                    await Bot.SendTextMessageAsync(message.Chat.Id, ae.Message);
                    Log.Logger.Fatal(ae, this.ToString());
                }
                catch (WebException we)
                {
                    await Bot.SendTextMessageAsync(message.Chat.Id, we.Message);
                    Log.Logger.Fatal(we, this.ToString());
                }
                catch (Exception ex)
                {
                    Log.Logger.Fatal(ex, this.ToString());
                }
            } 
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
                await SendMessageToTelegram(ActionController.GetExecuteMessage(message.Text), message);
            }
            catch (ArgumentException ae)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, ae.Message);
                Log.Logger.Fatal(ae, this.ToString());
            }
            catch (WebException we)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, we.Message);
                Log.Logger.Fatal(we, this.ToString());
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, this.ToString());
            }
        }

        private async Task SendMessageToTelegram(BotResponse botResponse, Message message, bool isEdit = false)
        {
            if (botResponse.Message.Length > LIMIT_LENGTH)
            {
                Log.Logger.Error($"Not support length of message than 4096.\n{message.Text}");
                await Bot.SendTextMessageAsync(message.Chat.Id, "지원되지 않는 텍스트 길이 입니다.");
                return;
            }


            if (botResponse.Keyboard != null)
            {
                if (isEdit)
                    await Bot.EditMessageTextAsync(message.Chat.Id, message.MessageId, botResponse.Message, replyMarkup: botResponse.Keyboard);
                else
                    await Bot.SendTextMessageAsync(message.Chat.Id, botResponse.Message, replyMarkup: botResponse.Keyboard);

                Log.Logger.Information($"Send replyMarkup : ({botResponse.Keyboard.InlineKeyboard.Sum(s => s.Count())})");
            }
            // NOTE: 자동설정 커맨드가 있으면 설정 메시지만 보내기
            else if (botResponse.AutoCommand != null)
            {
                botResponse.AutoCommand.ChatID = message.Chat.Id;
                botResponse.AutoCommand.UserID = message.From.Id;

                var msg = AutoCommandHelper.AddAutoCommand(botResponse.AutoCommand);

                await Bot.SendTextMessageAsync(message.Chat.Id, msg);

                Log.Logger.Information($"Set AutoCommand : ({botResponse.AutoCommand})");
            }
            else
            {
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
                        case FileType.Stream:
                            var stream = botResponse.File as InputFileStream;
                            InputOnlineFile file = new InputOnlineFile(stream.Content);
                            file.Content.Position = 0;
                            await Bot.SendPhotoAsync(message.Chat.Id, file);
                            break;
                    }

                    Log.Logger.Information($"Send Photo : ({botResponse.File.FileType})");
                }

                Log.Logger.Information($"Send Message : ({botResponse.Message})");
            } 
        }

        #endregion

        #region Events
        // TODO : Bot_OnMessage메서드와 Refactoring 필요
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
