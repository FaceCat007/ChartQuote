/*
 * iCTPMin期货行情交易(开源)
 * 上海卷卷猫信息技术有限公司
 */

#include "stdafx.h"
#include "CLock.h"

CLock::CLock()
{
	::InitializeCriticalSection(&m_cs);
}

CLock::~CLock()
{
	::DeleteCriticalSection(&m_cs);
}

void CLock::Lock()
{
	::EnterCriticalSection(&m_cs);
}

void CLock::UnLock()
{
	::LeaveCriticalSection(&m_cs);
}
