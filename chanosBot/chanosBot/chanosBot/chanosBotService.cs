using chanosBot.Bot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot
{
    public partial class chanosBotService : ServiceBase
    {
        private TelegramBot TelegramBot { get; set; }
        public chanosBotService()
        {
            InitializeComponent();
            InitializeControl();
        }

        private void InitializeControl()
        {            
            TelegramBot = new TelegramBot("key");
        }

        protected override void OnStart(string[] args)
        {
            TelegramBot.Run();
        }

        protected override void OnStop()
        {
            TelegramBot.Dispose();
        }
    }


}
