/*
 * iCTPMin�ڻ����齻��(��Դ)
 * �Ϻ����è��Ϣ�������޹�˾
 */

#ifndef __MT_H__
#define __MT_H__
#pragma once
#include "..\\stdafx.h"
#include "MD.h"
#include "TD.h"

class MD;
class TD;

class CTPListener
{
public:
	//�û���֤����ʻص�
	virtual void OnCommissionRatesCallBack(vector<CThostFtdcInstrumentCommissionRateField> *pInstrumentCommissionRates);
	//������ݻص�
	virtual void OnDepthMarketDatasCallBack(CThostFtdcDepthMarketDataField *pDepthMarketData);
	//��Լ�б�ص�
	virtual void OnInstrumentsCallBack(vector<CThostFtdcInstrumentField> *pInstruments);
	//�ֲ����ݻص�
	virtual void OnInvestorPositionsCallBack(vector<CThostFtdcInvestorPositionField> *pInvestorPosition);
	//�ֲ�������ݻص�
	virtual void OnInvestorPositionCombineDetailsCallBack(vector<CThostFtdcInvestorPositionCombineDetailField> *pInvestorPositionCombineDetails);
	//�ֲ���ϸ���ݻص�
	virtual void OnInvestorPositionDetailsCallBack(vector<CThostFtdcInvestorPositionDetailField> *pInvestorPositionDetails);
	//��־�ص�
	virtual void OnLogCallBack(const String& time, const String& log);
	//�û���֤���ʻص�
	virtual void OnMarginRatesCallBack(vector<CThostFtdcInstrumentMarginRateField> *pInstrumentMarginRates);
	//ί����Ϣ�ص�
	virtual void OnOrderInfoCallBack(CThostFtdcOrderField *pOrder);
	//ί���б�ص�
	virtual void OnOrderInfosCallBack(vector<CThostFtdcOrderField> *pOrders);
	//״̬�ı���Ϣ
	virtual void OnStateCallBack();
	//�û��˻��ص�
	virtual void OnTradeAccountCallBack(CThostFtdcTradingAccountField *pTradingAccount);
	//�ɽ���¼�ص�
	virtual void OnTradeRecordCallBack(CThostFtdcTradeField *pTradeInfo);
	//�ɽ���¼�б�ص�
	virtual void OnTradeRecordsCallBack(vector<CThostFtdcTradeField> *pTradeInfos);
};

//��ѯ���ݽṹ
class CTPQuery{
public:
	CTPQuery(int id, int reqID, void *args, const string &code = "");
	virtual ~CTPQuery();
	int m_id;
	void *m_args;
	string m_code;
	int m_reqID;
};

//���ֲ�����
class CTPOpenQuery{
public:
	CTPOpenQuery(const string &code, const string& exchangeID, double price, int qty, int type, char timeCondition, int reqID, const string &orderRef);
	string m_code;
	string m_exchangeID;
	string m_orderRef;
	double m_price;
	int m_qty;
	int m_reqID;
	char m_timeCondition;
	int m_type;
};

//ƽ�ֲ�����
class CTPCloseQuery{
public:
	CTPCloseQuery(char closeType, const string &code, const string& exchangeID, double price, int qty, int type, char timeCondition, int reqID, const string &orderRef);
	char m_closeType;
	string m_exchangeID;
	string m_code;
	string m_orderRef;
	double m_price;
	int m_qty;
	int m_reqID;
	int m_type;
	char m_timeCondition;
};

//����������
class CTPCancelOrderQuery{
public:
	CTPCancelOrderQuery(const string &exchangeID, const string &orderSysID, int m_reqID, const string &orderRef);
	string m_exchangeID;
	string m_orderRef;
	string m_orderSysID;
	int m_reqID;
};

//ע��������
class CTPSubMarketDataQuery{
public:
	CTPSubMarketDataQuery();
	vector<string> m_codes;
};

