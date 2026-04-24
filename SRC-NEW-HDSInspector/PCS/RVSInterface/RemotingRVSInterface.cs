using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RVS.Generic;

namespace PCS.Interface
{
    /// <summary>   Delegate for handling VerifyDone events. </summary>
    /// <param name="DoneMessage">  Message describing the done. </param>
    public delegate void VerifyDoneEventHandler(VerifyResult anResultMessage, int[,] Result);

    /// <summary>   Remoting rvs interface.  </summary>
    public class RemotingRVSInterface : MarshalByRefObject
    {
        /// <summary> Event queue for all listeners interested in VerifyDone events. </summary>
        public static event VerifyDoneEventHandler VerifyDoneEvent;

        /// <summary>   Verify done message to pcs. </summary>
        /// <param name="anResultMessage">  Message describing the done. </param>
        public void VerifyDoneMessageToPCS(VerifyResult anResultMessage, int[,] aResult)
        {
            VerifyDoneEventHandler verifyDone = VerifyDoneEvent;

            if (verifyDone != null)
            {
                try
                {
                    verifyDone(anResultMessage, aResult);
                }
                catch { }
            }
        }
    }
}
