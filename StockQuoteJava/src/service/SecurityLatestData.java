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
public class SecurityLatestData {
    /*
    * 名称
    */
    public String m_name = "";
    
    /*
    * 成交额
    */
    public double m_amount;

    /*
    (* 买一量
    */
    public int m_buyVolume1;

    /*
    * 买二量
    */
    public int m_buyVolume2;

    /*
    * 买三量
    */
    public int m_buyVolume3;

    /*
    * 买四量
    */
    public int m_buyVolume4;

    /*
    * 买五量
    */
    public int m_buyVolume5;

    /*
    * 买六量
    */
    public int m_buyVolume6;

    /*
    * 买七量
    */
    public int m_buyVolume7;

    /*
    * 买八量
    */
    public int m_buyVolume8;

    /*
    * 买九量
    */
    public int m_buyVolume9;

    /*
    * 买十量
    */
    public int m_buyVolume10;

    /*
    * 买一价
    */
    public double m_buyPrice1;

    /*
    * 买二价
    */
    public double m_buyPrice2;

    /*
    * 买三价
    */
    public double m_buyPrice3;

    /*
    * 买四价
    */
    public double m_buyPrice4;

    /*
    * 买五价
    */
    public double m_buyPrice5;

    /*
    * 买六价
    */
    public double m_buyPrice6;

    /*
    * 买七价
    */
    public double m_buyPrice7;

    /*
    * 买八价
    */
    public double m_buyPrice8;

    /*
    * 买九价
    */
    public double m_buyPrice9;

    /*
    * 买十价
    */
    public double m_buyPrice10;

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
    * 卖一量
    */
    public int m_sellVolume1;

    /*
    * 卖二量
    */
    public int m_sellVolume2;

    /*
    * 卖三量
    */
    public int m_sellVolume3;

    /*
    * 卖四量
    */
    public int m_sellVolume4;

    /*
    * 卖五量
    */
    public int m_sellVolume5;

    /*
    * 卖六量
    */
    public int m_sellVolume6;

    /*
    * 卖七量
    */
    public int m_sellVolume7;

    /*
    * 卖八量
    */
    public int m_sellVolume8;

    /*
    * 卖九量
    */ 
    public int m_sellVolume9;

    /*
    * 卖十量
    */
    public int m_sellVolume10;

    /*
    * 卖一价
    */
    public double m_sellPrice1;

    /*
    * 卖二价
    */
    public double m_sellPrice2;

    /*
    * 卖三价
    */
    public double m_sellPrice3;

    /*
    * 卖四价
    */ 
    public double m_sellPrice4;

    /*
    * 卖五价
    */
    public double m_sellPrice5;

    /*
    * 卖六价
    */
    public double m_sellPrice6;

    /*
    * 卖七价
    */
    public double m_sellPrice7;

    /*
    * 卖八价
    */
    public double m_sellPrice8;

    /*
    * 卖九价
    */
    public double m_sellPrice9;

    /*
    * 卖十价
    */
    public double m_sellPrice10;

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
        m_buyVolume1 = data.m_buyVolume1;
        m_buyVolume2 = data.m_buyVolume2;
        m_buyVolume3 = data.m_buyVolume3;
        m_buyVolume4 = data.m_buyVolume4;
        m_buyVolume5 = data.m_buyVolume5;
        m_buyPrice1 = data.m_buyPrice1;
        m_buyPrice2 = data.m_buyPrice2;
        m_buyPrice3 = data.m_buyPrice3;
        m_buyPrice4 = data.m_buyPrice4;
        m_buyPrice5 = data.m_buyPrice5;
        m_buyVolume6 = data.m_buyVolume6;
        m_buyVolume7 = data.m_buyVolume7;
        m_buyVolume8 = data.m_buyVolume8;
        m_buyVolume9 = data.m_buyVolume9;
        m_buyVolume10 = data.m_buyVolume10;
        m_buyPrice6 = data.m_buyPrice6;
        m_buyPrice7 = data.m_buyPrice7;
        m_buyPrice8 = data.m_buyPrice8;
        m_buyPrice9 = data.m_buyPrice9;
        m_buyPrice10 = data.m_buyPrice10;
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
        m_sellVolume1 = data.m_sellVolume1;
        m_sellVolume2 = data.m_sellVolume2;
        m_sellVolume3 = data.m_sellVolume3;
        m_sellVolume4 = data.m_sellVolume4;
        m_sellVolume5 = data.m_sellVolume5;
        m_sellPrice1 = data.m_sellPrice1;
        m_sellPrice2 = data.m_sellPrice2;
        m_sellPrice3 = data.m_sellPrice3;
        m_sellPrice4 = data.m_sellPrice4;
        m_sellPrice5 = data.m_sellPrice5;
        m_settlePrice = data.m_settlePrice;
        m_sellVolume6 = data.m_sellVolume6;
        m_sellVolume7 = data.m_sellVolume7;
        m_sellVolume8 = data.m_sellVolume8;
        m_sellVolume9 = data.m_sellVolume9;
        m_sellVolume10 = data.m_sellVolume10;
        m_sellPrice6 = data.m_sellPrice6;
        m_sellPrice7 = data.m_sellPrice7;
        m_sellPrice8 = data.m_sellPrice8;
        m_sellPrice9 = data.m_sellPrice9;
        m_sellPrice10 = data.m_sellPrice10;
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
        && m_buyVolume1 == data.m_buyVolume1
        && m_buyVolume2 == data.m_buyVolume2
        && m_buyVolume3 == data.m_buyVolume3
        && m_buyVolume4 == data.m_buyVolume4
        && m_buyVolume5 == data.m_buyVolume5
        && m_buyPrice1 == data.m_buyPrice1
        && m_buyPrice2 == data.m_buyPrice2
        && m_buyPrice3 == data.m_buyPrice3
        && m_buyPrice4 == data.m_buyPrice4
        && m_buyPrice5 == data.m_buyPrice5
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
        && m_sellVolume1 == data.m_sellVolume1
        && m_sellVolume2 == data.m_sellVolume2
        && m_sellVolume3 == data.m_sellVolume3
        && m_sellVolume4 == data.m_sellVolume4
        && m_sellVolume5 == data.m_sellVolume5
        && m_sellPrice1 == data.m_sellPrice1
        && m_sellPrice2 == data.m_sellPrice2
        && m_sellPrice3 == data.m_sellPrice3
        && m_sellPrice4 == data.m_sellPrice4
        && m_sellPrice5 == data.m_sellPrice5
        && m_settlePrice == data.m_settlePrice
        && m_turnoverRate == data.m_turnoverRate
        && m_volume == data.m_volume)
        {
            return true;
        }
        return false;
    }
}
