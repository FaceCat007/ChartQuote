/*
 * iChart����ϵͳ(�ǿ�Դ)
 * �Ϻ����è��Ϣ�������޹�˾
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace future
{
    public class FCStrEx {
        /// <summary>
        /// ��ȡ֤ȯ������ļ�����
        /// </summary>
        /// <param name="code">����</param>
        /// <returns>�ļ�����</returns>
        public static String convertDBCodeToFileName(String code) {
            String fileName = code;
            if (fileName.IndexOf(".") != -1) {
                fileName = fileName.Substring(fileName.IndexOf('.') + 1) + fileName.Substring(0, fileName.IndexOf('.'));
            }
            fileName += ".txt";
            return fileName;
        }

        /// <summary>
        /// ����Ʊ����ת��Ϊ���˴���
        /// </summary>
        /// <param name="code">��Ʊ����</param>
        /// <returns>���˴���</returns>
        public static String convertDBCodeToSinaCode(String code) {
            String securityCode = code;
            int index = securityCode.IndexOf(".SH");
            if (index > 0) {
                securityCode = "sh" + securityCode.Substring(0, securityCode.IndexOf("."));
            }
            else {
                securityCode = "sz" + securityCode.Substring(0, securityCode.IndexOf("."));
            }
            return securityCode;
        }

        /// <summary>
        /// ����Ʊ����ת��Ϊ��Ѷ����
        /// </summary>
        /// <param name="code">��Ʊ����</param>
        /// <returns>��Ѷ����</returns>
        public static String convertDBCodeToTencentCode(String code) {
            String securityCode = code;
            int index = securityCode.IndexOf(".");
            if (index > 0) {
                index = securityCode.IndexOf(".SH");
                if (index > 0) {
                    securityCode = "sh" + securityCode.Substring(0, securityCode.IndexOf("."));
                }
                else {
                    securityCode = "sz" + securityCode.Substring(0, securityCode.IndexOf("."));
                }
            }
            return securityCode;
        }

        /// <summary>
        /// �����˴���ת��Ϊ��Ʊ����
        /// </summary>
        /// <param name="code">���˴���</param>
        /// <returns>��Ʊ����</returns>
        public static String convertSinaCodeToDBCode(String code) {
            int equalIndex = code.IndexOf('=');
            int startIndex = code.IndexOf("var hq_str_") + 11;
            String securityCode = equalIndex > 0 ? code.Substring(startIndex, equalIndex - startIndex) : code;
            securityCode = securityCode.Substring(2) + "." + securityCode.Substring(0, 2).ToUpper();
            return securityCode;
        }

        /// <summary>
        /// ����Ѷ����ת��Ϊ��Ʊ����
        /// </summary>
        /// <param name="code">��Ѷ����</param>
        /// <returns>��Ʊ����</returns>
        public static String convertTencentCodeToDBCode(String code) {
            int equalIndex = code.IndexOf('=');
            String securityCode = equalIndex > 0 ? code.Substring(0, equalIndex) : code;
            if (securityCode.StartsWith("v_sh")) {
                securityCode = securityCode.Substring(4) + ".SH";
            }
            else if (securityCode.StartsWith("v_sz")) {
                securityCode = securityCode.Substring(4) + ".SZ";
            }
            return securityCode;
        }

        /// <summary>
        /// ��ȡ���ݿ�ת���ַ���
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns>ת���ַ���</returns>
        public static String getDBString(String str) {
            return str.Replace("'", "''");
        }
    }
}
