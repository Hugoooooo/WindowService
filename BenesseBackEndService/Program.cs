using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BenesseBackEndService
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        static void Main()
        {
            //若此組件以Windows Service執行時，則該屬性回傳false
            if (Environment.UserInteractive)
            {
                //以 Console Application 執行
                ImageService s = new ImageService();
                s.MainStart(null);

                Console.WriteLine("服務已啟動，請按下 Enter 鍵關閉服務...");
                // 必須要透過Console.ReadLine(); 先停止程式執行
                //因為Windows Service 大多是利用多Thread或Timer 執行長時間的工作
                //所以雖然主執行緒停止執行了，但服務中的執行緒已經在運行了!
                Console.ReadLine();

                s.MainStop();

                Console.WriteLine("服務已關閉");
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new ImageService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
