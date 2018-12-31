using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;
using netDxf;

namespace MainForm
{
    public struct xyzPoint
    {
        public double X, Y, Z, A;
        public xyzPoint(double x, double y, double z, double a = 0)
        { X = x; Y = y; Z = z; A = a; }
        // Overload + operator 
        public static xyzPoint operator +(xyzPoint b, xyzPoint c)
        {
            xyzPoint a = new xyzPoint();
            a.X = b.X + c.X;
            a.Y = b.Y + c.Y;
            a.Z = b.Z + c.Z;
            a.A = b.A + c.A;
            return a;
        }
        public static xyzPoint operator -(xyzPoint b, xyzPoint c)
        {
            xyzPoint a = new xyzPoint();
            a.X = b.X - c.X;
            a.Y = b.Y - c.Y;
            a.Z = b.Z - c.Z;
            a.A = b.A - c.A;
            return a;
        }
        public string Print()
        {
            bool ctrl4thUse = Properties.Settings.Default.ctrl4thUse;
            string ctrl4thName = Properties.Settings.Default.ctrl4thName;

            if (ctrl4thUse)
                return string.Format("X={0:0.000} Y={1:0.000} Z={2:0.000} {3}={4:0.000}", X, Y, Z, ctrl4thName, A);
            else
                return string.Format("X={0:0.000} Y={1:0.000} Z={2:0.000}", X, Y, Z);
        }

    };
    public struct xyPoint
    {
        public double X, Y;
        public xyPoint(double x, double y)
        { X = x; Y = y; }
        public xyPoint(xyPoint tmp)
        { X = tmp.X; Y = tmp.Y; }
        public xyPoint(Point tmp)
        { X = tmp.X; Y = tmp.Y; }
        public xyPoint(xyzPoint tmp)
        { X = tmp.X; Y = tmp.Y; }
        public static explicit operator xyPoint(Point tmp)
        { return new xyPoint(tmp); }
        public static explicit operator xyPoint(xyzPoint tmp)
        { return new xyPoint(tmp); }

        public Point ToPoint()
        { return new Point((int)X, (int)Y); }

        public double DistanceTo(xyPoint anotherPoint)
        {
            double distanceCodeX = X - anotherPoint.X;
            double distanceCodeY = Y - anotherPoint.Y;
            return Math.Sqrt(distanceCodeX * distanceCodeX + distanceCodeY * distanceCodeY);
        }
        public double AngleTo(xyPoint anotherPoint)
        {
            double distanceX = anotherPoint.X - X;
            double distanceY = anotherPoint.Y - Y;
            double radius = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            if (radius == 0) { return 0; }
            double cosinus = distanceX / radius;
            if (cosinus > 1) { cosinus = 1; }
            if (cosinus < -1) { cosinus = -1; }
            double angle = 180 * (float)(Math.Acos(cosinus) / Math.PI);
            if (distanceY > 0) { angle = -angle; }
            return angle;
        }

        // Overload + operator 
        public static xyPoint operator +(xyPoint b, xyPoint c)
        {
            xyPoint a = new xyPoint();
            a.X = b.X + c.X;
            a.Y = b.Y + c.Y;
            return a;
        }
        // Overload / operator 
        public static xyPoint operator /(xyPoint b, double c)
        {
            xyPoint a = new xyPoint();
            a.X = b.X / c;
            a.Y = b.Y / c;
            return a;
        }
    };


    /*   class eventArgsTemplates    // just copy and paste 
       {
           public event EventHandler<XYZEventArgs> RaiseXYZEvent;
           protected virtual void OnRaiseXYZEvent(XYZEventArgs e)
           {
               EventHandler<XYZEventArgs> handler = RaiseXYZEvent;
               if (handler != null)
               {
                   handler(this, e);
               }
           }
       }
       */

    public class XYEventArgs : EventArgs
    {
        private double angle, scale;
        private xyPoint point;
        string command;
        public XYEventArgs(double a, double s, xyPoint p, string cmd)
        {
            angle = a;
            scale = s;
            point = p;
            command = cmd;
        }
        public XYEventArgs(double a, double x, double y, string cmd)
        {
            angle = a;
            point.X = x;
            point.Y = y;
            command = cmd;
        }
        public double Angle
        { get { return angle; } }
        public double Scale
        { get { return scale; } }
        public xyPoint Point
        { get { return point; } }
        public double PosX
        { get { return point.X; } }
        public double PosY
        { get { return point.Y; } }
        public string Command
        { get { return command; } }
    }

