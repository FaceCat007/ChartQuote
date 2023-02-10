/*
 * iChart行情系统(开源)
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using FaceCat;

namespace chart {
    /// <summary>
    /// 示例控件
    /// </summary>
    public partial class MainForm : FCForm {
        /// <summary>
        ///  创建图形控件
        /// </summary>
        public MainForm() {
            InitializeComponent();
            m_chart = new MainFrame();
            m_chart.createNative();

            UIXmlEx.m_outReWrites["floatdiv"] = new FloatDiv();
            UIXmlEx.m_outReWrites["indexdiv"] = new IndexDiv();
            UIXmlEx.m_outReWrites["latestdiv"] = new LatestDiv();
            UIXmlEx.m_outReWrites["searchdiv"] = new SearchDiv();
            UIXmlEx.m_outReWrites["chartexex"] = new MyChartDiv();

            m_xml = m_chart;
            m_native = m_chart.getNative();
            m_native.setPaint(new GdiPlusPaintEx());
            MyColor.setStyle(0);
            m_host = new WinHostEx();
            m_host.setAllowPartialPaint(false);
            m_host.setNative(m_native);
            m_native.setHost(m_host);
            m_host.setHWnd(Handle);
            m_native.setAllowScaleSize(true);
            m_native.setSize(new FCSize(ClientSize.Width, ClientSize.Height));
            String zoomFactor = MyColor.getZoomFactor();
            if (zoomFactor != "nil")
            {
                double scaleFactor = FCTran.strToDouble(zoomFactor);
                m_chart.setScaleFactor(scaleFactor);
            }
            m_chart.resetScaleSize(getClientSize());
            Invalidate();
            m_chart.load(DataCenter.getAppPath() + "\\config\\chart\\MainFrame.xml");
            m_native.update();
            DataCenter.connect();
            DataCenter.loadData(1);
            //m_chart.showLoginWindow();
        }

        /// <summary>
        /// 行情系统
        /// </summary>
        private MainFrame m_chart;

        public override void onClose()
        {
            m_chart.exit();
            base.onClose();
        }

        /// <summary>
        /// 尺寸改变方法
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            if (m_host != null) {
                m_chart.resetScaleSize(getClientSize());
                Invalidate();
            }
        }

        /// <summary>
        /// 鼠标滚动方法
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
            if (m_host.isKeyPress(0x11)) {
                double scaleFactor = m_chart.getScaleFactor();
                if (e.Delta > 0) {
                    if (scaleFactor > 0.2) {
                        scaleFactor -= 0.1;
                    }
                }
                else if (e.Delta < 0) {
                    if (scaleFactor < 10) {
                        scaleFactor += 0.1;
                    }
                }
                m_chart.setScaleFactor(scaleFactor);
                m_chart.resetScaleSize(getClientSize());
                Invalidate();
            }
        }

        /// <summary>
        /// 消息监听
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x100 || m.Msg == 260) {
                if (m_native != null) {
                    char key = (char)m.WParam;
                    m_chart.showSearchDiv(key);
                }
            }
            if (m_host != null) {
                if (m_host.onMessage(ref m) > 0) {
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }
}
