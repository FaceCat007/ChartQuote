/*
 * iChart行情系统(开源)
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using FaceCat;

namespace chart
{
    /// <summary>
    /// 股票权值、信息
    /// </summary>
    public class SecurityOrder
    {
        /// <summary>
        /// 创建权值信息
        /// </summary>
        public SecurityOrder()
        {
        }

        /// <summary>
        /// 权值
        /// </summary>
        public int m_order;

        /// <summary>
        /// 键盘精灵
        /// </summary>
        public Security m_security;
    }

    /// <summary>
    /// 证券服务扩展
    /// </summary>
    public class SecurityService
    {
        /// <summary>
        /// 创建证券服务扩展
        /// </summary>
        public SecurityService()
        {
            
        }

        /// <summary>
        /// 证券代码列表
        /// </summary>
        private CList<CList<Security>> m_securities = new CList<CList<Security>>();

        /// <summary>
        /// 证券数据
        /// </summary>
        public Dictionary<String, Security> m_securitiesMap = new Dictionary<String, Security>();

        private bool m_loaded;

        /// <summary>
        /// 获取是否已加载
        /// </summary>
        public bool isLoaded()
        {
            return m_loaded; 
        }

        /// <summary>
        /// 补充输入后匹配股票代码
        /// </summary>
        /// <param name="code">股票代码</param>
        /// <param name="makeupCode">补充用户输入</param>
        /// <returns>是否匹配</returns>
        private bool compareMakeupSecutitycode(String code, String makeupCode) {
            if (code == makeupCode) {
                return true;
            }
            int index = strStr(code, makeupCode);
            if (index != 0) {
                return false;
            }
            if (code.Length > makeupCode.Length) {
                if (code.Substring(makeupCode.Length, 1) == ".") {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 查询键盘精灵
        /// </summary>
        /// <param name="key">检索值</param>
        /// <param name="limitSize">最大值</param>
        /// <param name="securities">返回键盘精灵集合</param>
        /// <returns>预留</returns>
        public int find(String key, int limitSize, CList<Security> securities)
        {
            int order = 0, order1 = 0, order2 = 0, order3 = 0, minOrder = 0;
            int securityListSize = m_securities.size();
            CList<SecurityOrder> securityReturnList = new CList<SecurityOrder>();
            securityReturnList.set_capacity(limitSize);
            //遍历
            for (int i = 0; i < securityListSize; i++)
            {
                CList<Security> securityList = m_securities.get(i);
                int securitySize = securityList.size();
                for (int j = 0; j < securitySize; j++)
                {
                    Security security = securityList.get(j);
                    order1 = isMatching(security.m_code, security.m_type, key, 0, i);
                    order2 = isMatching(security.m_name, security.m_type, key, 1, i);
                    order3 = isMatching(security.m_pingyin, security.m_type, key, 2, i);
                    order = Math.Max(order1, Math.Max(order2, order3));
                    if (order > 0)
                    {
                        SecurityOrder securityReturn = new SecurityOrder();
                        securityReturn.m_order = order;
                        securityReturn.m_security = security;
                        if (securityReturnList.size() < limitSize)
                        {
                            securityReturnList.push_back(securityReturn);
                        }
                        else
                        {
                            int index = 0;
                            minOrder = securityReturnList.get(0).m_order;
                            for (int m = 0; m < securityReturnList.size(); m++)
                            {
                                if (securityReturnList.get(m).m_order < minOrder)
                                {
                                    minOrder = securityReturnList.get(m).m_order;
                                    index = m;
                                }
                            }
                            if (minOrder < order)
                            {
                                securityReturnList.get(index).m_order = order;
                                securityReturnList.get(index).m_security = security;
                            }
                        }
                    }
                }
            }
            //排序
            int retListSize = securityReturnList.size();
            SecurityOrder security1 = null;
            SecurityOrder security2 = null;
            for (int i = 0; i < retListSize - 1; i++)
            {
                for (int j = i + 1; j < retListSize; j++)
                {
                    security1 = securityReturnList.get(i);
                    security2 = securityReturnList.get(j);
                    if (security1.m_order < security2.m_order
                        || (security1.m_order == security2.m_order && String.Compare(security1.m_security.m_code, security2.m_security.m_code) > 0))
                    {
                        securityReturnList.set(i, security2);
                        securityReturnList.set(j, security1);
                    }
                }
            }
            //返回
            for (int i = 0; i < retListSize; i++)
            {
                securities.push_back(securityReturnList.get(i).m_security);
            }
            securityReturnList.clear();
            return securities.size();
        }

        /// <summary>
        /// 过滤证券数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="securities">证券数据列表</param>
        /// <returns>状态</returns>
        public bool filterSecurities(String key, CList<Security> securities)
        {
            if (!m_loaded)
            {
                return false;
            }
            find(key, 10, securities);
            return true;
        }

        /// <summary>
        /// 根据代码获取证券
        /// </summary>
        /// <param name="code">证券代码</param>
        /// <param name="security">证券信息</param>
        /// <returns>是否获取成功</returns>
        public bool getSecurityByCode(String code, ref Security security)
        {
            if (!m_loaded)
            {
                return false;
            }
            String key = code.ToUpper();
            key = key.Replace("\r", "");
            key = key.Replace("\n", "");
            if (m_securitiesMap.ContainsKey(code))
            {
                security = m_securitiesMap[code];
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据代码获取证券列表
        /// </summary>
        /// <param name="codes">代码</param>
        /// <param name="securities">证券列表</param>
        /// <returns>是否获取成功</returns>
        public bool getSecuritiesByCodes(String codes, List<Security> securities)
        {
            if (!m_loaded)
            {
                return false;
            }
            String[] strCodes = codes.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int strCodesSize = strCodes.Length;
            for (int i = 0; i < strCodesSize; i++)
            {
                String code = strCodes[i].ToUpper();
                if (m_securitiesMap.ContainsKey(code))
                {
                    securities.Add(m_securitiesMap[code]);
                }
            }
            return true;
        }

        /// <summary>
        /// 加载证券
        /// </summary>
        public void load(List<Security> securitiesCache)
        {
            //预创建内存
            m_securities.set_capacity(200);
            for (int i = 0; i < 200; i++)
            {
                CList<Security> list = new CList<Security>();
                m_securities.push_back(list);
            }
            int securitiesSize = securitiesCache.Count;
            int[] capacities = new int[1000];
            for (int i = 0; i < securitiesSize; i++)
            {
                Security security = securitiesCache[i];
                capacities[security.m_type] = capacities[security.m_type] + 1;
            }
            for (int i = 0; i < 200; i++)
            {
                CList<Security> list = m_securities.get(i);
                list.set_capacity(capacities[i]);
            }
            for (int i = 0; i < securitiesSize; i++)
            {
                Security security = securitiesCache[i];
                m_securities.get(security.m_type).push_back(security);
                m_securitiesMap[security.m_code.ToUpper()] = security;
            }
            securitiesCache.Clear();
            m_loaded = true;
            capacities = null;
            System.GC.Collect();
        }

        /// <summary>
        /// 键盘精灵补充匹配
        /// </summary>
        /// <param name="code">股票代码</param>
        /// <param name="input">输入</param>
        /// <param name="type">股票类型</param>
        /// <returns>是否匹配</returns>
        private bool makeupSecurity(String code, String input, int type)
        {
            String zore = "", compare = "";
            bool ret = false;
            int len = input.Length;
            if (code == "" || input == "" || len > 5)
            {
                return ret;
            }
            int zorenum = 5 - len;
            for (int i = 0; i < zorenum; i++)
            {
                zore += "0";
            }
            //第1位补6，匹配上证A股
            if (type == 1)
            {
                compare += "6";
                compare += zore;
                compare += input;
                ret = compareMakeupSecutitycode(code, compare);
            }
            if (ret)
            {
                return ret;
            }
            compare = "";
            //第1位补3，匹配深证A股,创业板
            if (type == 2)
            {
                compare += "3";
                compare += zore;
                compare += input;
                ret = compareMakeupSecutitycode(code, compare);
            }
            if (ret)
            {
                return ret;
            }
            compare = "";
            //匹配港股
            if (len <= 4 && (type == 71 || type == 72))
            {
                compare += zore;
                compare += input;
                ret = compareMakeupSecutitycode(code, compare);
            }
            if (ret)
            {
                return ret;
            }
            compare = "";
            //匹配深证A股，中小板
            if (type == 2 && len <= 3)
            {
                compare += "002";
                zorenum = 6 - len - 3;
                for (int i = 0; i < zorenum; i++)
                {
                    compare += "0";
                }
                compare += input;
                ret = compareMakeupSecutitycode(code, compare);
            }
            return ret;
        }

        /// <summary>
        /// 键盘精灵评分
        /// </summary>
        /// <param name="securityVal">键盘精灵成员变量</param>
        /// <param name="input">用户输入</param>
        /// <param name="type">代码、名称、拼音</param>
        /// <returns>评分</returns>
        private int isMatching(String securityVal, int securityType, String input, int type, int orderIndex)
        {
            int len1 = securityVal.Length, len2 = input.Length;
            int ret = 0;
            int index = strStr(securityVal, input);
            if (index == -1)
            {
                return ret;
            }
            else if (index == 0)
            {
                ret += 2000;
                if (securityVal == input)
                {
                    ret += 6500;
                }
                if (type == 0)
                {
                    if (len1 > len2)
                    {
                        if (securityVal[len2] == '.')
                        {
                            ret += 6500;
                        }
                    }
                    if (makeupSecurity(securityVal, input, securityType))
                    {
                        ret += 3500;
                        if ((securityType == 1) && (len2 <= 3))
                        {
                            ret += 1;
                        }
                    }
                }
            }
            else
            {
                ret += 500;
                //比较代码匹配情况下有可能出现补充匹配的情况
                if (type == 0)
                {
                    if (makeupSecurity(securityVal, input, securityType))
                    {
                        ret += 5000;
                        if (securityType == 1 && len2 <= 3)
                        {
                            ret += 1;
                        }
                    }
                }
                String rest = securityVal.Substring(index);
                if (String.Compare(rest, input) == 0)
                {
                    //向后完全匹配
                    ret += 1000;
                }
                if (type == 0)
                {
                    if (rest.Length > len2)
                    {
                        if (rest[len2] == '.')
                        {
                            ret += 500;
                        }
                    }
                }
            }
            ret = ret - orderIndex * 5;
            return ret;
        }

        /// <summary>
        /// 比较字符串
        /// </summary>
        /// <param name="str1">字符串1</param>
        /// <param name="str2">字符串2</param>
        /// <returns>索引</returns>
        public int strStr(String str1, String str2)
        {
            int result = -1, i = 0, j = 0, len1 = str1.Length, len2 = str2.Length;
            if (len1 >= len2)
            {
                while (i <= len2 - 1)
                {
                    if (str1[i] == str2[j])
                    {
                        if (j == len2 - 1)
                        {
                            result = i - j;
                            break;
                        }
                        else
                        {
                            j++;
                        }
                    }
                    else
                    {
                        i -= j;
                        j = 0;
                    }
                    i++;
                }
            }
            return result;
        }
    }
}
