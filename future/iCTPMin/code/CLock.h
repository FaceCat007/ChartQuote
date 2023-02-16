/*
 * iCTPMin期货行情交易(开源)
 * 上海卷卷猫信息技术有限公司
 */

#ifndef __CLOCK__H__
#define __CLOCK__H__
#pragma once

class CLock
{
public:
	CLock();
	~CLock();
	void Lock();
	void UnLock();
private:
	CRITICAL_SECTION m_cs;
};

#endif
