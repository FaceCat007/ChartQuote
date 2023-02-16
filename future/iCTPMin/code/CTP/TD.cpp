/*
 * iCTPMin期货行情交易(开源)
 * 上海卷卷猫信息技术有限公司
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
	req.HedgeFlag = '1';//@todo 先写死为投机了
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
	//::MessageBox(0, L"开始下单", L"提示", 0);
	printf("发送委托，合约%s, 方向%c, 开平%c, 价格%f, 手数%d\n", code.c_str(), direction, offsetFlag, price, volume);
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
	//::MessageBox(0, L"下单结束", L"提示", 0);
	if(res != 0)
	{
		//String msg = CStrA::ConvertIntToStr(res);
		//LPCWSTR wmsg = msg.c_str();
		//::MessageBox(0, wmsg, L"提示", 0);
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
	//撤单还是原来下单的requestID,不用设置
	QueryPerformanceCounter((LARGE_INTEGER*)&m_t1);//记录时间t1
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
///当客户端与交易后台建立起通信连接时（还未登录前），该方法被调用。
void TD::OnFrontConnected()
{
	ReqAuthenticate(GetRequestID());
}

///当客户端与交易后台通信连接断开时，该方法被调用。当发生这个情况后，API会自动重新连接，客户端可不做处理。
///@param nReason 错误原因
///        0x1001 网络读失败
///        0x1002 网络写失败
///        0x2001 接收心跳超时
///        0x2002 发送心跳失败
///        0x2003 收到错误报文
void TD::OnFrontDisconnected(int nReason)
{
	m_mt->SetTdLogined(false);
}
///心跳超时警告。当长时间未收到报文时，该方法被调用。
///@param nTimeLapse 距离上次接收报文的时间
void TD::OnHeartBeatWarning(int nTimeLapse)
{
	//API开发FAQ：永远不会发生
}

///客户端认证响应
void TD::OnRspAuthenticate(CThostFtdcRspAuthenticateField *pRspAuthenticateField, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	CThostFtdcReqUserLoginField reqUserLogin; 
	strcpy_s(reqUserLogin.BrokerID, m_mt->GetBrokerID().c_str());
	strcpy_s(reqUserLogin.UserID, m_mt->GetInvestorID().c_str()); 
	strcpy_s(reqUserLogin.Password, m_mt->GetPassword().c_str());
	// 发出登陆请求(int nRequestID在一个会话中不能重复，用户管理)
	m_pTradeUserApi->ReqUserLogin(&reqUserLogin, GetRequestID());
}

///登录请求响应
void TD::OnRspUserLogin(CThostFtdcRspUserLoginField *pRspUserLogin, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	CoutMP::Print() << "Login TD Success!" << CoutMP::ENDL;
	m_mt->SetSessionID(pRspUserLogin->SessionID);
	//查询所有合约
	ReqConfirmSettlementInfo(0);
	ReqInstrumentInfo(0);
}

///登出请求响应
void TD::OnRspUserLogout(CThostFtdcUserLogoutField *pUserLogout, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	m_mt->SetTdLogined(false);
}

///用户口令更新请求响应
void TD::OnRspUserPasswordUpdate(CThostFtdcUserPasswordUpdateField *pUserPasswordUpdate, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///资金账户口令更新请求响应
void TD::OnRspTradingAccountPasswordUpdate(CThostFtdcTradingAccountPasswordUpdateField *pTradingAccountPasswordUpdate, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///报单录入请求响应
void TD::OnRspOrderInsert(CThostFtdcInputOrderField *pInputOrder, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	int a = 0;
	if(pRspInfo && pRspInfo->ErrorID != 0)
	{
		//Thost 收到报单指令，如果没有通过参数校验，拒绝接受报单指令。用户就会收到OnRspOrderInsert消息
		if(pRspInfo->ErrorID == 31)//资金不足
		{
		}
	}
}

///预埋单录入请求响应
void TD::OnRspParkedOrderInsert(CThostFtdcParkedOrderField *pParkedOrder, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///预埋撤单录入请求响应
void TD::OnRspParkedOrderAction(CThostFtdcParkedOrderActionField *pParkedOrderAction, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///报单操作请求响应
void TD::OnRspOrderAction(CThostFtdcInputOrderActionField *pInputOrderAction, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	//Thost 收到撤单指令，如果没有通过参数校验，拒绝接受撤单指令。用户就会收到OnRspOrderAction 消息，其中包含了错误编码和错误消息。

}

///查询最大报单数量响应
void TD::OnRspQueryMaxOrderVolume(CThostFtdcQueryMaxOrderVolumeField *pQueryMaxOrderVolume, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///投资者结算结果确认响应
void TD::OnRspSettlementInfoConfirm(CThostFtdcSettlementInfoConfirmField *pSettlementInfoConfirm, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(bIsLast)
	{
		ReqQryConfirmSettlementInfo(0);
	}
}

///删除预埋单响应
void TD::OnRspRemoveParkedOrder(CThostFtdcRemoveParkedOrderField *pRemoveParkedOrder, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///删除预埋撤单响应
void TD::OnRspRemoveParkedOrderAction(CThostFtdcRemoveParkedOrderActionField *pRemoveParkedOrderAction, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询报单响应
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

///请求查询成交响应
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

///请求查询投资者持仓响应
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

///请求查询资金账户响应
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

///请求查询投资者响应
void TD::OnRspQryInvestor(CThostFtdcInvestorField *pInvestor, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询交易编码响应
void TD::OnRspQryTradingCode(CThostFtdcTradingCodeField *pTradingCode, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询合约保证金率响应
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

///请求查询合约手续费率响应
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

///请求查询交易所响应
void TD::OnRspQryExchange(CThostFtdcExchangeField *pExchange, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询产品响应
void TD::OnRspQryProduct(CThostFtdcProductField *pProduct, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast) 
{
}

///请求查询合约响应
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

///请求查询行情响应
void TD::OnRspQryDepthMarketData(CThostFtdcDepthMarketDataField *pDepthMarketData, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询投资者结算结果响应
void TD::OnRspQrySettlementInfo(CThostFtdcSettlementInfoField *pSettlementInfo, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(bIsLast)
	{
	}
}

///请求查询转帐银行响应
void TD::OnRspQryTransferBank(CThostFtdcTransferBankField *pTransferBank, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询投资者持仓明细响应
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

///请求查询客户通知响应
void TD::OnRspQryNotice(CThostFtdcNoticeField *pNotice, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询结算信息确认响应
void TD::OnRspQrySettlementInfoConfirm(CThostFtdcSettlementInfoConfirmField *pSettlementInfoConfirm, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
	if(bIsLast)
	{
	}
}

///请求查询投资者持仓明细响应
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

///查询保证金监管系统经纪公司资金账户密钥响应
void TD::OnRspQryCFMMCTradingAccountKey(CThostFtdcCFMMCTradingAccountKeyField *pCFMMCTradingAccountKey, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询仓单折抵信息响应
void TD::OnRspQryEWarrantOffset(CThostFtdcEWarrantOffsetField *pEWarrantOffset, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询投资者品种/跨品种保证金响应
void TD::OnRspQryInvestorProductGroupMargin(CThostFtdcInvestorProductGroupMarginField *pInvestorProductGroupMargin, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询交易所保证金率响应
void TD::OnRspQryExchangeMarginRate(CThostFtdcExchangeMarginRateField *pExchangeMarginRate, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询交易所调整保证金率响应
void TD::OnRspQryExchangeMarginRateAdjust(CThostFtdcExchangeMarginRateAdjustField *pExchangeMarginRateAdjust, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询汇率响应
void TD::OnRspQryExchangeRate(CThostFtdcExchangeRateField *pExchangeRate, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询二级代理操作员银期权限响应
void TD::OnRspQrySecAgentACIDMap(CThostFtdcSecAgentACIDMapField *pSecAgentACIDMap, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询转帐流水响应
void TD::OnRspQryTransferSerial(CThostFtdcTransferSerialField *pTransferSerial, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询银期签约关系响应
void TD::OnRspQryAccountregister(CThostFtdcAccountregisterField *pAccountregister, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///错误应答
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

///报单通知
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

///成交通知
void TD::OnRtnTrade(CThostFtdcTradeField *pTrade)
{
	//利用OnRtnOrder里的ExchangeID、OrderSysID，可以在这里定位成交回报
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

///报单录入错误回报
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

///报单操作错误回报
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

///合约交易状态通知
void TD::OnRtnInstrumentStatus(CThostFtdcInstrumentStatusField *pInstrumentStatus)
{
}

///交易通知
void TD::OnRtnTradingNotice(CThostFtdcTradingNoticeInfoField *pTradingNoticeInfo)
{
}

///提示条件单校验错误
void TD::OnRtnErrorConditionalOrder(CThostFtdcErrorConditionalOrderField *pErrorConditionalOrder)
{
}

///保证金监控中心用户令牌
void TD::OnRtnCFMMCTradingAccountToken(CThostFtdcCFMMCTradingAccountTokenField *pCFMMCTradingAccountToken)
{
}

///请求查询签约银行响应
void TD::OnRspQryContractBank(CThostFtdcContractBankField *pContractBank, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询预埋单响应
void TD::OnRspQryParkedOrder(CThostFtdcParkedOrderField *pParkedOrder, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询预埋撤单响应
void TD::OnRspQryParkedOrderAction(CThostFtdcParkedOrderActionField *pParkedOrderAction, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询交易通知响应
void TD::OnRspQryTradingNotice(CThostFtdcTradingNoticeField *pTradingNotice, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询经纪公司交易参数响应
void TD::OnRspQryBrokerTradingParams(CThostFtdcBrokerTradingParamsField *pBrokerTradingParams, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}

///请求查询经纪公司交易算法响应
void TD::OnRspQryBrokerTradingAlgos(CThostFtdcBrokerTradingAlgosField *pBrokerTradingAlgos, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}
///请求查询监控中心用户令牌
void TD::OnRspQueryCFMMCTradingAccountToken(CThostFtdcQueryCFMMCTradingAccountTokenField *pQueryCFMMCTradingAccountToken, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}
///银行发起银行资金转期货通知
void TD::OnRtnFromBankToFutureByBank(CThostFtdcRspTransferField *pRspTransfer)
{
}
///银行发起期货资金转银行通知
void TD::OnRtnFromFutureToBankByBank(CThostFtdcRspTransferField *pRspTransfer)
{
}
///银行发起冲正银行转期货通知
void TD::OnRtnRepealFromBankToFutureByBank(CThostFtdcRspRepealField *pRspRepeal)
{
}
///银行发起冲正期货转银行通知
void TD::OnRtnRepealFromFutureToBankByBank(CThostFtdcRspRepealField *pRspRepeal)
{
}
///期货发起银行资金转期货通知
void TD::OnRtnFromBankToFutureByFuture(CThostFtdcRspTransferField *pRspTransfer)
{
}
///期货发起期货资金转银行通知
void TD::OnRtnFromFutureToBankByFuture(CThostFtdcRspTransferField *pRspTransfer)
{
}
///系统运行时期货端手工发起冲正银行转期货请求，银行处理完毕后报盘发回的通知
void TD::OnRtnRepealFromBankToFutureByFutureManual(CThostFtdcRspRepealField *pRspRepeal)
{
}
///系统运行时期货端手工发起冲正期货转银行请求，银行处理完毕后报盘发回的通知
void TD::OnRtnRepealFromFutureToBankByFutureManual(CThostFtdcRspRepealField *pRspRepeal)
{
}
///期货发起查询银行余额通知
void TD::OnRtnQueryBankBalanceByFuture(CThostFtdcNotifyQueryAccountField *pNotifyQueryAccount)
{
}
///期货发起银行资金转期货错误回报
void TD::OnErrRtnBankToFutureByFuture(CThostFtdcReqTransferField *pReqTransfer, CThostFtdcRspInfoField *pRspInfo)
{
}
///期货发起期货资金转银行错误回报
void TD::OnErrRtnFutureToBankByFuture(CThostFtdcReqTransferField *pReqTransfer, CThostFtdcRspInfoField *pRspInfo)
{
}
///系统运行时期货端手工发起冲正银行转期货错误回报
void TD::OnErrRtnRepealBankToFutureByFutureManual(CThostFtdcReqRepealField *pReqRepeal, CThostFtdcRspInfoField *pRspInfo)
{
}
///系统运行时期货端手工发起冲正期货转银行错误回报
void TD::OnErrRtnRepealFutureToBankByFutureManual(CThostFtdcReqRepealField *pReqRepeal, CThostFtdcRspInfoField *pRspInfo)
{
}
///期货发起查询银行余额错误回报
void TD::OnErrRtnQueryBankBalanceByFuture(CThostFtdcReqQueryAccountField *pReqQueryAccount, CThostFtdcRspInfoField *pRspInfo)
{
}
///期货发起冲正银行转期货请求，银行处理完毕后报盘发回的通知
void TD::OnRtnRepealFromBankToFutureByFuture(CThostFtdcRspRepealField *pRspRepeal)
{
}
///期货发起冲正期货转银行请求，银行处理完毕后报盘发回的通知
void TD::OnRtnRepealFromFutureToBankByFuture(CThostFtdcRspRepealField *pRspRepeal)
{
}
///期货发起银行资金转期货应答
void TD::OnRspFromBankToFutureByFuture(CThostFtdcReqTransferField *pReqTransfer, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}
///期货发起期货资金转银行应答
void TD::OnRspFromFutureToBankByFuture(CThostFtdcReqTransferField *pReqTransfer, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}
///期货发起查询银行余额应答
void TD::OnRspQueryBankAccountMoneyByFuture(CThostFtdcReqQueryAccountField *pReqQueryAccount, CThostFtdcRspInfoField *pRspInfo, int nRequestID, bool bIsLast)
{
}
///银行发起银期开户通知
void TD::OnRtnOpenAccountByBank(CThostFtdcOpenAccountField *pOpenAccount)
{
}
///银行发起银期销户通知
void TD::OnRtnCancelAccountByBank(CThostFtdcCancelAccountField *pCancelAccount)
{
}
///银行发起变更银行账号通知
void TD::OnRtnChangeAccountByBank(CThostFtdcChangeAccountField *pChangeAccount)
{
}