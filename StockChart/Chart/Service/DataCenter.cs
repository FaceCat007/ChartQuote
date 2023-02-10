/*
 * iChart����ϵͳ(��Դ)
 * �Ϻ����è��Ϣ�������޹�˾
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
    /// ������������
    /// </summary>
    public class DataCenter
    {
        /// <summary>
        /// �����
        /// </summary>
        public static UIXmlEx m_mainUI;

        /// <summary>
        /// ������
        /// </summary>
        public static SecurityService m_securityService;

        /// <summary>
        /// �������ݷ���
        /// </summary>
        public static LatestServiceClient m_latestServiceClient;

        /// <summary>
        /// ��ʷ���ݷ���
        /// </summary>
        public static HistoryServiceClient m_historyServiceClient;

        /// <summary>
        /// ���۷���
        /// </summary>
        public static PriceDataServiceClient m_priceDataServiceClient;

        /// <summary>
        /// ���ӵ�������
        /// </summary>
        /// <returns>״̬</returns>
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
        /// ��ȡ����·��
        /// </summary>
        /// <returns>����·��</returns>
        public static String getAppPath()
        {
            return Application.StartupPath;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="state">״̬</param>
        /// <returns>����״̬</returns>
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
        /// ���ݼ۸��ȡ��ɫ
        /// </summary>
        /// <param name="price">�۸�</param>
        /// <param name="comparePrice">�Ƚϼ۸�</param>
        /// <returns>��ɫ</returns>
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
        /// ��������
        /// </summary>
        /// <param name="appPath">����·��</param>
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
