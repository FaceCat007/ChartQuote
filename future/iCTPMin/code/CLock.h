/*
 * iCTPMin�ڻ����齻��(��Դ)
 * �Ϻ����è��Ϣ�������޹�˾
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
