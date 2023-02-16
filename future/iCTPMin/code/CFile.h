/*
 * iCTPMin�ڻ����齻��(��Դ)
 * �Ϻ����è��Ϣ�������޹�˾
 */

#ifndef __CFileA_H__
#define __CFileA_H__
#pragma once
#include "stdafx.h"
#include "io.h"
#include <direct.h>
#include <fstream>
#include <sys/stat.h>

class CFileA
{
public:
	//׷������
	static bool Append(const char *file, const char *content);
	//����Ŀ¼
	static void CreateDirectory(const char *dir);
	//�ж�Ŀ¼�Ƿ����
	static bool IsDirectoryExist(const char *dir);
	//�ļ��Ƿ����
	static bool IsFileExist(const char *file);
	//��ȡĿ¼
	static bool GetDirectories(const char *dir, vector<string> *dirs);
	//��ȡ�ļ�����
	static int GetFileLength(const char *file);
	//��ȡ�ļ�
	static bool GetFiles(const char *dir, vector<string> *files);
	//��ȡ�ļ�״̬
	static int GetFileState(const char *file, struct stat *buf);
	//��ȡ�ļ�
	static bool Read(const char *file, string *content);
	//�Ƴ��ļ�
	static void RemoveFile(const char *file);
	//д���ļ�
	static bool Write(const char *file, const char *content);
	//��¼��־
	static void Log(string log, String Code);



};
#endif