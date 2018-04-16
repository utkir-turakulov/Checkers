using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Сheckers
{
    class Step
    {
        private Cell _from;
        private Cell _to;
        private IShape _shape;

        public Step(Cell from, Cell to, IShape shape)
        {
            _from = from;
            _to = to;
            _shape = shape;
        }

        public Cell From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        public Cell To
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
            }
        }
    }
}
