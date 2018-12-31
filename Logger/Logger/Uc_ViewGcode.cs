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
    public partial class Uc_ViewGcode : UserControl
    {

        #region 大小自动调节
        private float X, Y;
        //获得控件的长度、宽度、位置、字体大小的数据
        private void setTag(Control cons)//Control类，定义控件的基类
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;//获取或设置包含有关控件的数据的对象
                if (con.Controls.Count > 0)
                    setTag(con);//递归算法
            }
        }

        private void setControls(float newx, float newy, Control cons)//实现控件以及字体的缩放
        {
            foreach (Control con in cons.Controls)
            {
                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });
                float a = Convert.ToSingle(mytag[0]) * newx;
                con.Width = (int)a;
                a = Convert.ToSingle(mytag[1]) * newy;
                con.Height = (int)(a);
                a = Convert.ToSingle(mytag[2]) * newx;
                con.Left = (int)(a);
                a = Convert.ToSingle(mytag[3]) * newy;
                con.Top = (int)(a);
                Single currentSize = Convert.ToSingle(mytag[4]) * newy;
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);//递归
                }
            }
        }


        private void MyForm_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / X;//当前宽度与变化前宽度之比
            float newy = this.Height / Y;//当前高度与变化前宽度之比
            setControls(newx, newy, this);
            this.Text = this.Width.ToString() + " " + this.Height.ToString();  //窗体标题显示长度和宽度
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Resize += new EventHandler(MyForm_Resize);
            X = this.Width;
            Y = this.Height;
            setTag(this);
        }
        #endregion



        public Uc_ViewGcode()
        {
            InitializeComponent();

            Thread th = new Thread(new ThreadStart(ShowLog));
            th.IsBackground = true;
            th.Name = "日志显示线程";
            th.Start();


            this.listBox1.HorizontalScrollbar = true;//任何时候都显示水平滚动条
            this.listBox1.ScrollAlwaysVisible = true;//任何时候都显示垂直滚动条
        }

        #region 单实例多线程安全

        private static volatile Uc_ViewGcode _instance = null;
        private static readonly object lockHelper = new object();

        public static Uc_ViewGcode CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                    {
                        _instance = new Uc_ViewGcode();
                    }
                }
            }
            return _instance;
        }

        public static Uc_ViewGcode Instance
        {
            get { return CreateInstance(); }
        }

        #endregion

        private Queue logQ = new Queue();
        public void Clear()
        {
            listBox1.Text = "";
            listBox1.Items.Clear();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Clear();
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
                   // listBox1.Items.Insert(0, str);
                    listBox1.Items.Add(str);
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.StackTrace);
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Brush mybsh = Brushes.Black;
               // string[] arrstr = listBox1.Items[e.Index].ToString().Split('\t');
                // 判断是什么类型的标签
                //if (arrstr[1].ToLower().Contains("error"))
                //{
                //    mybsh = Brushes.Red;
                //}
                //else if (arrstr[1].ToLower().Contains("warning"))
                //{
                //    mybsh = Brushes.Blue;
                //}
                // 焦点框
                e.DrawFocusRectangle();
                //文本 
                // e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, mybsh, e.Bounds, StringFormat.GenericDefault);
                e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, mybsh, e.Bounds, StringFormat.GenericDefault);
            }
        }



        ////////
        /// 在listbox中追加状态信息
        /// ////
        public void AddItemToListBox(string str)
        {
            logQ.Enqueue(str);
        }
    }
}
