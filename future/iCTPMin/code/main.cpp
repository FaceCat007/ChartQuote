/*
 * iCTPMin�ڻ����齻��(��Դ)
 * �Ϻ����è��Ϣ�������޹�˾
 */

#include "stdafx.h"
#include <windows.h>
#include "Business\MyTrade.h"


int _tmain(int argc, _TCHAR* argv[])
{
	std::locale::global(std::locale(""));
	//��������
	MyTrade fycTrade;
	//��������
	fycTrade.Run("simnow_client_test", "0000000000000000", "tcp://180.168.146.187:10212", "tcp://180.168.146.187:10202", "9999", "021739", "123456");
	while(true)
	{
		string input;
		cin >> input;
		if( input == "exit" )
		{
			break;
		}
	}
	return 0;
}
