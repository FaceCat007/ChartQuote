/*
 * iCTPMin�ڻ����齻��(��Դ)
 * �Ϻ����è��Ϣ�������޹�˾
 */

#include "..\\stdafx.h"
#include "TD.h"

String TD::GetTradingData()
{
	string tradingDate = m_pTradeUserApi->GetTradingDay();
	String wDate = CStrA::stringTowstring(tradingDate);
	return wDate;
}

void TD::ReqCommissionRate(const string &code, int requestID)
{
	m_pInstrumentCommissionRates.clear();
	m_commisionRateReqID = requestID;
	CThostFtdcQryInstrumentCommissionRateField req;
	memset(&req, 0, sizeof(req));
	strcpy_s(req.BrokerID, m_mt->GetBrokerID().c_str()); 
	strcpy_s(req.InvestorID, m_mt->GetInvestorID().c_str()); 
	if((int)code.length() > 0)
	{
		strcpy_s(req.InstrumentID, code.c_str()); 
	}
	m_pTradeUserApi->ReqQryInstrumentCommissionRate(&req, requestID);
}

void TD::ReqConfirmSettlementInfo(int requestID)
{
	CThostFtdcSettlementInfoConfirmField settlementInfoConfirm;
	memset(&settlementInfoConfirm, 0, sizeof(CThostFtdcSettlementInfoConfirmField));
	strcpy_s(settlementInfoConfirm.BrokerID, m_mt->GetBrokerID().c_str()); 
	strcpy_s(settlementInfoConfirm.InvestorID, m_mt->GetInvestorID().c_str()); 
	m_pTradeUserApi->ReqSettlementInfoConfirm(&settlementInfoConfirm, requestID);
}

void TD::ReqInstrumentInfo(int requestID)
{
	m_pInstruments.clear();
	m_instrumentsReqID = requestID;
	CThostFtdcQryInstrumentField instrumentField;
	memset(&instrumentField, 0, sizeof(CThostFtdcQryInstrumentField));
	m_pTradeUserApi->ReqQryInstrument(&instrumentField, m_instrumentsReqID);
}

void TD::ReqQryConfirmSettlementInfo(int requestID)
{
	CThostFtdcQrySettlementInfoConfirmField qrySettlementInfoConfirm;
	memset(&qrySettlementInfoConfirm, 0, sizeof(CThostFtdcQrySettlementInfoConfirmField));
	strcpy_s(qrySettlementInfoConfirm.BrokerID, m_mt->GetBrokerID().c_str()); 
	strcpy_s(qrySettlementInfoConfirm.InvestorID, m_mt->GetInvestorID().c_str()); 
	m_pTradeUserApi->ReqQrySettlementInfoConfirm(&qrySettlementInfoConfirm, requestID);
}

void TD::ReqQryInvestorPosition(int requestID)
{
	m_pInvestorPosition.clear();
	m_investorPositionReqID = requestID;
	CThostFtdcQryInvestorPositionField qryInvestorPosition;
	memset(&qryInvestorPosition, 0, sizeof(CThostFtdcQryInvestorPositionField));
	strcpy_s(qryInvestorPosition.BrokerID, m_mt->GetBrokerID().c_str()); 
	strcpy_s(qryInvestorPosition.InvestorID, m_mt->GetInvestorID().c_str()); 
	m_pTradeUserApi->ReqQryInvestorPosition(&qryInvestorPosition, m_investorPositionReqID);
}

void TD::ReqQryInvestorPositionCombineDetail(int requestID)
{
	m_pInvestorPositionCombineDetails.clear();
	m_investorPositionCombineDetailReqID = requestID;
	CThostFtdcQryInvestorPositionCombineDetailField qryInvestorPositionCombineDetail;
	memset(&qryInvestorPositionCombineDetail, 0, sizeof(CThostFtdcQryInvestorPositionCombineDetailField));
	strcpy_s(qryInvestorPositionCombineDetail.BrokerID, m_mt->GetBrokerID().c_str()); 
	strcpy_s(qryInvestorPositionCombineDetail.InvestorID, m_mt->GetInvestorID().c_str()); 
	m_pTradeUserApi->ReqQryInvestorPositionCombineDetail(&qryInvestorPositionCombineDetail, m_investorPositionCombineDetailReqID);
}

