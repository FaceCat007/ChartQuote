/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package service;
import java.util.*;

/**
 *
 * 分时线缓存
 */
public class MinuteDatasCache {
    /*
    * 代码
    */
    public String m_code = "";

    /*
    * 数据
    */
    public ArrayList<SecurityData2> m_datas = new ArrayList<SecurityData2>();

    /*
    * 上次的成交额
    */
    public double m_lastAmount;

    /*
    * 上次日期
    */
    public double m_lastDate;

    /*
    * 上次的成交量
    */
    public double m_lastVolume;

    /*
    * 最新数据
    */
    public SecurityLatestData m_latestData;
}
