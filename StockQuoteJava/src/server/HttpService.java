/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package server;
import facecat.topin.service.*;
import facecat.topin.core.*;
import facecat.topin.sock.*;
import java.text.SimpleDateFormat;
import service.*;
import java.util.*;

/**
 *
 * Http服务
 */
public class HttpService implements FCHttpEasyService{
    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int onReceive(FCHttpData data)
    {
        if (data.m_parameters.containsKey("func"))
        {
            String func = data.m_parameters.get("func");
            //http://127.0.0.1:9958/quote?func=getcodes
            //获取代码
            if (func.equals("getcodes"))
            {
                StringBuilder sb = new StringBuilder();
                sb.append("代码,名称\r\n");
                for (Security security : StockService.m_codedMap.values())
                {
                    sb.append(security.m_code + security.m_name + "\r\n");
                }
                data.m_resStr = sb.toString();
            }
            //http://127.0.0.1:9958/quote?func=getdr
            //获取复权因子
            else if (func.equals("getdr"))
            {
                 String filePath = DataCenter.getAppPath() + DataCenter.m_seperator + "DR.txt";
                 if (FCFile.isFileExist(filePath))
                 {
                     String content = "";
                     RefObject<String> refContent = new RefObject<String>(content);
                     FCFile.read(filePath, refContent);
                     content = refContent.argvalue;
                     data.m_resStr = content;
                 }
            }
            //http://127.0.0.1:9958/quote?func=getkline&code=000001.SZ&cycle=day
            //http://127.0.0.1:9958/quote?func=getkline&code=000001.SZ&cycle=minute&subscription=forward
            //获取K线
            else if (func.equals("getkline"))
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
                    cycle = FCTran.strToInt(strCycle.replace("m", ""));
                }
                String name = "";
                RefObject<String> refName = new RefObject<String>(name);
                ArrayList<SecurityData2> datas = DataCenter.m_historyService.getHistoryDatas(code, cycle, refName);
                name = refName.argvalue;
                if (data.m_parameters.containsKey("subscription"))
                {
                    String subscription = data.m_parameters.get("subscription");
                    //前复权
                    if (subscription == "forward")
                    {
                        if (StockService.m_devideRightDatas.containsKey(code))
                        {
                            ArrayList<ArrayList<OneDivideRightBase>> divideData = StockService.m_devideRightDatas.get(code);
                            StockService.caculateDivideKLineData(datas, divideData, true);
                            float[] factors = StockService.caculateDivideKLineData2(datas, divideData, true);
                            if (factors != null)
                            {
                                for (int i = 0; i < factors.length; i++)
                                {
                                    StockService.getDataAfterDivide(IsDivideRightType.Forward, datas.get(i), factors[i]);
                                }
                            }
                        }
                    }
                    //后复权
                    else if (subscription == "backward")
                    {
                        if (StockService.m_devideRightDatas.containsKey(code))
                        {
                            ArrayList<ArrayList<OneDivideRightBase>> divideData = StockService.m_devideRightDatas.get(code);
                            StockService.caculateDivideKLineData(datas, divideData, false);
                            float[] factors = StockService.caculateDivideKLineData2(datas, divideData, false);
                            if (factors != null)
                            {
                                for (int i = 0; i < factors.length; i++)
                                {
                                    StockService.getDataAfterDivide(IsDivideRightType.Backward, datas.get(i), factors[i]);
                                }
                            }
                        }
                    }
                }
                StringBuilder sb = new StringBuilder();
                sb.append(code + " " + name + "\r\n");
                sb.append("日期,开盘价,最高价,最低价,收盘价,成交量,成交额\r\n");
                for (int i = 0; i < datas.size(); i++)
                {
                    SecurityData2 data2 = datas.get(i);
                    if (cycle >= 1440)
                    {
                        Calendar dateTime = FCTran.numToDate(data2.m_date);
                        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");
                        String dateStr = format.format(dateTime.getTime());
                        sb.append(String.format("%1$s,%2$s,%3$s,%4$s,%5$s,%6$s,%7$s\r\n", dateStr
                        , data2.m_open, data2.m_high, data2.m_low, data2.m_close, data2.m_volume, data2.m_amount));
                    }
                    else
                    {
                        Calendar dateTime = FCTran.numToDate(data2.m_date);
                        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd,HH:mm");
                        String dateStr = format.format(dateTime.getTime());
                        sb.append(String.format("%1$s,%2$s,%3$s,%4$s,%5$s,%6$s,%7$s\r\n", dateStr
                        , data2.m_open, data2.m_high, data2.m_low, data2.m_close, data2.m_volume, data2.m_amount));
                    }
                }
                data.m_resStr = sb.toString();
            }
            //http://127.0.0.1:9958/quote?func=getnewdata&codes=601857.SH,600028.SH
            //获取最新的行情
            else if (func.equals("getnewdata"))
            {
                ArrayList<SecurityLatestData> latestDatas = new ArrayList<SecurityLatestData>();
                String[] codes = data.m_parameters.get("codes").split("[,]");
                for (int i = 0; i < codes.length; i++)
                {
                    SecurityLatestData latestData = new SecurityLatestData();
                    if (StockService.getLatestData(codes[i], latestData) > 0)
                    {
                        if (StockService.m_codedMap.containsKey(codes[i]))
                        {
                            latestData.m_name = StockService.m_codedMap.get(codes[i]).m_name;
                            latestDatas.add(latestData);
                        }
                    }
                }
                StringBuilder sb = new StringBuilder();
                if (latestDatas.size() > 0)
                {
                    for (int i = 0; i < latestDatas.size(); i++)
                    {
                        SecurityLatestData latestData = latestDatas.get(i);
                        Calendar dateTime = FCTran.numToDate(latestData.m_date);
                        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
                        String dateStr = format.format(dateTime.getTime());
                        sb.append(String.format("%1$s,%2$s,%3$s,%4$s,%5$s,%6$s,%7$s,%8$s,%9$s,%10$s,%11$s,%12$s,%13$s,%14$s,%15$s,%16$s,%17$s,%18$s,%19$s,%20$s,%21$s,%22$s,%23$s,%24$s,%25$s,%26$s,%27$s,%28$s,%29$s", latestData.m_code, latestData.m_name,
                            latestData.m_close, latestData.m_high, latestData.m_low, latestData.m_open,
                            latestData.m_volume, latestData.m_amount, latestData.m_buyPrice1, latestData.m_buyPrice2, latestData.m_buyPrice3,
                            latestData.m_buyPrice4, latestData.m_buyPrice5, latestData.m_buyVolume1, latestData.m_buyVolume2, latestData.m_buyVolume3,
                            latestData.m_buyVolume4, latestData.m_buyVolume5, latestData.m_sellPrice1, latestData.m_sellPrice2, latestData.m_sellPrice3,
                            latestData.m_sellPrice4, latestData.m_sellPrice5, latestData.m_sellVolume1, latestData.m_sellVolume2,
                            latestData.m_sellVolume3, latestData.m_sellVolume4, latestData.m_sellVolume5, dateStr));
                    }
                }
                data.m_resStr = sb.toString();
            }
        }
        return 1;
    }
}
