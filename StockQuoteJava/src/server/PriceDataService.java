/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package server;
import facecat.topin.service.*;
import facecat.topin.core.*;
import facecat.topin.sock.*;
import service.*;
import java.util.*;

/**
 *
 * 报价服务
 */
public class PriceDataService extends FCServerService  {
     public PriceDataService() {
        setServiceID(SERVICEID);
    }

    /*
    * 服务编号
    */
    public final int SERVICEID = 10004;

    /*
    * 开始推送
    */
    public final int FUNCTION_SUBCODES = 0;

    /*
    * 取消推送
    */
    public final int FUNCTION_UNSUBCODES = 1;

    /*
    * 最新数据
    */
    public final int FUNCTION_NEWDATA = 2;

    /*
    * 套接字集合
    */
    private ArrayList<PriceDataRequestInfo> m_socketIDs = new ArrayList<PriceDataRequestInfo>();

    /*
    * 连接关闭方法
    * @param socketID 套接字ID
    * @param localSID 本地套接字ID
    */
    public void onClientClose(int socketID, int localSID) {
        super.onClientClose(socketID, localSID);
        synchronized (m_socketIDs) {
            int size = m_socketIDs.size();
            for (int i = 0; i < size; i++) {
                if (m_socketIDs.get(i).m_socketID == socketID) {
                    m_socketIDs.remove(i);
                    i--;
                    size--;
                }
            }
        }
    }

    /*
    * 接收数据方法
    */
    public void onReceive(FCMessage message) {
        super.onReceive(message);
        if (message.m_functionID == FUNCTION_SUBCODES) {
            try{
                synchronized (m_socketIDs) {
                    FCBinary br = new FCBinary();
                    br.write(message.m_body, message.m_bodyLength);
                    String codeStr = br.readString();
                    br.close();
                    PriceDataRequestInfo req = new PriceDataRequestInfo();
                    req.m_requestID = message.m_requestID;
                    req.m_codes = codeStr.split("[,]");
                    req.m_socketID = message.m_socketID;
                    m_socketIDs.add(req);
                    HashMap<String, SecurityLatestData> latestDatas = new HashMap<String, SecurityLatestData>();
                    for (int i = 0; i < req.m_codes.length; i++) {
                        SecurityLatestData securityLatestData = new SecurityLatestData();
                        if (StockService.getLatestData(req.m_codes[i], securityLatestData) > 0) {
                            latestDatas.put(req.m_codes[i], securityLatestData);
                        }
                    }
                    sendDatas(latestDatas, message.m_socketID);
                }
            }catch(Exception ex){
                
            }
        } else if (message.m_functionID == FUNCTION_UNSUBCODES) {
            synchronized (m_socketIDs) {
                int size = m_socketIDs.size();
                for (int i = 0; i < size; i++) {
                    if (m_socketIDs.get(i).m_socketID == message.m_socketID
                        && m_socketIDs.get(i).m_requestID == message.m_requestID) {
                        m_socketIDs.remove(i);
                        i--;
                        size--;
                    }
                }
            }
        }
    }
    /*
    * 发送数据
    * @param latestDatas 最新数据
    */
    public void sendDatas(HashMap<String, SecurityLatestData> latestDatas, int socketID) {
        try{
            ArrayList<PriceDataRequestInfo> requestInfos = new ArrayList<PriceDataRequestInfo>();
            synchronized (m_socketIDs) {
                for (PriceDataRequestInfo req : m_socketIDs) {
                    if (socketID == -1 || req.m_socketID == socketID) {
                        requestInfos.add(req);
                    }
                }
            }
            int requestInfosSize = requestInfos.size();
            for (int i = 0; i < requestInfosSize; i++) {
                PriceDataRequestInfo req = requestInfos.get(i);
                ArrayList<SecurityLatestData> sendDatas = new ArrayList<SecurityLatestData>();
                for (int j = 0; j < req.m_codes.length; j++) {
                    if(latestDatas.containsKey(req.m_codes[j])){
                        sendDatas.add(latestDatas.get(req.m_codes[j]));
                    }
                }
                if (sendDatas.size() > 0) {
                    FCBinary bw = new FCBinary();
                    bw.writeInt(sendDatas.size());
                    for (int j = 0; j < sendDatas.size(); j++) {
                        SecurityLatestData data = sendDatas.get(j);
                        bw.writeString(data.m_code);
                        bw.writeDouble(data.m_open);
                        bw.writeDouble(data.m_lastClose);
                        bw.writeDouble(data.m_close);
                        bw.writeDouble(data.m_high);
                        bw.writeDouble(data.m_low);
                        bw.writeDouble(data.m_volume);
                        bw.writeDouble(data.m_amount);
                        bw.writeDouble(data.m_date);
                    }
                    byte[] bytes = bw.getBytes();
                    bw.close();
                    int ret = send(new FCMessage(getServiceID(), FUNCTION_NEWDATA, req.m_requestID, req.m_socketID, 0, 0, bytes.length, bytes));
                    sendDatas.clear();
                    //Console.WriteLine(JsonConvert.SerializeObject(sendDatas));
                }
            }
        }catch(Exception ex){
            
        }
    }
}
