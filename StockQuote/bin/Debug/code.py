# -*- coding:utf-8 -*-
#! python3

# FaceCat-Python(OpenSource)
#Shanghai JuanJuanMao Information Technology Co., Ltd 

import math
import requests
import time
from requests.adapters import HTTPAdapter
import qstock as qs
import os
#pip install qstock

df = qs.realtime_data()
path = os.getcwd() + "\\codes.txt"
if os.path.exists(path):
	os.remove(path)
fs = open(path,'a+', encoding="UTF-8")
for i in range(0,len(df.columns)):
	print(df.columns[i])
for i in range(0, len(df)):
	sCode = df.iloc[i][df.columns[0]]
	if(sCode.find("6") == 0):
		sCode += ".SH"
	else:
		sCode += ".SZ"
	sName = df.iloc[i][df.columns[1]]
	fs.write(sCode + ',' + sName + '\n')
fs.close()