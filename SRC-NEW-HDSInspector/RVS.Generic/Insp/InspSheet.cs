using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCS
{
    class InspSheet : InspBase
    {
        
        public InspSheet(int h, int w)
            : base(h, w)
        {
           
        }

        public List<InspUnit> InspUnitList{ get; set;}


    }
}
