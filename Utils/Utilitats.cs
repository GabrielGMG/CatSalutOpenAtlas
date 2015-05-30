using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CatSalutOpenAtlas.Utils
{
    public class Utilitats
    {
        public static List<string> GeneratePalette(int n, string opacity)
        {
            List<string> colors = new List<string>();
            for (int i = n; i > 0; i--)
            {
                int R = (255 * (i*100/n)) / 100;
                int G = (255 * (100 - i * 100 / n)) / 100;
                int B = 50;
                colors.Add("[" + R + "," + G + "," + B + "," + opacity + "]");
            }
            return colors;
        }

        public static double CalculateColorSpacing(int elements, double max, double min)
        {
            return (max - min) / elements;
        }

        public static string IntToHex(int i)
        {
            return i.ToString("X2");
        }
    }
}