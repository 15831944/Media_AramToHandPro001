using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Logger
{
    public partial class FrmWarnShow : Form
    {
        public FrmWarnShow()
        {
            InitializeComponent();
        }
        public static readonly object mutxShow = new object();
        private bool isShowBtn;
        public DialogResult res = DialogResult.None;
        private void btnOK_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            res = DialogResult.OK;
           
            this.Dispose();
            this.Close();
          
        }
      
        private void btnCancel_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            res = DialogResult.Cancel;

            this.Dispose();
            this.Close();
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.TopMost = false;

            this.BringToFront();

            this.TopMost = true;
        }

        public FrmWarnShow(LogType LogType, string text, string caption,bool _isShowBtn = false)
        {
            InitializeComponent();
            timer1.Interval = 200;
            timer1.Start();
            SetMsg(LogType, text, caption, _isShowBtn);

        }

        private void SetMsg(LogType LogType, string text, string caption, bool _isShowBtn)
        {
            if (LogType == LogType.Normal)
            {
                lblMsg.BackColor = pnlFull.BackColor = Color.LimeGreen;
                
                
            }
            else if (LogType == LogType.Error)
            {
                lblMsg.BackColor = pnlFull.BackColor = Color.Tomato;
            }
            else if (LogType == LogType.Warning)
            {
                lblMsg.BackColor = pnlFull.BackColor = Color.HotPink;
            }
            Text = caption;
            lblMsg.Text = text;
            IsShowBtn = _isShowBtn;
           

        }
        public bool IsShowBtn
        {
            get { return isShowBtn; }
            set
            {
                isShowBtn = value;

                btnOK.Visible = isShowBtn;
                btnCancel.Visible = isShowBtn;
            }
        }

        public DialogResult DialogResultShow
        {
            get { return res; }
            set
            {
                res = value;

            }
           
        }

        /*
         *   string showMes = string.Format("黑场进入失败确认硬件后\r\n点击OK继续重试\r\n否则返回错误终止");
            var dr1 = MsgBox.ShowDialogMsgBox(TraceLogType.Warning, showMes, @"警告", false);
            if (dr1 != DialogResult.OK)
            {
                UserMotion.Instance.isAllHome = false;
                return EC;
            }
            else
                goto Retry;
         */
    }
}
