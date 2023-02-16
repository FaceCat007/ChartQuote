using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace future
{
    /// <summary>
    /// 期货数据服务
    /// </summary>
    public class FutureService
    {
        /// <summary>
        /// 代码表字典
        /// </summary>
        public static Dictionary<String, Security> m_codedMap = new Dictionary<String, Security>();

        /// <summary>
        /// 最新数据
        /// </summary>
        public static Dictionary<String, SecurityLatestData> m_latestDatas = new Dictionary<String, SecurityLatestData>();

        /// <summary>
        /// 交易时间
        /// </summary>
        public static double m_shTradeTime;

        /// <summary>
        /// 返回所有合约
        /// </summary>
        /// <param name="codes">合约代码集合</param>
        public static void getCodes(List<String> codes)
        {
            foreach (String key in m_codedMap.Keys)
            {
                codes.Add(key);
            }
        }

        /// <summary>
        /// 通过代码返回合约信息
        /// </summary>
        /// <param name="code">合约代码</param>
        /// <param name="sd">out 股票基本信息</param>
        public static int getSecurityByCode(String code, ref Security security)
        {
            int ret = 0;
            foreach (String key in m_codedMap.Keys)
            {
                if (key == code)
                {
                    security = m_codedMap[key];
                    ret = 1;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取最新数据
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="latestData">最新数据</param>
        /// <returns>状态</returns>
        public static int getLatestData(String code, ref SecurityLatestData latestData)
        {
            int state = 0;
            lock (m_latestDatas)
            {
                if (m_latestDatas.ContainsKey(code))
                {
                    latestData.copy(m_latestDatas[code]);
                    state = 1;
                }
            }
            return state;
        }

        /// <summary>
        /// 期货数据回调
        /// </summary>
        private static FutureLatestDataCallBack m_callBack;

        /// <summary>
        /// 交易所ID
        /// </summary>
        private static Dictionary<String, String> m_exchangeIDs = new Dictionary<String, String>();

        /// <summary>
        /// 加载数据
        /// </summary>
        public static void load()
        {
            //加载代码表//step 1
            if (m_codedMap.Count == 0)
            {
                m_callBack = new FutureLatestDataCallBack(callBackData);
                FutureAPI.addFutureLatestDataCallBack(m_callBack, "simnow_client_test", "0000000000000000", "tcp://180.168.146.187:10212", "tcp://180.168.146.187:10202", "9999", "021739", "123456");
                Thread.Sleep(3000);
                StringBuilder sb = new StringBuilder();
                while (sb.Length == 0)
                {
                    FutureAPI.getInstruments(sb);
                }
                String[] strs = sb.ToString().Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strs.Length; i++)
                {
                    String str = strs[i];
                    if (str.Length > 10)
                    {
                        String[] subStrs = str.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        Security security = new Security();
                        security.m_code = subStrs[0] + "." + subStrs[2];
                        security.m_name = subStrs[1];
                        m_exchangeIDs[subStrs[0]] = subStrs[2];
                        m_codedMap[security.m_code] = security;
                    }
                }
            }
        }

        /// <summary>
        /// 转换Double数据
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>Double数据</returns>
        public static double convertCTPDouble(double value)
        {
            if (value + 1 != value)
            {
                return value;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 行情数据回调
        /// </summary>
        /// <param name="data">数据</param>
        public static void callBackData(CThostFtdcDepthMarketDataField data)
        {
            if (data.TradingDay.Length > 0)
            {
                SecurityLatestData latestData = new SecurityLatestData();
                latestData.m_volume = convertCTPDouble(data.Volume);
                latestData.m_open = convertCTPDouble(data.OpenPrice);
                latestData.m_close = convertCTPDouble(data.LastPrice);
                latestData.m_lastClose = convertCTPDouble(data.PreClosePrice);
                latestData.m_high = convertCTPDouble(data.HighestPrice);
                latestData.m_low = convertCTPDouble(data.LowestPrice);
                latestData.m_buyPrice1 = convertCTPDouble(data.BidPrice1);
                latestData.m_buyVolume1 = data.BidVolume1;
                latestData.m_sellPrice1 = convertCTPDouble(data.AskPrice1);
                latestData.m_sellVolume1 = data.BidVolume1;
                DateTime dt = Convert.ToDateTime(data.TradingDay.Substring(0, 4) + "-" + data.TradingDay.Substring(4, 2) + "-" + data.TradingDay.Substring(6, 2) + " " + data.UpdateTime);
                latestData.m_date = FCTran.dateToNum(dt);
                if (m_exchangeIDs.ContainsKey(data.InstrumentID))
                {
                    latestData.m_code = data.InstrumentID + "." + m_exchangeIDs[data.InstrumentID];
                    if (m_shTradeTime < latestData.m_date)
                    {
                        m_shTradeTime = latestData.m_date;
                    }

                    Dictionary<String, SecurityLatestData> priceDatas = new Dictionary<string, SecurityLatestData>();
                    if (latestData.m_close == 0)
                    {
                        latestData.m_close = latestData.m_buyPrice1;
                    }
                    if (latestData.m_close == 0)
                    {
                        latestData.m_close = latestData.m_sellPrice1;
                    }
                    lock (m_latestDatas)
                    {
                        bool newData = false;
                        if (!m_latestDatas.ContainsKey(latestData.m_code))
                        {
                            m_latestDatas[latestData.m_code] = latestData;
                            newData = true;
                        }
                        else
                        {
                            if (!m_latestDatas[latestData.m_code].equal(latestData))
                            {
                                m_latestDatas[latestData.m_code].copy(latestData);
                                newData = true;
                            }
                        }
                        if (newData)
                        {
                            DataCenter.m_latestService.sendData(latestData, -1);
                            DataCenter.m_historyService.sendData(latestData);
                            priceDatas[latestData.m_code] = latestData;
                        }
                    }
                    if (priceDatas.Count > 0)
                    {
                        DataCenter.m_priceDataService.sendDatas(priceDatas, -1);
                    }
                }
            }
        }

        /// <summary>
        /// 交易时间
        /// </summary>
        public static List<int> m_shTradeTimes = new List<int>();

        /// <summary>
        /// 获取日线数据
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="name">名称</param>
        /// <returns>数据</returns>
        public static List<SecurityData2> getSecurityDatas(String code, ref String name)
        {
            List<SecurityData2> datas = new List<SecurityData2>();
            String appPath = Application.StartupPath;
            String[] marketCodes = new String[] { "28", "29", "30", "47" };
            for (int m = 0; m < marketCodes.Length; m++)
            {
                String inCode = code;
                if (inCode.IndexOf(".") != -1)
                {
                    inCode = inCode.Substring(0, inCode.IndexOf("."));
                }
                String filePath = appPath + "\\futureday\\" + marketCodes[m] + FCStrEx.convertDBCodeToFileName(inCode).ToUpper();
                if (FCFile.isFileExist(filePath))
                {
                    String content = "";
                    FCFile.read(filePath, ref content);
                    String[] strs = content.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    int strsSize = strs.Length;
                    String[] headStrs = strs[0].Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    name = headStrs[1];
                    for (int i = 2; i < strs.Length - 1; i++)
                    {
                        String str = strs[i];
                        String[] subStrs = str.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        SecurityData2 securityData = new SecurityData2();
                        securityData.m_date = FCTran.dateToNum(Convert.ToDateTime(subStrs[0]));
                        securityData.m_open = FCTran.strToDouble(subStrs[1]);
                        securityData.m_high = FCTran.strToDouble(subStrs[2]);
                        securityData.m_low = FCTran.strToDouble(subStrs[3]);
                        securityData.m_close = FCTran.strToDouble(subStrs[4]);
                        securityData.m_volume = FCTran.strToDouble(subStrs[5]);
                        securityData.m_openInterest = FCTran.strToDouble(subStrs[6]);
                        datas.Add(securityData);
                    }
                    break;
                }
            }
            return datas;
        }

        /// <summary>
        /// 获取分时线数据
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="name">名称</param>
        /// <returns>数据</returns>
        public static List<SecurityData2> getSecurityMinuteDatas(String code, ref String name)
        {
            List<SecurityData2> datas = new List<SecurityData2>();
            String appPath = Application.StartupPath;
            String[] marketCodes = new String[] { "28", "29", "30", "47" };
            for (int m = 0; m < marketCodes.Length; m++)
            {
                String inCode = code;
                if (inCode.IndexOf(".") != -1)
                {
                    inCode = inCode.Substring(0, inCode.IndexOf("."));
                }
                String filePath = appPath + "\\futureminute\\" + marketCodes[m] + FCStrEx.convertDBCodeToFileName(inCode).ToUpper();
                if (FCFile.isFileExist(filePath))
                {
                    String content = "";
                    FCFile.read(filePath, ref content);
                    String[] strs = content.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    int strsSize = strs.Length;
                    String[] headStrs = strs[0].Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    name = headStrs[1];
                    for (int i = 2; i < strs.Length - 1; i++)
                    {
                        String str = strs[i];
                        String[] subStrs = str.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        SecurityData2 securityData = new SecurityData2();
                        int hour = FCTran.strToInt(subStrs[1].Substring(0, 2));
                        int minute = FCTran.strToInt(subStrs[1].Substring(2, 2));
                        DateTime dt = Convert.ToDateTime(subStrs[0]);
                        securityData.m_date = FCTran.dateToNum(new DateTime(dt.Year, dt.Month, dt.Day, hour, minute, 0));
                        securityData.m_open = FCTran.strToDouble(subStrs[2]);
                        securityData.m_high = FCTran.strToDouble(subStrs[3]);
                        securityData.m_low = FCTran.strToDouble(subStrs[4]);
                        securityData.m_close = FCTran.strToDouble(subStrs[5]);
                        securityData.m_volume = FCTran.strToDouble(subStrs[6]);
                        securityData.m_openInterest = FCTran.strToDouble(subStrs[7]);
                        datas.Add(securityData);
                    }
                }
            }
            return datas;
        }

        /// <summary>
        /// 写日线数据
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="name">名称</param>
        /// <param name="datas">数据</param>
        public static void writeSecurityDatas(String code, String name, List<SecurityData2> datas)
        {
            int datasSize = datas.Count;
            StringBuilder sb = new StringBuilder();
            sb.Append(code + " " + name + " 日线\r\n");
            sb.Append("日期\t开盘\t最高\t最低\t收盘\t成交量\t持仓量\t结算价\r\n");
            for (int i = 0; i < datasSize; i++)
            {
                SecurityData2 data = datas[i];
                DateTime dateTime = FCTran.numToDate(data.m_date);
                sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6},0\r\n",
                    dateTime.ToString("yyyy-MM-dd"), data.m_open, data.m_high,
                    data.m_low, data.m_close, data.m_volume, data.m_openInterest));
            }
            sb.Append("数据来源:花卷猫");
            String appPath = Application.StartupPath;
            String filePath = appPath + "\\day\\" + FCStrEx.convertDBCodeToFileName(code);
            FCFile.write(filePath, sb.ToString());
        }

        /// <summary>
        /// 写分钟线数据
        /// </summary>
        /// <param name="code">代码</param>
        /// <param name="name">名称</param>
        /// <param name="datas">数据</param>
        public static void writeMinuteSecurityDatas(String code, String name, List<SecurityData2> datas)
        {
            int datasSize = datas.Count;
            StringBuilder sb = new StringBuilder();
            sb.Append(code + " " + name + " 1分钟线\r\n");
            sb.Append("日期\t时间\t开盘\t最高\t最低\t收盘\t成交量\t持仓量\t结算价\r\n");
            for (int i = 0; i < datasSize; i++)
            {
                SecurityData2 data = datas[i];
                DateTime dateTime = FCTran.numToDate(data.m_date);
                sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},0\r\n",
                    dateTime.ToString("yyyy-MM-dd"), dateTime.ToString("HHmm"), data.m_open, data.m_high,
                    data.m_low, data.m_close, data.m_volume, data.m_openInterest));
            }
            sb.Append("数据来源:花卷猫");
            String appPath = Application.StartupPath;
            String filePath = appPath + "\\minute\\" + FCStrEx.convertDBCodeToFileName(code);
            FCFile.write(filePath, sb.ToString());
        }

        /// <summary>
        /// 合并日线数据
        /// </summary>
        /// <param name="latestData">最新数据</param>
        /// <param name="oldDatas">旧的数据</param>
        public static void mergeSecurityDatas(SecurityLatestData latestData, List<SecurityData2> oldDatas)
        {
            SecurityData2 newDayData = new SecurityData2();
            DateTime dt = FCTran.numToDate(latestData.m_date);
            newDayData.m_date = FCTran.dateToNum(new DateTime(dt.Year, dt.Month, dt.Day));
            newDayData.m_close = latestData.m_close;
            newDayData.m_high = latestData.m_high;
            newDayData.m_low = latestData.m_low;
            newDayData.m_open = latestData.m_open;
            newDayData.m_openInterest = latestData.m_amount;
            newDayData.m_volume = latestData.m_volume;
            if (oldDatas.Count > 0)
            {
                if (oldDatas[oldDatas.Count - 1].m_date == newDayData.m_date)
                {
                    oldDatas[oldDatas.Count - 1] = newDayData;
                }
                else
                {
                    oldDatas.Add(newDayData);
                }
            }
            else
            {
                oldDatas.Add(newDayData);
            }
        }

        /// <summary>
        /// 合并分时线数据
        /// </summary>
        /// <param name="datas">老数据</param>
        /// <param name="latestDatas">新数据</param>
        public static void mergeMinuteSecurityDatas(List<SecurityData2> datas, List<SecurityData2> latestDatas)
        {
            for (int i = 0; i < latestDatas.Count; i++)
            {
                SecurityData2 newMinuteData = latestDatas[i];
                if (datas.Count > 0)
                {
                    if (datas[datas.Count - 1].m_date == newMinuteData.m_date)
                    {
                        datas[datas.Count - 1] = newMinuteData;
                    }
                    else
                    {
                        datas.Add(newMinuteData);
                    }
                }
                else
                {
                    datas.Add(newMinuteData);
                }
            }
        }

        /// <summary>
        /// 合并实时数据
        /// </summary>
        /// <param name="latestData">新数据</param>
        /// <param name="oldCache">旧数据缓存</param>
        public static void mergeRunTimeDatas(SecurityLatestData latestData, MinuteDatasCache oldCache)
        {
            double subVolume = latestData.m_volume - oldCache.m_lastVolume;
            double subAmount = latestData.m_amount - oldCache.m_lastAmount;
            if (oldCache.m_lastDate == 0)
            {
                subVolume = 0;
                subAmount = 0;
            }
            DateTime dt = FCTran.numToDate(latestData.m_date);
            double date = FCTran.dateToNum(new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0));
            //A股
            if (dt.Hour + dt.Minute * 60 < 570)
            {
                date = FCTran.dateToNum(new DateTime(dt.Year, dt.Month, dt.Day, 9, 30, 0));
                latestData.m_date = date;
            }
            //更新
            if (oldCache.m_lastDate == date)
            {
                SecurityData2 newDayData = oldCache.m_datas[oldCache.m_datas.Count - 1];
                newDayData.m_close = latestData.m_close;
                if (newDayData.m_high < latestData.m_close)
                {
                    newDayData.m_high = latestData.m_close;
                }
                if (newDayData.m_low > latestData.m_close)
                {
                    newDayData.m_low = latestData.m_close;
                }
                newDayData.m_openInterest += subAmount;
                newDayData.m_volume += subVolume;
                oldCache.m_lastAmount = latestData.m_amount;
                oldCache.m_lastVolume = latestData.m_volume;
            }
            //新增
            else
            {
                if (oldCache.m_lastDate != 0 && oldCache.m_datas.Count > 0)
                {
                    DateTime oldDate = FCTran.numToDate(oldCache.m_lastDate);
                    int oldMinute = oldDate.Hour * 60 + oldDate.Minute;
                    int thisMinute = dt.Hour * 60 + dt.Minute;
                    //填充K线
                    List<int> emptyDates = new List<int>();
                    bool isEmpty = false;
                    for (int i = 0; i < m_shTradeTimes.Count; i++)
                    {
                        if (!isEmpty)
                        {
                            if (m_shTradeTimes[i] == oldMinute)
                            {
                                isEmpty = true;
                            }
                        }
                        else
                        {
                            if (m_shTradeTimes[i] == oldMinute)
                            {
                                break;
                            }
                            else
                            {
                                emptyDates.Add(m_shTradeTimes[i]);
                            }
                        }
                    }
                    int emptyDatesSize = emptyDates.Count;
                    if (emptyDatesSize > 0)
                    {
                        double lastClose = oldCache.m_datas[oldCache.m_datas.Count - 1].m_close;
                        for (int i = 0; i < emptyDatesSize; i++)
                        {
                            SecurityData2 emptyData = new SecurityData2();
                            emptyData.m_date = FCTran.dateToNum(new DateTime(dt.Year, dt.Month, dt.Day, emptyDates[i] / 60, emptyDates[i] % 60, 0));
                            emptyData.m_close = lastClose;
                            emptyData.m_high = lastClose;
                            emptyData.m_low = lastClose;
                            emptyData.m_open = lastClose;
                            emptyData.m_openInterest = 0;
                            emptyData.m_volume = 0;
                            oldCache.m_datas.Add(emptyData);
                        }
                        emptyDates.Clear();
                    }
                }
                SecurityData2 newDayData = new SecurityData2();
                newDayData.m_date = date;
                newDayData.m_close = latestData.m_close;
                newDayData.m_high = latestData.m_close;
                newDayData.m_low = latestData.m_close;
                newDayData.m_open = latestData.m_close;
                newDayData.m_openInterest = subVolume;
                newDayData.m_volume = subAmount;
                oldCache.m_lastDate = date;
                oldCache.m_lastAmount = latestData.m_amount;
                oldCache.m_lastVolume = latestData.m_volume;
                oldCache.m_datas.Add(newDayData);
            }
        }

        /// <summary>
        /// 获取指定年月日的星期
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <returns>星期</returns>
        public static int dayOfWeek(int y, int m, int d)
        {
            if (m == 1 || m == 2)
            {
                m += 12;
                y--;
            }
            return (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7;
        }

        /// <summary>
        /// 获取周线的历史数据
        /// </summary>
        /// <param name="weekDatas">周线历史数据</param>
        /// <param name="dayDatas">日线历史数据</param>
        /// <returns>状态</returns>
        public static int getHistoryWeekDatas(List<SecurityData2> weekDatas, List<SecurityData2> dayDatas)
        {
            int dayDatasSize = dayDatas.Count;
            if (dayDatasSize > 0)
            {
                SecurityData2 weekData = new SecurityData2();
                int lDayOfWeek = 0, lDays = 0;
                if (weekDatas.Count > 0)
                {
                    weekData = weekDatas[weekDatas.Count - 1];
                    DateTime lDt = FCTran.numToDate(weekData.m_date);
                    int year = lDt.Year, month = lDt.Month, day = lDt.Day;
                    lDayOfWeek = dayOfWeek(year, month, day);
                    lDays = (int)(weekData.m_date / (3600 * 24));
                }
                else
                {
                    weekData.copy(dayDatas[0]);
                }
                for (int i = 0; i < dayDatasSize; i++)
                {
                    SecurityData2 dayData = new SecurityData2();
                    dayData.copy(dayDatas[i]);
                    int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0, days = (int)dayData.m_date / (3600 * 24);
                    FCTran.getDateByNum(dayData.m_date, ref year, ref month, ref day, ref hour, ref minute, ref second, ref ms);
                    int dow = dayOfWeek(year, month, day);
                    bool isNextWeek = true;
                    bool add = false;
                    if (days - lDays <= 5)
                    {
                        if (days != lDays)
                        {
                            isNextWeek = dow <= lDayOfWeek;
                        }
                    }
                    if (isNextWeek || i == dayDatasSize - 1)
                    {
                        add = true;
                    }
                    if (!isNextWeek)
                    {
                        weekData.m_close = dayData.m_close;
                        weekData.m_openInterest += dayData.m_openInterest;
                        weekData.m_volume += dayData.m_volume;
                        if (weekData.m_high < dayData.m_high)
                        {
                            weekData.m_high = dayData.m_high;
                        }
                        if (weekData.m_low > dayData.m_low)
                        {
                            weekData.m_low = dayData.m_low;
                        }

                    }
                    if (add)
                    {
                        weekDatas.Add(weekData);
                        weekData = dayData;
                    }
                    if (isNextWeek && i == dayDatasSize - 1)
                    {
                        weekData = dayData;
                        weekDatas.Add(weekData);
                    }
                    lDayOfWeek = dow;
                    lDays = days;
                }
            }
            return 1;
        }

        /// <summary>
        /// 获取月线的历史数据
        /// </summary>
        /// <param name="weekDatas">月线历史数据</param>
        /// <param name="dayDatas">日线历史数据</param>
        /// <returns>状态</returns>
        public static int getHistoryMonthDatas(List<SecurityData2> monthDatas, List<SecurityData2> dayDatas)
        {
            int monthDatasSize = dayDatas.Count;
            if (monthDatasSize > 0)
            {
                SecurityData2 monthData = new SecurityData2();
                int lYear = 0, lMonth = 0, lDay = 0;
                if (monthDatas.Count > 0)
                {
                    monthData = monthDatas[monthDatas.Count - 1];
                    DateTime lDt = FCTran.numToDate(monthData.m_date);
                    lYear = lDt.Year;
                    lMonth = lDt.Month;
                    lDay = lDt.Day;
                }
                else
                {
                    monthData.copy(dayDatas[0]);
                }
                for (int i = 0; i < monthDatasSize; i++)
                {
                    SecurityData2 dayData = new SecurityData2();
                    dayData.copy(dayDatas[i]);
                    int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0;
                    FCTran.getDateByNum(dayData.m_date, ref year, ref month, ref day, ref hour, ref minute, ref second, ref ms);
                    bool isNextMonth = year * 12 + month > lYear * 12 + lMonth;
                    bool add = false;
                    if (i == monthDatasSize - 1 || (i > 0 && isNextMonth))
                    {
                        add = true;
                    }
                    if (!isNextMonth)
                    {
                        monthData.m_close = dayData.m_close;
                        monthData.m_openInterest += dayData.m_openInterest;
                        monthData.m_volume += dayData.m_volume;
                        if (monthData.m_high < dayData.m_high)
                        {
                            monthData.m_high = dayData.m_high;
                        }
                        if (monthData.m_low > dayData.m_low)
                        {
                            monthData.m_low = dayData.m_low;
                        }
                    }
                    if (add)
                    {
                        monthDatas.Add(monthData);
                        monthData = dayData;
                    }
                    if (isNextMonth && i == monthDatasSize - 1)
                    {
                        monthData = dayData;
                        monthDatas.Add(monthData);
                    }
                    lYear = year;
                    lMonth = month;
                    lDay = day;
                }
            }
            return 1;
        }

        /// <summary>
        /// 计算多分钟线
        /// </summary>
        /// <param name="newDatas">新数据</param>
        /// <param name="minuteDatas">分钟线数据</param>
        /// <param name="cycle">周期</param>
        public static void multiMinuteSecurityDatas(List<SecurityData2> newDatas, List<SecurityData2> minuteDatas, int cycle)
        {
            int lastMinutes = 0;
            for (int i = 0; i < minuteDatas.Count; i++)
            {
                SecurityData2 minuteData = minuteDatas[i];
                int minutes = (int)(minuteData.m_date / 60);
                if (lastMinutes == 0)
                {
                    lastMinutes = minutes;
                }
                //更新
                if (newDatas.Count > 0 && minutes - lastMinutes < cycle)
                {
                    SecurityData2 lastData = newDatas[newDatas.Count - 1];
                    lastData.m_close = minuteData.m_close;
                    if (minuteData.m_high > lastData.m_high)
                    {
                        lastData.m_high = minuteData.m_high;
                    }
                    if (minuteData.m_low < lastData.m_low)
                    {
                        lastData.m_low = minuteData.m_low;
                    }
                    lastData.m_openInterest += minuteData.m_openInterest;
                    lastData.m_volume += minuteData.m_volume;
                    //新增
                }
                else
                {
                    SecurityData2 newData = new SecurityData2();
                    newData.m_date = minuteData.m_date;
                    newData.m_high = minuteData.m_high;
                    newData.m_low = minuteData.m_low;
                    newData.m_open = minuteData.m_open;
                    newData.m_close = minuteData.m_close;
                    newData.m_openInterest = minuteData.m_openInterest;
                    newData.m_volume = minuteData.m_volume;
                    newDatas.Add(newData);
                    lastMinutes = minutes;
                }
            }
        }

        /// <summary>
        /// 合并最新数据
        /// </summary>
        /// <param name="oldDatas">旧数据</param>
        /// <param name="latestData">最新数据</param>
        /// <param name="tickDataCache">TICK数据</param>
        /// <param name="cycle">周期</param>
        public static void mergeLatestData(List<SecurityData2> oldDatas, SecurityLatestData latestData, ClientTickDataCache tickDataCache, int cycle)
        {
            DateTime newDate = FCTran.numToDate(latestData.m_date);
            if (newDate.Hour + newDate.Minute * 60 < 570)
            {
                newDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 9, 30, 0);
                latestData.m_date = FCTran.dateToNum(newDate);
            }
            bool isNextCycle = true;
            if (cycle < 1440)
            {
                if (oldDatas.Count > 0)
                {
                    int newMinutes = (int)(latestData.m_date / 60);
                    int lastMinutes = (int)(oldDatas[oldDatas.Count - 1].m_date / 60);
                    isNextCycle = newMinutes - lastMinutes > cycle;
                }
            }
            else
            {
                if (cycle == 1440)
                {
                    if (oldDatas.Count > 0)
                    {
                        DateTime lastDate = FCTran.numToDate(oldDatas[oldDatas.Count - 1].m_date);
                        isNextCycle = FCTran.dateToNum(new DateTime(newDate.Year, newDate.Month, newDate.Day))
                        != FCTran.dateToNum(new DateTime(lastDate.Year, lastDate.Month, lastDate.Day));
                    }
                }
                else if (cycle == 10080)
                {
                    if (oldDatas.Count > 0)
                    {
                        DateTime lastDate = FCTran.numToDate(oldDatas[oldDatas.Count - 1].m_date);
                        int newDayOfWeek = dayOfWeek(newDate.Year, newDate.Month, newDate.Day);
                        int lDayOfWeek = dayOfWeek(lastDate.Year, lastDate.Month, lastDate.Day);
                        int newTotalDays = (int)(latestData.m_date / (3600 * 24));
                        int lastTotalDays = (int)(oldDatas[oldDatas.Count - 1].m_date / (3600 * 24));
                        if (newDayOfWeek - lDayOfWeek <= 5)
                        {
                            if (newTotalDays !=
                               lastTotalDays)
                            {
                                isNextCycle = newDayOfWeek <= lDayOfWeek;
                            }
                            else
                            {
                                isNextCycle = false;
                            }
                        }
                    }
                }
                else if (cycle == 43200)
                {
                    if (oldDatas.Count > 0)
                    {
                        DateTime lastDate = FCTran.numToDate(oldDatas[oldDatas.Count - 1].m_date);
                        isNextCycle = (newDate.Year * 12 + newDate.Month != lastDate.Year * 12 + lastDate.Month);
                    }
                }
            }
            if (isNextCycle)
            {
                SecurityData2 newCycleData = new SecurityData2();
                newCycleData.m_date = latestData.m_date;
                newCycleData.m_close = latestData.m_close;
                newCycleData.m_high = latestData.m_close;
                newCycleData.m_low = latestData.m_close;
                newCycleData.m_open = latestData.m_close;
                newCycleData.m_volume = latestData.m_amount - tickDataCache.m_lastAmount;
                newCycleData.m_openInterest = latestData.m_amount - tickDataCache.m_lastAmount;
                oldDatas.Add(newCycleData);
            }
            else
            {
                SecurityData2 lastCycleData = oldDatas[oldDatas.Count - 1];
                lastCycleData.m_close = latestData.m_close;
                if (lastCycleData.m_high < latestData.m_close)
                {
                    lastCycleData.m_high = latestData.m_close;
                }
                if (lastCycleData.m_low > lastCycleData.m_close)
                {
                    lastCycleData.m_low = latestData.m_close;
                }
                lastCycleData.m_openInterest += latestData.m_amount - tickDataCache.m_lastAmount;
                lastCycleData.m_volume += latestData.m_volume - tickDataCache.m_lastVolume;
            }
            tickDataCache.m_code = latestData.m_code;
            tickDataCache.m_lastAmount = latestData.m_amount;
            tickDataCache.m_lastDate = latestData.m_date;
            tickDataCache.m_lastVolume = latestData.m_volume;
        }
    }
}
