/*
 * iCTPMin期货行情交易(开源)
 * 上海卷卷猫信息技术有限公司
 */

#include "stdafx.h"
#include "CFile.h"
#include "CStr.h"

//static CLock lock;

bool CFileA::Append(const char *file, const char *content)
{
//	lock.Lock();
	fstream fs(file, ios::app);
	if(fs)
	{
		fs << content;
		fs.close();
		return true;
	}
//	lock.UnLock();
	return false;
}

void CFileA::CreateDirectory(const char *dir)
{
	_mkdir(dir);
}

bool CFileA::IsDirectoryExist(const char *dir)
{
	if( (_access(dir, 0 )) != -1 )
	{
		return true;
	}
	return false;
}

bool CFileA::IsFileExist(const char *file)
{
	fstream fs;
	fs.open(file, ios::in);
	if(fs)
	{
		fs.close();
		return true;
	}
	else
	{
		return false;
	}
}

bool CFileA::GetDirectories(const char *dir, vector<string> *dirs)
{
	long hFile = 0;  
	struct _finddata_t fileinfo;  
	string p;  
	if((hFile = (long)_findfirst(p.assign(dir).append("\\*").c_str(),&fileinfo)) !=  -1)  
	{  
		do  
		{  
			if(fileinfo.attrib &  _A_SUBDIR)  
			{  
				dirs->push_back(p.assign(dir).append("\\").append(fileinfo.name));  
			}  
		}
		while(_findnext(hFile, &fileinfo)  == 0);  
		_findclose(hFile); 
	} 
	return dirs->size() > 0;
}

int CFileA::GetFileLength(const char *file)
{
	FILE* fp = 0;
	int fileLen = 0;
	fp = fopen(file, "rb");
	if (!fp)
	{
	   return 0;
	}
	fseek(fp, 0, SEEK_END);        
	fileLen = ftell(fp);
	fclose(fp);
	return fileLen;
}

bool CFileA::GetFiles(const char *dir, vector<string> *files)
{
	long hFile = 0;  
	struct _finddata_t fileinfo;  
	string p;  
	if((hFile = (long)_findfirst(p.assign(dir).append("\\*").c_str(),&fileinfo)) !=  -1)  
	{  
		do  
		{  
			if(!(fileinfo.attrib &  _A_SUBDIR))  
			{  
				files->push_back(p.assign(dir).append("\\").append(fileinfo.name));  
			}  
		}
		while(_findnext(hFile, &fileinfo)  == 0);  
		_findclose(hFile); 
	} 
	return files->size() > 0;
}

int CFileA::GetFileState(const char *file, struct stat *buf)
{
	return stat(file, buf);
}

bool CFileA::Read(const char *file, string *content)
{
	if(CFileA::IsFileExist(file))
	{
		int fileLength = GetFileLength(file);
		char *szContent = new char[fileLength + 1];
		memset(szContent, '\0', fileLength + 1);
		ifstream fs(file, ios::in); 
		if(fs)
		{
			while(!fs.eof())
			{
				fs.read(szContent, fileLength); 
			}
			fs.close();
		}
		*content = szContent;
		delete[] szContent;
		szContent = 0;
		return true;
	}
	return false;
}

void CFileA::RemoveFile(const char *file)
{
	if(CFileA::IsFileExist(file))
	{
		String wFile = CStrA::stringTowstring(file);
		::DeleteFile(wFile.c_str());
	}
}

bool CFileA::Write(const char *file, const char *content)
{
	fstream fs(file, ios::out);
	if(fs)
	{
		fs << content;
		fs.close();
		return true;
	}
	return false;
}

//WQ
void CFileA::Log(string log, String Code)
{
	SYSTEMTIME st = {0};
	GetLocalTime(&st);

	char logmsg[1024];
	sprintf_s(logmsg, "[%02d:%02d:%02d %03d] %s\r\n", st.wHour, st.wMinute, st.wSecond, st.wMilliseconds, log.c_str());

	char filename[100];
	sprintf_s(filename, ".\\Log\\%ws_%d%02d%02d.txt", Code.c_str(), st.wYear, st.wMonth, st.wDay);
	Append(filename, logmsg);

}

