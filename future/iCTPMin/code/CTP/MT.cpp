/*
 * iCTPMin期货行情交易(开源)
 * 上海卷卷猫信息技术有限公司
 */

#include "..\stdafx.h"
#include "MT.h"

void CTPListener::OnCommissionRatesCallBack(vector<CThostFtdcInstrumentCommissionRateField> *pInstrumentCommissionRates)
{
}

void CTPListener::OnDepthMarketDatasCallBack(CThostFtdcDepthMarketDataField *pDepthMarketData)
{
}

void CTPListener::OnInstrumentsCallBack(vector<CThostFtdcInstrumentField> *pInstruments)
{
}

void CTPListener::OnInvestorPositionsCallBack(vector<CThostFtdcInvestorPositionField> *pInvestorPosition)
{
}

void CTPListener::OnInvestorPositionCombineDetailsCallBack(vector<CThostFtdcInvestorPositionCombineDetailField> *pInvestorPositionCombineDetails)
{
}

void CTPListener::OnInvestorPositionDetailsCallBack(vector<CThostFtdcInvestorPositionDetailField> *pInvestorPositionDetails)
{
}

void CTPListener::OnLogCallBack(const String& time, const String& log)
{
}

void CTPListener::OnMarginRatesCallBack(vector<CThostFtdcInstrumentMarginRateField> *pInstrumentMarginRates)
{
}

void CTPListener::OnOrderInfoCallBack(CThostFtdcOrderField *pOrder)
{
}

void CTPListener::OnOrderInfosCallBack(vector<CThostFtdcOrderField> *pOrders)
{
}

void CTPListener::OnStateCallBack()
{
}

void CTPListener::OnTradeAccountCallBack(CThostFtdcTradingAccountField *pTradingAccount)
{
}

void CTPListener::OnTradeRecordCallBack(CThostFtdcTradeField *pTradeInfo)
{
}

