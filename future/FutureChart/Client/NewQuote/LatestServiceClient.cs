using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;

namespace future {
    /// <summary>
    /// �������ݷ���
    /// </summary>
    public class LatestServiceClient : FCClientService{
        public LatestServiceClient() {
            setServiceID(SERVICEID);
        }

        public const int SERVICEID = 10000;

        /// <summary>
        /// ע�����
        /// </summary>
        public const int FUNCTION_SUBCODES = 0;

        /// <summary>
        /// ��ע�����
        /// </summary>
        public const int FUNCTION_UNSUBCODES = 1;

        /// <summary>
        /// ��������
        /// </summary>
        public const int FUNCTION_NEWDATA = 2;

        private int m_socketID;

        /// <summary>
        /// ��ȡ�׽���
        /// </summary>
        /// <returns></returns>
        public int getSocketID() {
            return m_socketID;
        }

        /// <summary>
        /// �����׽���
        /// </summary>
        /// <param name="value"></param>
        public void setSocketID(int value) {
            m_socketID = value;
        }

        /// <summary>
        /// ע�����
        /// </summary>
        /// <param name="requestID">����ID</param>
        /// <param name="codes">����</param>
        public void subCodes(int requestID, String codes) {
            FCBinary bw = new FCBinary();
            bw.writeString(codes);
            byte[] bytes = bw.getBytes();
            int ret = send(new FCMessage(getServiceID(), FUNCTION_SUBCODES, requestID, m_socketID, 0, 0, bytes.Length, bytes));
            bw.close();
            Console.WriteLine("1");
        }

        /// <summary>
        /// ��ע�����
        /// </summary>
        /// <param name="requestID">����ID</param>
        public void unSubCodes(int requestID) {
            FCBinary bw = new FCBinary();
            bw.writeString("1");
            byte[] bytes = bw.getBytes();
            int ret = send(new FCMessage(getServiceID(), FUNCTION_UNSUBCODES,  requestID, m_socketID, 0, 0, bytes.Length, bytes));
            bw.close();
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="body">����</param>
        /// <param name="bodyLength">���峤��</param>
        /// <returns>��������</returns>
        public static SecurityLatestData getLatestData(byte[] body, int bodyLength) {
            FCBinary br = new FCBinary();
            br.write(body, bodyLength);
            SecurityLatestData latestData = new SecurityLatestData();
            latestData.m_code = br.readString();
            latestData.m_open = br.readDouble();
            latestData.m_lastClose = br.readDouble();
            latestData.m_close = br.readDouble();
            latestData.m_high = br.readDouble();
            latestData.m_low = br.readDouble();
            latestData.m_volume = br.readDouble();
            latestData.m_amount = br.readDouble();
            latestData.m_buyVolume1 = br.readInt();
            latestData.m_buyPrice1 = br.readDouble();
            latestData.m_buyVolume2 = br.readInt();
            latestData.m_buyPrice2 = br.readDouble();
            latestData.m_buyVolume3 = br.readInt();
            latestData.m_buyPrice3 = br.readDouble();
            latestData.m_buyVolume4 = br.readInt();
            latestData.m_buyPrice4 = br.readDouble();
            latestData.m_buyVolume5 = br.readInt();
            latestData.m_buyPrice5 = br.readDouble();
            latestData.m_sellVolume1 = br.readInt();
            latestData.m_sellPrice1 = br.readDouble();
            latestData.m_sellVolume2 = br.readInt();
            latestData.m_sellPrice2 = br.readDouble();
            latestData.m_sellVolume3 = br.readInt();
            latestData.m_sellPrice3 = br.readDouble();
            latestData.m_sellVolume4 = br.readInt();
            latestData.m_sellPrice4 = br.readDouble();
            latestData.m_sellVolume5 = br.readInt();
            latestData.m_sellPrice5 = br.readDouble();
            latestData.m_innerVol = br.readInt();
            latestData.m_outerVol = br.readInt();
            latestData.m_turnoverRate = br.readDouble();
            latestData.m_openInterest = br.readDouble();
            latestData.m_settlePrice = br.readDouble();
            latestData.m_date = br.readDouble();
            br.close();
            //Console.WriteLine(JsonConvert.SerializeObject(latestData) + "\r\n");
            return latestData;
        }

        /// <summary>
        /// ������Ϣ����
        /// </summary>
        /// <param name="message">��Ϣ</param>
        public override void onReceive(FCMessage message) {
            base.onReceive(message);
            if (message.m_functionID == FUNCTION_NEWDATA) {
               
            }
            sendToListener(message);
        }
    }
}
