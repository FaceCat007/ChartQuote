using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using FaceCat;

namespace future
{
    ///深度行情
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcDepthMarketDataField
    {
        ///交易日
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=9)]
        public string TradingDay;
        ///合约代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=31)]
        public string InstrumentID;
        ///交易所代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=9)]
        public string ExchangeID;
        ///合约在交易所的代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=31)]
        public string ExchangeInstID;
        ///最新价
        public double LastPrice;
        ///上次结算价
        public double PreSettlementPrice;
        ///昨收盘
        public double PreClosePrice;
        ///昨持仓量
        public double PreOpenInterest;
        ///今开盘
        public double OpenPrice;
        ///最高价
        public double HighestPrice;
        ///最低价
        public double LowestPrice;
        ///数量
        public int Volume;
        ///成交金额
        public double Turnover;
        ///持仓量
        public double OpenInterest;
        ///今收盘
        public double ClosePrice;
        ///本次结算价
        public double SettlementPrice;
        ///涨停板价
        public double UpperLimitPrice;
        ///跌停板价
        public double LowerLimitPrice;
        ///昨虚实度
        public double PreDelta;
        ///今虚实度
        public double CurrDelta;
        ///最后修改时间
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=9)]
        public String UpdateTime;
        ///最后修改毫秒
        public int UpdateMillisec;
        ///申买价一
        public double BidPrice1;
        ///申买量一
        public int BidVolume1;
        ///申卖价一
        public double AskPrice1;
        ///申卖量一
        public int AskVolume1;
        ///申买价二
        public double BidPrice2;
        ///申买量二
        public int BidVolume2;
        ///申卖价二
        public double AskPrice2;
        ///申卖量二
        public int AskVolume2;
        ///申买价三
        public double BidPrice3;
        ///申买量三
        public int BidVolume3;
        ///申卖价三
        public double AskPrice3;
        ///申卖量三
        public int AskVolume3;
        ///申买价四
        public double BidPrice4;
        ///申买量四
        public int BidVolume4;
        ///申卖价四
        public double AskPrice4;
        ///申卖量四
        public int AskVolume4;
        ///申买价五
        public double BidPrice5;
        ///申买量五
        public int BidVolume5;
        ///申卖价五
        public double AskPrice5;
        ///申卖量五
        public int AskVolume5;
        ///当日均价
        public double AveragePrice;
        ///业务日期
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=9)]
        public String ActionDay;
    };

    /// <summary>
    /// 期货数据回调
    /// </summary>
    /// <param name="data"></param>
    public delegate void FutureLatestDataCallBack(CThostFtdcDepthMarketDataField data);

    /// <summary>
    /// 期货API
    /// </summary>
    public class FutureAPI
    {
        [DllImport("iCTPMin.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void addFutureLatestDataCallBack(FutureLatestDataCallBack callBack, String appID, String authCode, String mdServer, String tdServer, String brokerID, String investorID, String password);

        /// <summary>
        /// 获取交易时间
        /// </summary>
        [DllImport("iCTPMin.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int getInstruments(IntPtr data);
        public static int getInstruments(StringBuilder data)
        {
            IntPtr bufferIntPtr = Marshal.AllocHGlobal(1024 * 1024);
            int state = getInstruments(bufferIntPtr);
            String sbResult = Marshal.PtrToStringAnsi(bufferIntPtr);
            data.Append(sbResult);
            Marshal.FreeHGlobal(bufferIntPtr);
            return state;
        }
    }
}
