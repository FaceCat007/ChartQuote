/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package service;
import server.*;
import facecat.topin.service.*;
import facecat.topin.core.*;
import facecat.topin.sock.*;

/**
 *
 * @author taode
 */
public class DataCenter {
    /*
    * 文件分隔符号
    */
    //public static String m_seperator = "/"; //Linux
    public static String m_seperator = "\\"; //Windows
    
    /*
    * 获取程序路径
    */
    public static String getAppPath(){
        //return System.getProperty("user.dir").replace("\\", "/"); //Linux
        return System.getProperty("user.dir"); //Windows
    }
    
    /*
    * 历史数据服务
    */
    public static HistoryService m_historyService;

    /*
    * 最新数据服务
    */
    public static LatestService m_latestService;
    
    /*
    * 报价服务
    */
    public static PriceDataService m_priceDataService;
    
    /*
    * 启动服务
    */
    public static void startService() {
        String dayDir = getAppPath() + m_seperator + "day";
        String minuteDir = getAppPath() + m_seperator + "minute";
        if (!FCFile.isDirectoryExist(dayDir))
        {
            FCFile.createDirectory(dayDir);
        }
        if (!FCFile.isDirectoryExist(minuteDir))
        {
            FCFile.createDirectory(minuteDir);
        }
        int socketID = FCServerService.startServer(9957, new byte[] { 0, 0, 0, 0 });
        FCServerSockets.setLimit(socketID, 100, 5 * 60 * 1000);

        m_latestService = new LatestService();
        m_historyService = new HistoryService();
        m_priceDataService = new PriceDataService();
        FCServerService.addService(m_historyService);
        FCServerService.addService(m_latestService);
        FCServerService.addService(m_priceDataService);

        //启动行情服务端
        StockService.load();
        StockService.start();
        m_latestService.setSocketID(socketID);
        m_historyService.setSocketID(socketID);
        m_priceDataService.setSocketID(socketID);
        new Thread(runnable).start();
    }
    
    public static Runnable runnable = new Runnable() {
        @Override
        public void run() {
            FCHttpMonitor httpMonitor = new FCHttpMonitor(getAppPath() + m_seperator + "start.js");
            FCHttpMonitor.m_easyServices.put("quote", new HttpService());
            httpMonitor.start();
        }
    };
}
