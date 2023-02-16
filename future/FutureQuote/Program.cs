/*
 * iChart行情系统(非开源)
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
using FaceCat;

namespace future {

    public static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(String[] args) {
            try
            {
                DataCenter.startService();
            }
            catch (Exception ex)
            {
                Console.WriteLine("1");
            }
            Console.ReadLine();
        }
    }
}