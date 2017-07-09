using EloBuddy;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace vEvade
{
    public static class Utils
    {
        #region Public Properties

        /// <summary>
        ///     Gets the game time tick count.
        /// </summary>
        public static int GameTimeTickCount
        {
            get { return (int)(Game.Time * 1000); }
        }

        /// <summary>
        ///     Gets the tick count.
        /// </summary>
        public static int TickCount
        {
            get { return Environment.TickCount & int.MaxValue; }
        }

        #endregion

        public static void DrawLineInWorld(Vector3 start, Vector3 end, int width, Color color)
        {
            var from = Drawing.WorldToScreen(start);
            var to = Drawing.WorldToScreen(end);
            Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
            //Drawing.DrawLine(from.X, from.Y, to.X, to.Y, width, color);
        }
    }
}
