/*
 * iCTPMin期货行情交易(开源)
 * 上海卷卷猫信息技术有限公司
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
	//用户保证金费率回调
	virtual void OnCommissionRatesCallBack(vector<CThostFtdcInstrumentCommissionRateField> *pInstrumentCommissionRates);
	//深度数据回调
	virtual void OnDepthMarketDatasCallBack(CThostFtdcDepthMarketDataField *pDepthMarketData);
	//合约列表回调
	virtual void OnInstrumentsCallBack(vector<CThostFtdcInstrumentField> *pInstruments);
	//持仓数据回调
	virtual void OnInvestorPositionsCallBack(vector<CThostFtdcInvestorPositionField> *pInvestorPosition);
	//持仓组合数据回调
	virtual void OnInvestorPositionCombineDetailsCallBack(vector<CThostFtdcInvestorPositionCombineDetailField> *pInvestorPositionCombineDetails);
	//持仓明细数据回调
	virtual void OnInvestorPositionDetailsCallBack(vector<CThostFtdcInvestorPositionDetailField> *pInvestorPositionDetails);
	//日志回调
	virtual void OnLogCallBack(const String& time, const String& log);
	//用户保证金率回调
	virtual void OnMarginRatesCallBack(vector<CThostFtdcInstrumentMarginRateField> *pInstrumentMarginRates);
	//委托信息回调
	virtual void OnOrderInfoCallBack(CThostFtdcOrderField *pOrder);
	//委托列表回调
	virtual void OnOrderInfosCallBack(vector<CThostFtdcOrderField> *pOrders);
	//状态改变消息
	virtual void OnStateCallBack();
	//用户账户回调
	virtual void OnTradeAccountCallBack(CThostFtdcTradingAccountField *pTradingAccount);
	//成交记录回调
	virtual void OnTradeRecordCallBack(CThostFtdcTradeField *pTradeInfo);
	//成交记录列表回调
	virtual void OnTradeRecordsCallBack(vector<CThostFtdcTradeField> *pTradeInfos);
};

//查询数据结构
class CTPQuery{
public:
	CTPQuery(int id, int reqID, void *args, const string &code = "");
	virtual ~CTPQuery();
	int m_id;
	void *m_args;
	string m_code;
	int m_reqID;
};

//开仓参数类
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

//平仓参数类
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

//撤单参数类
class CTPCancelOrderQuery{
public:
	CTPCancelOrderQuery(const string &exchangeID, const string &orderSysID, int m_reqID, const string &orderRef);
	string m_exchangeID;
	string m_orderRef;
	string m_orderSysID;
	int m_reqID;
};

//注册行情类
class CTPSubMarketDataQuery{
public:
	CTPSubMarketDataQuery();
	vector<string> m_codes;
};

//保证金类型
enum CTPMarginType
{
	CTPMarginType_PreSettlementPrice = '1',//THOST_FTDC_MPT_PreSettlementPrice 昨结算
	CTPMarginType_SettlementPrice = '2',//THOST_FTDC_MPT_SettlementPrice 最新价
	CTPMarginType_AveragePrice = '3',//THOST_FTDC_MPT_AveragePrice 成交均价
	CTPMarginType_OpenPrice = '4',//THOST_FTDC_MPT_OpenPrice 开仓价

};

//盈亏计算类型
enum CTPProfitType
{
	CTPProfitType_All = '1',//THOST_FTDC_AG_All 浮盈浮亏都计算
	CTPProfitType_OnlyLost = '2',//THOST_FTDC_AG_OnlyLost 浮盈不计，浮亏计
	CTPProfitType_OnlyGain = '3',//THOST_FTDC_AG_OnlyGain 浮盈计，浮亏不计
	CTPProfitType_None = '4',//THOST_FTDC_AG_None 浮盈浮亏都不计算
};

//CTP字符串转换
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

//服务器信息
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

//CTP操作类
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
	//程序ID
	string GetAppID();
	void SetAppID(const string &appID);
	//用户ID
	string GetAuthCode();
	void SetAuthCode(const string &authCode);
	//机构代码
	string GetBrokerID();
	void SetBrokerID(const string &brokerID);
	//是否结算
	bool IsClearanced();
	void SetClearanced(bool clearanced);
	//是否连接
	bool IsConnected();
	void SetConnected(bool connected);
	//获取投资者帐号
	string GetInvestorID();
	void SetInvestorID(const string &investorID);
	//监听
	CTPListener* GetListener();
	void SetListener(CTPListener *listener);
	//行情对象
	MD* GetMD();
	void SetMD(MD *md);
	//行情是否已登录
	bool IsMdLogined();
	void SetMdLogined(bool mdIsLogined);
	//行情是否在运行
	bool IsMdRunning();
	void SetMdRunning(bool mdIsRunning);
	//行情服务器
	vector<string> GetMdServers();
	void AddMdServer(const string &mdServer);
	//密码
	string GetPassword();
	void SetPassword(const string &password);
	//会话ID
	int GetSessionID();
	void SetSessionID(int sessionID);
	//断线重连后,需重新订阅所有行情
	void ReSubMarketData(int reqID);
	//交易对象
	TD* GetTD();
	void SetTD(TD *td);
	//交易是否已登录
	bool IsTdLogined();
	void SetTdLogined(bool tdIsLogined);
	//交易是否在运行
	bool IsTdRunning();
	void SetTdRunning(bool tdIsRunning);
	//交易服务器
	vector<string> GetTdServers();
	void AddTdServer(const string &tdServer);
public:
	//增加订阅的合约代码
	bool AddInsID(const String &code);
	//卖平
	int AskClose(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef);
	//卖平今
	int AskCloseToday(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef);
	//卖开
	int AskOpen(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef);
	//买平
	int BidClose(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef);
	//买平今
	int BidCloseToday(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef);
	//买开
	int BidOpen(const String& code, const String& exchangeID, double  price, int qty, char timeCondition, int reqID, const String &orderRef);
	//撤单
	int CancelOrder(const String& exchangeID, const String& orderSysID, int reqID, const String &orderRef);
	//获取程序路径
	static string GetProgramDir();
	//获取最新的查询ID
	int GetNewQuery(CTPQuery *ctpQuery);
	//获取最新的查询ID2
	int GetNewQuery2(CTPQuery *ctpQuery);
	//获取交易日
	String GetTradingDate();
	//获取交易时间
	String GetTradingTime();
	//是否结算时间
	static bool IsClearanceTime();
	//是否交易时间
	static bool IsTradingTime();
	//是否允许交易时间
	static bool IsTradingTimeExact(String code);
	//请求资金保证金费率
	void ReqCommissionRate(const String &wCode, int reqID);
	//查询结算
	void ReqConfirmSettlementInfo(int reqID);
	//请求合约信息
	void ReqInstrumentInfo(int reqID);
	//请求持仓
	void ReqQryInvestorPosition(int reqID);
	//请求持仓明细
	void ReqQryInvestorPositionDetail(int reqID);
	//请求资金保证金率
	void ReqMarginRate(const String &wCode, int reqID);
	//请求委托信息
	void RspQryOrderInfo(int reqID);
	//请求资金账户
	void ReqQryTradingAccount(int reqID);	
	//请求交易信息
	void ReqQryTradeInfo(int reqID);
	//启动
	void Start();
	//启动服务
	void Start(const string &appID, const string &authCode, const string &mdServer, const string &tdServer, const string &brokerID, const string &investorID, const string &password);
	//注册单个合约的行情
	void SubMarketData(const String& code, int reqID);
	//注册多个合约的行情
	void SubMarketDatas(const vector<String> *codes, int reqID);
	//取消注册单个合约的行情
	void UnSubMarketData(const String& code, int reqID);
	//取消注册多个合约的行情
	void UnSubMarketDatas(vector<String> *codes, int reqID);
};

#endif