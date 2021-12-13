using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Chess.Tests
{
    [TestClass]
    public class ChessTests
    {
        [TestMethod]
        public void DistanceBetweenPointss()
        {
            var x1 = 1;
            var y1 = 2;
            var x2 = 2;
            var y2 = 1;

            var distance = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        [TestMethod]
        public void SlopeBetweenPoints()
        {
            decimal x1 = 2;
            decimal y1 = 2;
            decimal x2 = 1;
            decimal y2 = 1;

            decimal slope = (y2 - y1) / (x2 - x1);
        }
    }
}