void TD::ReqQryInvestorPositionDetail(int requestID)
{
	m_pInvestorPositionDetails.clear();
	m_investorPositionDetailReqID = requestID;
	CThostFtdcQryInvestorPositionDetailField qryInvestorPositionDetail;
	memset(&qryInvestorPositionDetail, 0, sizeof(CThostFtdcQryInvestorPositionDetailField));
	strcpy_s(qryInvestorPositionDetail.BrokerID, m_mt->GetBrokerID().c_str()); 
	strcpy_s(qryInvestorPositionDetail.InvestorID, m_mt->GetInvestorID().c_str()); 
	m_pTradeUserApi->ReqQryInvestorPositionDetail(&qryInvestorPositionDetail, m_investorPositionDetailReqID);
}

void TD::ReqMarginRate(const string &code, int requestID)
{
	m_pInstrumentMarginRates.clear();
	m_marginRateReqID = requestID;
	CThostFtdcQryInstrumentMarginRateField req;
	memset(&req, 0, sizeof(req));
	strcpy_s(req.BrokerID, m_mt->GetBrokerID().c_str()); 
	strcpy_s(req.InvestorID, m_mt->GetInvestorID().c_str()); 
	req.HedgeFlag = '1';//@todo ��д��ΪͶ����
	if((int)code.length() > 0)
	{
		strcpy_s(req.InstrumentID, code.c_str());
	}
	m_pTradeUserApi->ReqQryInstrumentMarginRate(&req, requestID);
}

void TD::ReqQryOrderInfo(int requestID)
{
	m_pOrders.clear();
	m_orderReqID = requestID;
	CThostFtdcQryOrderField qryOrder;
	memset(&qryOrder, 0, sizeof(CThostFtdcQryOrderField));
	strcpy_s(qryOrder.BrokerID, m_mt->GetBrokerID().c_str()); 
	strcpy_s(qryOrder.InvestorID, m_mt->GetInvestorID().c_str()); 
	m_pTradeUserApi->ReqQryOrder(&qryOrder, m_orderReqID);
}

void TD::ReqQryTradingAccount(int requestID)
{
	CThostFtdcQryTradingAccountField qryTradingAccount;
	memset(&qryTradingAccount, 0, sizeof(CThostFtdcQryTradingAccountField));
	strcpy_s(qryTradingAccount.BrokerID, m_mt->GetBrokerID().c_str()); 
	strcpy_s(qryTradingAccount.InvestorID, m_mt->GetInvestorID().c_str()); 
	m_pTradeUserApi->ReqQryTradingAccount(&qryTradingAccount, requestID);
}

void TD::ReqQryTradeInfo(int requestID)
{
	m_pTradeInfos.clear();
	m_tradeReqID = requestID;
	CThostFtdcQryTradeField qryTrade;
	memset(&qryTrade, 0, sizeof(CThostFtdcQryTradeField));
	strcpy_s(qryTrade.BrokerID, m_mt->GetBrokerID().c_str()); 
	strcpy_s(qryTrade.InvestorID, m_mt->GetInvestorID().c_str()); 
	m_pTradeUserApi->ReqQryTrade(&qryTrade, m_tradeReqID);
}

