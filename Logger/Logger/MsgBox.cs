using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Logger
{

    public class MsgBox
    {
        public static readonly object mutx = new object();
        public static DialogResult ShowDialogMsgBox(LogType logType, string text, string caption,
            bool _isShowBtn = false)
        {
            if (!_isShowBtn) text = text.Split('\n')[0]; //取消确定不要显示
            lock (mutx)
            {
                // 记录报警日志
                Logger.Log.AddMotion_Msg(logType, caption, text);

                Form f = new FrmWarnShow(logType, text, caption, _isShowBtn);
                return f.ShowDialog();



            }
        }

        public static DialogResult ShowDialogMsgBox_New(LogType logType, string text, string caption,
            bool _isShowBtn = false)
        {
            if (!_isShowBtn) text = text.Split('\n')[0]; //取消确定不要显示
            lock (mutx)
            {
                // 记录报警日志
               // Logger.Log.AddMotion_Msg(logType, caption, text);
                Logger.Log.AddMsg2(logType, caption, text);
                FrmWarnShow f = new FrmWarnShow(logType, text, caption, _isShowBtn);

                f.ShowDialog();
                return f.res; ;



            }
        }
    }
}
