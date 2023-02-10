/*
 * iChart����ϵͳ(��Դ)
 * �Ϻ����è��Ϣ�������޹�˾
 */

using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;

namespace chart
{
    /// <summary>
    /// ������ָ��
    /// </summary>
    public class FloatDiv : FCView, IOutReWrite
    {
        /// <summary>
        /// �����ؼ�
        /// </summary>
        public FloatDiv()
        {
            setBackColor(FCColor.Back);
            setBorderColor(MyColor.USERCOLOR96);
            setCursor(FCCursors.SizeAll);
        }

        private MainFrame m_mainFrame;

        /// <summary>
        /// ��ȡ����������ؼ�
        /// </summary>
        public MainFrame getMainFrame() {
            return m_mainFrame;
        }

        /// <summary>
        /// ��������ؼ�
        /// </summary>
        /// <param name="value"></param>
        public void setMainFrame(MainFrame value) {
            m_mainFrame = value;
        }

        private int m_digit = 2;

        /// <summary>
        /// ��ȡ�����ñ���С����λ��
        /// </summary>
        public int getDigit()
        {
            return m_digit;
        }

        /// <summary>
        /// ���ñ���С����λ��
        /// </summary>
        /// <param name="value"></param>
        public void setDigit(int value) {
            m_digit = value;
        }

        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="click">�������</param>
        /// <param name="delta">���ֹ���ֵ</param>
        public override void onClick(FCTouchInfo touchInfo)
        {
            base.onClick(touchInfo);
            FCPoint mp = touchInfo.m_firstPoint;
            if (touchInfo.m_firstTouch && touchInfo.m_clicks == 1)
            {
                int width = getWidth();
                if (mp.x >= width - 14 && mp.y <= 14)
                {
                    setVisible(false);
                    getNative().invalidate();
                }
            }
        }

        /// <summary>
        /// ����ƶ�����
        /// </summary>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="click">�������</param>
        /// <param name="delta">���ֹ���ֵ</param>
        public override void onTouchMove(FCTouchInfo touchInfo)
        {
            base.onTouchMove(touchInfo);
            FCPoint mp = touchInfo.m_firstPoint;
            int width = getWidth();
            if (mp.x >= width - 14 && mp.y <= 14)
            {
                setCursor(FCCursors.Arrow);
            }
            else
            {
                setCursor(FCCursors.SizeAll);
            }
            invalidate();
        }

