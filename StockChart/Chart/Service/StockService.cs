/*
* FaceCat图形通讯框架(非开源)
* 著作权编号:2015SR229355+2020SR0266727
* 上海卷卷猫信息技术有限公司
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using FaceCat;

namespace chart
{
    /// <summary>
    /// 股票服务
    /// </summary>
    public class StockService
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
        /// 上证交易时间
        /// </summary>
        public static double m_shTradeTime;

        /// <summary>
        /// 复权数据
        /// </summary>
        public static Dictionary<String, List<List<OneDivideRightBase>>> m_devideRightDatas = new Dictionary<string, List<List<OneDivideRightBase>>>();

        /// <summary>
        /// 下载复权因子
        /// </summary>
        public static Dictionary<String, List<List<OneDivideRightBase>>> getDevideRightDatas()
        {
            Dictionary<String, List<List<OneDivideRightBase>>> datas = new Dictionary<string, List<List<OneDivideRightBase>>>();
            String filePath = Application.StartupPath + "\\DR.txt";
            String content = "";
            FCFile.read(filePath, ref content);
            if (content != null && content.Length > 0)
            {
                String[] strs = content.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                String curCode = "";
                List<List<OneDivideRightBase>> oneDatas = new List<List<OneDivideRightBase>>();
                List<OneDivideRightBase> oneDatas2 = new List<OneDivideRightBase>();
                double date = -1;
                for (int i = 0; i < strs.Length; i++)
                {
                    String str = strs[i];
                    if (str.IndexOf("CODE=") != -1)
                    {
                        if (curCode.Length > 0)
                        {
                            datas[curCode] = oneDatas;
                            oneDatas = new List<List<OneDivideRightBase>>();
                            oneDatas2 = new List<OneDivideRightBase>();
                            date = -1;
                        }
                        curCode = str.Substring(str.IndexOf("=") + 1);

                    }
                    else
                    {
                        String[] subStrs = str.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        OneDivideRightBase oneDivideRightBase = new OneDivideRightBase();
                        int intDate = FCTran.strToInt(subStrs[0]);
                        int year = intDate / 10000;
                        int month = (intDate / 100) % 100;
                        int day = intDate % 100;
                        oneDivideRightBase.Date = FCTran.getDateNum(year, month, day, 0, 0, 0, 0);
                        oneDivideRightBase.DataValue = FCTran.strToFloat(subStrs[1]);
                        oneDivideRightBase.DivideType = (DivideRightType)Enum.Parse(typeof(DivideRightType), subStrs[2]);
                        oneDivideRightBase.PClose = FCTran.strToFloat(subStrs[3]);
                        oneDivideRightBase.Factor = FCTran.strToFloat(subStrs[4]);
                        if (date == -1)
                        {
                            oneDatas2.Add(oneDivideRightBase);
                            date = oneDivideRightBase.Date;
                            oneDatas.Add(oneDatas2);
                        }
                        else
                        {
                            if (date == oneDivideRightBase.Date)
                            {
                                oneDatas2.Add(oneDivideRightBase);
                            }
                            else
                            {
                                oneDatas2 = new List<OneDivideRightBase>();
                                oneDatas2.Add(oneDivideRightBase);
                                oneDatas.Add(oneDatas2);
                                date = oneDivideRightBase.Date;
                            }
                        }
                    }
                }
            }
            return datas;
        }

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
        /// 加载数据
        /// </summary>
        public static void load()
        {
            m_devideRightDatas = getDevideRightDatas();
            //加载代码表//step 1
            m_codes = "";
            if (m_codedMap.Count == 0)
            {
                ArrayList<String> files = new ArrayList<String>();
                FCFile.getFiles(Application.StartupPath + "\\day", files);
                int filesSize = files.size();
                for (int i = 0; i < filesSize; i++)
                {
                    FileStream fs = new FileStream(files.get(i), FileMode.Open);
                    StreamReader sr = new StreamReader(fs, Encoding.Default);

                    String headLine = sr.ReadLine();
                    String[] substrs = headLine.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (substrs.Length >= 2)
                    {
                        Security security = new Security();
                        String code = substrs[0];
                        if (code.IndexOf(".") == -1)
                        {
                            if (code.IndexOf("6") == 0 || (substrs[1] == "上证指数"))
                            {
                                code += ".SH";
                            }
                            else
                            {
                                code += ".SZ";
                            }
                        }
                        security.m_code = code;
                        security.m_name = substrs[1];
                        if (substrs.Length > 3 && substrs[3] == "日线")
                        {
                            security.m_name += substrs[2];
                        }
                        else if (substrs.Length > 4 && substrs[4] == "日线")
                        {
                            security.m_name += substrs[2] + substrs[3];
                        }
                        m_codedMap[security.m_code] = security;
                        m_codes += security.m_code;
                        m_codes += ",";
                    }
                    sr.Dispose();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 代码列表
        /// </summary>
        private static String m_codes;

        static StockService()
        {
            for (int i = 570; i <= 900; i++)
            {
                if (i < 690 && i > 780)
                {
                    m_shTradeTimes.Add(570);
                }
            }
        }

        /// <summary>
        /// 上证的交易时间
        /// </summary>
        public static List<int> m_shTradeTimes = new List<int>();

        /// <summary>
        /// 获取日线数据
        /// </summary>
        /// <returns></returns>
        public static List<SecurityData> getSecurityDatas(String code, ref String name)
        {
            List<SecurityData> datas = new List<SecurityData>();
            String appPath = Application.StartupPath;
            String filePath = appPath + "\\day\\" + FCStrEx.convertDBCodeToFileName(code);
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
                    SecurityData securityData = new SecurityData();
                    securityData.m_date = FCTran.dateToNum(Convert.ToDateTime(subStrs[0]));
                    securityData.m_open = FCTran.strToDouble(subStrs[1]);
                    securityData.m_high = FCTran.strToDouble(subStrs[2]);
                    securityData.m_low = FCTran.strToDouble(subStrs[3]);
                    securityData.m_close = FCTran.strToDouble(subStrs[4]);
                    securityData.m_volume = FCTran.strToDouble(subStrs[5]);
                    securityData.m_amount = FCTran.strToDouble(subStrs[6]);
                    datas.Add(securityData);
                }
            }
            return datas;
        }

        /// <summary>
        /// 获取分时线数据
        /// </summary>
        /// <returns></returns>
        public static List<SecurityData> getSecurityMinuteDatas(String code, ref String name)
        {
            List<SecurityData> datas = new List<SecurityData>();
            String appPath = Application.StartupPath;
            String filePath = appPath + "\\minute\\" + FCStrEx.convertDBCodeToFileName(code);
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
                SecurityData securityData = new SecurityData();
                int hour = FCTran.strToInt(subStrs[1].Substring(0, 2));
                int minute = FCTran.strToInt(subStrs[1].Substring(2, 2));
                DateTime dt = Convert.ToDateTime(subStrs[0]);
                securityData.m_date = FCTran.dateToNum(new DateTime(dt.Year, dt.Month, dt.Day, hour, minute, 0));
                securityData.m_open = FCTran.strToDouble(subStrs[2]);
                securityData.m_high = FCTran.strToDouble(subStrs[3]);
                securityData.m_low = FCTran.strToDouble(subStrs[4]);
                securityData.m_close = FCTran.strToDouble(subStrs[5]);
                securityData.m_volume = FCTran.strToDouble(subStrs[6]);
                securityData.m_amount = FCTran.strToDouble(subStrs[7]);
                datas.Add(securityData);
            }
            return datas;
        }

        /// <summary>
        /// 写日线数据
        /// </summary>
        /// <param name="datas"></param>
        public static void writeSecurityDatas(String code, String name, List<SecurityData> datas)
        {
            int datasSize = datas.Count;
            StringBuilder sb = new StringBuilder();
            sb.Append(code + " " + name + " 日线\r\n");
            sb.Append("日期\t开盘\t最高\t最低\t收盘\t成交量\t成交额\r\n");
            for (int i = 0; i < datasSize; i++)
            {
                SecurityData data = datas[i];
                DateTime dateTime = FCTran.numToDate(data.m_date);
                sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6}\r\n",
                    dateTime.ToString("yyyy-MM-dd"), data.m_open, data.m_high,
                    data.m_low, data.m_close, data.m_volume, data.m_amount));
            }
            sb.Append("数据来源:卷卷猫");
            String appPath = Application.StartupPath;
            String filePath = appPath + "\\day\\" + FCStrEx.convertDBCodeToFileName(code);
            FCFile.write(filePath, sb.ToString());
        }

        public static void writeMinuteSecurityDatas(String code, String name, List<SecurityData> datas)
        {
            int datasSize = datas.Count;
            StringBuilder sb = new StringBuilder();
            sb.Append(code + " " + name + " 1分钟线\r\n");
            sb.Append("日期\t时间\t开盘\t最高\t最低\t收盘\t成交量\t成交额\r\n");
            for (int i = 0; i < datasSize; i++)
            {
                SecurityData data = datas[i];
                DateTime dateTime = FCTran.numToDate(data.m_date);
                sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6},{7}\r\n",
                    dateTime.ToString("yyyy-MM-dd"), dateTime.ToString("HHmm"), data.m_open, data.m_high,
                    data.m_low, data.m_close, data.m_volume, data.m_amount));
            }
            sb.Append("数据来源:卷卷猫");
            String appPath = Application.StartupPath;
            String filePath = appPath + "\\minute\\" + FCStrEx.convertDBCodeToFileName(code);
            FCFile.write(filePath, sb.ToString());
        }

        /// <summary>
        /// 合并日线数据
        /// </summary>
        /// <param name="latestData"></param>
        /// <param name="datas"></param>
        public static void mergeSecurityDatas(SecurityLatestData latestData, List<SecurityData> oldDatas)
        {
            SecurityData newDayData = new SecurityData();
            DateTime dt = FCTran.numToDate(latestData.m_date);
            newDayData.m_date = FCTran.dateToNum(new DateTime(dt.Year, dt.Month, dt.Day));
            newDayData.m_close = latestData.m_close;
            newDayData.m_high = latestData.m_high;
            newDayData.m_low = latestData.m_low;
            newDayData.m_open = latestData.m_open;
            newDayData.m_amount = latestData.m_amount;
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
        /// <param name="latestDatas"></param>
        /// <param name="datas"></param>
        public static void mergeMinuteSecurityDatas(List<SecurityData> datas, List<SecurityData> latestDatas)
        {
            for (int i = 0; i < latestDatas.Count; i++)
            {
                SecurityData newMinuteData = latestDatas[i];
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
                SecurityData newDayData = oldCache.m_datas[oldCache.m_datas.Count - 1];
                newDayData.m_close = latestData.m_close;
                if (newDayData.m_high < latestData.m_close)
                {
                    newDayData.m_high = latestData.m_close;
                }
                if (newDayData.m_low > latestData.m_close)
                {
                    newDayData.m_low = latestData.m_close;
                }
                newDayData.m_amount += subAmount;
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
                            SecurityData emptyData = new SecurityData();
                            emptyData.m_date = FCTran.dateToNum(new DateTime(dt.Year, dt.Month, dt.Day, emptyDates[i] / 60, emptyDates[i] % 60, 0));
                            emptyData.m_close = lastClose;
                            emptyData.m_high = lastClose;
                            emptyData.m_low = lastClose;
                            emptyData.m_open = lastClose;
                            emptyData.m_amount = 0;
                            emptyData.m_volume = 0;
                            oldCache.m_datas.Add(emptyData);
                        }
                        emptyDates.Clear();
                    }
                }
                SecurityData newDayData = new SecurityData();
                newDayData.m_date = date;
                newDayData.m_close = latestData.m_close;
                newDayData.m_high = latestData.m_close;
                newDayData.m_low = latestData.m_close;
                newDayData.m_open = latestData.m_close;
                newDayData.m_amount = subVolume;
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
        public static int getHistoryWeekDatas(List<SecurityData> weekDatas, List<SecurityData> dayDatas)
        {
            int dayDatasSize = dayDatas.Count;
            if (dayDatasSize > 0)
            {
                SecurityData weekData = new SecurityData();
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
                    SecurityData dayData = new SecurityData();
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
                        weekData.m_amount += dayData.m_amount;
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
        public static int getHistoryMonthDatas(List<SecurityData> monthDatas, List<SecurityData> dayDatas)
        {
            int monthDatasSize = dayDatas.Count;
            if (monthDatasSize > 0)
            {
                SecurityData monthData = new SecurityData();
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
                    SecurityData dayData = new SecurityData();
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
                        monthData.m_amount += dayData.m_amount;
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
        /// <param name="minuteDatas"></param>
        /// <param name="newDatas"></param>
        /// <param name="cycle"></param>
        public static void multiMinuteSecurityDatas(List<SecurityData> newDatas, List<SecurityData> minuteDatas, int cycle)
        {
            int lastMinutes = 0;
            for (int i = 0; i < minuteDatas.Count; i++)
            {
                SecurityData minuteData = minuteDatas[i];
                int minutes = (int)(minuteData.m_date / 60);
                if (lastMinutes == 0)
                {
                    lastMinutes = minutes;
                }
                //更新
                if (newDatas.Count > 0 && minutes - lastMinutes < cycle)
                {
                    SecurityData lastData = newDatas[newDatas.Count - 1];
                    lastData.m_close = minuteData.m_close;
                    if (minuteData.m_high > lastData.m_high)
                    {
                        lastData.m_high = minuteData.m_high;
                    }
                    if (minuteData.m_low < lastData.m_low)
                    {
                        lastData.m_low = minuteData.m_low;
                    }
                    lastData.m_amount += minuteData.m_amount;
                    lastData.m_volume += minuteData.m_volume;
                    //新增
                }
                else
                {
                    SecurityData newData = new SecurityData();
                    newData.m_date = minuteData.m_date;
                    newData.m_high = minuteData.m_high;
                    newData.m_low = minuteData.m_low;
                    newData.m_open = minuteData.m_open;
                    newData.m_close = minuteData.m_close;
                    newData.m_amount = minuteData.m_amount;
                    newData.m_volume = minuteData.m_volume;
                    newDatas.Add(newData);
                    lastMinutes = minutes;
                }
            }
        }

        /// <summary>
        /// 合并最新数据
        /// </summary>
        /// <param name="oldDatas"></param>
        /// <param name="latestData"></param>
        /// <param name="tickDataCache"></param>
        /// <param name="cycle"></param>
        public static void mergeLatestData(List<SecurityData> oldDatas, SecurityLatestData latestData, ClientTickDataCache tickDataCache, int cycle)
        {
            DateTime newDate = FCTran.numToDate(latestData.m_date);
            //A股
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
                SecurityData newCycleData = new SecurityData();
                newCycleData.m_date = latestData.m_date;
                newCycleData.m_close = latestData.m_close;
                newCycleData.m_high = latestData.m_close;
                newCycleData.m_low = latestData.m_close;
                newCycleData.m_open = latestData.m_close;
                newCycleData.m_volume = latestData.m_amount - tickDataCache.m_lastAmount;
                newCycleData.m_amount = latestData.m_amount - tickDataCache.m_lastAmount;
                oldDatas.Add(newCycleData);
            }
            else
            {
                SecurityData lastCycleData = oldDatas[oldDatas.Count - 1];
                lastCycleData.m_close = latestData.m_close;
                if (lastCycleData.m_high < latestData.m_close)
                {
                    lastCycleData.m_high = latestData.m_close;
                }
                if (lastCycleData.m_low > lastCycleData.m_close)
                {
                    lastCycleData.m_low = latestData.m_close;
                }
                lastCycleData.m_amount += latestData.m_amount - tickDataCache.m_lastAmount;
                lastCycleData.m_volume += latestData.m_volume - tickDataCache.m_lastVolume;
            }
            tickDataCache.m_code = latestData.m_code;
            tickDataCache.m_lastAmount = latestData.m_amount;
            tickDataCache.m_lastDate = latestData.m_date;
            tickDataCache.m_lastVolume = latestData.m_volume;
        }

        /// <summary>
        /// 计算复权，只改变复权因子
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <param name="isCycleYear"></param>
        /// <param name="isForward"></param>
        /// <returns></returns>
        public static void caculateDivideKLineData(List<SecurityData> data, List<List<OneDivideRightBase>> divideData, bool isForward)
        {
            if (divideData == null || data == null)
                return;
            float factor = 1;
            for (int i = 0; i < divideData.Count; i++)
                factor *= divideData[i][0].Factor;

            int indexLast = 0;
            if (isForward)
            {
                indexLast = -1;
                for (int i = 0; i < divideData.Count; i++)
                {
                    if (factor == 0 || factor == 1)
                        continue;
                    for (int j = indexLast + 1; j < data.Count; j++)
                    {
                        if (data[j].m_date < divideData[i][0].Date)
                        {
                            data[j].m_forwardFactor = factor;
                            indexLast = j;
                        }
                        else
                        {
                            if (i == divideData.Count - 1)
                                data[j].m_forwardFactor = 1.0f;
                            else
                                break;
                        }

                    }
                    factor /= divideData[i][0].Factor;
                }
            }
            else
            {
                indexLast = data.Count;
                for (int i = divideData.Count - 1; i >= 0; i--)
                {
                    if (divideData[i][0].Factor == 0 || divideData[i][0].Factor == 1)
                        continue;

                    for (int j = indexLast - 1; j >= 0; j--)
                    {
                        if (data[j].m_date >= divideData[i][0].Date)
                        {
                            data[j].m_backFactor = factor;
                            indexLast = j;
                        }
                        else
                        {
                            if (i == 0)
                                data[j].m_backFactor = 1.0f;
                            else
                                break;
                        }
                    }
                    factor /= divideData[i][0].Factor;
                }
            }
        }

        /// <summary>
        /// 计算复权，只改变复权因子
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <param name="isCycleYear"></param>
        /// <param name="isForward"></param>
        /// <returns></returns>
        public static float[] caculateDivideKLineData2(List<SecurityData> data, List<List<OneDivideRightBase>> divideData, bool isForward)
        {
            if (divideData == null || data == null)
                return null;
            float[] ArrResult = new float[data.Count];
            float factor = 1;
            for (int index = 0; index < data.Count; index++)
            {
                if (index < ArrResult.Length)
                    ArrResult[index] = factor;
            }
            if (divideData.Count == 0)
                return ArrResult;
            for (int i = 0; i < divideData.Count; i++)
                factor *= divideData[i][0].Factor;

            int indexLast = 0;
            if (isForward)
            {
                indexLast = -1;
                for (int i = 0; i < divideData.Count; i++)
                {
                    if (factor == 0 || factor == 1)
                        continue;
                    for (int j = indexLast + 1; j < data.Count; j++)
                    {
                        if (j >= ArrResult.Length)
                            continue;
                        if (data == null || divideData == null || data[j] == null
                            || divideData[i] == null || divideData[i][0] == null)
                        {
                            int s = 0;
                        }
                        if (data[j].m_date < divideData[i][0].Date)
                        {
                            ArrResult[j] = factor;
                            indexLast = j;
                        }
                        else
                        {
                            if (i == divideData.Count - 1)
                                ArrResult[j] = 1.0f;
                            else
                                break;
                        }
                    }
                    factor /= divideData[i][0].Factor;
                }
            }
            else
            {
                indexLast = data.Count;
                for (int i = divideData.Count - 1; i >= 0; i--)
                {
                    if (divideData[i][0].Factor == 0 || divideData[i][0].Factor == 1)
                        continue;

                    for (int j = indexLast - 1; j >= 0; j--)
                    {
                        if (j >= ArrResult.Length)
                            continue;
                        if (data[j].m_date >= divideData[i][0].Date)
                        {
                            ArrResult[j] = factor;
                            indexLast = j;
                        }
                        else
                        {
                            if (i == 0)
                                ArrResult[j] = 1.0f;
                            else
                                break;
                        }
                    }
                    factor /= divideData[i][0].Factor;
                }
            }

            return ArrResult;
        }

        /// <summary>
        /// 获得复权数据
        /// </summary>
        /// <param name="divideType">复权类型</param>
        /// <param name="data">原始数据</param>
        /// <param name="factor">复权因子</param>
        /// <returns>复权后的k线数据</returns>
        public static void getDataAfterDivide(IsDivideRightType divideType, SecurityData data, float factor)
        {
            if (data == null)
                return;
            float tempFactor = 1.0F;
            if (divideType == IsDivideRightType.Forward)
                tempFactor = factor;
            else if (divideType == IsDivideRightType.Backward)
                tempFactor = 1.0F / factor;

            data.m_high = data.m_high * tempFactor;
            data.m_open = data.m_open * tempFactor;
            data.m_low = data.m_low * tempFactor;
            data.m_close = data.m_close * tempFactor;
        }

        /// <summary>
        /// 获得复权数据
        /// </summary>
        /// <param name="divideType">复权类型</param>
        /// <param name="data">原始数据</param>
        /// <param name="factor">复权因子</param>
        /// <returns>复权后的k线数据</returns>
        public static void getDataAfterDivide(IsDivideRightType divideType, SecurityLatestData data, float factor)
        {
            if (data == null)
                return;
            float tempFactor = 1.0F;
            if (divideType == IsDivideRightType.Forward)
                tempFactor = factor;
            else if (divideType == IsDivideRightType.Backward)
                tempFactor = 1.0F / factor;

            data.m_high = data.m_high * tempFactor;
            data.m_open = data.m_open * tempFactor;
            data.m_low = data.m_low * tempFactor;
            data.m_close = data.m_close * tempFactor;
        }
    }
}
