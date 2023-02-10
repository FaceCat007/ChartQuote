/*
 * iChart行情系统(开源)
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace FaceCat
{
    /// <summary>
    /// 证券历史数据
    /// </summary>
    public class SecurityData2
    {
        /// <summary>
        /// 收盘价
        /// </summary>
        public double m_close;

        /// <summary>
        /// 日期
        /// </summary>
        public double m_date;

        /// <summary>
        /// 最高价
        /// </summary>
        public double m_high;

        /// <summary>
        /// 最低价
        /// </summary>
        public double m_low;

        /// <summary>
        /// 开盘价
        /// </summary>
        public double m_open;

        /// <summary>
        /// 成交量
        /// </summary>
        public double m_volume;

        /// <summary>
        /// 成交额
        /// </summary>
        public double m_amount;

        /// <summary>
        /// 前复权因子
        /// </summary>
        public float m_forwardFactor;

        /// <summary>
        /// 后复权因子
        /// </summary>
        public float m_backFactor;

        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="data">数据</param>
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

    /// <summary>
    /// 分时线缓存
    /// </summary>
    public class MinuteDatasCache
    {
        /// <summary>
        /// 代码
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// 数据
        /// </summary>
        public List<SecurityData2> m_datas = new List<SecurityData2>();

        /// <summary>
        /// 上次的成交额
        /// </summary>
        public double m_lastAmount;

        /// <summary>
        /// 上次日期
        /// </summary>
        public double m_lastDate;

        /// <summary>
        /// 上次的成交量
        /// </summary>
        public double m_lastVolume;

        /// <summary>
        /// 最新数据
        /// </summary>
        public SecurityLatestData m_latestData;
    }

    /// <summary>
    /// 客户端数据缓存
    /// </summary>
    public class ClientTickDataCache
    {
        /// <summary>
        /// 代码
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// 上次的成交额
        /// </summary>
        public double m_lastAmount;

        /// <summary>
        /// 上次日期
        /// </summary>
        public double m_lastDate;

        /// <summary>
        /// 上次的成交量
        /// </summary>
        public double m_lastVolume;
    }

    /// <summary>
    /// 最新数据的请求
    /// </summary>
    public class LatestRequestInfo
    {
        /// <summary>
        /// 套接字ID
        /// </summary>
        public int m_socketID;

        /// <summary>
        /// 请求ID
        /// </summary>
        public int m_requestID;
    }
    /// <summary>
    /// 股票信息
    /// </summary>
    public class Security
    {
        /// <summary>
        /// 创建键盘精灵
        /// </summary>
        public Security()
        {
        }

        /// <summary>
        /// 股票代码
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// 股票名称
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// 拼音
        /// </summary>
        public String m_pingyin = "";

        /// <summary>
        /// 状态
        /// </summary>
        public String m_status;

        /// <summary>
        /// 市场类型
        /// </summary>
        public String m_market;
    }

    /// <summary>
    /// 股票实时数据
    /// </summary>
    public class SecurityLatestData
    {
        /// <summary>
        /// 名称
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// 成交额
        /// </summary>
        public double m_amount;

        /// <summary>
        /// 买一量
        /// </summary>
        public int m_buyVolume1;

        /// <summary>
        /// 买二量
        /// </summary>
        public int m_buyVolume2;

        /// <summary>
        /// 买三量
        /// </summary>
        public int m_buyVolume3;

        /// <summary>
        /// 买四量
        /// </summary>
        public int m_buyVolume4;

        /// <summary>
        /// 买五量
        /// </summary>
        public int m_buyVolume5;

        /// <summary>
        /// 买六量
        /// </summary>
        public int m_buyVolume6;

        /// <summary>
        /// 买七量
        /// </summary>
        public int m_buyVolume7;

        /// <summary>
        /// 买八量
        /// </summary>
        public int m_buyVolume8;

        /// <summary>
        /// 买九量
        /// </summary>
        public int m_buyVolume9;

        /// <summary>
        /// 买十量
        /// </summary>
        public int m_buyVolume10;

        /// <summary>
        /// 买一价
        /// </summary>
        public double m_buyPrice1;

        /// <summary>
        /// 买二价
        /// </summary>
        public double m_buyPrice2;

        /// <summary>
        /// 买三价
        /// </summary>
        public double m_buyPrice3;

        /// <summary>
        /// 买四价
        /// </summary>
        public double m_buyPrice4;

        /// <summary>
        /// 买五价
        /// </summary>
        public double m_buyPrice5;

        /// <summary>
        /// 买六价
        /// </summary>
        public double m_buyPrice6;

        /// <summary>
        /// 买七价
        /// </summary>
        public double m_buyPrice7;

        /// <summary>
        /// 买八价
        /// </summary>
        public double m_buyPrice8;

        /// <summary>
        /// 买九价
        /// </summary>
        public double m_buyPrice9;

        /// <summary>
        /// 买十价
        /// </summary>
        public double m_buyPrice10;

        /// <summary>
        /// 当前价格
        /// </summary>
        public double m_close;

        /// <summary>
        /// 股票代码
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// 日期及时间
        /// </summary>
        public double m_date;

        /// <summary>
        /// 最高价
        /// </summary>
        public double m_high;

        /// <summary>
        /// 内盘成交量
        /// </summary>
        public int m_innerVol;

        /// <summary>
        /// 昨日收盘价
        /// </summary>
        public double m_lastClose;

        /// <summary>
        /// 最低价
        /// </summary>
        public double m_low;

        /// <summary>
        /// 开盘价
        /// </summary>
        public double m_open;

        /// <summary>
        /// 期货持仓量
        /// </summary>
        public double m_openInterest;

        /// <summary>
        /// 外盘成交量
        /// </summary>
        public int m_outerVol;

        /// <summary>
        /// 卖一量
        /// </summary>
        public int m_sellVolume1;

        /// <summary>
        /// 卖二量
        /// </summary>
        public int m_sellVolume2;

        /// <summary>
        /// 卖三量
        /// </summary>
        public int m_sellVolume3;

        /// <summary>
        /// 卖四量
        /// </summary>
        public int m_sellVolume4;

        /// <summary>
        /// 卖五量
        /// </summary>
        public int m_sellVolume5;

        /// <summary>
        /// 卖六量
        /// </summary>
        public int m_sellVolume6;

        /// <summary>
        /// 卖七量
        /// </summary>
        public int m_sellVolume7;

        /// <summary>
        /// 卖八量
        /// </summary>
        public int m_sellVolume8;

        /// <summary>
        /// 卖九量
        /// </summary>
        public int m_sellVolume9;

        /// <summary>
        /// 卖十量
        /// </summary>
        public int m_sellVolume10;

        /// <summary>
        /// 卖一价
        /// </summary>
        public double m_sellPrice1;

        /// <summary>
        /// 卖二价
        /// </summary>
        public double m_sellPrice2;

        /// <summary>
        /// 卖三价
        /// </summary>
        public double m_sellPrice3;

        /// <summary>
        /// 卖四价
        /// </summary>
        public double m_sellPrice4;

        /// <summary>
        /// 卖五价
        /// </summary>
        public double m_sellPrice5;

        /// <summary>
        /// 卖六价
        /// </summary>
        public double m_sellPrice6;

        /// <summary>
        /// 卖七价
        /// </summary>
        public double m_sellPrice7;

        /// <summary>
        /// 卖八价
        /// </summary>
        public double m_sellPrice8;

        /// <summary>
        /// 卖九价
        /// </summary>
        public double m_sellPrice9;

        /// <summary>
        /// 卖十价
        /// </summary>
        public double m_sellPrice10;

        /// <summary>
        /// 期货结算价
        /// </summary>
        public double m_settlePrice;

        /// <summary>
        /// 换手率
        /// </summary>
        public double m_turnoverRate;

        /// <summary>
        /// 成交量
        /// </summary>
        public double m_volume;

        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="data">数据</param>
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

        /// <summary>
        /// 比较是否相同
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>是否相同</returns>
        public bool equal(SecurityLatestData data)
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

    /// <summary>
    /// 请求价格信息
    /// </summary>
    public class PriceDataRequestInfo
    {
        /// <summary>
        /// 代码
        /// </summary>
        public String[] m_codes;

        /// <summary>
        /// 请求ID
        /// </summary>
        public int m_requestID;

        /// <summary>
        /// 套接字ID
        /// </summary>
        public int m_socketID;
    }

    /// <summary>
    /// 股票实时数据
    /// </summary>
    public class PriceData
    {
        /// <summary>
        /// 成交额
        /// </summary>
        public double m_amount;

        /// <summary>
        /// 当前价格
        /// </summary>
        public double m_close;

        /// <summary>
        /// 股票代码
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// 日期及时间
        /// </summary>
        public double m_date;

        /// <summary>
        /// 最高价
        /// </summary>
        public double m_high;

        /// <summary>
        /// 内盘成交量
        /// </summary>
        public int m_innerVol;

        /// <summary>
        /// 昨日收盘价
        /// </summary>
        public double m_lastClose;

        /// <summary>
        /// 最低价
        /// </summary>
        public double m_low;

        /// <summary>
        /// 开盘价
        /// </summary>
        public double m_open;

        /// <summary>
        /// 期货持仓量
        /// </summary>
        public double m_openInterest;

        /// <summary>
        /// 外盘成交量
        /// </summary>
        public int m_outerVol;

        /// <summary>
        /// 期货结算价
        /// </summary>
        public double m_settlePrice;

        /// <summary>
        /// 换手率
        /// </summary>
        public double m_turnoverRate;

        /// <summary>
        /// 成交量
        /// </summary>
        public double m_volume;

        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="data">数据</param>
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

        /// <summary>
        /// 比较是否相同
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>是否相同</returns>
        public bool equal(SecurityLatestData data)
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

    /// <summary>
    /// 复权类型
    /// </summary>
    public enum DivideRightType
    {
        ZengFa = 1,
        PeiGu = 2,
        PaiXi = 3,
        GengMing = 4,
        SongGu = 5,
        ZhuanZeng = 6,
        BingGu = 7,
        ChaiGu = 8,
        Jili = 9
    }

    /// <summary>
    /// 复权基础数据
    /// </summary>
    public class OneDivideRightBase
    {
        public float DataValue;
        public double Date;
        public DivideRightType DivideType;
        public float Factor;
        public float PClose;

        public OneDivideRightBase()
        {
        }
    }

    /// <summary>
    /// 复权状态
    /// </summary>
    public enum IsDivideRightType
    {
        /// <summary>
        /// 不复权
        /// </summary>
        Non = 1,
        /// <summary>
        /// 前复权
        /// </summary>
        Forward = 2,
        /// <summary>
        /// 后复权
        /// </summary>
        Backward = 3,
    }
}
