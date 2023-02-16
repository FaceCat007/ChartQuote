/*
 * iCTPMin�ڻ����齻��(��Դ)
 * �Ϻ����è��Ϣ�������޹�˾
 */

#ifndef __MYTRADE_H__
#define __MYTRADE_H__
#pragma once
#include "..\CTP\MT.h"

class MT;
//�ҵĽ���
class MyTrade
	: public CTPListener
{
public:
	//���׶���
	MT *m_pTrade;
public:
	//���캯��
	MyTrade();
	//��������
	virtual ~MyTrade();
public:
	//��������
	void Run(char *appID, char *authCode, char *mdServer, char *tdServer, char *brokerID, char *investorID, char *password);
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

#endif