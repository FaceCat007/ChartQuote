/*
 * iChart行情系统(开源)
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


namespace chart
{
    /// <summary>
    /// 处理行情数据
    /// </summary>
    public class DataCenter
    {
        /// <summary>
        /// 主框架
        /// </summary>
        public static UIXmlEx m_mainUI;

        /// <summary>
        /// 码表服务
        /// </summary>
        public static SecurityService m_securityService;

        /// <summary>
        /// 最新数据服务
        /// </summary>
        public static LatestServiceClient m_latestServiceClient;

        /// <summary>
        /// 历史数据服务
        /// </summary>
        public static HistoryServiceClient m_historyServiceClient;

        /// <summary>
        /// 报价服务
        /// </summary>
        public static PriceDataServiceClient m_priceDataServiceClient;

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <returns>状态</returns>
        public static int connect()
        {
            String ip = "127.0.0.1", proxyIP = "", proxyUserName = "", proxyPwd = "", proxyDomain = "";
            int port = 9957;
            int socketID = FCClientService.connectToServer(0, ip, port, proxyIP, 0, proxyUserName, proxyPwd, proxyDomain, 6, new byte[] { 0, 0, 0, 0 });
            if (socketID > 0)
            {
                m_latestServiceClient.setSocketID(socketID);
                m_historyServiceClient.setSocketID(socketID);
                m_priceDataServiceClient.setSocketID(socketID);
            }
            return socketID;
        }

        /// <summary>
        /// 获取程序路径
        /// </summary>
        /// <returns>程序路径</returns>
        public static String getAppPath()
        {
            return Application.StartupPath;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>加载状态</returns>
        public static int loadData(int state)
        {
            m_historyServiceClient.reqCodes(FCClientService.getRequestID());
            m_historyServiceClient.reqDevideDatas(FCClientService.getRequestID());
            if (m_mainUI != null)
            {
                m_mainUI.loadData();
            }
            return 0;
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
        /// 开启服务
        /// </summary>
        /// <param name="appPath">程序路径</param>
        public static void startService()
        {
            if (m_securityService == null)
            {
                m_securityService = new SecurityService();
            }
            if (m_latestServiceClient == null)
            {
                m_latestServiceClient = new LatestServiceClient();
                FCClientService.addService(m_latestServiceClient);
            }
            if (m_historyServiceClient == null)
            {
                m_historyServiceClient = new HistoryServiceClient();
                FCClientService.addService(m_historyServiceClient);
            }
            if (m_priceDataServiceClient == null)
            {
                m_priceDataServiceClient = new PriceDataServiceClient();
                FCClientService.addService(m_priceDataServiceClient);
            }
        }
    }
}
