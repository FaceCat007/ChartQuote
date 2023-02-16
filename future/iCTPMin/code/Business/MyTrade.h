/*
 * iCTPMin期货行情交易(开源)
 * 上海卷卷猫信息技术有限公司
 */

#ifndef __MYTRADE_H__
#define __MYTRADE_H__
#pragma once
#include "..\CTP\MT.h"

class MT;
//我的交易
class MyTrade
	: public CTPListener
{
public:
	//交易对象
	MT *m_pTrade;
public:
	//构造函数
	MyTrade();
	//析构函数
	virtual ~MyTrade();
public:
	//启动服务
	void Run(char *appID, char *authCode, char *mdServer, char *tdServer, char *brokerID, char *investorID, char *password);
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

#endif