int TD::ReqAuthenticate(int requestID)
{
	CThostFtdcReqAuthenticateField req;
	memset(&req, 0, sizeof(req));
	strcpy_s(req.BrokerID, m_mt->GetBrokerID().c_str());
	strcpy_s(req.AppID, m_mt->GetAppID().c_str());
	strcpy(req.AuthCode, m_mt->GetAuthCode().c_str());
	strcpy(req.UserID, m_mt->GetInvestorID().c_str());
	m_pTradeUserApi->ReqAuthenticate(&req, requestID);
	return 1;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

int TD::order(const string &code, const string& exchangeID, char direction, char offsetFlag, double  price, int volume, char timeCondition, int requestID, const string &orderRef)
{
	//::MessageBox(0, L"��ʼ�µ�", L"��ʾ", 0);
	printf("����ί�У���Լ%s, ����%c, ��ƽ%c, �۸�%f, ����%d\n", code.c_str(), direction, offsetFlag, price, volume);
	CThostFtdcInputOrderField req; 
	memset(&req, 0, sizeof(req)); 
	strcpy_s(req.BrokerID, m_mt->GetBrokerID().c_str());
	strcpy_s(req.InvestorID, m_mt->GetInvestorID().c_str()); 
	strcpy_s(req.InstrumentID, code.c_str()); 
	strcpy_s(req.ExchangeID, exchangeID.c_str()); 
	memset(req.OrderRef, '\0', 13);
	if((int)orderRef.length() == 0)
	{
		string maxOrderRef = GetMaxOrderRef();
		memcpy(req.OrderRef, maxOrderRef.c_str(), sizeof(req.OrderRef) - 1);
	}
	else
	{
		memcpy(req.OrderRef, orderRef.c_str(), sizeof(req.OrderRef) - 1);
	}
	strcpy_s(req.UserID, m_mt->GetInvestorID().c_str());
	req.Direction = direction;
	req.CombOffsetFlag[0] = offsetFlag;
	req.CombHedgeFlag[0] = '1';
	if (price > 0)
	{
		req.OrderPriceType = THOST_FTDC_OPT_LimitPrice;
		req.LimitPrice = price; 
		req.TimeCondition = timeCondition;
	}
	else
	{
		req.OrderPriceType = THOST_FTDC_OPT_AnyPrice;
		req.LimitPrice = 0; 
		req.TimeCondition = timeCondition;
		//req.TimeCondition = THOST_FTDC_TC_IOC;
	}
	req.VolumeTotalOriginal = volume;
	req.VolumeCondition = THOST_FTDC_VC_AV; 
	req.ContingentCondition = THOST_FTDC_CC_Immediately; 
	req.ForceCloseReason = THOST_FTDC_FCC_NotForceClose;
	req.IsAutoSuspend = 0;
	req.RequestID = requestID;
	QueryPerformanceCounter((LARGE_INTEGER *)&m_t1);
	int res = m_pTradeUserApi->ReqOrderInsert(&req, requestID);
	//::MessageBox(0, L"�µ�����", L"��ʾ", 0);
	if(res != 0)
	{
		//String msg = CStrA::ConvertIntToStr(res);
		//LPCWSTR wmsg = msg.c_str();
		//::MessageBox(0, wmsg, L"��ʾ", 0);
	}
	return res;
}

int TD::cancelOrder(const char *ExchangeID, const char *OrderSysID, int requestID, const string &orderRef)
{
	CThostFtdcInputOrderActionField req;
	memset(&req, 0, sizeof(req)); 
	strcpy_s(req.BrokerID, m_mt->GetBrokerID().c_str());
	strcpy_s(req.InvestorID, m_mt->GetInvestorID().c_str()); 
	strcpy_s(req.ExchangeID, ExchangeID);
	memset(req.OrderRef, '\0', 13);
	if((int)orderRef.length() == 0)
	{
		string maxOrderRef = GetMaxOrderRef();
		memcpy(req.OrderRef, maxOrderRef.c_str(), sizeof(req.OrderRef) - 1);
	}
	else
	{
		memcpy(req.OrderRef, orderRef.c_str(), sizeof(req.OrderRef) - 1);
	}
	strcpy_s(req.OrderSysID, OrderSysID);
	req.ActionFlag = THOST_FTDC_AF_Delete;
	strcpy_s(req.UserID, m_mt->GetInvestorID().c_str()); 
	//��������ԭ���µ���requestID,��������
	QueryPerformanceCounter((LARGE_INTEGER*)&m_t1);//��¼ʱ��t1
	return m_pTradeUserApi->ReqOrderAction(&req, requestID);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

TD::~TD()
{
	m_pInstruments.clear();
	m_pInstrumentCommissionRates.clear();
	m_pInvestorPosition.clear();
	m_pInvestorPositionCombineDetails.clear();
	m_pInvestorPositionDetails.clear();
	m_pInstrumentMarginRates.clear();
	m_pOrders.clear();
	m_pTradeInfos.clear();
	m_mt = 0;
	m_pTradeUserApi = 0;
}

string TD::GetMaxOrderRef()
{
	m_maxOrderRef++;
	char strOrder[6];
	sprintf_s(strOrder, 5, "%d", m_maxOrderRef);
	string order = strOrder;
	int len = (int)order.length();
	for(int i = 0; i < 5 - len; i++)
	{
		order = "0" + order;
	}
	return order;
}

MT* TD::GetMT()
{
	return m_mt;
}

void TD::SetMT(MT* mt)
{
	m_mt = mt;
}

int TD::GetRequestID()
{
	return ++m_requestID;
}

CThostFtdcTraderApi* TD::GetUserApi()
{
	return m_pTradeUserApi;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///���ͻ����뽻�׺�̨������ͨ������ʱ����δ��¼ǰ�����÷��������á�
void TD::OnFrontConnected()
{
	ReqAuthenticate(GetRequestID());
}

///���ͻ����뽻�׺�̨ͨ�����ӶϿ�ʱ���÷��������á���������������API���Զ��������ӣ��ͻ��˿ɲ�������
///@param nReason ����ԭ��
///        0x1001 �����ʧ��
///        0x1002 ����дʧ��
///        0x2001 ����������ʱ
///        0x2002 ��������ʧ��
///        0x2003 �յ�������
void TD::OnFrontDisconnected(int nReason)
{
	m_mt->SetTdLogined(false);
}
///������ʱ���档����ʱ��δ�յ�����ʱ���÷��������á�
///@param nTimeLapse �����ϴν��ձ��ĵ�ʱ��
void TD::OnHeartBeatWarning(int nTimeLapse)
{
	//API����FAQ����Զ���ᷢ��
}

///�ͻ�����֤��Ӧ
void TD::OnRspAuthenticate(CThostFtdcRspAuthenticateField *pRspAuthenticateField, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	CThostFtdcReqUserLoginField reqUserLogin; 
	strcpy_s(reqUserLogin.BrokerID, m_mt->GetBrokerID().c_str());
	strcpy_s(reqUserLogin.UserID, m_mt->GetInvestorID().c_str()); 
	strcpy_s(reqUserLogin.Password, m_mt->GetPassword().c_str());
	// ������½����(int nRequestID��һ���Ự�в����ظ����û�����)
	m_pTradeUserApi->ReqUserLogin(&reqUserLogin, GetRequestID());
}

///��¼������Ӧ
void TD::OnRspUserLogin(CThostFtdcRspUserLoginField *pRspUserLogin, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	CoutMP::Print() << "Login TD Success!" << CoutMP::ENDL;
	m_mt->SetSessionID(pRspUserLogin->SessionID);
	//��ѯ���к�Լ
	ReqConfirmSettlementInfo(0);
	ReqInstrumentInfo(0);
}

///�ǳ�������Ӧ
void TD::OnRspUserLogout(CThostFtdcUserLogoutField *pUserLogout, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	m_mt->SetTdLogined(false);
}

///�û��������������Ӧ
void TD::OnRspUserPasswordUpdate(CThostFtdcUserPasswordUpdateField *pUserPasswordUpdate, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�ʽ��˻��������������Ӧ
void TD::OnRspTradingAccountPasswordUpdate(CThostFtdcTradingAccountPasswordUpdateField *pTradingAccountPasswordUpdate, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///����¼��������Ӧ
void TD::OnRspOrderInsert(CThostFtdcInputOrderField *pInputOrder, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	int a = 0;
	if(pRspInfo && pRspInfo->ErrorID != 0)
	{
		//Thost �յ�����ָ����û��ͨ������У�飬�ܾ����ܱ���ָ��û��ͻ��յ�OnRspOrderInsert��Ϣ
		if(pRspInfo->ErrorID == 31)//�ʽ���
		{
		}
	}
}

///Ԥ��¼��������Ӧ
void TD::OnRspParkedOrderInsert(CThostFtdcParkedOrderField *pParkedOrder, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///Ԥ�񳷵�¼��������Ӧ
void TD::OnRspParkedOrderAction(CThostFtdcParkedOrderActionField *pParkedOrderAction, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///��������������Ӧ
void TD::OnRspOrderAction(CThostFtdcInputOrderActionField *pInputOrderAction, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	//Thost �յ�����ָ����û��ͨ������У�飬�ܾ����ܳ���ָ��û��ͻ��յ�OnRspOrderAction ��Ϣ�����а����˴������ʹ�����Ϣ��

}

///��ѯ��󱨵�������Ӧ
void TD::OnRspQueryMaxOrderVolume(CThostFtdcQueryMaxOrderVolumeField *pQueryMaxOrderVolume, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///Ͷ���߽�����ȷ����Ӧ
void TD::OnRspSettlementInfoConfirm(CThostFtdcSettlementInfoConfirmField *pSettlementInfoConfirm, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(bIsLast)
	{
		ReqQryConfirmSettlementInfo(0);
	}
}

///ɾ��Ԥ����Ӧ
void TD::OnRspRemoveParkedOrder(CThostFtdcRemoveParkedOrderField *pRemoveParkedOrder, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///ɾ��Ԥ�񳷵���Ӧ
void TD::OnRspRemoveParkedOrderAction(CThostFtdcRemoveParkedOrderActionField *pRemoveParkedOrderAction, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ������Ӧ
void TD::OnRspQryOrder(CThostFtdcOrderField *pOrder, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(m_orderReqID == nRequestID)
	{
		if(pOrder)
		{
			m_pOrders.push_back(*pOrder);
		}
		if(bIsLast)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				listener->OnOrderInfosCallBack(&m_pOrders);
			}
			m_pOrders.clear();
		}
	}
}

///�����ѯ�ɽ���Ӧ
void TD::OnRspQryTrade(CThostFtdcTradeField *pTrade, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(m_tradeReqID == nRequestID)
	{
		if(pTrade)
		{
			m_pTradeInfos.push_back(*pTrade);
		}
		if(bIsLast)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				listener->OnTradeRecordsCallBack(&m_pTradeInfos);
			}
			m_pTradeInfos.clear();
		}
	}
}

///�����ѯͶ���ֲ߳���Ӧ
void TD::OnRspQryInvestorPosition(CThostFtdcInvestorPositionField *pInvestorPosition, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(m_investorPositionReqID == nRequestID)
	{
		if(pInvestorPosition)
		{
			m_pInvestorPosition.push_back(*pInvestorPosition);
		}
		if(bIsLast)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				listener->OnInvestorPositionsCallBack(&m_pInvestorPosition);
			}
			m_pInvestorPosition.clear();
		}
	}
}

///�����ѯ�ʽ��˻���Ӧ
void TD::OnRspQryTradingAccount(CThostFtdcTradingAccountField *pTradingAccount, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(pTradingAccount)
	{
		CTPListener *listener = m_mt->GetListener();
		if(listener)
		{
			listener->OnTradeAccountCallBack(pTradingAccount);
		}
	}
}

///�����ѯͶ������Ӧ
void TD::OnRspQryInvestor(CThostFtdcInvestorField *pInvestor, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ���ױ�����Ӧ
void TD::OnRspQryTradingCode(CThostFtdcTradingCodeField *pTradingCode, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ��Լ��֤������Ӧ
void TD::OnRspQryInstrumentMarginRate(CThostFtdcInstrumentMarginRateField *pInstrumentMarginRate, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(nRequestID == m_marginRateReqID)
	{
		if(pInstrumentMarginRate)
		{
			m_pInstrumentMarginRates.push_back(*pInstrumentMarginRate);
		}
		if(bIsLast)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				listener->OnMarginRatesCallBack(&m_pInstrumentMarginRates);
			}
			m_pInstrumentMarginRates.clear();
		}
	}
}

///�����ѯ��Լ����������Ӧ
void TD::OnRspQryInstrumentCommissionRate(CThostFtdcInstrumentCommissionRateField *pInstrumentCommissionRate, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(nRequestID == m_commisionRateReqID)
	{
		if(pInstrumentCommissionRate)
		{
			m_pInstrumentCommissionRates.push_back(*pInstrumentCommissionRate);
		}
		if(bIsLast)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				listener->OnCommissionRatesCallBack(&m_pInstrumentCommissionRates);
			}
			m_pInstrumentCommissionRates.clear();
		}
	}
}

///�����ѯ��������Ӧ
void TD::OnRspQryExchange(CThostFtdcExchangeField *pExchange, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ��Ʒ��Ӧ
void TD::OnRspQryProduct(CThostFtdcProductField *pProduct, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast) 
{
}

///�����ѯ��Լ��Ӧ
void TD::OnRspQryInstrument(CThostFtdcInstrumentField *pInstrument, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(m_instrumentsReqID == nRequestID)
	{
		if(pInstrument)
		{
			m_pInstruments.push_back(*pInstrument);
		}
		if(bIsLast)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				listener->OnInstrumentsCallBack(&m_pInstruments);
			}
			m_mt->SetTdLogined(true);
			m_pInstruments.clear();
		}
	}
}

///�����ѯ������Ӧ
void TD::OnRspQryDepthMarketData(CThostFtdcDepthMarketDataField *pDepthMarketData, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯͶ���߽�������Ӧ
void TD::OnRspQrySettlementInfo(CThostFtdcSettlementInfoField *pSettlementInfo, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(bIsLast)
	{
	}
}

///�����ѯת��������Ӧ
void TD::OnRspQryTransferBank(CThostFtdcTransferBankField *pTransferBank, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯͶ���ֲ߳���ϸ��Ӧ
void TD::OnRspQryInvestorPositionDetail(CThostFtdcInvestorPositionDetailField *pInvestorPositionDetail, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(m_investorPositionDetailReqID == nRequestID)
	{
		if(pInvestorPositionDetail)
		{
			m_pInvestorPositionDetails.push_back(*pInvestorPositionDetail);
		}
		if(bIsLast)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				listener->OnInvestorPositionDetailsCallBack(&m_pInvestorPositionDetails);
			}
			m_pInvestorPositionDetails.clear();
		}
	}
}

///�����ѯ�ͻ�֪ͨ��Ӧ
void TD::OnRspQryNotice(CThostFtdcNoticeField *pNotice, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ������Ϣȷ����Ӧ
void TD::OnRspQrySettlementInfoConfirm(CThostFtdcSettlementInfoConfirmField *pSettlementInfoConfirm, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(bIsLast)
	{
	}
}

///�����ѯͶ���ֲ߳���ϸ��Ӧ
void TD::OnRspQryInvestorPositionCombineDetail(CThostFtdcInvestorPositionCombineDetailField *pInvestorPositionCombineDetail, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(m_investorPositionCombineDetailReqID == nRequestID)
	{
		if(pInvestorPositionCombineDetail)
		{
			m_pInvestorPositionCombineDetails.push_back(*pInvestorPositionCombineDetail);
		}
		if(bIsLast)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				listener->OnInvestorPositionCombineDetailsCallBack(&m_pInvestorPositionCombineDetails);
			}
			m_pInvestorPositionCombineDetails.clear();
		}
	}
}

