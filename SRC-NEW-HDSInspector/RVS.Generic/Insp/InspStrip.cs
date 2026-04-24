using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCS
{
    class InspStrip : InspBase
    {
  

        public InspStrip(int h, int w)
            : base(h, w)
        {
           
        }

        public List<InspSheet> InspSheetList{get ; set;}

    }
}
