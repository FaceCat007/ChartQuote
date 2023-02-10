/*
 * iChart行情系统(开源)
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using FaceCat;
using System.Text;
using System.Runtime.InteropServices;

namespace FaceCat {
    public static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(String[] args) {
            DataCenter.startService();
            while (true)
            {
                Console.WriteLine("1.导出日K线");
                Console.WriteLine("2.导出1分钟线");
                String str = Console.ReadLine();
                if (str == "1")
                {
                    DataCenter.HistoryService.fallSecurityDatas();
                }
                else if (str == "2")
                {
                    DataCenter.HistoryService.fallMinuteSecurityDatas();
                }
            }
        }
    }
}