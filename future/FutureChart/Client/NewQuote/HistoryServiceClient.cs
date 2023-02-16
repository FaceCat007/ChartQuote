using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;

namespace future
{
    /// <summary>
    /// 历史数据服务
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
        /// 获取套接字
        /// </summary>
        /// <returns></returns>
        public int getSocketID() {
            return m_socketID;
        }

        /// <summary>
        /// 设置套接字
        /// </summary>
        /// <param name="value"></param>
        public void setSocketID(int value) {
            m_socketID = value;
        }

        /// <summary>
        /// 注册代码
        /// </summary>
        /// <param name="requestID">请求ID</param>
        public void reqCodes(int requestID) {
            FCBinary bw = new FCBinary();
            bw.writeString("1");
            byte[] bytes = bw.getBytes();
            bw.close();
            int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETCODE, requestID, m_socketID, 0, 0, bytes.Length, bytes));
        }

        /// <summary>
        /// 请求K线
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <param name="code">代码</param>
        /// <param name="cycle">周期</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
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
        /// 获取K线数据
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="cycle">周期</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="nowDate">当前时间</param>
        /// <param name="nowVolume">当前量</param>
        /// <param name="nowOpenInterest">当前的持仓量</param>
        /// <param name="body">包体</param>
        /// <param name="bodyLength">包体长度</param>
        /// <returns>数据</returns>
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
        /// 获取代码
        /// </summary>
        /// <param name="body">包体</param>
        /// <param name="bodyLength">包体长度</param>
        /// <returns>数据</returns>
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
        /// 接收消息方法
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
