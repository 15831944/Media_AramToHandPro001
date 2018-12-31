using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APPConfig
{
    public class VisionSectionName
    {
        public const string Top = "top";
        public const string BarcodeTop = "barcodeTop";
        public const string BarcodeBottom = "barcodeBottom";
        public const string Bottom = "bottom";
        public const string Bottom_reserve = "bottom_reserve";
        public const string Nozzle = "nozzle";
        public const string Nozzle0 = "nozzle0";
        public const string Nozzle1 = "nozzle1";
        public const string Nozzle2 = "nozzle2";
        public const string Mark0 = "mark0";
        public const string Mark1 = "mark1";

        public const string SocketDut = "SocketDut";
        public const string SocketDut0 = "SocketDut0";
        public const string SocketDut1 = "SocketDut1";

        //spark add
        public const string TopMark = "TopMark";
        public const string TopMatrix = "TopMatrix";
        public const string BotMatrix = "BotMatrix";
        //spark add , 20170115
        public const string TrayHasDut = "TrayHasDut";
        //20170626
        public const string TopBarcodeScan = "TopBarcodeScan";
        public const string BotBarcodeScan = "BotBarcodeScan";
     
        public static string FocusePR_3 =  "FocusePR_3";
        public static string FocusePR_4 =  "FocusePR_4";
        public static string FocusePR_8 =  "FocusePR_8";
        public static string DispensePR_3 = "DispensePR_3";
        public static string DispensePR_4 = "DispensePR_4";
        public static string DispensePR_8 = "DispensePR_8";


        public static string R_YaoxingPr = "R_YaoxingPr";
        public static string R_YuanxingPr = "R_YuanxingPr";

        public static string L_YaoxingPr = "L_YaoxingPr";
        public static string L_YuanxingPr = "L_YuanxingPr";



        
    }

    public class SectionName
    {
        public const string Main = "main";
        public const string Camera = "camera";
        public const string TopCameraCal = "TopCameraCal";
        public const string BottomCameraCal = "BottomCameraCal";
        public const string ServerInfo = "ServerInfo";
        public const string AxisResolution = "AxisResolution";
        public const string AxisErrorGap = "AxisErrorGap";
        public const string HomeParm = "Home";
        public const string HomeOffset = "HomeOffset";
        public const string JogParm = "Jog";
        public const string RunParm = "Run";
        public const string NozzleBasicInfo = "NozzleBasicInfo";
        public const string NozzleInBottomLoc = "NozzleInBottomPRLoc";
        public const string RuntimeParm = "Runtime";
        public const string ZHeight = "ZHeight";
        public const string SocketOffset = "Socket";
        public const string Timeouts = "Timeout";

        public const string visionOpt = "VisionCommunication";
        public const string lightController = "LightController";

        public const string AxisMap = "AxisMap";
        public const string InputMap = "InputMap";
        public const string OutputMap = "OutputMap";
        public const string InputConfig = "InputConfig";
        public const string OutputConfig = "OutputConfig";

        public const string TrayInfo = "TrayInfo";
        public const string NGTrayInfo = "NGTrayInfo";

        public const string LotInfo = "LotInfo";
        public const string InlineConfig = "InlineConfig";

        public const string AxisMLiftLoc = "AxisMLiftLoc";
    }
}
