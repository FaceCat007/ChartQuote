/*
 * iChart����ϵͳ(��Դ)
 * �Ϻ����è��Ϣ�������޹�˾
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FaceCat {
    /// <summary>
    /// ��������
    /// </summary>
    public class DataCenter {
        private static HistoryService m_historyService;

        public static HistoryService HistoryService {
            get { return DataCenter.m_historyService; }
            set { DataCenter.m_historyService = value; }
        }

        private static LatestService m_latestService;

        public static LatestService LatestService {
            get { return DataCenter.m_latestService; }
            set { DataCenter.m_latestService = value; }
        }

        private static PriceDataService m_priceDataService;

        public static PriceDataService PriceDataService {
            get { return DataCenter.m_priceDataService; }
            set { DataCenter.m_priceDataService = value; }
        }

        /// <summary>
        /// ����http����
        /// </summary>
        private static void startHttpService()
        {
            FCHttpMonitor httpMonitor = new FCHttpMonitor(Application.StartupPath + "\\start.js");
            FCHttpMonitor.m_easyServices.put("quote", new HttpService());
            httpMonitor.start();
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="port">�˿ں�</param>
        public static void startService() {
            String dayDir = Application.StartupPath + "\\day";
            String minuteDir = Application.StartupPath + "\\minute";
            if (!FCFile.isDirectoryExist(dayDir))
            {
                FCFile.createDirectory(dayDir);
            }
            if (!FCFile.isDirectoryExist(minuteDir))
            {
                FCFile.createDirectory(minuteDir);
            }
            int socketID = FCServerService.startServer(9957, new byte[] { 0, 0, 0, 0 });
            FCServerSockets.setLimit(socketID, 100, 5 * 60 * 1000);

            m_latestService = new LatestService();
            m_historyService = new HistoryService();
            m_priceDataService = new PriceDataService();
            FCServerService.addService(m_historyService);
            FCServerService.addService(m_latestService);
            FCServerService.addService(m_priceDataService);

            //������������
            StockService.load();
            StockService.start();
            m_latestService.setSocketID(socketID);
            m_historyService.setSocketID(socketID);
            m_priceDataService.setSocketID(socketID);
            Thread tThread = new Thread(new ThreadStart(startHttpService));
            tThread.Start();
        }
    }
}
