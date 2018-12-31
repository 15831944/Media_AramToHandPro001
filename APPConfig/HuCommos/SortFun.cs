using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPConfig.HuCommos
{
   public class SortFun
    {
        

/// 插入排序算法
/// </summary>
/// <param name="dblArray"></param>

    public  static void InsertSort(ref double[] dblArray)
        {
            for (int i = 1; i < dblArray.Length; i++)
            {
                int frontArrayIndex = i - 1;
                int CurrentChangeIndex = i;
                while (frontArrayIndex >= 0)
                {
                    if (dblArray[CurrentChangeIndex] < dblArray[frontArrayIndex])
                    {
                        ChangeValue(ref dblArray[CurrentChangeIndex], ref dblArray[frontArrayIndex]);
                        CurrentChangeIndex = frontArrayIndex;
                    }
                    frontArrayIndex--;
                }
            }
        }

        //Point
        public static void InsertSort_X(ref List<Point> dblArray)
        {
            for (int i = 1; i < dblArray.Count; i++)
            {
                int frontArrayIndex = i - 1;
                int CurrentChangeIndex = i;
                while (frontArrayIndex >= 0)
                {
                    if (dblArray[CurrentChangeIndex].Position.X < dblArray[frontArrayIndex].Position.X)//小在前
                    {
                        ChangeValue(ref dblArray.ToArray()[CurrentChangeIndex], ref dblArray.ToArray()[frontArrayIndex]);
                        CurrentChangeIndex = frontArrayIndex;
                    }
                    frontArrayIndex--;
                }
            }
        }

        public static void InsertSort_Y(ref List<Point> dblArray)
        {
            for (int i = 1; i < dblArray.Count; i++)
            {
                int frontArrayIndex = i - 1;
                int CurrentChangeIndex = i;
                while (frontArrayIndex >= 0)
                {
                    if (dblArray[CurrentChangeIndex].Position.Y < dblArray[frontArrayIndex].Position.Y)//小在前
                    {
                        ChangeValue(ref dblArray.ToArray()[CurrentChangeIndex], ref dblArray.ToArray()[frontArrayIndex]);
                        CurrentChangeIndex = frontArrayIndex;
                    }
                    frontArrayIndex--;
                }
            }
        }
        /// <summary>
        /// 在内存中交换两个数字的值
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>

        static void ChangeValue(ref Point A, ref Point B)
        {
            Point Temp = A;
            A = B;
            B = Temp;
        }

        static void ChangeValue(ref double A, ref double B)
        {
            double Temp = A;
            A = B;
            B = Temp;
        }
    }
}
