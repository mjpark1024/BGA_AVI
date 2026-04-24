using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace HDSInspector
{
    /// <summary>   Interaction logic for InspectionResultNGCountList.xaml. </summary>   
    public partial class InspectionResultNGCountList : UserControl
    {
        public InspectionResultNGCountList()
        {
            InitializeComponent();

            lbNGCountList.PreviewMouseWheel += new MouseWheelEventHandler(lbNGCountList_PreviewMouseWheel);
        }

        void lbNGCountList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double m_MouseScroll = this.NGContListScrollViewer.VerticalOffset;

            m_MouseScroll -= e.Delta / 8;
            //At Start Point
            if (m_MouseScroll < 0)
            {
                m_MouseScroll = 0;
                this.NGContListScrollViewer.ScrollToVerticalOffset(m_MouseScroll);
            }
            //At End Point
            else if (m_MouseScroll > this.NGContListScrollViewer.ScrollableHeight)
            {
                m_MouseScroll = this.NGContListScrollViewer.ScrollableHeight;
                this.NGContListScrollViewer.ScrollToEnd();
            }
            //Middle
            else
            {
                this.NGContListScrollViewer.ScrollToVerticalOffset(m_MouseScroll);
            }
        }

        public void AddItem(NGCountListData aNGCountListData)
        {
            this.lbNGCountList.Dispatcher.Invoke(new Action(delegate
            {
                lbNGCountList.Items.Add(aNGCountListData);
            }));
        }


        public void Clear()
        {
            this.lbNGCountList.Dispatcher.Invoke(new Action(delegate
           {
               if (lbNGCountList.Items.Count > 0)
               {
                   lbNGCountList.Items.Clear();
               }
           }));
        }


        public void SetItem(string astrNGName, int anNGCount)
        {
            foreach (NGCountListData ng in lbNGCountList.Items)
            {
                if (ng.NGName1 == astrNGName)
                {
                    ng.NGCnt1 = anNGCount;
                }

                if (ng.NGName2 == astrNGName)
                {
                    ng.NGCnt2 = anNGCount;
                }
            }
        }
    }
}