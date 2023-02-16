using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;

namespace future
{
    /// <summary>
    /// ��ʷ���ݷ���
    /// </summary>
    public class HistoryServiceClient : FCClientService {
        public HistoryServiceClient() {
            setServiceID(SERVICEID);
        }

        public const int SERVICEID = 10001;

        public const int FUNCTIONID_GETDATA = 0;

        public const int FUNCTIONID_GETCODE = 1;

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
        public void reqCodes(int requestID) {
            FCBinary bw = new FCBinary();
            bw.writeString("1");
            byte[] bytes = bw.getBytes();
            bw.close();
            int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETCODE, requestID, m_socketID, 0, 0, bytes.Length, bytes));
        }

        /// <summary>
        /// ����K��
        /// </summary>
        /// <param name="requestID">����ID</param>
        /// <param name="code">����</param>
        /// <param name="cycle">����</param>
        /// <param name="startDate">��ʼʱ��</param>
        /// <param name="endDate">����ʱ��</param>
        public void reqData(int requestID, String code, int cycle, double startDate, double endDate) {
            FCBinary bw = new FCBinary();
            bw.writeString(code);
            bw.writeInt(cycle);
            bw.writeDouble(startDate);
            bw.writeDouble(endDate);
            byte[] bytes = bw.getBytes();
            bw.close();
            int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETDATA, requestID, m_socketID, 0, 0, bytes.Length, bytes));
        }

        /// <summary>
        /// ��ȡK������
        /// </summary>
        /// <param name="code">����</param>
        /// <param name="cycle">����</param>
        /// <param name="startDate">��ʼʱ��</param>
        /// <param name="endDate">����ʱ��</param>
        /// <param name="nowDate">��ǰʱ��</param>
        /// <param name="nowVolume">��ǰ��</param>
        /// <param name="nowOpenInterest">��ǰ�ĳֲ���</param>
        /// <param name="body">����</param>
        /// <param name="bodyLength">���峤��</param>
        /// <returns>����</returns>
        public static List<SecurityData2> getDatas(ref String code, ref int cycle, ref double startDate, ref double endDate, ref double nowDate, ref double nowVolume, ref double nowOpenInterest, byte[] body, int bodyLength)
        {
            FCBinary br = new FCBinary();
            br.write(body, bodyLength);
            code = br.readString();
            cycle = br.readInt();
            startDate = br.readDouble();
            endDate = br.readDouble();
            nowDate = br.readDouble();
            nowVolume = br.readDouble();
            nowOpenInterest = br.readDouble();
            int datasSize = br.readInt();
            List<SecurityData2> datas = new List<SecurityData2>();
            for (int i = 0; i < datasSize; i++) {
                SecurityData2 data = new SecurityData2();
                data.m_date = br.readDouble();
                data.m_close = br.readDouble();
                data.m_high = br.readDouble();
                data.m_low = br.readDouble();
                data.m_open = br.readDouble();
                data.m_volume = br.readDouble();
                data.m_openInterest = br.readDouble();
                datas.Add(data);
            }
            br.close();
            return datas;
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="body">����</param>
        /// <param name="bodyLength">���峤��</param>
        /// <returns>����</returns>
        public static List<Security> getCodes(byte[] body, int bodyLength) {
            FCBinary br = new FCBinary();
            br.write(body, bodyLength);
            int datasSize = br.readInt();
            List<Security> datas = new List<Security>();
            for (int i = 0; i < datasSize; i++) {
                Security security = new Security();
                security.m_code = br.readString();
                security.m_name = br.readString();
                security.m_pingyin = br.readString();
                security.m_status = br.readInt();
                security.m_type = br.readInt();
                datas.Add(security);
            }
            br.close();
            return datas;
        }

        /// <summary>
        /// ������Ϣ����
        /// </summary>
        /// <param name="message"></param>
        public override void onReceive(FCMessage message)
        {
            base.onReceive(message);
            if (message.m_functionID == FUNCTIONID_GETDATA) {

            }
            else if (message.m_functionID == FUNCTIONID_GETCODE)
            {
                List<Security> securities = getCodes(message.m_body, message.m_bodyLength);
                DataCenter.m_securityService.load(securities);
            }
            sendToListener(message);
        }
    }
}
