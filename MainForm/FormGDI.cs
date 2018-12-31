using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainForm
{
    public partial class FormGDI : Form
    {
        public FormGDI()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
         //   pictureBox1.Invalidate();
          
            getImageFromDxf("", pictureBox1.Height, pictureBox1.Width, pictureBox1);
         // pictureBox1.Update();


          //  Graphics g = pictureBox1.CreateGraphics();
          //  Pen pen = new Pen(Color.Red, 0.5f);

           // g.DrawLine(pen, 0, 0, 100, 100); // 画线

          //  pictureBox1.Update();

        }

        public System.Drawing.Image getImageFromDxf(string dxfPaths, int height = 200, int width = 300, PictureBox picturebox1 = null)
        {
            //Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            //System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), 
            //   System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            //Graphics g = Graphics.FromImage(你的Image对象);
            System.Drawing.Image IMAG = System.Drawing.Image.FromHbitmap(bmp.GetHbitmap());
            Graphics g = picturebox1.CreateGraphics();
            Pen pen = new Pen(Color.Red, 0.5f);
            g.DrawLine(pen,0, 30, 100, 100); // 画线

            //foreach (var item in DXFglobarVarlis.entityObjects)
            //{
            //    switch (item.Type)
            //    {
            //        case EntityType.Line:
            //            g.DrawLine(pen, (float)(((Line)item).StartPoint.X), (float)(((Line)item).StartPoint.Y),

            //                (float)(((Line)item).EndPoint.X), (float)(((Line)item).EndPoint.Y));
            //            break;

            //    }

            //}
            // bmp.UnlockBits(bmpData);
            //Image.FromHbitmap(bmp.GetHbitmap()); ;
            //return System.Drawing.Image.FromHbitmap(bmp.GetHbitmap());
            //  picturebox1.Refresh();
            pictureBox1.Update();
            return IMAG;
            // throw new NotImplementedException();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Myprintpage1(g,pictureBox1.Width, pictureBox1.Height);

        }

        private void Myprintpage1(Graphics formGraphics, int w, int h)
        {

            Pen myPen = new Pen(Color.FromArgb(255, Color.Black), 1.0F);
            Font MyFont1 = new Font("宋体", 12, FontStyle.Bold);
            Font MyFont2 = new Font("宋体", 10, FontStyle.Bold);

             formGraphics.TranslateTransform(0.0F, 0.0F);
            //画表格横线
            int stepheight = h / 8;
            int stepWidht = (w ) / 7;
            for (int i = 0; i <8; i++)
            {
                formGraphics.DrawLine(myPen, new Point(0,i* stepheight), new Point(7 * stepWidht, i* stepheight));
                
            }

            formGraphics.DrawLine(myPen, new Point(0, 0), new Point(0, 7*stepheight));
            formGraphics.DrawLine(myPen, new Point(7 * stepWidht, 0), new Point(7*stepWidht, 7* stepheight));


           
            for (int i = 0; i <8; i +=1)
            {
                formGraphics.DrawLine(myPen, new Point(i* stepWidht, stepheight), new Point(i* stepWidht, 3* stepheight));
            }

            formGraphics.DrawLine(myPen, new Point(stepWidht, 4*stepheight), new Point( stepWidht, 7 * stepheight));
            formGraphics.DrawLine(myPen, new Point(2*stepWidht,  4 * stepheight), new Point(2 * stepWidht, 7 * stepheight));
            formGraphics.DrawLine(myPen, new Point(3* stepWidht, 4 * stepheight), new Point(3 * stepWidht, 7 * stepheight));

          
            formGraphics.DrawLine(myPen, new Point(5 * stepWidht, 4 * stepheight), new Point(5 * stepWidht, 7 * stepheight));
            formGraphics.DrawLine(myPen, new Point(6 * stepWidht, 4 * stepheight), new Point(6 * stepWidht , 7 * stepheight));


            formGraphics.DrawString("构建总信息表", new Font("宋体", 20, FontStyle.Bold), Brushes.DimGray, (5*stepWidht)/2, stepheight / 2-10);
            for (int i = 0; i < 7; i++)
            {
                formGraphics.DrawString(global.titleName[i], MyFont1, Brushes.DimGray, i* stepWidht+10, stepheight*3/2);
            }
            for (int i = 0; i < 7; i++)
            {
                formGraphics.DrawString(global.titleNameValue[i], MyFont2, Brushes.DimGray, i * stepWidht + 10, stepheight * 5 / 2);
            }


            //预埋清单信息表
            formGraphics.DrawString("预埋清单信息表", new Font("宋体", 20, FontStyle.Bold), Brushes.DimGray, (5 * stepWidht) / 2, stepheight*7 / 2 - 10);
            for (int i = 0; i < 3; i++)
            {
                formGraphics.DrawString(global.titleName2[i], MyFont1, Brushes.DimGray, (i) * stepWidht + 10, 4 * stepheight+stepheight/2);
            }
            formGraphics.DrawString(global.titleName2[3], MyFont1, Brushes.DimGray, (7) * stepWidht/2 , 4 * stepheight + stepheight / 2);
            for (int i = 4; i < 6; i++)
            {
                formGraphics.DrawString(global.titleName2[i], MyFont1, Brushes.DimGray, (i + 1) * stepWidht + 10, 4 * stepheight + stepheight / 2);
            }
         
        }

        public class global
        {
            public static string[] titleName = new string[7]
            {"构建编号","构建重量","混凝土体积","混凝土等级","钢筋重量","钢筋保护层厚度","单栋构件数量"};
            public static string[] titleNameValue = new string[7]
            {"TC01","2.61","1.04M³","C35","95.43 KG","20 mm","1"};
            public static string[] titleName2 = new string[6]
            {"序号","编号","名称","规格型号","图例","数量"};
            public static string[] temstr = new string[5];
        }
    }
}
