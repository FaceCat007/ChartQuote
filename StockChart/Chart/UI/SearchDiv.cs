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
    /// ���̾���
    /// </summary>
    public class SearchDiv : FCMenu, FCGridCellTouchEventCallBack, FCKeyEventCallBack, FCEventCallBack, IOutReWrite
    {
        /// <summary>
        /// �������̾���
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
        /// ���ؼ�
        /// </summary>
        private FCGrid m_grid;

        private String m_categoryID;

        /// <summary>
        /// Ҫ��ӵ�����ѡ��ID
        /// </summary>
        public String getCategoryID() {
            return m_categoryID;
        }

        /// <summary>
        /// ������ѡ��ID
        /// </summary>
        /// <param name="value"></param>
        public void setCategoryID(String value) {
            m_categoryID = value;
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

        private FCTextBox m_searchTextBox;

        /// <summary>
        /// ��ȡ�����ò����ı���
        /// </summary>
        public FCTextBox getSearchTextBox()
        {
            return m_searchTextBox;
        }

        /// <summary>
        /// ���ò����ı���
        /// </summary>
        /// <param name="value"></param>
        public void setSearchTextBox(FCTextBox value) {
            m_searchTextBox = value;
        }

        /// <summary>
        /// ���ٷ���
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
        /// ���˲���
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
        /// ���Ԫ�����¼�
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="cell">��Ԫ��</param>
        /// <param name="mp">����</param>
        /// <param name="button">��ť</param>
        /// <param name="clicks">�������</param>
        /// <param name="delta">����ֵ</param>
        public virtual void callGridCellTouchEvent(String eventName, object sender, FCGridCell cell, FCTouchInfo touchInfo, object invoke)
        {
            if (touchInfo.m_firstTouch && touchInfo.m_clicks == 2)
            {
                onSelectRow();
            }
        }

        /// <summary>
        /// �������¼�
        /// </summary>
        /// <param name="sender">������</param>
        /// <param name="key">����</param>
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
        /// ��ӿؼ�����
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
                //�����
                FCGridColumn securityCodeColumn = new FCGridColumn("��Ʊ����");
                securityCodeColumn.setBackColor(MyColor.USERCOLOR94);
                securityCodeColumn.setBorderColor(MyColor.USERCOLOR9);
                securityCodeColumn.setFont(new FCFont("Default", 14, true, false, false));
                securityCodeColumn.setTextColor(MyColor.USERCOLOR28);
                securityCodeColumn.setTextAlign(FCContentAlignment.MiddleLeft);
                securityCodeColumn.setWidth(120);
                m_grid.addColumn(securityCodeColumn);
                FCGridColumn securityNameColumn = new FCGridColumn("��Ʊ����");
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
        /// ѡ���з���
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
        /// �ı�������
        /// </summary>
        /// <param name="sender">�ؼ�</param>
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
        /// ���̷���
        /// </summary>
        /// <param name="key">����</param>
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
        /// �ɼ�״̬�ı䷽��
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
