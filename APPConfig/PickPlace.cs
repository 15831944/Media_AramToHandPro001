//使用内嵌康耐视视觉PR
#define _Use_InnerCognexPR_
//单机检测，不进行AutoSocket真空检测，实际生产需要开启此宏定义
#define _Release_ForPickAndPlace_
//是否使用标定库计算角度转换
//#define _Ues_caliLib_calcAgle_

//启用对准条码中心扫描代替线性3点扫描
#define _Use_ScanBarcodeCenter_


using DevComponents.DotNetBar;
using Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace APPConfig
{
    public class PickPlace
    {

        #region 单实例多线程安全
        private volatile static PickPlace _instance = null;
        private static readonly object lockHelper = new object();
        private PickPlace() { }
        public static PickPlace CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                        _instance = new PickPlace();
                }
            }
            return _instance;
        }
        public static PickPlace Instance
        {
            get { return CreateInstance(); }
        }
        #endregion

        public enum PRType
        {
            TOP_DUT_IN_TRAY = 0,
            TOP_DUT_IN_SOCKET0 = 15,    // 15:tester0   16 tester1
            TOP_DUT_IN_SOCKET1 = 16,
            BARCODE_DUT = 1,
            BOTTOM_DUT = 2,
            MARK_SOCKET0 = 3,           // 3: tester0    4: tester1
            MARK_SOCKET1 = 4,
            NOZZLE0 = 5,                // 5: nozzle0    6: nozzle1    7:nozzle2
            NOZZLE1 = 6,
            NOZZLE2 = 7,
            //spark add
            TOP_CALI_MARK = 18,  //MARK点PR
            TOP_MATRIX = 19,//上相机PR16个点的矩阵
            BOT_MATRIX = 20,//下相机PR116个点的矩阵 

            TOP_DUT_IN_TRAY2 = 10,      // 双摄AA 托盘2 顶部识别
            BOTTOM_DUT_2 = 11,          // 双摄AA 托盘2 底部识别
            //spark add, 20170115
            //检测托盘格子里面是否有物料，此功能是飞拍识别失败后，调用单拍时首先使用此功能检测格子里面是否有模组，有模组再进行
            //有模组就接着进行详细定位，目的是获取格子有无模组的状态
            //后续使用深度定制的视觉可以知道每一步的测试结果时，可以不使用此功能
            Tray_Has_Dut = 21,
            BOTTOM_DUT_reserve = 22,//20170315，底部副模板，主模板识别失败就调用副模板识别，飞行还是使用主模板
            Tray_Has_Dut_TRAY2 = 23,
            //20170626,相机扫码
            Top_DutBarcodeScan,
            Bot_DutBarcodeScan,

            LeftFocusPR3 = 29,  // 花瓣数量3
            LeftFocusPR4 = 30,
            LeftFocusPR8 = 31,


            RifhtDispensePR3 = 35,
            RifhtDispensePR4 = 36,
            RifhtDispensePR8 = 37,
           

            
             leftCirclePR = 38,
             leftYaoPR = 39,
             RifhtCirclePR=40,
             RifhtYaoPR = 41,


        }


        /// <summary>
        /// 20170314，转换上相机角度到全局角度
        /// </summary>
        /// <param name="angle"></param>
        /// 




        public double DtopCcdToGlobalAngle = 0;//上相机与机械坐标系的夹角     
        public double DbotCcdToGlobalAngle = 0;//下相机与机械坐标系的夹角
        private void ConvertTopCcdPRAngleToWorkAngle(ref double angle)
        {
#if _Ues_caliLib_calcAgle_
           try
           {
               angle = (double)OTPCoordSysWrapper.GetThis().GetAglFromDwnCam((float)angle);
           }
           catch (Exception exp)
           {
               TaskDialog.Show("系统提示", "标定库运行异常", exp.Message, eTaskDialogButton.Ok);
           }
#else
            angle += DtopCcdToGlobalAngle;
            AppConfig.Instance.NormalizeAngle(ref angle);
#endif
        }

        /// <summary>
        /// 20170314，转换下相机角度到全局角度
        /// </summary>
        /// <param name="angle"></param>
        private void ConvertBotCcdPRAngleToWorkAngle(ref double angle)
        {
#if _Ues_caliLib_calcAgle_
           try
           {
               angle = (double)OTPCoordSysWrapper.GetThis().GetAglFromUpCam((float)angle);
           }
           catch (Exception exp)
           {
               TaskDialog.Show("系统提示", "标定库运行异常", exp.Message, eTaskDialogButton.Ok);
           }
#else
            angle += DbotCcdToGlobalAngle;
            AppConfig.Instance.NormalizeAngle(ref angle);
#endif
        }
       
    }

    public class ProductInfo
    {
        public string ProductName = "默认产品";
        public string ProductDes = "默认项目文件";
        public string CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        public string CreateLastUpdateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        public string Key = "A93ABED9-4D64-419A-A2E9-1FB643FEA259";
    }
    public class ProductManage
    {
        public string ProductDir = AppDomain.CurrentDomain.BaseDirectory + @"Product\";
        public ProductInfo CurrentProductInfo = null;
        public string[] KeyDirNames = new string[] { @"Vpps\", @"sys\", @"Config\" };
        public List<ProductInfo> LstProductInfos = new List<ProductInfo>();
        public string ProductDesFileName = "_productInfo.json";
        public string CurrentProductName = "默认产品";

        #region 单实例多线程安全
        private volatile static ProductManage _instance = null;
        private static readonly object lockHelper = new object();
        private ProductManage() { }
        public static ProductManage CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                        _instance = new ProductManage();
                }
            }
            return _instance;
        }
        public static ProductManage Instance
        {
            get { return CreateInstance(); }
        }
        #endregion//单实例多线程安全


        public bool RenameProductName(string newName)
        {
            try
            {
                if (Directory.Exists(ProductDir + newName))
                {
                    Log.AddMsg(LogType.Error, "<RenameProductName>", string.Format("当前输入的产品名【{0}】称已经被使用，失败，重命名产品名称失败!", newName));
                 //   ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("当前输入的产品名【{0}】称已经被使用，失败，重命名产品名称失败!", newName));
                    return false;
                }
                Directory.Move(ProductDir + CurrentProductName, ProductDir + newName);
                AppConfig.Instance.SaveCurrentProductName(newName);
                CurrentProductName = newName;
            }
            catch (Exception exp)
            {
                Log.AddMsg(LogType.Error, "<RenameProductName>", string.Format("重命名产品名称为【{0}】失败, 异常：{0}!", exp.Message));
              //  ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("重命名产品名称为【{0}】失败, 异常：{0}!", exp.Message));
                return false;
            }
            return true;
        }

        private bool CopyFileToProductDir(string productDir)
        {
            try
            {
                if (!Directory.Exists(productDir))
                {
                    Directory.CreateDirectory(productDir);
                    if (!Directory.Exists(productDir))
                    {
                        Log.AddMsg(LogType.Error, "<CopyFileToProductDir>", string.Format("创建产品文件目录：{0}，失败，无法进行正常的产品管理操作!", productDir));
                       // ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("创建产品文件目录：{0}，失败，无法进行正常的产品管理操作!", productDir));
                        return false;
                    }
                    Log.AddMsg(LogType.Normal, "<CopyFileToProductDir>", string.Format("创建产品文件目录：{0}，成功", productDir));
                }
                for (int i = 0; i < KeyDirNames.Length; i++)
                {
                    string curDir = productDir + KeyDirNames[i];

                    //确认目录存在
                    if (!Directory.Exists(curDir))
                    {
                        Directory.CreateDirectory(curDir);
                        if (!Directory.Exists(curDir))
                        {
                            Log.AddMsg(LogType.Error, "<CopyFileToProductDir>", string.Format("创建产品文件目录：{0}，失败，无法进行正常的产品管理操作!", curDir));
                           // ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("创建产品文件目录：{0}，失败，无法进行正常的产品管理操作!", curDir));
                            return false;
                        }
                        Log.AddMsg(LogType.Normal, "<CopyFileToProductDir>", string.Format("创建产品文件目录：{0}，成功", curDir));
                    }
                    //Copy 文件
                    string sourcePath = AppDomain.CurrentDomain.BaseDirectory + KeyDirNames[i];
                    string[] files = System.IO.Directory.GetFiles(sourcePath);

                    foreach (string s in files)
                    {
                        string destFile = curDir + Path.GetFileName(s);
                        File.Copy(s, destFile, true);
                        Log.AddMsg(LogType.Normal, "<CopyFileToProductDir>", string.Format("复制文件<{0}>到<{1}>成功", s, destFile));
                    }
                }//end for loop with i
            }//end try
            catch (Exception exp)
            {
                Log.AddMsg(LogType.Error, "<CopyFileToProductDir>", string.Format("复制数据文件到产品文件目录：{0}，失败，无法进行正常的产品管理操作!", ProductDir));
                Log.AddMsg(LogType.Error, "<CopyFileToProductDir>", string.Format("复制数据文件到产品文件目录出现异常：{0}", exp.Message));
                Log.AddMsg(LogType.Error, "<CopyFileToProductDir>", string.Format("复制数据文件到产品文件目录出现异常，堆栈信息如下：{0}", exp.StackTrace));
              //  ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("复制数据文件到产品文件目录：{0}，失败，无法进行正常的产品管理操作!", ProductDir));
                return false;
            }
            return true;
        }

        public bool UpdateProdcutDataForRun(string productDir)
        {
            try
            {
                if (!Directory.Exists(productDir))
                {
                    Log.AddMsg(LogType.Error, "<UpdateProdcutDataForRun>", string.Format("产品文件目录：{0}，不存在，无法进行正常的产品管理操作!", productDir));
                 //   ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("创建产品文件目录：{0}，失败，无法进行正常的产品管理操作!", productDir));
                    return false;
                }
                //先确认必要的几个目录都存在
                bool bKeyDirOK = true;
                for (int i = 0; i < KeyDirNames.Length; i++)
                {
                    string curDir = productDir + KeyDirNames[i];

                    //确认目录存在
                    if (!Directory.Exists(curDir))
                    {
                        Log.AddMsg(LogType.Error, "<UpdateProdcutDataForRun>",
                            string.Format("产品文件目录：{0}，不存在，无法进行正常的产品管理操作!", curDir));
                   //     ShowDialogMsg.Dialog.Show_Error_Close("运行告警",
                       //     string.Format("产品文件目录：{0}，失败，无法进行正常的产品管理操作!", curDir));
                        bKeyDirOK = false;
                    }
                }
                if (!bKeyDirOK)
                {
                    return false;
                }
                for (int i = 0; i < KeyDirNames.Length; i++)
                {
                    string curDir = productDir + KeyDirNames[i];
                    //Copy 文件
                    string sourcePath = AppDomain.CurrentDomain.BaseDirectory + KeyDirNames[i];
                    string[] files = Directory.GetFiles(curDir);

                    foreach (string s in files)
                    {
                        string srcFile = curDir + Path.GetFileName(s);
                        string destFile = AppDomain.CurrentDomain.BaseDirectory + KeyDirNames[i] + Path.GetFileName(s);
                        File.Copy(srcFile, destFile, true);
                        Log.AddMsg(LogType.Normal, "<UpdateProdcutDataForRun>", string.Format("覆盖文件<{0}>到<{1}>成功", srcFile, destFile));
                    }
                }//end for loop with i
            }//end try
            catch (Exception exp)
            {
                Log.AddMsg(LogType.Error, "<UpdateProdcutDataForRun>", string.Format("复制数据文件：{0}，到生产目录失败，无法进行正常的产品管理操作!", ProductDir));
                Log.AddMsg(LogType.Error, "<UpdateProdcutDataForRun>", string.Format("复制数据文件到生产文件目录出现异常：{0}", exp.Message));
                Log.AddMsg(LogType.Error, "<UpdateProdcutDataForRun>", string.Format("复制数据文件到生产文件目录出现异常，堆栈信息如下：{0}", exp.StackTrace));
              //  ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("复制数据文件：{0}，到生产文件目录失败，无法进行正常的产品管理操作!", ProductDir));
                return false;
            }
            return true;
        }

        private bool ValidCheckProductFolder()
        {
            try
            {
                //产品数据目录校验
                if (!Directory.Exists(ProductDir))
                {
                    Directory.CreateDirectory(ProductDir);
                    if (!Directory.Exists(ProductDir))
                    {
                        Log.AddMsg(LogType.Error, "<ValidCheckProductFolder>", string.Format("创建产品文件目录：{0}，失败，无法进行正常的产品管理操作!", ProductDir));
                        //ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("创建产品文件目录：{0}，失败，无法进行正常的产品管理操作!", ProductDir));
                        return false;
                    }
                }
                //默认产品数据校验
                string defaultDir = ProductDir + @"默认产品\";
                if (!Directory.Exists(defaultDir))
                {
                    Log.AddMsg(LogType.ShowNormal, "<ValidCheckProductFolder>", string.Format("开始创建默认产品文件目录{0}，请稍后...", defaultDir));
                    Directory.CreateDirectory(defaultDir);
                    if (!Directory.Exists(defaultDir))
                    {
                        Log.AddMsg(LogType.Error, "<ValidCheckProductFolder>", string.Format("创建默认产品文件目录：{0}，失败，无法进行正常的产品管理操作!", defaultDir));
                      //  ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("创建默认产品文件目录：{0}，失败，无法进行正常的产品管理操作!", defaultDir));
                        return false;
                    }
                    ProductInfo pinfo = new ProductInfo();
                    SaveProductInfo(pinfo);
                    //生成默认产品信息文件
                    Log.AddMsg(LogType.ShowNormal, "<ValidCheckProductFolder>", string.Format("开始复制产品数据到默认产品文件目录，请稍后..."));
                    if (!CopyFileToProductDir(defaultDir))
                    {
                        Log.AddMsg(LogType.Error, "<ValidCheckProductFolder>", string.Format("复制默认产品数据文件到目录：{0}，失败，无法进行正常的产品管理操作!", defaultDir));
                        return false;
                    }
                    Log.AddMsg(LogType.ShowNormal, "<ValidCheckProductFolder>", string.Format("复制产品数据到默认产品文件目录，成功完成"));
                }
            }
            catch (Exception exp)
            {
                Log.AddMsg(LogType.Error, "<ValidCheckProductFolder>", string.Format("产品文件目录校验出现异常：{0}", exp.Message));
                Log.AddMsg(LogType.Error, "<ValidCheckProductFolder>", string.Format("产品文件目录校验出现异常，堆栈信息如下：{0}", exp.StackTrace));
              //  ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("产品文件目录校验出现异常：{0}", exp.Message));
                return false;
            }

            return true;
        }

        public bool LoadProductInfo()
        {
            bool bret = false;

            try
            {
                LstProductInfos.Clear();
                if (!ValidCheckProductFolder())
                {
                    return false;
                }
                List<string> lstPt = Directory.GetDirectories(ProductDir).ToList();
                for (int i = 0; i < lstPt.Count; i++)
                {
                    string strFile = Path.Combine(lstPt[i], ProductDesFileName);
                    string strData = "";

                    if (!File.Exists(strFile))
                    {
                        continue;
                    }
                    using (StreamReader sr = new StreamReader(strFile))
                    {
                        strData = sr.ReadToEnd();
                    }
                    LstProductInfos.Add(JsonConvert.DeserializeObject<ProductInfo>(strData));
                    if (LstProductInfos.Count > 0 && !bret)
                    {
                        if (LstProductInfos[LstProductInfos.Count - 1].ProductName == CurrentProductName)
                        {
                            CurrentProductInfo = LstProductInfos[LstProductInfos.Count - 1];
                            bret = true;
                        }
                    }
                }//end for loop with i
            }
            catch (Exception exp)
            {
                Log.AddMsg(LogType.Error, "<LoadProductInfo>", string.Format("加载产品信息出现异常：{0}", exp.Message));
                Log.AddMsg(LogType.Error, "<LoadProductInfo>", string.Format("加载产品信息出现异常，堆栈信息如下：{0}", exp.StackTrace));
              //  ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("加载产品信息出现异常：{0}", exp.Message));
                return false;
            }

            return bret;
        }

        public bool ValidProductName(string prductName)
        {
            for (int i = 0; i < LstProductInfos.Count; i++)
            {
                if (prductName == LstProductInfos[i].ProductName)
                {
                    return false;
                }
            }
            return true;
        }

        public void SaveAsNewProduct(string productName, string productDes)
        {
            if (!ValidProductName(productName))
            {
                Log.AddMsg(LogType.Error, "<CreateNewProduct>", string.Format("新建产品名称：{0}，重复，创建失败!", productName));
                return;
            }
            string productDir = ProductDir + productName + "\\";
            Directory.CreateDirectory(productDir);
            if (!Directory.Exists(productDir))
            {
                Log.AddMsg(LogType.Error, "<SaveAsNewProduct>", string.Format("创建产品文件目录：{0}，失败，无法进行正常的产品管理操作!", productDir));
               // ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("创建产品文件目录：{0}，失败，无法进行正常的产品管理操作!", productDir));
                return;
            }
            ProductInfo pinfo = new ProductInfo();
            pinfo.ProductName = productName;
            pinfo.ProductDes = productDes;
            SaveProductInfo(pinfo);
            LstProductInfos.Add(pinfo);
            if (CopyFileToProductDir(productDir))
            {
                Log.AddMsg(LogType.ShowNormal, "<CreateNewProduct>", string.Format("新建产品名称：{0}，成功!", productName));
            }
            else
            {
                Log.AddMsg(LogType.Error, "<CreateNewProduct>", string.Format("新建产品名称：{0}，失败!", productName));
            }
        }

        public void SaveCurrentProductData()
        {
            if (null == CurrentProductInfo)
            {
               // ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("当前产品信息为空！无法保存"));
                return;
            }
            string productDir = ProductDir + CurrentProductInfo.ProductName + "\\";
            if (CopyFileToProductDir(productDir))
            {
                Log.AddMsg(LogType.ShowNormal, "<CreateNewProduct>", string.Format("保存产品数据：{0}，成功!", productDir));
            }
            else
            {
                Log.AddMsg(LogType.Error, "<CreateNewProduct>", string.Format("保存产品数据：{0}，失败!", productDir));
            }
        }

        public void SaveCurrentProductInfo()
        {
            try
            {
                if (null == CurrentProductInfo)
                {
                  //  ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("当前产品信息为空！无法保存"));
                    return;
                }
                string strRet = JsonConvert.SerializeObject(CurrentProductInfo);
                string strFile = ProductDir + CurrentProductInfo.ProductName + "\\" + ProductDesFileName;
                using (StreamWriter sw = new StreamWriter(strFile, false))
                {
                    sw.WriteLine(strRet);
                }
            }
            catch (Exception exp)
            {
                Log.AddMsg(LogType.Error, "<SaveCurrentProductInfo>", string.Format("保存当前产品<{1}>描述信息出现异常：{0}", exp.Message, CurrentProductInfo.ProductName));
                Log.AddMsg(LogType.Error, "<SaveCurrentProductInfo>", string.Format("保存当前产品描述信息出现异常，堆栈信息如下：{0}", exp.StackTrace));
               // ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("保存当前产品描述信息出现异常：{0}", exp.Message));
            }
        }

        public void SaveProductInfo(ProductInfo curInfo)
        {
            try
            {
                if (null == curInfo)
                {
                   // ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("当前产品信息为空！无法保存"));
                    return;
                }
                string strRet = JsonConvert.SerializeObject(curInfo);
                string strFile = ProductDir + curInfo.ProductName + "\\" + ProductDesFileName;
                using (StreamWriter sw = new StreamWriter(strFile, false))
                {
                    sw.WriteLine(strRet);
                }
            }
            catch (Exception exp)
            {
                Log.AddMsg(LogType.Error, "<SaveProductInfo>", string.Format("保存产品<{1}>描述信息出现异常：{0}", exp.Message, curInfo.ProductName));
                Log.AddMsg(LogType.Error, "<SaveProductInfo>", string.Format("保存产品描述信息出现异常，堆栈信息如下：{0}", exp.StackTrace));
              //  ShowDialogMsg.Dialog.Show_Error_Close("运行告警", string.Format("保存产品描述信息出现异常：{0}", exp.Message));
            }
        }

    }
    public class LablePrintDataMsg
    {
        //出货条形码
        public static List<string> BarcodeList = new List<string>()
        {
            "24022368","G18080302312","120","DDDDDDD","EEEEEEEEEE"
        };

        public static string ClientLiaoBar = "2405444444";//客户料号
        public static string Carton_NoBar = "M5655444444";// 箱号
        public static string Operator_User = "1234567890";// 箱号
        public static string Qty_Barcount = "120";// 数量
        public static string CellDateToWeeks 
        {
            get { return BarcodeCircle.ToString(); }
        }//电池周期转为周数
        public static string Qty_120 = "120";// 二维码数量数据
      

        public static string Stock_code_Bar = "100110110111";//存货编码

        public static string Bar2Dcode  // DATE CODE 二维码
        {
            get { return "1D" + BarcodeCircle.ToString() + " 1Q" + CellCount.ToString(); }
          
        }
        public static double RealWeight = 7200.0;//实际质量
        public static double TheorWeight = 7200;//理论重量
        public static double BatWeight = 6770;//电池托盘
        public static double PackeageWeight = 430;//包材质量


        public static DateTime productTime = DateTime.Now;//生产日期
        public static DateTime CellDate= DateTime.Now;   //UNDONE::MES提供电池日期
        public static int BarcodeCircle    //电池周期
        {
            get { return (CellDate.Year % 100) * 100 + DatePart(CellDate); }
        }
        public static int  CellCount=120;// 电池个数
        public static string MesInformation="";

        // 产品型号：
        public static string ProductTypes = "HB366481ECW";//产品型号和颜色
        public static string ClientName = "华为";//客户名称
        public static string XWDcode = "A04215";
        public static string StockCode = "1001000057561";//存货编号：1001000057561，电池成品料号，与电池追溯信息一致，固定码，；


        #region 1#打印不良记录信息;
        public static List<string> ErrorMsgList = new List<string>();
        //public static string LogMsgPrint
        //{
        //    get
        //    {
        //        string msg = "";
        //        foreach (var item in ErrorMsgList)
        //        {
        //            msg += item + "\n";
        //        }
        //        return msg;
        //    }
        //}

        public static int circlrCount = 1;
        public static string LogMsgPrint
        {
            get
            {
                string msg = "";
                var txt = AppConfig.ReadIniString("BarIni", "tbCountPerRows", 255, "");
                int count = 1;
                if (txt != "255")
                {
                    count = int.Parse(txt);

                }
                circlrCount = 0;
                foreach (var item in ErrorMsgList)
                {

                    // msg += item + "\n";
                    if (item == "") continue;
                    circlrCount++;
                    if (circlrCount % count == 0)
                        msg += item + "\n";
                    else
                        msg += item + " ";


                }

                return msg;
            }
        }
        #endregion


        /// <summary> 
        /// 取指定日期是一年中的第几周 
        /// </summary> 
        /// <param name="dt">给定的日期</param> 
        /// <returns>数字 一年中的第几周</returns> 
        private static int DatePart(DateTime dt)
        {
            try
            {
                int weeknow = Convert.ToInt32(dt.DayOfWeek);//今天星期几
                int daydiff = (-1) * (weeknow + 1);//今日与上周末的天数差
                int days = System.DateTime.Now.AddDays(daydiff).DayOfYear;//上周末是本年第几天
                int weeks = days / 7;
                if (days % 7 != 0)
                {
                    weeks++;
                }
                //此时，weeks为上周是本年的第几周
                return (weeks + 1);
            }
            catch (Exception ex)
            {
                Log.AddMsg2(LogType.Error, "DatePart:"+ex.Message, dt.ToString()); ;
            }
            return 0;
        }
        
    }


}
