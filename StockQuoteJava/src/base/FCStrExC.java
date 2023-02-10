/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package base;

/**
 *
 * 字符串转换服务
 */
public class FCStrExC {
    /*
    * 获取证券代码的文件名称
    * @param code 代码
    * @return 文件名称
    */
    public static String convertDBCodeToFileName(String code) {
        String fileName = code;
        if (fileName.indexOf(".") != -1) {
            fileName = fileName.substring(fileName.indexOf('.') + 1) + fileName.substring(0, fileName.indexOf('.'));
        }
        fileName += ".txt";
        return fileName;
    }

    /*
    * 将股票代码转化为新浪代码
    * @param code 股票代码
    * @return 新浪代码
    */
    public static String convertDBCodeToSinaCode(String code) {
        String securityCode = code;
        int index = securityCode.indexOf(".SH");
        if (index > 0) {
            securityCode = "sh" + securityCode.substring(0, securityCode.indexOf("."));
        }
        else {
            securityCode = "sz" + securityCode.substring(0, securityCode.indexOf("."));
        }
        return securityCode;
    }

    /*
    * 将股票代码转化为腾讯代码
    * @param code 股票代码
    * @return 腾讯代码
    */
    public static String convertDBCodeToTencentCode(String code) {
        String securityCode = code;
        int index = securityCode.indexOf(".");
        if (index > 0) {
            index = securityCode.indexOf(".SH");
            if (index > 0) {
                securityCode = "sh" + securityCode.substring(0, securityCode.indexOf("."));
            }
            else {
                securityCode = "sz" + securityCode.substring(0, securityCode.indexOf("."));
            }
        }
        return securityCode;
    }

    /*
    * 将新浪代码转化为股票代码
    * @param code 新浪代码
    * @return 股票代码
    */
    public static String convertSinaCodeToDBCode(String code) {
        int equalIndex = code.indexOf('=');
        int startIndex = code.indexOf("var hq_str_") + 11;
        String securityCode = equalIndex > 0 ? code.substring(startIndex, equalIndex) : code;
        securityCode = securityCode.substring(2) + "." + securityCode.substring(0, 2).toUpperCase();
        return securityCode;
    }

    /*
    * 将腾讯代码转化为股票代码
    * @param code 腾讯代码
    * @return 股票代码
    */
    public static String convertTencentCodeToDBCode(String code) {
        int equalIndex = code.indexOf('=');
        String securityCode = equalIndex > 0 ? code.substring(0, equalIndex) : code;
        if (securityCode.startsWith("v_sh")) {
            securityCode = securityCode.substring(4) + ".SH";
        }
        else if (securityCode.startsWith("v_sz")) {
            securityCode = securityCode.substring(4) + ".SZ";
        }
        return securityCode;
    }

    /*
    * 获取数据库转义字符串
    * @param str 字符串
    * @return 转义字符串
    */
    public static String getDBString(String str) {
        return str.replace("'", "''");
    }
}
