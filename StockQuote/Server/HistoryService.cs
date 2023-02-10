using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FaceCat {
    /// <summary>
    /// 历史数据服务
    /// </summary>
    public class HistoryService : FCServerService {
        /// <summary>
        /// 创建服务
        /// </summary>
        public HistoryService() {
            setServiceID(10001);
            setCompressType(COMPRESSTYPE_NONE);
        }

        /// <summary>
        /// 服务编号
        /// </summary>
        public const int SERVICEID = 10001;

        /// <summary>
        /// 获取数据
        /// </summary>
        public const int FUNCTIONID_GETDATA = 0;

        /// <summary>
        /// 获取码表
        /// </summary>
        public const int FUNCTIONID_GETCODE = 1;

        /// <summary>
        /// 获取复权因子
        /// </summary>
        public const int FUNCTIONID_GETDR = 2;

        /// <summary>
        /// 分时数据缓存
        /// </summary>
        public Dictionary<String, MinuteDatasCache> m_minuteDatasCache = new Dictionary<string, MinuteDatasCache>();

        /// <summary>
        /// 接收最新数据
        /// </summary>
        /// <param name="latestData">最新数据</param>
        public void sendData(SecurityLatestData latestData) {
            lock (m_minuteDatasCache) {
                MinuteDatasCache minuteDatasCache = null;
                if (!m_minuteDatasCache.ContainsKey(latestData.m_code)) {
                    minuteDatasCache = new MinuteDatasCache();
                    minuteDatasCache.m_code = latestData.m_code;
                    m_minuteDatasCache[latestData.m_code] = minuteDatasCache;
                } else {
                    minuteDatasCache = m_minuteDatasCache[latestData.m_code];
                }
                minuteDatasCache.m_latestData = latestData;
                StockService.mergeRunTimeDatas(latestData, minuteDatasCache);
            }
        }

        /// <summary>
        /// 落地日线数据
        /// </summary>
        public void fallSecurityDatas()
        {
            lock (m_minuteDatasCache)
            {
                foreach (String code in m_minuteDatasCache.Keys)
                {
                    MinuteDatasCache minuteDatasCache = m_minuteDatasCache[code];
                    String name = "";
                    List<SecurityData2> oldDatas = StockService.getSecurityDatas(code, ref name);
                    StockService.mergeSecurityDatas(minuteDatasCache.m_latestData, oldDatas);
                    StockService.writeSecurityDatas(code, name, oldDatas);
                    oldDatas.Clear();
                }
            }
        }

        /// <summary>
        /// 落地分钟线数据
        /// </summary>
        public void fallMinuteSecurityDatas() {
            lock (m_minuteDatasCache)
            {
                foreach (String code in m_minuteDatasCache.Keys)
                {
                    MinuteDatasCache minuteDatasCache = m_minuteDatasCache[code];
                    String name = "";
                    List<SecurityData2> oldDatas = StockService.getSecurityMinuteDatas(code, ref name);
                    StockService.mergeMinuteSecurityDatas(minuteDatasCache.m_datas, oldDatas);
                    StockService.writeMinuteSecurityDatas(code, name, oldDatas);
                    oldDatas.Clear();
                }
            }
        }

        /// <summary>
        /// 获取历史数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cycle"></param>
        /// <returns></returns>
        public List<SecurityData2> getHistoryDatas(String code, int cycle, ref String name)
        {
            List<SecurityData2> sendDatas = new List<SecurityData2>();
            //分钟线
            if (cycle < 1440)
            {
                sendDatas = StockService.getSecurityMinuteDatas(code, ref name);
                lock (m_minuteDatasCache)
                {
                    if (m_minuteDatasCache.ContainsKey(code))
                    {
                        StockService.mergeMinuteSecurityDatas(sendDatas, m_minuteDatasCache[code].m_datas);
                    }
                }
                if (cycle > 1)
                {
                    List<SecurityData2> newDatas = new List<SecurityData2>();
                    StockService.multiMinuteSecurityDatas(newDatas, sendDatas, cycle);
                    sendDatas = newDatas;
                }
            }
            //日线
            else
            {
                sendDatas = StockService.getSecurityDatas(code, ref name);
                SecurityLatestData latestData = null;
                lock (m_minuteDatasCache)
                {
                    if (m_minuteDatasCache.ContainsKey(code))
                    {
                        latestData = m_minuteDatasCache[code].m_latestData;
                    }
                }
                if (latestData != null)
                {
                    StockService.mergeSecurityDatas(latestData, sendDatas);
                }
                if (cycle == 10080)
                {
                    List<SecurityData2> newDatas = new List<SecurityData2>();
                    StockService.getHistoryWeekDatas(newDatas, sendDatas);
                    sendDatas = newDatas;
                }
                else if (cycle == 43200)
                {
                    List<SecurityData2> newDatas = new List<SecurityData2>();
                    StockService.getHistoryMonthDatas(newDatas, sendDatas);
                    sendDatas = newDatas;
                }
            }
            return sendDatas;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="message">消息</param>
        public override void onReceive(FCMessage message) {
            base.onReceive(message);
            if (message.m_functionID == FUNCTIONID_GETDATA) {
                FCBinary br = new FCBinary();
                br.write(message.m_body, message.m_bodyLength);
                String code = br.readString();
                int cycle = br.readInt();
                double startDate = br.readDouble();
                double endDate = br.readDouble();
                br.close();
                String name = "";
                List<SecurityData2> sendDatas = getHistoryDatas(code, cycle, ref name);
                FCBinary bw = new FCBinary();
                bw.writeString(code);
                bw.writeInt(cycle);
                bw.writeDouble(startDate);
                bw.writeDouble(endDate);
                SecurityLatestData ld = new SecurityLatestData();
                if (StockService.getLatestData(code, ref ld) > 0) {
                    bw.writeDouble(ld.m_date);
                    bw.writeDouble(ld.m_volume);
                    bw.writeDouble(ld.m_amount);
                } else {
                    bw.writeDouble(0);
                    bw.writeDouble(0);
                    bw.writeDouble(0);
                }
                int oldDatasSize = sendDatas.Count;
                if (startDate > 0 || endDate > 0) {
                    for (int i = 0; i < oldDatasSize; i++) {
                        SecurityData2 data = sendDatas[i];
                        if (startDate > 0 && data.m_date < startDate) {
                            sendDatas.RemoveAt(i);
                            i--;
                            oldDatasSize--;
                        } else if (endDate > 0 && data.m_date > endDate) {
                            sendDatas.RemoveAt(i);
                            i--;
                            oldDatasSize--;
                        }
                    }
                }
                oldDatasSize = sendDatas.Count;
                bw.writeInt(oldDatasSize);
                for (int i = 0; i < oldDatasSize; i++) {
                    SecurityData2 data = sendDatas[i];
                    bw.writeDouble(data.m_date);
                    bw.writeDouble(data.m_close);
                    bw.writeDouble(data.m_high);
                    bw.writeDouble(data.m_low);
                    bw.writeDouble(data.m_open);
                    bw.writeDouble(data.m_volume);
                    bw.writeDouble(data.m_amount);
                }
                byte[] bytes = bw.getBytes();
                bw.close();
                int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETDATA, message.m_requestID, message.m_socketID, 0, 0, bytes.Length, bytes));
            } else if (message.m_functionID == FUNCTIONID_GETCODE) {
                FCBinary bw = new FCBinary();
                bw.writeInt(StockService.m_codedMap.Count);
                foreach (Security security in StockService.m_codedMap.Values) {
                    bw.writeString(security.m_code);
                    bw.writeString(security.m_name);
                    bw.writeString(security.m_pingyin);
                    bw.writeString(security.m_status);
                    bw.writeString(security.m_market);
                }
                byte[] bytes = bw.getBytes();
                bw.close();
                int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETCODE, message.m_requestID, message.m_socketID, 0, 0, bytes.Length, bytes));
            }
            else if (message.m_functionID == FUNCTIONID_GETDR)
            {
                FCBinary bw = new FCBinary();
                Dictionary<String, List<List<OneDivideRightBase>>> devideRightDatas = StockService.m_devideRightDatas;
                bw.writeInt(devideRightDatas.Count);
                foreach (String key in devideRightDatas.Keys)
                {
                    bw.writeString(key);
                    List<List<OneDivideRightBase>> oneDatas = devideRightDatas[key];
                    bw.writeInt(oneDatas.Count);
                    for (int i = 0; i < oneDatas.Count; i++)
                    {
                        List<OneDivideRightBase> oneDatas2 = oneDatas[i];
                        bw.writeInt(oneDatas2.Count);
                        for (int j = 0; j < oneDatas2.Count; j++)
                        {
                            OneDivideRightBase oneDivideRightBase = oneDatas2[j];
                            bw.writeDouble(oneDivideRightBase.Date);
                            bw.writeFloat(oneDivideRightBase.DataValue);
                            bw.writeString(oneDivideRightBase.DivideType.ToString());
                            bw.writeFloat(oneDivideRightBase.PClose);
                            bw.writeFloat(oneDivideRightBase.Factor);
                        }
                    }
                }
                byte[] bytes = bw.getBytes();
                bw.close();
                int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETDR, message.m_requestID, message.m_socketID, 0, 0, bytes.Length, bytes));
            }
        }
    }
}
