using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;

namespace chart {
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

        public const int FUNCTIONID_GETDR = 2;

        /// <summary>
        /// ��Ȩ����
        /// </summary>
        public Dictionary<String, List<List<OneDivideRightBase>>> m_devideRightDatas = new Dictionary<string, List<List<OneDivideRightBase>>>();

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
        /// ��ȡ��Ȩ����
        /// </summary>
        /// <param name="code">����</param>
        /// <returns>��Ȩ����</returns>
        public List<List<OneDivideRightBase>> getDivideRight(String code)
        {
            if (m_devideRightDatas.ContainsKey(code))
            {
                return m_devideRightDatas[code];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// �������
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
        /// ��������
        /// </summary>
        /// <param name="requestID">����ID</param>
        /// <param name="code">����</param>
        /// <param name="cycle">����</param>
        /// <param name="startDate">��ʼ����</param>
        /// <param name="endDate">��������</param>
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
        /// ����Ȩ����
        /// </summary>
        /// <param name="requestID">����ID</param>
        public void reqDevideDatas(int requestID)
        {
            FCBinary bw = new FCBinary();
            bw.writeInt(0);
            byte[] bytes = bw.getBytes();
            bw.close();
            int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETDR, requestID, m_socketID, 0, 0, bytes.Length, bytes));
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="code">����</param>
        /// <param name="cycle">����</param>
        /// <param name="startDate">��ʼ����</param>
        /// <param name="endDate">��������</param>
        /// <param name="nowDate">��ǰ����</param>
        /// <param name="nowVolume">��ǰ�ɽ���</param>
        /// <param name="nowAmount">��ǰ�ɽ���</param>
        /// <param name="body">����</param>
        /// <param name="bodyLength">���峤��</param>
        /// <returns></returns>
        public static List<SecurityData> getDatas(ref String code, ref int cycle, ref double startDate, ref double endDate, ref double nowDate, ref double nowVolume, ref double nowAmount, byte[] body, int bodyLength) {
            FCBinary br = new FCBinary();
            br.write(body, bodyLength);
            code = br.readString();
            cycle = br.readInt();
            startDate = br.readDouble();
            endDate = br.readDouble();
            nowDate = br.readDouble();
            nowVolume = br.readDouble();
            nowAmount = br.readDouble();
            int datasSize = br.readInt();
            List<SecurityData> datas = new List<SecurityData>();
            for (int i = 0; i < datasSize; i++) {
                SecurityData data = new SecurityData();
                data.m_date = br.readDouble();
                data.m_close = br.readDouble();
                data.m_high = br.readDouble();
                data.m_low = br.readDouble();
                data.m_open = br.readDouble();
                data.m_volume = br.readDouble();
                data.m_amount = br.readDouble();
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
        /// <returns>֤ȯ�б�</returns>
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
                security.m_status = br.readString();
                security.m_market = br.readString();
                datas.Add(security);
            }
            br.close();
            return datas;
        }

        /// <summary>
        /// ��ȡ��Ȩ����
        /// </summary>
        /// <param name="body">����</param>
        /// <param name="bodyLength">���峤��</param>
        /// <returns>��Ȩ����</returns>
        public static Dictionary<String, List<List<OneDivideRightBase>>> getDevideDatas(byte[] body, int bodyLength)
        {
            Dictionary<String, List<List<OneDivideRightBase>>> datas = new Dictionary<string, List<List<OneDivideRightBase>>>();
            FCBinary br = new FCBinary();
            br.write(body, bodyLength);
            int datasSize = br.readInt();
            for (int i = 0; i < datasSize; i++)
            {
                List<List<OneDivideRightBase>> oneDatas = new List<List<OneDivideRightBase>>();
                String code = br.readString();
                int oneDatasSize = br.readInt();
                for (int j = 0; j < oneDatasSize; j++)
                {
                    int oneDatasSize2 = br.readInt();
                    List<OneDivideRightBase> oneDatas2 = new List<OneDivideRightBase>();
                    for (int x = 0; x < oneDatasSize2; x++)
                    {
                        OneDivideRightBase oneDivideRightBase = new OneDivideRightBase();
                        oneDivideRightBase.Date = br.readDouble();
                        oneDivideRightBase.DataValue = br.readFloat();
                        oneDivideRightBase.DivideType = (DivideRightType)Enum.Parse(typeof(DivideRightType), br.readString());
                        oneDivideRightBase.PClose = br.readFloat();
                        oneDivideRightBase.Factor = br.readFloat();
                        oneDatas2.Add(oneDivideRightBase);
                    }
                    oneDatas.Add(oneDatas2);
                }
                datas[code] = oneDatas;
            }
            return datas;
        }

        /// <summary>
        /// ������Ϣ����
        /// </summary>
        /// <param name="message">��Ϣ</param>
        public override void onReceive(FCMessage message)
        {
            base.onReceive(message);
            if (message.m_functionID == FUNCTIONID_GETDATA) {

            } else if (message.m_functionID == FUNCTIONID_GETCODE) {
                List<Security> securities = getCodes(message.m_body, message.m_bodyLength);
                DataCenter.m_securityService.load(securities);
            }
            else if (message.m_functionID == FUNCTIONID_GETDR)
            {
                m_devideRightDatas = getDevideDatas(message.m_body, message.m_bodyLength);
            }
            sendToListener(message);
        }
    }
}
