/*
 * iCTPMin期货行情交易(开源)
 * 上海卷卷猫信息技术有限公司
 */

#include "stdafx.h"
#include "CStr.h"

CodeConvert_Win::CodeConvert_Win( const char* input, unsigned int fromCodePage, unsigned int toCodePage )
{
	int len = MultiByteToWideChar(fromCodePage, 0, input, -1, 0, 0);
	wcharBuf = new wchar_t[len + 1];
	memset(wcharBuf,0,sizeof(wchar_t)*(len + 1));
	MultiByteToWideChar(fromCodePage, 0, input, -1, wcharBuf, len);
	len = WideCharToMultiByte(toCodePage, 0, wcharBuf, -1, 0, 0, 0, 0);
	charBuf = new char[len + 1];
	memset(charBuf,0,sizeof(char)*(len + 1));
	WideCharToMultiByte(toCodePage, 0, wcharBuf, -1, charBuf, len, 0, 0);
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

double CStrA::round(double x) 
{     
	int sa = 0, si = 0;  
	if(x == 0.0)   
	{
		return 0.0; 
	}
	else    
	{
		if(x > 0.0)  
		{
			sa = (int)x;   
			si = (int)(x + 0.5);        
			if(sa == floor((double)si))   
			{
				return sa;    
			}
			else     
			{
				return sa + 1; 
			}
		}       
		else    
		{
			sa = (int)x;   
			si = (int)(x - 0.5);      
			if(sa == ceil((double)si))  
			{
				return sa;       
			}
			else         
			{
				return sa - 1;      
			}
		}
	}
}

void CStrA::ANSCToUnicode(string& out, const char* inSrc)
{
	if (!inSrc)
	{
		return ;
	}
	out = CodeConvert_Win(inSrc, CP_UTF8, CP_ACP).ToString();
}

//int CStrA::Compress(Byte *data, uLong ndata, Byte *zdata, uLong *nzdata)
//{
//	return compress(zdata, nzdata, data, ndata);
//}

void CStrA::Contact(wchar_t *str, LPCTSTR str1, LPCTSTR str2, LPCTSTR str3)
{
	str[0] = _T('\0');
	lstrcat(str, str1);
	if(lstrlen(str2) > 0)
	{
		lstrcat(str, str2);
	}
	if(lstrlen(str3) > 0)
	{
		lstrcat(str, str3);
	}
}

String CStrA::ConvertThousands(double value, int digit)
{
	if(digit == 0)
	{
		double newValue = round(value);
		if(abs(newValue - value) < 1)
		{
			value = newValue;
		}
	}
	wchar_t szValue[100] = {0};
	_stprintf_s(szValue, 99, L"%I64d", (_int64)abs(value));
	String str = szValue;
	int strSize = (int)str.size();
	String result = L"";
	for(int i = 0; i < strSize; i++)
	{
		result = str[strSize - i - 1] + result;
		if(i != strSize - 1 && (i > 0 && (i + 1) % 3 == 0))
		{
			result = L"," + result;
		}
	}
	if(value < 0)
	{
		result = L"-" + result;
	}
	if(digit > 0)
	{
		GetValueByDigit(value, digit, szValue);
		String dszValue = szValue;
		if(dszValue.find(L".") != -1)
		{
			result += L"." + dszValue.substr(dszValue.find(L".") + 1);
		}
	}
	return result;
}

string CStrA::ConvertDBCodeToFileName(const string& code)
{
	string fileName = code;
	if (fileName.find(".") != -1)
    {
        fileName = fileName.substr(fileName.find('.') + 1) + fileName.substr(0, fileName.find('.'));
    }
	fileName += ".txt";
	return fileName;
}

string CStrA::ConvertDBCodeToSinaCode(const string& code)
{
	string securityCode = code;
	int index = (int)securityCode.find(".SH");
    if (index > 0)
    {
        securityCode = "sh" + securityCode.substr(0, securityCode.find("."));
    }
    else
    {
        securityCode = "sz" + securityCode.substr(0, securityCode.find("."));
    }
	return securityCode;
}

string CStrA::ConvertDBCodeToTencentCode(const string& code)
{
	string securityCode = code;
	int index = (int)securityCode.find(".");
	if(index > 0)
	{
		index = (int)securityCode.find(".SH");
		if (index > 0)
		{
			securityCode = "sh" + securityCode.substr(0, securityCode.find("."));
		}
		else
		{
			securityCode = "sz" + securityCode.substr(0, securityCode.find("."));
		}
	}
	return securityCode;
}

String CStrA::ConvertSinaCodeToDBCode(const String& code)
{
	int equalIndex = (int)code.find(L"=");
	int startIndex = (int)code.find(L"var hq_str_") + 11;
	String securityCode = equalIndex > 0 ?code.substr(startIndex, equalIndex - startIndex) : code;
	if (securityCode.find(L"sh") == 0 || securityCode.find(L"SH") == 0)
	{
		securityCode = securityCode.substr(2) + L".SH";
	}
	else if (securityCode.find(L"sz") == 0 || securityCode.find(L"SZ") == 0)
	{
		securityCode = securityCode.substr(2) + L".SZ";
	}
	return securityCode;
}

double CStrA::ConvertStrToDouble(const wchar_t *str)
{
	return _wtof(str);
}

int CStrA::ConvertStrToInt(const wchar_t *str)
{
	return _wtoi(str);
}

String CStrA::ConvertTencentCodeToDBCode(const String& code)
{
	int equalIndex = (int)code.find(L'=');
	String securityCode = equalIndex > 0 ? code.substr(0, equalIndex) : code;
	if (securityCode.find(L"v_sh") == 0)
	{
		securityCode = securityCode.substr(4) + L".SH";
	}
	else if (securityCode.find(L"v_sz") == 0)
	{
		securityCode = securityCode.substr(4) + L".SZ";
	}
	return securityCode;
}

//int CStrA::DeCompress(Byte *zdata, uLong nzdata, Byte *data, uLong *ndata)
//{
//	return uncompress(data, ndata, zdata, nzdata);
//}

String CStrA::GetDBString(const String& strSrc)
{
	String str = Replace(strSrc, L"'", L"''");
	return str;
}

string CStrA::GetGuid()
{
	static char buf[64] = {0};
	GUID guid;
	if (S_OK == ::CoCreateGuid(&guid))
	{
		_snprintf(buf, sizeof(buf)
			, "{%08X-%04X-%04x-%02X%02X-%02X%02X%02X%02X%02X%02X}"
			, guid.Data1
			, guid.Data2
			, guid.Data3
			, guid.Data4[0], guid.Data4[1]
		, guid.Data4[2], guid.Data4[3], guid.Data4[4], guid.Data4[5]
		, guid.Data4[6], guid.Data4[7]
		);
	}
	return buf;
}

void CStrA::GetFormatDate(double date, wchar_t *str)
{

	int year = 0,month = 0,day = 0,hour = 0,minute = 0,second = 0,msecond = 0;
	M130(date, &year, &month, &day, &hour, &minute, &second, &msecond);
	_stprintf_s(str, 99, L"%d/%d/%d", year, month, day);
}

int CStrA::GetStringCapacity(const string& str)
{
	return (int)str.length() + 1 + sizeof(_int64);
}

int CStrA::GetWStringCapacity(const String& str)
{
	return ((int)str.length() + 1) * 2 + sizeof(_int64);
}

void CStrA::GetValueByDigit(double value, int digit, wchar_t *str)
{
	if(digit == 0)
	{
		double newValue = round(value);
		if(abs(newValue - value) < 1)
		{
			value = newValue;
		}
	}
	switch(digit)
	{
	case 1:
		_stprintf_s(str, 99, L"%.f", value);
		break;
	case 2:
		_stprintf_s(str, 99, L"%.2f", value);
		break;
	case 3:
		_stprintf_s(str, 99, L"%.3f", value);
		break;
	case 4:
		_stprintf_s(str, 99, L"%.4f", value);
		break;
	case 5:
		_stprintf_s(str, 99, L"%.5f", value);
		break;
	default:
		_stprintf_s(str, 99, L"%d", (long)value);
	}
	str = 0;
}

double CStrA::M129(int tm_year, int tm_mon, int tm_mday, int tm_hour, int tm_min, int tm_sec, int tm_msec)
{
	struct tm t;
	time_t tn;
	t.tm_sec = tm_sec;
	t.tm_min = tm_min;
	t.tm_hour = tm_hour;
	t.tm_mday = tm_mday;
	t.tm_mon = tm_mon - 1;
	t.tm_year = tm_year - 1900;
	tn = mktime(&t);
	return (double)tn + (double)tm_msec / 1000 + 28800;
}

void CStrA::M130(double num, int *tm_year, int *tm_mon, int *tm_mday, int *tm_hour, int *tm_min, int *tm_sec, int *tm_msec)
{
	time_t tn = (_int64)num;
	struct tm* t = gmtime(&tn);
	*tm_sec = t->tm_sec;
	*tm_min = t->tm_min;
	*tm_hour = t->tm_hour;
	*tm_mday = t->tm_mday;
	*tm_mon = t->tm_mon + 1;
	*tm_year = t->tm_year + 1900;
	*tm_msec = (int)((num * 1000 - floor(num) * 1000));
}

string CStrA::Replace(const string& str, const string& src, const string& dest)
{
	string newStr = str;
	std::string::size_type pos = 0;
	while( (pos = newStr.find(src, pos)) != string::npos )
	{
		newStr.replace(pos, src.length(), dest);
		pos += dest.length();
	}
	return newStr;
}

String CStrA::Replace(const String& str, const String& src, const String& dest)
{
	String newStr = str;
	String::size_type pos = 0;
	while( (pos = newStr.find(src, pos)) != string::npos )
	{
		newStr.replace(pos, src.length(), dest);
		pos += dest.length();
	}
	return newStr;
}

vector<string> CStrA::Split(const string& str, const string& pattern)
{
	 int pos = -1;
	 vector<string> result;
	 string newStr = str;
	 newStr += pattern;
	 int size = (int)str.size();
	 for(int i = 0; i < size; i++)
	 {
		pos = (int)newStr.find(pattern, i);
		if((int)pos <= size)
		{
			string s = newStr.substr(i, pos - i);
			if(s.length() > 0)
			{
				result.push_back(s);
			}
			i = pos + (int)pattern.size() - 1;
		}
	}
	return result;
}

vector<String> CStrA::Split(const String& str, const String& pattern)
{
	 int pos = -1;
	 vector<String> result;
	 String newStr = str;
	 newStr += pattern;
	 int size = (int)str.size();
	 for(int i = 0; i < size; i++)
	 {
		pos = (int)newStr.find(pattern, i);
		if((int)pos <= size)
		{
			String s = newStr.substr(i, pos - i);
			result.push_back(s);
			i = pos + (int)pattern.size() - 1;
		}
	}
	return result;
}

String CStrA::stringTowstring( const string& strSrc)
{
	String strDest;
	int  unicodeLen = ::MultiByteToWideChar(CP_ACP, 0, strSrc.c_str(), -1, 0, 0);
	wchar_t *pUnicode = new  wchar_t[unicodeLen + 1];
	memset(pUnicode,0,(unicodeLen + 1) * sizeof(wchar_t));
	::MultiByteToWideChar(CP_ACP, 0, strSrc.c_str(), - 1, (LPWSTR)pUnicode, unicodeLen);
	strDest = ( wchar_t* )pUnicode;
	delete[] pUnicode;
	return strDest;
}

String CStrA::ToLower(const String& str)
{
	String lowerStr = str;
	transform(lowerStr.begin(), lowerStr.end(), lowerStr.begin(), tolower);
	return lowerStr;
}

String CStrA::ToUpper(const String& str)
{
	String upperStr = str;
	transform(upperStr.begin(), upperStr.end(), upperStr.begin(), toupper);
	return upperStr;
}

void CStrA::UnicodeToANSC(string& out, const char* inSrc)
{
	if (!inSrc)
	{
		return ;
	}
	out = CodeConvert_Win(inSrc, CP_ACP, CP_UTF8).ToString();
}

string CStrA::wstringTostring(const String& strSrc)
{
	string strDest;
	int iTextLen = WideCharToMultiByte(CP_ACP, 0, strSrc.c_str(), -1, NULL, 0, NULL, NULL);
	char *pElementText = new char[iTextLen + 1];
	memset( ( void* )pElementText, 0, sizeof( char ) * ( iTextLen + 1 ) );
	::WideCharToMultiByte(CP_ACP, 0, strSrc.c_str(), - 1, pElementText, iTextLen, 0, 0);
	strDest = pElementText;
	delete[] pElementText;
	return strDest;
}