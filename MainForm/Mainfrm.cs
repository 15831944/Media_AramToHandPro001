using FastColoredTextBoxNS;
using GRBL_Plotter;
using Logger;
using MainForm.DXFviwer;
using MainForm.GcodeFun;
using MainForm.MachineControl;
using netDxf;
//using NetDXFViewer;
//using TestDxfDocument;
//using MainForm.GcodeFun;
//using netDxf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TestDxfDocument;
using FontStyle = System.Drawing.FontStyle;
using GCodeVisuAndTransform = MainForm.GcodeFun.GCodeVisuAndTransform;
using MessageBox = System.Windows.MessageBox;
using Point = System.Drawing.Point;
using Style = FastColoredTextBoxNS.Style;
using toolTable = MainForm.GcodeFun.toolTable;

namespace MainForm
{
    public partial class Mainfrm : Form
    {
        public Mainfrm()
        {
            InitializeComponent();
        }

        #region 系统全部变量
        private string dxfPath = "";
        #endregion
        private void Mainfrm_Load(object sender, EventArgs e)
        {
            try
            {
                #region 加载窗体控件
                // 历史日志记录控件
                panellog.Controls.Add(UcLogList2.Instance);
                UcLogList2.Instance.Dock = DockStyle.Fill;
                Log.Instance.Initialize("LogThread");


                collapsibleSplitContainer2.Panel1.Controls.Add(Uc_ViewGcode.Instance);
              //  splitContainer3.Panel2.Controls.Add(Uc_ViewGcode.Instance);
                Uc_ViewGcode.Instance.Dock = DockStyle.Fill;
                Log.Instance.InitializeGcode("LogThread2");
                //加载控件显示dxf
               // NetDXFViewer.userDxfViwer ndf = new NetDXFViewer.userDxfViwer();
               // elementHost1.Child = ndf;
               // elementHost1.Child = NetDXFViewer.userDxfViwer.Instance;

                #endregion

                #region 加载ini文件
                LoadIni();

                #endregion

                //ControlSetupForm inifrom = new ControlSetupForm();
                //inifrom.LoadDxfIni();
                //inifrom.buttonColorsSeting();
              
                if (_setup_form == null)
                {
                    _setup_form = new MachineControl.ControlSetupForm();
                    _setup_form.FormClosed += formClosed_SetupForm;
                    _setup_form.btnApplyChangings.Click += loadSettings;
                    _setup_form.btnReloadFile.Click += reStartConvertSVG;
                    _setup_form.btnMoveToolXY.Click += moveToPickup;
                    _setup_form.setLastLoadedFile(lastLoadSource);
                    _setup_form.buttonColorsSeting();
                    _setup_form.LoadDxfIni();
                    
                    //gamePadTimer.Enabled = false;
                }
                else
                {
                    _setup_form.Visible = false;
                }

                loadSettings(null,null);
            }
            catch (Exception err)
            {
                MessageBox.Show("Mainfrm_Load:" + err.Message);
            }
          
        }

        private void LoadIni()
        {
            var it = APPConfig.AppConfig.ReadIniString("BackColor", "Color_R", 255, "");
            if (it != "255")
                sliderR.Value = int.Parse(it);
            it = APPConfig.AppConfig.ReadIniString("BackColor", "Color_G", 255, "");
            if (it != "255")
                sliderG.Value = Int32.Parse(it);
            it = APPConfig.AppConfig.ReadIniString("BackColor", "Color_B", 255, "");
            if (it != "255")
                sliderB.Value = int.Parse(it);

            it = APPConfig.AppConfig.ReadIniString("DXFPath", "path", 255, "");
            if (it != "255")
                dxfPath = it;
           
        }

        private void elementHost1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {
           // NetDXFViewer.userDxfViwer.Instance.actualWight = elementHost1.Width;
           // NetDXFViewer.userDxfViwer.Instance.actualHeight = elementHost1.Height;
        }
        System.Windows.Media.Color  m_Color = System.Windows.Media.Color.FromArgb(255, (byte)128, (byte)128, (byte)128);
        private void sliderR_ValueChanged(object sender, EventArgs e)
        {
            BackgroundChnageUI();
            APPConfig.AppConfig.WriteStringIni("BackColor", "Color_R", sliderR.Value.ToString(), "");
            labelXR.Text = sliderR.Value.ToString();
        }


        private void sliderG_ValueChanged(object sender, EventArgs e)
        {
            BackgroundChnageUI();
            APPConfig.AppConfig.WriteStringIni("BackColor", "Color_G", sliderG.Value.ToString(), "");
            labelXG.Text = sliderG.Value.ToString();
        }

        private void sliderB_ValueChanged(object sender, EventArgs e)
        {
            BackgroundChnageUI();
            APPConfig.AppConfig.WriteStringIni("BackColor", "Color_B", sliderB.Value.ToString(), "");
            labelXB.Text = sliderB.Value.ToString();
        }


