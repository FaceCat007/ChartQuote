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
 * 最新数据
 */
public class LatestService extends FCServerService  {
    public LatestService() {
        setServiceID(SERVICEID);
    }

    /*
    * 服务编号
    */
    public final int SERVICEID = 10003;

    /*
    * 开始行情推送
    */
    public final int FUNCTION_SUBCODES = 0;

    /*
    * 取消行情推送
    */
    public final int FUNCTION_UNSUBCODES = 1;
    
    /*
    * 最新数据
    */
    public final int FUNCTION_NEWDATA = 2;

    /*
    * 套接字ID
    */
    private HashMap<String, ArrayList<LatestRequestInfo>> m_socketIDs = new HashMap<String, ArrayList<LatestRequestInfo>>();

    /*
    * 服务关闭
    * @param socketID 套接字ID
    * @param localSID 本地套接字ID
    */
    public void onClientClose(int socketID, int localSID) {
        super.onClientClose(socketID, localSID);
        synchronized (m_socketIDs) {
            for (String code : m_socketIDs.keySet()) {
                ArrayList<LatestRequestInfo> riList = m_socketIDs.get(code);
                int riListSize = riList.size();
                for (int i = 0; i < riListSize; i++) {
                    if (riList.get(i).m_socketID == socketID) {
                        riList.remove(riList.get(i));
                        i--;
                        riListSize--;
                    }
                }
            }
        }
    } 

    /*
    * 接收消息
    * @param message 消息
    */
    public void onReceive(FCMessage message) {
        super.onReceive(message);
        if (message.m_functionID == FUNCTION_SUBCODES) {
            try{
                ArrayList<String> subCodes = new ArrayList<String>();
                synchronized (m_socketIDs) {
                    FCBinary br = new FCBinary();
                    br.write(message.m_body, message.m_bodyLength);
                    String codeStr = br.readString();
                    br.close();
                    String[] codes = codeStr.split("[,]");
                    for (String code : codes) {
                        if (!m_socketIDs.containsKey(code)) {
                            m_socketIDs.put(code, new ArrayList<LatestRequestInfo>());
                        }
                        boolean hasSocketID = false;
                        for (LatestRequestInfo ri : m_socketIDs.get(code)) {
                            if (ri.m_socketID == message.m_socketID) {
                                hasSocketID = true;
                                break;
                            }
                        }
                        if (!hasSocketID) {
                            LatestRequestInfo requestInfo = new LatestRequestInfo();
                            requestInfo.m_socketID = message.m_socketID;
                            requestInfo.m_requestID = message.m_requestID;
                            m_socketIDs.get(code).add(requestInfo);
                            subCodes.add(code);
                        }
                    }
                }
                for (String code : subCodes) {
                    SecurityLatestData securityLatestData = new SecurityLatestData();
                    if (StockService.getLatestData(code, securityLatestData) > 0) {
                        sendData(securityLatestData, message.m_socketID);
                    }
                }
            }catch(Exception ex){
            }
        } else if (message.m_functionID == FUNCTION_UNSUBCODES) {
            try{
                synchronized (m_socketIDs) {
                    FCBinary br = new FCBinary();
                    br.write(message.m_body, message.m_bodyLength);
                    String codeStr = br.readString();
                    br.close();
                    for (ArrayList<LatestRequestInfo> req : m_socketIDs.values()) {
                        for (int i = 0; i < req.size(); i++) {
                            LatestRequestInfo ri = req.get(i);
                            if (ri.m_socketID == message.m_socketID && ri.m_requestID == message.m_requestID) {
                                req.remove(ri);
                            }
                        }
                    }
                }
            }catch(Exception ex){
                
            }
        }
    }

    /*
    * 发送最新数据
    */
    public void sendData(SecurityLatestData data, int socketID) {
        try{
            ArrayList<LatestRequestInfo> socketIDs = new ArrayList<LatestRequestInfo>();
            synchronized (m_socketIDs) {
                if (m_socketIDs.containsKey(data.m_code)) {
                    for(int i = 0; i < m_socketIDs.get(data.m_code).size(); i++){
                        socketIDs.add(m_socketIDs.get(data.m_code).get(i));
                    }
                }
            }
            if (socketIDs.size() > 0) {
                byte[] body = null;
                if (true) {
                    FCBinary bw = new FCBinary();
                    bw.writeString(data.m_code);
                    bw.writeDouble(data.m_open);
                    bw.writeDouble(data.m_lastClose);
                    bw.writeDouble(data.m_close);
                    bw.writeDouble(data.m_high);
                    bw.writeDouble(data.m_low);
                    bw.writeDouble(data.m_volume);
                    bw.writeDouble(data.m_amount);
                    bw.writeInt(data.m_buyVolume1);
                    bw.writeDouble(data.m_buyPrice1);
                    bw.writeInt(data.m_buyVolume2);
                    bw.writeDouble(data.m_buyPrice2);
                    bw.writeInt(data.m_buyVolume3);
                    bw.writeDouble(data.m_buyPrice3);
                    bw.writeInt(data.m_buyVolume4);
                    bw.writeDouble(data.m_buyPrice4);
                    bw.writeInt(data.m_buyVolume5);
                    bw.writeDouble(data.m_buyPrice5);
                    bw.writeInt(data.m_sellVolume1);
                    bw.writeDouble(data.m_sellPrice1);
                    bw.writeInt(data.m_sellVolume2);
                    bw.writeDouble(data.m_sellPrice2);
                    bw.writeInt(data.m_sellVolume3);
                    bw.writeDouble(data.m_sellPrice3);
                    bw.writeInt(data.m_sellVolume4);
                    bw.writeDouble(data.m_sellPrice4);
                    bw.writeInt(data.m_sellVolume5);
                    bw.writeDouble(data.m_sellPrice5);
                    bw.writeInt(data.m_innerVol);
                    bw.writeInt(data.m_outerVol);
                    bw.writeDouble(data.m_turnoverRate);
                    bw.writeDouble(data.m_openInterest);
                    bw.writeDouble(data.m_settlePrice);
                    bw.writeDouble(data.m_date);
                    body = bw.getBytes();
                    bw.close();
                }
                byte[] finalBytes = null;
                if (true) {
                    FCBinary bw = new FCBinary();
                    int bodyLength = body.length;
                    int uncBodyLength = bodyLength;
                    int len = 4 * 3 + bodyLength + 2 * 2 + 1 * 2;
                    bw.writeInt(len);
                    bw.writeShort((short)getServiceID());
                    bw.writeShort((short)FUNCTION_NEWDATA);
                    bw.writeInt(0);
                    bw.writeByte((byte)0);
                    bw.writeByte((byte)0);
                    bw.writeInt(uncBodyLength);
                    bw.writeBytes(body);
                    finalBytes = bw.getBytes();
                    bw.close();
                }

                for (int i = 0; i < socketIDs.size(); i++) {
                    LatestRequestInfo ri = socketIDs.get(i);
                    if (socketID == -1 || socketID == ri.m_socketID) {
                        finalBytes[8] = (byte)(ri.m_requestID & 0xff);
                        finalBytes[9] = (byte)((ri.m_requestID & 0xff00) >> 8);
                        finalBytes[10] = (byte)((ri.m_requestID & 0xff0000) >> 16);
                        finalBytes[11] = (byte)((ri.m_requestID & 0xff000000) >> 24);
                        int ret = FCServerSockets.send(ri.m_socketID, getSocketID(), finalBytes, finalBytes.length);
                        m_upFlow += ret;
                    }
                }
            }
        }catch(Exception ex){
            
        }
    }
}
