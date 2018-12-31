using Logger;
using Microsoft.Win32;
using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetDXFViewer
{
    /// <summary>
    /// userDxfViwer.xaml 的交互逻辑
    /// </summary>
    public partial class userDxfViwer : UserControl
    {
        public readonly static object mutx = new object();
        private static userDxfViwer instance = null;
        public static userDxfViwer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (mutx)
                    {
                        instance = new userDxfViwer();
                    }
                }
                return instance;
            }
        }
        public Color bgColor
        {
            set {DXF2WPF.Window_bgColor = value; }
            get { return DXF2WPF.Window_bgColor; }
        }

        
        public userDxfViwer()
        {
            InitializeComponent();

            //添加控件
            #region 
            myDXF.GridHeight = 5000;
            myDXF.GridWidth = 5000;
            myDXF.ViewHeight = grid1.ActualHeight;
            myDXF.ViewWidth = grid1.ActualWidth;
            myDXF.WinHeight = this.Height;
            myDXF.WinWidth = this.Width;
           // DXF2WPF.Window_bgColor = bgColor;
            myDXF.border.Reset(myDXF.GridHeight, myDXF.GridWidth, true, this.Height, this.Width, this.Height, this.Width);
            DrawDXF();

            Button resetBtn = new Button();
            resetBtn.Width = 120;
            resetBtn.VerticalAlignment = VerticalAlignment.Top;
            resetBtn.HorizontalAlignment = HorizontalAlignment.Left;
            resetBtn.Content = "Reset";
            resetBtn.Click += ZoomOut_Click;
            StackPanel stack = new StackPanel();
            stack.Children.Add(resetBtn);


            Button openBtn = new Button();
            openBtn.Width = 120;
            openBtn.VerticalAlignment = VerticalAlignment.Top;
            openBtn.HorizontalAlignment = HorizontalAlignment.Left;
            openBtn.Content = "Open";
            openBtn.Click += btnOpenFile_Click;


            stack.Children.Add(openBtn);

            Button ZoomAutoBtn = new Button();
            ZoomAutoBtn.Width = 120;
            ZoomAutoBtn.VerticalAlignment = VerticalAlignment.Top;
            ZoomAutoBtn.HorizontalAlignment = HorizontalAlignment.Left;
            ZoomAutoBtn.Content = "ZoomAuto";
            ZoomAutoBtn.Click += ZoomAuto_Click;

            stack.Children.Add(ZoomAutoBtn);

            Button ColorBtn = new Button();
            ColorBtn.Width = 120;
            ColorBtn.VerticalAlignment = VerticalAlignment.Top;
            ColorBtn.HorizontalAlignment = HorizontalAlignment.Left;
            ColorBtn.Content = "ColorBtn";
            //ColorBtn.Click += ColorBtn_Click; ;

            stack.Children.Add(ColorBtn);

            ComboBox cbb = new ComboBox();
            cbb.Items.Add(1); cbb.Items.Add(0.5); cbb.Items.Add(0.2);
            cbb.Items.Add(0.1); cbb.Items.Add(0.05); cbb.Items.Add(0.02);
            cbb.Items.Add(0.01);
            cbb.SelectedIndex = 0;
            cbb.VerticalAlignment = VerticalAlignment.Top;
            cbb.HorizontalAlignment = HorizontalAlignment.Left;
            cbb.SelectionChanged += Cbb_SelectionChanged;
            cbb.Width = 120;
            stack.Children.Add(cbb);

            myDXF.mainGrid.Children.Add(stack);

            #endregion
        }
        public DXF2WPF myDXF = new DXF2WPF();
        public double actualHeight = 500;
        public double actualWight = 800;


        private void Cbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            var val = (sender as ComboBox).SelectedItem.ToString();
            myDXF.Scale = double.Parse(val);
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //Point current = myDXF.border.CurrentPosition(myDXF.GridHeight,myDXF.GridWidth,((Grid)Application.Current.MainWindow.Content).ActualHeight,((Grid)Application.Current.MainWindow.Content).ActualWidth);
                //DrawUtils.DrawPoint(current.X,current.Y,this.myDXF.mainCanvas,Colors.Red,25,1);
                double scale = myDXF.Scale;// 修改比例

                var it = grid1.Height;
                myDXF.border.Zoom(myDXF.GridHeight, myDXF.GridWidth, actualHeight, actualWight, 0, 0, scale);
                //myDXF.border.Zoom(myDXF.GridHeight, myDXF.GridWidth, ((Grid)Application.Current.MainWindow.Content).ActualHeight, ((Grid)Application.Current.MainWindow.Content).ActualWidth, 0, 0, scale);

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                Log.AddMsg2(LogType.Error, "ZoomOut_Click", err.Message);

            }
        }


        private void ZoomAuto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                //  myDXF.border.ZoomAuto(myDXF.GridHeight, myDXF.GridWidth, ((Grid)Application.Current.MainWindow.Content).ActualHeight, ((Grid)Application.Current.MainWindow.Content).ActualWidth);
                myDXF.border.ZoomAuto(5000, 5000, actualHeight, actualWight);
                //myDXF.border.ZoomAuto(5000,5000,Application.Current.MainWindow.Height,Application.Current.MainWindow.Width);
            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message);
                Log.AddMsg2(LogType.Error, "ZoomAuto_Click", err.Message);


            }
            
        }


        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {


            myDXF.border.Reset();
            /*myDXF.border.ZoomOut(myDXF.GridHeight,myDXF.GridWidth,myDXF.WinHeight,myDXF.WinWidth);
			myDXF.border.ZoomAuto(5000,5000,523,784);*/

        }

        private void DrawDXF_Click(object sender, RoutedEventArgs e)
        {
            DrawDXF();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    PathFiles = openFileDialog.FileName;
                    // var length=AppDomain.CurrentDomain.u
                    
                    DrawDXF(PathFiles);
                }

            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message);
                Log.AddMsg2(LogType.Error, "btnOpenFile_Click", err.Message);


            }
      
        }
        public string PathFiles = "";
        private void DrawDXF(string fileDXF)
        {

            TypeConverter.defaultThickness = 0.01;


            myDXF.DxfDoc = new DxfDocument();



            /*
			netDxf.Blocks.Block myBlock = netDxf.Blocks.Block.Load("P4035PINM.dxf");
			netDxf.Entities.Insert myInsert = new netDxf.Entities.Insert(myBlock);
			myInsert.Lineweight= Lineweight.W100;
			myInsert.LinetypeScale= 100;
			myInsert.Position = new Vector3(0,100,0);
			myInsert.Scale = new Vector3(-2,2,0);
			Vector3 pos0 = new Vector3(myInsert.Position.X,myInsert.Position.Y,0);
			myInsert.Position = pos0;
			myInsert.Rotation = 0;
			AciColor bgcolor = new AciColor();
			myDXF.DxfDoc.AddEntity(myInsert);
			*/



            /*netDxf.Blocks.Block myBlock2 = netDxf.Blocks.Block.Load(fileDXF);

			netDxf.Entities.Insert myInsert2 = new netDxf.Entities.Insert(myBlock2);
			myInsert2.Position = new Vector3(30,40,0);
			myInsert2.Scale = new Vector3(1,1,0);
			
			myInsert2.Color = AciColor.Blue;

			myInsert2.Rotation = 0;

			myDXF.DxfDoc.AddEntity(myInsert2);*/



            /*
			netDxf.Entities.Insert myInsert2 = new netDxf.Entities.Insert(myBlock);

			Vector3 scaleInsert = new Vector3(1,-1,1);
			myInsert2.Scale = scaleInsert;
			Vector3 pos= new Vector3(myInsert2.Position.X+5,myInsert2.Position.Y,0);
			myInsert2.Position = pos;
			myDXF.DxfDoc.AddEntity(myInsert2);
			 */

            if (fileDXF == "")
            {
                OpenFileDialog of = new OpenFileDialog();
                if (of.ShowDialog() == true)
                    PathFiles= fileDXF = of.FileName; ;
            }
            if (fileDXF != "")
            {
                this.Content = myDXF.GetMainGrid(fileDXF, true, true);
                //DrawUtils.SaveAsPng(DrawUtils.GetImage(myDXF.mainCanvas));

            }
            else
            {
                this.Content = myDXF.GetMainGrid(true, true);
            }
            var length = UserControl.UseLayoutRoundingProperty;
           // myDXF.border.ZoomAuto(5000,5000,((Grid)Application.Current.MainWindow.Content).ActualHeight,((Grid)Application.Current.MainWindow.Content).ActualWidth);
           myDXF.border.ZoomAuto(5000, 5000, actualHeight, actualWight);
            
            
            //DrawUtils.DrawPoint(100,0,myDXF.mainCanvas,Colors.Red,25,1);
            //DrawUtils.DrawPoint(-225,0,myDXF.mainCanvas,Colors.Red,25,1);
            //DrawUtils.SaveAsPng(DrawUtils.GetImage(myDXF.mainCanvas));

        }

        private void DrawDXFInsert(string fileDXF)
        {

            TypeConverter.defaultThickness = 0.01;
            DrawEntities.RazMaxDim();
            /*netDxf.Entities.Line ligneTmp = new netDxf.Entities.Line();
			
			ligneTmp.StartPoint = new Vector3(0,0,0);
			ligneTmp.EndPoint = new Vector3(100,100,0);*/

            myDXF.DxfDoc = new DxfDocument();
            //myDXF.DxfDoc.AddEntity(ligneTmp);

            if (fileDXF == "") fileDXF = "raptor.dxf";
            netDxf.Blocks.Block myBlock = netDxf.Blocks.Block.Load(fileDXF);
            netDxf.Entities.Insert myInsert = new netDxf.Entities.Insert(myBlock);

            myDXF.DxfDoc.AddEntity(myInsert);

            netDxf.Entities.Insert myInsert2 = new netDxf.Entities.Insert(myBlock);
            //myInsert2.Rotation = 180;
            Vector3 scaleInsert = new Vector3(1, -1, 1);
            myInsert2.Scale = scaleInsert;
            Vector3 pos = new Vector3(myInsert2.Position.X + 5, myInsert2.Position.Y, 0);
            myInsert2.Position = pos;
            myDXF.DxfDoc.AddEntity(myInsert2);



            this.Content = myDXF.GetMainGrid(myDXF.DxfDoc, true, true);

        }

        private void DrawDXF()
        {

            DrawDXF("");
        }
        public void UpdateUI( )
        {
            if(PathFiles!="")
            DrawDXF(PathFiles);
        }

    }
}
