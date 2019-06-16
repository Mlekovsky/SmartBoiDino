using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBoyDIno.Structs
{
    public readonly struct InfoColumn
    {
        public readonly Vector2 Row1;
        public readonly Vector2 Row2;
        public readonly Vector2 Row3;
        public readonly Vector2 Row4;
        public readonly Vector2 Row5;
        public readonly Vector2 Row6;
        public readonly Vector2 Row7;
        public readonly Vector2 Row8;

        public InfoColumn(Vector2 row1, Vector2 row2, Vector2 row3, Vector2 row4, Vector2 row5, Vector2 row6, Vector2 row7, Vector2 row8)
        {
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
            Row4 = row4;
            Row5 = row5;
            Row6 = row6;
            Row7 = row7;
            Row8 = row8;
        }
    }
}
