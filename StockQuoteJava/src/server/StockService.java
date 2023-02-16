/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package server;
import service.*;
import java.util.*;
import facecat.topin.core.*;
import facecat.topin.service.*;
import base.*;
import java.text.SimpleDateFormat;

/**
 *
 * 股票服务
 */
public class StockService {
    /*
    * 代码表字典
    */
    public static HashMap<String, Security> m_codedMap = new HashMap<String, Security>();

    /*
    * 最新数据
    */
    public static HashMap<String, SecurityLatestData> m_latestDatas = new HashMap<String, SecurityLatestData>();

    /*
    * 上证交易时间
    */
    public static double m_shTradeTime;

    /*
    * 复权数据
    */
    public static HashMap<String, ArrayList<ArrayList<OneDivideRightBase>>> m_devideRightDatas = new HashMap<String, ArrayList<ArrayList<OneDivideRightBase>>>();

    /*
    * 下载复权因子
    */
    public static HashMap<String,ArrayList<ArrayList<OneDivideRightBase>>> getDevideRightDatas()
    {
        HashMap<String, ArrayList<ArrayList<OneDivideRightBase>>> datas = new HashMap<String, ArrayList<ArrayList<OneDivideRightBase>>>();
        String filePath = DataCenter.getAppPath() + DataCenter.m_seperator + "DR.txt";
        if (FCFile.isFileExist(filePath))
        {
            String content = "";
            RefObject<String> refContent = new RefObject<String>(content);
            FCFile.read(filePath, refContent);
            content = refContent.argvalue;
            if (content != null && content.length() > 0)
            {
                String[] strs = content.split("[\r\n]");
                String curCode = "";
                ArrayList<ArrayList<OneDivideRightBase>> oneDatas = new ArrayList<ArrayList<OneDivideRightBase>>();
                ArrayList<OneDivideRightBase> oneDatas2 = new ArrayList<OneDivideRightBase>();
                double date = -1;
                for (int i = 0; i < strs.length; i++)
                {
                    String str = strs[i];
                    if (str.indexOf("CODE=") != -1)
                    {
                        if (curCode.length() > 0)
                        {
                            datas.put(curCode, oneDatas);
                            oneDatas = new ArrayList<ArrayList<OneDivideRightBase>>();
                            oneDatas2 = new ArrayList<OneDivideRightBase>();
                            date = -1;
                        }
                        curCode = str.substring(str.indexOf("=") + 1);

                    }
                    else
                    {
                        String[] subStrs = str.split("[,]" );
                        OneDivideRightBase oneDivideRightBase = new OneDivideRightBase();
                        int intDate = FCTran.strToInt(subStrs[0]);
                        int year = intDate / 10000;
                        int month = (intDate / 100) % 100;
                        int day = intDate % 100;
                        oneDivideRightBase.Date = FCTran.getDateNum(year, month, day, 0, 0, 0, 0);
                        oneDivideRightBase.DataValue = FCTran.strToFloat(subStrs[1]);
                        //oneDivideRightBase.DivideType = (DivideRightType)Enum.Parse(typeof(DivideRightType), subStrs[2]); //modify
                        switch(subStrs[2]){
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
                            oneDatas2.add(oneDivideRightBase);
                            date = oneDivideRightBase.Date;
                            oneDatas.add(oneDatas2);
                        }
                        else
                        {
                            if (date == oneDivideRightBase.Date)
                            {
                                oneDatas2.add(oneDivideRightBase);
                            }
                            else
                            {
                                oneDatas2 = new ArrayList<OneDivideRightBase>();
                                oneDatas2.add(oneDivideRightBase);
                                oneDatas.add(oneDatas2);
                                date = oneDivideRightBase.Date;
                            }
                        }
                    }
                }
            }
        }
        return datas;
    }
    
    /*
    * 返回所有合约
    * @param codes 合约代码集合
    */
    public static void getCodes(ArrayList<String> codes) {
        for (String key : m_codedMap.keySet()) {
            codes.add(key);
        }
    }

    /*
    * 通过代码返回合约信息
    * @param code 合约代码
    * @param security 股票基本信息
    */
    public static int getSecurityByCode(String code, RefObject<Security> security) {
        int ret = 0;
        for (String key : m_codedMap.keySet()) {
            if (key.equals(code)) {
                security.argvalue = m_codedMap.get(key);
                ret = 1;
                break;
            }
        }
        return ret;
    }

