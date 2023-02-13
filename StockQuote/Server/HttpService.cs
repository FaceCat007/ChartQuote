using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FaceCat
{
    /// <summary>
    /// Http服务
    /// </summary>
    public class HttpService:FCHttpEasyService
    {
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>状态</returns>
        public int onReceive(FCHttpData data)
        {
            if (data.m_parameters.containsKey("func"))
            {
                String func = data.m_parameters.get("func");
                //http://127.0.0.1:9958/quote?func=exportday
                //获取代码
                if (func == "exportday")
                {
                    DataCenter.m_historyService.fallSecurityDatas();
                }
                //http://127.0.0.1:9958/quote?func=exportday
                //获取代码
                else if (func == "exportminute")
                {
                    DataCenter.m_historyService.fallMinuteSecurityDatas();
                }
                //http://127.0.0.1:9958/quote?func=getcodes
                //获取代码
                else if (func == "getcodes"){
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("代码,名称");
                    foreach (Security security in StockService.m_codedMap.Values)
                    {
                        sb.AppendLine(String.Format("{0},{1}", security.m_code, security.m_name));
                    }
                    data.m_resStr = sb.ToString();
                }
                //http://127.0.0.1:9958/quote?func=getdr
                //获取复权因子
                else if (func == "getdr")
                {
                    String filePath = Application.StartupPath + "\\DR.txt";
                    if (FCFile.isFileExist(filePath))
                    {
                        String content = "";
                        FCFile.read(filePath, ref content);
                        data.m_resStr = content;
                    }
                }
                //http://127.0.0.1:9958/quote?func=getkline&code=000001.SZ&cycle=day
                //http://127.0.0.1:9958/quote?func=getkline&code=000001.SZ&cycle=minute&subscription=forward
                //获取K线
                else if (func == "getkline")
                {
                    String code = data.m_parameters.get("code");
                    int cycle = 1440;
                    String strCycle = data.m_parameters.get("cycle");
                    if (strCycle == "day")
                    {
                        cycle = 1440;
                    }
                    else if (strCycle == "week")
                    {
                        cycle = 10800;
                    }
                    else if (strCycle == "month")
                    {
                        cycle = 43200;
                    }
                    else
                    {
                        cycle = FCTran.strToInt(strCycle.Replace("m", ""));
                    }
                    String name = "";
                    List<SecurityData2> datas = DataCenter.m_historyService.getHistoryDatas(code, cycle, ref name);
                    if (data.m_parameters.containsKey("subscription"))
                    {
                        String subscription = data.m_parameters.get("subscription");
                        //前复权
                        if (subscription == "forward")
                        {
                            if (StockService.m_devideRightDatas.ContainsKey(code))
                            {
                                List<List<OneDivideRightBase>> divideData = StockService.m_devideRightDatas[code];
                                StockService.caculateDivideKLineData(datas, divideData, true);
                                float[] factors = StockService.caculateDivideKLineData2(datas, divideData, true);
                                if (factors != null)
                                {
                                    for (int i = 0; i < factors.Length; i++)
                                    {
                                        StockService.getDataAfterDivide(IsDivideRightType.Forward, datas[i], factors[i]);
                                    }
                                }
                            }
                        }
                        //后复权
                        else if (subscription == "backward")
                        {
                            if (StockService.m_devideRightDatas.ContainsKey(code))
                            {
                                List<List<OneDivideRightBase>> divideData = StockService.m_devideRightDatas[code];
                                StockService.caculateDivideKLineData(datas, divideData, false);
                                float[] factors = StockService.caculateDivideKLineData2(datas, divideData, false);
                                if (factors != null)
                                {
                                    for (int i = 0; i < factors.Length; i++)
                                    {
                                        StockService.getDataAfterDivide(IsDivideRightType.Backward, datas[i], factors[i]);
                                    }
                                }
                            }
                        }
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(code + " " + name);
                    sb.AppendLine("日期,开盘价,最高价,最低价,收盘价,成交量,成交额");
                    for (int i = 0; i < datas.Count; i++)
                    {
                        SecurityData2 data2 = datas[i];
                        if (cycle >= 1440)
                        {
                            sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}", FCTran.numToDate(data2.m_date).ToString("yyyy-MM-dd"),
                                data2.m_open, data2.m_high, data2.m_low, data2.m_close, data2.m_volume, data2.m_amount));
                        }
                        else
                        {
                            sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}", FCTran.numToDate(data2.m_date).ToString("yyyy-MM-dd HH:mm:ss"),
                                data2.m_open, data2.m_high, data2.m_low, data2.m_close, data2.m_volume, data2.m_amount));
                        }
                    }
                    data.m_resStr = sb.ToString();
                }
                //http://127.0.0.1:9958/quote?func=getnewdata&codes=601857.SH,600028.SH
                //获取最新的行情
                else if (func == "getnewdata")
                {
                    ArrayList<SecurityLatestData> latestDatas = new ArrayList<SecurityLatestData>();
                    String[] codes = data.m_parameters.get("codes").Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < codes.Length; i++)
                    {
                        SecurityLatestData latestData = new SecurityLatestData();
                        if (StockService.getLatestData(codes[i], ref latestData) > 0)
                        {
                            if (StockService.m_codedMap.ContainsKey(codes[i]))
                            {
                                latestData.m_name = StockService.m_codedMap[codes[i]].m_name;
                                latestDatas.add(latestData);
                            }
                        }
                    }
                    StringBuilder sb = new StringBuilder();
                    if (latestDatas.size() > 0)
                    {
                        for (int i = 0; i < latestDatas.size(); i++)
                        {
                            SecurityLatestData latestData = latestDatas[i];
                            sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28}", latestData.m_code, latestData.m_name,
                                latestData.m_close, latestData.m_high, latestData.m_low, latestData.m_open,
                                latestData.m_volume, latestData.m_amount, latestData.m_buyPrice1, latestData.m_buyPrice2, latestData.m_buyPrice3,
                                latestData.m_buyPrice4, latestData.m_buyPrice5, latestData.m_buyVolume1, latestData.m_buyVolume2, latestData.m_buyVolume3,
                                latestData.m_buyVolume4, latestData.m_buyVolume5, latestData.m_sellPrice1, latestData.m_sellPrice2, latestData.m_sellPrice3,
                                latestData.m_sellPrice4, latestData.m_sellPrice5, latestData.m_sellVolume1, latestData.m_sellVolume2,
                                latestData.m_sellVolume3, latestData.m_sellVolume4, latestData.m_sellVolume5, FCTran.numToDate(latestData.m_date).ToString("yyyy-MM-dd HH:mm:ss")));

                        }
                    }
                    data.m_resStr = sb.ToString();
                }
            }
            return 1;
        }
    }
}
