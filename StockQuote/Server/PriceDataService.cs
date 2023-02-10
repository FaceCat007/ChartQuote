using System;
using System.Collections.Generic;
using System.Text;

namespace FaceCat {
    /// <summary>
    /// 报价服务
    /// </summary>
    public class PriceDataService : FCServerService {
        public PriceDataService() {
            setServiceID(SERVICEID);
        }

        /// <summary>
        /// 服务编号
        /// </summary>
        public const int SERVICEID = 10004;

        /// <summary>
        /// 开始推送
        /// </summary>
        public const int FUNCTION_SUBCODES = 0;

        /// <summary>
        /// 取消推送
        /// </summary>
        public const int FUNCTION_UNSUBCODES = 1;

        /// <summary>
        /// 最新数据
        /// </summary>
        public const int FUNCTION_NEWDATA = 2;

        /// <summary>
        /// 套接字集合
        /// </summary>
        private List<PriceDataRequestInfo> m_socketIDs = new List<PriceDataRequestInfo>();

        /// <summary>
        /// 连接关闭方法
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <param name="localSID">本地套接字ID</param>
        public override void onClientClose(int socketID, int localSID) {
            base.onClientClose(socketID, localSID);
            lock (m_socketIDs) {
                int size = m_socketIDs.Count;
                for (int i = 0; i < size; i++) {
                    if (m_socketIDs[i].m_socketID == socketID) {
                        m_socketIDs.RemoveAt(i);
                        i--;
                        size--;
                    }
                }
            }
        }

        /// <summary>
        /// 接收数据方法
        /// </summary>
        /// <param name="message"></param>
        public override void onReceive(FCMessage message) {
            base.onReceive(message);
            if (message.m_functionID == FUNCTION_SUBCODES) {
                lock (m_socketIDs) {
                    FCBinary br = new FCBinary();
                    br.write(message.m_body, message.m_bodyLength);
                    String codeStr = br.readString();
                    br.close();
                    PriceDataRequestInfo req = new PriceDataRequestInfo();
                    req.m_requestID = message.m_requestID;
                    req.m_codes = codeStr.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    req.m_socketID = message.m_socketID;
                    m_socketIDs.Add(req);
                    Dictionary<String, SecurityLatestData> latestDatas = new Dictionary<string, SecurityLatestData>();
                    for (int i = 0; i < req.m_codes.Length; i++) {
                        SecurityLatestData securityLatestData = new SecurityLatestData();
                        if (StockService.getLatestData(req.m_codes[i], ref securityLatestData) > 0) {
                            latestDatas[req.m_codes[i]] = securityLatestData;
                        }
                    }
                    sendDatas(latestDatas, message.m_socketID);
                }
            } else if (message.m_functionID == FUNCTION_UNSUBCODES) {
                lock (m_socketIDs) {
                    int size = m_socketIDs.Count;
                    for (int i = 0; i < size; i++) {
                        if (m_socketIDs[i].m_socketID == message.m_socketID
                            && m_socketIDs[i].m_requestID == message.m_requestID) {
                            m_socketIDs.RemoveAt(i);
                            i--;
                            size--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="latestDatas">最新数据</param>
        /// <param name="latestDatas">套接字ID</param>
        public void sendDatas(Dictionary<String, SecurityLatestData> latestDatas, int socketID) {
            List<PriceDataRequestInfo> requestInfos = new List<PriceDataRequestInfo>();
            lock (m_socketIDs) {
                foreach (PriceDataRequestInfo req in m_socketIDs) {
                    if (socketID == -1 || req.m_socketID == socketID) {
                        requestInfos.Add(req);
                    }
                }
            }
            int requestInfosSize = requestInfos.Count;
            for (int i = 0; i < requestInfosSize; i++) {
                PriceDataRequestInfo req = requestInfos[i];
                List<SecurityLatestData> sendDatas = new List<SecurityLatestData>();
                for (int j = 0; j < req.m_codes.Length; j++) {
                    if(latestDatas.ContainsKey(req.m_codes[j])){
                        sendDatas.Add(latestDatas[req.m_codes[j]]);
                    }
                }
                if (sendDatas.Count > 0) {
                    FCBinary bw = new FCBinary();
                    bw.writeInt(sendDatas.Count);
                    for (int j = 0; j < sendDatas.Count; j++) {
                        SecurityLatestData data = sendDatas[j];
                        bw.writeString(data.m_code);
                        bw.writeDouble(data.m_open);
                        bw.writeDouble(data.m_lastClose);
                        bw.writeDouble(data.m_close);
                        bw.writeDouble(data.m_high);
                        bw.writeDouble(data.m_low);
                        bw.writeDouble(data.m_volume);
                        bw.writeDouble(data.m_amount);
                        bw.writeDouble(data.m_date);
                    }
                    byte[] bytes = bw.getBytes();
                    bw.close();
                    int ret = send(new FCMessage(getServiceID(), FUNCTION_NEWDATA, req.m_requestID, req.m_socketID, 0, 0, bytes.Length, bytes));
                    sendDatas.Clear();
                    //Console.WriteLine(JsonConvert.SerializeObject(sendDatas));
                }
            }
        }
    } 
}