    public class XYZEventArgs : EventArgs
    {
        private double? posX, posY, posZ;
        string command;
        public XYZEventArgs(double? x, double? y, string cmd)
        {
            posX = x;
            posY = y;
            posZ = null;
            command = cmd;
        }
        public XYZEventArgs(double? x, double? y, double? z, string cmd)
        {
            posX = x;
            posY = y;
            posZ = z;
            command = cmd;
        }
        public double? PosX
        { get { return posX; } }
        public double? PosY
        { get { return posY; } }
        public double? PosZ
        { get { return posZ; } }
        public string Command
        { get { return command; } }
    }



    public class HeightMap
    {
        public double?[,] Points { get; private set; }
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        public int TotalPoints { get { return SizeX * SizeY; } }

        public Queue<Tuple<int, int>> NotProbed { get; private set; } = new Queue<Tuple<int, int>>();

        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        public Vector2 Delta { get { return Max - Min; } }

        public double MinHeight { get; set; } = double.MaxValue;
        public double MaxHeight { get; set; } = double.MinValue;

        //        public event Action MapUpdated;

        public double GridX { get { return (Max.X - Min.X) / (SizeX - 1); } }
        public double GridY { get { return (Max.Y - Min.Y) / (SizeY - 1); } }


        public HeightMap(double gridSize, Vector2 min, Vector2 max)
        {
            MinHeight = double.MaxValue;
            MaxHeight = double.MinValue;

            if (min.X == max.X || min.Y == max.Y)
                throw new Exception("Height map can't be infinitely narrow");

            int pointsX = (int)Math.Ceiling((max.X - min.X) / gridSize) + 1;
            int pointsY = (int)Math.Ceiling((max.Y - min.Y) / gridSize) + 1;

            if (pointsX == 0 || pointsY == 0)
                throw new Exception("Height map must have at least 4 points");

            Points = new double?[pointsX, pointsY];

            if (max.X < min.X)
            {
                double a = min.X;
                min.X = max.X;
                max.X = a;
            }

            if (max.Y < min.Y)
            {
                double a = min.Y;
                min.Y = max.Y;
                max.Y = a;
            }

            Min = min;
            Max = max;

            SizeX = pointsX;
            SizeY = pointsY;

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                    NotProbed.Enqueue(new Tuple<int, int>(x, y));

                if (++x >= SizeX)
                    break;

                for (int y = SizeY - 1; y >= 0; y--)
                    NotProbed.Enqueue(new Tuple<int, int>(x, y));
            }
        }

        public double InterpolateZ(double x, double y)
        {
            if (x > Max.X || x < Min.X || y > Max.Y || y < Min.Y)
                return MaxHeight;

            x -= Min.X;
            y -= Min.Y;

            x /= GridX;
            y /= GridY;

            int iLX = (int)Math.Floor(x);   //lower integer part
            int iLY = (int)Math.Floor(y);

            int iHX = (int)Math.Ceiling(x); //upper integer part
            int iHY = (int)Math.Ceiling(y);

            //     try
            //     {
            double fX = x - iLX;             //fractional part
            double fY = y - iLY;

            double linUpper = Points[iHX, iHY].Value * fX + Points[iLX, iHY].Value * (1 - fX);       //linear immediates
            double linLower = Points[iHX, iLY].Value * fX + Points[iLX, iLY].Value * (1 - fX);

            return linUpper * fY + linLower * (1 - fY);     //bilinear result
                                                            //   } catch { return MaxHeight; }
        }

        public Vector2 GetCoordinates(int x, int y, bool applyOffset = true)
        {
            if (applyOffset)
                return new Vector2(x * (Delta.X / (SizeX - 1)) + Min.X, y * (Delta.Y / (SizeY - 1)) + Min.Y);
            else
                return new Vector2(x * (Delta.X / (SizeX - 1)), y * (Delta.Y / (SizeY - 1)));
        }

        private HeightMap()
        { }