//��֤������
enum CTPMarginType
{
	CTPMarginType_PreSettlementPrice = '1',//THOST_FTDC_MPT_PreSettlementPrice �����
	CTPMarginType_SettlementPrice = '2',//THOST_FTDC_MPT_SettlementPrice ���¼�
	CTPMarginType_AveragePrice = '3',//THOST_FTDC_MPT_AveragePrice �ɽ�����
	CTPMarginType_OpenPrice = '4',//THOST_FTDC_MPT_OpenPrice ���ּ�

};

//ӯ����������
enum CTPProfitType
{
	CTPProfitType_All = '1',//THOST_FTDC_AG_All ��ӯ����������
	CTPProfitType_OnlyLost = '2',//THOST_FTDC_AG_OnlyLost ��ӯ���ƣ�������
	CTPProfitType_OnlyGain = '3',//THOST_FTDC_AG_OnlyGain ��ӯ�ƣ���������
	CTPProfitType_None = '4',//THOST_FTDC_AG_None ��ӯ������������
};

//CTP�ַ���ת��
struct StringPtr
{
	char *m_ptr;
	StringPtr(const string &str)
	{
		int len = (int)str.length();
		m_ptr = new char[len + 1];
		memset(m_ptr, 0, len + 1);
		memcpy_s(m_ptr, len+1, str.c_str(), len);
	}
	~StringPtr()
	{
		if (m_ptr)
		{
			delete []m_ptr;
			m_ptr = 0;
		}
	}
	char* GetPtr()
	{
		return m_ptr;
	}
};

//��������Ϣ
class CTPServerConfig
{
public:
	CTPServerConfig();
	string m_appID;
	string m_authCode;
	string m_brokerID;
	string m_investorID;
	vector<string> m_mtFronts;
	string m_name;
	string m_password;
	vector<string> m_tdFronts;
public:
	void Clear();
};

