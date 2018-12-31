using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;

namespace Logger
{
    public class Log
    {
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filepath);

        public static String readIni(string section, string key, string def, int size, string filepath)
        {//读
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, def, sb, size, filepath);
            return sb.ToString();
        }

        public static Hashtable HeaderList = new Hashtable();
        static bool isRun = false;
        static bool isRunGcode = false;
        static Queue queue = Queue.Synchronized(new Queue());
        static Queue queueGcode = Queue.Synchronized(new Queue());
        Thread thread;
        private static string defaultPath = AppDomain.CurrentDomain.BaseDirectory + "Logger\\";
        static readonly string logBasedPath = @"D:\APPLogger\";

        public delegate void DelegateMsgAdd(LogData msg);
        /// <summary> 添加log界面显示 </summary>
        public static event DelegateMsgAdd OnMsgAdd = null;

        public delegate void DelegateProgramInfAdd(LogData msg);
        /// <summary> 添加ProgramInf界面显示 </summary>
        public static event DelegateProgramInfAdd OnProgramInfAdd = null;

        //public static readonly Log Instance = new Log();
        private static Log instance = null;
        public static Log Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (mutx)
                    {
                        instance = new Log();
                    }
                }
                return instance;
            }
        }

        private Log() { }

        public void Initialize(string threadName)
        {
            if (isRun) return;
            isRun = true;
            thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Name = threadName;
            thread.Start();
            System.Threading.Thread.Sleep(100);
            //LogNotifier.Log(LogType.Info, "线程",thread.Name+"  启动", "");
        }

        public void InitializeGcode(string threadName)
        {
            if (isRunGcode) return;
            isRunGcode = true;
            thread = new Thread(new ThreadStart(ThreadMethodGcode));
            thread.Name = threadName;
            thread.Start();
            System.Threading.Thread.Sleep(100);
            //LogNotifier.Log(LogType.Info, "线程",thread.Name+"  启动", "");
        }

        /// <summary>
        /// ThreadStart委托处理
        /// </summary>
        private void ThreadMethod()
        {
            while (isRun)
            {

                //取作业指令
                object o = null;
                lock (queue)
                {
                    if (queue.Count > 0) 
                        o = queue.Peek();
                }

                //作业处理
                if (o != null)
                {
                    try
                    {
                        lock (o)
                        {
                            LogData e = (LogData)o;
                            OnStateChanged(e);
                        }
                        //去掉成功执行的作业指令
                        lock (queue)
                        {
                            if (queue.Count > 0) queue.Dequeue();
                        }
                    }
                    catch
                    {
                        //执行错误，去除当前引起错误的任务
                        lock (queue)
                        {
                            if (queue.Count > 0) queue.Dequeue();
                        }
                    }

                }
                else
                {
                    System.Threading.Thread.Sleep(10);
                }

            }//end while
        }
        private void ThreadMethodGcode()
        {
            while (isRunGcode)
            {

                //取作业指令
                object o = null;
                lock (queueGcode)
                {
                    if (queueGcode.Count > 0)
                        o = queueGcode.Peek();
                }

                //作业处理
                if (o != null)
                {
                    try
                    {
                        lock (o)
                        {
                            LogData e = (LogData)o;
                            OnStateChanged(e);
                        }
                        //去掉成功执行的作业指令
                        lock (queueGcode)
                        {
                            if (queueGcode.Count > 0) queueGcode.Dequeue();
                        }
                    }
                    catch
                    {
                        //执行错误，去除当前引起错误的任务
                        lock (queueGcode)
                        {
                            if (queueGcode.Count > 0) queueGcode.Dequeue();
                        }
                    }

                }
                else
                {
                    System.Threading.Thread.Sleep(10);
                }

            }//end while
        }

        #region 添加日志信息方法集
        public readonly static object mutx = new object();
        /// <summary>
        /// 添加日志记录
        /// </summary>
        /// <param name="logType">类型</param>
        /// <param name="controlText">操作类别</param>
        /// <param name="Message">结果</param>
        public static void AddMsg(LogType logType, EnumStepType fileName, string controlText, string Message, bool bErrorStop = false)
        {
            try
            {
                lock (mutx)
                {
                    LogData msg = new LogData(logType, fileName, controlText, Message, bErrorStop);

                    if (!isRun) return;
                    //作业指令入列
                    lock (queue)
                    {
                        queue.Enqueue(msg);
                    }
                    //if (OnMsgAdd != null) OnMsgAdd(msg);
                    UcLogList.Instance.AddItemToListBox(msg.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("日志窗体出错，原因：" + ex.Message);
            }
        }

        private string Path_Motion_Log = @"D:\TKLog\Motion Log\";
        private string _path = "";
        public string LogLevel = "release";//release，debug
        /// <summary>
        /// 将异常打印到LOG文件
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="LogAddress">日志文件地址</param>
        /// <param name="Tag">传入标签（这里用于标识函数由哪个线程调用）</param>
        public void WriteLog(string type, string head, string context)
        {
            //Task.Factory.StartNew(() =>
            //{
            try
            {
                //if (LogLevel.ToLower() == "release")
                //{
                //    if (!type.ToLower().Contains("error") && !type.ToLower().Contains("warning") && !type.ToLower().Contains("thrift") && !type.ToLower().Contains("record"))
                //        return;
                //}
                _path = Path_Motion_Log + DateTime.Now.ToString("yyyy-MM-dd") + @"\";
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }
                string file = "";
                string nowTime = "";
                switch (type)
                {
                    case TraceLogType2.Normal:
                    case TraceLogType2.Record:
                    case TraceLogType2.Warning:
                    case TraceLogType2.Error:
                        file = "Motion log";
                        nowTime = _path + file +
                                      DateTime.Now.ToString("yyyy-MM-dd HH") + @".txt";

                        break;
                    case TraceLogType2.AutoFocusU_axiesRecord:
                        file = "U_Axies log ";
                        nowTime = _path + file + DateTime.Now.ToString("yyyy-MM-dd HH") + @".txt";
                        break;
                    case TraceLogType2.WarningRecord:
                        file = "WarningRecord log ";
                        nowTime = _path + file + DateTime.Now.ToString("yyyy-MM-dd HH") + @".txt";
                        break;

                    case TraceLogType2.Thrift:
                    case TraceLogType2.ThriftError:
                        file = "Thrift log";
                        nowTime = _path + file +
                                      DateTime.Now.ToString("yyyy-MM-dd HH") + @".txt";

                        break;

                    case TraceLogType2.PreFocus:
                        file = "PreFocus log";
                        nowTime = _path + file +
                                      DateTime.Now.ToString("yyyy-MM-dd HH") + @".txt";
                        break;

                    case TraceLogType2.AppSocketRecord:
                    case TraceLogType2.AppSocketRecordError:
                        file = "AppSocket log";
                        nowTime = _path + file +
                                      DateTime.Now.ToString("yyyy-MM-dd HH") + @".txt";

                        break;
                    case TraceLogType2.MotionStepTime:
                        file = "MotionStepTime log";
                        nowTime = _path + file +
                                      DateTime.Now.ToString("yyyy-MM-dd HH") + @".txt";

                        break;
                }
                //如果日志文件为空，则默认在Debug目录下新建 YYYY-mm-dd_Log.log文件
                string LogAddress = nowTime;

                string temp = DateTime.Now.ToString("HH:mm:ss") + "\t  " + type.Trim()
                              + "\t  " + head.Trim() + "\t  " + context.Trim();

                //lock (locker)
                {
                    //把异常信息输出到文件
                    StreamWriter sw = new StreamWriter(LogAddress, true);
                    sw.WriteLine(temp);
                    sw.Close();
                    UcLogList.Instance.AddItemToListBox(temp);
                }
            }
            catch (Exception ex)
            {

            }
            //});
        }

        public static void AddMsg(LogType logType, string controlText, Exception ex)
        {
            AddMsg(logType, controlText, ex.Message + "\r\n" + ex.StackTrace);
        }

        public static void AddMsg(LogType logType, string controlText, string Message, bool bErrorStop = false)
        {
            AddMsg(logType, EnumStepType.None, controlText, Message);
            if (logType == LogType.Error)
                AddMsg(logType, EnumStepType.ErrorLog, controlText, Message, bErrorStop);
        }

        public static void AddMotion_Msg(LogType logType, string controlText, string Message)
        {
            AddMsg(logType, EnumStepType.Motion, controlText, Message);
        }

        public static void AddTimer_Msg(string controlText, string Message)
        {
            AddMsg(LogType.Normal, EnumStepType.Timer, controlText, Message);
        }

        public static void AddPR_Msg(string controlText, string Message)
        {
            AddMsg(LogType.Normal, EnumStepType.PR_OPT, controlText, Message);
        }

        public static void AddSocketMarkLoc_Msg(string controlText, string Message)
        {
            AddMsg(LogType.Normal, EnumStepType.SocketMark, controlText, Message);
        }

        public static void AddOther_Msg(LogType logType, string controlText, string Message)
        {
            AddMsg(logType, EnumStepType.Other, controlText, Message);
        }

        public static void AddNCSComm_Msg(LogType logType, string controlText, string Message)
        {
            AddMsg(logType, EnumStepType.NCSComm, controlText, Message);
            if (logType == LogType.Error)
                AddMsg(logType, EnumStepType.ErrorLog, controlText, Message);
        }

        public static void AddLocation_Msg(LogType logType, string controlText, string Message)
        {
            AddMsg(LogType.Normal, EnumStepType.LocationLog, controlText, Message);            
        }

        public static void AddPlaceFailPRInfo_Msg(LogType logType, string controlText, string Message)
        {
            AddMsg(LogType.Normal, EnumStepType.SavePRTimeInfo, controlText, Message);
        }


        /// <summary>
        /// 添加日志记录2
        /// </summary>
        /// <param name="logType">类型</param>
        /// <param name="controlText">操作类别</param>
        /// <param name="Message">结果</param>
        public static void AddMsg2(LogType logType, EnumStepType fileName, string controlText, string Message, bool bErrorStop = false)
        {
            try
            {
                lock (mutx)
                {
                    LogData msg = new LogData(logType, fileName, controlText, Message, bErrorStop);

                    if (!isRun) return;
                    //作业指令入列
                    lock (queue)
                    {
                        queue.Enqueue(msg);
                    }
                    //if (OnMsgAdd != null) OnMsgAdd(msg);
                    UcLogList2.Instance.AddItemToListBox(msg.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("日志窗体出错，原因：" + ex.Message);
            }
        }
        // 只显示内容str
        public static void AddMsgGcodeView(string controlText)
        {
            try
            {
                lock (mutx)
                {
                   // LogData msg = new LogData(controlText);

                    if (!isRunGcode) return;
                   
                    lock (queueGcode)
                    {
                        queueGcode.Enqueue(controlText);
                    }
                    
                    Uc_ViewGcode.Instance.AddItemToListBox(controlText);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("日志窗体出错，原因：" + ex.Message);
            }
        }

        public static void AddMsg2(string conTxt)
        {
            AddMsg2(LogType.Normal, conTxt, "");
        }
        public static void AddMsg2(LogType logType, string controlText, Exception ex)
        {
            AddMsg2(logType, controlText, ex.Message + "\r\n" + ex.StackTrace);
        }

        public static void AddMsg2(LogType logType, string controlText, string Message, bool bErrorStop = false)
        {
            controlText = controlText.Replace("\r\n", "");
            if (logType == LogType.Error)
                AddMsg2(logType, EnumStepType.ErrorLog, controlText, Message, bErrorStop);
            else 
                AddMsg2(logType, EnumStepType.None, controlText, Message);
        }

        /// <summary>
        /// 添加最后的测试结果
        /// </summary>
        /// <param name="Message">具体结果</param>
        public static void Add_FocusValues(LogType _type, string controlText, string Message)
        {
            AddMsg(_type, EnumStepType.Focus, controlText, Message);
        }

        public static void Add_ProgramInf(EnumStepType stepType, string Message)
        {
            AddMsg(LogType.Normal, stepType, "", Message);
        }

        public static void Add_ImageAnalyseInf(string title, string Message)
        {
            AddMsg(LogType.Normal, EnumStepType.ImageAnalyse, title + " -----------\r\n", Message);
        }

        public static void AddTimer_APP_Msg(string controlText, string Message)
        {
            AddMsg(LogType.Normal, EnumStepType.Timer_APP, controlText, Message);
        }
        #endregion

        int hh = 0;
        string tempName = "";
        protected void OnStateChanged(LogData e)
        {
            try
            {
                lock (e)
                {                   
                    string logPath = logBasedPath + DateTime.Now.ToString("yyyy_MM_dd") + "\\";
                    if (!System.IO.Directory.Exists(logPath)) System.IO.Directory.CreateDirectory(logPath);
                    DateTime dNow = System.DateTime.Now;
                    string timeNow = dNow.ToString("yyyy_MM_dd_HH");
                    string fileName = "";
                    string fullPath = "";
                    string data = "";
                    switch (e.StepType)
                    {
                        case EnumStepType.None: //普通日志
                            fileName = "log " + timeNow + ".txt";
                            fullPath = logPath + fileName;
                            if (OnMsgAdd != null) OnMsgAdd(e);
                            break;
                        case EnumStepType.Motion:      //运动统计日志
                            fileName = "Motion_Log " + timeNow + ".txt";
                            fullPath = logPath + fileName;
                            if (OnMsgAdd != null) OnMsgAdd(e);
                            break;
                        case EnumStepType.Timer:      //计时日志
                            fileName = "Timer_Log " + timeNow + ".txt";
                            fullPath = logPath + fileName;
                            if (OnMsgAdd != null) OnMsgAdd(e);
                            break;

                        case EnumStepType.Focus:      //调焦过程日志
                            fileName = "Focus_log " + timeNow + ".txt";
                            fullPath = logPath + fileName;
                            //if (OnProgramInfAdd != null) OnProgramInfAdd(e);
                            break;
                        case EnumStepType.ProgramInfAdd:      //板卡烧录过程日志
                            fileName = "Program_log " + timeNow + ".txt";
                            fullPath = logPath + fileName;
                            //if (OnProgramInfAdd != null) OnProgramInfAdd(e);
                            break;
                        case EnumStepType.ImageAnalyse:      //图片分析过程日志
                            fileName = "ImageAnalyse_log " + timeNow + ".txt";
                            fullPath = logPath + fileName;
                            //if (OnProgramInfAdd != null) OnProgramInfAdd(e);
                            break;

                        case EnumStepType.PR_OPT:
                            fileName = "PR_OPT " + timeNow + ".txt";
                            fullPath = logPath + fileName;
                            //if (OnProgramInfAdd != null) OnProgramInfAdd(e);
                            break;

                        case EnumStepType.SocketMark:
                            fileName = "SocketMark " + dNow.ToString("yyyy_MM_dd") + ".txt";
                            fullPath = logPath + fileName;
                            //if (OnProgramInfAdd != null) OnProgramInfAdd(e);
                            break;

                        case EnumStepType.NCSComm:
                            fileName = "NCSComm " +  timeNow + ".txt";
                            fullPath = logPath + fileName;
                            //if (OnProgramInfAdd != null) OnProgramInfAdd(e);
                            break;
                            
                        case EnumStepType.ErrorLog:
                            fileName = "ErrorLog " + dNow.ToString("yyyy_MM_dd") + ".txt";
                            fullPath = logPath + fileName;
                            if (OnProgramInfAdd != null) OnProgramInfAdd(e);
                            //spark add, 20170115
                            if (e.ErrorStopAutoRun)
                            {
                                string logStopErrorPath = logPath + "ErrorStop_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
                                string strEDate = e.TimeNow + "      " + e.Message + "\n\n";
                                WriteText(logStopErrorPath, strEDate);
                            }
                            break;


                        case EnumStepType.LocationLog:
                            fileName = "LocationLog " + dNow.ToString("yyyy_MM_dd") + ".txt";
                            fullPath = logPath + fileName;
                            //if (OnProgramInfAdd != null) OnProgramInfAdd(e);
                            break;

                        case EnumStepType.SavePRTimeInfo:
                            fileName = "PlaceFailPRTimeInfo " + dNow.ToString("yyyy_MM_dd") + ".txt";
                            fullPath = logPath + fileName;
                            //if (OnProgramInfAdd != null) OnProgramInfAdd(e);
                            break;

                        case EnumStepType.Other:
                            fileName = "Other " + dNow.ToString("yyyy_MM_dd") + ".txt";
                            fullPath = logPath + fileName;
                            //if (OnProgramInfAdd != null) OnProgramInfAdd(e);
                            break;

                        default:        //测试结果
                            try
                            {
                                fileName = e.ControlText + ".csv";   //这里需要改进，文件名应该是:P_SN_Date_HH_LogType_Result
                                //if (LogType.Result != e.LogType)
                                //{
                                //    //fullPath = reportPath  + e.LogType.ToString() + @"\";
                                //    fullPath = reportPath + "Detail" + @"\";
                                //    if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);
                                //    fullPath += fileName;
                                //}
                                //else    //总测试结果文件
                                //{
                                fullPath = logPath + fileName;
                                string path2 = GetFolderPath(fullPath);
                                if (!System.IO.Directory.Exists(path2)) System.IO.Directory.CreateDirectory(path2);
                                if ((!System.IO.File.Exists(fullPath)) && (HeaderList[e.StepType] != null))   //不存在文件则新建文件并创建表头
                                {
                                    //System.IO.File.AppendAllText(fullPath, (string)HeaderList[e.StepType] + "\r\n\r\n", Encoding.Default);
                                    WriteText(fullPath, (string)HeaderList[e.StepType] + "\r\n");
                                }
                            }
                            catch (Exception ex)
                            { }
                            //}
                            break;
                    }
                    data = e.ToString();
                    //System.IO.File.AppendAllText(fullPath, data, Encoding.Default);
                    WriteText(fullPath, data);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("写入日志文件出错！" + ex.Message);
            }

        }

        void WriteText(string path, string text)
        {
            //FileStream fsFile = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter swWriter = new StreamWriter(path, true);
            //写入数据
            swWriter.WriteLine(text);
            swWriter.Flush();
            swWriter.Close();
        }

        string GetFolderPath(string filePath)
        {
            string[] temp = filePath.Split('\\');
            int len = filePath.Length - temp[temp.Length - 1].Length;
            string path = filePath.Substring(0, len);
            return path;
        }
    }
    public class TraceLogType2
    {
        public const string Error = "Error";
        public const string Warning = "Warning";
        public const string Normal = "Normal";
        public const string Record = "Record";
        public const string PreFocus = "PreFocus";
        public const string Thrift = "Thrift";
        public const string ThriftError = "ThriftError";
        public const string MotionStepTime = "MotionStepTime";
        public const string AppSocketRecord = "AppSocketRecord";
        public const string AppSocketRecordError = "AppSocketRecordError";
        public const string AutoFocusU_axiesRecord = "AutoFocusU_axiesRecord";
        public const string WarningRecord = "WarningRecord";
    }
    public partial class LogData
    {
        #region 变量
        private string timeNow;
        private LogType logType;
        private EnumStepType stepType;
        private string controlText;
        private string message;
        #endregion 变量
        //spark add 
        public bool ErrorStopAutoRun = false;//是否是会让自动生产停止的错误信息

        #region 属性
        public string TimeNow
        {
            get { return timeNow; }
            set { timeNow = value; }
        }


        public LogType LogType
        {
            get { return logType; }
            set { logType = value; }
        }

        public EnumStepType StepType
        {
            get { return stepType; }
            set { stepType = value; }
        }

        public string ControlText
        {
            get { return controlText; }
            set { controlText = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        #endregion 属性

        public LogData(LogType _logType, EnumStepType _fileName, string _controlText, string _message, bool bErrorStop = false)
        {
            timeNow = System.DateTime.Now.ToString("HH:mm:ss");//yyyy-MM-dd 
            logType = _logType;
            controlText = _controlText;
            message = _message;
            stepType = _fileName;
            ErrorStopAutoRun = bErrorStop;
        }
        //G代码格式 
        public LogData(string _message)
        {
            //timeNow = System.DateTime.Now.ToString("HH:mm:ss");//yyyy-MM-dd 
            
            message = _message;
           
           
        }

        public override string ToString()
        {
            string data = "";
          
            data = timeNow + "\t  " + logType.ToString() + "\t  " + controlText + "\t  " + message;
           
            return data;
        }
    }

    public enum LogType
    {
        Result = 0,

        Normal = 0x1,
        Warning = 0x2,
        Error = 0x04,//3,
        ShowNormal=0x08//spark add，20161225,一些阶段性的标志还是需要以普通日志的形势显示在列表里面
    }

    public enum EnumStepType
    {
        None = 0,
        T_Result = 1,
        T_Header = 2,
        TISExeMsgAdd = 3,

        ProgramInfAdd = 4,

        ImageAnalyse = 5,
        Motion = 6,
        Timer = 7,
        Focus = 8,
        PR_OPT = 9,
        SocketMark = 10,
        NCSComm = 11,
        ErrorLog = 12,

        LocationLog = 13,
        SavePRTimeInfo = 14,
        Other = 15,
        Timer_APP=16,
    }

    // 系统级的错误码定义
    // 各个模块自行维护一个号段
    public enum ERROR_CODE
    {
        // 通用错误码
        SUCCESS = 0x00000000,                   // 约定：0x00，无错误，执行成功

        ERROR_INVALID_PATA = 0x0A000001,        // 输入参数无效

        // Motion Control模块的自定义错误码，范围：0x0f00,0001 ~ 0x0f00,0fff
        MC_RESET_ERROR_FAIL = 0x0f000001,      //  Axis轴RestError失败
        MC_AXIS_MOVE_TIMEOUT = 0x0f000002,     //  Axis轴运动超时，即在最大允许时间内，未停止运行
        MC_AXIS_STOP_WITH_ERROR = 0x0f000003,  //  Axis轴因错误而停止运动，如到达限位开关等
        MC_AXIS_MOVING = 0x0f000004,           //  Axis轴正在运动中 
        MC_AXIS_HOME_FAIL = 0x0f000005,        //  Axis轴回Home失败

        // DUT Socket没有关闭
        MC_DUT_SOCKET_NOT_CLOSED = 0x0f000010,

        //OpenShort相关
        OS_OPEN_TEST_FAILED_NOMODULE = 0x0f000023,
        OS_OPEN_TEST_FAILED = 0x0f000020,
        OS_SHORT_TEST_FAILED = 0x0f000021,
        OS_TEST_FAILED = 0x0f000022,

        // 汽缸相关
        MC_CYLD_NOT_IN_DOWN_POSITION = 0x0f000030,  // 汽缸不在下位
        MC_CYLD_NOT_IN_UP_POSITION = 0x0f000031,  // 汽缸不在上位

        // Focus相关
        MC_FOCUS_FAILED = 0x0f000040,

        // PR相关
        PR_FAIL = 0x0f000060,
        PR_CENTER_NOT_IN_RANGE = 0x0f000061,

        // Height Sensor返回值错误
        HEIGHT_RESULT_INVALID = 0x0f000070,

        // Vision相机取图失败
        VISION_CAPTURE_FAIL = 0x0f000071,

        // 安全性检测错误码, 范围：0x0f01,0001 ~ 0x0f01,0fff
        SAFE_CHECK_FAIL_FOR_ROTARY = 0x0f010001,
        SAFE_CHECK_CYLD_NOT_DOWN = 0x0f010002,

        // Motion 系统相关错误定义
        /// <summary> 针座顶起时进行运动（不允许） </summary>
        Montion_Needle_Down_Moving = 0x0f020001,
        /// <summary> 运动位置不正确，气缸顶起（不允许） </summary>
        Montion_PositionErr_Needle_Up = 0x0f020002,
        Montion_Start_Error = 0x0f020003,
        Montion_Finish_Error = 0x0f020009,


        // PC与MCU通信
        MCU_FAILED = 0x0f020000,
        MCU_NO_RESPONSE = 0x0f020001,
        MCU_RS232_RES_WORNG = 0x0f020002,
        MCU_RS232_RES_HEADER_WORNG = 0x0f020003,
        MCU_RS232_RES_CS_WORNG = 0x0f020004,
        MCU_IIC_FAILED = 0x0f020005,
        PROGRAM_FAILED = 0x0f020006,    //烧录DUT失败
        NO_DETECTED_SF600 = 0x0f020007,          //SF600烧录器不存在

        #region PC与MCU相关指令通信错误集合

        CMD_Receive_Cmd_Error = 0x0F020000 + 0x40,
        CMD_Connect_Error = 0x0F020000 + 0x41,
        CMD_MCU_Output_Wave_On_Error = 0x0F020000 + 0x42,
        CMD_MCU_Output_Wave_Off_Error = 0x0F020000 + 0x43,
        CMD_Enable_Debug_Print_Error = 0x0F020000 + 0x44,
        CMD_Disable_Debug_Print_Error = 0x0F020000 + 0x45,
        CMD_Enter_Flash_Prog_Error = 0x0F020000 + 0x4A,
        CMD_Reset_DUT2Normal_Error = 0x0F020000 + 0x4B,
        CMD_WRTIE_IIC_Error = 0x0F020000 + 0x50,
        CMD_Entry_FAT_Mode_Error = 0x0F020000 + 0x51,
        CMD_Exit_FAT_Mode_Error = 0x0F020000 + 0x52,
        CMD_Illum_On_Error = 0x0F020000 + 0x53,
        CMD_Illum_Off_Error = 0x0F020000 + 0x54,
        CMD_Aim_On_Error = 0x0F020000 + 0x55,
        CMD_Aim_Off_Error = 0x0F020000 + 0x56,
        CMD_VCC_1V8_Current_Error = 0x0F020000 + 0x57,
        CMD_VCC_3V3_Current_Error = 0x0F020000 + 0x58,
        CMD_IL_3V3_Current_Error = 0x0F020000 + 0x59,
        CMD_Cal_Select_Mux_No_Error = 0x0F020000 + 0x5A,
        CMD_Cal_Report_Result_Error = 0x0F020000 + 0x5B,
        CMD_Write_Parameter_Error = 0x0F020000 + 0x5C,
        CMD_Read_Parameter_Error = 0x0F020000 + 0x5D,

        CMD_Illum_High_Error = 0x0F020000 + 0x5E,
        CMD_Illum_Low_Error = 0x0F020000 + 0x5F,
        CMD_Store_Trim_Flash_Error = 0x0F020000 + 0x60,
        CMD_Read_DUT_Temp_Error = 0x0F020000 + 0x61,
        CMD_Cal_DUT_Temp_Error = 0x0F020000 + 0x62,
        CMD_Drive_Osc_EXT_ILLUM_Error = 0x0F020000 + 0x63,
        CMD_Cal_CPU_Osc_Error = 0x0F020000 + 0x64,
        CMD_Drive_1V4_EXT_ILLUM_Error = 0x0F020000 + 0x65,
        CMD_Cal_1V4_Ref_Error = 0x0F020000 + 0x66,
        CMD_Cal_Aiming_Error = 0x0F020000 + 0x67,
        CMD_Cal_Se_Engine_Current_Error = 0x0F020000 + 0x68,
        CMD_Cal_Store_Model_Number_Error = 0x0F020000 + 0x69,
        CMD_Cal_Store_Serial_Number_Error = 0x0F020000 + 0x6A,
        CMD_Cal_Store_Manu_Date_Error = 0x0F020000 + 0x6B,
        CMD_Total_Power_Measurement_Error = 0x0F020000 + 0x6C,
        CMD_Close_Aim_Illum_Error = 0x0F020000 + 0x6D,
        CMD_Read_Current_TrimValue_Error = 0x0F020000 + 0x6E,


        CMD_Proc_Error = 0x0F020000 + 0xEF,
        /// <summary> 参数错误 </summary>
        CMD_Parameter_Error = 0x0F020000 + 0xFF,
        #endregion

        TISExe_ReadLine_Error = 0x0f020101,
        MesExe_ReadLine_Error = 0x0f020102,

        #region 串口操作部分
        /// <summary>串口操作异常</summary>
        Port_Ctrl_ERR_0500 = 0x0f020500,
        /// <summary>串口超时跳出</summary>
        Port_TimeOut_ERR_0501 = 0x0f020501,
        /// <summary>串口未打开</summary>
        Port_NotOpen_ERR_0502 = 0x0f020502,
        /// <summary>打开串口时报错</summary>   
        Port_Open_ERR_0503 = 0x0f020503,
        /// <summary>串口未初始化</summary>
        Port_NotInit_ERR_0504 = 0x0f020504,
        /// <summary>串口已经打开</summary>
        Port_IsOpened_ERR_0505 = 0x0f020505,

        //0505 -0510// 串口错误备用
        #endregion
    }

}
