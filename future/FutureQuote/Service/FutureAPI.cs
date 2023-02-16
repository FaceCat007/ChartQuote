using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using FaceCat;

namespace future
{
    ///�������
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcDepthMarketDataField
    {
        ///������
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=9)]
        public string TradingDay;
        ///��Լ����
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=31)]
        public string InstrumentID;
        ///����������
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=9)]
        public string ExchangeID;
        ///��Լ�ڽ������Ĵ���
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=31)]
        public string ExchangeInstID;
        ///���¼�
        public double LastPrice;
        ///�ϴν����
        public double PreSettlementPrice;
        ///������
        public double PreClosePrice;
        ///��ֲ���
        public double PreOpenInterest;
        ///����
        public double OpenPrice;
        ///��߼�
        public double HighestPrice;
        ///��ͼ�
        public double LowestPrice;
        ///����
        public int Volume;
        ///�ɽ����
        public double Turnover;
        ///�ֲ���
        public double OpenInterest;
        ///������
        public double ClosePrice;
        ///���ν����
        public double SettlementPrice;
        ///��ͣ���
        public double UpperLimitPrice;
        ///��ͣ���
        public double LowerLimitPrice;
        ///����ʵ��
        public double PreDelta;
        ///����ʵ��
        public double CurrDelta;
        ///����޸�ʱ��
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=9)]
        public String UpdateTime;
        ///����޸ĺ���
        public int UpdateMillisec;
        ///�����һ
        public double BidPrice1;
        ///������һ
        public int BidVolume1;
        ///������һ
        public double AskPrice1;
        ///������һ
        public int AskVolume1;
        ///����۶�
        public double BidPrice2;
        ///��������
        public int BidVolume2;
        ///�����۶�
        public double AskPrice2;
        ///��������
        public int AskVolume2;
        ///�������
        public double BidPrice3;
        ///��������
        public int BidVolume3;
        ///��������
        public double AskPrice3;
        ///��������
        public int AskVolume3;
        ///�������
        public double BidPrice4;
        ///��������
        public int BidVolume4;
        ///��������
        public double AskPrice4;
        ///��������
        public int AskVolume4;
        ///�������
        public double BidPrice5;
        ///��������
        public int BidVolume5;
        ///��������
        public double AskPrice5;
        ///��������
        public int AskVolume5;
        ///���վ���
        public double AveragePrice;
        ///ҵ������
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=9)]
        public String ActionDay;
    };

    /// <summary>
    /// �ڻ����ݻص�
    /// </summary>
    /// <param name="data"></param>
    public delegate void FutureLatestDataCallBack(CThostFtdcDepthMarketDataField data);

    /// <summary>
    /// �ڻ�API
    /// </summary>
    public class FutureAPI
    {
        [DllImport("iCTPMin.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void addFutureLatestDataCallBack(FutureLatestDataCallBack callBack, String appID, String authCode, String mdServer, String tdServer, String brokerID, String investorID, String password);

        /// <summary>
        /// ��ȡ����ʱ��
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