///��ѯ��֤����ϵͳ���͹�˾�ʽ��˻���Կ��Ӧ
void TD::OnRspQryCFMMCTradingAccountKey(CThostFtdcCFMMCTradingAccountKeyField *pCFMMCTradingAccountKey, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ�ֵ��۵���Ϣ��Ӧ
void TD::OnRspQryEWarrantOffset(CThostFtdcEWarrantOffsetField *pEWarrantOffset, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯͶ����Ʒ��/��Ʒ�ֱ�֤����Ӧ
void TD::OnRspQryInvestorProductGroupMargin(CThostFtdcInvestorProductGroupMarginField *pInvestorProductGroupMargin, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ��������֤������Ӧ
void TD::OnRspQryExchangeMarginRate(CThostFtdcExchangeMarginRateField *pExchangeMarginRate, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ������������֤������Ӧ
void TD::OnRspQryExchangeMarginRateAdjust(CThostFtdcExchangeMarginRateAdjustField *pExchangeMarginRateAdjust, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ������Ӧ
void TD::OnRspQryExchangeRate(CThostFtdcExchangeRateField *pExchangeRate, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ�����������Ա����Ȩ����Ӧ
void TD::OnRspQrySecAgentACIDMap(CThostFtdcSecAgentACIDMapField *pSecAgentACIDMap, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯת����ˮ��Ӧ
void TD::OnRspQryTransferSerial(CThostFtdcTransferSerialField *pTransferSerial, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ����ǩԼ��ϵ��Ӧ
void TD::OnRspQryAccountregister(CThostFtdcAccountregisterField *pAccountregister, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///����Ӧ��
void TD::OnRspError(CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(pRspInfo)
	{
		if(m_mt)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				String errorMsg = CStrA::stringTowstring(pRspInfo->ErrorMsg);
				listener->OnLogCallBack(m_mt->GetTradingTime(), errorMsg);
				printf("errorMSG:%ws", errorMsg);
			}
		}
	}
}

///����֪ͨ
void TD::OnRtnOrder(CThostFtdcOrderField *pOrder)
{
	if(m_mt)
	{
		CTPListener *listener = m_mt->GetListener();
		if(listener)
		{
			listener->OnOrderInfoCallBack(pOrder);
		}
	}
}

///�ɽ�֪ͨ
void TD::OnRtnTrade(CThostFtdcTradeField *pTrade)
{
	//����OnRtnOrder���ExchangeID��OrderSysID�����������ﶨλ�ɽ��ر�
	if(m_mt)
	{
		CTPListener *listener = m_mt->GetListener();
		if(listener)
		{
			listener->OnTradeRecordCallBack(pTrade);
			m_mt->ReqQryInvestorPosition(GetRequestID());
			m_mt->ReqQryInvestorPositionDetail(GetRequestID());
			m_mt->ReqQryTradingAccount(GetRequestID());
		}
	}
}

///����¼�����ر�
void TD::OnErrRtnOrderInsert(CThostFtdcInputOrderField *pInputOrder, CThostFtdcRspInfoField *pRspInfo)
{
	if(pRspInfo)
	{
		if(m_mt)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				String errorMsg = CStrA::stringTowstring(pRspInfo->ErrorMsg);
				listener->OnLogCallBack(m_mt->GetTradingTime(), errorMsg);
			}
		}
	}
}

///������������ر�
void TD::OnErrRtnOrderAction(CThostFtdcOrderActionField *pOrderAction, CThostFtdcRspInfoField *pRspInfo)
{
	if(pRspInfo)
	{
		if(m_mt)
		{
			CTPListener *listener = m_mt->GetListener();
			if(listener)
			{
				String errorMsg = CStrA::stringTowstring(pRspInfo->ErrorMsg);
				listener->OnLogCallBack(m_mt->GetTradingTime(), errorMsg);
			}
		}
	}
}

///��Լ����״̬֪ͨ
void TD::OnRtnInstrumentStatus(CThostFtdcInstrumentStatusField *pInstrumentStatus)
{
}

///����֪ͨ
void TD::OnRtnTradingNotice(CThostFtdcTradingNoticeInfoField *pTradingNoticeInfo)
{
}

///��ʾ������У�����
void TD::OnRtnErrorConditionalOrder(CThostFtdcErrorConditionalOrderField *pErrorConditionalOrder)
{
}

///��֤���������û�����
void TD::OnRtnCFMMCTradingAccountToken(CThostFtdcCFMMCTradingAccountTokenField *pCFMMCTradingAccountToken)
{
}

///�����ѯǩԼ������Ӧ
void TD::OnRspQryContractBank(CThostFtdcContractBankField *pContractBank, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯԤ����Ӧ
void TD::OnRspQryParkedOrder(CThostFtdcParkedOrderField *pParkedOrder, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯԤ�񳷵���Ӧ
void TD::OnRspQryParkedOrderAction(CThostFtdcParkedOrderActionField *pParkedOrderAction, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ����֪ͨ��Ӧ
void TD::OnRspQryTradingNotice(CThostFtdcTradingNoticeField *pTradingNotice, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ���͹�˾���ײ�����Ӧ
void TD::OnRspQryBrokerTradingParams(CThostFtdcBrokerTradingParamsField *pBrokerTradingParams, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///�����ѯ���͹�˾�����㷨��Ӧ
void TD::OnRspQryBrokerTradingAlgos(CThostFtdcBrokerTradingAlgosField *pBrokerTradingAlgos, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}
///�����ѯ��������û�����
void TD::OnRspQueryCFMMCTradingAccountToken(CThostFtdcQueryCFMMCTradingAccountTokenField *pQueryCFMMCTradingAccountToken, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}
///���з��������ʽ�ת�ڻ�֪ͨ
void TD::OnRtnFromBankToFutureByBank(CThostFtdcRspTransferField *pRspTransfer)
{
}
///���з����ڻ��ʽ�ת����֪ͨ
void TD::OnRtnFromFutureToBankByBank(CThostFtdcRspTransferField *pRspTransfer)
{
}
///���з����������ת�ڻ�֪ͨ
void TD::OnRtnRepealFromBankToFutureByBank(CThostFtdcRspRepealField *pRspRepeal)
{
}
///���з�������ڻ�ת����֪ͨ
void TD::OnRtnRepealFromFutureToBankByBank(CThostFtdcRspRepealField *pRspRepeal)
{
}
///�ڻ����������ʽ�ת�ڻ�֪ͨ
void TD::OnRtnFromBankToFutureByFuture(CThostFtdcRspTransferField *pRspTransfer)
{
}
///�ڻ������ڻ��ʽ�ת����֪ͨ
void TD::OnRtnFromFutureToBankByFuture(CThostFtdcRspTransferField *pRspTransfer)
{
}
///ϵͳ����ʱ�ڻ����ֹ������������ת�ڻ��������д�����Ϻ��̷��ص�֪ͨ
void TD::OnRtnRepealFromBankToFutureByFutureManual(CThostFtdcRspRepealField *pRspRepeal)
{
}
///ϵͳ����ʱ�ڻ����ֹ���������ڻ�ת�����������д�����Ϻ��̷��ص�֪ͨ
void TD::OnRtnRepealFromFutureToBankByFutureManual(CThostFtdcRspRepealField *pRspRepeal)
{
}
///�ڻ������ѯ�������֪ͨ
void TD::OnRtnQueryBankBalanceByFuture(CThostFtdcNotifyQueryAccountField *pNotifyQueryAccount)
{
}
///�ڻ����������ʽ�ת�ڻ�����ر�
void TD::OnErrRtnBankToFutureByFuture(CThostFtdcReqTransferField *pReqTransfer, CThostFtdcRspInfoField *pRspInfo)
{
}
///�ڻ������ڻ��ʽ�ת���д���ر�
void TD::OnErrRtnFutureToBankByFuture(CThostFtdcReqTransferField *pReqTransfer, CThostFtdcRspInfoField *pRspInfo)
{
}
///ϵͳ����ʱ�ڻ����ֹ������������ת�ڻ�����ر�
void TD::OnErrRtnRepealBankToFutureByFutureManual(CThostFtdcReqRepealField *pReqRepeal, CThostFtdcRspInfoField *pRspInfo)
{
}
///ϵͳ����ʱ�ڻ����ֹ���������ڻ�ת���д���ر�
void TD::OnErrRtnRepealFutureToBankByFutureManual(CThostFtdcReqRepealField *pReqRepeal, CThostFtdcRspInfoField *pRspInfo)
{
}
///�ڻ������ѯ����������ر�
void TD::OnErrRtnQueryBankBalanceByFuture(CThostFtdcReqQueryAccountField *pReqQueryAccount, CThostFtdcRspInfoField *pRspInfo)
{
}
///�ڻ������������ת�ڻ��������д�����Ϻ��̷��ص�֪ͨ
void TD::OnRtnRepealFromBankToFutureByFuture(CThostFtdcRspRepealField *pRspRepeal)
{
}
///�ڻ���������ڻ�ת�����������д�����Ϻ��̷��ص�֪ͨ
void TD::OnRtnRepealFromFutureToBankByFuture(CThostFtdcRspRepealField *pRspRepeal)
{
}
///�ڻ����������ʽ�ת�ڻ�Ӧ��
void TD::OnRspFromBankToFutureByFuture(CThostFtdcReqTransferField *pReqTransfer, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}
///�ڻ������ڻ��ʽ�ת����Ӧ��
void TD::OnRspFromFutureToBankByFuture(CThostFtdcReqTransferField *pReqTransfer, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}
///�ڻ������ѯ�������Ӧ��
void TD::OnRspQueryBankAccountMoneyByFuture(CThostFtdcReqQueryAccountField *pReqQueryAccount, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}
///���з������ڿ���֪ͨ
void TD::OnRtnOpenAccountByBank(CThostFtdcOpenAccountField *pOpenAccount)
{
}
///���з�����������֪ͨ
void TD::OnRtnCancelAccountByBank(CThostFtdcCancelAccountField *pCancelAccount)
{
}
///���з����������˺�֪ͨ
void TD::OnRtnChangeAccountByBank(CThostFtdcChangeAccountField *pChangeAccount)
{
}