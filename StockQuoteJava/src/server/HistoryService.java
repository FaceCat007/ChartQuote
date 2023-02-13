/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package server;
import facecat.topin.service.*;
import facecat.topin.core.*;
import service.*;
import java.util.*;

/**
 *
 * 历史数据服务
 */
public class HistoryService extends FCServerService  {
    /*
    * 创建服务
    */
    public HistoryService() {
        setServiceID(10001);
        setCompressType(COMPRESSTYPE_NONE);
    }
    
    /*
    * 服务编号
    */
    public final int SERVICEID = 10001;

    /*
    * 获取数据
    */
    public final int FUNCTIONID_GETDATA = 0;

    /*
    * 获取码表
    */
    public final int FUNCTIONID_GETCODE = 1;

    /*
    * 获取复权因子
    */
    public final int FUNCTIONID_GETDR = 2;

    /*
    * 分时数据缓存
    */
    public HashMap<String, MinuteDatasCache> m_minuteDatasCache = new HashMap<String, MinuteDatasCache>();

    /*
    * 接收最新数据
    * @param latestData 最新数据
    */
    public void sendData(SecurityLatestData latestData) {
        synchronized (m_minuteDatasCache) {
            MinuteDatasCache minuteDatasCache = null;
            if (!m_minuteDatasCache.containsKey(latestData.m_code)) {
                minuteDatasCache = new MinuteDatasCache();
                minuteDatasCache.m_code = latestData.m_code;
                m_minuteDatasCache.put(latestData.m_code, minuteDatasCache);
            } else {
                minuteDatasCache = m_minuteDatasCache.get(latestData.m_code);
            }
            minuteDatasCache.m_latestData = latestData;
            StockService.mergeRunTimeDatas(latestData, minuteDatasCache);
        }
    }
    
    /*
    * 落地日线数据
    */
    public void fallSecurityDatas() {
        synchronized (m_minuteDatasCache) {
            for (String code : m_minuteDatasCache.keySet()) {
                MinuteDatasCache minuteDatasCache = m_minuteDatasCache.get(code);
                String name = "";
                RefObject<String> refName = new RefObject<String>(name);
                ArrayList<SecurityData2> oldDatas = StockService.getSecurityDatas(code, refName);
                name = refName.argvalue;
                StockService.mergeSecurityDatas(minuteDatasCache.m_latestData, oldDatas);
                StockService.writeSecurityDatas(code, name, oldDatas);
                oldDatas.clear();
            }
        }
    }

    /// <summary>
    /// 落地分钟线数据
    /// </summary>
    /*
    *
    */
    public void fallMinuteSecurityDatas() {
        synchronized (m_minuteDatasCache) {
            for (String code : m_minuteDatasCache.keySet()) {
                MinuteDatasCache minuteDatasCache = m_minuteDatasCache.get(code);
                String name = "";
                RefObject<String> refName = new RefObject<String>(name);
                ArrayList<SecurityData2> oldDatas = StockService.getSecurityMinuteDatas(code, refName);
                name = refName.argvalue;
                StockService.mergeMinuteSecurityDatas(minuteDatasCache.m_datas, oldDatas);
                StockService.writeMinuteSecurityDatas(code, name, oldDatas);
                oldDatas.clear();
            }
        }
    }

    /*
    * 获取历史数据
    * @param code 代码
    * @param cycle 周期
    * @name 返回名称
    */
    public ArrayList<SecurityData2> getHistoryDatas(String code, int cycle, RefObject<String> name)
    {
        ArrayList<SecurityData2> sendDatas = new ArrayList<SecurityData2>();
        //分钟线
        if (cycle < 1440)
        {
            sendDatas = StockService.getSecurityMinuteDatas(code, name);
            synchronized (m_minuteDatasCache)
            {
                if (m_minuteDatasCache.containsKey(code))
                {
                    StockService.mergeMinuteSecurityDatas(sendDatas, m_minuteDatasCache.get(code).m_datas);
                }
            }
            if (cycle > 1)
            {
                ArrayList<SecurityData2> newDatas = new ArrayList<SecurityData2>();
                StockService.multiMinuteSecurityDatas(newDatas, sendDatas, cycle);
                sendDatas = newDatas;
            }
        }
        //日线
        else
        {
            sendDatas = StockService.getSecurityDatas(code, name);
            SecurityLatestData latestData = null;
            synchronized (m_minuteDatasCache)
            {
                if (m_minuteDatasCache.containsKey(code))
                {
                    latestData = m_minuteDatasCache.get(code).m_latestData;
                }
            }
            if (latestData != null)
            {
                StockService.mergeSecurityDatas(latestData, sendDatas);
            }
            if (cycle == 10080)
            {
                ArrayList<SecurityData2> newDatas = new ArrayList<SecurityData2>();
                StockService.getHistoryWeekDatas(newDatas, sendDatas);
                sendDatas = newDatas;
            }
            else if (cycle == 43200)
            {
                ArrayList<SecurityData2> newDatas = new ArrayList<SecurityData2>();
                StockService.getHistoryMonthDatas(newDatas, sendDatas);
                sendDatas = newDatas;
            }
        }
        return sendDatas;
    }

