/*
 * iChart行情系统(开源)
 * 上海卷卷猫信息技术有限公司
 */

using System;
using System.Collections.Generic;
using System.Text;
using FaceCat;

namespace chart
{
    /// <summary>
    /// 键盘精灵
    /// </summary>
    public class SearchDiv : FCMenu, FCGridCellTouchEventCallBack, FCKeyEventCallBack, FCEventCallBack, IOutReWrite
    {
        /// <summary>
        /// 创建键盘精灵
        /// </summary>
        public SearchDiv()
        {
            setBackColor(FCColor.None);
            setIsWindow(true);
        }

        public FCView createView(UIXmlEx uiXmlEx, System.Xml.XmlNode node)
        {
            return new SearchDiv();
        }

        public bool moreAnalysis(FCView view, UIXmlEx uiXmlEx, System.Xml.XmlNode node)
        {
            return false;
        }


        /// <summary>
        /// 表格控件
        /// </summary>
        private FCGrid m_grid;

        private String m_categoryID;

        /// <summary>
        /// 要添加到的自选股ID
        /// </summary>
        public String getCategoryID() {
            return m_categoryID;
        }

        /// <summary>
        /// 设置自选股ID
        /// </summary>
        /// <param name="value"></param>
        public void setCategoryID(String value) {
            m_categoryID = value;
        }

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

        private FCTextBox m_searchTextBox;

        /// <summary>
        /// 获取或设置查找文本框
        /// </summary>
        public FCTextBox getSearchTextBox()
        {
            return m_searchTextBox;
        }

        /// <summary>
        /// 设置查找文本框
        /// </summary>
        /// <param name="value"></param>
        public void setSearchTextBox(FCTextBox value) {
            m_searchTextBox = value;
        }

        /// <summary>
        /// 销毁方法
        /// </summary>
        public override void delete()
        {
            if (!isDeleted())
            {
                if (m_grid != null)
                {
                    m_grid.removeEvent(this, "ongridcellclick");
                    m_grid.removeEvent(this, "onkeydown");
                }
                if (m_searchTextBox != null)
                {
                    m_searchTextBox.removeEvent(this, "ontextchanged");
                    m_searchTextBox.removeEvent(this, "onkeydown");
                }
            }
            base.delete();
        }

        /// <summary>
        /// 过滤查找
        /// </summary>
        public void filterSearch()
        {
            String sText = m_searchTextBox.getText().ToUpper();
            m_grid.beginUpdate();
            m_grid.clearRows();
            int row = 0;
            CList<Security> securities = new CList<Security>();
            DataCenter.m_securityService.filterSecurities(sText, securities);
            if (securities != null)
            {
                int rowCount = securities.size();
                for (int i = 0; i < rowCount; i++)
                {
                    Security security = securities.get(i);
                    FCGridRow gridRow = new FCGridRow();
                    m_grid.addRow(gridRow);
                    gridRow.addCell(0, new FCGridStringCell(security.m_code));
                    gridRow.addCell(1, new FCGridStringCell(security.m_name));
                    row++;
                }
            }
            securities.delete();
            m_grid.endUpdate();
        }

        /// <summary>
        /// 表格单元格点击事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="cell">单元格</param>
        /// <param name="mp">坐标</param>
        /// <param name="button">按钮</param>
        /// <param name="clicks">点击次数</param>
        /// <param name="delta">滚轮值</param>
        public virtual void callGridCellTouchEvent(String eventName, object sender, FCGridCell cell, FCTouchInfo touchInfo, object invoke)
        {
            if (touchInfo.m_firstTouch && touchInfo.m_clicks == 2)
            {
                onSelectRow();
            }
        }

        /// <summary>
        /// 表格键盘事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="key">按键</param>
        public override void callKeyEvent(String eventName, object sender, char key, object invoke)
        {
            base.callKeyEvent(eventName, sender, key, invoke);
            if (sender == m_searchTextBox) {
                if (key == 13) {
                    onSelectRow();
                } else if (key == 38 || key == 40) {
                    onKeyDown(key);
                }
            } else if (sender == m_grid) {
                if (key == 13) {
                    onSelectRow();
                }
            }
        }

