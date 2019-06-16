using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBoyDIno.Structs
{
    public readonly struct InfoObjectsPositions
    {
        public readonly InfoColumn Column1;
        public readonly InfoColumn Column2;
        public readonly InfoColumn Column3;
        public readonly InfoColumn Column4;

        public InfoObjectsPositions(InfoColumn column1, InfoColumn column2, InfoColumn column3, InfoColumn column4)
        {
            Column1 = column1;
            Column2 = column2;
            Column3 = column3;
            Column4 = column4;
        }
    }
}
