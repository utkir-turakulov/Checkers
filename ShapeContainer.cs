using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Сheckers
{
   public class ShapeContainer
    {
        private IShape shape;


        public ShapeContainer(IShape shape)
        {
            this.shape = shape;
        }
        public ShapeContainer()
        {

        }

        public IShape Shape
        {
            get
            {
                return shape;
            }
        }
        public void SetShape(IShape shape)
        {
            this.shape = shape;
        }

    }
}
