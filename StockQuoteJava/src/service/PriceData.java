/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package service;

/**
 *
 * 股票实时数据
 */
public class PriceData {
    /*
    * 成交额
    */
    public double m_amount;

    /*
    * 当前价格
    */
    public double m_close;

    /*
    * 股票代码
    */
    public String m_code = "";

    /*
    * 日期及时间
    */
    public double m_date;

    /*
    * 最高价
    */
    public double m_high;

    /*
    * 内盘成交量
    */
    public int m_innerVol;

    /*
    * 昨日收盘价
    */
    public double m_lastClose;

    /*
    * 最低价
    */
    public double m_low;

    /*
    * 开盘价
    */
    public double m_open;

    /*
    * 期货持仓量
    */
    public double m_openInterest;

    /*
    * 外盘成交量
    */
    public int m_outerVol;

    /*
    * 期货结算价
    */
    public double m_settlePrice;

    /*
    * 换手率
    */
    public double m_turnoverRate;

    /*
    * 成交量
    */
    public double m_volume;

    /*
    * 复制数据
    * @param data 数据
    */
    public void copy(SecurityLatestData data)
    {
        if (data == null) return;
        m_amount = data.m_amount;
        m_close = data.m_close;
        m_date = data.m_date;
        m_high = data.m_high;
        m_innerVol = data.m_innerVol;
        m_lastClose = data.m_lastClose;
        m_low = data.m_low;
        m_open = data.m_open;
        m_openInterest = data.m_openInterest;
        m_outerVol = data.m_outerVol;
        m_code = data.m_code;
        m_settlePrice = data.m_settlePrice;
        m_turnoverRate = data.m_turnoverRate;
        m_volume = data.m_volume;
    }

    /*
    * 比较是否相同
    * @param data 数据
    * @return 是否相同
    */
    public boolean equal(SecurityLatestData data)
    {
        if (data == null) return false;
        if (m_amount == data.m_amount
        && m_close == data.m_close
        && m_date == data.m_date
        && m_high == data.m_high
        && m_innerVol == data.m_innerVol
        && m_lastClose == data.m_lastClose
        && m_low == data.m_low
        && m_open == data.m_open
        && m_openInterest == data.m_openInterest
        && m_outerVol == data.m_outerVol
        && m_code == data.m_code
        && m_settlePrice == data.m_settlePrice
        && m_turnoverRate == data.m_turnoverRate
        && m_volume == data.m_volume)
        {
            return true;
        }
        return false;
    }
}