//CTP������
class MT
{
private:
	vector<String> m_allInsID;
	CLock m_allCommissionIDLock;
	map<String, String> m_allCommissionID;
	CLock m_allMarginIDLock;
	map<String,String> m_allMarginID;
	vector<int> m_allReqIDs;
	bool m_checkingLogined;
	bool m_clearanced;
	map<String, String> m_codesMap;
	bool m_connected;
	CTPListener *m_listener;
	MD *m_md;
	bool m_mdIsLogined;
	bool m_mdIsRunning;
	vector<CTPQuery> m_querys;
	vector<CTPQuery> m_querys2;
	map<int, string> m_reqID2codes;
	int m_sessionID;
	CTPServerConfig m_serverConfig;
	TD *m_td;
	bool m_tdIsLogined;
	bool m_tdIsRunning;
private:
	static DWORD WINAPI CheckLogined(LPVOID lpParam);
	static DWORD WINAPI CheckMT(LPVOID lpParam);
	static DWORD WINAPI CheckTradingTime(LPVOID lpParam);
	static DWORD WINAPI LoadMD(LPVOID lpParam);
	static DWORD WINAPI LoadTD(LPVOID lpParam);
	static DWORD WINAPI QueryData(LPVOID lpParam);
	int tradeHandler(const CTPQuery &ctpQuery);
protected:
	map<int, vector<void*>*> m_events;
	map<int, vector<void*>*> m_invokes;
public:
	HANDLE m_mdEvent;
	HANDLE m_tdEvent; 
private:
	CLock m_Lock;
public:
	MT();
	virtual ~MT();
	//����ID
	string GetAppID();
	void SetAppID(const string &appID);
	//�û�ID
	string GetAuthCode();
	void SetAuthCode(const string &authCode);
	//��������
	string GetBrokerID();
	void SetBrokerID(const string &brokerID);
	//�Ƿ����
	bool IsClearanced();
	void SetClearanced(bool clearanced);
	//�Ƿ�����
	bool IsConnected();
	void SetConnected(bool connected);
	//��ȡͶ�����ʺ�
	string GetInvestorID();
	void SetInvestorID(const string &investorID);
	//����
	CTPListener* GetListener();
	void SetListener(CTPListener *listener);
	//�������
	MD* GetMD();
	void SetMD(MD *md);
	//�����Ƿ��ѵ�¼
	bool IsMdLogined();
	void SetMdLogined(bool mdIsLogined);
	//�����Ƿ�������
	bool IsMdRunning();
	void SetMdRunning(bool mdIsRunning);
	//���������
	vector<string> GetMdServers();
	void AddMdServer(const string &mdServer);
	//����
	string GetPassword();
	void SetPassword(const string &password);
	//�ỰID
	int GetSessionID();
	void SetSessionID(int sessionID);
	//����������,�����¶�����������
	void ReSubMarketData(int reqID);
	//���׶���
	TD* GetTD();
	void SetTD(TD *td);
	//�����Ƿ��ѵ�¼
	bool IsTdLogined();
	void SetTdLogined(bool tdIsLogined);
	//�����Ƿ�������
	bool IsTdRunning();
	void SetTdRunning(bool tdIsRunning);
	//���׷�����
	vector<string> GetTdServers();
	void AddTdServer(const string &tdServer);
public:
	//���Ӷ��ĵĺ�Լ����
	bool AddInsID(const String &code);
	//��ƽ
	int AskClose(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef);
	//��ƽ��
	int AskCloseToday(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef);
	//����
	int AskOpen(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef);
	//��ƽ
	int BidClose(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef);
	//��ƽ��
	int BidCloseToday(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef);
	//��
	int BidOpen(const String& code, const String& exchangeID, double  price, int qty, char timeCondition, int reqID, const String &orderRef);
	//����
	int CancelOrder(const String& exchangeID, const String& orderSysID, int reqID, const String &orderRef);
	//��ȡ����·��
	static string GetProgramDir();
	//��ȡ���µĲ�ѯID
	int GetNewQuery(CTPQuery *ctpQuery);
	//��ȡ���µĲ�ѯID2
	int GetNewQuery2(CTPQuery *ctpQuery);
	//��ȡ������
	String GetTradingDate();
	//��ȡ����ʱ��
	String GetTradingTime();
	//�Ƿ����ʱ��
	static bool IsClearanceTime();
	//�Ƿ���ʱ��
	static bool IsTradingTime();
	//�Ƿ�������ʱ��
	static bool IsTradingTimeExact(String code);
	//�����ʽ�֤�����
	void ReqCommissionRate(const String &wCode, int reqID);
	//��ѯ����
	void ReqConfirmSettlementInfo(int reqID);
	//�����Լ��Ϣ
	void ReqInstrumentInfo(int reqID);
	//����ֲ�
	void ReqQryInvestorPosition(int reqID);
	//����ֲ���ϸ
	void ReqQryInvestorPositionDetail(int reqID);
	//�����ʽ�֤����
	void ReqMarginRate(const String &wCode, int reqID);
	//����ί����Ϣ
	void RspQryOrderInfo(int reqID);
	//�����ʽ��˻�
	void ReqQryTradingAccount(int reqID);	
	//��������Ϣ
	void ReqQryTradeInfo(int reqID);
	//����
	void Start();
	//��������
	void Start(const string &appID, const string &authCode, const string &mdServer, const string &tdServer, const string &brokerID, const string &investorID, const string &password);
	//ע�ᵥ����Լ������
	void SubMarketData(const String& code, int reqID);
	//ע������Լ������
	void SubMarketDatas(const vector<String> *codes, int reqID);
	//ȡ��ע�ᵥ����Լ������
	void UnSubMarketData(const String& code, int reqID);
	//ȡ��ע������Լ������
	void UnSubMarketDatas(vector<String> *codes, int reqID);
};

#endif