void CTPListener::OnTradeRecordsCallBack(vector<CThostFtdcTradeField> *pTradeInfos)
{
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

CTPQuery::CTPQuery(int id, int reqID, void *args, const string &code)
: m_code(code){
	m_id = id;
	m_reqID = reqID;
	m_args = args;
}

CTPQuery::~CTPQuery(){
	m_args = 0;
}

CTPOpenQuery::CTPOpenQuery(const string &code, const string& exchangeID, double price, int qty, int type, char timeCondition, int reqID, const string &orderRef)
: m_code(code)
, m_price(price)
, m_orderRef(orderRef){
	m_qty = qty;
	m_reqID = reqID;
	m_timeCondition = timeCondition;
	m_type = type;
	m_exchangeID = exchangeID;
}

CTPCloseQuery::CTPCloseQuery(char closeType, const string &code, const string& exchangeID, double price, int qty, int type, char timeCondition, int reqID, const string &orderRef)
: m_code(code)
, m_price(price)
, m_orderRef(orderRef){
	m_closeType = closeType;
	m_qty = qty;
	m_reqID = reqID;
	m_timeCondition = timeCondition;
	m_type = type;
	m_exchangeID = exchangeID;
}

CTPCancelOrderQuery::CTPCancelOrderQuery(const string &exchangeID, const string &orderSysID, int reqID, const string &orderRef)
: m_exchangeID(exchangeID)
, m_orderSysID(orderSysID)
, m_orderRef(orderRef){
	m_reqID = reqID;
}

CTPSubMarketDataQuery::CTPSubMarketDataQuery(){
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

CTPServerConfig::CTPServerConfig()
{
	m_brokerID = "";
	m_investorID = "";
	m_name = "";
	m_password = "";
}

void CTPServerConfig::Clear()
{
	m_brokerID = "";
	m_investorID = "";
	m_mtFronts.clear();
	m_name = "";
	m_password = "";
	m_tdFronts.clear();
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

DWORD WINAPI MT::CheckMT(LPVOID lpParam)
{
	MT *mt = (MT*)lpParam;
	int i = 0;
	while(mt->IsMdRunning() && mt->IsTdRunning())
	{
		i += 10;
		if(i > 10000)
		{
			i = 0;
		}
		::Sleep(10);
	}
	if(mt->IsClearanced() && IsClearanceTime())
	{
		mt->SetClearanced(false);
	}
	//销毁交易
	return 0;
}

DWORD WINAPI MT::CheckLogined(LPVOID lpParam)
{
	MT *mt = (MT*)lpParam;
	int i = 0;
	while(i < 5000)
	{
		if(mt->IsMdLogined() && mt->IsTdLogined())
		{
			HANDLE checkMTThread = ::CreateThread(0, 0, CheckMT, (LPVOID)mt, 0, 0);
			CloseHandle(checkMTThread);
			return 0;
		}
		i += 10;
		::Sleep(10);
	}
	if(mt->IsTdRunning())
	{
		SetEvent(mt->m_tdEvent);
	}
	if(mt->IsMdRunning())
	{
		SetEvent(mt->m_mdEvent);
	}
	//销毁交易
	return 0;
}

DWORD WINAPI MT::CheckTradingTime(LPVOID lpParam)
{
	MT *mt = (MT*)lpParam;
	while(1)
	{
		::Sleep(1000);
		if(!IsTradingTime())
		{
			if(mt->IsTdRunning())
			{
				SetEvent(mt->m_tdEvent);
			}
			if(mt->IsMdRunning())
			{
				SetEvent(mt->m_mdEvent);
			}
			mt->SetConnected(false);
		}
		else
		{
			if(!mt->IsConnected())
			{
				mt->Start();
			}
		}
	}
	return 0;
}

DWORD WINAPI MT::LoadMD(LPVOID lpParam)
{
	MT *mt = (MT*)lpParam;
	ResetEvent(mt->m_mdEvent);
	string mdConDir = GetProgramDir() + "\\data";
	if(!CFileA::IsDirectoryExist(mdConDir.c_str()))
	{
		CFileA::CreateDirectory(mdConDir.c_str());
	}
	mdConDir += "\\MD_"+ mt->GetBrokerID() + "_" + mt->GetInvestorID() +"\\";
	if(!CFileA::IsDirectoryExist(mdConDir.c_str()))
	{
		CFileA::CreateDirectory(mdConDir.c_str());
	}
	CThostFtdcMdApi *pUserApi = CThostFtdcMdApi::CreateFtdcMdApi(mdConDir.c_str(), false, false); 
	MD sh(pUserApi); 
	sh.SetMT(mt);
	mt->SetMD(&sh);
	pUserApi->RegisterSpi(&sh); 
	vector<string> fronts = mt->GetMdServers();	
	for (size_t i = 0; i < fronts.size(); ++i)
	{
		StringPtr ptr(fronts[i]);
		pUserApi->RegisterFront(ptr.GetPtr()); 
	}
	pUserApi->Init(); 
	WaitForSingleObject(mt->m_mdEvent, INFINITE);
	mt->SetMD(0);
	pUserApi->Release();
	mt->SetMdRunning(false);
	return 0;
}

DWORD WINAPI MT::LoadTD(LPVOID lpParam)
{
	MT *mt = (MT*)lpParam;
	ResetEvent(mt->m_tdEvent);
	string tdConDir = GetProgramDir() + "\\data";
	if(!CFileA::IsDirectoryExist(tdConDir.c_str()))
	{
		CFileA::CreateDirectory(tdConDir.c_str());
	}
	tdConDir += "\\TD_"+ mt->GetBrokerID() + "_" + mt->GetInvestorID() +"\\";
	if(!CFileA::IsDirectoryExist(tdConDir.c_str()))
	{
		CFileA::CreateDirectory(tdConDir.c_str());
	}
	CThostFtdcTraderApi *pTradeUserApi= CThostFtdcTraderApi::CreateFtdcTraderApi(tdConDir.c_str()); 
	TD td(pTradeUserApi);
	td.SetMT(mt);
	mt->SetTD(&td);
	pTradeUserApi->RegisterSpi(&td);
	pTradeUserApi->SubscribePrivateTopic(THOST_TERT_QUICK); 
	pTradeUserApi->SubscribePublicTopic(THOST_TERT_QUICK); 
	vector<string> fronts = mt->GetTdServers();	
	for (size_t i = 0; i < fronts.size(); ++i)
	{
		StringPtr ptr(fronts[i]);
		pTradeUserApi->RegisterFront(ptr.GetPtr()); 
	}
	pTradeUserApi->Init();
	WaitForSingleObject(mt->m_tdEvent, INFINITE);
	mt->SetTD(0);
	pTradeUserApi->Release();
	mt->SetTdRunning(false);
	return 0;
}

DWORD WINAPI MT::QueryData(LPVOID lpParam)
{
	int tick = 0;
	MT *mt = (MT*)lpParam;	
	while (1){
		//checkTradingTime(mt);
		if (mt->IsTdLogined()){
			if (tick == 0){
				::Sleep(1000);
			}
			TD *td = mt->m_td;
			MD *md = mt->m_md;
			CTPQuery ctpQuery(0, 0, 0);
			bool sleep = true;
			if (mt->GetNewQuery(&ctpQuery)){
				switch (ctpQuery.m_id){
				case 0:{
						if(td){
							td->ReqCommissionRate(ctpQuery.m_code, ctpQuery.m_reqID);
						}		
					}					
					break;
				case 1:{
						if(td){
							td->ReqInstrumentInfo(ctpQuery.m_reqID);
						}
					}
					break;
				case 2:{
						if(td){
							td->ReqQryInvestorPosition(ctpQuery.m_reqID);
						}
					}					
					break;
				case 3:{
						if(td){
							td->ReqQryInvestorPositionDetail(ctpQuery.m_reqID);
						}
					}					
					break;
				case 4:{
						if(td){
							td->ReqMarginRate(ctpQuery.m_code, ctpQuery.m_reqID);
						}
					}					
					break;
				case 5:{
						if(td){
							td->ReqQryOrderInfo(ctpQuery.m_reqID);
						}
					}					
					break;
				case 6:{
						if(td){
							td->ReqQryTradingAccount(ctpQuery.m_reqID);
						}
					}					
					break;
				case 7:{
						if(td){
							td->ReqQryTradeInfo(ctpQuery.m_reqID);
						}
					}
					break;
				case 8:{
						if(td){
							td->ReqConfirmSettlementInfo(ctpQuery.m_reqID);
						}
					}
					break;
				case 12:
				case 13:{
						if(md){
							//注册行情
							CTPSubMarketDataQuery *subMarketDataQuery = (CTPSubMarketDataQuery*)ctpQuery.m_args;
							if (subMarketDataQuery){
								vector<string> codes = subMarketDataQuery->m_codes;
								int codesSize = (int)codes.size();
								if(codesSize > 0){
									char **instrumnets = new char*[codesSize];
									memset(instrumnets, 0, codesSize*sizeof(char*));
									for(int i = 0; i < codesSize; i++){
										string code = codes[i];
										int cLen = (int)code.length() + 1;
										char *strCode = new char[31];
										memcpy(strCode, code.c_str(), cLen);
										instrumnets[i] = strCode;
									}
									if(md){
										if(ctpQuery.m_id == 12){
											int rs = md->GetUserApi()->SubscribeMarketData(instrumnets, codesSize);
											int a = rs;
										}
										else if(ctpQuery.m_id == 13){
											md->GetUserApi()->UnSubscribeMarketData(instrumnets, codesSize);
										}
									}
									for (int j = 0; j < codesSize; ++j){
										if (instrumnets[j]){
											delete []instrumnets[j];
											instrumnets[j] = 0;
										}
									}
									delete[] instrumnets;
									instrumnets = 0;
								}
							}
						}
					}
					break;
				case 14:
					if(td){
						//td->ReqDepthMarketData(ctpQuery.m_code, ctpQuery.m_reqID);
					}
					break;
				case 15:
					if(td){
						//td->ReqSettlementInfo(ctpQuery.m_code, ctpQuery.m_reqID);
					}
					break;
				}
				if(sleep){
					::Sleep(1000);
				}
				tick++;
			}
		}
		::Sleep(1);
	}
}


int MT::tradeHandler(const CTPQuery &ctpQuery){
	TD *td = m_td;
	switch(ctpQuery.m_id){
	case 9:{
			CTPOpenQuery *openQuery = (CTPOpenQuery*)ctpQuery.m_args;
			if(openQuery){
				//买开仓
				if(openQuery->m_type == 0){
					td->order(openQuery->m_code, openQuery->m_exchangeID, THOST_FTDC_D_Buy, THOST_FTDC_OF_Open, openQuery->m_price, openQuery->m_qty, openQuery->m_timeCondition, openQuery->m_reqID, openQuery->m_orderRef);
				}
				//卖开仓
				else if(openQuery->m_type == 1){
					td->order(openQuery->m_code, openQuery->m_exchangeID, THOST_FTDC_D_Sell, THOST_FTDC_OF_Open, openQuery->m_price, openQuery->m_qty, openQuery->m_timeCondition, openQuery->m_reqID, openQuery->m_orderRef);
				}
			}
			break;
		}
	case 10:{
			CTPCloseQuery *closeQuery = (CTPCloseQuery*)ctpQuery.m_args;
			if(closeQuery){
				//买平仓
				if(closeQuery->m_type == 0){
					td->order(closeQuery->m_code, closeQuery->m_exchangeID, THOST_FTDC_D_Buy, closeQuery->m_closeType, closeQuery->m_price, closeQuery->m_qty, closeQuery->m_timeCondition, closeQuery->m_reqID, closeQuery->m_orderRef);
				}
				//卖平仓
				else if(closeQuery->m_type == 1){
					td->order(closeQuery->m_code, closeQuery->m_exchangeID, THOST_FTDC_D_Sell, closeQuery->m_closeType, closeQuery->m_price, closeQuery->m_qty, closeQuery->m_timeCondition, closeQuery->m_reqID, closeQuery->m_orderRef);
				}
			}
			break;
		}
	case 11:{
			//撤单
			CTPCancelOrderQuery *cancelOrderQuery = (CTPCancelOrderQuery*)ctpQuery.m_args;
			if(cancelOrderQuery){
				td->cancelOrder(cancelOrderQuery->m_exchangeID.c_str(), cancelOrderQuery->m_orderSysID.c_str(), cancelOrderQuery->m_reqID, cancelOrderQuery->m_orderRef);
			}
			break;
		}
	}
	return 0;
}

MT::MT()
{
	m_checkingLogined = false;
	m_clearanced = false;
	m_connected = false;
	m_listener = 0;
	m_md = 0;
	m_mdEvent = CreateEvent(0, true, false, 0);
	m_mdIsLogined = false;
	m_mdIsRunning = false;
	m_sessionID = 0;
	m_td = 0;
	m_tdEvent = CreateEvent(0, true, false, 0);
	m_tdIsLogined = false;
	m_tdIsRunning = false;
}

MT::~MT()
{
	m_codesMap.clear();
	if(m_events.size() > 0)
	{
		map<int, vector<void*>*>::iterator sIter = m_events.begin();
		for(; sIter != m_events.end(); ++sIter)
		{
			delete sIter->second;
		}
		m_events.clear();
	}
	if(m_invokes.size() > 0)
	{
		map<int, vector<void*>*>::iterator sIter = m_invokes.begin();
		for(; sIter != m_invokes.end(); ++sIter)
		{
			delete sIter->second;
		}
		m_invokes.clear();
	}
	m_listener = 0;
	m_Lock.Lock();
	for (size_t j = 0; j < m_querys.size(); ++j)
	{
		delete m_querys[j].m_args;
		m_querys[j].m_args = 0;
	}
	m_querys.clear();
	for (size_t i = 0; i <m_querys2.size(); ++i)
	{
		delete m_querys2[i].m_args;
		m_querys2[i].m_args = 0;
	}
	m_querys2.clear();
	m_Lock.UnLock();
}

string MT::GetAppID(){
	return m_serverConfig.m_appID;
}

void MT::SetAppID(const string &appID){
	m_serverConfig.m_appID = appID;
}

string MT::GetAuthCode(){
	return m_serverConfig.m_authCode;
}

void MT::SetAuthCode(const string &authCode){
	m_serverConfig.m_authCode = authCode;
}

string MT::GetBrokerID()
{
	return m_serverConfig.m_brokerID;
}

void MT::SetBrokerID(const string &brokerID)
{
	m_serverConfig.m_brokerID = brokerID;
}

bool MT::IsClearanced()
{
	return m_clearanced;
}

void MT::SetClearanced(bool clearanced)
{
	m_clearanced = clearanced;
}

bool MT::IsConnected()
{
	return m_connected;
}

void MT::SetConnected(bool connected)
{
	m_connected = connected;
}

string MT::GetInvestorID()
{
	return m_serverConfig.m_investorID;
}

void MT::SetInvestorID(const string &investorID)
{
	m_serverConfig.m_investorID = investorID;
}

CTPListener* MT::GetListener()
{
	return m_listener;
}

void MT::SetListener(CTPListener *listener)
{
	m_listener = listener;
}

MD* MT::GetMD()
{
	return m_md;
}

void MT::SetMD(MD *md)
{
	m_md = md;
}

bool MT::IsMdLogined()
{
	return m_mdIsLogined;
}

void MT::SetMdLogined(bool mdIsLogined)
{
	m_mdIsLogined = mdIsLogined;
	m_listener->OnStateCallBack();
	m_listener->OnLogCallBack(GetTradingTime(), L"行情服务器登录成功");
}

bool MT::IsMdRunning()
{
	return m_mdIsRunning;
}

void MT::SetMdRunning(bool mdIsRunning)
{
	m_mdIsRunning = mdIsRunning;
}

vector<string> MT::GetMdServers()
{
	return m_serverConfig.m_mtFronts;
}

void MT::AddMdServer(const string &mdServer)
{
	m_serverConfig.m_mtFronts.push_back(mdServer);
}

string MT::GetPassword()
{
	return m_serverConfig.m_password;
}

void MT::SetPassword(const string &password)
{
	m_serverConfig.m_password = password;
}

int MT::GetSessionID()
{
	return m_sessionID;
}

void MT::SetSessionID(int sessionID)
{
	m_sessionID = sessionID;
}

TD* MT::GetTD()
{
	return m_td;
}

void MT::SetTD(TD *td)
{
	m_td = td;
}

bool MT::IsTdLogined()
{
	return m_tdIsLogined;
}

void MT::SetTdLogined(bool tdIsLogined)
{
	m_tdIsLogined = tdIsLogined;
	if(tdIsLogined)
	{
		ReqQryTradingAccount(0);	
		ReqQryTradeInfo(0);
		ReqCommissionRate(L"", 0);
		ReqMarginRate(L"", 0);
		m_listener->OnStateCallBack();
		m_listener->OnLogCallBack(GetTradingTime(), L"交易服务器登录成功");
	}
}

bool MT::IsTdRunning()
{
	return m_tdIsRunning;
}

void MT::SetTdRunning(bool tdIsRunning)
{
	m_tdIsRunning = tdIsRunning;
}

vector<string> MT::GetTdServers()
{
	return m_serverConfig.m_tdFronts;
}

void MT::AddTdServer(const string &tdServer)
{
	m_serverConfig.m_tdFronts.push_back(tdServer);
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

bool MT::AddInsID( const String &code )
{
	bool res = false;
	vector<String>::const_iterator sIter = find(m_allInsID.begin(), m_allInsID.end(), code);
	if (sIter == m_allInsID.end()){
		m_allInsID.push_back(code);
		res = true;
	}
	return res;
}

int MT::AskClose(const String& code, const String& exchangeID, double  price, int qty, char timeCondition, int reqID, const String &orderRef){
	if(m_td){
		string sCode = CStrA::wstringTostring(code);
		string gID = CStrA::wstringTostring(orderRef);
		string eID = CStrA::wstringTostring(exchangeID);
		CTPCloseQuery closeQuery('4', sCode, eID, price, qty, 1, timeCondition, reqID, gID);
		tradeHandler(CTPQuery(10, reqID, &closeQuery));
	}
	return reqID;
}

int MT::AskCloseToday(const String& code, const String& exchangeID, double  price, int qty, char timeCondition, int reqID, const String &orderRef){
	if(m_td){
		string sCode = CStrA::wstringTostring(code);
		string gID = CStrA::wstringTostring(orderRef);
		string eID = CStrA::wstringTostring(exchangeID);
		CTPCloseQuery closeQuery('3', sCode, eID, price, qty, 1, timeCondition, reqID, gID);
		tradeHandler(CTPQuery(10, reqID, &closeQuery));
	}
	return reqID;
}

int MT::AskOpen(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef){
	if(m_td){
		string sCode = CStrA::wstringTostring(code);
		string gID = CStrA::wstringTostring(orderRef);
		string eID = CStrA::wstringTostring(exchangeID);
		CTPOpenQuery openQuery(sCode, eID, price, qty, 1, timeCondition, reqID, gID);
		tradeHandler(CTPQuery(9, reqID, &openQuery));
	}
	return reqID;
}

int MT::BidClose(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef){
	if(m_td){
		string sCode = CStrA::wstringTostring(code);
		string gID = CStrA::wstringTostring(orderRef);
		string eID = CStrA::wstringTostring(exchangeID);
		CTPCloseQuery closeQuery('4', sCode, eID, price, qty, 0, timeCondition, reqID, gID);
		tradeHandler(CTPQuery(10, reqID, &closeQuery));
	}
	return reqID;
}

int MT::BidCloseToday(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef){
	if(m_td){
		string sCode = CStrA::wstringTostring(code);
		string gID = CStrA::wstringTostring(orderRef);
		string eID = CStrA::wstringTostring(exchangeID);
		CTPCloseQuery closeQuery('3', sCode, eID, price, qty, 0, timeCondition, reqID, gID);
		tradeHandler(CTPQuery(10, reqID, &closeQuery));
	}
	return reqID;
}

int MT::BidOpen(const String& code, const String& exchangeID, double price, int qty, char timeCondition, int reqID, const String &orderRef){
	if(m_td){
		//::messageBox(0, L"MT::BidOpen", L"提示", 0);
		string sCode = CStrA::wstringTostring(code);
		string gID = CStrA::wstringTostring(orderRef);
		string eID = CStrA::wstringTostring(exchangeID);
		CTPOpenQuery openQuery(sCode, eID, price, qty, 0, timeCondition, reqID, gID);
		tradeHandler(CTPQuery(9, reqID, &openQuery));
	}
	return reqID;
}

int MT::CancelOrder(const String& exchangeID, const String& orderSysID, int reqID, const String &orderRef){
	if(m_td){
		string sExchangeID = CStrA::wstringTostring(exchangeID);
		string sOrderSysID = CStrA::wstringTostring(orderSysID);
		string gID = CStrA::wstringTostring(orderRef);
		CTPCancelOrderQuery cancelOrderQuery(sExchangeID, sOrderSysID, reqID, gID);
		tradeHandler(CTPQuery(11, reqID, &cancelOrderQuery));
	}
	return reqID;
}

string MT::GetProgramDir()
{
	char exeFullPath[MAX_PATH]; 
	string strPath = "";
	GetModuleFileNameA(0, exeFullPath, MAX_PATH);
	strPath = (string)exeFullPath; 
	int pos = (int)strPath.find_last_of('\\', strPath.length());
	return strPath.substr(0, pos);
}

int MT::GetNewQuery(CTPQuery *ctpQuery)
{
	int state = 0;
	m_Lock.Lock();
	int querysSize = (int)m_querys.size();
	if(querysSize > 0)
	{
		*ctpQuery = m_querys[0];
		m_querys.erase(m_querys.begin());
		state = 1;
	}
	m_Lock.UnLock();
	return state;
}

int MT::GetNewQuery2(CTPQuery *ctpQuery)
{
	int state = 0;
	m_Lock.Lock();
	int querysSize = (int)m_querys2.size();
	if(querysSize > 0)
	{
		*ctpQuery = m_querys2[0];
		m_querys2.erase(m_querys2.begin());
		state = 1;
	}
	m_Lock.UnLock();
	return state;
}

String MT::GetTradingDate()
{
	if(m_td)
	{
		return m_td->GetTradingData();
	}
	return L"";
}

String MT::GetTradingTime()
{
	SYSTEMTIME sysTime;    
	ZeroMemory(&sysTime, sizeof(sysTime));    
	GetLocalTime(&sysTime);
	wchar_t szTime[100] = {0};
	_stprintf_s(szTime, 99, L"%02d:%02d:%02d", sysTime.wHour, sysTime.wMinute, sysTime.wSecond);
	return szTime;
}

bool MT::IsClearanceTime()
{
	SYSTEMTIME sysTime;    
	ZeroMemory(&sysTime, sizeof(sysTime));    
	GetLocalTime(&sysTime);
	long quoteTime = sysTime.wHour * 100 + sysTime.wMinute;
	if(sysTime.wDayOfWeek != 0 && sysTime.wDayOfWeek != 6 && quoteTime >= 1520 && quoteTime <= 1530)
	{
		return true;
	}
	return false;
}

bool MT::IsTradingTime()
{
	return true;
	SYSTEMTIME sysTime;    
	ZeroMemory(&sysTime, sizeof(sysTime));    
	GetLocalTime(&sysTime);
	long quoteTime = sysTime.wHour * 10000 + sysTime.wMinute * 100 + sysTime.wSecond;
	if((quoteTime >= 85000 && quoteTime <= 152500) || (quoteTime >= 205000 || quoteTime <= 24000))
	{
		return true;
	}
	return false;
}

bool MT::IsTradingTimeExact(String code)
{
	return true;
	SYSTEMTIME sysTime;    
	ZeroMemory(&sysTime, sizeof(sysTime));    
	GetLocalTime(&sysTime);
	long quoteTime = sysTime.wHour * 10000 + sysTime.wMinute * 100 + sysTime.wSecond;
	if(code == L"cu")
	{
		if((quoteTime >= 90000 && quoteTime <= 101400) || (quoteTime >= 103000 && quoteTime <= 112900) || (quoteTime >= 133000 && quoteTime <= 145900) || (quoteTime >= 210000 || quoteTime <= 5900))
		{
			return true;
		}
	}
	else if(code == L"ag")
	{
		if((quoteTime >= 90000 && quoteTime <= 101400) || (quoteTime >= 103000 && quoteTime <= 112900) || (quoteTime >= 133000 && quoteTime <= 145900) || (quoteTime >= 210000 || quoteTime <= 22900))
		{
			return true;
		}
	}
	else if(code == L"m")
	{
		if((quoteTime >= 90000 && quoteTime <= 101400) || (quoteTime >= 103000 && quoteTime <= 112900) || (quoteTime >= 133000 && quoteTime <= 145900) || (quoteTime >= 210000 && quoteTime <= 232900))
		{
			return true;
		}
	}

	return false;
}

void MT::ReqCommissionRate(const String &wCode, int reqID){
	if((int)wCode.length() == 0){
		m_Lock.Lock();
		m_querys.push_back(CTPQuery(0, reqID, 0, ""));
		m_Lock.UnLock();
		return;
	}
	bool exists = false;
	m_allCommissionIDLock.Lock();
	if(m_allCommissionID.find(wCode) != m_allCommissionID.end()){
		exists = true;
	}
	else{
		m_allCommissionID[wCode] = L"";
	}
	m_allCommissionIDLock.UnLock();
	if(!exists){
		string code = CStrA::wstringTostring(wCode);
		m_Lock.Lock();
		m_reqID2codes[reqID] = code;
		m_querys.push_back(CTPQuery(0, reqID, 0, code));
		m_Lock.UnLock();
	}
}

void MT::ReqConfirmSettlementInfo(int reqID)
{
	m_Lock.Lock();
	m_querys.push_back(CTPQuery(8, reqID, 0));
	m_Lock.UnLock();
}

void MT::ReqInstrumentInfo(int reqID)
{
	m_Lock.Lock();
	m_querys.push_back(CTPQuery(1, reqID, 0));
	m_Lock.UnLock();
}

void MT::ReqQryInvestorPosition(int reqID)
{
	m_Lock.Lock();
	m_querys.push_back(CTPQuery(2, reqID, 0));
	m_Lock.UnLock();
}

void MT::ReqQryInvestorPositionDetail(int reqID)
{
	m_Lock.Lock();
	m_querys.push_back(CTPQuery(3, reqID, 0));
	m_Lock.UnLock();
}

void MT::ReqMarginRate(const String &wCode, int reqID)
{
	if((int)wCode.length() == 0){
		m_Lock.Lock();
		m_querys.push_back(CTPQuery(4, reqID, 0, ""));
		m_Lock.UnLock();
		return;
	}
	bool exists = false;
	m_allMarginIDLock.Lock();
	if(m_allMarginID.find(wCode) != m_allMarginID.end()){
		exists = true;
	}
	else{
		m_allMarginID[wCode] = L"";
	}
	m_allMarginIDLock.UnLock();
	if(!exists){
		string code = CStrA::wstringTostring(wCode);
		m_Lock.Lock();
		m_querys.push_back(CTPQuery(4, reqID, 0, code));
		m_Lock.UnLock();
	}
}

void MT::RspQryOrderInfo(int reqID)
{
	m_Lock.Lock();
	m_querys.push_back(CTPQuery(5, reqID,  0));
	m_Lock.UnLock();
}

void MT::ReqQryTradingAccount(int reqID)
{
	m_Lock.Lock();
	m_querys.push_back(CTPQuery(6, reqID, 0));
	m_Lock.UnLock();
}

void MT::ReqQryTradeInfo(int reqID)
{
	m_Lock.Lock();
	m_querys.push_back(CTPQuery(7, reqID, 0));
	m_Lock.UnLock();
}

void MT::Start()
{
	if(IsTradingTime())
	{
		m_tdIsRunning = true;
		m_mdIsRunning = true;
		m_connected = true;
		m_clearanced = true;
		HANDLE loadMDThread = ::CreateThread(0, 0, LoadMD, (LPVOID)this, 0, 0);
		CloseHandle(loadMDThread);
		HANDLE loadTDThread = ::CreateThread(0, 0, LoadTD, (LPVOID)this, 0, 0);
		CloseHandle(loadTDThread);
		HANDLE loginedThread = ::CreateThread(0, 0, CheckLogined, (LPVOID)this, 0, 0);
		CloseHandle(loginedThread);
	}
	if(!m_checkingLogined)
	{
		m_checkingLogined = true;
		HANDLE checkThread = ::CreateThread(0, 0, CheckTradingTime, (LPVOID)this, 0, 0);
		CloseHandle(checkThread);
		HANDLE queryDataThread = ::CreateThread(0, 0, QueryData, (LPVOID)this, 0, 0);
		CloseHandle(queryDataThread);
	}
}

void MT::Start(const string &appID, const string &authCode, const string &mdServer, const string &tdServer, const string &brokerID, const string &investorID, const string &password)
{
	m_serverConfig.m_appID = appID;
	m_serverConfig.m_authCode = authCode;
	m_serverConfig.m_brokerID = brokerID;
	m_serverConfig.m_investorID = investorID;
	m_serverConfig.m_password = password;
	m_serverConfig.m_tdFronts.push_back(tdServer);
	m_serverConfig.m_mtFronts.push_back(mdServer);
	Start();
}

void MT::SubMarketData(const String& code, int reqID){
	if((int)code.length() == 0){
		return;
	}
	vector<String> codes;
	codes.push_back(code);

	SubMarketDatas(&codes, reqID);
	codes.clear();

	//目前只增不减,以后应该需要考虑
	AddInsID(code);
}

void MT::SubMarketDatas(const vector<String> *codes, int reqID){
	vector<string> sCodes;
	int codesSize = (int)codes->size();
	for(int i = 0; i < codesSize; i++){
		String wCode = (*codes)[i];
		if (AddInsID((*codes)[i])){
			string code = CStrA::wstringTostring(wCode);
			sCodes.push_back(code);
		}
	}
	if ((int)sCodes.size() > 0){
		CTPSubMarketDataQuery *subMarketDataQuery = new CTPSubMarketDataQuery();
		subMarketDataQuery->m_codes = sCodes;
		m_Lock.Lock();
		m_querys.insert(m_querys.begin(), CTPQuery(12, reqID, subMarketDataQuery));
		m_Lock.UnLock();
	}
}

void MT::UnSubMarketData(const String& code, int reqID){
	vector<String> codes;
	codes.push_back(code);
	UnSubMarketDatas(&codes, reqID);
	codes.clear();
}

void MT::UnSubMarketDatas(vector<String> *codes, int reqID){
	int codesSize = (int)codes->size();
	if(codesSize > 0){
		vector<string> sCodes;
		for(int i = 0; i < codesSize; i++){
			String wCode = (*codes)[i];
			string code = CStrA::wstringTostring(wCode);
			sCodes.push_back(code);
		}
		CTPSubMarketDataQuery *subMarketDataQuery = new CTPSubMarketDataQuery();
		subMarketDataQuery->m_codes = sCodes;
		m_Lock.Lock();
		m_querys.insert(m_querys.begin(), CTPQuery(13, reqID, subMarketDataQuery));
		m_Lock.UnLock();
	}
}

void MT::ReSubMarketData(int reqID){
	if ((int)m_allInsID.size() > 0){
		vector<string> sCodes;
		size_t sz = m_allInsID.size();
		for (size_t i = 0; i < sz; ++i){
			string code = CStrA::wstringTostring(m_allInsID[i]);
			sCodes.push_back(code);
		}
		CTPSubMarketDataQuery *subMarketDataQuery = new CTPSubMarketDataQuery();
		subMarketDataQuery->m_codes = sCodes;
		m_Lock.Lock();
		m_querys.insert(m_querys.begin(), CTPQuery(12, reqID, subMarketDataQuery));
		m_Lock.UnLock();
	}
}
