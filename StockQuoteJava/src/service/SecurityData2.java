/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package service;

/*
* 证券历史数据
*/
public class SecurityData2 {
    /*
    * 收盘价
    */
    public double m_close;

    /*
    * 日期
    */
    public double m_date;

    /*
    * 最高价
    */
    public double m_high;

    /*
    * 最低价
    */
    public double m_low;

    /*
    * 开盘价
    */
    public double m_open;

    /// <summary>
    /// 
    /// </summary>
    public double m_volume;

    /*
    * 成交量
    */
    public double m_amount;

    /// <summary>
    /// 前复权因子
    /// </summary>
    public float m_forwardFactor;

    /*
    * 后复权因子
    */
    public float m_backFactor;
    
    /*
    * 复制数据
    * @param data 数据
    */
    public void copy(SecurityData2 data)
    {
        m_close = data.m_close;
        m_date = data.m_date;
        m_high = data.m_high;
        m_low = data.m_low;
        m_open = data.m_open;
        m_volume = data.m_volume;
        m_amount = data.m_amount;
        m_forwardFactor = data.m_forwardFactor;
        m_backFactor = data.m_backFactor;
    }
}
