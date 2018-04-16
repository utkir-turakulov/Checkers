using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Сheckers
{
   public class Cell
    {
        private int _Row;
        private int _Coll;

        public Cell(int x, int y)
        {
            _Row = x;
            _Coll = y;
        }
        public Cell() { } 

        public int Row
        {
            get
            {
                return _Row;
            }
            set
            {
                _Row = value;
            }
        }

        public int Coll
        {
            get
            {
                return _Coll;
            }
            set
            {
                _Coll = value;
            }
        }

    }
}
