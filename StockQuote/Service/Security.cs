/*
 * iChart����ϵͳ(��Դ)
 * �Ϻ�����è��Ϣ�������޹�˾
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace FaceCat
{
    /// <summary>
    /// ֤ȯ��ʷ����
    /// </summary>
    public class SecurityData2
    {
        /// <summary>
        /// ���̼�
        /// </summary>
        public double m_close;

        /// <summary>
        /// ����
        /// </summary>
        public double m_date;

        /// <summary>
        /// ��߼�
        /// </summary>
        public double m_high;

        /// <summary>
        /// ��ͼ�
        /// </summary>
        public double m_low;

        /// <summary>
        /// ���̼�
        /// </summary>
        public double m_open;

        /// <summary>
        /// �ɽ���
        /// </summary>
        public double m_volume;

        /// <summary>
        /// �ɽ���
        /// </summary>
        public double m_amount;

        /// <summary>
        /// ǰ��Ȩ����
        /// </summary>
        public float m_forwardFactor;

        /// <summary>
        /// ��Ȩ����
        /// </summary>
        public float m_backFactor;

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="data">����</param>
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
    /// ��ʱ�߻���
    /// </summary>
    public class MinuteDatasCache
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// ����
        /// </summary>
        public List<SecurityData2> m_datas = new List<SecurityData2>();

        /// <summary>
        /// �ϴεĳɽ���
        /// </summary>
        public double m_lastAmount;

        /// <summary>
        /// �ϴ�����
        /// </summary>
        public double m_lastDate;

        /// <summary>
        /// �ϴεĳɽ���
        /// </summary>
        public double m_lastVolume;

        /// <summary>
        /// ��������
        /// </summary>
        public SecurityLatestData m_latestData;
    }

    /// <summary>
    /// �ͻ������ݻ���
    /// </summary>
    public class ClientTickDataCache
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// �ϴεĳɽ���
        /// </summary>
        public double m_lastAmount;

        /// <summary>
        /// �ϴ�����
        /// </summary>
        public double m_lastDate;

        /// <summary>
        /// �ϴεĳɽ���
        /// </summary>
        public double m_lastVolume;
    }

    /// <summary>
    /// �������ݵ�����
    /// </summary>
    public class LatestRequestInfo
    {
        /// <summary>
        /// �׽���ID
        /// </summary>
        public int m_socketID;

        /// <summary>
        /// ����ID
        /// </summary>
        public int m_requestID;
    }
    /// <summary>
    /// ��Ʊ��Ϣ
    /// </summary>
    public class Security
    {
        /// <summary>
        /// �������̾���
        /// </summary>
        public Security()
        {
        }

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// ƴ��
        /// </summary>
        public String m_pingyin = "";

        /// <summary>
        /// ״̬
        /// </summary>
        public String m_status;

        /// <summary>
        /// �г�����
        /// </summary>
        public String m_market;
    }

    /// <summary>
    /// ��Ʊʵʱ����
    /// </summary>
    public class SecurityLatestData
    {
        /// <summary>
        /// ����
        /// </summary>
        public String m_name = "";

        /// <summary>
        /// �ɽ���
        /// </summary>
        public double m_amount;

        /// <summary>
        /// ��һ��
        /// </summary>
        public int m_buyVolume1;

        /// <summary>
        /// �����
        /// </summary>
        public int m_buyVolume2;

        /// <summary>
        /// ������
        /// </summary>
        public int m_buyVolume3;

        /// <summary>
        /// ������
        /// </summary>
        public int m_buyVolume4;

        /// <summary>
        /// ������
        /// </summary>
        public int m_buyVolume5;

        /// <summary>
        /// ������
        /// </summary>
        public int m_buyVolume6;

        /// <summary>
        /// ������
        /// </summary>
        public int m_buyVolume7;

        /// <summary>
        /// �����
        /// </summary>
        public int m_buyVolume8;

        /// <summary>
        /// �����
        /// </summary>
        public int m_buyVolume9;

        /// <summary>
        /// ��ʮ��
        /// </summary>
        public int m_buyVolume10;

        /// <summary>
        /// ��һ��
        /// </summary>
        public double m_buyPrice1;

        /// <summary>
        /// �����
        /// </summary>
        public double m_buyPrice2;

        /// <summary>
        /// ������
        /// </summary>
        public double m_buyPrice3;

        /// <summary>
        /// ���ļ�
        /// </summary>
        public double m_buyPrice4;

        /// <summary>
        /// �����
        /// </summary>
        public double m_buyPrice5;

        /// <summary>
        /// ������
        /// </summary>
        public double m_buyPrice6;

        /// <summary>
        /// ���߼�
        /// </summary>
        public double m_buyPrice7;

        /// <summary>
        /// ��˼�
        /// </summary>
        public double m_buyPrice8;

        /// <summary>
        /// ��ż�
        /// </summary>
        public double m_buyPrice9;

        /// <summary>
        /// ��ʮ��
        /// </summary>
        public double m_buyPrice10;

        /// <summary>
        /// ��ǰ�۸�
        /// </summary>
        public double m_close;

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// ���ڼ�ʱ��
        /// </summary>
        public double m_date;

        /// <summary>
        /// ��߼�
        /// </summary>
        public double m_high;

        /// <summary>
        /// ���̳ɽ���
        /// </summary>
        public int m_innerVol;

        /// <summary>
        /// �������̼�
        /// </summary>
        public double m_lastClose;

        /// <summary>
        /// ��ͼ�
        /// </summary>
        public double m_low;

        /// <summary>
        /// ���̼�
        /// </summary>
        public double m_open;

        /// <summary>
        /// �ڻ��ֲ���
        /// </summary>
        public double m_openInterest;

        /// <summary>
        /// ���̳ɽ���
        /// </summary>
        public int m_outerVol;

        /// <summary>
        /// ��һ��
        /// </summary>
        public int m_sellVolume1;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume2;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume3;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume4;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume5;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume6;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume7;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume8;

        /// <summary>
        /// ������
        /// </summary>
        public int m_sellVolume9;

        /// <summary>
        /// ��ʮ��
        /// </summary>
        public int m_sellVolume10;

        /// <summary>
        /// ��һ��
        /// </summary>
        public double m_sellPrice1;

        /// <summary>
        /// ������
        /// </summary>
        public double m_sellPrice2;

        /// <summary>
        /// ������
        /// </summary>
        public double m_sellPrice3;

        /// <summary>
        /// ���ļ�
        /// </summary>
        public double m_sellPrice4;

        /// <summary>
        /// �����
        /// </summary>
        public double m_sellPrice5;

        /// <summary>
        /// ������
        /// </summary>
        public double m_sellPrice6;

        /// <summary>
        /// ���߼�
        /// </summary>
        public double m_sellPrice7;

        /// <summary>
        /// ���˼�
        /// </summary>
        public double m_sellPrice8;

        /// <summary>
        /// ���ż�
        /// </summary>
        public double m_sellPrice9;

        /// <summary>
        /// ��ʮ��
        /// </summary>
        public double m_sellPrice10;

        /// <summary>
        /// �ڻ������
        /// </summary>
        public double m_settlePrice;

        /// <summary>
        /// ������
        /// </summary>
        public double m_turnoverRate;

        /// <summary>
        /// �ɽ���
        /// </summary>
        public double m_volume;

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="data">����</param>
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
        /// �Ƚ��Ƿ���ͬ
        /// </summary>
        /// <param name="data">����</param>
        /// <returns>�Ƿ���ͬ</returns>
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
    /// ����۸���Ϣ
    /// </summary>
    public class PriceDataRequestInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        public String[] m_codes;

        /// <summary>
        /// ����ID
        /// </summary>
        public int m_requestID;

        /// <summary>
        /// �׽���ID
        /// </summary>
        public int m_socketID;
    }

    /// <summary>
    /// ��Ʊʵʱ����
    /// </summary>
    public class PriceData
    {
        /// <summary>
        /// �ɽ���
        /// </summary>
        public double m_amount;

        /// <summary>
        /// ��ǰ�۸�
        /// </summary>
        public double m_close;

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        public String m_code = "";

        /// <summary>
        /// ���ڼ�ʱ��
        /// </summary>
        public double m_date;

        /// <summary>
        /// ��߼�
        /// </summary>
        public double m_high;

        /// <summary>
        /// ���̳ɽ���
        /// </summary>
        public int m_innerVol;

        /// <summary>
        /// �������̼�
        /// </summary>
        public double m_lastClose;

        /// <summary>
        /// ��ͼ�
        /// </summary>
        public double m_low;

        /// <summary>
        /// ���̼�
        /// </summary>
        public double m_open;

        /// <summary>
        /// �ڻ��ֲ���
        /// </summary>
        public double m_openInterest;

        /// <summary>
        /// ���̳ɽ���
        /// </summary>
        public int m_outerVol;

        /// <summary>
        /// �ڻ������
        /// </summary>
        public double m_settlePrice;

        /// <summary>
        /// ������
        /// </summary>
        public double m_turnoverRate;

        /// <summary>
        /// �ɽ���
        /// </summary>
        public double m_volume;

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="data">����</param>
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
        /// �Ƚ��Ƿ���ͬ
        /// </summary>
        /// <param name="data">����</param>
        /// <returns>�Ƿ���ͬ</returns>
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
    /// ��Ȩ����
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
    /// ��Ȩ��������
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
    /// ��Ȩ״̬
    /// </summary>
    public enum IsDivideRightType
    {
        /// <summary>
        /// ����Ȩ
        /// </summary>
        Non = 1,
        /// <summary>
        /// ǰ��Ȩ
        /// </summary>
        Forward = 2,
        /// <summary>
        /// ��Ȩ
        /// </summary>
        Backward = 3,
    }
}