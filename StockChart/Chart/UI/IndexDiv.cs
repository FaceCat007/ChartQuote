/*
 * iChart行情系统(开源)
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FaceCat;


namespace chart {
    /// <summary>
    /// 指数栏
    /// </summary>
    public class IndexDiv : FCView, FCListenerMessageCallBack,  IOutReWrite {
        /// <summary>
        /// 创建控件
        /// </summary>
        public IndexDiv() {
            setBackColor(FCColor.Back);
            setBorderColor(MyColor.USERCOLOR96);
            DataCenter.m_priceDataServiceClient.addListener(m_requestID, this, null);
        }

        /// <summary>
        /// 创业板指数数据
        /// </summary>
        private PriceData m_cyLatestData = new PriceData();

        /// <summary>
        /// 请求编号
        /// </summary>
        private int m_requestID = FCClientService.getRequestID();

        /// <summary>
        /// 上证指数数据
        /// </summary>
        private PriceData m_ssLatestData = new PriceData();

        /// <summary>
        /// 深证指数数据
        /// </summary>
        private PriceData m_szLatestData = new PriceData();

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

        /// <summary>
        /// 最新的数据的回调
        /// </summary>
        /// <param name="message">消息</param>
        public void callListenerMessageEvent(Object sender, FCMessage message, Object invoke) {
            FCMessage copyMessage = new FCMessage();
            copyMessage.copy(message);
            beginInvoke(copyMessage);
        }

        /// <summary>
        /// 调用控件线程方法
        /// </summary>
        /// <param name="args">参数</param>
        public override void onInvoke(object args) {
            base.onInvoke(args);
            if (args != null) {
                FCMessage message = (FCMessage)args;
                //LatestDataInfo dataInfo = new LatestDataInfo();
                List<PriceData> datas = PriceDataServiceClient.getPriceDatas(message.m_body, message.m_bodyLength);
                int datasSize = datas.Count;
                for (int i = 0; i < datasSize; i++) {
                    PriceData latestData = datas[i];
                    if (latestData.m_code == "000001.SH") {
                        if (!latestData.equal(m_ssLatestData)) {
                            m_ssLatestData = latestData;
                        }
                    }
                    else if (latestData.m_code == "399001.SZ") {
                        if (!latestData.equal(m_szLatestData)) {
                            m_szLatestData = latestData;
                        }
                    }
                    else if (latestData.m_code == "399006.SZ") {
                        if (!latestData.equal(m_cyLatestData)) {
                            m_cyLatestData = latestData;
                        }
                    }
                }
                invalidate();
            }
        }

        /// <summary>
        /// 鼠标按下方法
        /// </summary>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="click">点击次数</param>
        /// <param name="delta">滚轮滚动值</param>
        public override void onTouchDown(FCTouchInfo touchInfo) {
            base.onTouchDown(touchInfo);
            FCPoint mp = touchInfo.m_firstPoint;
            if (touchInfo.m_firstTouch && touchInfo.m_clicks == 1) {
                int width = getWidth();
                Security security = new Security();
                security.m_type = 18;
                if (mp.x < width / 3) {
                    security.m_code = "000001.SH";
                    security.m_name = "上证指数";
                }
                else if (mp.x < width * 2 / 3) {
                    security.m_code = "399001.SZ";
                    security.m_name = "深证成指";
                }
                else {
                    security.m_code = "399006.SZ";
                    security.m_name = "创业板指数";
                }
                m_mainFrame.searchSecurity(security);
            }
        }

        /// <summary>
        /// 绘制前景方法
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        public override void onPaintForeground(FCPaint paint, FCRect clipRect) {
            FCRect bounds = getBounds();
            int width = bounds.right - bounds.left;
            int height = bounds.bottom - bounds.top;
            if (width > 0 && height > 0) {
                if (m_ssLatestData != null && m_szLatestData != null && m_cyLatestData != null) {
                    long titleColor = MyColor.USERCOLOR98;
                    FCFont font = new FCFont("Default", 16, false, false, false);
                    FCFont indexFont = new FCFont("Default", 14, true, false, false);
                    long grayColor = MyColor.USERCOLOR99;
                    //上证指数
                    long indexColor = DataCenter.getPriceColor(m_ssLatestData.m_close, m_ssLatestData.m_lastClose);
                    int left = 1;
                    FCDraw.drawText(paint, "上证", titleColor, font, left, 3);
                    left += 40;
                    paint.drawLine(grayColor, 1, 0, left, 0, left, height);
                    String amount = (m_ssLatestData.m_amount / 100000000).ToString("0.0") + "亿";
                    FCSize amountSize = paint.textSize(amount, indexFont, -1);
                    FCDraw.drawText(paint, amount, titleColor, indexFont, width / 3 - amountSize.cx, 3);
                    left += (width / 3 - 40 - amountSize.cx) / 4;
                    int length = FCDraw.drawUnderLineNum(paint, m_ssLatestData.m_close, 2, indexFont, indexColor, false, left, 3);
                    left += length + (width / 3 - 40 - amountSize.cx) / 4;
                    length = FCDraw.drawUnderLineNum(paint, m_ssLatestData.m_close - m_ssLatestData.m_lastClose, 2, indexFont, indexColor, false, left, 3);
                    //深证指数
                    left = width / 3;
                    paint.drawLine(grayColor, 1, 0, left, 0, left, height);
                    indexColor = DataCenter.getPriceColor(m_szLatestData.m_close, m_szLatestData.m_lastClose);
                    FCDraw.drawText(paint, "深证", titleColor, font, left, 3);
                    left += 40;
                    paint.drawLine(grayColor, 1, 0, left, 0, left, height);
                    amount = (m_szLatestData.m_amount / 100000000).ToString("0.0") + "亿";
                    amountSize = paint.textSize(amount, indexFont, -1);
                    FCDraw.drawText(paint, amount, titleColor, indexFont, width * 2 / 3 - amountSize.cx, 3);
                    left += (width / 3 - 40 - amountSize.cx) / 4;
                    length = FCDraw.drawUnderLineNum(paint, m_szLatestData.m_close, 2, indexFont, indexColor, false, left, 3);
                    left += length + (width / 3 - 40 - amountSize.cx) / 4;
                    length = FCDraw.drawUnderLineNum(paint, m_szLatestData.m_close - m_szLatestData.m_lastClose, 2, indexFont, indexColor, false, left, 3);
                    //创业指数
                    left = width * 2 / 3;
                    paint.drawLine(grayColor, 1, 0, left, 0, left, height);
                    indexColor = DataCenter.getPriceColor(m_cyLatestData.m_close, m_cyLatestData.m_lastClose);
                    FCDraw.drawText(paint, "创业", titleColor, font, left, 3);
                    left += 40;
                    paint.drawLine(grayColor, 1, 0, left, 0, left, height);
                    amount = (m_cyLatestData.m_amount / 100000000).ToString("0.0") + "亿";
                    amountSize = paint.textSize(amount, indexFont, -1);
                    FCDraw.drawText(paint, amount, titleColor, indexFont, width - amountSize.cx, 3);
                    left += (width / 3 - 40 - amountSize.cx) / 4;
                    length = FCDraw.drawUnderLineNum(paint, m_cyLatestData.m_close, 2, indexFont, indexColor, false, left, 3);
                    left += (width / 3 - 40 - amountSize.cx) / 4 + length;
                    length = FCDraw.drawUnderLineNum(paint, m_cyLatestData.m_close - m_cyLatestData.m_lastClose, 2, indexFont, indexColor, false, left, 3);
                    paint.drawRect(grayColor, 1, 0, new FCRect(0, 0, width - 1, height - 1));
                }
            }
        }

        /// <summary>
        /// 开始请求
        /// </summary>
        public void start() {
            DataCenter.m_priceDataServiceClient.subCodes(m_requestID, "000001.SH,399001.SZ,399006.SZ");
        }

        public FCView createView(UIXmlEx uiXmlEx, System.Xml.XmlNode node)
        {
            return new IndexDiv();
        }

        public bool moreAnalysis(FCView view, UIXmlEx uiXmlEx, System.Xml.XmlNode node)
        {
            return false;
        }
    }
}
