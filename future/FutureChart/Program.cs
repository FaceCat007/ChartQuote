/*
 * iChart����ϵͳ(�ǿ�Դ)
 * �Ϻ����è��Ϣ�������޹�˾
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Net;
using System.Text;
using FaceCat;

namespace future
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DataCenter.startService();
            PreViewForm preViewForm = new PreViewForm();
            preViewForm.loadFile("", false, new FCSize(1000, 800));
            preViewForm.Text = "K������";
            MainForm mainForm = new MainForm();
            preViewForm.addForm2(mainForm);
            preViewForm.m_xml.findView("divWindow").setPadding(new FCPadding(2));
            preViewForm.m_xml.getNative().update();
            preViewForm.m_xml.getNative().invalidate();
            Application.Run(preViewForm);
        }
    }

    public class AppHost
    {
        /// <summary>
        /// ��������
        /// </summary>
        public static PreViewForm run(string cmd)
        {
            DataCenter.startService();
            PreViewForm preViewForm = FaceCatAPI.createPreViewForm();
            preViewForm.setIsMerge(true);
            preViewForm.loadFile("", false, new FCSize(1000, 800));
            preViewForm.Text = "K������";
            preViewForm.setUrl("https://www.jjmfc.com/app_Chart.html");
            preViewForm.setIconViewName("app_Chart");
            MainForm mainForm = new MainForm();
            preViewForm.addForm(mainForm);
            preViewForm.m_xml.findView("divWindow").setPadding(new FCPadding(2));
            preViewForm.Show();
            return preViewForm;
        }
    }
}