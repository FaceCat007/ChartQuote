using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;
using System.IO;
using System.IO.Compression;

namespace FaceCat {
    /// <summary>
    /// 最新数据
    /// </summary>
    public class LatestService : FCServerService {
        public LatestService() {
            setServiceID(SERVICEID);
        }

        /// <summary>
        /// 服务编号
        /// </summary>
        public const int SERVICEID = 10003;

        /// <summary>
        /// 开始行情推送
        /// </summary>
        public const int FUNCTION_SUBCODES = 0;

        /// <summary>
        /// 取消行情推送
        /// </summary>
        public const int FUNCTION_UNSUBCODES = 1;

        /// <summary>
        /// 最新数据
        /// </summary>
        public const int FUNCTION_NEWDATA = 2;

        /// <summary>
        /// 套接字ID
        /// </summary>
        private Dictionary<String, List<LatestRequestInfo>> m_socketIDs = new Dictionary<String, List<LatestRequestInfo>>();

        /// <summary>
        /// 服务关闭
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <param name="localSID">本地套接字ID</param>
        public override void onClientClose(int socketID, int localSID) {
            base.onClientClose(socketID, localSID);
            lock (m_socketIDs) {
                foreach (String code in m_socketIDs.Keys) {
                    List<LatestRequestInfo> riList = m_socketIDs[code];
                    int riListSize = riList.Count;
                    for (int i = 0; i < riListSize; i++) {
                        if (riList[i].m_socketID == socketID) {
                            riList.Remove(riList[i]);
                            i--;
                            riListSize--;
                        }
                    }
                }
            }
        } 

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="message">消息</param>
        public override void onReceive(FCMessage message) {
            base.onReceive(message);
            if (message.m_functionID == FUNCTION_SUBCODES) {
                List<String> subCodes = new List<string>();
                lock (m_socketIDs) {
                    FCBinary br = new FCBinary();
                    br.write(message.m_body, message.m_bodyLength);
                    String codeStr = br.readString();
                    br.close();
                    String[] codes = codeStr.Split(new String[]{","}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (String code in codes) {
                        if (!m_socketIDs.ContainsKey(code)) {
                            m_socketIDs[code] = new List<LatestRequestInfo>();
                        }
                        bool hasSocketID = false;
                        foreach (LatestRequestInfo ri in m_socketIDs[code]) {
                            if (ri.m_socketID == message.m_socketID) {
                                hasSocketID = true;
                                break;
                            }
                        }
                        if (!hasSocketID) {
                            LatestRequestInfo requestInfo = new LatestRequestInfo();
                            requestInfo.m_socketID = message.m_socketID;
                            requestInfo.m_requestID = message.m_requestID;
                            m_socketIDs[code].Add(requestInfo);
                            subCodes.Add(code);
                        }
                    }
                }
                foreach (String code in subCodes) {
                    SecurityLatestData securityLatestData = new SecurityLatestData();
                    if (StockService.getLatestData(code, ref securityLatestData) > 0) {
                        sendData(securityLatestData, message.m_socketID);
                    }
                }
            } else if (message.m_functionID == FUNCTION_UNSUBCODES) {
                lock (m_socketIDs) {
                    FCBinary br = new FCBinary();
                    br.write(message.m_body, message.m_bodyLength);
                    String codeStr = br.readString();
                    br.close();
                    foreach (List<LatestRequestInfo> req in m_socketIDs.Values) {
                        for (int i = 0; i < req.Count; i++) {
                            LatestRequestInfo ri = req[i];
                            if (ri.m_socketID == message.m_socketID && ri.m_requestID == message.m_requestID) {
                                req.Remove(ri);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 发送最新数据
        /// </summary>
        /// <param name="latestData">最新数据</param>
        /// <param name="socketID">套接字ID</param>
        public void sendData(SecurityLatestData data, int socketID) {
            List<LatestRequestInfo> socketIDs = new List<LatestRequestInfo>();
            lock (m_socketIDs) {
                if (m_socketIDs.ContainsKey(data.m_code)) {
                    socketIDs.AddRange(m_socketIDs[data.m_code].ToArray());
                }
            }
            if (socketIDs.Count > 0) {
                byte[] body = null;
                if (true) {
                    FCBinary bw = new FCBinary();
                    bw.writeString(data.m_code);
                    bw.writeDouble(data.m_open);
                    bw.writeDouble(data.m_lastClose);
                    bw.writeDouble(data.m_close);
                    bw.writeDouble(data.m_high);
                    bw.writeDouble(data.m_low);
                    bw.writeDouble(data.m_volume);
                    bw.writeDouble(data.m_amount);
                    bw.writeInt(data.m_buyVolume1);
                    bw.writeDouble(data.m_buyPrice1);
                    bw.writeInt(data.m_buyVolume2);
                    bw.writeDouble(data.m_buyPrice2);
                    bw.writeInt(data.m_buyVolume3);
                    bw.writeDouble(data.m_buyPrice3);
                    bw.writeInt(data.m_buyVolume4);
                    bw.writeDouble(data.m_buyPrice4);
                    bw.writeInt(data.m_buyVolume5);
                    bw.writeDouble(data.m_buyPrice5);
                    bw.writeInt(data.m_sellVolume1);
                    bw.writeDouble(data.m_sellPrice1);
                    bw.writeInt(data.m_sellVolume2);
                    bw.writeDouble(data.m_sellPrice2);
                    bw.writeInt(data.m_sellVolume3);
                    bw.writeDouble(data.m_sellPrice3);
                    bw.writeInt(data.m_sellVolume4);
                    bw.writeDouble(data.m_sellPrice4);
                    bw.writeInt(data.m_sellVolume5);
                    bw.writeDouble(data.m_sellPrice5);
                    bw.writeInt(data.m_innerVol);
                    bw.writeInt(data.m_outerVol);
                    bw.writeDouble(data.m_turnoverRate);
                    bw.writeDouble(data.m_openInterest);
                    bw.writeDouble(data.m_settlePrice);
                    bw.writeDouble(data.m_date);
                    body = bw.getBytes();
                    bw.close();
                }
                byte[] finalBytes = null;
                if (true) {
                    FCBinary bw = new FCBinary();
                    int bodyLength = body.Length;
                    int uncBodyLength = bodyLength;
                    int len = sizeof(int) * 3 + bodyLength + sizeof(short) * 2 + sizeof(byte) * 2;
                    bw.writeInt(len);
                    bw.writeShort((short)getServiceID());
                    bw.writeShort((short)FUNCTION_NEWDATA);
                    bw.writeInt(0);
                    bw.writeByte((byte)0);
                    bw.writeByte((byte)0);
                    bw.writeInt(uncBodyLength);
                    bw.writeBytes(body);
                    finalBytes = bw.getBytes();
                    bw.close();
                }

                for (int i = 0; i < socketIDs.Count; i++) {
                    LatestRequestInfo ri = socketIDs[i];
                    if (socketID == -1 || socketID == ri.m_socketID) {
                        finalBytes[8] = (byte)(ri.m_requestID & 0xff);
                        finalBytes[9] = (byte)((ri.m_requestID & 0xff00) >> 8);
                        finalBytes[10] = (byte)((ri.m_requestID & 0xff0000) >> 16);
                        finalBytes[11] = (byte)((ri.m_requestID & 0xff000000) >> 24);
                        int ret = FCServerSockets.send(ri.m_socketID, getSocketID(), finalBytes, finalBytes.Length);
                        m_upFlow += ret;
                    }
                }
            }
        }
    }
}
