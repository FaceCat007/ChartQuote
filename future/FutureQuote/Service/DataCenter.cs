/*
 * iChart行情系统(非开源)
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using FaceCat;

namespace future {
    /// <summary>
    /// 数据中心
    /// </summary>
    public class DataCenter {
        /// <summary>
        /// 历史数据服务
        /// </summary>
        public static HistoryService m_historyService;

        /// <summary>
        /// 最新数据服务
        /// </summary>
        public static LatestService m_latestService;

        /// <summary>
        /// 报价服务
        /// </summary>
        public static PriceDataService m_priceDataService;

        /// <summary>
        /// 获取程序路径
        /// </summary>
        /// <returns>程序路径</returns>
        public static String getAppDir() {
            return Application.StartupPath;
        }

        /// <summary>
        /// 启动http服务
        /// </summary>
        private static void startHttpService()
        {
            FCHttpMonitor httpMonitor = new FCHttpMonitor(Application.StartupPath + "\\start.js");
            FCHttpMonitor.m_easyServices.put("quote", new HttpService());
            httpMonitor.start();
        }


        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="port">端口号</param>
        public static void startService() {
            int socketID = FCServerService.startServer(9963, new byte[] { 0, 0, 0, 0});

            m_latestService = new LatestService();
            m_historyService = new HistoryService();
            m_priceDataService = new PriceDataService();
            FCServerService.addService(m_historyService);
            FCServerService.addService(m_latestService);
            FCServerService.addService(m_priceDataService);

            //启动行情服务端
            FutureService.load();
            m_latestService.setSocketID(socketID);
            m_historyService.setSocketID(socketID);
            m_priceDataService.setSocketID(socketID);
            Thread tThread = new Thread(new ThreadStart(startHttpService));
            tThread.Start();
        }
    }
}
