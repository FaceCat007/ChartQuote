using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace FaceCat {
    /// <summary>
    /// 股票服务
    /// </summary>
    public class StockService {
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
        public static Dictionary<String,List<List<OneDivideRightBase>>> getDevideRightDatas()
        {
            Dictionary<String, List<List<OneDivideRightBase>>> datas = new Dictionary<string, List<List<OneDivideRightBase>>>();
            String filePath = Application.StartupPath + "\\DR.txt";
            if (FCFile.isFileExist(filePath))
            {
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
                            switch (subStrs[2])
                            {
                                case "ZengFa":
                                    oneDivideRightBase.DivideType = DivideRightType.ZengFa;
                                    break;
                                case "PeiGu":
                                    oneDivideRightBase.DivideType = DivideRightType.PeiGu;
                                    break;
                                case "PaiXi":
                                    oneDivideRightBase.DivideType = DivideRightType.PaiXi;
                                    break;
                                case "GengMing":
                                    oneDivideRightBase.DivideType = DivideRightType.GengMing;
                                    break;
                                case "SongGu":
                                    oneDivideRightBase.DivideType = DivideRightType.SongGu;
                                    break;
                                case "ZhuanZeng":
                                    oneDivideRightBase.DivideType = DivideRightType.ZhuanZeng;
                                    break;
                                case "BingGu":
                                    oneDivideRightBase.DivideType = DivideRightType.BingGu;
                                    break;
                                case "ChaiGu":
                                    oneDivideRightBase.DivideType = DivideRightType.ChaiGu;
                                    break;
                                case "Jili":
                                    oneDivideRightBase.DivideType = DivideRightType.Jili;
                                    break;
                            }
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
            }
            return datas;
        }

        /// <summary>
        /// 返回所有合约
        /// </summary>
        /// <param name="codes">合约代码集合</param>
        public static void getCodes(List<String> codes) {
            foreach (String key in m_codedMap.Keys) {
                codes.Add(key);
            }
        }

        /// <summary>
        /// 通过代码返回合约信息
        /// </summary>
        /// <param name="code">合约代码</param>
        /// <param name="sd">out 股票基本信息</param>
        public static int getSecurityByCode(String code, ref Security security) {
            int ret = 0;
            foreach (String key in m_codedMap.Keys) {
                if (key == code) {
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
        public static int getLatestData(String code, ref SecurityLatestData latestData) {
            int state = 0;
            lock (m_latestDatas) {
                if (m_latestDatas.ContainsKey(code)) {
                    latestData.copy(m_latestDatas[code]);
                    state = 1;
                }
            }
            return state;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public static void load() {
            try
            {
                m_devideRightDatas = getDevideRightDatas();
            }
            catch (Exception ex)
            {
            }
            //加载代码表//step 1
            m_codes = "";
            if (m_codedMap.Count == 0)
            {
                String content = "";
                FCFile.read(Application.StartupPath + "\\codes.txt", ref content);
                String[] strs = content.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<String, String> maps = new Dictionary<string, string>();
                for (int i = 0; i < strs.Length; i++)
                {
                    String[] subStrs = strs[i].Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    Security security = new Security();
                    String code = subStrs[0];
                    if (!maps.ContainsKey(code))
                    {
                        security.m_code = code;
                        security.m_name = subStrs[1];
                        //security.m_pingyin = subStrs[2];
                        //security.m_status = subStrs[3];
                        //security.m_market = subStrs[4];
                        security.m_pingyin = "";
                        security.m_status = "";
                        security.m_market = "";
                        m_codedMap[code] = security;
                        m_codes += code;
                        m_codes += ",";
                        maps[code] = "";
                    }
                }
            }
        }

        /// <summary>
        /// 代码列表
        /// </summary>
        private static String m_codes;

        /// <summary>
        /// 开始策略
        /// </summary>
        public static void start() {
            Thread thread = new Thread(new ThreadStart(startWork));
            thread.Start();
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        private static void startWork() {
            while (true) {
                if (m_codes != null && m_codes.Length > 0) {
                    if (m_codes.EndsWith(",")) {
                        m_codes.Remove(m_codes.Length - 1);
                    }
                    String[] strCodes = m_codes.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    int codesSize = strCodes.Length;
                    String latestCodes = "";
                    Dictionary<String, SecurityLatestData> priceDatas = new Dictionary<string, SecurityLatestData>();
                    for (int i = 0; i < codesSize; i++) {
                        latestCodes += strCodes[i];
                        if (i == codesSize - 1 || (i > 0 && i % 500 == 0)) {
                            String latestDatasResult = get163LatestDatasByCodes(latestCodes);
                            if (latestDatasResult != null && latestDatasResult.Length > 0) {
                                List<SecurityLatestData> latestDatas = new List<SecurityLatestData>();
                                getLatestDatasBy163Str(latestDatasResult, 0, latestDatas);
                                String[] subStrs = latestDatasResult.Split(new String[] { ";\n" }, StringSplitOptions.RemoveEmptyEntries);
                                int latestDatasSize = latestDatas.Count;
                                for (int j = 0; j < latestDatasSize; j++) {
                                    SecurityLatestData latestData = latestDatas[j];
                                    if (latestData.m_close == 0) {
                                        latestData.m_close = latestData.m_buyPrice1;
                                    }
                                    if (latestData.m_close == 0) {
                                        latestData.m_close = latestData.m_sellPrice1;
                                    }
                                    lock (m_latestDatas) {
                                        bool newData = false;
                                        if (!m_latestDatas.ContainsKey(latestData.m_code)) {
                                            m_latestDatas[latestData.m_code] = latestData;
                                            newData = true;
                                        } else {
                                            if (!m_latestDatas[latestData.m_code].equal(latestData)) {
                                                m_latestDatas[latestData.m_code].copy(latestData);
                                                newData = true;
                                            }
                                        }
                                        if (newData) {
                                            DataCenter.LatestService.sendData(latestData, -1);
                                            DataCenter.HistoryService.sendData(latestData);
                                            priceDatas[latestData.m_code] = latestData;
                                        }
                                        if (latestData.m_code == "600000.SH") {
                                            m_shTradeTime = latestData.m_date;
                                        }
                                    }
                                }
                                latestDatas.Clear();
                            }
                            latestCodes = "";
                        } else {
                            latestCodes += ",";
                        }
                    }
                    if (priceDatas.Count > 0) {
                        DataCenter.PriceDataService.sendDatas(priceDatas, -1);
                    }
                }
                Thread.Sleep(10000);
            }
        }

        static StockService() {
            for (int i = 570; i <= 900; i++) {
                if (i < 690 && i > 780) {
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
        public static List<SecurityData2> getSecurityDatas(String code, ref String name) {
            List<SecurityData2> datas = new List<SecurityData2>();
            String appPath = Application.StartupPath;
            String filePath = appPath + "\\day\\" + FCStrExC.convertDBCodeToFileName(code);
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
        public static List<SecurityData2> getSecurityMinuteDatas(String code, ref String name) {
            List<SecurityData2> datas = new List<SecurityData2>();
            String appPath = Application.StartupPath;
            String filePath = appPath + "\\minute\\" + FCStrExC.convertDBCodeToFileName(code);
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
                    securityData.m_amount = FCTran.strToDouble(subStrs[7]);
                    datas.Add(securityData);
                }
            }
            return datas;
        }

        /// <summary>
        /// 写日线数据
        /// </summary>
        /// <param name="datas"></param>
        public static void writeSecurityDatas(String code, String name, List<SecurityData2> datas) {
            int datasSize = datas.Count;
            StringBuilder sb = new StringBuilder();
            sb.Append(code + " " + name + " 日线\r\n");
            sb.Append("日期\t开盘\t最高\t最低\t收盘\t成交量\t成交额\r\n");
            for (int i = 0; i < datasSize; i++) {
                SecurityData2 data = datas[i];
                DateTime dateTime = FCTran.numToDate(data.m_date);
                sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6}\r\n",
                    dateTime.ToString("yyyy-MM-dd"), data.m_open, data.m_high,
                    data.m_low, data.m_close, data.m_volume, data.m_amount));
            }
            sb.Append("数据来源:花卷猫");
            String appPath = Application.StartupPath;
            String filePath = appPath + "\\day\\" + FCStrExC.convertDBCodeToFileName(code);
            FCFile.write(filePath, sb.ToString());
        }

        /// <summary>
        /// 写分钟线数据
        /// </summary>
        public static void writeMinuteSecurityDatas(String code, String name, List<SecurityData2> datas) {
            int datasSize = datas.Count;
            StringBuilder sb = new StringBuilder();
            sb.Append(code + " " + name + " 1分钟线\r\n");
            sb.Append("日期\t时间\t开盘\t最高\t最低\t收盘\t成交量\t成交额\r\n");
            for (int i = 0; i < datasSize; i++) {
                SecurityData2 data = datas[i];
                DateTime dateTime = FCTran.numToDate(data.m_date);
                sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6},{7}\r\n",
                    dateTime.ToString("yyyy-MM-dd"), dateTime.ToString("HHmm"), data.m_open, data.m_high,
                    data.m_low, data.m_close, data.m_volume, data.m_amount));
            }
            sb.Append("数据来源:花卷猫");
            String appPath = Application.StartupPath;
            String filePath = appPath + "\\minute\\" + FCStrExC.convertDBCodeToFileName(code);
            FCFile.write(filePath, sb.ToString());
        }

        /// <summary>
        /// 合并日线数据
        /// </summary>
        /// <param name="latestData"></param>
        /// <param name="datas"></param>
        public static void mergeSecurityDatas(SecurityLatestData latestData, List<SecurityData2> oldDatas) {
            SecurityData2 newDayData = new SecurityData2();
            DateTime dt = FCTran.numToDate(latestData.m_date);
            newDayData.m_date = FCTran.dateToNum(new DateTime(dt.Year, dt.Month, dt.Day));
            newDayData.m_close = latestData.m_close;
            newDayData.m_high = latestData.m_high;
            newDayData.m_low = latestData.m_low;
            newDayData.m_open = latestData.m_open;
            newDayData.m_amount = latestData.m_amount;
            newDayData.m_volume = latestData.m_volume;
            if (oldDatas.Count > 0) {
                if (oldDatas[oldDatas.Count - 1].m_date == newDayData.m_date) {
                    oldDatas[oldDatas.Count - 1] = newDayData;
                } else {
                    oldDatas.Add(newDayData);
                }
            } else {
                oldDatas.Add(newDayData);
            }
        }

        /// <summary>
        /// 合并分时线数据
        /// </summary>
        /// <param name="latestDatas"></param>
        /// <param name="datas"></param>
        public static void mergeMinuteSecurityDatas(List<SecurityData2> datas, List<SecurityData2> latestDatas) {
            for (int i = 0; i < latestDatas.Count; i++) {
                SecurityData2 newMinuteData = latestDatas[i];
                if (datas.Count > 0) {
                    if (datas[datas.Count - 1].m_date == newMinuteData.m_date) {
                        datas[datas.Count - 1] = newMinuteData;
                    } else {
                        datas.Add(newMinuteData);
                    }
                } else {
                    datas.Add(newMinuteData);
                }
            }
        }

        /// <summary>
        /// 合并实时数据
        /// </summary>
        public static void mergeRunTimeDatas(SecurityLatestData latestData, MinuteDatasCache oldCache) {
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
            if (dt.Hour + dt.Minute * 60 < 570) {
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
                            SecurityData2 emptyData = new SecurityData2();
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
                SecurityData2 newDayData = new SecurityData2();
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
        public static int dayOfWeek(int y, int m, int d) {
            if (m == 1 || m == 2) {
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
        public static int getHistoryWeekDatas(List<SecurityData2> weekDatas, List<SecurityData2> dayDatas) {
            int dayDatasSize = dayDatas.Count;
            if (dayDatasSize > 0) {
                SecurityData2 weekData = new SecurityData2();
                int lDayOfWeek = 0, lDays = 0;
                if (weekDatas.Count > 0) {
                    weekData = weekDatas[weekDatas.Count - 1];
                    DateTime lDt = FCTran.numToDate(weekData.m_date);
                    int year = lDt.Year, month = lDt.Month, day = lDt.Day;
                    lDayOfWeek = dayOfWeek(year, month, day);
                    lDays = (int)(weekData.m_date / (3600 * 24));
                } else {
                    weekData.copy(dayDatas[0]);
                }
                for (int i = 0; i < dayDatasSize; i++) {
                    SecurityData2 dayData = new SecurityData2();
                    dayData.copy(dayDatas[i]);
                    int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0, days = (int)dayData.m_date / (3600 * 24);
                    FCTran.getDateByNum(dayData.m_date, ref year, ref month, ref day, ref hour, ref minute, ref second, ref ms);
                    int dow = dayOfWeek(year, month, day);
                    bool isNextWeek = true;
                    bool add = false;
                    if (days - lDays <= 5) {
                        if (days != lDays) {
                            isNextWeek = dow <= lDayOfWeek;
                        }
                    }
                    if (isNextWeek || i == dayDatasSize - 1) {
                        add = true;
                    }
                    if (!isNextWeek) {
                        weekData.m_close = dayData.m_close;
                        weekData.m_amount += dayData.m_amount;
                        weekData.m_volume += dayData.m_volume;
                        if (weekData.m_high < dayData.m_high) {
                            weekData.m_high = dayData.m_high;
                        }
                        if (weekData.m_low > dayData.m_low) {
                            weekData.m_low = dayData.m_low;
                        }

                    }
                    if (add) {
                        weekDatas.Add(weekData);
                        weekData = dayData;
                    }
                    if (isNextWeek && i == dayDatasSize - 1) {
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
        public static int getHistoryMonthDatas(List<SecurityData2> monthDatas, List<SecurityData2> dayDatas) {
            int monthDatasSize = dayDatas.Count;
            if (monthDatasSize > 0) {
                SecurityData2 monthData = new SecurityData2();
                int lYear = 0, lMonth = 0, lDay = 0;
                if (monthDatas.Count > 0) {
                    monthData = monthDatas[monthDatas.Count - 1];
                    DateTime lDt = FCTran.numToDate(monthData.m_date);
                    lYear = lDt.Year;
                    lMonth = lDt.Month;
                    lDay = lDt.Day;
                } else {
                    monthData.copy(dayDatas[0]);
                }
                for (int i = 0; i < monthDatasSize; i++) {
                    SecurityData2 dayData = new SecurityData2();
                    dayData.copy(dayDatas[i]);
                    int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0;
                    FCTran.getDateByNum(dayData.m_date, ref year, ref month, ref day, ref hour, ref minute, ref second, ref ms);
                    bool isNextMonth = year * 12 + month > lYear * 12 + lMonth;
                    bool add = false;
                    if (i == monthDatasSize - 1 || (i > 0 && isNextMonth)) {
                        add = true;
                    }
                    if (!isNextMonth) {
                        monthData.m_close = dayData.m_close;
                        monthData.m_amount += dayData.m_amount;
                        monthData.m_volume += dayData.m_volume;
                        if (monthData.m_high < dayData.m_high) {
                            monthData.m_high = dayData.m_high;
                        }
                        if (monthData.m_low > dayData.m_low) {
                            monthData.m_low = dayData.m_low;
                        }
                    }
                    if (add) {
                        monthDatas.Add(monthData);
                        monthData = dayData;
                    }
                    if (isNextMonth && i == monthDatasSize - 1) {
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
        public static void multiMinuteSecurityDatas(List<SecurityData2> newDatas, List<SecurityData2> minuteDatas, int cycle) {
            int lastMinutes = 0;
            for (int i = 0; i < minuteDatas.Count; i++) {
                SecurityData2 minuteData = minuteDatas[i];
                int minutes = (int)(minuteData.m_date / 60);
                if (lastMinutes == 0) {
                    lastMinutes = minutes;
                }
                //更新
                if (newDatas.Count > 0 && minutes - lastMinutes < cycle) {
                    SecurityData2 lastData = newDatas[newDatas.Count - 1];
                    lastData.m_close = minuteData.m_close;
                    if (minuteData.m_high > lastData.m_high) {
                        lastData.m_high = minuteData.m_high;
                    }
                    if (minuteData.m_low < lastData.m_low) {
                        lastData.m_low = minuteData.m_low;
                    }
                    lastData.m_amount += minuteData.m_amount;
                    lastData.m_volume += minuteData.m_volume;
                    //新增
                } else {
                    SecurityData2 newData = new SecurityData2();
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
        public static void mergeLatestData(List<SecurityData2> oldDatas, SecurityLatestData latestData, ClientTickDataCache tickDataCache, int cycle) {
            DateTime newDate = FCTran.numToDate(latestData.m_date);
            //A股
            if (newDate.Hour + newDate.Minute * 60 < 570) {
                newDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 9, 30, 0);
                latestData.m_date = FCTran.dateToNum(newDate);
            }
            bool isNextCycle = true;
            if (cycle < 1440) {
                if (oldDatas.Count > 0) {
                    int newMinutes = (int)(latestData.m_date / 60);
                    int lastMinutes = (int)(oldDatas[oldDatas.Count - 1].m_date / 60);
                    isNextCycle = newMinutes - lastMinutes > cycle;
                }
            } else {
                if (cycle == 1440) {
                    if (oldDatas.Count > 0)
                    {
                        DateTime lastDate = FCTran.numToDate(oldDatas[oldDatas.Count - 1].m_date);
                        isNextCycle = FCTran.dateToNum(new DateTime(newDate.Year, newDate.Month, newDate.Day))
                        != FCTran.dateToNum(new DateTime(lastDate.Year, lastDate.Month, lastDate.Day));
                    }
                } else if (cycle == 10080) {
                    if (oldDatas.Count > 0) {
                        DateTime lastDate = FCTran.numToDate(oldDatas[oldDatas.Count - 1].m_date);
                        int newDayOfWeek = dayOfWeek(newDate.Year, newDate.Month, newDate.Day);
                        int lDayOfWeek = dayOfWeek(lastDate.Year, lastDate.Month, lastDate.Day);
                        int newTotalDays = (int)(latestData.m_date / (3600 * 24));
                        int lastTotalDays = (int)(oldDatas[oldDatas.Count - 1].m_date / (3600 * 24));
                        if (newDayOfWeek - lDayOfWeek <= 5) {
                            if (newTotalDays !=
                               lastTotalDays) {
                                isNextCycle = newDayOfWeek <= lDayOfWeek;
                            } else {
                                isNextCycle = false;
                            }
                        }
                    }
                } else if (cycle == 43200) {
                    if (oldDatas.Count > 0) {
                        DateTime lastDate = FCTran.numToDate(oldDatas[oldDatas.Count - 1].m_date);
                        isNextCycle = (newDate.Year * 12 + newDate.Month != lastDate.Year * 12 + lastDate.Month);
                    }
                }
            }
            if (isNextCycle) {
                SecurityData2 newCycleData = new SecurityData2();
                newCycleData.m_date = latestData.m_date;
                newCycleData.m_close = latestData.m_close;
                newCycleData.m_high = latestData.m_close;
                newCycleData.m_low = latestData.m_close;
                newCycleData.m_open = latestData.m_close;
                newCycleData.m_volume = latestData.m_amount - tickDataCache.m_lastAmount;
                newCycleData.m_amount = latestData.m_amount - tickDataCache.m_lastAmount;
                oldDatas.Add(newCycleData);
            } else {
                SecurityData2 lastCycleData = oldDatas[oldDatas.Count - 1];
                lastCycleData.m_close = latestData.m_close;
                if (lastCycleData.m_high < latestData.m_close) {
                    lastCycleData.m_high = latestData.m_close;
                }
                if (lastCycleData.m_low > lastCycleData.m_close) {
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
        public static void caculateDivideKLineData(List<SecurityData2> data, List<List<OneDivideRightBase>> divideData, bool isForward)
        {
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
        public static float[] caculateDivideKLineData2(List<SecurityData2> data, List<List<OneDivideRightBase>> divideData, bool isForward)
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
        public static void getDataAfterDivide(IsDivideRightType divideType, SecurityData2 data, float factor)
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

        public static String get163LatestDatasByCodes(String strCodes)
        {
            String url = "http://api.money.126.net/data/feed/{0},money.api%5D";
            String[] codes = strCodes.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<String> requestCodes = new List<string>();
            for (int i = 0; i < codes.Length; i++)
            {
                String strCode = codes[i];
                if (strCode.IndexOf(".SH") != -1)
                {
                    strCode = "0" + strCode.Replace(".SH", "");
                }
                else if (strCode.IndexOf(".SZ") != -1)
                {
                    strCode = "1" + strCode.Replace(".SZ", "");
                }
                requestCodes.Add(strCode);
            }
            String codesStr = "";
            for (int i = 0; i < requestCodes.Count; i++)
            {
                codesStr += requestCodes[i];
                if (i != requestCodes.Count - 1)
                {
                    codesStr += ",";
                }
            }
            return FCHttpGetService.get(String.Format(url, codesStr));
        }

        /// <summary>
        /// 根据字符串获取新浪的最新数据
        /// </summary>
        /// <param name="str">数据字符串</param>
        /// <param name="formatType">格式</param>
        /// <param name="data">最新数据</param>
        /// <returns>状态</returns>
        public static int getLatestDataBy163Str(String str, int formatType, ref SecurityLatestData data)
        {
            try
            {
                str = str.Replace("\"", "");
                String[] strs = str.Split(',');
                for (int i = 0; i < strs.Length; i++)
                {
                    String key = strs[i].Substring(0, strs[i].IndexOf(": ")).Replace(" ", "");
                    String value = strs[i].Substring(strs[i].IndexOf(": ") + 2);
                    switch (key)
                    {
                        case "time":
                            data.m_date = FCTran.dateToNum(Convert.ToDateTime(value));
                            break;
                        case "code":
                            if (value.IndexOf("0") == 0)
                            {
                                data.m_code = value.Substring(1) + ".SH";
                            }
                            else
                            {
                                data.m_code = value.Substring(1) + ".SZ";
                            }
                            break;
                        case "high":
                            data.m_high = FCTran.strToDouble(value);
                            break;
                        case "low":
                            data.m_low = FCTran.strToDouble(value);
                            break;
                        case "open":
                            data.m_open = FCTran.strToDouble(value);
                            break;
                        case "yestclose":
                            data.m_lastClose = FCTran.strToDouble(value);
                            break;
                        case "turnover":
                            data.m_amount = FCTran.strToDouble(value);
                            data.m_turnoverRate = data.m_amount;
                            break;
                        case "name":
                            data.m_name = value;
                            break;
                        case "ask1":
                            data.m_sellPrice1 = FCTran.strToDouble(value);
                            break;
                        case "ask2":
                            data.m_sellPrice2 = FCTran.strToDouble(value);
                            break;
                        case "ask3":
                            data.m_sellPrice3 = FCTran.strToDouble(value);
                            break;
                        case "ask4":
                            data.m_sellPrice4 = FCTran.strToDouble(value);
                            break;
                        case "ask5":
                            data.m_sellPrice5 = FCTran.strToDouble(value);
                            break;
                        case "askvol1":
                            data.m_sellVolume1 = FCTran.strToInt(value);
                            break;
                        case "askvol2":
                            data.m_sellVolume2 = FCTran.strToInt(value);
                            break;
                        case "askvol3":
                            data.m_sellVolume3 = FCTran.strToInt(value);
                            break;
                        case "askvol4":
                            data.m_sellVolume4 = FCTran.strToInt(value);
                            break;
                        case "askvol5":
                            data.m_sellVolume5 = FCTran.strToInt(value);
                            break;
                        case "bid1":
                            data.m_buyPrice1 = FCTran.strToDouble(value);
                            break;
                        case "bid2":
                            data.m_buyPrice2 = FCTran.strToDouble(value);
                            break;
                        case "bid3":
                            data.m_buyPrice3 = FCTran.strToDouble(value);
                            break;
                        case "bid4":
                            data.m_buyPrice4 = FCTran.strToDouble(value);
                            break;
                        case "bid5":
                            data.m_buyPrice5 = FCTran.strToDouble(value);
                            break;
                        case "bidvol1":
                            data.m_buyVolume1 = FCTran.strToInt(value);
                            break;
                        case "bidvol2":
                            data.m_buyVolume2 = FCTran.strToInt(value);
                            break;
                        case "bidvol3":
                            data.m_buyVolume3 = FCTran.strToInt(value);
                            break;
                        case "bidvol4":
                            data.m_buyVolume4 = FCTran.strToInt(value);
                            break;
                        case "bidvol5":
                            data.m_buyVolume5 = FCTran.strToInt(value);
                            break;
                        case "price":
                            data.m_close = FCTran.strToDouble(value);
                            break;
                        case "volume":
                            data.m_volume = FCTran.strToDouble(value);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        /// <summary>
        /// 根据字符串获取新浪最新数据
        /// </summary>
        /// <param name="str">数据字符串</param>
        /// <param name="formatType">格式</param>
        /// <param name="datas">最新数据</param>
        /// <returns>状态</returns>
        public static int getLatestDatasBy163Str(String str, int formatType, List<SecurityLatestData> datas)
        {
            String[] strs = str.Split(new String[] { "}" }, StringSplitOptions.RemoveEmptyEntries);
            int strLen = strs.Length;
            for (int i = 0; i < strLen; i++)
            {
                SecurityLatestData latestData = new SecurityLatestData();
                String dataStr = strs[i];
                if (dataStr.Length > 50)
                {
                    if (dataStr.LastIndexOf("{") != -1)
                    {
                        dataStr = dataStr.Substring(dataStr.LastIndexOf("{") + 1);
                    }
                    getLatestDataBy163Str(dataStr, formatType, ref latestData);
                    if (latestData.m_date > 0)
                    {
                        datas.Add(latestData);
                    }
                }
            }
            return 1;
        }
    }
}