    /*
    * 获取最新数据
    * @param code 代码
    * @param latestData 最新数据
    */
    public static int getLatestData(String code, SecurityLatestData latestData) {
        int state = 0;
        synchronized (m_latestDatas) {
            if (m_latestDatas.containsKey(code)) {
                latestData.copy(m_latestDatas.get(code));
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
        if (m_codedMap.size() == 0)
        {
            String content = "";
            RefObject<String> refContent = new RefObject<String>(content);
            FCFile.read(DataCenter.getAppPath() + DataCenter.m_seperator + "codes.txt", refContent);
            content = refContent.argvalue;
            String[] strs = content.split("[\r\n]");
            HashMap<String, String> maps = new HashMap<String, String>();
            for (int i = 0; i < strs.length; i++)
            {
                String[] subStrs = strs[i].split("[,]");
                if(subStrs.length >= 2){
                    Security security = new Security();
                    String code = subStrs[0];
                    if (!maps.containsKey(code))
                    {
                        security.m_code = code;
                        security.m_name = subStrs[1];
                        //security.m_pingyin = subStrs[2];
                        //security.m_status = subStrs[3];
                        //security.m_market = subStrs[4];
                        security.m_pingyin = "";
                        security.m_status = "";
                        security.m_market = "";
                        m_codedMap.put(code, security);
                        m_codes += code;
                        m_codes += ",";
                        maps.put(code, "");
                    }
                }
            }
        }
    }

    /*
    * 代码列表
    */
    private static String m_codes;
    
    /*
    * 上证的交易时间
    */
    public static ArrayList<Integer> m_shTradeTimes = new ArrayList<Integer>();
    
    /*
    * 开始策略
    */
    public static void start() {
        new Thread(runnable).start();
    }
    
    /*
    * 拉取最新数据的线程
    */
    public static Runnable runnable = new Runnable() {
        @Override
        public void run() {
            while (true) {
                if (m_codes != null && m_codes.length() > 0) {
                    if (m_codes.endsWith(",")) {
                        m_codes = m_codes.substring(0, m_codes.length() - 2);
                    }
                    String[] strCodes = m_codes.split("[,]");
                    int codesSize = strCodes.length;
                    String latestCodes = "";
                    HashMap<String, SecurityLatestData> priceDatas = new HashMap<String, SecurityLatestData>();
                    for (int i = 0; i < codesSize; i++) {
                        latestCodes += strCodes[i];
                        if (i == codesSize - 1 || (i > 0 && i % 500 == 0)) {
                            String latestDatasResult = get163LatestDatasByCodes(latestCodes);
                            if (latestDatasResult != null && latestDatasResult.length() > 0) {
                                ArrayList<SecurityLatestData> latestDatas = new ArrayList<SecurityLatestData>();
                                getLatestDatasBy163Str(latestDatasResult, 0, latestDatas);
                                String[] subStrs = latestDatasResult.split("[;\n]");
                                int latestDatasSize = latestDatas.size();
                                for (int j = 0; j < latestDatasSize; j++) {
                                    SecurityLatestData latestData = latestDatas.get(j);
                                    if (latestData.m_close == 0) {
                                        latestData.m_close = latestData.m_buyPrice1;
                                    }
                                    if (latestData.m_close == 0) {
                                        latestData.m_close = latestData.m_sellPrice1;
                                    }
                                    synchronized (m_latestDatas) {
                                        boolean newData = false;
                                        if (!m_latestDatas.containsKey(latestData.m_code)) {
                                            m_latestDatas.put(latestData.m_code, latestData);
                                            newData = true;
                                        } else {
                                            if (!m_latestDatas.get(latestData.m_code).equal(latestData)) {
                                                m_latestDatas.get(latestData.m_code).copy(latestData);
                                                newData = true;
                                            }
                                        }
                                        if (newData) {
                                            DataCenter.m_latestService.sendData(latestData, -1);
                                            DataCenter.m_historyService.sendData(latestData);
                                            priceDatas.put(latestData.m_code, latestData);
                                        }
                                        if (latestData.m_code.equals("600000.SH")) {
                                            m_shTradeTime = latestData.m_date;
                                        }
                                    }
                                }
                                latestDatas.clear();
                            }
                            latestCodes = "";
                        } else {
                            latestCodes += ",";
                        }
                    }
                    if (priceDatas.size() > 0) {
                        DataCenter.m_priceDataService.sendDatas(priceDatas, -1);
                    }
                }
                try{
                    Thread.sleep(10000);
                }catch(Exception ex){
                    
                }
            }
        }
    };
    
    /*
    * 获取日线数据
    * @param code 代码
    * @param name 名称
    */
    public static ArrayList<SecurityData2> getSecurityDatas(String code, RefObject<String> name) {
        ArrayList<SecurityData2> datas = new ArrayList<SecurityData2>();
        String appPath = DataCenter.getAppPath();
        String filePath = appPath + DataCenter.m_seperator +"day" + DataCenter.m_seperator + FCStrExC.convertDBCodeToFileName(code);
        if (FCFile.isFileExist(filePath))
        {
            String content = "";
            RefObject<String> refContent = new RefObject<String>(content);
            FCFile.read(filePath, refContent);
            content = refContent.argvalue;
            String[] strs = content.split("[\r\n]");
            int strsSize = strs.length;
            String[] headStrs = strs[0].split("[ ]");
            name.argvalue = headStrs[1];
            for (int i = 2; i < strs.length - 1; i++)
            {
                String str = strs[i];
                String[] subStrs = str.split("[,]");
                if(subStrs.length >= 7){
                    SecurityData2 securityData = new SecurityData2();
                    //securityData.m_date = FCTran.dateToNum(Convert.ToDateTime(subStrs[0])); //modify
                    int year = 0, month = 0, day = 0;
                    String[] dateStrs = subStrs[0].split("-");
                    year = FCTran.strToInt(dateStrs[0]);
                    if(dateStrs[1].indexOf("0") == 0){
                        month = FCTran.strToInt(dateStrs[1].substring(1));
                    }else{
                        month = FCTran.strToInt(dateStrs[1]);
                    }
                    if(dateStrs[2].indexOf("0") == 0){
                        day = FCTran.strToInt(dateStrs[2].substring(1));
                    }else{
                        day = FCTran.strToInt(dateStrs[2]);
                    }
                    securityData.m_date = FCTran.getDateNum(year, month, day, 0, 0, 0, 0);
                    securityData.m_open = FCTran.strToDouble(subStrs[1]);
                    securityData.m_high = FCTran.strToDouble(subStrs[2]);
                    securityData.m_low = FCTran.strToDouble(subStrs[3]);
                    securityData.m_close = FCTran.strToDouble(subStrs[4]);
                    securityData.m_volume = FCTran.strToDouble(subStrs[5]);
                    securityData.m_amount = FCTran.strToDouble(subStrs[6]);
                    datas.add(securityData);
                }
            }
        }
        return datas;
    }
    
    /*
    * 获取分时线数据
    * @param code 代码
    * @param name 名称
    */
    public static ArrayList<SecurityData2> getSecurityMinuteDatas(String code, RefObject<String> name) {
        ArrayList<SecurityData2> datas = new ArrayList<SecurityData2>();
        String appPath = DataCenter.getAppPath();
        String filePath = appPath + DataCenter.m_seperator + "minute" + DataCenter.m_seperator + FCStrExC.convertDBCodeToFileName(code);
        if (FCFile.isFileExist(filePath))
        {
            String content = "";
            RefObject<String> refContent = new RefObject<String>(content);
            FCFile.read(filePath, refContent);
            content = refContent.argvalue;
            String[] strs = content.split("[\r\n]");
            int strsSize = strs.length;
            String[] headStrs = strs[0].split("[ ]");
            name.argvalue = headStrs[1];
            for (int i = 2; i < strs.length - 1; i++)
            {
                String str = strs[i];
                String[] subStrs = str.split("[,]");
                if(subStrs.length >= 7){
                    SecurityData2 securityData = new SecurityData2();
                    int hour = FCTran.strToInt(subStrs[1].substring(0, 2));
                    int minute = FCTran.strToInt(subStrs[1].substring(2, 4));
                    int year = 0, month = 0, day = 0;
                    String[] dateStrs = subStrs[0].split("-");
                    year = FCTran.strToInt(dateStrs[0]);
                    if(dateStrs[1].indexOf("0") == 0){
                        month = FCTran.strToInt(dateStrs[1].substring(1));
                    }else{
                        month = FCTran.strToInt(dateStrs[1]);
                    }
                    if(dateStrs[2].indexOf("0") == 0){
                        day = FCTran.strToInt(dateStrs[2].substring(1));
                    }else{
                        day = FCTran.strToInt(dateStrs[2]);
                    }
                    securityData.m_date = FCTran.getDateNum(year, month, day, hour, minute, 0, 0);
                    securityData.m_open = FCTran.strToDouble(subStrs[2]);
                    securityData.m_high = FCTran.strToDouble(subStrs[3]);
                    securityData.m_low = FCTran.strToDouble(subStrs[4]);
                    securityData.m_close = FCTran.strToDouble(subStrs[5]);
                    securityData.m_volume = FCTran.strToDouble(subStrs[6]);
                    securityData.m_amount = FCTran.strToDouble(subStrs[7]);
                    datas.add(securityData);
                }
            }
        }
        return datas;
    }
    
    /*
    * 写日线数据
    * @param code 代码
    * @param name 名称
    * @param datas 数据
    */
    public static void writeSecurityDatas(String code, String name, ArrayList<SecurityData2> datas) {
        int datasSize = datas.size();
        StringBuilder sb = new StringBuilder();
        sb.append(code + " " + name + " 日线\r\n");
        sb.append("日期\t开盘\t最高\t最低\t收盘\t成交量\t成交额\r\n");
        for (int i = 0; i < datasSize; i++) {
            SecurityData2 data = datas.get(i);
            Calendar dateTime = FCTran.numToDate(data.m_date);
            SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");
            String dateStr = format.format(dateTime.getTime());
            sb.append(String.format("%1$s,%2$s,%3$s,%4$s,%5$s,%6$s,%7$s\r\n", dateStr, data.m_open, data.m_high,
                data.m_low, data.m_close, data.m_volume, data.m_amount)); 
        }
        sb.append("数据来源:花卷猫");
        String appPath = DataCenter.getAppPath();
        String filePath = appPath + DataCenter.m_seperator + "day" + DataCenter.m_seperator + FCStrExC.convertDBCodeToFileName(code);
        FCFile.write(filePath, sb.toString());
    }

    /*
    * 写分钟线数据
    * @param code 代码
    * @param name 名称
    * @param datas 数据
    */
    public static void writeMinuteSecurityDatas(String code, String name, ArrayList<SecurityData2> datas) {
        int datasSize = datas.size();
        StringBuilder sb = new StringBuilder();
        sb.append(code + " " + name + " 1分钟线\r\n");
        sb.append("日期\t时间\t开盘\t最高\t最低\t收盘\t成交量\t成交额\r\n");
        for (int i = 0; i < datasSize; i++) {
            SecurityData2 data = datas.get(i);
            Calendar dateTime = FCTran.numToDate(data.m_date);
            SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd,HH:mm");
            String dateStr = format.format(dateTime.getTime());
            sb.append(String.format("%1$s,%2$s,%3$s,%4$s,%5$s,%6$s,%7$s\r\n", dateStr, data.m_open, data.m_high,
                data.m_low, data.m_close, data.m_volume, data.m_amount)); 
        }
        sb.append("数据来源:花卷猫");
        String appPath = DataCenter.getAppPath();
        String filePath = appPath + DataCenter.m_seperator + "minute" + DataCenter.m_seperator + FCStrExC.convertDBCodeToFileName(code);
        FCFile.write(filePath, sb.toString());
    }
    
    /*
    * 合并日线数据
    * @param latestData 最新数据
    * @param oldDatas 老数据
    */
    public static void mergeSecurityDatas(SecurityLatestData latestData, ArrayList<SecurityData2> oldDatas) {
        SecurityData2 newDayData = new SecurityData2();
        int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0;
        RefObject<Integer> refYear = new RefObject<Integer>(year);
        RefObject<Integer> refMonth = new RefObject<Integer>(month);
        RefObject<Integer> refDay = new RefObject<Integer>(day);
        RefObject<Integer> refHour = new RefObject<Integer>(hour);
        RefObject<Integer> refMinute = new RefObject<Integer>(minute);
        RefObject<Integer> refSecond = new RefObject<Integer>(second);
        RefObject<Integer> refMs = new RefObject<Integer>(ms);
        FCTran.getDateByNum(latestData.m_date, refYear, refMonth, refDay, refHour, refMinute, refSecond, refMs);
        year = refYear.argvalue;
        month = refMonth.argvalue;
        day = refDay.argvalue;
        hour = refHour.argvalue;
        minute = refMinute.argvalue;
        second = refSecond.argvalue;
        ms = refMs.argvalue;
        newDayData.m_date = FCTran.getDateNum(year, month, day, 0, 0, 0, 0);
        newDayData.m_close = latestData.m_close;
        newDayData.m_high = latestData.m_high;
        newDayData.m_low = latestData.m_low;
        newDayData.m_open = latestData.m_open;
        newDayData.m_amount = latestData.m_amount;
        newDayData.m_volume = latestData.m_volume;
        if (oldDatas.size() > 0) {
            if (oldDatas.get(oldDatas.size() - 1).m_date == newDayData.m_date) {
                oldDatas.set(oldDatas.size() - 1, newDayData);
            } else {
                oldDatas.add(newDayData);
            }
        } else {
            oldDatas.add(newDayData);
        }
    }
    
    /*
    * 合并分时线数据
    * @param datas 数据
    * @param latestDatas 最新数据
    */
    public static void mergeMinuteSecurityDatas(ArrayList<SecurityData2> datas, ArrayList<SecurityData2> latestDatas) {
        for (int i = 0; i < latestDatas.size(); i++) {
            SecurityData2 newMinuteData = latestDatas.get(i);
            if (datas.size() > 0) {
                if (datas.get(datas.size() - 1).m_date == newMinuteData.m_date) {
                    datas.set(datas.size() - 1, newMinuteData);
                } else {
                    datas.add(newMinuteData);
                }
            } else {
                datas.add(newMinuteData);
            }
        }
    }
    
    /*
    * 合并实时数据
    * @param latestData 最新数据
    * @param oldCache 旧数据缓存
    */
    public static void mergeRunTimeDatas(SecurityLatestData latestData, MinuteDatasCache oldCache) {
        double subVolume = latestData.m_volume - oldCache.m_lastVolume;
        double subAmount = latestData.m_amount - oldCache.m_lastAmount;
        if (oldCache.m_lastDate == 0)
        {
            subVolume = 0;
            subAmount = 0;
        }
        int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0;
        RefObject<Integer> refYear = new RefObject<Integer>(year);
        RefObject<Integer> refMonth = new RefObject<Integer>(month);
        RefObject<Integer> refDay = new RefObject<Integer>(day);
        RefObject<Integer> refHour = new RefObject<Integer>(hour);
        RefObject<Integer> refMinute = new RefObject<Integer>(minute);
        RefObject<Integer> refSecond = new RefObject<Integer>(second);
        RefObject<Integer> refMs = new RefObject<Integer>(ms);
        FCTran.getDateByNum(latestData.m_date, refYear, refMonth, refDay, refHour, refMinute, refSecond, refMs);
        year = refYear.argvalue;
        month = refMonth.argvalue;
        day = refDay.argvalue;
        hour = refHour.argvalue;
        minute = refMinute.argvalue;
        second = refSecond.argvalue;
        ms = refMs.argvalue;
        double date = FCTran.getDateNum(year, month, day, hour, minute, 0, 0);
        //A股
        if (hour + minute * 60 < 570) {
            date = FCTran.getDateNum(year, month, day, 9, 30, 0, 0);
            latestData.m_date = date;
        }
        //更新
        if (oldCache.m_lastDate == date)
        {
            SecurityData2 newDayData = oldCache.m_datas.get(oldCache.m_datas.size() - 1);
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
            if (oldCache.m_lastDate != 0 && oldCache.m_datas.size() > 0)
            {
                int oY = 0, oM = 0, oD = 0, oldH = 0, oldM = 0, oldS = 0, olsMs = 0;
                RefObject<Integer> refOldYear = new RefObject<Integer>(oY);
                RefObject<Integer> refOldMonth = new RefObject<Integer>(oM);
                RefObject<Integer> refOldDay = new RefObject<Integer>(oD);
                RefObject<Integer> refOldHour = new RefObject<Integer>(oldH);
                RefObject<Integer> refOldMinute = new RefObject<Integer>(oldM);
                RefObject<Integer> refOldSecond = new RefObject<Integer>(oldS);
                RefObject<Integer> refOldMs = new RefObject<Integer>(olsMs);
                FCTran.getDateByNum(oldCache.m_lastDate, refOldYear, refOldMonth, refOldDay, refOldHour, refOldMinute, refOldSecond, refOldMs);
                year = refYear.argvalue;
                month = refMonth.argvalue;
                day = refDay.argvalue;
                hour = refHour.argvalue;
                minute = refMinute.argvalue;
                second = refSecond.argvalue;
                ms = refMs.argvalue;
                //DateTime oldDate = FCTran.numToDate(oldCache.m_lastDate);
                int oldMinute = oldH * 60 + oldM;
                int thisMinute = hour * 60 + minute;
                //填充K线
                ArrayList<Integer> emptyDates = new ArrayList<Integer>();
                boolean isEmpty = false;
                for (int i = 0; i < m_shTradeTimes.size(); i++)
                {
                    if (!isEmpty)
                    {
                        if (m_shTradeTimes.get(i) == oldMinute)
                        {
                            isEmpty = true;
                        }
                    }
                    else
                    {
                        if (m_shTradeTimes.get(i) == oldMinute)
                        {
                            break;
                        }
                        else
                        {
                            emptyDates.add(m_shTradeTimes.get(i));
                        }
                    }
                }
                int emptyDatesSize = emptyDates.size();
                if (emptyDatesSize > 0)
                {
                    double lastClose = oldCache.m_datas.get(oldCache.m_datas.size() - 1).m_close;
                    for (int i = 0; i < emptyDatesSize; i++)
                    {
                        SecurityData2 emptyData = new SecurityData2();
                        emptyData.m_date = FCTran.getDateNum(year, month, day, emptyDates.get(i) / 60, emptyDates.get(i) % 60, 0, 0);
                        emptyData.m_close = lastClose;
                        emptyData.m_high = lastClose;
                        emptyData.m_low = lastClose;
                        emptyData.m_open = lastClose;
                        emptyData.m_amount = 0;
                        emptyData.m_volume = 0;
                        oldCache.m_datas.add(emptyData);
                    }
                    emptyDates.clear();
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
            oldCache.m_datas.add(newDayData);
        }
    }
    
    /*
    * 获取指定年月日的星期
    * @param year 年
    * @param month 月
    * @param day 日
    * @return 星期
    */
    public static int dayOfWeek(int y, int m, int d) {
        if (m == 1 || m == 2) {
            m += 12;
            y--;
        }
        return (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7;
    }
    
    /*
    * 获取周线的历史数据
    * @param weekDatas 周线历史数据
    * @param dayDatas 日线历史数据
    */
    public static int getHistoryWeekDatas(ArrayList<SecurityData2> weekDatas, ArrayList<SecurityData2> dayDatas) {
        int dayDatasSize = dayDatas.size();
        if (dayDatasSize > 0) {
            SecurityData2 weekData = new SecurityData2();
            int lDayOfWeek = 0, lDays = 0;
            if (weekDatas.size() > 0) {
                weekData = weekDatas.get(weekDatas.size() - 1);
                int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0;
                RefObject<Integer> refYear = new RefObject<Integer>(year);
                RefObject<Integer> refMonth = new RefObject<Integer>(month);
                RefObject<Integer> refDay = new RefObject<Integer>(day);
                RefObject<Integer> refHour = new RefObject<Integer>(hour);
                RefObject<Integer> refMinute = new RefObject<Integer>(minute);
                RefObject<Integer> refSecond = new RefObject<Integer>(second);
                RefObject<Integer> refMs = new RefObject<Integer>(ms);
                FCTran.getDateByNum(weekData.m_date, refYear, refMonth, refDay, refHour, refMinute, refSecond, refMs);
                year = refYear.argvalue;
                month = refMonth.argvalue;
                day = refDay.argvalue;
                hour = refHour.argvalue;
                minute = refMinute.argvalue;
                second = refSecond.argvalue;
                ms = refMs.argvalue;
                lDayOfWeek = dayOfWeek(year, month, day);
                lDays = (int)(weekData.m_date / (3600 * 24));
            } else {
                weekData.copy(dayDatas.get(0));
            }
            for (int i = 0; i < dayDatasSize; i++) {
                SecurityData2 dayData = new SecurityData2();
                dayData.copy(dayDatas.get(i));
                int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0, days = (int)dayData.m_date / (3600 * 24);
                //FCTran.getDateByNum(dayData.m_date, ref year, ref month, ref day, ref hour, ref minute, ref second, ref ms);int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0;
                RefObject<Integer> refYear = new RefObject<Integer>(year);
                RefObject<Integer> refMonth = new RefObject<Integer>(month);
                RefObject<Integer> refDay = new RefObject<Integer>(day);
                RefObject<Integer> refHour = new RefObject<Integer>(hour);
                RefObject<Integer> refMinute = new RefObject<Integer>(minute);
                RefObject<Integer> refSecond = new RefObject<Integer>(second);
                RefObject<Integer> refMs = new RefObject<Integer>(ms);
                FCTran.getDateByNum(dayData.m_date, refYear, refMonth, refDay, refHour, refMinute, refSecond, refMs);
                year = refYear.argvalue;
                month = refMonth.argvalue;
                day = refDay.argvalue;
                hour = refHour.argvalue;
                minute = refMinute.argvalue;
                second = refSecond.argvalue;
                ms = refMs.argvalue;
                int dow = dayOfWeek(year, month, day);
                boolean isNextWeek = true;
                boolean add = false;
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
                    weekDatas.add(weekData);
                    weekData = dayData;
                }
                if (isNextWeek && i == dayDatasSize - 1) {
                    weekData = dayData;
                    weekDatas.add(weekData);
                }
                lDayOfWeek = dow;
                lDays = days;
            }
        }
        return 1;
    }
    
    /*
    * 获取月线的历史数据
    * @param weekDatas 月线历史数据
    * @param dayDatas 日线历史数据
    */
    public static int getHistoryMonthDatas(ArrayList<SecurityData2> monthDatas, ArrayList<SecurityData2> dayDatas) {
        int monthDatasSize = dayDatas.size();
        if (monthDatasSize > 0) {
            SecurityData2 monthData = new SecurityData2();
            int lYear = 0, lMonth = 0, lDay = 0;
            if (monthDatas.size() > 0) {
                monthData = monthDatas.get(monthDatas.size() - 1);
                int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0;
                RefObject<Integer> refYear = new RefObject<Integer>(year);
                RefObject<Integer> refMonth = new RefObject<Integer>(month);
                RefObject<Integer> refDay = new RefObject<Integer>(day);
                RefObject<Integer> refHour = new RefObject<Integer>(hour);
                RefObject<Integer> refMinute = new RefObject<Integer>(minute);
                RefObject<Integer> refSecond = new RefObject<Integer>(second);
                RefObject<Integer> refMs = new RefObject<Integer>(ms);
                FCTran.getDateByNum(monthData.m_date, refYear, refMonth, refDay, refHour, refMinute, refSecond, refMs);
                year = refYear.argvalue;
                month = refMonth.argvalue;
                day = refDay.argvalue;
                hour = refHour.argvalue;
                minute = refMinute.argvalue;
                second = refSecond.argvalue;
                ms = refMs.argvalue;
                lYear = year;
                lMonth =  month;
                lDay = day;
            } else {
                monthData.copy(dayDatas.get(0));
            }
            for (int i = 0; i < monthDatasSize; i++) {
                SecurityData2 dayData = new SecurityData2();
                dayData.copy(dayDatas.get(i));
                int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0, ms = 0;
                RefObject<Integer> refYear = new RefObject<Integer>(year);
                RefObject<Integer> refMonth = new RefObject<Integer>(month);
                RefObject<Integer> refDay = new RefObject<Integer>(day);
                RefObject<Integer> refHour = new RefObject<Integer>(hour);
                RefObject<Integer> refMinute = new RefObject<Integer>(minute);
                RefObject<Integer> refSecond = new RefObject<Integer>(second);
                RefObject<Integer> refMs = new RefObject<Integer>(ms);
                FCTran.getDateByNum(dayData.m_date, refYear, refMonth, refDay, refHour, refMinute, refSecond, refMs);
                year = refYear.argvalue;
                month = refMonth.argvalue;
                day = refDay.argvalue;
                hour = refHour.argvalue;
                minute = refMinute.argvalue;
                second = refSecond.argvalue;
                ms = refMs.argvalue;
                boolean isNextMonth = year * 12 + month > lYear * 12 + lMonth;
                boolean add = false;
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
                    monthDatas.add(monthData);
                    monthData = dayData;
                }
                if (isNextMonth && i == monthDatasSize - 1) {
                    monthData = dayData;
                    monthDatas.add(monthData);
                }
                lYear = year;
                lMonth = month;
                lDay = day;
            }
        }
        return 1;
    }
    
    /*
    * 计算多分钟线
    * @param newDatas 最新数据
    * @param minuteDatas 分钟线数据
    * @param cycle 周期
    */
    public static void multiMinuteSecurityDatas(List<SecurityData2> newDatas, List<SecurityData2> minuteDatas, int cycle) {
        int lastMinutes = 0;
        for (int i = 0; i < minuteDatas.size(); i++) {
            SecurityData2 minuteData = minuteDatas.get(i);
            int minutes = (int)(minuteData.m_date / 60);
            if (lastMinutes == 0) {
                lastMinutes = minutes;
            }
            //更新
            if (newDatas.size() > 0 && minutes - lastMinutes < cycle) {
                SecurityData2 lastData = newDatas.get(newDatas.size() - 1);
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
                newDatas.add(newData);
                lastMinutes = minutes;
            }
        }
    }
    
    /*
    * 合并最新数据
    * @param oldDatas 老数据
    * @param latestData 最新数据
    * @param tickDataCache tick数据
    * @param cycle 周期
    */
    public static void mergeLatestData(ArrayList<SecurityData2> oldDatas, SecurityLatestData latestData, ClientTickDataCache tickDataCache, int cycle) {
        //A股
        int newDateYear = 0, newDateMonth = 0, newDateDay = 0, newDateHour = 0, newDateMiute = 0, newDateSecond = 0, newDateMs = 0;
        RefObject<Integer> refYear = new RefObject<Integer>(newDateYear);
        RefObject<Integer> refMonth = new RefObject<Integer>(newDateMonth);
        RefObject<Integer> refDay = new RefObject<Integer>(newDateDay);
        RefObject<Integer> refHour = new RefObject<Integer>(newDateHour);
        RefObject<Integer> refMinute = new RefObject<Integer>(newDateMiute);
        RefObject<Integer> refSecond = new RefObject<Integer>(newDateSecond);
        RefObject<Integer> refMs = new RefObject<Integer>(newDateMs);
        FCTran.getDateByNum(latestData.m_date, refYear, refMonth, refDay, refHour, refMinute, refSecond, refMs);
        newDateYear = refYear.argvalue;
        newDateMonth = refMonth.argvalue;
        newDateDay = refDay.argvalue;
        newDateHour = refHour.argvalue;
        newDateMiute = refMinute.argvalue;
        newDateSecond = refSecond.argvalue;
        newDateMs = refMs.argvalue;
        if (newDateHour + newDateMiute * 60 < 570) {
            latestData.m_date = FCTran.getDateNum(newDateYear, newDateMonth, newDateDay, 9, 30, 0, 0);
        }
        boolean isNextCycle = true;
        if (cycle < 1440) {
            if (oldDatas.size() > 0) {
                int newMinutes = (int)(latestData.m_date / 60);
                int lastMinutes = (int)(oldDatas.get(oldDatas.size() - 1).m_date / 60);
                isNextCycle = newMinutes - lastMinutes > cycle;
            }
        } else {
            if (cycle == 1440) {
                if (oldDatas.size() > 0)
                {
                    int oldDateYear = 0, oldDateMonth = 0, oldDateDay = 0, oldDateHour = 0, oldDateMiute = 0, oldDateSecond = 0, oldDateMs = 0;
                    RefObject<Integer> refOldYear = new RefObject<Integer>(oldDateYear);
                    RefObject<Integer> refOldMonth = new RefObject<Integer>(oldDateMonth);
                    RefObject<Integer> refOldDay = new RefObject<Integer>(oldDateDay);
                    RefObject<Integer> refOldHour = new RefObject<Integer>(oldDateHour);
                    RefObject<Integer> refOldMinute = new RefObject<Integer>(oldDateMiute);
                    RefObject<Integer> refOldSecond = new RefObject<Integer>(oldDateSecond);
                    RefObject<Integer> refOldMs = new RefObject<Integer>(oldDateMs);
                    FCTran.getDateByNum(oldDatas.get(oldDatas.size() - 1).m_date, refOldYear, refOldMonth, refOldDay, refOldHour, refOldMinute, refOldSecond, refOldMs);
                    oldDateYear = refYear.argvalue;
                    oldDateMonth = refMonth.argvalue;
                    oldDateDay = refDay.argvalue;
                    oldDateHour = refHour.argvalue;
                    oldDateMiute = refMinute.argvalue;
                    oldDateSecond = refSecond.argvalue;
                    oldDateMs = refMs.argvalue;
                    isNextCycle = FCTran.getDateNum(newDateYear, newDateMonth, newDateDay, 0, 0, 0, 0)
                    != FCTran.getDateNum(oldDateYear, oldDateMonth, oldDateDay, 0, 0, 0, 0);
                }
            } else if (cycle == 10080) {
                if (oldDatas.size() > 0) {
                    int oldDateYear = 0, oldDateMonth = 0, oldDateDay = 0, oldDateHour = 0, oldDateMiute = 0, oldDateSecond = 0, oldDateMs = 0;
                    RefObject<Integer> refOldYear = new RefObject<Integer>(oldDateYear);
                    RefObject<Integer> refOldMonth = new RefObject<Integer>(oldDateMonth);
                    RefObject<Integer> refOldDay = new RefObject<Integer>(oldDateDay);
                    RefObject<Integer> refOldHour = new RefObject<Integer>(oldDateHour);
                    RefObject<Integer> refOldMinute = new RefObject<Integer>(oldDateMiute);
                    RefObject<Integer> refOldSecond = new RefObject<Integer>(oldDateSecond);
                    RefObject<Integer> refOldMs = new RefObject<Integer>(oldDateMs);
                    FCTran.getDateByNum(oldDatas.get(oldDatas.size() - 1).m_date, refOldYear, refOldMonth, refOldDay, refOldHour, refOldMinute, refOldSecond, refOldMs);
                    oldDateYear = refYear.argvalue;
                    oldDateMonth = refMonth.argvalue;
                    oldDateDay = refDay.argvalue;
                    oldDateHour = refHour.argvalue;
                    oldDateMiute = refMinute.argvalue;
                    oldDateSecond = refSecond.argvalue;
                    oldDateMs = refMs.argvalue;
                    int newDayOfWeek = dayOfWeek(newDateYear, newDateMonth, newDateDay);
                    int lDayOfWeek = dayOfWeek(oldDateYear, oldDateMonth, oldDateDay);
                    int newTotalDays = (int)(latestData.m_date / (3600 * 24));
                    int lastTotalDays = (int)(oldDatas.get(oldDatas.size() - 1).m_date / (3600 * 24));
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
                if (oldDatas.size() > 0) {
                    int oldDateYear = 0, oldDateMonth = 0, oldDateDay = 0, oldDateHour = 0, oldDateMiute = 0, oldDateSecond = 0, oldDateMs = 0;
                    RefObject<Integer> refOldYear = new RefObject<Integer>(oldDateYear);
                    RefObject<Integer> refOldMonth = new RefObject<Integer>(oldDateMonth);
                    RefObject<Integer> refOldDay = new RefObject<Integer>(oldDateDay);
                    RefObject<Integer> refOldHour = new RefObject<Integer>(oldDateHour);
                    RefObject<Integer> refOldMinute = new RefObject<Integer>(oldDateMiute);
                    RefObject<Integer> refOldSecond = new RefObject<Integer>(oldDateSecond);
                    RefObject<Integer> refOldMs = new RefObject<Integer>(oldDateMs);
                    FCTran.getDateByNum(oldDatas.get(oldDatas.size() - 1).m_date, refOldYear, refOldMonth, refOldDay, refOldHour, refOldMinute, refOldSecond, refOldMs);
                    oldDateYear = refYear.argvalue;
                    oldDateMonth = refMonth.argvalue;
                    oldDateDay = refDay.argvalue;
                    oldDateHour = refHour.argvalue;
                    oldDateMiute = refMinute.argvalue;
                    oldDateSecond = refSecond.argvalue;
                    oldDateMs = refMs.argvalue;
                    isNextCycle = (newDateYear * 12 + newDateMonth != oldDateYear * 12 + oldDateMonth);
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
            oldDatas.add(newCycleData);
        } else {
            SecurityData2 lastCycleData = oldDatas.get(oldDatas.size() - 1);
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

    /*
    * 计算复权，只改变复权因子
    * @param data 数据
    * @param divideData 复权因子
    * @param isForward 是否前复权
    */
    public static void caculateDivideKLineData(ArrayList<SecurityData2> data, ArrayList<ArrayList<OneDivideRightBase>> divideData, boolean isForward)
    {
        float factor = 1;
        for (int i = 0; i < divideData.size(); i++)
            factor *= divideData.get(i).get(0).Factor;

        int indexLast = 0;
        if (isForward)
        {
            indexLast = -1;
            for (int i = 0; i < divideData.size(); i++)
            {
                if (factor == 0 || factor == 1)
                    continue;
                for (int j = indexLast + 1; j < data.size(); j++)
                {
                    if (data.get(j).m_date < divideData.get(i).get(0).Date)
                    {
                        data.get(j).m_forwardFactor = factor;
                        indexLast = j;
                    }
                    else
                    {
                        if (i == divideData.size() - 1)
                            data.get(j).m_forwardFactor = 1.0f;
                        else
                            break;
                    }

                }
                factor /= divideData.get(i).get(0).Factor;
            }
        }
        else
        {
            indexLast = data.size();
            for (int i = divideData.size() - 1; i >= 0; i--)
            {
                if (divideData.get(i).get(0).Factor == 0 || divideData.get(i).get(0).Factor == 1)
                    continue;

                for (int j = indexLast - 1; j >= 0; j--)
                {
                    if (data.get(j).m_date >= divideData.get(i).get(0).Date)
                    {
                        data.get(j).m_backFactor = factor;
                        indexLast = j;
                    }
                    else
                    {
                        if (i == 0)
                            data.get(j).m_backFactor = 1.0f;
                        else
                            break;
                    }
                }
                factor /= divideData.get(i).get(0).Factor;
            }
        }
    }
    
    /*
    * 计算复权，只改变复权因子
    * @param data 数据
    * @param divideData 复权因子
    * @param isForward 是否前复权
    */
    public static float[] caculateDivideKLineData2(List<SecurityData2> data, ArrayList<ArrayList<OneDivideRightBase>> divideData, boolean isForward)
    {
        if (divideData == null || data == null)
            return null;
        float[] ArrResult = new float[data.size()];
        float factor = 1;
        for (int index = 0; index < data.size(); index++)
        {
            if (index < ArrResult.length)
                ArrResult[index] = factor;
        }
        if (divideData.size() == 0)
            return ArrResult;
        for (int i = 0; i < divideData.size(); i++)
            factor *= divideData.get(i).get(0).Factor;

        int indexLast = 0;
        if (isForward)
        {
            indexLast = -1;
            for (int i = 0; i < divideData.size(); i++)
            {
                if (factor == 0 || factor == 1)
                    continue;
                for (int j = indexLast + 1; j < data.size(); j++)
                {
                    if (j >= ArrResult.length)
                        continue;
                    if (data == null || divideData == null || data.get(j) == null
                        || divideData.get(i) == null || divideData.get(i).get(0) == null)
                    {
                        int s = 0;
                    }
                    if (data.get(j).m_date < divideData.get(i).get(0).Date)
                    {
                        ArrResult[j] = factor;
                        indexLast = j;
                    }
                    else
                    {
                        if (i == divideData.size() - 1)
                            ArrResult[j] = 1.0f;
                        else
                            break;
                    }
                }
                factor /= divideData.get(i).get(0).Factor;
            }
        }
        else
        {
            indexLast = data.size();
            for (int i = divideData.size() - 1; i >= 0; i--)
            {
                if (divideData.get(i).get(0).Factor == 0 || divideData.get(i).get(0).Factor == 1)
                    continue;

                for (int j = indexLast - 1; j >= 0; j--)
                {
                    if (j >= ArrResult.length)
                        continue;
                    if (data.get(j).m_date >= divideData.get(i).get(0).Date)
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
                factor /= divideData.get(i).get(0).Factor;
            }
        }
        return ArrResult;
    }
    
    /*
    * 获得复权数据
    * @param divideType 复权类型 
    * @param data 原始数据 
    * @param factor 复权因子
    * @return 复权后的k线数据
    */
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
    
    /*
    * 获得复权数据
    * @param divideType 复权类型
    * @param data 原始数据
    * @param factor 复权因子
    * @return 复权后的k线数据
    */
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

    /*
    * 获取163的K线
    * @param strCodes 代码
    */
    public static String get163LatestDatasByCodes(String strCodes)
    {
        String url = "http://api.money.126.net/data/feed/{0},money.api%5D";
        String[] codes = strCodes.split("[,]");
        ArrayList<String> requestCodes = new ArrayList<String>();
        for (int i = 0; i < codes.length; i++)
        {
            String strCode = codes[i];
            if (strCode.indexOf(".SH") != -1)
            {
                strCode = "0" + strCode.replace(".SH", "");
            }
            else if (strCode.indexOf(".SZ") != -1)
            {
                strCode = "1" + strCode.replace(".SZ", "");
            }
            requestCodes.add(strCode);
        }
        String codesStr = "";
        for (int i = 0; i < requestCodes.size(); i++)
        {
            codesStr += requestCodes.get(i);
            if (i != requestCodes.size() - 1)
            {
                codesStr += ",";
            }
        }
        FCHttpGetService httpGetService = new FCHttpGetService();
        return httpGetService.get(url.replace("{0}", codesStr));
    }
    
    /*
    * 根据字符串获取新浪的最新数据
    * @param str 数据字符串
    * @param formatType 格式
    * @param data 最新数据
    * @return 状态
    */
    public static int getLatestDataBy163Str(String str, int formatType, SecurityLatestData data)
    {
        try
        {
            str = str.replace("\"", "");
            String[] strs = str.split("[,]");
            for (int i = 0; i < strs.length; i++)
            {
                String key = strs[i].substring(0, strs[i].indexOf(": ")).replace(" ", "");
                String value = strs[i].substring(strs[i].indexOf(": ") + 2);
                switch (key)
                {
                    case "time":
                        String strDate = value;
                        String[] subStrs = strDate.split("[ ]");
                        String[] leftStrs = subStrs[0].split("[/]");
                        String[] rightStrs = subStrs[1].split("[:]");
                        data.m_date = FCTran.getDateNum(FCTran.strToInt(leftStrs[0]), FCTran.strToInt(leftStrs[1]), FCTran.strToInt(leftStrs[2]), FCTran.strToInt(rightStrs[0]), FCTran.strToInt(rightStrs[1]), FCTran.strToInt(rightStrs[2]), 0);
                        break;
                    case "code":
                        if (value.indexOf("0") == 0)
                        {
                            data.m_code = value.substring(1) + ".SH";
                        }
                        else
                        {
                            data.m_code = value.substring(1) + ".SZ";
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
    
    /*
    * 根据字符串获取新浪最新数据
    * @param str 数据字符串
    * @param formatType 格式
    * @param datas 最新数据
    * @return 状态
    */
    public static int getLatestDatasBy163Str(String str, int formatType, ArrayList<SecurityLatestData> datas)
    {
        String[] strs = str.split("[}]");
        int strLen = strs.length;
        for (int i = 0; i < strLen; i++)
        {
            SecurityLatestData latestData = new SecurityLatestData();
            String dataStr = strs[i];
            if (dataStr.length() > 50)
            {
                if (dataStr.lastIndexOf("{") != -1)
                {
                    dataStr = dataStr.substring(dataStr.lastIndexOf("{") + 1);
                }
                getLatestDataBy163Str(dataStr, formatType, latestData);
                if (latestData.m_date > 0)
                {
                    datas.add(latestData);
                }
            }
        }
        return 1;
    }
}