        /// <summary>
        /// 添加控件方法
        /// </summary>
        public override void onLoad()
        {
            base.onLoad();
            if (m_grid == null)
            {
                m_grid = new FCGrid();
                m_grid.setAutoEllipsis(true);
                m_grid.setGridLineColor((FCColor.None));
                m_grid.setSize(new FCSize(240, 200));
                m_grid.addEvent(this, "ongridcellclick", null);
                m_grid.addEvent(this, "onkeydown", null);
                addView(m_grid);
                m_grid.beginUpdate();
                //添加列
                FCGridColumn securityCodeColumn = new FCGridColumn("股票代码");
                securityCodeColumn.setBackColor(MyColor.USERCOLOR94);
                securityCodeColumn.setBorderColor(MyColor.USERCOLOR9);
                securityCodeColumn.setFont(new FCFont("Default", 14, true, false, false));
                securityCodeColumn.setTextColor(MyColor.USERCOLOR28);
                securityCodeColumn.setTextAlign(FCContentAlignment.MiddleLeft);
                securityCodeColumn.setWidth(120);
                m_grid.addColumn(securityCodeColumn);
                FCGridColumn securityNameColumn = new FCGridColumn("股票名称");
                securityNameColumn.setBackColor(MyColor.USERCOLOR94);
                securityNameColumn.setBorderColor(MyColor.USERCOLOR9);
                securityNameColumn.setFont(new FCFont("Default", 14, true, false, false));
                securityNameColumn.setTextColor(MyColor.USERCOLOR28);
                securityNameColumn.setTextAlign(FCContentAlignment.MiddleLeft);
                securityNameColumn.setWidth(110);
                m_grid.addColumn(securityNameColumn);
                m_grid.endUpdate();
            }
            if (m_searchTextBox == null)
            {
                m_searchTextBox = new FCTextBox();
                m_searchTextBox.setLocation(new FCPoint(0, 200));
                m_searchTextBox.setSize(new FCSize(240, 20));
                m_searchTextBox.setFont(new FCFont("Default", 16, true, false, false));
                m_searchTextBox.addEvent(this, "ontextchanged", null);
                m_searchTextBox.addEvent(this, "onkeydown", null);
                addView(m_searchTextBox);
            }
        }

        /// <summary>
        /// 选中行方法
        /// </summary>
        private void onSelectRow()
        {
            List<FCGridRow> rows = m_grid.getSelectedRows();
            if (rows != null && rows.Count > 0)
            {
                FCGridRow selectedRow = rows[0];
                Security security = new Security();
                DataCenter.m_securityService.getSecurityByCode(selectedRow.getCell(0).getText(), ref security);
                setVisible(false);
                invalidate();
                if (m_mainFrame != null)
                {
                    m_mainFrame.searchSecurity(security);
                }
            }
        }

        /// <summary>
        /// 文本框输入
        /// </summary>
        /// <param name="sender">控件</param>
        public override void callEvent(String eventName, object sender, object invoke)
        {
            base.callEvent(eventName, sender, invoke);
            FCTextBox control = sender as FCTextBox;
            setSearchTextBox(control);
            filterSearch();
            String text = control.getText();
            if (text != null && text.Length == 0)
            {
                setVisible(false);
            }
            invalidate();
        }

        /// <summary>
        /// 键盘方法
        /// </summary>
        /// <param name="key">按键</param>
        public override void onKeyDown(char key)
        {
            base.onKeyDown(key);
            if (key == 13)
            {
                onSelectRow();
            }
            else if (key == 38 || key == 40)
            {
                m_grid.onKeyDown(key);
            }
        }

        /// <summary>
        /// 可见状态改变方法
        /// </summary>
        public override void onVisibleChanged()
        {
            if (!isVisible())
            {
                if (m_mainFrame != null)
                {
                    m_mainFrame.getChart().setFocused(true);
                }
            }
            base.onVisibleChanged();
        }
    }
}
