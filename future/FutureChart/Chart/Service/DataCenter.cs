/*
 * iChart行情系统(非开源)
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using FaceCat;

namespace future {
    /// <summary>
    /// 处理行情数据
    /// </summary>
    public class DataCenter {
        /// <summary>
        /// 主界面
        /// </summary>
        public static UIXmlEx m_mainUI;

        /// <summary>
        /// 证券服务
        /// </summary>
        public static SecurityService m_securityService = new SecurityService();

        /// <summary>
        /// 最新数据服务
        /// </summary>
        public static LatestServiceClient m_latestServiceClient = new LatestServiceClient();

        /// <summary>
        /// 历史数据服务
        /// </summary>
        public static HistoryServiceClient m_historyServiceClient = new HistoryServiceClient();

        /// <summary>
        /// 报价服务
        /// </summary>
        public static PriceDataServiceClient m_priceDataServiceClient = new PriceDataServiceClient();

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <returns>状态</returns>
        public static int connect() {
            String ip = "127.0.0.1", proxyIP = "", proxyUserName = "", proxyPwd = "", proxyDomain = "";
            int port = 9963;
            int socketID = FCClientService.connectToServer(0, ip, port, proxyIP, 0, proxyUserName, proxyPwd, proxyDomain, 6, new byte[] { 0, 0, 0, 0});
            if (socketID > 0) {
                m_latestServiceClient.setSocketID(socketID);
                m_historyServiceClient.setSocketID(socketID);
                m_priceDataServiceClient.setSocketID(socketID);
            }
            return socketID;
        }

        /// <summary>
        /// 根据价格获取颜色
        /// </summary>
        /// <param name="price">价格</param>
        /// <param name="comparePrice">比较价格</param>
        /// <returns>颜色</returns>
        public static long getPriceColor(double price, double comparePrice)
        {
            if (price != 0)
            {
                if (price > comparePrice)
                {
                    return MyColor.USERCOLOR69;
                }
                else if (price < comparePrice)
                {
                    return MyColor.USERCOLOR70;
                }
            }
            return MyColor.USERCOLOR3;
        }

        /// <summary>
        /// 获取程序路径
        /// </summary>
        /// <returns>程序路径</returns>
        public static String getAppPath() {
            return Application.StartupPath;
        }

        /// <summary>
        /// 获取用户目录
        /// </summary>
        /// <returns>用户目录</returns>
        public static String getUserPath() {
            String userPath = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            if (!FCFile.isDirectoryExist(userPath)) {
                userPath = getAppPath();
            }
            else {
                userPath += "\\future";
                if (!FCFile.isDirectoryExist(userPath)) {
                    FCFile.createDirectory(userPath);
                }
            }
            return userPath;

        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>加载状态</returns>
        public static int loadData(int state) {
            m_historyServiceClient.reqCodes(FCClientService.getRequestID());
            if (m_mainUI != null) {
                m_mainUI.loadData();
            }
            return 0;
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="appPath">程序路径</param>
        public static void startService() {

            FCClientService.addService(m_latestServiceClient);
            FCClientService.addService(m_historyServiceClient);
            FCClientService.addService(m_priceDataServiceClient);
            //List<FCClientService> services = new List<FCClientService>();
            //FCClientService.GetServices(services);
            //int servicesSize = services.Count;
            //for (int i = 0; i < servicesSize; i++)
            //{
            //    FCClientService service = services[i];
            //    service.getCompressType() = FCClientService.COMPRESSTYPE_NONE;
            //}
        }
    }
}