    /*
    * 接收数据
    * @param message 消息
    */
    public void onReceive(FCMessage message) {
        super.onReceive(message);
        if (message.m_functionID == FUNCTIONID_GETDATA) {
            try{
                FCBinary br = new FCBinary();
                br.write(message.m_body, message.m_bodyLength);
                String code = br.readString();
                int cycle = br.readInt();
                double startDate = br.readDouble();
                double endDate = br.readDouble();
                br.close();
                String name = "";
                RefObject<String> refName = new RefObject<String>(name);
                ArrayList<SecurityData2> sendDatas = getHistoryDatas(code, cycle, refName);
                name = refName.argvalue;
                FCBinary bw = new FCBinary();
                bw.writeString(code);
                bw.writeInt(cycle);
                bw.writeDouble(startDate);
                bw.writeDouble(endDate);
                SecurityLatestData ld = new SecurityLatestData();
                if (StockService.getLatestData(code, ld) > 0) {
                    bw.writeDouble(ld.m_date);
                    bw.writeDouble(ld.m_volume);
                    bw.writeDouble(ld.m_amount);
                } else {
                    bw.writeDouble(0);
                    bw.writeDouble(0);
                    bw.writeDouble(0);
                }
                int oldDatasSize = sendDatas.size();
                if (startDate > 0 || endDate > 0) {
                    for (int i = 0; i < oldDatasSize; i++) {
                        SecurityData2 data = sendDatas.get(i);
                        if (startDate > 0 && data.m_date < startDate) {
                            sendDatas.remove(i);
                            i--;
                            oldDatasSize--;
                        } else if (endDate > 0 && data.m_date > endDate) {
                            sendDatas.remove(i);
                            i--;
                            oldDatasSize--;
                        }
                    }
                }
                oldDatasSize = sendDatas.size();
                bw.writeInt(oldDatasSize);
                for (int i = 0; i < oldDatasSize; i++) {
                    SecurityData2 data = sendDatas.get(i);
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
                int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETDATA, message.m_requestID, message.m_socketID, 0, 0, bytes.length, bytes));
            }catch(Exception ex){
                
            }
        } else if (message.m_functionID == FUNCTIONID_GETCODE) {
            try{
                FCBinary bw = new FCBinary();
                bw.writeInt(StockService.m_codedMap.size());
                for (Security security : StockService.m_codedMap.values()) {
                    bw.writeString(security.m_code);
                    bw.writeString(security.m_name);
                    bw.writeString(security.m_pingyin);
                    bw.writeString(security.m_status);
                    bw.writeString(security.m_market);
                }
                byte[] bytes = bw.getBytes();
                bw.close();
                int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETCODE, message.m_requestID, message.m_socketID, 0, 0, bytes.length, bytes));
            }catch(Exception ex){
                
            }
        }
        else if (message.m_functionID == FUNCTIONID_GETDR)
        {
            try{
                FCBinary bw = new FCBinary();
                HashMap<String, ArrayList<ArrayList<OneDivideRightBase>>> devideRightDatas = StockService.m_devideRightDatas;
                bw.writeInt(devideRightDatas.size());
                for (String key : devideRightDatas.keySet())
                {
                    bw.writeString(key);
                    ArrayList<ArrayList<OneDivideRightBase>> oneDatas = devideRightDatas.get(key);
                    bw.writeInt(oneDatas.size());
                    for (int i = 0; i < oneDatas.size(); i++)
                    {
                        ArrayList<OneDivideRightBase> oneDatas2 = oneDatas.get(i);
                        bw.writeInt(oneDatas2.size());
                        for (int j = 0; j < oneDatas2.size(); j++)
                        {
                            OneDivideRightBase oneDivideRightBase = oneDatas2.get(j);
                            bw.writeDouble(oneDivideRightBase.Date);
                            bw.writeFloat(oneDivideRightBase.DataValue);
                            bw.writeString(oneDivideRightBase.DivideType.toString());
                            bw.writeFloat(oneDivideRightBase.PClose);
                            bw.writeFloat(oneDivideRightBase.Factor);
                        }
                    }
                }
                byte[] bytes = bw.getBytes();
                bw.close();
                int ret = send(new FCMessage(getServiceID(), FUNCTIONID_GETDR, message.m_requestID, message.m_socketID, 0, 0, bytes.length, bytes));
            }catch(Exception ex){
                
            }
        }
    }
}