        public void AddPoint(int x, int y, double height)
        {
            Points[x, y] = height;

            if (height > MaxHeight)
                MaxHeight = height;
            if (height < MinHeight)
                MinHeight = height;
        }
        public double? GetPoint(int x, int y)
        {
            return Points[x, y];
        }
        public void setZOffset(double offset)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = Points[ix, iy] + offset;
                }
            }
            MaxHeight = MaxHeight + offset;
            MinHeight = MinHeight + offset;
        }
        public void setZZoom(double zoom)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = Points[ix, iy] * zoom;
                }
            }
            MaxHeight = MaxHeight * zoom;
            MinHeight = MinHeight * zoom;
        }
        public void setZInvert()
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    Points[ix, iy] = -Points[ix, iy];
                }
            }
            double tmp = MaxHeight;
            MaxHeight = -MinHeight;
            MinHeight = -tmp;
        }
        public void setZCutOff(double limit)
        {
            for (int iy = 0; iy < SizeY; iy++)
            {
                for (int ix = 0; ix < SizeX; ix++)
                {
                    if (Points[ix, iy] < limit)
                        Points[ix, iy] = limit;
                }
            }
            MinHeight = limit;
        }

        public static HeightMap Load(string path)
        {
            HeightMap map = new HeightMap();

            XmlReader r = XmlReader.Create(path);
            map.MaxHeight = double.MinValue;
            map.MinHeight = double.MaxValue;

            while (r.Read())
            {
                if (!r.IsStartElement())
                    continue;

                switch (r.Name)
                {
                    case "heightmap":
                        map.Min = new Vector2(double.Parse(r["MinX"].Replace(',', '.'), NumberFormatInfo.InvariantInfo), double.Parse(r["MinY"].Replace(',', '.'), NumberFormatInfo.InvariantInfo));
                        map.Max = new Vector2(double.Parse(r["MaxX"].Replace(',', '.'), NumberFormatInfo.InvariantInfo), double.Parse(r["MaxY"].Replace(',', '.'), NumberFormatInfo.InvariantInfo));
                        map.SizeX = int.Parse(r["SizeX"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        map.SizeY = int.Parse(r["SizeY"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        map.Points = new double?[map.SizeX, map.SizeY];
                        break;
                    case "point":
                        int x = int.Parse(r["X"].Replace(',', '.')), y = int.Parse(r["Y"].Replace(',', '.'), NumberFormatInfo.InvariantInfo);
                        double height = double.Parse(r.ReadInnerXml().Replace(',', '.'), NumberFormatInfo.InvariantInfo);

                        map.Points[x, y] = height;

                        if (height > map.MaxHeight)
                            map.MaxHeight = height;
                        if (height < map.MinHeight)
                            map.MinHeight = height;

                        break;
                }
            }

            //       r.Dispose();

            for (int x = 0; x < map.SizeX; x++)
            {
                for (int y = 0; y < map.SizeY; y++)
                    if (!map.Points[x, y].HasValue)
                        map.NotProbed.Enqueue(new Tuple<int, int>(x, y));

                if (++x >= map.SizeX)
                    break;

                for (int y = map.SizeY - 1; y >= 0; y--)
                    if (!map.Points[x, y].HasValue)
                        map.NotProbed.Enqueue(new Tuple<int, int>(x, y));
            }

            return map;
        }

        // vertex coordinates must be positive-definite (nonnegative and nonzero) numbers. 
        // The StL file does not contain any scale information; the coordinates are in arbitrary units.
        public void SaveSTL(string path)
        {
            StringBuilder data = new StringBuilder();
            data.AppendLine("solid ASCII_STL_GRBL_Plotter");
            double z0, z1, z2, z3;
            Vector2 p0, p1, p2, p3;
            for (int y = 0; y < (SizeY - 1); y++)
            {
                for (int x = 0; x < (SizeX - 1); x++)
                {
                    if (!Points[x, y].HasValue)
                        continue;
                    p0 = GetCoordinates(x, y, false);       // vertex coordinates must be positive-definite (nonnegative and nonzero) numbers. 
                    p1 = GetCoordinates(x, y + 1, false);
                    p2 = GetCoordinates(x + 1, y, false);
                    p3 = GetCoordinates(x + 1, y + 1, false);
                    z0 = -1 * Points[x, y].Value;    // vertex coordinates must be positive-definite (nonnegative and nonzero) numbers. 
                    z1 = -1 * Points[x, y + 1].Value;
                    z2 = -1 * Points[x + 1, y].Value;
                    z3 = -1 * Points[x + 1, y + 1].Value;

                    data.AppendLine(" facet normal 0 0 0");
                    data.AppendLine("  outer loop");
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p0.X, p0.Y, z0);
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p1.X, p1.Y, z1);
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p2.X, p2.Y, z2);
                    data.AppendLine("  endloop");
                    data.AppendLine(" endfacet");

                    data.AppendLine(" facet normal 0 0 0");
                    data.AppendLine("  outer loop");
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p1.X, p1.Y, z1);
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p3.X, p3.Y, z3);
                    data.AppendFormat("   vertex {0} {1} {2:0.0000}\r\n", p2.X, p2.Y, z2);
                    data.AppendLine("  endloop");
                    data.AppendLine(" endfacet");
                }
            }
            data.AppendLine("endsolid ASCII_STL_GRBL_Plotter");
            File.WriteAllText(path, data.ToString().Replace(',', '.'));
        }

        public void SaveX3D(string path)
        {
            StringBuilder object_code = new StringBuilder();
            StringBuilder color_code = new StringBuilder();
            bool first_val = true;
            if (true)//elevation)
            {
                object_code.AppendLine(" <Transform DEF='elevationgrid' containerField='children' translation='0 0 0'>");
                object_code.AppendLine("  <Shape DEF='GRBL-Plotter Height Map' containerField='children'>");
                object_code.AppendFormat("    <ElevationGrid creaseAngle='3.14159' solid='false' xDimension='{0}' xSpacing='1' zDimension='{1}' zSpacing='1' height='", SizeX, SizeY);
                for (int y = (SizeY - 1); y >= 0; y--) //(int y = 0; y < SizeY; y++)
                {
                    for (int x = 0; x < SizeX; x++)
                    {
                        if (first_val) { first_val = false; }
                        else { object_code.Append(","); color_code.Append(","); }
                        if (x == 0) { object_code.Append("\r\n      "); color_code.Append("\r\n         "); }
                        object_code.Append(Points[x, y].Value.ToString());
                        color_code.Append(getColorString(Points[x, y].Value));
                    }
                }
                object_code.Append("'>\r\n");
                object_code.AppendFormat("        <Color color='{0}'/>\r\n", color_code);
                object_code.Append("     </ElevationGrid>\r\n");
                object_code.Append("  </Shape>\r\n");
                object_code.Append(" </Transform>\r\n");
            }
            string file_head = "", file_foot = "";
            file_head += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n";
            file_head += "<!DOCTYPE X3D PUBLIC \"ISO//Web3D//DTD X3D 3.0//EN\" \"http://www.web3d.org/specifications/x3d-3.0.dtd\">\r\n";
            file_head += "<X3D profile='Immersive' >\r\n";
            file_head += "<head></head>\r\n";
            file_head += "<Scene>\r\n";

            file_head += "<NavigationInfo containerField='children' avatarSize='.25 1.6 .75' visibilityLimit='0' speed='10' headlight='true' type='\"EXAMINE\" \"ANY\"'/>\r\n";
            file_head += "<Background containerField='children' skyAngle=' .7854 1.91986' skyColor='  0 .2 .70196 0 .50196 1 1 1 1' groundAngle='1.5708' groundColor='  .2 .2 .2 .8 .8 .8'/>\r\n";

            file_head += "<Transform DEF='dad_Group_light' rotation='-.286 -.914 -.286 1.66'>\r\n";
            file_head += "  <Transform DEF='light1_t' containerField='children' translation='" + SizeY / 2 + " 0 " + 3 * SizeX + "' scale='2 2 2'>\r\n";
            file_head += "    <SpotLight DEF='light1' containerField='children' ambientIntensity='0.000' intensity='1.000' radius='100.000' cutOffAngle='1.309' beamWidth='0.785' attenuation='1 0 0' color='1 1 1' on='true'/>\r\n";
            file_head += "  </Transform>\r\n";
            file_head += "</Transform>\r\n";

            // camera static
            var camera_static_distance = SizeX * 2;
            var camera_static_angle = 45 * Math.PI / 180;
            file_head += "<Transform DEF='dad_Group_static_camera' translation='" + SizeX / 2 + " 0 " + SizeY / 2 + "' rotation='-1 0 0 " + camera_static_angle + "'>\r\n";
            file_head += " <Viewpoint DEF='Viewpoint_static_camera' containerField='children' description='Static camera' jump='true' fieldOfView='0.785' position='0 0 " + camera_static_distance + "' orientation='0 0 1 0'/>\r\n";
            file_head += "</Transform>\r\n";

            //            file_head += navi + back + light + camera + plate + text + legend;
            file_foot += "</Scene>\r\n</X3D>\r\n";
            string file_data = file_head + object_code.ToString() + file_foot;
            File.WriteAllText(path, file_data.Replace(',', '.'));
        }

        public void Save(string path)
        {
            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;
            XmlWriter w = XmlWriter.Create(path, set);
            w.WriteStartDocument();
            w.WriteStartElement("heightmap");
            w.WriteAttributeString("MinX", Min.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("MinY", Min.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("MaxX", Max.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("MaxY", Max.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("SizeX", SizeX.ToString().Replace(',', '.'));
            w.WriteAttributeString("SizeY", SizeY.ToString().Replace(',', '.'));

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    if (!Points[x, y].HasValue)
                        continue;

                    w.WriteStartElement("point");
                    w.WriteAttributeString("X", x.ToString().Replace(',', '.'));
                    w.WriteAttributeString("Y", y.ToString().Replace(',', '.'));
                    w.WriteString(Points[x, y].Value.ToString().Replace(',', '.'));
                    w.WriteEndElement();
                }
            }
            w.WriteEndElement();
            w.Close();
        }

        public static Color getColor(double min, double max, double value, bool gray)
        {
            int R = 0, G = 0, B = 0;
            if (gray)
            {
                int valC = (int)(255 * (value - min) / (max - min));
                if (valC < 0) valC = 0;
                if (valC > 255) valC = 255;
                R = G = B = valC;
            }
            else
            {
                int segments = 3;
                int valC = (int)(255 * segments * (value - min) / (max - min));
                if (valC < 0) valC = 0;
                if (valC > 255 * segments) valC = 255 * segments;

                if ((valC >= 0) && (valC < 256 * 1))
                { R = 0; G = valC; B = 255 - valC; }

                else if ((valC >= 256) && (valC < 256 * 2))
                { R = valC - (256 * 1); G = 255; B = 0; }

                else if ((valC >= 256 * 2) && (valC < 256 * 3))
                { R = 255; G = (256 * 3 - 1) - valC; B = 0; }
            }
            return Color.FromArgb(R, G, B);
        }
        public String getColorString(double value)
        {
            Color tmp = getColor(MinHeight, MaxHeight, value, false);
            return string.Format("{0:0.00} {1:0.00} {2:0.00}", (double)tmp.R / 255, (double)tmp.G / 255, (double)tmp.B / 255);
        }

        public void FillWithTestPattern(string pattern)
        {
            DataTable t = new DataTable();

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    double X = (x * (Max.X - Min.X)) / (SizeX - 1) + Min.X;
                    double Y = (y * (Max.Y - Min.Y)) / (SizeY - 1) + Min.Y;

                    decimal d = (decimal)t.Compute(pattern.Replace("x", X.ToString()).Replace("y", Y.ToString()), "");
                    AddPoint(x, y, (double)d);
                }
            }
        }
    }

    //public struct Vector22222 : IEquatable<Vector2>
    //{

    //    private double x;

    //    private double y;

    //    public Vector2(double x, double y)
    //    {
    //        // Pre-initialisation initialisation
    //        // Implemented because a struct's variables always have to be set in the constructor before moving control
    //        this.x = 0;
    //        this.y = 0;

    //        // Initialisation
    //        X = x;
    //        Y = y;
    //    }

    //    public double X
    //    {
    //        get { return x; }
    //        set { x = value; }
    //    }

    //    public double Y
    //    {
    //        get { return y; }
    //        set { y = value; }
    //    }

    //    public static Vector2 operator +(Vector2 v1, Vector2 v2)
    //    {
    //        return
    //        (
    //            new Vector2
    //                (
    //                    v1.X + v2.X,
    //                    v1.Y + v2.Y
    //                )
    //        );
    //    }

    //    public static Vector2 operator -(Vector2 v1, Vector2 v2)
    //    {
    //        return
    //        (
    //            new Vector2
    //                (
    //                    v1.X - v2.X,
    //                    v1.Y - v2.Y
    //                )
    //        );
    //    }

    //    public static Vector2 operator *(Vector2 v1, double s2)
    //    {
    //        return
    //        (
    //            new Vector2
    //            (
    //                v1.X * s2,
    //                v1.Y * s2
    //            )
    //        );
    //    }

    //    public static Vector2 operator *(double s1, Vector2 v2)
    //    {
    //        return v2 * s1;
    //    }

    //    public static Vector2 operator /(Vector2 v1, double s2)
    //    {
    //        return
    //        (
    //            new Vector2
    //                (
    //                    v1.X / s2,
    //                    v1.Y / s2
    //                )
    //        );
    //    }

    //    public static Vector2 operator -(Vector2 v1)
    //    {
    //        return
    //        (
    //            new Vector2
    //                (
    //                    -v1.X,
    //                    -v1.Y
    //                )
    //        );
    //    }

    //    public static bool operator ==(Vector2 v1, Vector2 v2)
    //    {
    //        return
    //        (
    //            Math.Abs(v1.X - v2.X) <= EqualityTolerence &&
    //            Math.Abs(v1.Y - v2.Y) <= EqualityTolerence
    //        );
    //    }

    //    public static bool operator !=(Vector2 v1, Vector2 v2)
    //    {
    //        return !(v1 == v2);
    //    }

    //    public bool Equals(Vector2 other)
    //    {
    //        return other == this;
    //    }

    //    public override bool Equals(object other)
    //    {
    //        // Check object other is a Vector3 object
    //        if (other is Vector2)
    //        {
    //            // Convert object to Vector3
    //            Vector2 otherVector = (Vector2)other;

    //            // Check for equality
    //            return otherVector == this;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    public override int GetHashCode()
    //    {
    //        return
    //        (
    //            (int)((X + Y) % Int32.MaxValue)
    //        );
    //    }

    //    public const double EqualityTolerence = double.Epsilon;
    //}


    public struct Vector2 :
       IEquatable<Vector2>
    {
        #region private fields

        private double x;
        private double y;
        private bool isNormalized;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of Vector3.
        /// </summary>
        /// <param name="value">X, Y component.</param>
        public Vector2(double value)
        {
            this.x = value;
            this.y = value;
            this.isNormalized = false;
        }

        /// <summary>
        /// Initializes a new instance of Vector2.
        /// </summary>
        /// <param name="x">X component.</param>
        /// <param name="y">Y component.</param>
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
            this.isNormalized = false;
        }

        /// <summary>
        /// Initializes a new instance of Vector2.
        /// </summary>
        /// <param name="array">Array of two elements that represents the vector.</param>
        public Vector2(double[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (array.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(array), array.Length, "The dimension of the array must be two");
            this.x = array[0];
            this.y = array[1];
            this.isNormalized = false;
        }

        #endregion

        #region constants

        /// <summary>
        /// Zero vector.
        /// </summary>
        public static Vector2 Zero
        {
            get { return new Vector2(0, 0); }
        }

        /// <summary>
        /// Unit X vector.
        /// </summary>
        public static Vector2 UnitX
        {
            get { return new Vector2(1, 0) { isNormalized = true }; }
        }

        /// <summary>
        /// Unit Y vector.
        /// </summary>
        public static Vector2 UnitY
        {
            get { return new Vector2(0, 1) { isNormalized = true }; }
        }

        /// <summary>
        /// Represents a vector with not a number components.
        /// </summary>
        public static Vector2 NaN
        {
            get { return new Vector2(double.NaN, double.NaN); }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the X component.
        /// </summary>
        public double X
        {
            get { return this.x; }
            set
            {
                this.isNormalized = false;
                this.x = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y component.
        /// </summary>
        public double Y
        {
            get { return this.y; }
            set
            {
                this.isNormalized = false;
                this.y = value;
            }
        }

        /// <summary>
        /// Gets or sets a vector element defined by its index.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
            set
            {
                this.isNormalized = false;
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        /// <summary>
        /// Gets if the vector has been normalized.
        /// </summary>
        public bool IsNormalized
        {
            get { return this.isNormalized; }
        }

        #endregion

        #region static methods

        /// <summary>
        /// Returns a value indicating if any component of the specified vector evaluates to a value that is not a number <see cref="System.Double.NaN"/>.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>Returns true if any component of the specified vector evaluates to <see cref="System.Double.NaN"/>; otherwise, false.</returns>
        public static bool IsNaN(Vector2 u)
        {
            return double.IsNaN(u.X) || double.IsNaN(u.Y);
        }

        /// <summary>
        /// Obtains the dot product of two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The dot product.</returns>
        public static double DotProduct(Vector2 u, Vector2 v)
        {
            return u.X * v.X + u.Y * v.Y;
        }

        /// <summary>
        /// Obtains the cross product of two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Vector2.</returns>
        public static double CrossProduct(Vector2 u, Vector2 v)
        {
            return u.X * v.Y - u.Y * v.X;
        }

        /// <summary>
        /// Obtains the counter clockwise perpendicular vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 Perpendicular(Vector2 u)
        {
            return new Vector2(-u.Y, u.X) { isNormalized = u.IsNormalized };
        }

        /// <summary>
        /// Rotates a vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="angle">Rotation angles in radians.</param>
        /// <returns></returns>
        public static Vector2 Rotate(Vector2 u, double angle)
        {
            if (MathHelper.IsZero(angle))
                return u;
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            return new Vector2(u.X * cos - u.Y * sin, u.X * sin + u.Y * cos) { isNormalized = u.IsNormalized };
        }

        /// <summary>
        /// Obtains a the polar point of another point. 
        /// </summary>
        /// <param name="u">Reference point.</param>
        /// <param name="distance">Distance from point u.</param>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>The polar point of the specified point.</returns>
        public static Vector2 Polar(Vector2 u, double distance, double angle)
        {
            Vector2 dir = new Vector2(Math.Cos(angle), Math.Sin(angle));
            return u + dir * distance;
        }

        /// <summary>
        /// Obtains the distance between two points.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Distance.</returns>
        public static double Distance(Vector2 u, Vector2 v)
        {
            return Math.Sqrt((u.X - v.X) * (u.X - v.X) + (u.Y - v.Y) * (u.Y - v.Y));
        }

        /// <summary>
        /// Obtains the square distance between two points.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Square distance.</returns>
        public static double SquareDistance(Vector2 u, Vector2 v)
        {
            return (u.X - v.X) * (u.X - v.X) + (u.Y - v.Y) * (u.Y - v.Y);
        }

        /// <summary>
        /// Obtains the angle of a vector.
        /// </summary>
        /// <param name="u">A Vector2.</param>
        /// <returns>Angle in radians.</returns>
        public static double Angle(Vector2 u)
        {
            double angle = Math.Atan2(u.Y, u.X);
            if (angle < 0)
                return MathHelper.TwoPI + angle;
            return angle;
        }

        /// <summary>
        /// Obtains the angle of a line defined by two points.
        /// </summary>
        /// <param name="u">A Vector2.</param>
        /// <param name="v">A Vector2.</param>
        /// <returns>Angle in radians.</returns>
        public static double Angle(Vector2 u, Vector2 v)
        {
            Vector2 dir = v - u;
            return Angle(dir);
        }

        /// <summary>
        /// Obtains the angle between two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Angle in radians.</returns>
        public static double AngleBetween(Vector2 u, Vector2 v)
        {
            double cos = DotProduct(u, v) / (u.Modulus() * v.Modulus());
            if (cos >= 1.0)
                return 0.0;
            if (cos <= -1.0)
                return Math.PI;

            return Math.Acos(cos);
        }

        /// <summary>
        /// Obtains the midpoint.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 MidPoint(Vector2 u, Vector2 v)
        {
            return new Vector2((v.X + u.X) * 0.5, (v.Y + u.Y) * 0.5);
        }

        /// <summary>
        /// Checks if two vectors are perpendicular.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>True if are perpendicular or false in any other case.</returns>
        public static bool ArePerpendicular(Vector2 u, Vector2 v)
        {
            return ArePerpendicular(u, v, MathHelper.Epsilon);
        }

        /// <summary>
        /// Checks if two vectors are perpendicular.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <param name="threshold">Tolerance used.</param>
        /// <returns>True if are perpendicular or false in any other case.</returns>
        public static bool ArePerpendicular(Vector2 u, Vector2 v, double threshold)
        {
            return MathHelper.IsZero(DotProduct(u, v), threshold);
        }

        /// <summary>
        /// Checks if two vectors are parallel.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>True if are parallel or false in any other case.</returns>
        public static bool AreParallel(Vector2 u, Vector2 v)
        {
            return AreParallel(u, v, MathHelper.Epsilon);
        }

        /// <summary>
        /// Checks if two vectors are parallel.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <param name="threshold">Tolerance used.</param>
        /// <returns>True if are parallel or false in any other case.</returns>
        public static bool AreParallel(Vector2 u, Vector2 v, double threshold)
        {
            return MathHelper.IsZero(CrossProduct(u, v), threshold);
        }

        /// <summary>
        /// Rounds the components of a vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="numDigits">Number of decimal places in the return value.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 Round(Vector2 u, int numDigits)
        {
            return new Vector2(Math.Round(u.X, numDigits), Math.Round(u.Y, numDigits));
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="u">Vector to normalize</param>
        /// <returns>A normalized vector.</returns>
        public static Vector2 Normalize(Vector2 u)
        {
            if (u.isNormalized) return u;

            double mod = u.Modulus();
            if (MathHelper.IsZero(mod))
                return NaN;
            double modInv = 1 / mod;
            return new Vector2(u.x * modInv, u.y * modInv) { isNormalized = true };
        }

        #endregion

        #region overloaded operators

        /// <summary>
        /// Check if the components of two vectors are equal.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>True if the two components are equal or false in any other case.</returns>
        public static bool operator ==(Vector2 u, Vector2 v)
        {
            return Equals(u, v);
        }

        /// <summary>
        /// Check if the components of two vectors are different.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>True if the two components are different or false in any other case.</returns>
        public static bool operator !=(Vector2 u, Vector2 v)
        {
            return !Equals(u, v);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The addition of u plus v.</returns>
        public static Vector2 operator +(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X + v.X, u.Y + v.Y);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The addition of u plus v.</returns>
        public static Vector2 Add(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X + v.X, u.Y + v.Y);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The subtraction of u minus v.</returns>
        public static Vector2 operator -(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X - v.X, u.Y - v.Y);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The subtraction of u minus v.</returns>
        public static Vector2 Subtract(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X - v.X, u.Y - v.Y);
        }

        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>The negative vector of u.</returns>
        public static Vector2 operator -(Vector2 u)
        {
            return new Vector2(-u.X, -u.Y) { isNormalized = u.IsNormalized };
        }

        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>The negative vector of u.</returns>
        public static Vector2 Negate(Vector2 u)
        {
            return new Vector2(-u.X, -u.Y) { isNormalized = u.IsNormalized };
        }

        /// <summary>
        /// Multiplies a vector with an scalar.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector2 operator *(Vector2 u, double a)
        {
            return new Vector2(u.X * a, u.Y * a);
        }

        /// <summary>
        /// Multiplies a vector with an scalar.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector2 Multiply(Vector2 u, double a)
        {
            return new Vector2(u.X * a, u.Y * a);
        }

        /// <summary>
        /// Multiplies a scalar with a vector.
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector3.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector2 operator *(double a, Vector2 u)
        {
            return new Vector2(u.X * a, u.Y * a);
        }

        /// <summary>
        /// Multiplies a scalar with a vector.
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector3.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector2 Multiply(double a, Vector2 u)
        {
            return new Vector2(u.X * a, u.Y * a);
        }

        /// <summary>
        /// Multiplies two vectors component by component.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector2 operator *(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X * v.X, u.Y * v.Y);
        }

        /// <summary>
        /// Multiplies two vectors component by component.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector2 Multiply(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X * v.X, u.Y * v.Y);
        }

        /// <summary>
        /// Divides a scalar with a vector.
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector2.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector2 operator /(double a, Vector2 u)
        {
            return new Vector2(a * u.X, a * u.Y);
        }

        /// <summary>
        /// Divides a scalar with a vector.
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector2.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector2 Divide(double a, Vector2 u)
        {
            return new Vector2(a * u.X, a * u.Y);
        }

        /// <summary>
        /// Divides a vector with an scalar.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector2 operator /(Vector2 u, double a)
        {
            double invEscalar = 1 / a;
            return new Vector2(u.X * invEscalar, u.Y * invEscalar);
        }

        /// <summary>
        /// Divides a vector with an scalar.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector2 Divide(Vector2 u, double a)
        {
            double invEscalar = 1 / a;
            return new Vector2(u.X * invEscalar, u.Y * invEscalar);
        }

        /// <summary>
        /// Divides two vectors component by component.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector2 operator /(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X / v.X, u.Y / v.Y);
        }

        /// <summary>
        /// Divides two vectors component by component.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector2 Divide(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X / v.X, u.Y / v.Y);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public void Normalize()
        {
            if (this.isNormalized) return;

            double mod = this.Modulus();
            if (MathHelper.IsZero(mod))
                this = NaN;
            else
            {
                double modInv = 1 / mod;
                this.x *= modInv;
                this.y *= modInv;
            }
            this.isNormalized = true;
        }

        /// <summary>
        /// Obtains the modulus of the vector.
        /// </summary>
        /// <returns>Vector modulus.</returns>
        public double Modulus()
        {
            return Math.Sqrt(DotProduct(this, this));
        }

        /// <summary>
        /// Returns an array that represents the vector.
        /// </summary>
        /// <returns>Array.</returns>
        public double[] ToArray()
        {
            return new[] { this.x, this.y };
        }

        #endregion

        #region comparison methods

        /// <summary>
        /// Check if the components of two vectors are approximate equal.
        /// </summary>
        /// <param name="a">Vector2.</param>
        /// <param name="b">Vector2.</param>
        /// <returns>True if the two components are almost equal or false in any other case.</returns>
        public static bool Equals(Vector2 a, Vector2 b)
        {
            return a.Equals(b, MathHelper.Epsilon);
        }

        /// <summary>
        /// Check if the components of two vectors are approximate equal.
        /// </summary>
        /// <param name="a">Vector2.</param>
        /// <param name="b">Vector2.</param>
        /// <param name="threshold">Maximum tolerance.</param>
        /// <returns>True if the two components are almost equal or false in any other case.</returns>
        public static bool Equals(Vector2 a, Vector2 b, double threshold)
        {
            return a.Equals(b, threshold);
        }

        /// <summary>
        /// Check if the components of two vectors are approximate equals.
        /// </summary>
        /// <param name="other">Another Vector2 to compare to.</param>
        /// <returns>True if the three components are almost equal or false in any other case.</returns>
        public bool Equals(Vector2 other)
        {
            return this.Equals(other, MathHelper.Epsilon);
        }

        /// <summary>
        /// Check if the components of two vectors are approximate equals.
        /// </summary>
        /// <param name="other">Another Vector2 to compare to.</param>
        /// <param name="threshold">Maximum tolerance.</param>
        /// <returns>True if the three components are almost equal or false in any other case.</returns>
        public bool Equals(Vector2 other, double threshold)
        {
            return MathHelper.IsEqual(other.X, this.x, threshold) && MathHelper.IsEqual(other.Y, this.y, threshold);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">Another object to compare to.</param>
        /// <returns>True if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is Vector2)
                return this.Equals((Vector2)other);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        #endregion

        #region overrides

        /// <summary>
        /// Obtains a string that represents the vector.
        /// </summary>
        /// <returns>A string text.</returns>
        public override string ToString()
        {
            return string.Format("{0}{2} {1}", this.x, this.y, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
        }

        /// <summary>
        /// Obtains a string that represents the vector.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A string text.</returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format("{0}{2} {1}", this.x.ToString(provider), this.y.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
        }

        #endregion
    }
}