        /// <summary>
        /// �ػ汳������
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void onPaintBackground(FCPaint paint, FCRect clipRect)
        {
            int width = getWidth();
            int height = getHeight();
            if (width > 0 && height > 0)
            {
                FCChart chart = m_mainFrame.getChart();
                //ʮ���߳���ʱ���л���
                if (chart.showCrossLine())
                {
                    FCDataTable dataSource = chart.getDataSource();
                    //��ȡ���ͣ������
                    int crossStopIndex = chart.getCrossStopIndex();
                    if (dataSource.getRowsCount() > 0)
                    {
                        if (crossStopIndex < 0)
                        {
                            crossStopIndex = chart.getFirstVisibleIndex();
                        }
                        if (crossStopIndex > chart.getLastVisibleIndex())
                        {
                            crossStopIndex = chart.getLastVisibleIndex();
                        }
                    }
                    else
                    {
                        crossStopIndex = -1;
                    }
                    //��ȡK�ߺͳɽ���
                    FCRect rectangle = new FCRect(0, 0, width, height);
                    long win32Color = FCColor.None;
                    paint.fillRect(getPaintingBackColor(), rectangle);
                    paint.drawRect(getPaintingBorderColor(), 1, 0, rectangle);
                    //���رհ�ť
                    long lineColor = MyColor.USERCOLOR3;
                    paint.drawLine(lineColor, 2, 0, width - 6, 4, width - 14, 12);
                    paint.drawLine(lineColor, 2, 0, width - 6, 12, width - 14, 4);
                    //��������
                    FCFont font = new FCFont("Default", 14, false, false, false);
                    FCFont lfont = new FCFont("Default", 12, false, false, false);
                    FCFont nfont = new FCFont("Default", 14, true, false, false);
                    //������
                    FCDraw.drawText(paint, "ʱ ��", MyColor.USERCOLOR3, font, rectangle.left + 25, rectangle.top + 2);
                    DateTime date = DateTime.Now;
                    if (crossStopIndex >= 0)
                    {
                        double dateNum = dataSource.getXValue(crossStopIndex);
                        if (dateNum != 0)
                        {
                            date = FCTran.numToDate(dateNum);
                        }
                        String dateStr = "";
                        int cycle = m_mainFrame.getCycle();
                        if (cycle <= 1)
                        {
                            dateStr = date.ToString("hh:mm");
                        }
                        else if (cycle >= 5 && cycle <= 60)
                        {
                            dateStr = date.ToString("MM-dd hh:mm");
                        }
                        else
                        {
                            dateStr = date.ToString("yyyy-MM-dd");
                        }
                        FCSize dtSize = paint.textSize(dateStr, lfont, -1);
                        FCDraw.drawText(paint, dateStr, MyColor.USERCOLOR97,
                        lfont, rectangle.left + width / 2 - dtSize.cx / 2, rectangle.top + 20);
                        //��ȡֵ
                        double close = 0, high = 0, low = 0, open = 0, amount = 0;
                        if (crossStopIndex >= 0)
                        {
                            close = dataSource.get2(crossStopIndex, KeyFields.CLOSE_INDEX);
                            high = dataSource.get2(crossStopIndex, KeyFields.HIGH_INDEX);
                            low = dataSource.get2(crossStopIndex, KeyFields.LOW_INDEX);
                            open = dataSource.get2(crossStopIndex, KeyFields.OPEN_INDEX);
                            amount = dataSource.get2(crossStopIndex, KeyFields.AMOUNT_INDEX);
                        }
                        if (double.IsNaN(close))
                        {
                            close = 0;
                        }
                        if (double.IsNaN(high))
                        {
                            high = 0;
                        }
                        if (double.IsNaN(low))
                        {
                            low = 0;
                        }
                        if (double.IsNaN(open))
                        {
                            open = 0;
                        }
                        if (double.IsNaN(amount))
                        {
                            amount = 0;
                        }
                        double rate = 1;
                        double lastClose = 0;
                        if (crossStopIndex > 1)
                        {
                            lastClose = dataSource.get2(crossStopIndex - 1, KeyFields.CLOSE_INDEX);
                            if (cycle == 0)
                            {
                                lastClose = m_mainFrame.getLatestData().m_lastClose;
                            }
                            if (!double.IsNaN(lastClose))
                            {
                                if (lastClose != 0)
                                {
                                    rate = (close - lastClose) / lastClose;
                                }
                            }
                        }
                        //���̼�
                        String openStr = double.IsNaN(open) ? "" : FCTran.getValueByDigit(open, m_digit).ToString();
                        FCSize tSize = paint.textSize(openStr, nfont, -1);
                        FCDraw.drawText(paint, openStr, DataCenter.getPriceColor(open, lastClose), nfont, rectangle.left + width / 2 - tSize.cx / 2, rectangle.top + 60);
                        //��߼�
                        String highStr = double.IsNaN(high) ? "" : FCTran.getValueByDigit(high, m_digit).ToString();
                        tSize = paint.textSize(highStr, nfont, -1);
                        FCDraw.drawText(paint, highStr, DataCenter.getPriceColor(high, lastClose), nfont, rectangle.left + width / 2 - tSize.cx / 2, rectangle.top + 100);
                        //��ͼ�
                        String lowStr = double.IsNaN(low) ? "" : FCTran.getValueByDigit(low, m_digit).ToString();
                        tSize = paint.textSize(lowStr, nfont, -1);
                        FCDraw.drawText(paint, lowStr, DataCenter.getPriceColor(low, lastClose), nfont, rectangle.left + width / 2 - tSize.cx / 2, rectangle.top + 140);
                        //��ͼ�
                        String closeStr = double.IsNaN(close) ? "" : FCTran.getValueByDigit(close, m_digit).ToString();
                        tSize = paint.textSize(closeStr, nfont, -1);
                        FCDraw.drawText(paint, closeStr, DataCenter.getPriceColor(close, lastClose), nfont, rectangle.left + width / 2 - tSize.cx / 2, rectangle.top + 180);
                        //�ɽ���
                        String unit = "";
                        if (amount > 100000000)
                        {
                            amount /= 100000000;
                            unit = "��";
                        }
                        else if (amount > 10000)
                        {
                            amount /= 10000;
                            unit = "��";
                        }
                        String amountStr = FCTran.getValueByDigit(amount, 2) + unit;
                        tSize = paint.textSize(amountStr, lfont, -1);
                        FCDraw.drawText(paint, amountStr, MyColor.USERCOLOR97, lfont, rectangle.left + width / 2 - tSize.cx / 2, rectangle.top + 220);
                        //�Ƿ�
                        String rangeStr = double.IsNaN(rate) ? "0.00%" : rate.ToString("0.00%");
                        tSize = paint.textSize(rangeStr, nfont, -1);
                        FCDraw.drawText(paint, rangeStr, DataCenter.getPriceColor(close, lastClose), lfont, rectangle.left + width / 2 - tSize.cx / 2, rectangle.top + 260);
                    }
                    long whiteColor = MyColor.USERCOLOR3;
                    FCDraw.drawText(paint, "�� ��", whiteColor, font, rectangle.left + 25, rectangle.top + 40);
                    FCDraw.drawText(paint, "�� ��", whiteColor, font, rectangle.left + 25, rectangle.top + 80);
                    FCDraw.drawText(paint, "�� ��", whiteColor, font, rectangle.left + 25, rectangle.top + 120);
                    FCDraw.drawText(paint, "�� ��", whiteColor, font, rectangle.left + 25, rectangle.top + 160);
                    FCDraw.drawText(paint, "�� ��", whiteColor, font, rectangle.left + 25, rectangle.top + 200);
                    FCDraw.drawText(paint, "�� ��", whiteColor, font, rectangle.left + 25, rectangle.top + 240);
                }
            }
        }

        /// <summary>
        /// �ػ���߷���
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void onPaintBorder(FCPaint paint, FCRect clipRect)
        {
        }

        public FCView createView(UIXmlEx uiXmlEx, System.Xml.XmlNode node)
        {
            return new FloatDiv();
        }

        public bool moreAnalysis(FCView view, UIXmlEx uiXmlEx, System.Xml.XmlNode node)
        {
            return false;
        }
    }
}
