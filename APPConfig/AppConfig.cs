//使用内嵌康耐视视觉PR

#define _Use_InnerCognexPR_
//左右堆垛阻挡气缸
//#define _LR_MAG_Stopper_


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace APPConfig
{
    public class AppConfig
    {
        #region 单实例多线程安全

        private volatile static AppConfig _instance = null;
        private static readonly object lockHelper = new object();

        public AppConfig()
        {
        }

        public static AppConfig CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                        _instance = new AppConfig();
                }
            }
            return _instance;
        }

        public static AppConfig Instance
        {
            get { return CreateInstance(); }
        }

        #endregion

        #region //声明读写INI文件的API函数

      
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size,
            string filePath);

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, byte[] def, byte[] retVal, int size,
            string filePath);

        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retval,
            int size, string filepath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileIni(string section, string key, string def, string filepath);

        //[DllImport("kernel32")]
        //private static extern bool WritePrivateProfileString(string section, string key, string val, string filepath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(string section, string key, int def, string filepath);


        /// <summary> 返回所读取的字符串值的真实长度 </summary>
        [DllImport("Kernel32.dll")]
        private extern static int GetPrivateProfileStringA(string strAppName, string strKeyName, string sDefault,
            byte[] buffer, int nSize, string strFileName);

        /// <summary> 根据传入参数的不同进行写入或修改或删除操作（返回值 Long，非零表示成功，零表示失败） </summary> 
        [DllImport("Kernel32.dll")]
        public static extern long WritePrivateProfileString(string strAppName, string strKeyName, string strKeyValue,
            string strFileName);

        #endregion



        public static String readIni(string section, string key, string def, int size, string filepath)
        {
            //读
            var sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, def, sb, size, filepath);
            return sb.ToString().Trim();
        }

        public static int readIni(string section, string key, int def, string filepath)
        {
            //读取整数
            return GetPrivateProfileInt(section, key, def, filepath);
        }

        //Spark add
        public static double readIniDouble(string section, string key, int def, string filepath)
        {
            //读取整数
            var sb = new StringBuilder(255);
            double dRet = 0;
            GetPrivateProfileString(section, key, String.Format("{0}", def), sb, 255, filepath);
            try
            {
                double.TryParse(sb.ToString().Trim(), out dRet);
            }
            catch (Exception)
            {
                dRet = 0;
            }
            return dRet;
        }

        //Spark add
        public static float readIniFloat(string section, string key, int def, string filepath)
        {
            //读取整数
            var sb = new StringBuilder(255);
            float fRet = 0;
            GetPrivateProfileString(section, key, String.Format("{0}", def), sb, 255, filepath);
            try
            {
                float.TryParse(sb.ToString().Trim(), out fRet);
            }
            catch (Exception)
            {
                fRet = 0;
            }
            return fRet;
        }

        //Spark add
        public static bool readIniBool(string section, string key, string def, int size, string filepath)
        {
            //读
            var sb = new StringBuilder(255);
            bool bRet = false;
            GetPrivateProfileString(section, key, def, sb, size, filepath);
            try
            {
                bool.TryParse(sb.ToString().Trim(), out bRet);
            }
            catch (Exception)
            {
                bRet = false;
            }
            return bRet;
        }

        public static bool writeIni(string section, string key, string val, string filepath)
        {
            //写  
            if (val.Trim() != "")
            {
                if (0 == WritePrivateProfileString(section, key, val, filepath))
                    return false;
                return true;
            }
            return false;
        }

        public static bool WriteStringIni(string section, string key, string val, string filepath)
        {
            //写  
            if(filepath.Trim()=="")
            {
                filepath = WeldConfigPath;
                if (System.IO.Directory.Exists(PathMeuIni) == false)
                {
                    System.IO.Directory.CreateDirectory(PathMeuIni);
                }
                if (System.IO.File.Exists(filepath) == false)
                {
                    System.IO.File.Create(filepath);
                    while (true)
                    {
                        if (System.IO.File.Exists(filepath))
                            break;
                    }
                }

            }
               
            if (val.Trim() != "")
            {
                if (0 == WritePrivateProfileString(section, key, val, filepath))
                    return false;
                return true;
            }
            return false;
        }
        public static string ReadIniString(string section, string key, int def, string filepath)
        {
            if (filepath.Trim() == "")
                filepath = WeldConfigPath;
            //读取整数
            var sb = new StringBuilder(255);
            string fRet = "";
            GetPrivateProfileString(section, key, String.Format("{0}", def), sb, 255, filepath);
            try
            {
              
                fRet = sb.ToString().Trim();
            }
            catch (Exception rx)
            {
                fRet = rx.Message;
            }
            return fRet;
        }

        public string visionPath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Vision.ini";
        public string sysConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\SysConfig.ini";
        public string extConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\_ExtConfig.ini";
        public static string WeldConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\_WeldConfig.ini";
        public static string WeldProductTypespath = AppDomain.CurrentDomain.BaseDirectory + "ProductTypes\\";
        private static string PathMeuIni = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\";

        // 读取cognex的视觉模板，选择不同模板测试pr
        public void ReadCognexVisionParam(int prMode, out string vppName)
        {
            vppName = "";
            string strTemp = "";



            switch (prMode)
            {

                #region MyRegion
                
             
                ////case 0:
                //case (int)PickPlace.PRType.TOP_DUT_IN_TRAY:
                //    strTemp = readIni(VisionSectionName.Top, "vpp", "", 100, visionPath);
                //    vppName = strTemp.Trim();
                //    break;

                //// case 1:
                //case (int)PickPlace.PRType.BARCODE_DUT:
                //    int barcodeMode = 1;
                //    ReadBarcodeMode(out barcodeMode);
                //    if (0 == barcodeMode)
                //    {
                //        strTemp = readIni(VisionSectionName.BarcodeTop, "vpp", "", 100, visionPath);
                //        if (strTemp.Trim() == "")
                //            writeIni(VisionSectionName.BarcodeTop, "vpp", "T8", visionPath);
                //    }
                //    else if (1 == barcodeMode)
                //    {
                //        strTemp = readIni(VisionSectionName.BarcodeBottom, "vpp", "", 100, visionPath);
                //        if (strTemp.Trim() == "")
                //            writeIni(VisionSectionName.BarcodeBottom, "vpp", "T9", visionPath);
                //    }

                //    vppName = strTemp.Trim();
                //    break;
                //case (int)PickPlace.PRType.Top_DutBarcodeScan:
                //    strTemp = readIni(VisionSectionName.TopBarcodeScan, "vpp", "", 100, visionPath);
                //    if (strTemp.Trim() == "")
                //    {
                //        strTemp = @"D:\APP\bin\Vpps\Top_DutBarcodeScan.vpp";
                //        writeIni(VisionSectionName.TopBarcodeScan, "vpp", strTemp, visionPath);

                //    }
                //    vppName = strTemp.Trim();
                //    break;
                //case (int)PickPlace.PRType.Bot_DutBarcodeScan:
                //    strTemp = readIni(VisionSectionName.BotBarcodeScan, "vpp", "", 100, visionPath);
                //    if (strTemp.Trim() == "")
                //    {
                //        strTemp = @"D:\APP\bin\Vpps\Bot-DutBarcodeScan.vpp";
                //        writeIni(VisionSectionName.BotBarcodeScan, "vpp", strTemp, visionPath);

                //    }
                //    vppName = strTemp.Trim();
                //    break;
                ////case 2:
                //case (int)PickPlace.PRType.BOTTOM_DUT:
                //    strTemp = readIni(VisionSectionName.Bottom, "vpp", "", 100, visionPath);
                //    vppName = strTemp.Trim();
                //    // 处理类型切换,20170505                                                                      20170725
                //    if (RunCtrl.Instance.CurrentWorkFlow.DutBotPrVppType ==
                //        WorkFlow.WorkFlowBase.EnumDutBotPrVppType.EPatternAndOneLine &&
                //        vppName.LastIndexOf('.') != -1)
                //    {
                //        vppName = vppName.Substring(0, vppName.LastIndexOf('.')) + "_ext.vpp";
                //    }
                //    break;

                //case (int)PickPlace.PRType.BOTTOM_DUT_reserve:
                //    strTemp = readIni(VisionSectionName.Bottom_reserve, "vpp", "", 100, visionPath);
                //    vppName = strTemp.Trim();
                //    // 处理类型切换,20170505
                //    if (RunCtrl.Instance.CurrentWorkFlow.DutBotPrVppType ==
                //        WorkFlow.WorkFlowBase.EnumDutBotPrVppType.EPatternAndOneLine &&
                //        vppName.LastIndexOf('.') != -1)
                //    {
                //        vppName = vppName.Substring(0, vppName.LastIndexOf('.')) + "_ext.vpp";
                //    }
                //    break;

                ////case 3:
                //case (int)PickPlace.PRType.MARK_SOCKET0:
                //    strTemp = readIni(VisionSectionName.Mark0, "vpp", "", 100, visionPath);
                //    vppName = strTemp.Trim();
                //    //处理类型切换,20170505
                //    if (RunCtrl.Instance.CurrentWorkFlow.SocketPrVppType ==
                //        WorkFlow.WorkFlowBase.EnumSocketPrVppType.EPatternAndVacuumCircle &&
                //        vppName.LastIndexOf('.') != -1)
                //    {
                //        vppName = vppName.Substring(0, vppName.LastIndexOf('.')) + "_ext.vpp";
                //    }
                //    break;

                //// case 4:
                //case (int)PickPlace.PRType.MARK_SOCKET1:
                //    strTemp = readIni(VisionSectionName.Mark1, "vpp", "", 100, visionPath);
                //    vppName = strTemp.Trim();
                //    //处理类型切换,20170505
                //    if (RunCtrl.Instance.CurrentWorkFlow.SocketPrVppType ==
                //        WorkFlow.WorkFlowBase.EnumSocketPrVppType.EPatternAndVacuumCircle &&
                //        vppName.LastIndexOf('.') != -1)
                //    {
                //        vppName = vppName.Substring(0, vppName.LastIndexOf('.')) + "_ext.vpp";
                //    }
                //    break;

                //// case 5:
                //case (int)PickPlace.PRType.NOZZLE0:
                //    strTemp = readIni(VisionSectionName.Nozzle0, "vpp", "", 100, visionPath);
                //    if (strTemp.Trim() == "")
                //    {
                //        Logger.Log.AddMsg(Logger.LogType.Error, "ReadVisionParam Nozzle0 failed",
                //            "Try to read Nozzle vision param!");
                //        strTemp = readIni(VisionSectionName.Nozzle, "vpp", "", 100, visionPath);
                //        if (strTemp.Trim() == "")
                //        {
                //            Logger.Log.AddMsg(Logger.LogType.Error, "ReadVisionParam Nozzle failed",
                //                "Use default value!");
                //            strTemp = "T1";
                //        }
                //    }
                //    vppName = strTemp.Trim();
                //    break;

                //// case 6:
                //case (int)PickPlace.PRType.NOZZLE1:
                //    strTemp = readIni(VisionSectionName.Nozzle1, "vpp", "", 100, visionPath);
                //    if (strTemp.Trim() == "")
                //    {
                //        Logger.Log.AddMsg(Logger.LogType.Error, "ReadVisionParam Nozzle1 failed",
                //            "Try to read Nozzle vision param!");
                //        strTemp = readIni(VisionSectionName.Nozzle, "vpp", "", 100, visionPath);
                //        if (strTemp.Trim() == "")
                //        {
                //            Logger.Log.AddMsg(Logger.LogType.Error, "ReadVisionParam Nozzle failed",
                //                "Use default value!");
                //            strTemp = "T1";
                //        }
                //    }
                //    vppName = strTemp.Trim();
                //    break;

                ////   case 7:
                //case (int)PickPlace.PRType.NOZZLE2:
                //    strTemp = readIni(VisionSectionName.Nozzle2, "vpp", "", 100, visionPath);
                //    if (strTemp.Trim() == "")
                //    {
                //        Logger.Log.AddMsg(Logger.LogType.Error, "ReadVisionParam Nozzle2 failed",
                //            "Try to read Nozzle vision param!");
                //        strTemp = readIni(VisionSectionName.Nozzle, "vpp", "", 100, visionPath);
                //        if (strTemp.Trim() == "")
                //        {
                //            Logger.Log.AddMsg(Logger.LogType.Error, "ReadVisionParam Nozzle failed",
                //                "Use default value!");
                //            strTemp = "T1";
                //        }
                //    }
                //    vppName = strTemp.Trim();
                //    break;

                //// case 15:
               // case (int)PickPlace.PRType.TOP_DUT_IN_SOCKET0:
                //    strTemp = readIni(VisionSectionName.SocketDut0, "vpp", "", 100, visionPath);
                //    if (strTemp.Trim() == "")
                //    {
                //        Logger.Log.AddMsg(Logger.LogType.Error, "ReadVisionParam SocketDut0 failed",
                //            "Try to read SocketDut vision param!");
                //        strTemp = readIni(VisionSectionName.SocketDut, "vpp", "", 100, visionPath);
                //        if (strTemp.Trim() == "")
                //        {
                //            Logger.Log.AddMsg(Logger.LogType.Error, "ReadVisionParam SocketDut failed",
                //                "Use default value!");
                //            strTemp = "T2";
                //        }
                //    }
                //    vppName = strTemp.Trim();
                //    break;

                //// case 16:
                //case (int)PickPlace.PRType.TOP_DUT_IN_SOCKET1:
                //    strTemp = readIni(VisionSectionName.SocketDut1, "vpp", "", 100, visionPath);
                //    if (strTemp.Trim() == "")
                //    {
                //        Logger.Log.AddMsg(Logger.LogType.Error, "ReadVisionParam SocketDut1 failed",
                //            "Try to read SocketDut vision param!");
                //        strTemp = readIni(VisionSectionName.SocketDut, "vpp", "", 100, visionPath);
                //        if (strTemp.Trim() == "")
                //        {
                //            Logger.Log.AddMsg(Logger.LogType.Error, "ReadVisionParam SocketDut failed",
                //                "Use default value!");
                //            strTemp = "T2";
                //        }
                //    }
                //    vppName = strTemp.Trim();
                //    break;
                #endregion
                case (int)PickPlace.PRType.LeftFocusPR3:
                    strTemp = readIni(VisionSectionName.FocusePR_3, "vpp", "", 120, visionPath);
                  
                    vppName = strTemp.Trim();
                    break;
                case (int)PickPlace.PRType.LeftFocusPR4:
               
                    strTemp = readIni(VisionSectionName.FocusePR_4, "vpp", "", 120, visionPath);

                    vppName = strTemp.Trim();
                    break;
                case (int)PickPlace.PRType.leftCirclePR:
                    strTemp = readIni(VisionSectionName.L_YuanxingPr, "vpp", "", 120, visionPath);

                    vppName = strTemp.Trim();
                    break;
                case (int)PickPlace.PRType.LeftFocusPR8:
                    strTemp = readIni(VisionSectionName.FocusePR_8, "vpp", "", 120, visionPath);  

                    vppName = strTemp.Trim();
                    break;
                case (int)PickPlace.PRType.RifhtDispensePR3:
                    strTemp = readIni(VisionSectionName.DispensePR_3, "vpp", "", 120, visionPath);
                    vppName = strTemp.Trim();
                    break;
               
                case (int)PickPlace.PRType.RifhtDispensePR4:
             
                    strTemp = readIni(VisionSectionName.DispensePR_4, "vpp", "", 120, visionPath);
                    vppName = strTemp.Trim();
                    break;
                case (int)PickPlace.PRType.leftYaoPR:
                    strTemp = readIni(VisionSectionName.L_YaoxingPr, "vpp", "", 120, visionPath);
                    vppName = strTemp.Trim();
                    break;

                case (int)PickPlace.PRType.RifhtDispensePR8:
                    strTemp = readIni(VisionSectionName.DispensePR_8, "vpp", "", 120, visionPath);

                    vppName = strTemp.Trim();
                    break;

                case (int)PickPlace.PRType.RifhtYaoPR:
                    strTemp = readIni(VisionSectionName.R_YaoxingPr, "vpp", "", 120, visionPath);

                    vppName = strTemp.Trim();
                    break;
                case (int)PickPlace.PRType.RifhtCirclePR:
                    strTemp = readIni(VisionSectionName.R_YuanxingPr, "vpp", "", 120, visionPath);

                    vppName = strTemp.Trim();


                 
                    break;
            }
        }


        public string ReadandWriteCognexVisionParam(int PrMode, out bool resultWrite, bool focusOrDis = true, string prFileName = "FocusCCD")
        {
            resultWrite = false;
            string vppName = "";
            string strTemp = "";
            switch (PrMode)
            {

              

                case (int)PickPlace.PRType.LeftFocusPR3:
                    strTemp = readIni(VisionSectionName.FocusePR_3, "vpp", "", 120, visionPath);
                    vppName = strTemp.Trim();
                    if (strTemp.Trim() != "")
                    {
                        strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\FocusCCD\Vpps\FocusePR3_Dut.vpp";
                       // strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\" + prFileName + @"\Vpps\FocusePR3_Dut.vpp";
                        resultWrite = writeIni(VisionSectionName.FocusePR_3, "vpp", strTemp, visionPath);
                    }
                    break;
                case (int)PickPlace.PRType.LeftFocusPR4: 
              
                    strTemp = readIni(VisionSectionName.FocusePR_4, "vpp", "", 120, visionPath);

                    vppName = strTemp.Trim();
                    if (strTemp.Trim() != "")
                    {
                        strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\FocusCCD\Vpps\FocusePR4_Dut.vpp";
                        resultWrite = writeIni(VisionSectionName.FocusePR_4, "vpp", strTemp, visionPath);
                    }
                    break;
                case (int)PickPlace.PRType.LeftFocusPR8:
                    strTemp = readIni(VisionSectionName.FocusePR_8, "vpp", "", 120, visionPath);

                    vppName = strTemp.Trim();
                    if (strTemp.Trim() != "")
                    {
                        strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\FocusCCD\Vpps\FocusePR8_Dut.vpp";
                        resultWrite = writeIni(VisionSectionName.FocusePR_8, "vpp", strTemp, visionPath);
                    }
                    break;
                case (int)PickPlace.PRType.RifhtDispensePR3:
                    strTemp = readIni(VisionSectionName.DispensePR_3, "vpp", "", 120, visionPath);
                    if (strTemp.Trim() != "")
                    {
                        strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\FocusCCD\Vpps\\DispensePR3_Dut.vpp";
                        resultWrite = writeIni(VisionSectionName.DispensePR_3, "vpp", strTemp, visionPath);
                    }
                    break;

                case (int)PickPlace.PRType.RifhtDispensePR4:
                
                    strTemp = readIni(VisionSectionName.DispensePR_4, "vpp", "", 120, visionPath);
                    if (strTemp.Trim() != "")
                    {
                        strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\FocusCCD\Vpps\\DispensePR4_Dut.vpp";
                        resultWrite = writeIni(VisionSectionName.FocusePR_4, "vpp", strTemp, visionPath);
                    }
                    break;

                case (int)PickPlace.PRType.RifhtDispensePR8:
                    strTemp = readIni(VisionSectionName.DispensePR_8, "vpp", "", 120, visionPath);
                    if (strTemp.Trim() != "")
                    {
                        strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\FocusCCD\Vpps\\DispensePR8_Dut.vpp";
                        resultWrite = writeIni(VisionSectionName.FocusePR_8, "vpp", strTemp, visionPath);
                    }


                    vppName = strTemp.Trim();
                    break;
                case (int)PickPlace.PRType.leftCirclePR:
                    strTemp = readIni(VisionSectionName.L_YuanxingPr, "vpp", "", 120, visionPath);
                    if (strTemp.Trim() != "")
                    {
                       // strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\FocusCCD\Vpps\\L_YuanxingPr_Dut.vpp";
                        strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\" + prFileName + @"\Vpps\L_YuanxingPr_Dut.vpp";
                        resultWrite = writeIni(VisionSectionName.L_YuanxingPr, "vpp", strTemp, visionPath);
                    }


                    vppName = strTemp.Trim();
                    break;
                case (int)PickPlace.PRType.leftYaoPR:
                    strTemp = readIni(VisionSectionName.L_YaoxingPr, "vpp", "", 120, visionPath);
                    if (strTemp.Trim() != "")
                    {
                        //strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\FocusCCD\Vpps\\L_YaoxingPr_Dut.vpp";
                        strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\" + prFileName + @"\Vpps\L_YaoxingPr_Dut.vpp";
                      
                        resultWrite = writeIni(VisionSectionName.L_YaoxingPr, "vpp", strTemp, visionPath);
                    }


                    vppName = strTemp.Trim();
                    break;

                case (int)PickPlace.PRType.RifhtCirclePR:
                    strTemp = readIni(VisionSectionName.R_YuanxingPr, "vpp", "", 120, visionPath);
                    if (strTemp.Trim() != "")
                    {
                        //strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\FocusCCD\Vpps\\R_YuanxingPr_Dut.vpp";
                        strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\" + prFileName + @"\Vpps\R_YuanxingPr_Dut.vpp";
                      
                        resultWrite = writeIni(VisionSectionName.R_YuanxingPr, "vpp", strTemp, visionPath);
                    }


                    vppName = strTemp.Trim();
                    break;
                case (int)PickPlace.PRType.RifhtYaoPR:
                    strTemp = readIni(VisionSectionName.R_YaoxingPr, "vpp", "", 120, visionPath);
                    if (strTemp.Trim() != "")
                    {
                       // strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\FocusCCD\Vpps\\R_YaoxingPr_Dut.vpp";
                        strTemp = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\" + prFileName + @"\Vpps\R_YaoxingPr_Dut.vpp";
                      
                        resultWrite = writeIni(VisionSectionName.R_YaoxingPr, "vpp", strTemp, visionPath);
                    }


                    vppName = strTemp.Trim();
                    break;


            }

            return strTemp;
        }

        #region CameraDisplayRotate

        // 读取相机显示旋转角度
        public void ReadCameraDisplayRotate(out int topAngle, out int bottomAngle)
        {
            topAngle = 270;
            bottomAngle = 180;

            string strTemp = "";
            strTemp = readIni(SectionName.Camera, "TopDisplayRotate", "0", 10, sysConfigPath);
            if (!int.TryParse(strTemp.Trim().Trim('\t'), out topAngle))
                topAngle = 0;

            strTemp = readIni(SectionName.Camera, "BottomDisplayRotate", "0", 10, sysConfigPath);
            if (!int.TryParse(strTemp.Trim().Trim('\t'), out bottomAngle))
                bottomAngle = 0;
        }

        // 保存相机显示旋转角度
        public bool SaveCameraDisplayRotate(int topAngle, int bottomAngle)
        {
            try
            {
                if (!writeIni(SectionName.Camera, "TopDisplayRotate", topAngle.ToString(), sysConfigPath))
                    return false;

                return writeIni(SectionName.Camera, "BottomDisplayRotate", bottomAngle.ToString(), sysConfigPath);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion



        public int ReadBarcodeCodeStandardLength()
        {
            try
            {
                string strTemp;
                strTemp = readIni(SectionName.Main, "BarcodeCodeStandardLength", "", 100,
                    extConfigPath);
                if (strTemp.Trim() != "")
                    return int.Parse(strTemp);
                else
                {
                    writeIni(SectionName.Main, "BarcodeCodeStandardLength", "9", extConfigPath);
                    return 9;
                }
            }
            catch (Exception ex)
            {
                return 9;
            }
            return 9;
        }

        public bool IsHandleCcdLostEvent()
        {
            try
            {
                bool bEnable;
                string strTemp = "";

                strTemp = readIni(SectionName.Main, "HandleCcdLostEvent", "", 100, sysConfigPath);

                if (strTemp.Trim() != "")
                {
                    if (bool.TryParse(strTemp.Trim().ToLower(), out bEnable))
                        return bEnable;
                }
                writeIni(SectionName.Main, "HandleCcdLostEvent", "true", sysConfigPath);
                return true;
            }
            catch
            {
                writeIni(SectionName.Main, "HandleCcdLostEvent", "true", sysConfigPath);
                return false;
            }
        }

        // 读取曝光时间，光源控制通道号，光源亮度值
        public void ReadExposureForPRImage(int prMode, out int exposure, out int chanel, out int light)
        {
            string strTemp = "";
            exposure = 2000;
            chanel = 0;
            light = 100;
            switch (prMode)
            {
                //case 0: // top
                case (int)PickPlace.PRType.TOP_DUT_IN_TRAY:
                case (int)PickPlace.PRType.Tray_Has_Dut: //检测托盘格子是否有模组和检测模组位置使用相同高度参数即可, spark add, 20170115
                    strTemp = readIni("DutTopVision", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("DutTopVision", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("DutTopVision", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;

                #region  增加调焦点胶pr

                case 30: // 调焦
                    strTemp = readIni("[FocusVision]", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("[FocusVision]", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("[FocusVision]", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;
                case 31: // 点焦
                    strTemp = readIni("[DispenseVision]", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("[DispenseVision]", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("[DispenseVision]", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;

                #endregion

                //case 15: // socket dut top
                case (int)PickPlace.PRType.TOP_DUT_IN_SOCKET0:
                    strTemp = readIni("SocketDutTopVision0", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("SocketDutTopVision0", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("SocketDutTopVision0", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;

                //case 16: // socket dut top
                case (int)PickPlace.PRType.TOP_DUT_IN_SOCKET1:
                    strTemp = readIni("SocketDutTopVision1", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("SocketDutTopVision1", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("SocketDutTopVision1", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;

                //case 1: // barcode
                case (int)PickPlace.PRType.BARCODE_DUT:
                    strTemp = readIni("DutBarcodeVision", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("DutBarcodeVision", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("DutBarcodeVision", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;

                //case 2: // bottom
                case (int)PickPlace.PRType.BOTTOM_DUT:
                case (int)PickPlace.PRType.BOTTOM_DUT_reserve:
                case (int)PickPlace.PRType.Bot_DutBarcodeScan://20170927
                    strTemp = readIni("DutBottomVision", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("DutBottomVision", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("DutBottomVision", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;

                // case 3: // mark 0
                case (int)PickPlace.PRType.MARK_SOCKET0:
                    strTemp = readIni("TesterMark0Vision", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("TesterMark0Vision", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("TesterMark0Vision", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;

                //case 4:  // mark 1
                case (int)PickPlace.PRType.MARK_SOCKET1:
                    strTemp = readIni("TesterMark1Vision", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("TesterMark1Vision", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("TesterMark1Vision", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;

                // case 5: // nozzle
                case (int)PickPlace.PRType.NOZZLE0:
                    strTemp = readIni("NozzleVision0", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("NozzleVision0", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("NozzleVision0", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;

                //   case 6: // nozzle 1
                case (int)PickPlace.PRType.NOZZLE1:
                    strTemp = readIni("NozzleVision1", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("NozzleVision1", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("NozzleVision1", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;

                //  case 7: // nozzle 2
                case (int)PickPlace.PRType.NOZZLE2:
                    strTemp = readIni("NozzleVision2", "Exposure", "2000", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni("NozzleVision2", "LightChanel", "0", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni("NozzleVision2", "Light", "100", 100, sysConfigPath);
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;
                //spark add, 20170115
                //case (int)PickPlace.PRType.Tray_Has_Dut:
                //    strTemp = readIni("TrayHasDutVision", "Exposure", "2000", 100, sysConfigPath);
                //    Int32.TryParse(strTemp.Trim(), out exposure);
                //    strTemp = readIni("TrayHasDutVision", "LightChanel", "0", 100, sysConfigPath);
                //    Int32.TryParse(strTemp.Trim(), out chanel);
                //    if (chanel > 3 || chanel < 0)
                //        chanel = 0;
                //    strTemp = readIni("TrayHasDutVision", "Light", "100", 100, sysConfigPath);
                //    Int32.TryParse(strTemp.Trim(), out light);
                //    if (light > 255 || light < 0)
                //        light = 100;
                //    break;
                case (int)PickPlace.PRType.Top_DutBarcodeScan: //20170626
                    strTemp = readIni(VisionSectionName.TopBarcodeScan, "Exposure", "", 100, sysConfigPath);
                    if (strTemp.Trim() == "")
                    {
                        strTemp = "100";
                        writeIni(VisionSectionName.TopBarcodeScan, "Exposure", strTemp, sysConfigPath);
                    }
                    Int32.TryParse(strTemp.Trim(), out exposure);
                    strTemp = readIni(VisionSectionName.TopBarcodeScan, "LightChanel", "", 100, sysConfigPath);
                    if (strTemp.Trim() == "")
                    {
                        strTemp = "0";
                        writeIni(VisionSectionName.TopBarcodeScan, "LightChanel", strTemp, sysConfigPath);
                    }
                    Int32.TryParse(strTemp.Trim(), out chanel);
                    if (chanel > 3 || chanel < 0)
                        chanel = 0;
                    strTemp = readIni(VisionSectionName.TopBarcodeScan, "Light", "100", 100, sysConfigPath);
                    if (strTemp.Trim() == "")
                    {
                        strTemp = "100";
                        writeIni(VisionSectionName.TopBarcodeScan, "Light", strTemp, sysConfigPath);
                    }
                    Int32.TryParse(strTemp.Trim(), out light);
                    if (light > 255 || light < 0)
                        light = 100;
                    break;
                //使用和底部PR一样的参数即可
                //case (int) PickPlace.PRType.Bot_DutBarcodeScan: //20170626
                //    strTemp = readIni(VisionSectionName.BotBarcodeScan, "Exposure", "", 100, sysConfigPath);
                //    if (strTemp.Trim() == "")
                //    {
                //        strTemp = "100";
                //        writeIni(VisionSectionName.BotBarcodeScan, "Exposure", strTemp, sysConfigPath);
                //    }
                //    Int32.TryParse(strTemp.Trim(), out exposure);
                //    strTemp = readIni(VisionSectionName.BotBarcodeScan, "LightChanel", "", 100, sysConfigPath);
                //    if (strTemp.Trim() == "")
                //    {
                //        strTemp = "0";
                //        writeIni(VisionSectionName.BotBarcodeScan, "LightChanel", strTemp, sysConfigPath);
                //    }
                //    Int32.TryParse(strTemp.Trim(), out chanel);
                //    if (chanel > 3 || chanel < 0)
                //        chanel = 0;
                //    strTemp = readIni(VisionSectionName.BotBarcodeScan, "Light", "100", 100, sysConfigPath);
                //    if (strTemp.Trim() == "")
                //    {
                //        strTemp = "100";
                //        writeIni(VisionSectionName.BotBarcodeScan, "Light", strTemp, sysConfigPath);
                //    }
                //    Int32.TryParse(strTemp.Trim(), out light);
                //    if (light > 255 || light < 0)
                //        light = 100;
                //    break;
                default:
                    break;

            }
        }

        public void ReadLighrChanel(ref int focusChanel,ref int DispenseChanel)
        {
            string strTemp = readIni("FocusVision", "LightChanel", "0", 100, sysConfigPath);
            Int32.TryParse(strTemp.Trim(), out focusChanel);
            strTemp = readIni("DispenseVision", "LightChanel", "0", 100, sysConfigPath);
            Int32.TryParse(strTemp.Trim(), out DispenseChanel);
        }

        public void WriteExposureForPRImage(int prMode, int exposure, int chanel, int light)
        {
            if (chanel > 3 || chanel < 0)
                chanel = 0;

            if (light > 255 || light < 0)
                light = 100;
    
            switch (prMode)
            {
         
                #region  增加调焦点胶pr

                case 30: // 调焦
                        
                    try
                    {
                       // writeIni("FocusVision", "Exposure", exposure.ToString(), extConfigPath);
                        writeIni("FocusVision", "LightChanel", chanel.ToString(), extConfigPath);
                        writeIni("FocusVision", "Light", light.ToString(), extConfigPath);
                    }
                    catch (Exception)
                    {

                    }
                    break;
                case 31: // 点焦

                  //  writeIni("DispenseVision", "Exposure", exposure.ToString(), extConfigPath);
                    writeIni("DispenseVision", "LightChanel", chanel.ToString(), extConfigPath);
                    writeIni("DispenseVision", "Light", light.ToString(), extConfigPath);
                    
                    break;

                #endregion
              
                default:
                    break;

            }
        }


        public bool SaveGlobalExposeForBotCcd(int expose)
        {
            try
            {
                writeIni(SectionName.Main, "GlobalExposeForBotCcd", expose.ToString(), extConfigPath);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool ReadGlobalExposeForTopCcd(out int expose)
        {
            expose = 0;
            try
            {
                string strTemp;

                strTemp = readIni(SectionName.Main, "GlobalTopCcdExpose", "", 100,
                    extConfigPath);
                if (strTemp.Trim() != "")
                {
                    expose = int.Parse(strTemp.Trim().ToLower());
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

     

        public bool ReadGlobalExposeForBotCcd(out int expose)
        {
            expose = 0;
            try
            {
                string strTemp;

                strTemp = readIni(SectionName.Main, "GlobalExposeForBotCcd", "", 100,
                    extConfigPath);
                if (strTemp.Trim() != "")
                {
                    expose = int.Parse(strTemp.Trim().ToLower());
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }


        public bool SaveGlobalExposeForTopCcd(int expose)
        {
            try
            {
                writeIni(SectionName.Main, "GlobalTopCcdExpose", expose.ToString(), extConfigPath);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        // 读取APP是否控制相机
        public bool IsAPPControlCCD()
        {
            string strTemp = "";
            bool bCtrl = false;
            strTemp = readIni(SectionName.Main, "APPControlCCD", "false", 10, sysConfigPath);
            bool.TryParse(strTemp.Trim().ToLower(), out bCtrl);

            return bCtrl;
        }

        // 读取PR前延时（Z下降后，延时多长时间再PR）
        public void ReadDelayBeforePR(int prMode, out double time)
        {
            string strTemp = "";
            time = 0;
            switch (prMode)
            {
                case 0: // top
                    strTemp = readIni("DutTopVision", "DelayBeforePR", "", 100, sysConfigPath);
                    if (strTemp == "")
                    {
                        Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                            "DutTopVision DelayBeforePR not setting, use default value 0!");
                        strTemp = "0";
                    }
                    double.TryParse(strTemp.Trim(), out time);
                    break;

                case 15: // socket dut top
                    strTemp = readIni("SocketDutTopVision0", "DelayBeforePR", "", 100, sysConfigPath);
                    if (strTemp == "")
                    {
                        Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                            "SocketDutTopVision0 DelayBeforePR not setting, try to read SocketDutTopVision param!");
                        strTemp = readIni("SocketDutTopVision", "DelayBeforePR", "", 100, sysConfigPath);
                        if (strTemp == "")
                        {
                            Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                                "SocketDutTopVision DelayBeforePR not setting, use default value 0!");
                            strTemp = "0";
                        }
                    }
                    double.TryParse(strTemp.Trim(), out time);
                    break;

                case 16: // socket dut top
                    strTemp = readIni("SocketDutTopVision1", "DelayBeforePR", "", 100, sysConfigPath);
                    if (strTemp == "")
                    {
                        Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                            "SocketDutTopVision1 DelayBeforePR not setting, try to read SocketDutTopVision param!");
                        strTemp = readIni("SocketDutTopVision", "DelayBeforePR", "", 100, sysConfigPath);
                        if (strTemp == "")
                        {
                            Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                                "SocketDutTopVision DelayBeforePR not setting, use default value 0!");
                            strTemp = "0";
                        }
                    }
                    double.TryParse(strTemp.Trim(), out time);
                    break;

                case 1: // barcode
                    strTemp = readIni("DutBarcodeVision", "DelayBeforePR", "", 100, sysConfigPath);
                    if (strTemp == "")
                    {
                        Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                            "DutBarcodeVision DelayBeforePR not setting, use default value 0!");
                        strTemp = "0";
                    }
                    double.TryParse(strTemp.Trim(), out time);
                    break;

                case 2: // bottom
                    strTemp = readIni("DutBottomVision", "DelayBeforePR", "", 100, sysConfigPath);
                    if (strTemp == "")
                    {
                        Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                            "DutBottomVision DelayBeforePR not setting, use default value 0!");
                        strTemp = "0";
                    }
                    double.TryParse(strTemp.Trim(), out time);
                    break;

                case 3: // mark 0
                    strTemp = readIni("TesterMark0Vision", "DelayBeforePR", "", 100, sysConfigPath);
                    if (strTemp == "")
                    {
                        Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                            "TesterMark0Vision DelayBeforePR not setting, use default value 0!");
                        strTemp = "0";
                    }
                    double.TryParse(strTemp.Trim(), out time);
                    break;

                case 4: // mark 1
                    strTemp = readIni("TesterMark1Vision", "DelayBeforePR", "", 100, sysConfigPath);
                    if (strTemp == "")
                    {
                        Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                            "TesterMark1Vision DelayBeforePR not setting, use default value 0!");
                        strTemp = "0";
                    }
                    double.TryParse(strTemp.Trim(), out time);
                    break;

                case 5: // nozzle 0
                    strTemp = readIni("NozzleVision0", "DelayBeforePR", "", 100, sysConfigPath);
                    if (strTemp == "")
                    {
                        Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                            "NozzleVision0 DelayBeforePR not setting, try to read NozzleVision param.");
                        strTemp = readIni("NozzleVision", "DelayBeforePR", "", 100, sysConfigPath);
                        if (strTemp == "")
                        {
                            Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                                "NozzleVision DelayBeforePR not setting, use default value 0!");
                            strTemp = "0";
                        }
                    }
                    double.TryParse(strTemp.Trim(), out time);
                    break;

                case 6: // nozzle 1
                    strTemp = readIni("NozzleVision1", "DelayBeforePR", "", 100, sysConfigPath);
                    if (strTemp == "")
                    {
                        Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                            "NozzleVision1 DelayBeforePR not setting, try to read NozzleVision param.");
                        strTemp = readIni("NozzleVision", "DelayBeforePR", "", 100, sysConfigPath);
                        if (strTemp == "")
                        {
                            Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                                "NozzleVision DelayBeforePR not setting, use default value 0!");
                            strTemp = "0";
                        }
                    }
                    double.TryParse(strTemp.Trim(), out time);
                    break;

                case 7: // nozzle 0
                    strTemp = readIni("NozzleVision2", "DelayBeforePR", "", 100, sysConfigPath);
                    if (strTemp == "")
                    {
                        Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                            "NozzleVision2 DelayBeforePR not setting, try to read NozzleVision param.");
                        strTemp = readIni("NozzleVision", "DelayBeforePR", "", 100, sysConfigPath);
                        if (strTemp == "")
                        {
                            Logger.Log.AddMsg(Logger.LogType.Warning, "SysConfig file",
                                "NozzleVision DelayBeforePR not setting, use default value 0!");
                            strTemp = "0";
                        }
                    }
                    double.TryParse(strTemp.Trim(), out time);
                    break;
                default:
                    break;

            }
        }

        // 保存PR前延时（Z下降后，延时多长时间再PR）
        public void SaveDelayBeforePR(int prMode, double time)
        {
            switch (prMode)
            {
                case 0: // top
                    writeIni("DutTopVision", "DelayBeforePR", time.ToString(), sysConfigPath);
                    break;

                case 15: // socket dut top
                    writeIni("SocketDutTopVision0", "DelayBeforePR", time.ToString(), sysConfigPath);
                    break;

                case 16: // socket dut top
                    writeIni("SocketDutTopVision1", "DelayBeforePR", time.ToString(), sysConfigPath);
                    break;

                case 1: // barcode
                    writeIni("DutBarcodeVision", "DelayBeforePR", time.ToString(), sysConfigPath);
                    break;

                case 2: // bottom
                    writeIni("DutBottomVision", "DelayBeforePR", time.ToString(), sysConfigPath);
                    break;

                case 3: // mark 0
                    writeIni("TesterMark0Vision", "DelayBeforePR", time.ToString(), sysConfigPath);
                    break;

                case 4: // mark 1
                    writeIni("TesterMark1Vision", "DelayBeforePR", time.ToString(), sysConfigPath);
                    break;

                case 5: // nozzle 0
                    writeIni("NozzleVision0", "DelayBeforePR", time.ToString(), sysConfigPath);
                    break;

                case 6: // nozzle 1
                    writeIni("NozzleVision1", "DelayBeforePR", time.ToString(), sysConfigPath);
                    break;

                case 7: // nozzle 0
                    writeIni("NozzleVision2", "DelayBeforePR", time.ToString(), sysConfigPath);
                    break;
                default:
                    break;

            }
        }

        public void ReadPRTimeOut(out int iTimeout)
        {
            #region ReadPRTimeOut

            iTimeout = 10000;

            string strTemp;
            strTemp = readIni(SectionName.Timeouts, "PRTimeout", "", 100, sysConfigPath);

            if (strTemp.Trim().ToUpper() != "")
            {
                if (!int.TryParse(strTemp, out iTimeout))
                    iTimeout = 10000;
            }
            else
            {
                iTimeout = 10000;
                writeIni(SectionName.Timeouts, "PRTimeout", "10000", sysConfigPath);
            }

            #endregion

        }

        //spark add，归一化角度
        public void NormalizeAngle(ref double angle)
        {
            do
            {
                if (angle > 180)
                {
                    angle -= 360;
                }
                if (angle < -180)
                {
                    angle += 360;
                }
            } while (Math.Abs(angle) > 180);
        }

        public void NormalizeAngle(ref float angle)
        {
            do
            {
                if (angle > 180)
                {
                    angle -= 360;
                }
                if (angle < -180)
                {
                    angle += 360;
                }
            } while (Math.Abs(angle) > 180);
        }

        public string ReadCurrentProductName()
        {
            string strTemp;
            strTemp = readIni(SectionName.Main, "CurrentProductName", "", 200, sysConfigPath).Trim();

            if (strTemp == "")
            {
                writeIni(SectionName.Main, "CurrentProductName", "默认产品", sysConfigPath);
                strTemp = "默认产品";
            }
            return strTemp;
        }

        public bool SaveCurrentProductName(string productName)
        {
            try
            {
                return writeIni(SectionName.Main, "CurrentProductName", productName, sysConfigPath);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveUpdateProductFile(bool bEnable)
        {
            try
            {
               return   writeIni(SectionName.Main, "UpdateProductFile", bEnable.ToString(), sysConfigPath);
            }
            catch (Exception ex)
            {
                return false;
            };
        }

        public static bool SaveProductFile(string PfoductName,string section, string key, string val)
        {
              
            // 查找某文件下是否存在ini文件，并新建ini文件
           
                //写  
           
                if (!Directory.Exists(WeldProductTypespath))
                {
                    Directory.CreateDirectory(WeldProductTypespath);
                }
                string filepath = WeldProductTypespath+PfoductName +@".txt";
                //if (!File.Exists(filepath))
                //{
                //    FileStream fs1 = new FileStream(filepath, FileMode.Create, FileAccess.Write);//创建写入文件 
                //    StreamWriter sw = new StreamWriter(fs1);
                //    sw.WriteLine(section + "+" + key + "+" + val);//开始写入值
                //    sw.Close();
                //    fs1.Close();
                //}
                //else
                //{
                //    FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Write);
                //    StreamWriter sr = new StreamWriter(fs);
                //    sr.WriteLine(section + "+" + key + "+" + val);//开始写入值
                //    sr.Close();
                //    fs.Close();
                //}
            filepath = WeldProductTypespath + PfoductName + @".ini";
           // string extConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\_ExtConfig.ini";
            OperateIniFile.filePath = filepath;
           // writeIni("FocusVision", "Light", light.ToString(), extConfigPath);
           // if (val.Trim() != "")
            {
               
                if (0 == WritePrivateProfileString(section, key, val, filepath))
                    return false;
                return true;
            }
            //return true;
        }

        public static bool AddProductToIni(string section, string key, string val)
        {
            string filepath = "";
            filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\_ExtConfig.ini";
            OperateIniFile.filePath = filepath;
            if (val.Trim() != "")
            {
                if (0 == WritePrivateProfileString(section, key, val, filepath))
                    return false;
                return true;
            }
            return true;

        }
        public static bool CopyFiles(string soucename, string DestFileName)
        {
            try
            {
               // string beginfilepath = WeldProductTypespath + soucename + @".ini";
                string Destfilepath = WeldProductTypespath + DestFileName + @".ini";

                File.Copy(soucename, Destfilepath, true);
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            return true;
        }

        public static string ChaZhaoIniFilesByName(string inifileName)
        {
            string beginfilepath = WeldProductTypespath + inifileName + @".ini";

            if (File.Exists(beginfilepath))
                return WeldProductTypespath + inifileName + @".ini";
            else
                return "";
        }

        public static List<string> GetIniFiles()
        {
            List<string> strList = new List<string>();
            if (!Directory.Exists(WeldProductTypespath))
            {
                Directory.CreateDirectory(WeldProductTypespath);
            }
            var files = Directory.GetFiles(WeldProductTypespath, "*.ini");
            foreach (var item in files)
            {
                strList.Add(item);
            }
            return strList;
        }

        public static bool  SetIniFileToPeizhiIni(string ininame)
        {
            bool result = true;
            if (ChaZhaoIniFilesByName(ininame)!="")
                AddProductToIni("IniPath", "path", ininame);
            else
            {
                result = false;
            }
            return result;
        }
        public static bool SetIniFilePathToPeizhiPath(string path)
        {
            bool result = true;
           
              result=  AddProductToIni("IniPath", "path", path);
          
            return result;
        }

        public static bool DeleteIniFile(string namepath)
        {
            bool result = true;
            try
            {
                File.Delete(namepath);

            }
            catch (Exception )
            {
                
                result = false;
            }
          
            return result;
        }

        public static bool DeleteFilsAndfiles(string pathfiles)
        {
            bool result = true;
            try
            {
                DirectoryInfo di = new DirectoryInfo(pathfiles);
                di.Delete(true);
            }
            catch (Exception)
            {

                result = false;
            }

            return result;
            
        }
        public static string ReadIniFileToPeizhiIni(string section = "IniPath", string key="path", int def=255)
        {
            string filepath = "";
            filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\_ExtConfig.ini";
          
            //读取整数
            var sb = new StringBuilder(255);
            string fRet = "";
            GetPrivateProfileString(section, key, String.Format("{0}", def), sb, 255, filepath);
            try
            {

                fRet = sb.ToString().Trim();
            }
            catch (Exception rx)
            {
                fRet = rx.Message;
            }
            return fRet;
        }


        public static bool CopyDirectory(string srcPath, string destPath)
        {
            bool result = false;
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                throw;
            }

            return result;
        }


        // 更改vision.ini文件的文件名
        //  strTemp = readIni(VisionSectionName.R_YaoxingPr, "vpp", "", 120, visionPath);
        public static bool ModifyVisionIni(string FileName)
        {
           string visionPath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Vision.ini";
            try
            {
                FileName = AppDomain.CurrentDomain.BaseDirectory + @"\Config\CCD_PR\" + FileName + @"\Vpps\";
               
                var tmp = FileName + @"R_YaoxingPr_Dut.vpp";
                writeIni(VisionSectionName.R_YaoxingPr, "vpp", tmp, visionPath);
                
                tmp = FileName + @"R_YuanxingPr_Dut.vpp";
                writeIni(VisionSectionName.R_YuanxingPr, "vpp", tmp, visionPath);
               
                tmp = FileName + @"L_YaoxingPr_Dut.vpp";
                writeIni(VisionSectionName.L_YaoxingPr, "vpp", tmp, visionPath);
               
                tmp = FileName + @"L_YuanxingPr_Dut.vpp";
                writeIni(VisionSectionName.L_YuanxingPr, "vpp", tmp, visionPath); 
            }                                                                
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool AddPrName(string fileName)
        {
            string filepath = "";
            filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\_ExtConfig.ini";

            if (fileName.Trim() != "")
            {
                if (0 == WritePrivateProfileString("PRCCD", "PRfilesName", fileName, filepath))
                    return true;
                else
                {
                    return false;
                }
            }

            return true;
        }

        
    }
}
              