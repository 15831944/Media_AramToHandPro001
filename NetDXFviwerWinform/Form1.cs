using netDxf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace NetDXFviwerWinform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            myDXF.GridHeight = 5000;
            myDXF.GridWidth = 5000;
            myDXF.ViewHeight = grid1.ActualHeight;
            myDXF.ViewWidth = grid1.ActualWidth;
            myDXF.WinHeight = this.Height;
            myDXF.WinWidth = this.Width;

            myDXF.border.Reset(myDXF.GridHeight, myDXF.GridWidth, true, this.Height, this.Width, this.Height, this.Width);
            DrawDXF();
        }

        private void btOpenDxf_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                String fileDXF = openFileDialog.FileName;
                DrawDXF(fileDXF);
            }

        }
        public DXF2WPF myDXF = new DXF2WPF();
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

            if (fileDXF == "") fileDXF = @"test8.dxf";
            if (fileDXF != "")
            {
                this.Content = myDXF.GetMainGrid(fileDXF, true, true);
                //DrawUtils.SaveAsPng(DrawUtils.GetImage(myDXF.mainCanvas));

            }
            else
            {
                this.Content = myDXF.GetMainGrid(true, true);
            }

            //myDXF.border.ZoomAuto(5000,5000,((Grid)Application.Current.MainWindow.Content).ActualHeight,((Grid)Application.Current.MainWindow.Content).ActualWidth);
            myDXF.border.ZoomAuto(5000, 5000, mainWin.myDXF.WinHeight, mainWin.myDXF.WinWidth);
           // DrawUtils.DrawPoint(100,0,myDXF.mainCanvas,Colors.Red,25,1);
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

        private void DrawDXF2()
        {
           // canvas1.Children.Clear();
            /*this.Content = DXF2WPF.GetMainGrid("sample2.dxf");*/
            //this.Content = myDXF.GetMainGrid("sample2.dxf",true,false);
            netDxf.Entities.Line ligneTmp = new netDxf.Entities.Line();

            ligneTmp.StartPoint = new Vector3(0, 0, 0);
            ligneTmp.EndPoint = new Vector3(100, 100, 0);
            /*ligneTmp.Thickness=20;
			ligneTmp.Lineweight = (Lineweight)15;
			ligneTmp.Color = new AciColor(8);*/
            myDXF.DxfDoc = new DxfDocument();
            myDXF.DxfDoc.AddEntity(ligneTmp);

            Grid mainGrid = new Grid();
            Canvas newMainCanvas = new Canvas();
            DXF2WPF.GetCanvas(myDXF.DxfDoc, myDXF.mainCanvas);
            myDXF.mainCanvas.Background = new SolidColorBrush(Colors.Blue);
            mainGrid.Children.Add(myDXF.mainCanvas);
            this.Content = mainGrid;
            //this.Content = myDXF.GetMainGrid("panther.dxf");
            /*CanvasCreator.GetCanvas(DxfDoc,canvas1);
			DrawUtils.DrawOrigin(canvas1);*/

           
        }


    }
}
