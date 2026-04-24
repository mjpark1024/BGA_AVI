using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;


namespace Inspector
{

    class InspectionProcess
    {

        public static void InspectProc()
        {


        }

        public static void MoveProc()
        {


        }

        public static void ResultProc()
        {

        }


        public void Inspection()
        {
            Thread InspProc = new Thread(new ThreadStart(InspectProc));
            InspProc.Start();

        }









    }
}
