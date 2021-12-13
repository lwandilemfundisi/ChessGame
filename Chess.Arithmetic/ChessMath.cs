using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Arithmetic
{
    public static class ChessMath
    {
        /// <summary>
        /// Determines the direction a piece is moving to on the x-axis.
        /// </summary>
        /// <param name="x2">x value to new location</param>
        /// <param name="x1">x value from location</param>
        /// <returns></returns>
        public static int DirectionX_Axis(int x2, int x1)
        {
            return x2 - x1;
        }

        /// <summary>
        /// Determines the direction a piece is moving to on the y-axis.
        /// </summary>
        /// <param name="y2">y value to new location</param>
        /// <param name="y1">y value from location</param>
        /// <returns></returns>
        public static int DirectionY_Axis(int y2, int y1)
        {
            return y2 - y1;
        }

        /// <summary>
        /// Determines the distance between 2 points (x1, y1) and (x2, y2)
        /// </summary>
        /// <param name="x2">x value to new location</param>
        /// <param name="x1">x value from new location</param>
        /// <param name="y2">y value to new location</param>
        /// <param name="y1">y value from new location</param>
        /// <returns></returns>
        public static double Distance(double x2, double x1, double y2, double y1)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        /// <summary>
        /// Determines the slope between 2 points (x1, y1) and (x2, y2)
        /// </summary>
        /// <param name="y2">y value to new location</param>
        /// <param name="y1">y value from new location</param>
        /// <param name="x2">x value to new location</param>
        /// <param name="x1">x value from new location</param>
        /// <returns></returns>
        public static double Slope(double y2, double y1, double x2, double x1)
        {
            return (y2 - y1) / (x2 - x1);
        }
    }
}
