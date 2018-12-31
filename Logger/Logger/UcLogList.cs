using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Diagnostics;

namespace Logger
{
    public partial class UcLogList : UserControl
    {
        #region 单实例多线程安全

        private static volatile UcLogList _instance = null;
        private static readonly object lockHelper = new object();

        public static UcLogList CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                    {
                        _instance = new UcLogList();
                    }
                }
            }
            return _instance;
        }

        public static UcLogList Instance
        {
            get { return CreateInstance(); }
        }

        #endregion

        private Queue logQ = new Queue();

        public UcLogList()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            Thread th = new Thread(new ThreadStart(ShowLog));
            th.IsBackground = true;
            th.Name = "日志显示线程";
            th.Start();
        }

        private void ShowLog()
        {
            try
            {
                int count = 0;
                while (true)
                {
                    object o = null;
                    lock (logQ)
                    {
                        count = logQ.Count;
                        if (count > 0)
                        {
                            o = logQ.Dequeue();
                        }
                    }
                    if (o != null)
                    {
                        SetText((string)o);
                    }
                    if (count - 1 <= 0)
                        Thread.Sleep(100);
                    else
                        Thread.Sleep(1);
                }
            }
            catch { }
        }

        private delegate void SetTextGridCallBack(string str);
        private void SetText(string str)
        {
            try
            {
                if (this.listBox1.InvokeRequired)
                {
                    SetTextGridCallBack cb = new SetTextGridCallBack(SetText);
                    this.listBox1.Invoke(cb, new object[] { str });
                }
                else
                {

                    int index = this.listBox1.Items.Count;
                    if (index > 100)
                    {
                        this.listBox1.Items.RemoveAt(index - 1);
                    }
                     //listBox1.Items.Add(str);
                    listBox1.Items.Insert(0, str);

                   
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.StackTrace);
            }
        }

        ////////
        /// 在listbox中追加状态信息
        /// ////
        public void AddItemToListBox(string str)
        {
            logQ.Enqueue(str);
        }

        public void  Clear()
        {
            listBox1.Text = "";
            listBox1.Items.Clear();
        }

        //private void lbLog_DrawItem(object sender, DrawItemEventArgs e)
        //{
        //    if (e.Index >= 0)
        //    {
        //        e.DrawBackground();
        //        Brush mybsh = Brushes.Black;
        //        string[] arrstr = listBox1.Items[e.Index].ToString().Split('\t');
        //        // 判断是什么类型的标签
        //        if (arrstr[1].ToLower().Contains("error"))
        //        {
        //            mybsh = Brushes.Red;
        //        }
        //        else if (arrstr[1].ToLower().Contains("warning"))
        //        {
        //            mybsh = Brushes.Blue;
        //        }
        //        // 焦点框
        //        e.DrawFocusRectangle();
        //        //文本 
        //        e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, mybsh, e.Bounds, StringFormat.GenericDefault);
        //    }
        //}

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Brush mybsh = Brushes.Black;
                 string[] arrstr = listBox1.Items[e.Index].ToString().Split('\t');
                // 判断是什么类型的标签
                if (arrstr[1].ToLower().Contains("error"))
                {
                    mybsh = Brushes.Red;
                }
                else if (arrstr[1].ToLower().Contains("warning"))
                {
                    mybsh = Brushes.Blue;
                }
                // 焦点框
                e.DrawFocusRectangle();
                //文本 
                // e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, mybsh, e.Bounds, StringFormat.GenericDefault);
                e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, mybsh, e.Bounds, StringFormat.GenericDefault);
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Clear();
        }

    }
}
