using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCS
{
    class InspUnit : InspBase
    {
   
        public InspUnit(int h, int w)
            : base(h, w)
        {
           
        }

        public List<InspRoi> InspRoiList { get; set; }

    }
}
