/*
 * iCTPMin期货行情交易(开源)
 * 上海卷卷猫信息技术有限公司
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
	//追加内容
	static bool Append(const char *file, const char *content);
	//创建目录
	static void CreateDirectory(const char *dir);
	//判断目录是否存在
	static bool IsDirectoryExist(const char *dir);
	//文件是否存在
	static bool IsFileExist(const char *file);
	//获取目录
	static bool GetDirectories(const char *dir, vector<string> *dirs);
	//获取文件长度
	static int GetFileLength(const char *file);
	//获取文件
	static bool GetFiles(const char *dir, vector<string> *files);
	//获取文件状态
	static int GetFileState(const char *file, struct stat *buf);
	//读取文件
	static bool Read(const char *file, string *content);
	//移除文件
	static void RemoveFile(const char *file);
	//写入文件
	static bool Write(const char *file, const char *content);
	//记录日志
	static void Log(string log, String Code);



};
#endif