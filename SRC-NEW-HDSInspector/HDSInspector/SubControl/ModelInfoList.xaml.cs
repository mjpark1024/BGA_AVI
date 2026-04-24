using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HDSInspector.SubControl
{
    /// <summary>
    /// ModelInfoList.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public delegate void CloseEventHandler();

    public class GroupAndModel
    {
        private string m_Group;
        private List<string> m_Model = new List<string>();

        public string Group
        {
            get { return m_Group; }
            set { m_Group = value; }
        }

        public List<string> Model
        {
            get { return m_Model; }
            set { m_Model = value; }
        }

    }

    public partial class ModelInfoList : UserControl
    {
        public List<GroupAndModel> lstGM = new List<GroupAndModel>();
        public event CloseEventHandler CloseEvent;
        public ModelInfoList()
        {
            InitializeComponent();
            this.btnClose.Click += new RoutedEventHandler(btnClose_Click);
            this.lbGroup.SelectionChanged += new SelectionChangedEventHandler(lbGroup_SelectionChanged);
        }

        void lbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lbGroup.SelectedIndex;
            if (index >= 0)
            {
                this.lbModel.DataContext = lstGM[index].Model;
                this.lbModel.SelectedIndex = 0;
            }
        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseEventHandler eventRunner = CloseEvent;
            eventRunner();
        }


    }
}
