using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PCS.ModelTeaching.OfflineTeaching
{
    public class MachineInformation
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }

        public string IP { get; set; }

        public double CamResolutionX { get; private set; }
        public double CamResolutionY { get; private set; }

        public MachineInformation(string aszCode, string aszName, string aszType)
        {
            this.Code = aszCode;
            this.Name = aszName;
            this.Type = aszType;

            double fResolution = ResolutionHelper.GetCameraResolution(aszType);
            SetResolution(fResolution);

            Debug.Assert(!string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Type));
            Debug.Assert(CamResolutionX >= 4.0 && CamResolutionY >= 4.0);
        }

        #region Set Camera Resolution.
        public void SetResolution(double afResolution)
        {
            if (afResolution < 4.0) afResolution = 12.0;
            CamResolutionX = afResolution;
            CamResolutionY = afResolution;
        }

        public void SetResolutionX(double afResolutionX)
        {
            if (afResolutionX < 4.0) afResolutionX = 12.0;
            CamResolutionX = afResolutionX;
        }

        public void SetResolutionY(double afResolutionY)
        {
            if (afResolutionY < 4.0) afResolutionY = 12.0;
            CamResolutionY = afResolutionY;
        }
        #endregion

        public override string ToString()
        {
            // For Debug.
            return string.Format("Machine Code:{0}, Name:{1}, Type:{2}, IP:{3}, Resolution X:{4:F2}, Resolution Y:{5:F2}", Code, Name, Type, IP, CamResolutionX, CamResolutionY);
        }
    }
}
