using System;
using System.Windows;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using RVS.Generic;
using PCS.Interface;

namespace RVS_Test
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public RemotingPCSInterface m_RVSObj;
        VerifyResult m_nVerifyDoneMassage = VerifyResult.None;
        int[,] m_aVerifyResult;
        bool bStop = true;
        int StripID = 0;
        int[,] map;
        //InspectionResultDataControl data = new InspectionResultDataControl(null);
        
        public MainWindow()
        {
            InitializeComponent();
            RVStoPCSRemotingServer();
            PCStoRVSRemotingClient();
            btnStop.IsEnabled = false;
            AMap.SetStripMap(30, 6);
            BMap.SetStripMap(30, 6);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InspectionEquipDataControl equip = new InspectionEquipDataControl();
            equip.EquipName = "CAI04";
            equip.GroupName = "";
            equip.ModelName = "TEST";
            equip.LotNum = "12345678";
            equip.UnitX = 30;
            equip.UnitY = 6;
            bStop = false;
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            //data = new InspectionResultDataControl(null);

            RemotingRVSInterface.VerifyDoneEvent += RemotingRVSInterface_VerifyDoneEvent; 

            if (!m_RVSObj.ConnectEquipment(equip, 30))
            {
                MessageBox.Show(string.Format("Verify PC에 [CAI04 장비]를 등록해주세요"));
                return;
            }
            
        }

        void RemotingRVSInterface_VerifyDoneEvent(VerifyResult anResultMessage, int[,] Result)
        {
            m_nVerifyDoneMassage = anResultMessage;
            m_aVerifyResult = Result; ;
        }

        #region Handling .Net Remoting
        /// <summary>   Sets Pcs remoting server. </summary>
        [STAThread]
        private void RVStoPCSRemotingServer()
        {
            try
            {
                BinaryClientFormatterSinkProvider clientProvider = null;
                BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();

                serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                System.Collections.IDictionary props = new System.Collections.Hashtable();
                props["port"] = 50001;
                props["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                TcpChannel tcpChannel = new TcpChannel(props, clientProvider, serverProvider);

                //Port Parsing: getPort
                ChannelServices.RegisterChannel(tcpChannel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemotingRVSInterface), "PCSUri", WellKnownObjectMode.Singleton);
            }
            catch
            {

            }
        }

        private void PCStoRVSRemotingClient()
        {
            try
            {
                //IP, Port Parsing : getIP, getPort
                if (m_RVSObj == null)
                {
                    m_RVSObj = (RemotingPCSInterface)Activator.GetObject(typeof(RVS.Generic.RemotingPCSInterface), "tcp://55.60.103.177:50006/RVSUri");
                }
            }
            catch
            {
                m_RVSObj = null;
            }
        }

        #endregion

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            bStop = true;
            Thread.Sleep(1000);
            m_RVSObj.DisconnectEquipment("CAI04");
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            
        }
    }
}