        private void BackgroundChnageUI()
        {
            m_Color = System.Windows.Media.Color.FromArgb(255, (byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value);
            //NetDXFViewer.userDxfViwer.Instance.bgColor = m_Color;
           // NetDXFViewer.userDxfViwer.Instance.UpdateUI();
        }
       
        private void btMakeGcode_Click(object sender, EventArgs e)
        {
            //  DxfDocument doc = DXF_Operator_Fun.Test(dxfPath);
            //   var it=  NetDXFViewer.DXFoperator.Test(dxfPath);
            DxfDocument doc= TestDxfDocument.DXFoperator.Test(dxfPath,"runCNC");
        }

        private void BtopenDXF_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dxfPath = openFileDialog.FileName;
                    APPConfig.AppConfig.WriteStringIni("DXFPath", "path", dxfPath, "");

                    loadFile(dxfPath);
                   // isHeightMapApplied = false;
                }

            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message);
                //Log.AddMsg2(LogType.Error, "btnOpenFile_Click", err.Message);

            }
        }

        private void loadFile(string fileName)
        {
            //if (fileName.IndexOf("http") >= 0)
            //{
            //    tBURL.Text = fileName;
            //    return;
            //}
            //else
            //{
            //    if (!File.Exists(fileName))
            //    {
            //        MessageBox.Show("File not found: '" + fileName + "'");
            //        return;
            //    }
            //}
            preset2DView();

            String ext = Path.GetExtension(fileName).ToLower();
            if (ext == ".svg")
            {// startConvertSVG(fileName);
            }
            else if (ext == ".dxf")
            {
                startConvertDXF(fileName);
            }
            else if ((ext == ".drd") || (ext == ".drl") || (ext == ".dri"))
            {// startConvertDrill(fileName);
            }
            else if (ext == ".nc")
            {
               // tbFile.Text = fileName;
               // loadGcode();
            }
            else if ((ext == ".bmp") || (ext == ".gif") || (ext == ".png") || (ext == ".jpg"))
            {
                //if (_image_form == null)
                //{
                //    _image_form = new GCodeFromImage(true);
                //    _image_form.FormClosed += formClosed_ImageToGCode;
                //    _image_form.btnGenerate.Click += getGCodeFromImage;      // assign btn-click event
                //}
                //else
                //{
                //    _image_form.Visible = false;
                //}
                //_image_form.Show(this);
                //_image_form.loadExtern(fileName);
            }
          //  SaveRecentFile(fileName);
            setLastLoadedFile("Data from file: " + fileName);

            //if (ext == ".url")
            //{ getURL(fileName); }
            Cursor.Current = Cursors.Default;
            pBoxTransform.Reset();
        }
        private void setLastLoadedFile(string text)
        {
            lastLoadSource = text;
            if (_setup_form != null)
            { _setup_form.setLastLoadedFile(lastLoadSource); }
        }
        private void preset2DView()
        {
            Cursor.Current = Cursors.WaitCursor;
            pictureBox1.BackgroundImage = null;
            visuGCode.setPosMarker(new xyPoint(0, 0));
            visuGCode.createMarkerPath(); ;
            visuGCode.drawMachineLimit(toolTable.getToolCordinates());
        }
        private string lastSource = "";
        private void startConvertDXF(string source)
        {
            lastSource = source;                        // store current file-path/name
            preset2DView();
            string gcode = GCodeFromDXF.ConvertFromFile(source);
            blockFCTB_Events = true;
            if (gcode.Length > 2)
            {
                fCTBCode.Text = gcode;
                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                redrawGCodePath();
               SaveRecentFile(source);
              // this.Text = appName + " | Source: " + source;
            }
            this.Cursor = Cursors.Default;
            //updateControls();
        }
        private int MRUnumber = 20;
        private List<string> MRUlist = new List<string>();
        private void SaveRecentFile(string path)
        {
            //   recentToolStripMenuItem.DropDownItems.Clear();
          //  toolStripMenuItem2.DropDownItems.Clear();
            LoadRecentList(); //load list from file
            if (MRUlist.Contains(path)) //prevent duplication on recent list
                MRUlist.Remove(path);
            MRUlist.Insert(0, path);    //insert given path into list on top
                                        //keep list number not exceeded the given value
            while (MRUlist.Count > MRUnumber)
            { MRUlist.RemoveAt(MRUlist.Count - 1); }
            //foreach (string item in MRUlist)
            //{
            //    ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);
            //    //           recentToolStripMenuItem.DropDownItems.Add(fileRecent);
            //    toolStripMenuItem2.DropDownItems.Add(fileRecent); //add the menu to "recent" menu
            //}
            StreamWriter stringToWrite =
            new StreamWriter(System.Environment.CurrentDirectory + "\\Recent.txt");
            foreach (string item in MRUlist)
            { stringToWrite.WriteLine(item); }
            stringToWrite.Flush(); //write stream to file
            stringToWrite.Close(); //close the stream and reclaim memory
        }
        private void LoadRecentList()
        {
            MRUlist.Clear();
            try
            {
                StreamReader listToRead =
                new StreamReader(System.Environment.CurrentDirectory + "\\Recent.txt");
                string line;
                MRUlist.Clear();
                while ((line = listToRead.ReadLine()) != null) //read each line until end of file
                    MRUlist.Add(line); //insert to list
                listToRead.Close(); //close the stream
            }
            catch (Exception) { }
        }
        //
        private Pen penUp = new Pen(Color.Green, 0.1F);
        private Pen penDown = new Pen(Color.Red, 0.4F);
        private Pen penHeightMap = new Pen(Color.Yellow, 1F);
        private Pen penRuler = new Pen(Color.Blue, 0.1F);
        private Pen penTool = new Pen(Color.Black, 0.5F);
        private Pen penMarker = new Pen(Color.DeepPink, 1F);
        //       SolidBrush machineLimit = new SolidBrush(Color.Red);
        private HatchBrush brushMachineLimit = new HatchBrush(HatchStyle.Horizontal, Color.Yellow);
        private double picAbsPosX = 0;
        private double picAbsPosY = 0;
        private Bitmap picBoxBackround;
        private bool showPicBoxBgImage = false;
        private bool showPathPenUp = true;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                double minx = GCodeVisuAndTransform.drawingSize.minX;                  // extend dimensions
                double maxx = GCodeVisuAndTransform.drawingSize.maxX;
                double miny = GCodeVisuAndTransform.drawingSize.minY;
                double maxy = GCodeVisuAndTransform.drawingSize.maxY;
                double xRange = (maxx - minx);                                              // calculate new size
                double yRange = (maxy - miny);
                double picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
                string unit = (Properties.Settings.Default.importUnitmm) ? "mm" : "Inch";
                if ((picScaling > 0.001) && (picScaling < 10000))
                {
                    double relposX = zoomOffsetX + zoomRange * (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).X) / pictureBox1.Width);
                    double relposY = zoomOffsetY + zoomRange * (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).Y) / pictureBox1.Height);
                    double ratioVisu = xRange / yRange;
                    double ratioPic = Convert.ToDouble(pictureBox1.Width) / pictureBox1.Height;
                    if (ratioVisu > ratioPic)
                        relposY = relposY * ratioVisu / ratioPic;
                    else
                        relposX = relposX * ratioPic / ratioVisu;

                    picAbsPosX = relposX * xRange + minx;
                    picAbsPosY = yRange - relposY * yRange + miny;
                    int offX = +5;

                    if (pictureBox1.PointToClient(MousePosition).X > (pictureBox1.Width - 100))
                    { offX = -75; }

                    Point stringpos = new Point(pictureBox1.PointToClient(MousePosition).X + offX, pictureBox1.PointToClient(MousePosition).Y - 10);
                    e.Graphics.DrawString(String.Format("Worl-Pos:\r\nX:{0,7:0.00}\r\nY:{1,7:0.00}", picAbsPosX, picAbsPosY), new Font("Lucida Console", 8), Brushes.Black, stringpos);
                    e.Graphics.DrawString(String.Format("Zooming   : {0,2:0.00}%\r\nRuler Unit: {1}", 100 / zoomRange, unit), new Font("Lucida Console", 8), Brushes.Black, new Point(5, 5));

                    e.Graphics.Transform = pBoxTransform;
                    e.Graphics.ScaleTransform((float)picScaling, (float)-picScaling);        // apply scaling (flip Y)
                    e.Graphics.TranslateTransform((float)-minx, (float)(-yRange - miny));       // apply offset

                    if (!showPicBoxBgImage)
                        onPaint_drawToolPath(e.Graphics);   // draw real path if background image is not shown
                    e.Graphics.DrawPath(penMarker, GCodeVisuAndTransform.pathMarker);
                    e.Graphics.DrawPath(penTool, GCodeVisuAndTransform.pathTool);
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private void onPaint_scaling(Graphics e)
        {
            double minx =  GCodeVisuAndTransform.drawingSize.minX;                  // extend dimensions
            double maxx =  GCodeVisuAndTransform.drawingSize.maxX;
            double miny =  GCodeVisuAndTransform.drawingSize.minY;
            double maxy = GCodeVisuAndTransform.drawingSize.maxY;
            double xRange = (maxx - minx);                                              // calculate new size
            double yRange = (maxy - miny);
            double picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
            e.ScaleTransform((float)picScaling, (float)-picScaling);        // apply scaling (flip Y)
            e.TranslateTransform((float)-minx, (float)(-yRange - miny));       // apply offset
        }
        private void onPaint_drawToolPath(Graphics e)
        {
            if (Properties.Settings.Default.machineLimitsShow)
            {
                e.FillPath(brushMachineLimit, GCodeVisuAndTransform.pathMachineLimit);
                e.DrawPath(penTool, GCodeVisuAndTransform.pathToolTable);
            }

            e.DrawPath(penHeightMap, GCodeVisuAndTransform.pathMachineLimit);
            if (!Properties.Settings.Default.importUnitmm)
            {
                penDown.Width = 0.01F; penUp.Width = 0.01F; penRuler.Width = 0.01F; penHeightMap.Width = 0.01F;
                penMarker.Width = 0.01F; penTool.Width = 0.01F;
            }
            e.DrawPath(penHeightMap, GCodeVisuAndTransform.pathHeightMap);
            e.DrawPath(penRuler, GCodeVisuAndTransform.pathRuler);
            e.DrawPath(penDown, GCodeVisuAndTransform.pathPenDown);
            if (showPathPenUp)
                e.DrawPath(penUp, GCodeVisuAndTransform.pathPenUp);
        }

        // Generante a background-image for pictureBox to avoid frequent drawing of pen-up/down paths
        private void onPaint_setBackground()
        {
            if (Properties.Settings.Default.guiBackgroundImageEnable)
            {
                showPicBoxBgImage = true;
                pBoxTransform.Reset(); zoomRange = 1; zoomOffsetX = 0; zoomOffsetY = 0;
                pictureBox1.BackgroundImageLayout = ImageLayout.None;
                picBoxBackround = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics graphics = Graphics.FromImage(picBoxBackround);
                graphics.DrawString("Streaming, zooming disabled", new Font("Lucida Console", 8), Brushes.Black, 1, 1);
                onPaint_scaling(graphics);
                onPaint_drawToolPath(graphics);     // draw path
                pictureBox1.BackgroundImage = new Bitmap(picBoxBackround);//Properties.Resources.modell;
            }
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (showPicBoxBgImage)
                onPaint_setBackground();
            pictureBox1.Invalidate();
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (MoveFlag)
            //{
            //    pictureBox1.Left += Convert.ToInt16(e.X - xPos);//设置x坐标.
            //    pictureBox1.Top += Convert.ToInt16(e.Y - yPos);//设置y坐标.
            //}
            //pictureBox1.Invalidate();
        }
        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }


        // find closest coordinate in GCode and mark
        private void pictureBox1_Click(object sender, EventArgs e)
        {   // MessageBox.Show(picAbsPosX + "  " + picAbsPosY);
            pictureBox1.Focus();
            if (fCTBCode.LinesCount > 2)
            {
                int line;
                line = visuGCode.setPosMarkerNearBy(picAbsPosX, picAbsPosY);
                fCTBCode.Selection = fCTBCode.GetLine(line);
                fCTBCodeClickedLineNow = line;
                fCTBCodeMarkLine();
                fCTBCode.DoCaretVisible();
            }
        }


        private Matrix pBoxTransform = new Matrix();
        private static float s_dScrollValue = 2f; // zoom factor   
        private float zoomRange = 1f;
        private float zoomOffsetX = 0f;
        private float zoomOffsetY = 0f;
        bool blockFCTB_Events = true;

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
           
            pictureBox1.Focus();
            if (pictureBox1.Focused == true && e.Delta != 0)
            { ZoomScroll(e.Location, e.Delta > 0); }
        }
        bool MoveFlag = false;int xPos = 0, yPos = 0;
        private void PictureBox1_MouseDown(object sender,MouseEventArgs e)
        {
            MoveFlag = true;//已经按下.
            xPos = e.X;//当前x坐标.
            yPos = e.Y;//当前y坐标.
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            MoveFlag = false;//已经按shang.
        }
        private void ZoomScroll(Point location, bool zoomIn)
        {
            if (showPicBoxBgImage)              // don't zoom if background image is shown
                return;
            float locX = -location.X;
            float locY = -location.Y;
            float posX = (float)location.X / (float)pictureBox1.Width;      // range 0..1
            float posY = (float)location.Y / (float)pictureBox1.Height;     // range 0..1
            float valX = zoomOffsetX + posX * zoomRange;                    // range offset...(offset+range)
            float valY = zoomOffsetY + posY * zoomRange;

            pBoxTransform.Translate(-locX, -locY);
            if (zoomIn)
            {
                pBoxTransform.Scale((float)s_dScrollValue, (float)s_dScrollValue);
                zoomRange *= 1 / s_dScrollValue;
            }
            else
            {
                pBoxTransform.Scale(1 / (float)s_dScrollValue, 1 / (float)s_dScrollValue);
                zoomRange *= s_dScrollValue;
            }
            zoomOffsetX = valX - posX * zoomRange;
            zoomOffsetY = valY - posY * zoomRange;
            pBoxTransform.Translate(locX, locY);
            if (zoomRange == 1)
            { pBoxTransform.Reset(); zoomRange = 1; zoomOffsetX = 0; zoomOffsetY = 0; }

            pictureBox1.Invalidate();
        }

        private bool commentOut;        // comment out unknown GCode to avoid errors from GRBL

        #region fCTB FastColoredTextBox related
        // highlight code in editor
        FastColoredTextBoxNS.Style StyleComment = new TextStyle(Brushes.Gray, null, FontStyle.Italic);
        FastColoredTextBoxNS.Style StyleGWord = new TextStyle(Brushes.Blue, null, System.Drawing.FontStyle.Bold);
        Style StyleMWord = new TextStyle(Brushes.SaddleBrown, null, FontStyle.Regular);
        Style StyleFWord = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        Style StyleSWord = new TextStyle(Brushes.OrangeRed, null, FontStyle.Regular);
        Style StyleTool = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        Style StyleXAxis = new TextStyle(Brushes.Green, null, FontStyle.Bold);
        Style StyleYAxis = new TextStyle(Brushes.BlueViolet, null, FontStyle.Bold);
        Style StyleZAxis = new TextStyle(Brushes.Red, null, FontStyle.Bold);

        private void fCTBCode_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(StyleComment);
            e.ChangedRange.SetStyle(StyleComment, "(\\(.*\\))", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleGWord, "(G\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleMWord, "(M\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleFWord, "(F\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleSWord, "(S\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleTool, "(T\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleXAxis, "[XIxi]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleYAxis, "[YJyj]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleZAxis, "[Zz]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        //bool showChangedMessage = true;     // show Message if TextChanged
        private void fCTBCode_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {   //showChangedMessage = true;
            if (fCTBCode.LinesCount > 2)            // only redraw if GCode is available, otherwise startup picture disappears
            {
                if (commentOut)
                { fCTB_CheckUnknownCode(); }
                pictureBox1.BackgroundImage = null;
                if (!blockFCTB_Events)
                {
                    redrawGCodePath();
                    //               MessageBox.Show("textchangedelayed");
                }
                blockFCTB_Events = false;
            }
        }
        // Refresh drawing path in GCodeVisuAndTransform by applying no transform
        private void redrawGCodePath()
        {
            //System.Diagnostics.StackTrace s = new System.Diagnostics.StackTrace(System.Threading.Thread.CurrentThread, true);
            //MessageBox.Show("Methode B wurde von Methode " + s.GetFrame(1).GetMethod().Name + " aufgerufen");
            visuGCode.getGCodeLines(fCTBCode.Lines);
            updateGUI();
            /*           updateDrawing();
                        lbDimension.Text = visuGCode.xyzSize.getMinMaxString(); //String.Format("X:[ {0:0.0} | {1:0.0} ];    Y:[ {2:0.0} | {3:0.0} ];    Z:[ {4:0.0} | {5:0.0} ]", visuGCode.xyzSize.minx, visuGCode.xyzSize.maxx, visuGCode.xyzSize.miny, visuGCode.xyzSize.maxy, visuGCode.xyzSize.minz, visuGCode.xyzSize.maxz);
                        lbDimension.Select(0, 0);
                        toolStrip_tb_XY_X_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimx);
                        toolStrip_tb_X_X_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimx);
                        toolStrip_tb_XY_Y_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimy);
                        toolStrip_tb_Y_Y_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimy);
             */
        }
        private void updateGUI()
        {
            updateDrawing();
           // lbDimension.Text = visuGCode.xyzSize.getMinMaxString(); //String.Format("X:[ {0:0.0} | {1:0.0} ];    Y:[ {2:0.0} | {3:0.0} ];    Z:[ {4:0.0} | {5:0.0} ]", visuGCode.xyzSize.minx, visuGCode.xyzSize.maxx, visuGCode.xyzSize.miny, visuGCode.xyzSize.maxy, visuGCode.xyzSize.minz, visuGCode.xyzSize.maxz);
           // lbDimension.Select(0, 0);
          //  checkMachineLimit();
            //toolStrip_tb_XY_X_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimx);
            //toolStrip_tb_X_X_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimx);
            //toolStrip_tb_XY_Y_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimy);
            //toolStrip_tb_Y_Y_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimy);
        }

        // update drawing on Main form and enable / disable 
        private void updateDrawing()
        {
            visuGCode.createImagePath();                                // show initial empty picture . just ruler and tool-pos
           // visuGCode.drawMachineLimit(toolTable.getToolCordinates());
            pictureBox1.Invalidate();                                   // resfresh view
            if (visuGCode.containsG2G3Command())                        // disable X/Y independend scaling if G2 or G3 GCode is in use
            {                                                           // because it's not possible to stretch (convert 1st to G1 GCode)                skaliereXUmToolStripMenuItem.Enabled = false;
                //skaliereAufXUnitsToolStripMenuItem.Enabled = false;
                //skaliereYUmToolStripMenuItem.Enabled = false;
                //skaliereAufYUnitsToolStripMenuItem.Enabled = false;
                //skaliereXAufDrehachseToolStripMenuItem.Enabled = false;
                //skaliereYAufDrehachseToolStripMenuItem.Enabled = false;
                //ersetzteG23DurchLinienToolStripMenuItem.Enabled = true;
            }
            else
            {
                //skaliereXUmToolStripMenuItem.Enabled = true;                // enable X/Y independend scaling because no G2 or G3 GCode
                //skaliereAufXUnitsToolStripMenuItem.Enabled = true;
                //skaliereYUmToolStripMenuItem.Enabled = true;
                //skaliereAufYUnitsToolStripMenuItem.Enabled = true;
                //skaliereXAufDrehachseToolStripMenuItem.Enabled = true;
                //skaliereYAufDrehachseToolStripMenuItem.Enabled = true;
                //ersetzteG23DurchLinienToolStripMenuItem.Enabled = false;
            }
        }
        private void fCTB_CheckUnknownCode()
        {
            string curLine;
            string allowed = "NGMFIJKLNPRSTUVWXYZOPLngmfijklnprstuvwxyzopl ";
            string number = " +-.0123456789";
            string cmt = "(;";
            string message = "";
            for (int i = 0; i < fCTBCode.LinesCount; i++)
            {
                curLine = fCTBCode.Lines[i].Trim();
                if ((curLine.Length > 0) && (cmt.IndexOf(curLine[0]) >= 0))             // if comment, nothing to do
                {
                    if (curLine[0] == '(')
                    {
                        if (curLine.IndexOf(')') < 0)                               // if last ')' is missing
                        {
                            fCTBCode.Selection = fCTBCode.GetLine(i);
                            fCTBCode.SelectedText = curLine + " <- unknown command)";
                            message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                        }
                    }
                }
                else if ((curLine.Length > 0) && (allowed.IndexOf(curLine[0]) < 0))     // if 1st char is unknown - no gcode
                {
                    fCTBCode.Selection = fCTBCode.GetLine(i);
                    fCTBCode.SelectedText = "(" + curLine + " <- unknown command)";
                    message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                }
                else if ((curLine.Length > 1) && (number.IndexOf(curLine[1]) < 0))  // if 1st known but 2nd not part of number
                {
                    fCTBCode.Selection = fCTBCode.GetLine(i);
                    fCTBCode.SelectedText = "(" + curLine + " <- unknown command)";
                    message += "Line " + i.ToString() + " : " + curLine + "\r\n";
                }
            }
            if (message.Length > 0)
                System.Windows.MessageBox.Show("Fixed some unknown GCode:\r\n" + message);
        }

        // mark clicked line in editor
        int fCTBCodeClickedLineNow = 0;
        int fCTBCodeClickedLineLast = 0;
        public GCodeVisuAndTransform visuGCode = new GCodeVisuAndTransform();
        private void fCTBCode_Click(object sender, EventArgs e)
        {
            fCTBCodeClickedLineNow = fCTBCode.Selection.ToLine;
            fCTBCodeMarkLine();
            //           MessageBox.Show(visuGCode.getLineInfo(fCTBCodeClickedLineNow));
            //            fCTBCode.t  (visuGCode.getLineInfo(fCTBCodeClickedLineNow));
        }
        private void fCTBCode_KeyDown(object sender, KeyEventArgs e)
        {
            int key = e.KeyValue;
            if ((key == 38) && (fCTBCodeClickedLineNow > 0))
            {
                fCTBCodeClickedLineNow -= 1;
                fCTBCode.Selection = fCTBCode.GetLine(fCTBCodeClickedLineNow);
                fCTBCodeMarkLine();
            }
            if ((key == 40) && (fCTBCodeClickedLineNow < (fCTBCode.Lines.Count - 1)))
            {
                fCTBCodeClickedLineNow += 1;
                fCTBCode.Selection = fCTBCode.GetLine(fCTBCodeClickedLineNow);
                fCTBCodeMarkLine();
            }
        }
        private void fCTBCodeMarkLine()
        {
            if ((fCTBCodeClickedLineNow <= fCTBCode.LinesCount) && (fCTBCodeClickedLineNow >= 0))
            {
                if (fCTBCodeClickedLineNow != fCTBCodeClickedLineLast)
                {
                    fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                    fCTBCode.BookmarkLine(fCTBCodeClickedLineNow);
                    Range selected = fCTBCode.GetLine(fCTBCodeClickedLineNow);
                    fCTBCode.Selection = selected;
                    fCTBCode.SelectionColor = Color.Orange;
                    fCTBCodeClickedLineLast = fCTBCodeClickedLineNow;
                    // Set marker in drawing
                    //visuGCode.setMarkerOnDrawing(fCTBCode.SelectedText);
                    visuGCode.setPosMarkerLine(fCTBCodeClickedLineNow);
                    pictureBox1.Invalidate(); // avoid too much events
                    //if (_camera_form != null)
                    //{
                    //    _camera_form.setPosMarker(visuGCode.GetPosMarker());// X(), visuGCode.GetPosMarkerY());
                    //    //MessageBox.Show("x "+visuGCode.GetPosMarkerX().ToString()+ "  y "+ visuGCode.GetPosMarkerY().ToString());
                    //}
                }
            }
        }
        private void fCTBCode_MouseHover(object sender, EventArgs e)
        {
            fCTBCode.Focus();
        }
        // context Menu on fastColoredTextBox
        private void cmsCode_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "cmsCodeSelect")
            {
                fCTBCode.SelectAll();
            }
            if (e.ClickedItem.Name == "cmsCodeCopy")
            {
                if (fCTBCode.SelectedText.Length > 0)
                    fCTBCode.Copy();
            }
            if (e.ClickedItem.Name == "cmsCodePaste")
            {
                fCTBCode.Paste();
            }
            if (e.ClickedItem.Name == "cmsCodeSendLine")
            {
                int clickedLine = fCTBCode.Selection.ToLine;
             //   sendCommand(fCTBCode.Lines[clickedLine], false);
                //MessageBox.Show(fCTBCode.Lines[clickedLine]);
            }
            if (e.ClickedItem.Name == "cmsCommentOut")
            {
                fCTB_CheckUnknownCode();
            }
        }

        #endregion
        MachineControl.ControlSetupForm _setup_form;
        private string lastLoadSource = "Nothing loaded";
        private void btSetting_Click(object sender, EventArgs e)
        {
            if (_setup_form == null)
            {
                _setup_form = new MachineControl.ControlSetupForm();
                _setup_form.FormClosed += formClosed_SetupForm;
                _setup_form.btnApplyChangings.Click += loadSettings;
                _setup_form.btnReloadFile.Click += reStartConvertSVG;
                _setup_form.btnMoveToolXY.Click += moveToPickup;
                _setup_form.setLastLoadedFile(lastLoadSource);

                
                //gamePadTimer.Enabled = false;
            }
            else
            {
                _setup_form.Visible = false;
            }
            _setup_form.Show(this);
        }

        private void formClosed_SetupForm(object sender, FormClosedEventArgs e)
        {
            loadSettings(sender, e);
            _setup_form = null;
            updateDrawing();
           // gamePadTimer.Enabled = Properties.Settings.Default.gPEnable;
        }
        private bool isStreaming = false;
        public void reStartConvertSVG(object sender, EventArgs e)   // event from setup form
        {
            if (!isStreaming)
            {
                //this.Cursor = Cursors.WaitCursor;
                //if (lastLoadSource.IndexOf("Clipboard") >= 0)
                //{ loadFromClipboard(); }
                //else
                //{ loadFile(lastSource); }
                //this.Cursor = Cursors.Default;
            }
        }
        public void moveToPickup(object sender, EventArgs e)   // event from setup form
        {
          //  sendCommand(_setup_form.commandToSend);
            _setup_form.commandToSend = "";
        }
        // load settings
        public void loadSettings(object sender, EventArgs e)
        {
            try
            {
                if (Properties.Settings.Default.UpgradeRequired)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.UpgradeRequired = false;
                    Properties.Settings.Default.Save();
                }
               // tbFile.Text = Properties.Settings.Default.file;
                //setCustomButton(btnCustom1, Properties.Settings.Default.custom1);
                //setCustomButton(btnCustom2, Properties.Settings.Default.custom2);
                //setCustomButton(btnCustom3, Properties.Settings.Default.custom3);
                //setCustomButton(btnCustom4, Properties.Settings.Default.custom4);
                //setCustomButton(btnCustom5, Properties.Settings.Default.custom5);
                //setCustomButton(btnCustom6, Properties.Settings.Default.custom6);
                //setCustomButton(btnCustom7, Properties.Settings.Default.custom7);
                //setCustomButton(btnCustom8, Properties.Settings.Default.custom8);
                fCTBCode.BookmarkColor = Properties.Settings.Default.colorMarker; ;
                pictureBox1.BackColor = Properties.Settings.Default.colorBackground;
                //                visuGCode.setColors();
                penUp.Color = Properties.Settings.Default.colorPenUp;
                penDown.Color = Properties.Settings.Default.colorPenDown;
                penHeightMap.Color = Properties.Settings.Default.colorHeightMap;
                penRuler.Color = Properties.Settings.Default.colorRuler;
                penTool.Color = Properties.Settings.Default.colorTool;
                penMarker.Color = Properties.Settings.Default.colorMarker;
                penHeightMap.Width = (float)Properties.Settings.Default.widthHeightMap;
                penRuler.Width = (float)Properties.Settings.Default.widthRuler;
                penUp.Width = (float)Properties.Settings.Default.widthPenUp;
                penDown.Width = (float)Properties.Settings.Default.widthPenDown;
                penTool.Width = (float)Properties.Settings.Default.widthTool;
                penMarker.Width = (float)Properties.Settings.Default.widthMarker;
                brushMachineLimit = new HatchBrush(HatchStyle.DiagonalCross, Properties.Settings.Default.colorMachineLimit, Color.Transparent);
                picBoxBackround = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                commentOut = Properties.Settings.Default.ctrlCommentOut;
                updateDrawing();

                //joystickXYStep[1] = (double)Properties.Settings.Default.joyXYStep1;
                //joystickXYStep[2] = (double)Properties.Settings.Default.joyXYStep2;
                //joystickXYStep[3] = (double)Properties.Settings.Default.joyXYStep3;
                //joystickXYStep[4] = (double)Properties.Settings.Default.joyXYStep4;
                //joystickXYStep[5] = (double)Properties.Settings.Default.joyXYStep5;
                //joystickZStep[1] = (double)Properties.Settings.Default.joyZStep1;
                //joystickZStep[2] = (double)Properties.Settings.Default.joyZStep2;
                //joystickZStep[3] = (double)Properties.Settings.Default.joyZStep3;
                //joystickZStep[4] = (double)Properties.Settings.Default.joyZStep4;
                //joystickZStep[5] = (double)Properties.Settings.Default.joyZStep5;
                //joystickXYSpeed[1] = (double)Properties.Settings.Default.joyXYSpeed1;
                //joystickXYSpeed[2] = (double)Properties.Settings.Default.joyXYSpeed2;
                //joystickXYSpeed[3] = (double)Properties.Settings.Default.joyXYSpeed3;
                //joystickXYSpeed[4] = (double)Properties.Settings.Default.joyXYSpeed4;
                //joystickXYSpeed[5] = (double)Properties.Settings.Default.joyXYSpeed5;
                //joystickZSpeed[1] = (double)Properties.Settings.Default.joyZSpeed1;
                //joystickZSpeed[2] = (double)Properties.Settings.Default.joyZSpeed2;
                //joystickZSpeed[3] = (double)Properties.Settings.Default.joyZSpeed3;
                //joystickZSpeed[4] = (double)Properties.Settings.Default.joyZSpeed4;
                //joystickZSpeed[5] = (double)Properties.Settings.Default.joyZSpeed5;
                //virtualJoystickXY.JoystickLabel = joystickXYStep;
                //virtualJoystickZ.JoystickLabel = joystickZStep;
                //virtualJoystickA.JoystickLabel = joystickZStep;
                //skaliereXAufDrehachseToolStripMenuItem.Enabled = false;
                //skaliereXAufDrehachseToolStripMenuItem.BackColor = SystemColors.Control;
                //skaliereXAufDrehachseToolStripMenuItem.ToolTipText = "Enable rotary axis in Setup - Control";
                //skaliereAufXUnitsToolStripMenuItem.BackColor = SystemColors.Control;
                //skaliereAufXUnitsToolStripMenuItem.ToolTipText = "Enable in Setup - Control";
                //skaliereYAufDrehachseToolStripMenuItem.Enabled = false;
                //skaliereYAufDrehachseToolStripMenuItem.BackColor = SystemColors.Control;
                //skaliereYAufDrehachseToolStripMenuItem.ToolTipText = "Enable rotary axis in Setup - Control";
                //skaliereAufYUnitsToolStripMenuItem.BackColor = SystemColors.Control;
                //skaliereAufYUnitsToolStripMenuItem.ToolTipText = "Enable in Setup - Control";
                //toolStrip_tb_rotary_diameter.Text = string.Format("{0:0.00}", Properties.Settings.Default.rotarySubstitutionDiameter);

                if (Properties.Settings.Default.rotarySubstitutionEnable)
                {
                    string tmp = string.Format("Calculating rotary angle depending on part diameter ({0:0.00} units) and desired size.\r\nSet part diameter in Setup - Control.", Properties.Settings.Default.rotarySubstitutionDiameter);
                    //if (Properties.Settings.Default.rotarySubstitutionX)
                    //{
                    //    skaliereXAufDrehachseToolStripMenuItem.Enabled = true;
                    //    skaliereXAufDrehachseToolStripMenuItem.BackColor = Color.Yellow;
                    //    skaliereAufXUnitsToolStripMenuItem.BackColor = Color.Yellow;
                    //    skaliereAufXUnitsToolStripMenuItem.ToolTipText = tmp;
                    //    skaliereXAufDrehachseToolStripMenuItem.ToolTipText = "";
                    //}
                    //else
                    //{
                    //    skaliereYAufDrehachseToolStripMenuItem.Enabled = true;
                    //    skaliereYAufDrehachseToolStripMenuItem.BackColor = Color.Yellow;
                    //    skaliereAufYUnitsToolStripMenuItem.BackColor = Color.Yellow;
                    //    skaliereAufYUnitsToolStripMenuItem.ToolTipText = tmp;
                    //    skaliereYAufDrehachseToolStripMenuItem.ToolTipText = "";
                    //}
                }
                if (Properties.Settings.Default.rotarySubstitutionSetupEnable)
                {
                    string[] commands;
                    if (Properties.Settings.Default.rotarySubstitutionEnable)
                    { commands = Properties.Settings.Default.rotarySubstitutionSetupOn.Split(';'); }
                    else
                    { commands = Properties.Settings.Default.rotarySubstitutionSetupOff.Split(';'); }
                    //if (_serial_form.serialPortOpen)
                    //    foreach (string cmd in commands)
                    //    {
                    //        sendCommand(cmd.Trim());
                    //        Thread.Sleep(100);
                    //    }
                }

                //ctrl4thAxis = Properties.Settings.Default.ctrl4thUse;
                //ctrl4thName = Properties.Settings.Default.ctrl4thName;
                //label_a.Visible = ctrl4thAxis;
                //label_a.Text = ctrl4thName;
                //label_wa.Visible = ctrl4thAxis;
                //label_ma.Visible = ctrl4thAxis;
                //btnZeroA.Visible = ctrl4thAxis;
                //btnZeroA.Text = "Zero " + ctrl4thName;
                //if (Properties.Settings.Default.language == "de-DE")
                //    btnZeroA.Text = ctrl4thName + " nullen";

                //virtualJoystickA.Visible = ctrl4thAxis;
                //btnJogZeroA.Visible = ctrl4thAxis;
                //btnJogZeroA.Text = ctrl4thName + "=0";
                //if (ctrl4thAxis)
                //{
                //    label_status0.Location = new Point(1, 128);
                //    label_status.Location = new Point(1, 148);
                //    btnHome.Location = new Point(122, 138);
                //    btnHome.Size = new Size(117, 30);
                //    virtualJoystickXY.Size = new Size(160, 160);
                //    virtualJoystickZ.Size = new Size(30, 160);
                //    virtualJoystickZ.Location = new Point(166, 115);
                //}
                //else
                //{
                //    label_status0.Location = new Point(1, 118);
                //    label_status.Location = new Point(1, 138);
                //    btnHome.Location = new Point(122, 111);
                //    btnHome.Size = new Size(117, 57);
                //    virtualJoystickXY.Size = new Size(180, 180);
                //    virtualJoystickZ.Size = new Size(40, 180);
                //    virtualJoystickZ.Location = new Point(186, 115);
                //}
                //gamePadTimer.Enabled = Properties.Settings.Default.gPEnable;
                checkMachineLimit();
            }
            catch (Exception a)
            {
                MessageBox.Show("Load Settings: " + a);
                //               logError("Loading settings", e);
            }
        }

        //     private static int limitcnt = 0;
        private void checkMachineLimit()
        {
            if ((Properties.Settings.Default.machineLimitsShow) && (pictureBox1.BackgroundImage == null))
           {
            //    if (!visuGCode.xyzSize.withinLimits(posMachine, posWorld))
            //    {
            //        lbDimension.BackColor = Color.Fuchsia;
            //        decimal minx = Properties.Settings.Default.machineLimitsHomeX;
            //        decimal maxx = minx + Properties.Settings.Default.machineLimitsRangeX;
            //        decimal miny = Properties.Settings.Default.machineLimitsHomeY;
            //        decimal maxy = miny + Properties.Settings.Default.machineLimitsRangeY;
            //        btnLimitExceed.Visible = true;
            //    }
            //    else
            //    {
            //        lbDimension.BackColor = Color.Lime;
            //        toolTip1.SetToolTip(lbDimension, "");
            //        btnLimitExceed.Visible = false;
            //    }
            }
           else
            {
                   // lbDimension.BackColor = Color.FromArgb(255, 255, 128);
                   // btnLimitExceed.Visible = false;
                }
            }
        // Save settings
        public void saveSettings()
        {
            try
            {
                //Properties.Settings.Default.file = tbFile.Text;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                MessageBox.Show("Save Settings: " + e);
                //               logError("Saving settings", e);
            }
        }

        private void btSettings_Click(object sender, EventArgs e)
        {
            btSetting_Click(sender, e);
            
        }
        public DXF2WPF myDXF = new DXF2WPF();
        private void btLoaddxfViwer_Click(object sender, EventArgs e)
        {
            myDXF = new DXF2WPF();
            myDXF.GridHeight = 5000;
            myDXF.GridWidth = 5000;
            myDXF.ViewHeight = elementHost1.Height;
            myDXF.ViewWidth = elementHost1.Width;
            myDXF.WinHeight = this.Height;
            myDXF.WinWidth = this.Width;

            myDXF.border.Reset(myDXF.GridHeight, myDXF.GridWidth, true, this.Height, this.Width, this.Height, this.Width);
            myDXF.GetMainGrid( true, true);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                String fileDXF = openFileDialog.FileName;
                 elementHost1.Child= myDXF.GetMainGrid(fileDXF, true, true);
                // myDXF.border.ZoomAuto(5000, 5000, elementHost1.Height, elementHost1.Width);

            }
        }

        private void btCreateDxfToGcode_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string dxfPaths = openFileDialog.FileName;

                    
                    var name = @"D:\WorkPro\Debug\DXF_files\G_" + openFileDialog.SafeFileName;// + DateTime.Now.ToString("hhmmsss");

                    DXFoperaotor2 dXFoperaotor2 = new DXFoperaotor2(dxfPaths);

                    dXFoperaotor2.SaveDXF_GcodeDatadfun(name);

                }

            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message);

            }
        }

        private void btCreateDxfToTable_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string dxfPaths = openFileDialog.FileName;

                   
                    var name = @"D:\WorkPro\Debug\DXF_files\T_" + openFileDialog.SafeFileName;// + DateTime.Now.ToString("hhmmsss");
                  
                    DXFoperaotor2 dXFoperaotor2 = new DXFoperaotor2(dxfPaths);

                    dXFoperaotor2.SaveTableTxtfun(name);

                }

            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message);

            }
        }

        private void btInformationdxf_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string dxfPaths = openFileDialog.FileName;

                    DXFoperaotor2.InforMationOnDxf(dxfPaths);
                }

            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message);

            }
        }

        private void btLoadDxftoviewer_Click(object sender, EventArgs e)
        {
          
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string dxfPaths = openFileDialog.FileName;

                   
                    DXFoperaotor2 dXFoperaotor2 = new DXFoperaotor2(dxfPaths);
                    dXFoperaotor2.SaveTableTxtfun("2015555");
                    Image images=  dXFoperaotor2.getImageFromDxf(dxfPaths, pictureBox2.Height, pictureBox2.Width,pictureBox2);
                   // pictureBox2.Image = images;// 
                   // pictureBox2.Update();
                }

            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message);

            }
        }

        private void btCreatNewDxf_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                   string dxfPaths = openFileDialog.FileName;
                 
                    //DxfDocument doc = TestDxfDocument.DXFoperator.Test(dxfPaths, "");
                    var name = @"D:\WorkPro\Debug\DXF_files\T_" + openFileDialog.SafeFileName;// + DateTime.Now.ToString("hhmmsss");
                    //var result=TestDxfDocument.DXFoperator.SaveDxf(name, TestDxfDocument.DXFoperator.linesStr);
                    //Log.AddMsg2("Dxf转化完成:" + name + "testDxfs.dxf");



                    DXFoperaotor2 dXFoperaotor2 = new DXFoperaotor2(dxfPaths);

                    dXFoperaotor2.SaveTableTxtfun(name);

                }

            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message);
             
            }
        }
    }
}
