using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace Logger
{
   
    public partial class UcLogList2 : UserControl
    {
        #region 单实例多线程安全

        private static volatile UcLogList2 _instance = null;
        private static readonly object lockHelper = new object();

        public static UcLogList2 CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                    {
                        _instance = new UcLogList2();
                    }
                }
            }
            return _instance;
        }

        public static UcLogList2 Instance
        {
            get { return CreateInstance(); }
        }

        #endregion

        private Queue logQ = new Queue();

        public UcLogList2()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            Thread th = new Thread(new ThreadStart(ShowLog));
            th.IsBackground = true;
            th.Name = "日志显示线程";
            th.Start();


            this.listBox1.HorizontalScrollbar = true;//任何时候都显示水平滚动条
            this.listBox1.ScrollAlwaysVisible = true;//任何时候都显示垂直滚动条
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
                    if (index > 200)
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

        public void Clear()
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

        private void listBox1_DrawItem_1(object sender, DrawItemEventArgs e)
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked==false)
                listBox1.DrawMode = DrawMode.Normal;
            else
                listBox1.DrawMode = DrawMode.OwnerDrawFixed;
        }

    }
}
