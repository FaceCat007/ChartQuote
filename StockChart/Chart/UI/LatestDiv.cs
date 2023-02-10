/*
 * iChart行情系统(开源)
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using FaceCat;


namespace chart {
    /// <summary>
    /// 实时数据
    /// </summary>
    public class LatestDiv : FCView, FCListenerMessageCallBack, IOutReWrite
    {
        /// <summary>
        /// 创建控件
        /// </summary>
        public LatestDiv() {
            setBackColor(MyColor.USERCOLOR64);
            setBorderColor(FCColor.None);
        }

        /// <summary>
        /// 买卖文字
        /// </summary>
        private List<String> m_buySellStrs = new List<String>();

        /// <summary>
        /// 最新数据请求编号
        /// </summary>
        private int m_requestID = FCClientService.getRequestID();

        private MainFrame m_mainFrame;

        /// <summary>
        /// 获取或设置行情控件
        /// </summary>
        public MainFrame getMainFrame() {
            return m_mainFrame;
        }

        /// <summary>
        /// 设置行情控件
        /// </summary>
        /// <param name="value"></param>
        public void setMainFrame(MainFrame value) {
            m_mainFrame = value;
        }

        private int m_digit = 2;

        /// <summary>
        /// 获取保留小数的位数
        /// </summary>
        public int getDigit() {
            return m_digit;
        }

        /// <summary>
        /// 设置保留小数的位数
        /// </summary>
        /// <param name="value"></param>
        public void setDigit(int value) {
            m_digit = value; 
        }

        private SecurityLatestData m_latestData = new SecurityLatestData();

        /// <summary>
        /// 获取实时数据
        /// </summary>
        public SecurityLatestData getLatestData() {
            return m_latestData;
        }

        private bool m_lV2;

        /// <summary>
        /// 获取或设置是否LV2
        /// </summary>
        public bool isLV2() {
            return m_lV2;
        }

        /// <summary>
        /// 设置是否LV2
        /// </summary>
        /// <param name="value"></param>
        public void setLV2(bool value) {
            m_lV2 = value;
        }

        private String m_securityCode;

        /// <summary>
        /// 获取股票代码
        /// </summary>
        public String getSecurityCode() {
            return m_securityCode; 
        }

        /// <summary>
        /// 设置股票代码
        /// </summary>
        /// <param name="value"></param>
        public void setSecurityCode(String value) {
            m_latestData = new SecurityLatestData();
            m_securityCode = value;
            DataCenter.m_latestServiceClient.unSubCodes(m_requestID);
            DataCenter.m_latestServiceClient.removeListener(m_requestID);
            m_requestID = FCClientService.getRequestID();
            DataCenter.m_latestServiceClient.addListener(m_requestID, this, null);
            DataCenter.m_latestServiceClient.subCodes(m_requestID, m_securityCode);
            update();
        }

        private String m_securityName;

        /// <summary>
        /// 获取或设置股票名称
        /// </summary>
        public String getSecurityName() {
            return m_securityName;
        }

        public void setSecurityName(String value) {
            m_securityName = value;
        }

        /// <summary>
        /// 最新数据的回调
        /// </summary>
        /// <param name="message">消息</param>
        public void callListenerMessageEvent(Object sender, FCMessage message, Object invoke) {
            if (message.m_requestID == m_requestID) {
                FCMessage copyMessage = new FCMessage();
                copyMessage.copy(message);
                beginInvoke(copyMessage);
            }
        }

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns>最大值</returns>
        public double max(List<double> list) {
            double max = 0;
            for (int i = 0; i < list.Count; i++) {
                if (i == 0) {
                    max = list[i];
                }
                else {
                    if (list[i] > max) {
                        max = list[i];
                    }
                }
            }
            return max;
        }


        /// <summary>
        /// 调用控件线程方法
        /// </summary>
        /// <param name="args">参数</param>
        public override void onInvoke(object args) {
            base.onInvoke(args);
            if (args != null) {
                FCMessage message = (FCMessage)args;
                if (message.m_requestID == m_requestID) {
                    //分时数据
                    if (message.m_functionID == LatestServiceClient.FUNCTION_NEWDATA) {
                        SecurityLatestData latestData = LatestServiceClient.getLatestData(message.m_body, message.m_bodyLength);
                        if (latestData != null && latestData.m_code == m_securityCode &&
                        !latestData.equal(m_latestData)) {
                            m_latestData = latestData;
                            //设置保留小数的位数
                            int digit = 2;
                            if (m_latestData.m_code.StartsWith("1") || m_latestData.m_code.StartsWith("5")) {
                                digit = 3;
                            }
                            m_mainFrame.setDigit(digit);
                            m_mainFrame.refreshData();
                        }
                    }
                }
                invalidate();
            }
        }

        /// <summary>
        /// 绘制前景方法
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        public override void onPaintForeground(FCPaint paint, FCRect clipRect) {
            int width = getWidth();
            int height = getHeight();
            if (width > 0 && height > 0) {
                FCFont font = new FCFont("Default", 14, false, false, false);
                FCFont lfont = new FCFont("Default", 12, false, false, false);
                long wordColor = MyColor.USERCOLOR100;
                int top = 32, step = 20;
                //画买卖盘
                FCDraw.drawText(paint, "卖", wordColor, font, 1, (m_lV2 ? 87 : 47));
                FCDraw.drawText(paint, "盘", wordColor, font, 1, (m_lV2 ? 140 : 100));
                FCDraw.drawText(paint, "买", wordColor, font, 1, (m_lV2 ? 267 : 147));
                FCDraw.drawText(paint, "盘", wordColor, font, 1, (m_lV2 ? 310 : 200));
                String buySellStr = "5,4,3,2,1,1,2,3,4,5";
                if (m_lV2) {
                    step = 16;
                    buySellStr = "总卖量,10,9,8,7,6," + buySellStr + ",6,7,8,9,10,总买量";
                    font.m_fontSize = 12;
                }
                String[] buySellStrs = buySellStr.Split(',');
                int strsSize = buySellStrs.Length;
                for (int i = 0; i < strsSize; i++) {
                    FCDraw.drawText(paint, buySellStrs[i], wordColor, font, 25, top);
                    top += step;
                }
                font.m_fontSize = 14;
                top = m_lV2 ? 390 : 232;
                FCDraw.drawText(paint, "最新", wordColor, font, 1, top);
                FCDraw.drawText(paint, "升跌", wordColor, font, 1, top + 20);
                FCDraw.drawText(paint, "幅度", wordColor, font, 1, top + 40);
                FCDraw.drawText(paint, "总手", wordColor, font, 1, top + 60);
                FCDraw.drawText(paint, "涨停", wordColor, font, 1, top + 80);
                FCDraw.drawText(paint, "外盘", wordColor, font, 1, top + 100);
                FCDraw.drawText(paint, "开盘", wordColor, font, 110, top);
                FCDraw.drawText(paint, "最高", wordColor, font, 110, top + 20);
                FCDraw.drawText(paint, "最低", wordColor, font, 110, top + 40);
                FCDraw.drawText(paint, "换手", wordColor, font, 110, top + 60);
                FCDraw.drawText(paint, "跌停", wordColor, font, 110, top + 80);
                FCDraw.drawText(paint, "内盘", wordColor, font, 110, top + 100);
                //画股票代码
                long yellowColor = MyColor.USERCOLOR73;
                if (m_latestData.m_code != null && m_latestData.m_code.Length > 0) {
                    double close = m_latestData.m_close, open = m_latestData.m_open, high = m_latestData.m_high, low = m_latestData.m_low, lastClose = m_latestData.m_lastClose;
                    if (close == 0) {
                        if (m_latestData.m_buyPrice1 > 0) {
                            close = m_latestData.m_buyPrice1;
                            open = m_latestData.m_buyPrice1;
                            high = m_latestData.m_buyPrice1;
                            low = m_latestData.m_buyPrice1;
                        }
                        else if (m_latestData.m_sellPrice1 > 0) {
                            close = m_latestData.m_sellPrice1;
                            open = m_latestData.m_sellPrice1;
                            high = m_latestData.m_sellPrice1;
                            low = m_latestData.m_sellPrice1;
                        }
                    }
                    if (lastClose == 0) {
                        lastClose = close;
                    }
                    List<double> plist = new List<double>();
                    List<double> vlist = new List<double>();
                    if (m_lV2) {
                    }
                    plist.Add(m_latestData.m_sellPrice5);
                    plist.Add(m_latestData.m_sellPrice4);
                    plist.Add(m_latestData.m_sellPrice3);
                    plist.Add(m_latestData.m_sellPrice2);
                    plist.Add(m_latestData.m_sellPrice1);
                    vlist.Add(m_latestData.m_sellVolume5);
                    vlist.Add(m_latestData.m_sellVolume4);
                    vlist.Add(m_latestData.m_sellVolume3);
                    vlist.Add(m_latestData.m_sellVolume2);
                    vlist.Add(m_latestData.m_sellVolume1);
                    plist.Add(m_latestData.m_buyPrice1);
                    plist.Add(m_latestData.m_buyPrice2);
                    plist.Add(m_latestData.m_buyPrice3);
                    plist.Add(m_latestData.m_buyPrice4);
                    plist.Add(m_latestData.m_buyPrice5);
                    vlist.Add(m_latestData.m_buyVolume1);
                    vlist.Add(m_latestData.m_buyVolume2);
                    vlist.Add(m_latestData.m_buyVolume3);
                    vlist.Add(m_latestData.m_buyVolume4);
                    vlist.Add(m_latestData.m_buyVolume5);
                    if (m_lV2) {
                    }
                    long color = 0;
                    double mx = max(vlist);
                    font.m_fontSize = m_lV2 ? 14 : 14;
                    if (mx > 0) {
                        //绘制买卖盘
                        int pLength = plist.Count;
                        top = 32;
                        if (m_lV2) {
                            top += step;
                        }
                        for (int i = 0; i < pLength; i++) {
                            color = DataCenter.getPriceColor(plist[i], lastClose);
                            FCDraw.drawUnderLineNum(paint, plist[i], m_digit, font, color, true, m_lV2 ? 80 : 60, top);
                            FCDraw.drawUnderLineNum(paint, vlist[i], 0, font, yellowColor, false, m_lV2 ? 130 : 110, top);
                            paint.fillRect(color, new FCRect(width - (int)(vlist[i] / mx * 50), top + step / 2 - 2, width, top + step / 2 + 2));
                            top += step;
                        }
                        if (m_lV2) {
                            top += step;
                        }
                    }
                    vlist.Clear();
                    plist.Clear();
                    top = m_lV2 ? 390 : 232;
                    //成交
                    color = DataCenter.getPriceColor(close, lastClose);
                    FCDraw.drawUnderLineNum(paint, close, m_digit, font, color, true, 45, top);
                    //升跌
                    double sub = 0;
                    if (close == 0) {
                        sub = m_latestData.m_buyPrice1 - lastClose;
                        double rate = 100 * (m_latestData.m_buyPrice1 - lastClose) / lastClose;
                        int pleft = FCDraw.drawUnderLineNum(paint, rate, 2, font, color, false, 45, top + 40);
                        FCDraw.drawText(paint, "%", color, font, pleft + 47, top + 40);
                    }
                    else {
                        sub = close - m_latestData.m_lastClose;
                        double rate = 100 * (close - lastClose) / lastClose;
                        int pleft = FCDraw.drawUnderLineNum(paint, rate, 2, font, color, false, 45, top + 40);
                        FCDraw.drawText(paint, "%", color, font, pleft + 47, top + 40);
                    }
                    FCDraw.drawUnderLineNum(paint, sub, m_digit, font, color, false, 45, top + 20);
                    double volume = m_latestData.m_volume / 100;
                    String unit = "";
                    if (volume > 100000000) {
                        volume /= 100000000;
                        unit = "亿";
                    }
                    else if (volume > 10000) {
                        volume /= 10000;
                        unit = "万";
                    }
                    //总手
                    int cleft = FCDraw.drawUnderLineNum(paint, volume, unit.Length > 0 ? m_digit : 0, font, yellowColor, true, 45, top + 60);
                    if (unit.Length > 0) {
                        FCDraw.drawText(paint, unit, yellowColor, font, cleft + 47, top + 60);
                    }
                    //换手
                    double turnoverRate = m_latestData.m_turnoverRate;
                    cleft = FCDraw.drawUnderLineNum(paint, turnoverRate, 2, font, yellowColor, true, 155, top + 60);
                    if (turnoverRate > 0) {
                        FCDraw.drawText(paint, "%", yellowColor, font, cleft + 157, top + 60);
                    }
                    //开盘
                    color = DataCenter.getPriceColor(open, lastClose);
                    FCDraw.drawUnderLineNum(paint, open, m_digit, font, color, true, 155, top);
                    //最高
                    color = DataCenter.getPriceColor(high, lastClose);
                    FCDraw.drawUnderLineNum(paint, high, m_digit, font, color, true, 155, top + 20);
                    //最低
                    color = DataCenter.getPriceColor(low, lastClose);
                    FCDraw.drawUnderLineNum(paint, low, m_digit, font, color, true, 155, top + 40);
                    //涨停
                    double upPrice = lastClose * 1.1;
                    if (m_securityName != null && m_securityName.Length > 0) {
                        if (m_securityName.StartsWith("ST") || m_securityName.StartsWith("*ST")) {
                            upPrice = lastClose * 1.05;
                        }
                    }
                    FCDraw.drawUnderLineNum(paint, upPrice, m_digit, font, MyColor.USERCOLOR37, true, 45, top + 80);
                    //跌停
                    double downPrice = lastClose * 0.9;
                    if (m_securityName != null && m_securityName.Length > 0) {
                        if (m_securityName.StartsWith("ST") || m_securityName.StartsWith("*ST")) {
                            downPrice = lastClose * 0.95;
                        }
                    }
                    FCDraw.drawUnderLineNum(paint, downPrice, m_digit, font, MyColor.USERCOLOR97, true, 155, top + 80);
                    //外盘
                    double outerVol = m_latestData.m_outerVol;
                    unit = "";
                    if (outerVol > 100000000) {
                        outerVol /= 100000000;
                        unit = "亿";
                    }
                    else if (outerVol > 10000) {
                        outerVol /= 10000;
                        unit = "万";
                    }
                    cleft = FCDraw.drawUnderLineNum(paint, outerVol, unit.Length > 0 ? m_digit : 0, font, MyColor.USERCOLOR37, false, 45, top + 100);
                    if (unit.Length > 0) {
                        FCDraw.drawText(paint, unit, MyColor.USERCOLOR37, font, cleft + 47, top + 100);
                    }
                    unit = "";
                    double innerVol = m_latestData.m_innerVol;
                    if (innerVol > 100000000) {
                        innerVol /= 100000000;
                        unit = "亿";
                    }
                    else if (innerVol > 10000) {
                        innerVol /= 10000;
                        unit = "万";
                    }
                    //内盘
                    cleft = FCDraw.drawUnderLineNum(paint, innerVol, unit.Length > 0 ? m_digit : 0, font, MyColor.USERCOLOR97, true, 155, top + 100);
                    if (unit.Length > 0) {
                        FCDraw.drawText(paint, unit, MyColor.USERCOLOR97, font, cleft + 157, top + 100);
                    }
                }
                font.m_bold = false;
                font.m_fontSize = 18;
                //股票代码
                if (m_securityCode != null && m_securityCode.Length > 0) {
                    FCDraw.drawText(paint, m_securityCode, MyColor.USERCOLOR3, font, 2, 4);
                }
                //股票名称
                if (m_securityName != null && m_securityName.Length > 0) {
                    FCDraw.drawText(paint, m_securityName, MyColor.USERCOLOR3, font, 100, 4);
                }
            }
        }

        public FCView createView(UIXmlEx uiXmlEx, System.Xml.XmlNode node)
        {
            return new LatestDiv();
        }

        public bool moreAnalysis(FCView view, UIXmlEx uiXmlEx, System.Xml.XmlNode node)
        {
            return false;
        }
    }
}
