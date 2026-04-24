using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RVS.Generic;

namespace RVS
{
    /// <summary>
    /// StatusCtrl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StatusCtrl : UserControl
    {
        public int ID { get; set; } 
        public StatusCtrl()
        {
            InitializeComponent();
        }

        public void SetModelInfo(InspectionEquipDataControl aInfo)
        {
            lblMCName.Content = aInfo.EquipName;
            lblModel.Content = aInfo.ModelName;
            lblLot.Content = aInfo.LotNum;
        }

        public void SetResult(ResultData aData)
        {
            lblBYield.Content = aData.BeforeYield.ToString("00.00");
            lblAYield.Content = aData.Yield.ToString("00.00");
            lblInspStrip.Content = aData.InspectStrip.ToString();
            lblFailStrip.Content = aData.FailStrip.ToString();
            lblTotalUnit.Content = aData.TotalUnits.ToString();
            lblBadUnits.Content = aData.BadUnits.ToString();
            
            //txtAlign.Text = aData.Align.ToString();
            //txtRaw.Text = aData.Raw.ToString();
            //txtOpen.Text = aData.Open.ToString();
            //txtShort.Text = aData.Short.ToString();
            //txtBP.Text = aData.BP.ToString();
            //txtBall.Text = aData.Ball.ToString();
            //txtPin.Text = aData.NonPSR.ToString();
            //txtPSR.Text = aData.PSR.ToString();
            //txtBurr.Text = aData.Burr.ToString();
            //txtCC.Text = aData.Crack.ToString();
            //txtVH.Text = aData.Venthole.ToString();
            //txtVia.Text = aData.Via.ToString();
        }
    }
}
