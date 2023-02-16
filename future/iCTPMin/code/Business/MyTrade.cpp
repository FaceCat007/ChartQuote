#include "..\\stdafx.h"
#include "MyTrade.h"
#include "..\\CFile.h"

typedef void (__stdcall *FutureLatestDataCallBack)(CThostFtdcDepthMarketDataField *field);

FutureLatestDataCallBack m_futureLatestDataCallBack = 0;

String m_strCodes;

static void __stdcall callBackFunc(CThostFtdcDepthMarketDataField *field){

}

MyTrade::MyTrade()
{
	m_pTrade = new MT();
	m_pTrade->SetListener(this);
	//m_futureLatestDataCallBack = &callBackFunc;
}

MyTrade::~MyTrade()
{
	if (m_pTrade)
	{
		delete m_pTrade;
	}
	m_pTrade = NULL;
}

void MyTrade::Run(char *appID, char *authCode, char *mdServer, char *tdServer, char *brokerID, char *investorID, char *password)
{
	if (m_pTrade)
	{
		m_pTrade->Start(appID, authCode, mdServer, tdServer, brokerID, investorID, password);
		m_pTrade->ReqInstrumentInfo(0);
		//m_pTrade->SubMarketData(L"cu2103", 0);
	}
}

void MyTrade::OnCommissionRatesCallBack(vector<CThostFtdcInstrumentCommissionRateField> *pInstrumentCommissionRates)
{
}

void MyTrade::OnDepthMarketDatasCallBack(CThostFtdcDepthMarketDataField *pDepthMarketData)
{
	//printf("%s\r\n", pDepthMarketData->InstrumentID);
	if(m_futureLatestDataCallBack){
		m_futureLatestDataCallBack(pDepthMarketData);
	}
}

void MyTrade::OnInstrumentsCallBack(vector<CThostFtdcInstrumentField> *pInstruments)
{
	vector<String> codes;
	for(int i = 0; i < pInstruments->size(); i++){
		CThostFtdcInstrumentField field = (*pInstruments)[i];
		codes.push_back(CStrA::stringTowstring(field.InstrumentID));
		m_strCodes += CStrA::stringTowstring(field.InstrumentID) + L"," + CStrA::stringTowstring(field.InstrumentName)
			+ L"," + CStrA::stringTowstring(field.ExchangeID) + L"," + CStrA::stringTowstring(field.ProductID) + L"\r\n";
	}
	m_pTrade->SubMarketDatas(&codes, 0);
}

void MyTrade::OnInvestorPositionsCallBack(vector<CThostFtdcInvestorPositionField> *pInvestorPosition)
{
}

void MyTrade::OnInvestorPositionCombineDetailsCallBack(vector<CThostFtdcInvestorPositionCombineDetailField> *pInvestorPositionCombineDetails)
{
}

void MyTrade::OnInvestorPositionDetailsCallBack(vector<CThostFtdcInvestorPositionDetailField> *pInvestorPositionDetails)
{
}

void MyTrade::OnLogCallBack(const String& time, const String& log)
{
}

void MyTrade::OnMarginRatesCallBack(vector<CThostFtdcInstrumentMarginRateField> *pInstrumentMarginRates)
{
}

void MyTrade::OnOrderInfoCallBack(CThostFtdcOrderField *pOrder)
{
}

void MyTrade::OnOrderInfosCallBack(vector<CThostFtdcOrderField> *pOrders)
{
}

void MyTrade::OnStateCallBack()
{
}

void MyTrade::OnTradeAccountCallBack(CThostFtdcTradingAccountField *pTradingAccount)
{
}

void MyTrade::OnTradeRecordCallBack(CThostFtdcTradeField *pTradeInfo)
{
}

void MyTrade::OnTradeRecordsCallBack(vector<CThostFtdcTradeField> *pTradeInfos)
{
}

extern "C" __declspec(dllexport) int getInstruments(char *str){
	if((int)m_strCodes.length() > 0){
		string sStr = CStrA::wstringTostring(m_strCodes);
		strcpy(str, sStr.c_str());
		return 1;
	}else{
		return 0;
	}
}

extern "C" __declspec(dllexport) int addFutureLatestDataCallBack(FutureLatestDataCallBack callBack, char *appID, char *authCode, char *mdServer, char *tdServer, char *brokerID, char *investorID, char *password){
	std::locale::global(std::locale(""));
	m_futureLatestDataCallBack = callBack;
	MyTrade *myTrade = new MyTrade;
	myTrade->Run(appID, authCode, mdServer, tdServer, brokerID, investorID, password);
	return 1;
}