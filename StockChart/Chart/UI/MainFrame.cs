/*
 * iChart����ϵͳ(��Դ)
 * �Ϻ����è��Ϣ�������޹�˾
 */

using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;
using System.Windows.Forms;
using System.Threading;

namespace chart {
    /// <summary>
    /// ����ϵͳ
    /// </summary>
    public class MainFrame : UIXmlEx, FCListenerMessageCallBack, FCMenuItemTouchEventCallBack, FCInvokeEventCallBack, FCTouchEventCallBack {
        /// <summary>
        /// ��������ϵͳ
        /// </summary>
        public MainFrame() {
        }

        /// <summary>
        /// �ɽ�����
        /// </summary>
        public BarShape m_bar;

        /// <summary>
        /// K��
        /// </summary>
        public CandleShape m_candle;

        /// <summary>
        /// ��div�ı��
        /// </summary>
        private ChartDiv m_candleDiv;

        /// <summary>
        /// K�ߵĺ�����
        /// </summary>
        private double m_candleHScalePixel;

        /// <summary>
        /// ��ǰ������ͼ��
        /// </summary>
        private ChartDiv m_currentDiv;

        /// <summary>
        /// ���еĲ�
        /// </summary>
        private List<ChartDiv> m_divs = new List<ChartDiv>();

        /// <summary>
        /// ������
        /// </summary>
        private FloatDiv m_floatDiv;

        /// <summary>
        /// ����Ĳ���
        /// </summary>
        private ArrayList<double> m_hScaleSteps = new ArrayList<double>();

        /// <summary>
        /// ָ�����
        /// </summary>
        private IndexDiv m_indexDiv;

        /// <summary>
        /// ʵʱ�������
        /// </summary>
        private LatestDiv m_latestDiv;

        /// <summary>
        /// ��ǰ����ID
        /// </summary>
        private String m_layoutID = "";

        /// <summary>
        /// ��ʱ�ߵ�ƽ����
        /// </summary>
        private PolylineShape m_minuteAvgLine;

        /// <summary>
        /// ��ʱ��
        /// </summary>
        private PolylineShape m_minuteLine;

        /// <summary>
        /// ������
        /// </summary>
        private FCNative m_native;

        /// <summary>
        /// ������2
        /// </summary>
        private int m_requestID2 = FCClientService.getRequestID();

        /// <summary>
        /// �Ƿ�ת������
        /// </summary>
        private bool m_reverseVScale = false;

        /// <summary>
        /// �Ҽ��˵�
        /// </summary>
        private FCMenu m_rightMenu;

        /// <summary>
        /// ���ڲ�ѯ��֤ȯ����
        /// </summary>
        private Security m_searchSecurity;

        /// <summary>
        /// ��Ʊ����
        /// </summary>
        private SecurityService m_securityService;

        /// <summary>
        /// ���ID
        /// </summary>
        private int m_timerID = FCView.getNewTimerID();

        /// <summary>
        /// �ͻ���Tick���ݻ���
        /// </summary>
        private ClientTickDataCache m_clientTickDataCache = new ClientTickDataCache();

        /// <summary>
        /// ��ʷ���ݻ���
        /// </summary>
        private List<SecurityData> m_securityDatasCache = new List<SecurityData>();

        /// <summary>
        /// �ɽ�����
        /// </summary>
        private ChartDiv m_volumeDiv;

        private FCChart m_chart;

        /// <summary>
        /// ��ȡͼ�οؼ�
        /// </summary>
        public FCChart getChart() {
            return m_chart;
        }

        /// <summary>
        /// ����ͼ�οؼ�
        /// </summary>
        /// <param name="value"></param>
        public void setChart(FCChart value) {
            m_chart = value; 
        }

        private int m_cycle = 1440;

