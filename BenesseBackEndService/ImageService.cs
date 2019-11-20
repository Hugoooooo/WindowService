using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace BenesseBackEndService
{
    public partial class ImageService : ServiceBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Timer MyTimer;
        private DateTime lastExcauteTime;
        public ImageService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            MyTimer = new Timer();
            MyTimer.Elapsed += new ElapsedEventHandler(action);
            MyTimer.Interval = 60 * 60 * 24 * 1000;
            MyTimer.Start();
        }

        protected override void OnStop()
        {
        }

        private bool checkExacute()
        {
            bool check = false;
            if (lastExcauteTime == DateTime.MinValue || new TimeSpan(DateTime.Now.Ticks - lastExcauteTime.Ticks).Days > 30)
            {
                //第一次執行
                lastExcauteTime = DateTime.Now;
                check = true;
            }
            return check;
        }

        private void action(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!checkExacute())
                {
                    return;
                }

                // 記錄錯誤日誌 
                logger.Info("暫存檔檢查開始時間:" + DateTime.Now.ToString("yyyy/mm/dd hh:mm:ss"));
                string imageFolder = ConfigurationManager.AppSettings["ImageFolder"].ToString();

                List<string> myList = new List<string>();
                // 取得資料夾內所有檔案
                foreach (string folder in Directory.GetDirectories(imageFolder))
                {
                    string tempPath = folder + "\\temp";
                    if (!Directory.Exists(tempPath))
                        continue;

                    foreach (string file in Directory.GetFiles(tempPath))
                    {
                        FileInfo f = new FileInfo(file);
                        int diff = new TimeSpan(DateTime.Now.Ticks - f.CreationTime.Ticks).Days;
                        if (diff > 7)
                        {
                            string info = "檔案名稱:" + f.Name + "建立日期:" + f.CreationTime + "\n" + "修改日期:" + f.LastWriteTime + "\n" + "存取日期:" + f.LastAccessTime;
                            logger.Info("刪除檔案資訊:" + info);
                            f.Delete();
                        }
                    }                
                }
                Console.WriteLine("Hello...");
                logger.Info("暫存檔檢查完成");
            }
            catch (Exception ex)
            {
                logger.Info("發生錯誤!錯誤資訊" + ex.ToString());
            }
        }

        #region for debug
        /// <summary>
        /// 使外部可呼叫Onstart以利Debug
        /// </summary>
        /// <param name="args"></param>
        internal void MainStart(string[] args)
        {
            OnStart(args);
        }
        /// <summary>
        /// 使外部可呼叫Stop以利Debug
        /// </summary>
        internal void MainStop()
        {
            OnStop();
        }
        #endregion
    }
}
