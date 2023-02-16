using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FaceCat;

namespace future
{
    /// <summary>
    /// ��ʷ���ݷ���
    /// </summary>
    public class HistoryService : FCServerService {
        public HistoryService() {
            setServiceID(10001);
            setCompressType(COMPRESSTYPE_NONE);
        }

        public const int SERVICEID = 10001;

        public const int FUNCTIONID_GETDATA = 0;

        public const int FUNCTIONID_GETCODE = 1;

        public const int FUNCTIONID_GETDR = 2;

        /// <summary>
        /// ��ʱ���ݻ���
        /// </summary>
        public Dictionary<String, MinuteDatasCache> m_minuteDatasCache = new Dictionary<string, MinuteDatasCache>();

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="latestData">��������</param>
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
                FutureService.mergeRunTimeDatas(latestData, minuteDatasCache);
            }
        }

        /// <summary>
        /// �����������
        /// </summary>
        public void fallSecurityDatas() {
            foreach (String code in m_minuteDatasCache.Keys) {
                MinuteDatasCache minuteDatasCache = m_minuteDatasCache[code];
                String name = "";
                List<SecurityData2> oldDatas = FutureService.getSecurityDatas(code, ref name);
                FutureService.mergeSecurityDatas(minuteDatasCache.m_latestData, oldDatas);
                FutureService.writeSecurityDatas(code, name, oldDatas);
                oldDatas.Clear();
            }
        }

        /// <summary>
        /// ��ط���������
        /// </summary>
        public void fallMinuteSecurityDatas() {
            foreach (String code in m_minuteDatasCache.Keys) {
                MinuteDatasCache minuteDatasCache = m_minuteDatasCache[code];
                String name = "";
                List<SecurityData2> oldDatas = FutureService.getSecurityMinuteDatas(code, ref name);
                FutureService.mergeMinuteSecurityDatas(minuteDatasCache.m_datas, oldDatas);
                FutureService.writeMinuteSecurityDatas(code, name, oldDatas);
                oldDatas.Clear();
            }
        }

        /// <summary>
        /// ��ȡ��ʷ����
        /// </summary>
        /// <param name="code">����</param>
        /// <param name="cycle">����</param>
        /// <param name="name">����</param>
        /// <returns>��ʷ����</returns>
        public List<SecurityData2> getHistoryDatas(String code, int cycle, ref String name)
        {
            List<SecurityData2> sendDatas = new List<SecurityData2>();
            //������
            if (cycle < 1440)
            {
                sendDatas = FutureService.getSecurityMinuteDatas(code, ref name);
                lock (m_minuteDatasCache)
                {
                    if (m_minuteDatasCache.ContainsKey(code))
                    {
                        FutureService.mergeMinuteSecurityDatas(sendDatas, m_minuteDatasCache[code].m_datas);
                    }
                }
                if (cycle > 1)
                {
                    List<SecurityData2> newDatas = new List<SecurityData2>();
                    FutureService.multiMinuteSecurityDatas(newDatas, sendDatas, cycle);
                    sendDatas = newDatas;
                }
            }
            //����
            else
            {
                sendDatas = FutureService.getSecurityDatas(code, ref name);
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
                    FutureService.mergeSecurityDatas(latestData, sendDatas);
                }
                if (cycle == 10080)
                {
                    List<SecurityData2> newDatas = new List<SecurityData2>();
                    FutureService.getHistoryWeekDatas(newDatas, sendDatas);
                    sendDatas = newDatas;
                }
                else if (cycle == 43200)
                {
                    List<SecurityData2> newDatas = new List<SecurityData2>();
                    FutureService.getHistoryMonthDatas(newDatas, sendDatas);
                    sendDatas = newDatas;
                }
            }
            return sendDatas;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="message">��Ϣ</param>
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
                if (FutureService.getLatestData(code, ref ld) > 0)
                {
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
                    bw.writeDouble(data.m_openInterest);
                }
                byte[] bytes = bw.getBytes();
                bw.close();
                int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETDATA, message.m_requestID, message.m_socketID, 0, 0, bytes.Length, bytes));
            } else if (message.m_functionID == FUNCTIONID_GETCODE) {
                FCBinary bw = new FCBinary();
                bw.writeInt(FutureService.m_codedMap.Count);
                foreach (Security security in FutureService.m_codedMap.Values)
                {
                    bw.writeString(security.m_code);
                    bw.writeString(security.m_name);
                    bw.writeString(security.m_pingyin);
                    bw.writeInt(security.m_status);
                    bw.writeInt(security.m_type);
                }
                byte[] bytes = bw.getBytes();
                bw.close();
                int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETCODE, message.m_requestID, message.m_socketID, 0, 0, bytes.Length, bytes));
            }
        }
    }
}