        /// <summary>
        /// ��ȡ����
        /// </summary>
        public int getCycle() {
            if (m_showMinuteLine) {
                return 0;
            } else {
                return m_cycle;
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="value"></param>
        public void setCycle(int value) {
            m_cycle = value;
        }

        private int m_digit = 2;

        /// <summary>
        /// ��ȡ�۸���С��λ��
        /// </summary>
        public int getDigit() {
            return m_digit;
        }

        /// <summary>
        /// ���ü۸���С��λ��
        /// </summary>
        /// <param name="value"></param>
        public void setDigit(int value) {
            m_digit = value;
        }

        private int m_index = -1;

        /// <summary>
        /// ��ȡ��ǰʵ�ʵ���������
        /// </summary>
        public int getIndex() {
            return m_index;
        }

        private ArrayList<FCScript> m_indicators = new ArrayList<FCScript>();

        /// <summary>
        /// ��ȡ����ָ��
        /// </summary>
        public ArrayList<FCScript> getIndicators()
        {
            return m_indicators;
        }

        /// <summary>
        /// ��������ָ��
        /// </summary>
        /// <param name="value"></param>
        public void setIndicators(ArrayList<FCScript> value) {
            m_indicators = value;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        public SecurityLatestData getLatestData() {
            return m_latestDiv.getLatestData();
        }

        private SearchDiv m_searchDiv;

        /// <summary>
        /// ��ȡ���̾���
        /// </summary>
        public SearchDiv getSearchDiv() {
            return m_searchDiv; 
        }

        /// <summary>
        /// ���ü��̾���
        /// </summary>
        /// <param name="value"></param>
        public void setSearchDiv(SearchDiv value) {
            m_searchDiv = value;
        }

        private bool m_showMinuteLine = false;

        /// <summary>
        /// ��ȡ�Ƿ��ʱͼ
        /// </summary>
        public bool showMinuteLine() {
            return m_showMinuteLine;
        }

        /// <summary>
        /// �����Ƿ��ʱͼ
        /// </summary>
        /// <param name="value"></param>
        public void setShowMinuteLine(bool value) {
            m_showMinuteLine = value;
        }

        private int m_subscription = 0;

        /// <summary>
        /// ��ȡ��Ȩ��ʽ
        /// </summary>
        public int getSubscription() {
            return m_subscription; 
        }

        /// <summary>
        /// ���ø�Ȩ��ʽ
        /// </summary>
        /// <param name="value"></param>
        public void setSubscription(int value) {
            m_subscription = value;
        }

        /// <summary>
        /// ��ӿհײ�
        /// </summary>
        public void addBlankDiv() {
            //���ص�x��
            int divSize = m_divs.Count;
            for (int i = 0; i < divSize; i++) {
                m_divs[i].getHScale().setVisible(false);
                m_divs[i].getHScale().setHeight(0);
            }
            ChartDiv div = m_chart.addDiv();
            div.setBackColor(FCColor.Back);
            m_divs.Add(div);
            div.getHScale().setHeight(22);
            div.getVGrid().setDistance(40);
            div.getLeftVScale().setTextColor(MyColor.USERCOLOR3);
            div.getLeftVScale().setFont(new FCFont("Default", 14, false, false, false));
            div.getRightVScale().setTextColor(MyColor.USERCOLOR3);
            div.getRightVScale().setFont(new FCFont("Default", 14, false, false, false));
            refreshData();
        }

        /// <summary>
        /// ������ͼָ��
        /// </summary>
        /// <param name="name">����</param>
        /// <param name="title">����</param>
        /// <param name="script">�ű�</param>
        /// <param name="parameters">����</param>
        /// <param name="div">ͼ��</param>
        /// <param name="update">�Ƿ����</param>
        /// <returns>ָ�����</returns>
        public FCScript addMainIndicator(String name, String title, String script, String parameters, ChartDiv div, bool update) {
            //��������
            FCScript indicator = SecurityDataHelper.createIndicator(m_chart, m_chart.getDataSource(), script, parameters);
            indicator.setName(name);
            //indicator.FullName = title;
            indicator.setAttachVScale(AttachVScale.Left);
            m_indicators.Add(indicator);
            indicator.setDiv(div);
            indicator.onCalculate(0);
            if (div != m_candleDiv && div != m_volumeDiv) {
                div.getTitleBar().setText(title);
            }
            //ˢ��ͼ��
            if (update) {
                m_chart.update();
                m_native.invalidate();
            }
            return indicator;
        }

        /// <summary>
        /// ��Ӹ�ͼָ��
        /// </summary>
        /// <param name="name">����</param>
        /// <param name="script">�ű�</param>
        /// <param name="param">����</param>
        /// <param name="div">ͼ��</param>
        /// <param name="update">�Ƿ���²���</param>
        /// <returns>ָ�����</returns>
        public FCScript addViceIndicator(String name, String script, String parameters, ChartDiv div, bool update) {
            FCScript indicator = SecurityDataHelper.createIndicator(m_chart, m_chart.getDataSource(), script, parameters);
            indicator.setAttachVScale(AttachVScale.Right);
            m_indicators.Add(indicator);
            indicator.setDiv(div);
            indicator.setName(name);
            //��������
            indicator.onCalculate(0);
            if (update) {
                m_chart.update();
                m_native.invalidate();
            }
            return indicator;
        }

        /// <summary>
        /// ����ʷ����
        /// </summary>
        /// <param name="dataInfo">������Ϣ</param>
        /// <param name="historyDatas">��ʷ����</param>
        public void bindHistoryData(String code, int cycle, List<SecurityData> historyDatas) {
            if (code == m_latestDiv.getSecurityCode() && cycle == m_cycle) {
                FCDataTable dataSource = m_chart.getDataSource();
                int[] fields = new int[] { KeyFields.CLOSE_INDEX, KeyFields.HIGH_INDEX, KeyFields.LOW_INDEX, KeyFields.OPEN_INDEX, KeyFields.VOL_INDEX, KeyFields.AMOUNT_INDEX, KeyFields.AVGPRICE_INDEX };
                int index = 0;
                SecurityDataHelper.bindHistoryDatas(m_chart, dataSource, m_indicators, fields, historyDatas);
                if (index >= 0) {
                    int rowsSize = dataSource.getRowsCount();
                    m_hScaleSteps.Clear();
                    //����������
                    if (m_showMinuteLine) {
                        DateTime date = FCTran.numToDate(dataSource.getXValue(0));
                        int year = date.Year, month = date.Month, day = date.Day;
                        m_hScaleSteps.Add(FCTran.dateToNum(new DateTime(year, month, day, 9, 0, 0)));
                        m_hScaleSteps.Add(FCTran.dateToNum(new DateTime(year, month, day, 9, 30, 0)));
                        m_hScaleSteps.Add(FCTran.dateToNum(new DateTime(year, month, day, 10, 0, 0)));
                        m_hScaleSteps.Add(FCTran.dateToNum(new DateTime(year, month, day, 10, 30, 0)));
                        m_hScaleSteps.Add(FCTran.dateToNum(new DateTime(year, month, day, 11, 0, 0)));
                        m_hScaleSteps.Add(FCTran.dateToNum(new DateTime(year, month, day, 13, 0, 0)));
                        m_hScaleSteps.Add(FCTran.dateToNum(new DateTime(year, month, day, 13, 30, 0)));
                        m_hScaleSteps.Add(FCTran.dateToNum(new DateTime(year, month, day, 14, 0, 0)));
                        m_hScaleSteps.Add(FCTran.dateToNum(new DateTime(year, month, day, 14, 30, 0)));
                        m_hScaleSteps.Add(FCTran.dateToNum(new DateTime(year, month, day, 15, 0, 0)));
                    }
                    for (int i = index; i < rowsSize; i++) {
                        double volume = dataSource.get2(i, KeyFields.VOL_INDEX);
                        if (!double.IsNaN(volume)) {
                            m_index = i;
                        }
                        if (!m_showMinuteLine) {
                            double close = dataSource.get2(i, KeyFields.CLOSE_INDEX);
                            double open = dataSource.get2(i, KeyFields.OPEN_INDEX);
                            if (close >= open) {
                                dataSource.set2(i, m_bar.getStyleField(), 1);
                                dataSource.set2(i, m_bar.getColorField(), MyColor.USERCOLOR71);
                            }
                            else {
                                dataSource.set2(i, m_bar.getStyleField(), 0);
                                dataSource.set2(i, m_bar.getColorField(), MyColor.USERCOLOR72);
                            }
                        }
                    }
                    refreshData();
                }
                m_chart.update();
                m_native.invalidate();
            }
        }

        /// <summary>
        /// �޸�����
        /// </summary>
        /// <param name="cycle">����</param>
        public void changeCycle(int cycle) {
            int oldCycle = getCycle();
            if (cycle > 0) {
                if (oldCycle > 0 && oldCycle != cycle) {
                    m_candleHScalePixel = m_chart.getHScalePixel();
                }
                setCycle(cycle);
                m_showMinuteLine = false;
            }
            else {
                setCycle(cycle);
                m_showMinuteLine = true;
            }
            String securityCode = m_latestDiv.getSecurityCode();
            if (securityCode != null && securityCode.Length > 0) {
                Security security = new Security();
                m_securityService.getSecurityByCode(securityCode, ref security);
                searchSecurity(security);
            }
        }

        private void newData(SecurityLatestData latestData)
        {
            lock (m_securityDatasCache)
            {
                if (m_securityDatasCache.Count > 0)
                {
                    //ǰ��Ȩ
                    if (m_subscription == 1)
                    {
                        StockService.getDataAfterDivide(IsDivideRightType.Forward, latestData, m_securityDatasCache[m_securityDatasCache.Count - 1].m_forwardFactor);
                    }
                    //��Ȩ
                    else if (m_subscription == 2)
                    {
                        StockService.getDataAfterDivide(IsDivideRightType.Forward, latestData, m_securityDatasCache[m_securityDatasCache.Count - 1].m_backFactor);
                    }
                }
                m_clientTickDataCache.m_code = latestData.m_code;
                int oldDatasSize = m_securityDatasCache.Count;
                StockService.mergeLatestData(m_securityDatasCache, latestData, m_clientTickDataCache, getCycle());
                oldDatasSize = m_securityDatasCache.Count;
                FCDataTable dataSource = m_chart.getDataSource();
                int startIndex = oldDatasSize - 1;
                int oldIndex = dataSource.getRowIndex(m_securityDatasCache[oldDatasSize - 1].m_date);
                if (oldIndex == -1)
                {
                    oldIndex = 0;
                }
                for (int i = startIndex; i < m_securityDatasCache.Count; i++)
                {
                    SecurityData securityData = m_securityDatasCache[i];
                    int[] fields = new int[] { KeyFields.CLOSE_INDEX, KeyFields.HIGH_INDEX, KeyFields.LOW_INDEX, KeyFields.OPEN_INDEX, KeyFields.VOL_INDEX, KeyFields.AMOUNT_INDEX, KeyFields.AVGPRICE_INDEX };
                    SecurityDataHelper.insertLatestData(m_chart, dataSource, m_indicators, fields, securityData);
                }
                for (int i = oldIndex; i < m_chart.getDataSource().getRowsCount(); i++)
                {
                    double close = dataSource.get2(i, KeyFields.CLOSE_INDEX);
                    double open = dataSource.get2(i, KeyFields.OPEN_INDEX);
                    if (close >= open)
                    {
                        dataSource.set2(i, m_bar.getStyleField(), 1);
                        dataSource.set2(i, m_bar.getColorField(), MyColor.USERCOLOR71);
                    }
                    else
                    {
                        dataSource.set2(i, m_bar.getStyleField(), 0);
                        dataSource.set2(i, m_bar.getColorField(), MyColor.USERCOLOR72);
                    }
                }
                m_chart.update();
                m_chart.invalidate();
            }
        }

        /// <summary>
        /// ���ÿؼ��̷߳���
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="args">����</param>
        public void callInvokeEvent(String eventName, object sender, object args, object invoke) {
            if (args != null) {
                FCMessage message = args as FCMessage;
                if (message != null) {
                    if (message.m_serviceID == DataCenter.m_latestServiceClient.getServiceID()) {
                        SecurityLatestData latestData = LatestServiceClient.getLatestData(message.m_body, message.m_bodyLength);
                        newData(latestData);
                    } else if (message.m_serviceID == DataCenter.m_historyServiceClient.getServiceID()) {
                        if (m_requestID2 == message.m_requestID) {
                            String code = "";
                            int cycle = 0;
                            double startDate = 0, endDate = 0;
                            double nowDate = 0, nowVolume = 0, nowAmount = 0;
                            List<SecurityData> datas = HistoryServiceClient.getDatas(ref code, ref cycle, ref startDate, ref endDate, ref nowDate, ref nowVolume, ref nowAmount, message.m_body, message.m_bodyLength);
                            //ǰ��Ȩ
                            if (m_subscription == 1)
                            {
                                List<List<OneDivideRightBase>> divideData = DataCenter.m_historyServiceClient.getDivideRight(code);
                                StockService.caculateDivideKLineData(datas, divideData, true);
                                float[] factors = StockService.caculateDivideKLineData2(datas, divideData, true);
                                if (factors != null)
                                {
                                    for (int i = 0; i < factors.Length; i++)
                                    {
                                        StockService.getDataAfterDivide(IsDivideRightType.Forward, datas[i], factors[i]);
                                    }
                                }
                            }
                            //��Ȩ
                            else if(m_subscription == 2){
                                List<List<OneDivideRightBase>> divideData = DataCenter.m_historyServiceClient.getDivideRight(code);
                                StockService.caculateDivideKLineData(datas, divideData, false);
                                float[] factors = StockService.caculateDivideKLineData2(datas, divideData, false);
                                if (factors != null)
                                {
                                    for (int i = 0; i < factors.Length; i++)
                                    {
                                        StockService.getDataAfterDivide(IsDivideRightType.Backward, datas[i], factors[i]);
                                    }
                                }
                            }
                            bindHistoryData(code, getCycle(), datas);
                            lock (m_securityDatasCache) {
                                m_securityDatasCache.Clear();
                                m_securityDatasCache.AddRange(datas.ToArray());
                                m_clientTickDataCache.m_code = code;
                                m_clientTickDataCache.m_lastAmount = nowAmount;
                                m_clientTickDataCache.m_lastDate = nowDate;
                                m_clientTickDataCache.m_lastVolume = nowVolume;
                            }
                        } 
                    }
                }
            }
        }

        /// <summary>
        /// ������¼�
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">�������</param>
        /// <param name="delta">����ֵ</param>
        public void callTouchEvent(String eventName, object sender, FCTouchInfo touchInfo, object invoke) {
            if (sender == m_chart) {
                if (eventName == "ontouchdown") {
                    FCView control = sender as FCView;
                    FCPoint mp = control.pointToNative(touchInfo.m_firstPoint);
                    String cmd = FaceCatAPI.getCurrentCmd();
                    if (cmd.IndexOf("addplot:") == 0)
                    {
                        FaceCat.MyChartDiv.addPlot(m_chart, touchInfo);
                    } 
                    else {
                        if (touchInfo.m_secondTouch && touchInfo.m_clicks == 1) {
                            if (m_rightMenu != null)
                            {
                                m_currentDiv = m_chart.getTouchOverDiv();
                                //�����Ҽ��˵�
                                FCSize nativeSize = m_native.getSize();
                                int rightMenuHeight = m_rightMenu.getHeight();
                                if (mp.y + rightMenuHeight > nativeSize.cy)
                                {
                                    mp.y = nativeSize.cy - rightMenuHeight;
                                }
                                m_rightMenu.setLocation(mp);
                                m_rightMenu.setFocused(true);
                                m_rightMenu.update();
                                m_rightMenu.setVisible(true);
                                m_rightMenu.bringToFront();
                                m_native.invalidate();
                            }
                            return;
                        }
                    }
                }else if (eventName == "ontouchup") {
                    String cmd = FaceCatAPI.getCurrentCmd();
                    if (cmd.IndexOf("deleteplot:") == 0)
                    {
                        FaceCat.MyChartDiv.deletePlot(m_chart);
                    }
                    else if (cmd.IndexOf("saveplot:") != -1)
                    {
                        FaceCat.MyChartDiv.savePlot(m_chart);
                    }
                    else if (cmd.IndexOf("deleteformula:") == 0)
                    {
                        FaceCat.MyChartDiv.deleteFormula(m_chart, m_indicators);
                    }
                    else if (cmd.IndexOf("saveformula:") == 0)
                    {
                        FaceCat.MyChartDiv.saveFormula(m_chart, m_indicators);
                    }
                    else if (cmd.IndexOf("useformula:") == 0)
                    {
                        FaceCat.MyChartDiv.useFormula(m_chart, m_indicators);
                    }
                    else if (cmd.IndexOf("useplot:") == 0)
                    {
                        FaceCat.MyChartDiv.usePlot(m_chart);
                    }
                    else if (cmd.IndexOf("remoteplot:") == 0)
                    {
                        FaceCat.MyChartDiv.remotePlot(m_chart);
                    }
                    else if (cmd.IndexOf("remoteformula:") == 0)
                    {
                        FaceCat.MyChartDiv.remoteFormula(m_chart, m_indicators);
                    }
                    else if (cmd.IndexOf("startcopyshape:") == 0)
                    {
                        FaceCat.MyChartDiv.startCopyShape(m_chart);
                    }
                    else if (cmd.IndexOf("copyshape:") == 0)
                    {
                        FaceCat.MyChartDiv.copyShape(m_chart);
                    }
                    else if (cmd.IndexOf("clearcopyshape:") == 0)
                    {
                        FaceCat.MyChartDiv.clearCopyShape(m_chart);
                    }
                    else if (cmd.IndexOf("addblankdiv:") == 0)
                    {
                        FaceCat.MyChartDiv.addBlankDiv(m_chart);
                    }
                    else if (cmd.IndexOf("removeblankdiv:") == 0)
                    {
                        FaceCat.MyChartDiv.removeBlankDiv(m_chart);
                    }
                    else
                    {
                        m_floatDiv.setVisible(m_chart.showCrossLine());
                    }
                    m_native.invalidate();
                }
            } else {
                FCButton closeButton = sender as FCButton;
                if (closeButton != null) {
                    FCWindow window = closeButton.getParent() as FCWindow;
                    if (window != null) {
                        window.close();
                        window.delete();
                        return;
                    }
                }
            }
        }

        public override void delete()
        {
            if (!isDeleted())
            {
                FaceCatAPI.setCopyShape(null, null);
            }
            base.delete();
        }

        /// <summary>
        /// ɾ��ָ��
        /// </summary>
        /// <param name="indicator">ָ��</param>
        public void deleteIndicator(FCScript indicator) {
            indicator.clear();
            m_indicators.Remove(indicator);
            indicator.delete();
            m_chart.update();
            m_native.invalidate();
        }

        /// <summary>
        /// ɾ������ָ��
        /// </summary>
        /// <param name="update">�Ƿ����</param>
        public void deleteIndicators(bool update) {
            int m_indicatorsSize = m_indicators.Count;
            for (int i = 0; i < m_indicatorsSize; i++) {
                FCScript indicator = m_indicators[i];
                indicator.clear();
                indicator.delete();
            }
            m_indicators.Clear();
            if (update) {
                m_chart.update();
                m_native.invalidate();
            }
        }

        /// <summary>
        /// ɾ��ѡ�е�ָ��
        /// </summary>
        public void deleteSelectedIndicator() {
            FCScript indicator = getSelectedIndicator();
            if (indicator != null) {
                indicator.clear();
                m_indicators.Remove(indicator);
                indicator.delete();
                m_chart.update();
                m_native.invalidate();
            }
        }

        /// <summary>
        /// ɾ��ѡ�еĻ���
        /// </summary>
        public void deleteSelectedPlot() {
            FCPlot selectedPlot = m_chart.getSelectedPlot();
            if (selectedPlot != null) {
                selectedPlot.getDiv().removePlot(selectedPlot);
                selectedPlot.delete();
                m_chart.update();
                m_native.invalidate();
            }
        }

        /// <summary>
        /// ��ȡѡ�е�ָ��
        /// </summary>
        /// <returns>ָ��</returns>
        private FCScript getSelectedIndicator() {
            BaseShape shape = m_chart.getSelectedShape();
            if (shape != null) {
                foreach (FCScript indicator in m_indicators) {
                    List<BaseShape> shapes = indicator.getShapes();
                    if (shapes.Contains(shape)) {
                        return indicator;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ��Ԫ��˫���¼�
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="cell">��Ԫ��</param>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">�������</param>
        /// <param name="delta">����ֵ</param>
        private void gridCellClick(object sender, FCGridCell cell, FCTouchInfo touchInfo, object invoke) {
            if (touchInfo.m_clicks == 2) {
                if (cell.getColumn().getName() != "colP11" && cell.getColumn().getName() != "colP12" && cell.getColumn().getName() != "colP13") {
                    //SearchSecurity(cell.Row.getCell("colP1").GetString());
                }
            }
            else {
                FCTextBox txtCode = getTextBox("txtCode");
                txtCode.setText(cell.getRow().getCell("colP1").getString());
                txtCode.invalidate();
            }
        }

        /// <summary>
        /// ���һ���µ�ͼ�㣬���������õı�����������߶�
        /// </summary>
        /// <param name="vPercent">����߶ȱ���</param>
        /// <returns>ͼ��</returns>
        public virtual ChartDiv addDiv2(FCChart chart, float vPercent)
        {
            if (vPercent <= 0) return null;
            //������
            ChartDiv cDiv = new ChartDiv();
            cDiv.setVerticalPercent(vPercent);
            cDiv.setChart(chart);
            chart.m_divs.add(cDiv);
            chart.update();
            return cDiv;
        }

        /// <summary>
        /// ��ʼ��ͼ�ν���
        /// </summary>
        private void initInterface() {
            try
            {
                m_chart = getChart("divKLine");
                FCDataTable dataSource = m_chart.getDataSource();
                m_chart.setBorderColor(FCColor.None);
                m_chart.addEvent(this, "oninvoke", null);
                m_chart.addEvent(this, "ontouchdown", null);
                m_chart.addEvent(this, "ontouchmove", null);
                m_chart.addEvent(this, "ontouchup", null);
                m_chart.setBackColor(MyColor.USERCOLOR64);
                //���ÿ����϶�K�ߣ��ɽ������߼����
                m_chart.setCanMoveShape(true);
                //���ù�������
                m_chart.setScrollAddSpeed(true);
                //��������Y��Ŀ��
                m_chart.setLeftVScaleWidth(85);
                m_chart.setRightVScaleWidth(85);
                //����X��̶ȼ��
                m_chart.setHScalePixel(3);
                //����X��
                m_chart.setHScaleFieldText("����");
                //���k�߲�
                m_candleDiv = addDiv2(m_chart, 60);
                m_candleDiv.setBackColor(FCColor.None);
                m_candleDiv.setBorderColor(FCColor.None);
                m_candleDiv.getTitleBar().setText("����");
                //������Div����Y����ֵ���»���
                m_candleDiv.getVGrid().setVisible(true);
                m_candleDiv.getLeftVScale().setNumberStyle(NumberStyle.UnderLine);
                m_candleDiv.getLeftVScale().setPaddingTop(2);
                m_candleDiv.getLeftVScale().setPaddingBottom(2);
                m_candleDiv.getLeftVScale().setFont(new FCFont("Default", 12, false, false, false));
                m_candleDiv.getRightVScale().setNumberStyle(NumberStyle.UnderLine);
                m_candleDiv.getRightVScale().setFont(new FCFont("Default", 12, false, false, false));
                m_candleDiv.getRightVScale().setPaddingTop(2);
                m_candleDiv.getRightVScale().setPaddingBottom(2);
                ChartTitle priceTitle = new ChartTitle(KeyFields.CLOSE_INDEX, "", MyColor.USERCOLOR101, 2, true);
                priceTitle.setFieldTextMode(TextMode.Value);
                m_candleDiv.getTitleBar().getTitles().Add(priceTitle);
                //���K��ͼ
                m_candle = new CandleShape();
                m_candleDiv.addShape(m_candle);
                m_candle.setCloseField(KeyFields.CLOSE_INDEX);
                m_candle.setHighField(KeyFields.HIGH_INDEX);
                m_candle.setLowField(KeyFields.LOW_INDEX);
                m_candle.setOpenField(KeyFields.OPEN_INDEX);
                m_candle.setCloseFieldText("����");
                m_candle.setHighFieldText("���");
                m_candle.setLowFieldText("���");
                m_candle.setOpenFieldText("����");
                m_candle.setVisible(false);
                //��ʱ��
                m_minuteLine = new PolylineShape();
                m_candleDiv.addShape(m_minuteLine);
                m_minuteLine.setColor(MyColor.USERCOLOR3);
                m_minuteLine.setFieldName(KeyFields.CLOSE_INDEX);
                //��ʱ�ߵ�ƽ����
                m_minuteAvgLine = new PolylineShape();
                m_candleDiv.addShape(m_minuteAvgLine);
                m_minuteAvgLine.setColor(MyColor.USERCOLOR98);
                m_minuteAvgLine.setFieldName(KeyFields.AVGPRICE_INDEX);
                //��ӳɽ�����
                m_volumeDiv = addDiv2(m_chart, 15);
                m_volumeDiv.setBackColor(FCColor.None);
                m_volumeDiv.setBorderColor(FCColor.None);
                //���óɽ����ĵ�λ
                m_volumeDiv.getLeftVScale().setDigit(0);
                m_volumeDiv.getLeftVScale().setFont(new FCFont("Default", 12, false, false, false));
                m_volumeDiv.getVGrid().setDistance(30);
                m_volumeDiv.getRightVScale().setDigit(0);
                m_volumeDiv.getRightVScale().setFont(new FCFont("Default", 12, false, false, false));
                //��ӳɽ���
                m_bar = new BarShape();
                m_bar.setColorField(FCDataTable.getAutoField());
                m_bar.setStyleField(FCDataTable.getAutoField());
                m_bar.setUpColor(MyColor.USERCOLOR98);
                m_volumeDiv.addShape(m_bar);
                m_bar.setFieldName(KeyFields.VOL_INDEX);
                //���ñ���
                m_volumeDiv.getTitleBar().setText("�ɽ���");
                //���óɽ�����ʾ����
                m_bar.setFieldText("�ɽ���");
                //���óɽ�������ֻ��ʾֵ
                ChartTitle barTitle = new ChartTitle(KeyFields.VOL_INDEX, "�ɽ���", m_bar.getDownColor(), 0, true);
                barTitle.setFieldTextMode(TextMode.Value);
                m_volumeDiv.getTitleBar().getTitles().Add(barTitle);
                //���ָ���
                ChartDiv indDiv = addDiv2(m_chart, 25);
                indDiv.setBackColor(FCColor.None);
                indDiv.setBorderColor(FCColor.None);
                indDiv.getVGrid().setDistance(40);
                indDiv.getLeftVScale().setPaddingTop(2);
                indDiv.getLeftVScale().setPaddingBottom(2);
                indDiv.getLeftVScale().setFont(new FCFont("Default", 12, false, false, false));
                indDiv.getRightVScale().setPaddingTop(2);
                indDiv.getRightVScale().setPaddingBottom(2);
                indDiv.getRightVScale().setFont(new FCFont("Default", 12, false, false, false));
                //����X�᲻�ɼ�
                m_candleDiv.getHScale().setVisible(false);
                m_candleDiv.getHScale().setHeight(0);
                m_volumeDiv.getHScale().setVisible(false);
                m_volumeDiv.getHScale().setHeight(0);
                indDiv.getHScale().setVisible(true);
                indDiv.getHScale().setHeight(22);
                //�������������ɫ
                m_candleDiv.getLeftVScale().setTextColor(MyColor.USERCOLOR67);
                m_candleDiv.getRightVScale().setTextColor(MyColor.USERCOLOR67);
                m_volumeDiv.getLeftVScale().setTextColor(MyColor.USERCOLOR67);
                m_volumeDiv.getRightVScale().setTextColor(MyColor.USERCOLOR67);
                indDiv.getLeftVScale().setTextColor(MyColor.USERCOLOR67);
                indDiv.getRightVScale().setTextColor(MyColor.USERCOLOR67);


                m_candleDiv.getLeftVScale().setScaleColor(MyColor.USERCOLOR8);
                m_volumeDiv.getLeftVScale().setScaleColor(MyColor.USERCOLOR8);
                indDiv.getLeftVScale().setScaleColor(MyColor.USERCOLOR8);

                m_candleDiv.getRightVScale().setScaleColor(MyColor.USERCOLOR8);
                m_volumeDiv.getRightVScale().setScaleColor(MyColor.USERCOLOR8);
                indDiv.getRightVScale().setScaleColor(MyColor.USERCOLOR8);

                m_candleDiv.getHScale().setScaleColor(MyColor.USERCOLOR8);
                m_volumeDiv.getHScale().setScaleColor(MyColor.USERCOLOR8);
                indDiv.getHScale().setScaleColor(MyColor.USERCOLOR8);
                m_candleDiv.getTitleBar().setUnderLineColor(MyColor.USERCOLOR8);
                m_volumeDiv.getTitleBar().setUnderLineColor(MyColor.USERCOLOR8);
                indDiv.getTitleBar().setUnderLineColor(MyColor.USERCOLOR8);


                m_candleDiv.getLeftVScale().getCrossLineTip().setBackColor(MyColor.USERCOLOR8);
                m_volumeDiv.getLeftVScale().getCrossLineTip().setBackColor(MyColor.USERCOLOR8);
                indDiv.getLeftVScale().getCrossLineTip().setBackColor(MyColor.USERCOLOR8);
                indDiv.getHScale().getCrossLineTip().setBackColor(MyColor.USERCOLOR8);
                indDiv.getHScale().getCrossLineTip().setBackColor(MyColor.USERCOLOR8);

                m_candleDiv.getHGrid().setGridColor(MyColor.USERCOLOR8);
                m_volumeDiv.getHGrid().setGridColor(MyColor.USERCOLOR8);
                indDiv.getHGrid().setGridColor(MyColor.USERCOLOR8);

                //����������������߼��
                //��ӵ�����
                m_divs.AddRange(new ChartDiv[] { m_candleDiv, m_volumeDiv, indDiv });
                //����û��Զ����
                m_floatDiv = findView("divFloat") as FloatDiv;
                m_floatDiv.setMainFrame(this);
                //��ǰ���ݲ�
                m_latestDiv = findView("divLatest") as LatestDiv;
                m_latestDiv.setMainFrame(this);
                m_indexDiv = findView("divIndex") as IndexDiv;
                m_indexDiv.setMainFrame(this);
                dataSource.addColumn(KeyFields.CLOSE_INDEX);
                dataSource.addColumn(KeyFields.HIGH_INDEX);
                dataSource.addColumn(KeyFields.LOW_INDEX);
                dataSource.addColumn(KeyFields.OPEN_INDEX);
                dataSource.addColumn(KeyFields.VOL_INDEX);
                dataSource.addColumn(KeyFields.AMOUNT_INDEX);
                dataSource.addColumn(KeyFields.AVGPRICE_INDEX);
                dataSource.addColumn(m_bar.getColorField());
                dataSource.addColumn(m_bar.getStyleField());
                dataSource.setColsCapacity(16);
                dataSource.setColsGrowStep(4);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// �Ƿ��д�����ʾ
        /// </summary>
        /// <returns>�Ƿ���ʾ</returns>
        public bool isWindowShowing() {
            List<FCView> controls = m_native.getViews();
            int controlsSize = controls.Count;
            for (int i = 0; i < controlsSize; i++) {
                FCWindowFrame frame = controls[i] as FCWindowFrame;
                if (frame != null) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public override void loadData() {
            m_securityService = DataCenter.m_securityService;
            if (m_rightMenu == null) {
                //�����Ҽ��˵�
                m_rightMenu = getMenu("rightMenu");
                m_rightMenu.addEvent(this, "onmenuitemclick", null);
                m_rightMenu.setVisible(false);
                m_rightMenu.update();
                //ָ����Ҽ��˵�
                //m_quoteService.addListener(m_vfRequestID, new ListenerMessageCallBack(quoteDataCallBack));
            }
            m_indexDiv.start();

            DataCenter.m_historyServiceClient.addListener(m_requestID2, this, null);
            addMainIndicator("MA", "", "CONST N1=5;CONST N2=10;CONST N3=20;CONST N4=30;CONST N5=120;CONST N6=250;\r\nMA5:MA(CLOSE,N1); \rMA10:MA(CLOSE,N2); \rMA20:MA(CLOSE,N3); \rMA30:MA(CLOSE,N4);\rMA120:MA(CLOSE,N5); \rMA250:MA(CLOSE,N6);", "", m_candleDiv, true);
            addMainIndicator("MACD", "", "CONST SHORT=12;CONST LONG=26;CONST MID=9;\r\nDIF:EMA(CLOSE,SHORT)-EMA(CLOSE,LONG);\rDEA:EMA(DIF,MID);\rMACD:(DIF-DEA)*2,COLORSTICK;", "", m_chart.getDivs()[2], true);
            //String t = sb.ToString();
            Security security = new Security();
            security.m_code = "600000.SH";
            security.m_name = "�ַ�����";
            searchSecurity(security);
        }

        /// <summary>
        /// ����XML
        /// </summary>
        /// <param name="xmlPath">XML·��</param>
        public override void load(String xmlPath) {
            loadFile(DataCenter.getAppPath() + "\\config\\chart\\MainFrame.xml", null);
            m_native = getNative();
            initInterface();
            DataCenter.m_mainUI = this;
        }

        /// <summary>
        /// ���
        /// </summary>
        /// <param name="sender">�ؼ�</param>
        /// <param name="item">�˵���</param>
        public void callMenuItemTouchEvent(String eventName, object sender, FCMenuItem item, FCTouchInfo touchInfo, object invoke) {
            String name = item.getName();
            if (name != null && name.Length > 0) {
                bool setChecked = false;
                if (name.StartsWith("CANDLE_")) {
                    if (!m_showMinuteLine) {
                        String candleStye = name.Substring(7);
                        switch (candleStye) {
                            case "STANDARD":
                                m_candle.setStyle(CandleStyle.Rect);
                                break;
                            case "TOWER":
                                m_candle.setStyle(CandleStyle.Tower);
                                break;
                            case "AMERICAN":
                                m_candle.setStyle(CandleStyle.American);
                                break;
                            case "CLOSE":
                                m_candle.setStyle(CandleStyle.CloseLine);
                                break;
                        }
                        setChecked = true;
                    }
                }
                //����������
                else if (name.StartsWith("SCALE_")) {
                    String scaleStyle = name.Substring(6);
                    switch (scaleStyle) {
                        case "STANDARD":
                            m_candleDiv.getLeftVScale().setSystem(VScaleSystem.Standard);
                            break;
                        case "LOG":
                            m_candleDiv.getLeftVScale().setSystem(VScaleSystem.Logarithmic);
                            break;
                        case "DIFF":
                            m_candleDiv.getLeftVScale().setType(VScaleType.EqualDiff);
                            break;
                        case "EQUALRATIO":
                            m_candleDiv.getLeftVScale().setType(VScaleType.EqualRatio);
                            break;
                        case "DIVIDE":
                            m_candleDiv.getLeftVScale().setType(VScaleType.Divide);
                            break;
                        case "PERCENT":
                            m_candleDiv.getLeftVScale().setType(VScaleType.Percent);
                            break;
                        case "GOLDENRATIO":
                            m_candleDiv.getLeftVScale().setType(VScaleType.GoldenRatio);
                            break;
                        case "REVERSEH":
                            m_chart.setReverseHScale(!m_chart.getReverseHScale());
                            break;
                        case "REVERSEV":
                            m_reverseVScale = !m_reverseVScale;
                            List<ChartDiv> divs = m_chart.getDivs();
                            int divsSize = divs.Count;
                            for (int i = 0; i < divsSize; i++) {
                                ChartDiv div = divs[i];
                                if (div != m_volumeDiv) {
                                    div.getLeftVScale().setReverse(m_reverseVScale);
                                    div.getRightVScale().setReverse(m_reverseVScale);
                                }
                            }
                            break;
                    }
                }
                //����
                else if (name.StartsWith("LAYOUT_")) {
                    String type = name.Substring(7);
                    switch (type) {
                        case "ADDBLANKDIV":
                            addBlankDiv();
                            break;
                        case "REMOVEBLANKDIVS":
                            removeBlankDivs(true);
                            break;
                    }
                }
                //�л�����
                else if (name.StartsWith("CYCLE_")) {
                    String type = name.Substring(6);
                    int cycle = 0;
                    switch (type) {
                        case "MINUTELINE":
                            m_showMinuteLine = true;
                            break;
                        case "1MINUTE":
                            cycle = 1;
                            break;
                        case "5MINUTE":
                            cycle = 5;
                            break;
                        case "15MINUTE":
                            cycle = 15;
                            break;
                        case "30MINUTE":
                            cycle = 30;
                            break;
                        case "60MINUTE":
                            cycle = 60;
                            break;
                        case "DAY":
                            cycle = 1440;
                            break;
                        case "WEEK":
                            cycle = 10080;
                            break;
                        case "MONTH":
                            cycle = 43200;
                            break;
                    }
                    changeCycle(cycle);
                    setChecked = true;
                }
                //��Ȩ��ʽ
                else if (name.StartsWith("SUBSCRIPTION_")) {
                    if (!m_showMinuteLine) {
                        String type = name.Substring(13);
                        switch (type) {
                            case "NONE":
                                m_subscription = 0;
                                break;
                            case "FRONT":
                                m_subscription = 1;
                                break;
                            case "BACK":
                                m_subscription = 2;
                                break;
                        }
                        String securityCode = m_latestDiv.getSecurityCode();
                        if (securityCode != null && securityCode.Length > 0) {
                            Security security = new Security();
                            m_securityService.getSecurityByCode(securityCode, ref security);
                            searchSecurity(security);
                        }
                        setChecked = true;
                    }
                }
                if (setChecked) {
                    List<FCMenuItem> items = item.getParentItem().getItems();
                    int itemsSize = items.Count;
                    for (int i = 0; i < itemsSize; i++) {
                        items[i].setChecked(items[i] == item);
                    }
                }
                m_native.update();
                m_native.invalidate();
            }
        }

        /// <summary>
        /// �������ݻص�
        /// </summary>
        /// <param name="message">��Ϣ</param>
        public void callListenerMessageEvent(Object sender, FCMessage message, Object invoke) {
            FCMessage copyMessage = new FCMessage();
            copyMessage.copy(message);
            m_chart.beginInvoke(copyMessage);
        }

        /// <summary>
        /// �Ƴ�����ӵĿհײ�
        /// </summary>
        /// <param name="update">�Ƿ���²���</param>
        public void removeBlankDivs(bool update) {
            List<ChartDiv> removeDivs = new List<ChartDiv>();
            //��ȡҪ�Ƴ��Ĳ�
            foreach (ChartDiv div in m_chart.getDivs()) {
                if (div != m_candleDiv && div != m_volumeDiv) {
                    if (div.getShapes(SortType.None).Count == 0) {
                        removeDivs.Add(div);
                        m_divs.Remove(div);
                    }
                }
            }
            //�Ƴ���
            int removeDivSize = removeDivs.Count;
            for (int i = 0; i < removeDivSize; i++) {
                m_chart.removeDiv(removeDivs[i]);
            }
            //��������X��
            List<ChartDiv> divsCopy = m_chart.getDivs();
            int divSize = divsCopy.Count;
            for (int i = 0; i < divSize; i++) {
                if (i == divSize - 1) {
                    divsCopy[i].getHScale().setVisible(true);
                    divsCopy[i].getHScale().setHeight(22);
                }
                else {
                    divsCopy[i].getHScale().setVisible(false);
                    divsCopy[i].getHScale().setHeight(0);
                }
            }
            if (update) {
                m_chart.update();
                m_native.invalidate();
            }
        }

        /// <summary>
        /// �Ҽ��˵��ɼ�״̬�ı��¼�
        /// </summary>
        /// <param name="sender">������</param>
        private void rightMenuVisibleChanged(object sender, object invoke) {
           
        }

        /// <summary>
        /// ˢ������
        /// </summary>
        public void refreshData() {
            if (m_showMinuteLine) {
                m_candleDiv.getLeftVScale().setTextColor2(MyColor.USERCOLOR97);
                m_candleDiv.getRightVScale().setTextColor2(MyColor.USERCOLOR97);
                m_candleDiv.getRightVScale().setType(VScaleType.Percent);
                m_candle.setDownColor(MyColor.USERCOLOR3);
                m_candle.setStyle(CandleStyle.CloseLine);
                m_candle.setTagColor(FCColor.None);
                m_candle.setUpColor(MyColor.USERCOLOR3);
                m_bar.setStyle(BarStyle.Line);
                m_minuteLine.setVisible(true);
                m_minuteAvgLine.setVisible(true);
                m_candle.setVisible(false);
                m_volumeDiv.getLeftVScale().setMagnitude(1);
                m_volumeDiv.getRightVScale().setMagnitude(1);
            }
            else {
                m_candleDiv.getLeftVScale().setTextColor2(FCColor.None);
                m_candleDiv.getRightVScale().setTextColor2(MyColor.USERCOLOR97);
                m_candleDiv.getRightVScale().setType(VScaleType.Percent);
                m_candle.setDownColor(MyColor.USERCOLOR70);
                m_candle.setStyle(CandleStyle.Rect);
                m_candle.setTagColor(MyColor.USERCOLOR3);
                m_candle.setUpColor(MyColor.USERCOLOR69);
                m_bar.setStyle(BarStyle.Rect);
                m_minuteLine.setVisible(false);
                m_candle.setVisible(true);
                m_minuteAvgLine.setVisible(false);
                m_volumeDiv.getLeftVScale().setMagnitude(1000);
                m_volumeDiv.getRightVScale().setMagnitude(1000);
            }
            int indicatorSize = m_indicators.Count;
            for (int i = 0; i < indicatorSize; i++) {
                FCScript indicator = m_indicators[i];
                ChartDiv div = indicator.getDiv();
                if (div == m_candleDiv) {
                    //������ʾ����
                    List<BaseShape> shapes = indicator.getShapes();
                    int shapesSize = shapes.Count;
                    for (int j = 0; j < shapesSize; j++) {
                        BaseShape shape = shapes[j];
                        shape.setVisible(!m_showMinuteLine);
                    }
                    //������ʾ����
                    List<ChartTitle> titles = div.getTitleBar().getTitles();
                    int titlesSize = titles.Count;
                    for (int j = 0; j < titlesSize; j++) {
                        ChartTitle title = titles[j];
                        if (title.getFieldName() == KeyFields.CLOSE_INDEX) {
                            title.setVisible(m_showMinuteLine);
                        }
                        else {
                            title.setVisible(!m_showMinuteLine);
                        }
                    }
                }
            }
            m_latestDiv.setDigit(m_digit);
            SecurityLatestData latestData = m_latestDiv.getLatestData();
            newData(latestData);
            FCDataTable dataSource = m_chart.getDataSource();
            foreach (ChartDiv div in m_chart.getDivs()) {

                if (div == m_candleDiv) {
                    double lastClose = 0;
                    if (latestData != null && latestData.m_code != null && latestData.m_code.Length > 0) {
                        lastClose = latestData.m_lastClose;
                    } else {
                        int rowsSize = dataSource.getRowsCount();
                        if (rowsSize > 0) {
                            if (m_showMinuteLine) {
                                lastClose = dataSource.get2(0, KeyFields.CLOSE_INDEX);
                            } else {
                                if (rowsSize == 1) {
                                    lastClose = dataSource.get2(0, KeyFields.OPEN_INDEX);
                                } else {
                                    lastClose = dataSource.get2(rowsSize - 2, KeyFields.CLOSE_INDEX);
                                }
                            }
                        }
                    }
                    if (m_showMinuteLine) {
                        div.getLeftVScale().setMidValue(lastClose);
                    } else {
                        div.getLeftVScale().setMidValue(0);
                    }
                    div.getRightVScale().setMidValue(lastClose);
                }
                if (div != m_volumeDiv) {
                    div.getLeftVScale().setDigit(m_digit);
                    div.getRightVScale().setDigit(m_digit);
                }
                div.getHScale().setScaleSteps(m_hScaleSteps);
                div.getVGrid().setVisible(m_showMinuteLine);
            }
        }

        /// <summary>
        /// ��ѯ��Ʊ
        /// </summary>
        /// <param name="security">��Ʊ</param>
        public void searchSecurity(Security security) {
            if (m_showMinuteLine) {
                m_chart.setAutoFillHScale(true);
                if (m_candleHScalePixel > 0) {
                    m_candleHScalePixel = m_chart.getHScalePixel();
                }
            }
            else {
                m_chart.setAutoFillHScale(false);
                if (m_candleHScalePixel == 0) {
                    m_candleHScalePixel = 9;
                }
                m_chart.setHScalePixel(m_candleHScalePixel);
            }
            bool showCrossLine = m_chart.showCrossLine();
            m_index = -1;
            m_chart.clear();
            m_chart.setShowCrossLine(showCrossLine);
            System.GC.Collect();
            m_searchSecurity = security;

            int cycle = getCycle();
            if (cycle <= 60) {
                if (m_showMinuteLine) {
                    m_candleDiv.getTitleBar().setText("��ʱ��");
                } else {
                    m_candleDiv.getTitleBar().setText(cycle.ToString() + "������");
                }
            } else {
                if (cycle == 1440) {
                    m_candleDiv.getTitleBar().setText("����");
                } else if (cycle == 10080) {
                    m_candleDiv.getTitleBar().setText("����");
                } else if (cycle == 43200) {
                    m_candleDiv.getTitleBar().setText("����");
                }
            }
            DataCenter.m_historyServiceClient.reqData(m_requestID2, security.m_code, cycle, -1, -1);
            m_latestDiv.setSecurityCode(security.m_code);
            m_latestDiv.setSecurityName(security.m_name);
            m_chart.update();
            m_native.invalidate();
        }

        /// <summary>
        /// ��ʾ��ʾ����
        /// </summary>
        /// <param name="text">�ı�</param>
        /// <param name="caption">����</param>
        /// <param name="uType">��ʽ</param>
        /// <returns>���</returns>
        public int showMessageBox(String text, String caption, int uType) {
            MessageBox.Show(text, caption);
            return 1;
        }


        /// <summary>
        /// ��ʾ���̾����
        /// </summary>
        /// <param name="key">����</param>
        public void showSearchDiv(char key) {
            FCView focusedView = m_native.getFocusedView();
            if (focusedView != null) {
                String name = focusedView.getName();
                if (isWindowShowing() && name != "txtSearch") {
                    return;
                }
                if (!(focusedView is FCTextBox) || (m_searchDiv != null && focusedView == m_searchDiv.getSearchTextBox())
                    || name == "txtSearch") {
                    Keys keyData = (Keys)key;
                    //�������̾���
                    if (m_searchDiv == null) {
                        m_searchDiv = new SearchDiv();
                        m_searchDiv.setPopup(true);
                        m_searchDiv.setSize(new FCSize(240, 200));
                        m_searchDiv.setVisible(false);
                        m_native.addView(m_searchDiv);
                        m_searchDiv.bringToFront();
                        m_searchDiv.setMainFrame(this);
                    }
                    //�˳�
                    if (keyData == Keys.Escape) {
                        m_searchDiv.setVisible(false);
                        m_searchDiv.invalidate();
                    }
                    //�л���ʱͼ��K��
                    else if (keyData == Keys.F5) {
                        m_showMinuteLine = !m_showMinuteLine;
                        if (m_showMinuteLine) {
                            m_cycle = 0;
                        }
                        else {
                            m_cycle = 1440;
                        }
                        String securityCode = m_latestDiv.getSecurityCode();
                        if (securityCode != null && securityCode.Length > 0) {
                            Security security = new Security();
                            m_securityService.getSecurityByCode(securityCode, ref security);
                            searchSecurity(security);
                        }
                    }
                    //����
                    else {
                        if (!m_searchDiv.isVisible()) {
                            char ch = '\0';
                            if ((keyData >= Keys.D0) && (keyData <= Keys.D9)) {
                                ch = (char)((0x30 + keyData) - 0x30);
                            }
                            else if ((keyData >= Keys.A) && (keyData <= Keys.Z)) {
                                ch = (char)((0x41 + keyData) - 0x41);
                            }
                            else if ((keyData >= Keys.NumPad0) && (keyData <= Keys.NumPad9)) {
                                ch = (char)((0x30 + keyData) - 0x60);
                            }
                            if (ch != '\0') {
                                FCSize size = m_native.getHost().getSize();
                                FCPoint location = new FCPoint(size.cx - m_searchDiv.getWidth(), size.cy - m_searchDiv.getHeight());
                                if (name == "txtSearch") {
                                    FCPoint fPoint = new FCPoint(0, 0);
                                    fPoint = focusedView.pointToNative(fPoint);
                                    location = new FCPoint(fPoint.x, fPoint.y - m_searchDiv.getHeight() + focusedView.getHeight());
                                    m_searchDiv.setCategoryID(focusedView.getTag().ToString());
                                }
                                else {
                                    m_searchDiv.setCategoryID("");
                                }
                                m_searchDiv.setLocation(location);
                                m_searchDiv.getSearchTextBox().setText("");
                                m_searchDiv.filterSearch();
                                m_searchDiv.setVisible(true);
                                m_searchDiv.getSearchTextBox().setFocused(true);
                                m_searchDiv.update();
                                m_searchDiv.invalidate();
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// K�߿ؼ�
    /// </summary>
    public class MyChartDiv : FCChart, IOutReWrite
    {
        /// <summary>
        /// ����K��
        /// </summary>
        public MyChartDiv()
        {
        }

        /// <summary>
        /// ����K��
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="div">Ҫ���ƵĲ�</param>
        /// <param name="cs">K��</param>
        public override void onPaintCandle(FCPaint paint, ChartDiv div, CandleShape cs)
        {
            int visibleMaxIndex = -1, visibleMinIndex = -1;
            double visibleMax = 0, visibleMin = 0;
            double vmax = double.NaN, vmin = double.NaN;
            if (cs.fillVScale())
            {
                getShapeMaxMin(cs, ref vmax, ref vmin);
            }
            int x = 0, y = 0;
            ArrayList<FCPoint> points = new ArrayList<FCPoint>();
            int ciHigh = m_dataSource.getColumnIndex(cs.getHighField());
            int ciLow = m_dataSource.getColumnIndex(cs.getLowField());
            int ciOpen = m_dataSource.getColumnIndex(cs.getOpenField());
            int ciClose = m_dataSource.getColumnIndex(cs.getCloseField());
            int ciStyle = m_dataSource.getColumnIndex(cs.getStyleField());
            int ciClr = m_dataSource.getColumnIndex(cs.getColorField());
            int defaultLineWidth = 1;
            if (!isOperating() && m_crossStopIndex != -1)
            {
                if (selectCandle(div, getTouchPoint().y, cs.getHighField(), cs.getLowField(), cs.getStyleField(), cs.getAttachVScale(), m_crossStopIndex, vmax, vmin))
                {
                    defaultLineWidth = 2;
                }
            }
            for (int i = m_firstVisibleIndex; i <= m_lastVisibleIndex; i++)
            {
                int thinLineWidth = 1;
                if (i == m_crossStopIndex)
                {
                    thinLineWidth = defaultLineWidth;
                }
                //��ʽ
                int style = -10000;
                switch (cs.getStyle())
                {
                    case CandleStyle.Rect:
                        style = 0;
                        break;
                    case CandleStyle.American:
                        style = 3;
                        break;
                    case CandleStyle.CloseLine:
                        style = 4;
                        break;
                    case CandleStyle.Tower:
                        style = 5;
                        break;
                }
                //�Զ�����ʽ
                if (ciStyle != -1)
                {
                    double defineStyle = m_dataSource.get3(i, ciStyle);
                    if (!double.IsNaN(defineStyle))
                    {
                        style = (int)defineStyle;
                    }
                }
                if (style == 10000)
                {
                    continue;
                }
                //��ȡֵ
                double open = m_dataSource.get3(i, ciOpen);
                double high = m_dataSource.get3(i, ciHigh);
                double low = m_dataSource.get3(i, ciLow);
                double close = m_dataSource.get3(i, ciClose);
                if (double.IsNaN(open) || double.IsNaN(high) || double.IsNaN(low) || double.IsNaN(close))
                {
                    if (i != m_lastVisibleIndex || style != 4)
                    {
                        continue;
                    }
                }
                int scaleX = (int)getX(i);
                if (cs.showMaxMin())
                {
                    //���ÿɼ����������Сֵ������
                    if (i == m_firstVisibleIndex)
                    {
                        //��ʼֵ
                        visibleMaxIndex = i;
                        visibleMinIndex = i;
                        visibleMax = high;
                        visibleMin = low;
                    }
                    else
                    {
                        //���ֵ
                        if (high > visibleMax)
                        {
                            visibleMax = high;
                            visibleMaxIndex = i;
                        }
                        //��Сֵ
                        if (low < visibleMin)
                        {
                            visibleMin = low;
                            visibleMinIndex = i;
                        }
                    }
                }
                //��ȡ��ֵ����Yֵ
                float highY = getY(div, high, cs.getAttachVScale(), vmax, vmin);
                float openY = getY(div, open, cs.getAttachVScale(), vmax, vmin);
                float lowY = getY(div, low, cs.getAttachVScale(), vmax, vmin);
                float closeY = getY(div, close, cs.getAttachVScale(), vmax, vmin);
                int cwidth = (int)(m_hScalePixel * 2 / 3);
                if (cwidth % 2 == 0)
                {
                    cwidth += 1;
                }
                if (cwidth < 3)
                {
                    cwidth = 1;
                }
                int xsub = cwidth / 2;
                if (xsub < 1)
                {
                    xsub = 1;
                }
                switch (style)
                {
                    //������
                    case 3:
                        {
                            long color = cs.getUpColor();
                            if (open > close)
                            {
                                color = cs.getDownColor();
                            }
                            if (ciClr != -1)
                            {
                                double defineColor = m_dataSource.get3(i, ciClr);
                                if (!double.IsNaN(defineColor))
                                {
                                    color = (long)defineColor;
                                }
                            }
                            if ((int)highY != (int)lowY)
                            {
                                if (m_hScalePixel <= 3)
                                {
                                    drawThinLine(paint, color, thinLineWidth, scaleX, highY, scaleX, lowY);
                                }
                                else
                                {
                                    drawThinLine(paint, color, thinLineWidth, scaleX, highY, scaleX, lowY);
                                    drawThinLine(paint, color, thinLineWidth, scaleX - xsub, openY, scaleX, openY);
                                    drawThinLine(paint, color, thinLineWidth, scaleX, closeY, scaleX + xsub, closeY);
                                }
                            }
                            else
                            {
                                drawThinLine(paint, color, thinLineWidth, scaleX - xsub, closeY, scaleX + xsub, closeY);
                            }
                        }
                        break;
                    //������
                    case 4:
                        {
                            onPaintPolyline(paint, div, cs.getUpColor(), FCColor.None, cs.getColorField(), defaultLineWidth, PolylineStyle.SolidLine, close, cs.getAttachVScale(), scaleX, (int)closeY, i, points, ref x, ref y, vmax, vmin);
                            break;
                        }
                    default:
                        {
                            //����
                            if (open <= close)
                            {
                                //��ȡ���ߵĸ߶�
                                float recth = getUpCandleHeight(close, open, div.getVScale(cs.getAttachVScale()).getVisibleMax(), div.getVScale(cs.getAttachVScale()).getVisibleMin(), div.getWorkingAreaHeight() - div.getVScale(cs.getAttachVScale()).getPaddingBottom() - div.getVScale(cs.getAttachVScale()).getPaddingTop());
                                if (recth < 1)
                                {
                                    recth = 1;
                                }
                                //��ȡ���ߵľ���
                                int rcUpX = scaleX - xsub, rcUpTop = (int)closeY, rcUpBottom = (int)openY, rcUpW = cwidth, rcUpH = (int)recth;
                                if (openY < closeY)
                                {
                                    rcUpTop = (int)openY;
                                    rcUpBottom = (int)closeY;
                                }
                                long upColor = FCColor.None;
                                int colorField = cs.getColorField();
                                if (colorField != FCDataTable.NULLFIELD)
                                {
                                    double defineColor = m_dataSource.get2(i, colorField);
                                    if (!double.IsNaN(defineColor))
                                    {
                                        upColor = (long)defineColor;
                                    }
                                }
                                if (upColor == FCColor.None)
                                {
                                    if (open == close && cs.getMidColor() != FCColor.None)
                                    {
                                        upColor = cs.getMidColor();
                                    }
                                    else
                                    {
                                        upColor = cs.getUpColor();
                                    }
                                }
                                switch (style)
                                {
                                    //����
                                    case 0:
                                    case 1:
                                    case 2:
                                        if ((int)highY != (int)lowY)
                                        {
                                            drawThinLine(paint, upColor, thinLineWidth, scaleX, highY, scaleX, lowY);
                                            if (m_hScalePixel > 3)
                                            {
                                                //�豳��
                                                if ((int)openY == (int)closeY)
                                                {
                                                    drawThinLine(paint, upColor, thinLineWidth, rcUpX, rcUpBottom, rcUpX + rcUpW, rcUpBottom);
                                                }
                                                else
                                                {
                                                    FCRect rcUp = new FCRect(rcUpX, rcUpTop, rcUpX + rcUpW, rcUpBottom);
                                                    if (style == 0 || style == 1)
                                                    {
                                                        if (rcUpW > 0 && rcUpH > 0 && m_hScalePixel > 3)
                                                        {
                                                            //paint.fillRect(div.getBackColor(), rcUp);
                                                        }
                                                        paint.fillRect(upColor, rcUp);
                                                    }
                                                    else if (style == 2)
                                                    {
                                                        paint.fillRect(upColor, rcUp);
                                                        if (thinLineWidth > 1)
                                                        {
                                                            paint.fillRect(upColor, rcUp);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            drawThinLine(paint, upColor, thinLineWidth, scaleX - xsub, closeY, scaleX + xsub, closeY);
                                        }
                                        break;
                                    //������
                                    case 5:
                                        {
                                            double lOpen = m_dataSource.get3(i - 1, ciOpen);
                                            double lClose = m_dataSource.get3(i - 1, ciClose);
                                            double lHigh = m_dataSource.get3(i - 1, ciHigh);
                                            double lLow = m_dataSource.get3(i - 1, ciLow);
                                            float top = highY;
                                            float bottom = lowY;
                                            if ((int)highY > (int)lowY)
                                            {
                                                top = lowY;
                                                bottom = highY;
                                            }
                                            if (i == 0 || double.IsNaN(lOpen) || double.IsNaN(lClose) || double.IsNaN(lHigh) || double.IsNaN(lLow))
                                            {
                                                if (m_hScalePixel <= 3)
                                                {
                                                    drawThinLine(paint, upColor, thinLineWidth, rcUpX, top, rcUpX, bottom);
                                                }
                                                else
                                                {
                                                    int rcUpHeight = (int)Math.Abs(bottom - top == 0 ? 1 : bottom - top);
                                                    if (rcUpW > 0 && rcUpHeight > 0)
                                                    {
                                                        FCRect rcUp = new FCRect(rcUpX, top, rcUpX + rcUpW, top + rcUpHeight);
                                                        paint.fillRect(upColor, rcUp);
                                                        if (thinLineWidth > 1)
                                                        {
                                                            paint.drawRect(upColor, thinLineWidth, 0, rcUp);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (m_hScalePixel <= 3)
                                                {
                                                    drawThinLine(paint, upColor, thinLineWidth, rcUpX, top, rcUpX, bottom);
                                                }
                                                else
                                                {
                                                    int rcUpHeight = (int)Math.Abs(bottom - top == 0 ? 1 : bottom - top);
                                                    if (rcUpW > 0 && rcUpHeight > 0)
                                                    {
                                                        FCRect rcUp = new FCRect(rcUpX, top, rcUpX + rcUpW, top + rcUpHeight);
                                                        paint.fillRect(upColor, rcUp);
                                                        if (thinLineWidth > 1)
                                                        {
                                                            paint.drawRect(upColor, thinLineWidth, 0, rcUp);
                                                        }
                                                    }
                                                }
                                                //��һ�ɼ�Ϊ�µ�����δ������ߵ㲿��
                                                if (lClose < lOpen && low < lHigh)
                                                {
                                                    //��ȡ����
                                                    int tx = rcUpX;
                                                    int ty = (int)getY(div, lHigh, cs.getAttachVScale(), vmax, vmin);
                                                    if (high < lHigh)
                                                    {
                                                        ty = (int)highY;
                                                    }
                                                    int width = rcUpW;
                                                    int height = (int)lowY - ty;
                                                    if (height > 0)
                                                    {
                                                        if (m_hScalePixel <= 3)
                                                        {
                                                            drawThinLine(paint, cs.getDownColor(), thinLineWidth, tx, ty, tx, ty + height);
                                                        }
                                                        else
                                                        {
                                                            if (width > 0 && height > 0)
                                                            {
                                                                FCRect tRect = new FCRect(tx, ty, tx + width, ty + height);
                                                                paint.fillRect(cs.getDownColor(), tRect);
                                                                if (thinLineWidth > 1)
                                                                {
                                                                    paint.drawRect(cs.getDownColor(), thinLineWidth, 0, tRect);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                            //����
                            else
                            {
                                //��ȡ���ߵĸ߶�
                                float recth = getDownCandleHeight(close, open, div.getVScale(cs.getAttachVScale()).getVisibleMax(), div.getVScale(cs.getAttachVScale()).getVisibleMin(), div.getWorkingAreaHeight() - div.getVScale(cs.getAttachVScale()).getPaddingBottom() - div.getVScale(cs.getAttachVScale()).getPaddingTop());
                                if (recth < 1)
                                {
                                    recth = 1;
                                }
                                //��ȡ���ߵľ���
                                int rcDownX = scaleX - xsub, rcDownTop = (int)openY, rcDownBottom = (int)closeY, rcDownW = cwidth, rcDownH = (int)recth;
                                if (closeY < openY)
                                {
                                    rcDownTop = (int)closeY;
                                    rcDownBottom = (int)openY;
                                }
                                long downColor = FCColor.None;
                                if (ciClr != -1)
                                {
                                    double defineColor = m_dataSource.get3(i, ciClr);
                                    if (!double.IsNaN(defineColor))
                                    {
                                        downColor = (long)defineColor;
                                    }
                                }
                                if (downColor == FCColor.None)
                                {
                                    if (open == close && cs.getMidColor() != FCColor.None)
                                    {
                                        downColor = cs.getMidColor();
                                    }
                                    else
                                    {
                                        downColor = cs.getDownColor();
                                    }
                                }
                                switch (style)
                                {
                                    //��׼
                                    case 0:
                                    case 1:
                                    case 2:
                                        if ((int)highY != (int)lowY)
                                        {
                                            drawThinLine(paint, downColor, thinLineWidth, scaleX, highY, scaleX, lowY);
                                            if (m_hScalePixel > 3)
                                            {
                                                FCRect rcDown = new FCRect(rcDownX, rcDownTop, rcDownX + rcDownW, rcDownBottom);
                                                if (style == 1)
                                                {
                                                    if (rcDownW > 0 && rcDownH > 0 && m_hScalePixel > 3)
                                                    {
                                                        paint.fillRect(div.getBackColor(), rcDown);
                                                    }
                                                    paint.drawRect(downColor, thinLineWidth, 0, rcDown);
                                                }
                                                else if (style == 0 || style == 2)
                                                {
                                                    paint.fillRect(downColor, rcDown);
                                                    if (thinLineWidth > 1)
                                                    {
                                                        paint.drawRect(downColor, thinLineWidth, 0, rcDown);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            drawThinLine(paint, downColor, thinLineWidth, scaleX - xsub, closeY, scaleX + xsub, closeY);
                                        }
                                        break;
                                    //������
                                    case 5:
                                        double lOpen = m_dataSource.get3(i - 1, ciOpen);
                                        double lClose = m_dataSource.get3(i - 1, ciClose);
                                        double lHigh = m_dataSource.get3(i - 1, ciHigh);
                                        double lLow = m_dataSource.get3(i - 1, ciLow);
                                        float top = highY;
                                        float bottom = lowY;
                                        if ((int)highY > (int)lowY)
                                        {
                                            top = lowY;
                                            bottom = highY;
                                        }
                                        if (i == 0 || double.IsNaN(lOpen) || double.IsNaN(lClose) || double.IsNaN(lHigh) || double.IsNaN(lLow))
                                        {
                                            if (m_hScalePixel <= 3)
                                            {
                                                drawThinLine(paint, downColor, thinLineWidth, rcDownX, top, rcDownX, bottom);
                                            }
                                            else
                                            {
                                                int rcDownHeight = (int)Math.Abs(bottom - top == 0 ? 1 : bottom - top);
                                                if (rcDownW > 0 && rcDownHeight > 0)
                                                {
                                                    FCRect rcDown = new FCRect(rcDownX, top, rcDownX + rcDownW, rcDownBottom);
                                                    paint.fillRect(downColor, rcDown);
                                                    if (thinLineWidth > 1)
                                                    {
                                                        paint.drawRect(downColor, thinLineWidth, 0, rcDown);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //�Ȼ����߲���
                                            if (m_hScalePixel <= 3)
                                            {
                                                drawThinLine(paint, downColor, thinLineWidth, rcDownX, top, rcDownX, bottom);
                                            }
                                            else
                                            {
                                                int rcDownHeight = (int)Math.Abs(bottom - top == 0 ? 1 : bottom - top);
                                                if (rcDownW > 0 && rcDownHeight > 0)
                                                {
                                                    FCRect rcDown = new FCRect(rcDownX, top, rcDownX + rcDownW, rcDownBottom);
                                                    paint.fillRect(downColor, rcDown);
                                                    if (thinLineWidth > 1)
                                                    {
                                                        paint.drawRect(downColor, thinLineWidth, 0, rcDown);
                                                    }
                                                }
                                            }
                                            //��һ�ɼ�Ϊ���ǣ���δ������ߵ㲿��
                                            if (lClose >= lOpen && high > lLow)
                                            {
                                                //��ȡ����
                                                int tx = rcDownX;
                                                int ty = (int)highY;
                                                int width = rcDownW;
                                                int height = (int)Math.Abs(getY(div, lLow, cs.getAttachVScale(), vmax, vmin) - ty);
                                                if (low > lLow)
                                                {
                                                    height = (int)lowY - ty;
                                                }
                                                if (height > 0)
                                                {
                                                    if (m_hScalePixel <= 3)
                                                    {
                                                        drawThinLine(paint, cs.getUpColor(), thinLineWidth, tx, ty, tx, ty + height);
                                                    }
                                                    else
                                                    {
                                                        if (width > 0 && height > 0)
                                                        {
                                                            FCRect tRect = new FCRect(tx, ty, tx + width, ty + height);
                                                            paint.fillRect(cs.getUpColor(), new FCRect(tx, ty, tx + width, ty + height));
                                                            if (thinLineWidth > 1)
                                                            {
                                                                paint.drawRect(cs.getUpColor(), thinLineWidth, 0, tRect);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                        }
                }
                //����ѡ��
                if (cs.isSelected())
                {
                    int kPInterval = m_maxVisibleRecord / 30;
                    if (kPInterval < 2)
                    {
                        kPInterval = 3;
                    }
                    if (i % kPInterval == 0)
                    {
                        if (!double.IsNaN(open) && !double.IsNaN(high) && !double.IsNaN(low) && !double.IsNaN(close))
                        {
                            if (closeY >= div.getTitleBar().getHeight()
                                && closeY <= div.getHeight() - div.getHScale().getHeight())
                            {
                                FCRect rect = new FCRect(scaleX - 3, (int)closeY - 4, scaleX + 4, (int)closeY + 3);
                                paint.fillRect(cs.getSelectedColor(), rect);
                            }
                        }
                    }
                }
            }
            onPaintCandleEx(paint, div, cs, visibleMaxIndex, visibleMax, visibleMinIndex, visibleMin);
        }

        public FCView createView(UIXmlEx uiXmlEx, System.Xml.XmlNode node)
        {
            return new MyChartDiv();
        }

        public bool moreAnalysis(FCView view, UIXmlEx uiXmlEx, System.Xml.XmlNode node)
        {
            return false;
        }
    }
}
