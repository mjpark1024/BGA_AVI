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

namespace HDSInspector
{
    /// <summary>
    /// TextLamp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TextLamp : UserControl
    {
        private const string imgON = "../Images/LED_ON.png";
        private const string imgOFF = "../Images/LED_OFF.png";
        bool m_Status = true;
        string m_Text = "";
        public string Text
        {
            set
            {
                m_Text = value;
                lblText.Content = m_Text;
            }
        }
        public bool Status
        {
            set
            {
                m_Status = value;
                StatusChange();
            }
            get
            {
                return m_Status;
            }
        }
        private void StatusChange()
        {
            try
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                if (m_Status)
                    bi3.UriSource = new Uri(imgON, UriKind.Relative);
                else
                    bi3.UriSource = new Uri(imgOFF, UriKind.Relative);
                bi3.EndInit();
                this.imgLED.Source = bi3;
            }
            catch { }
        }
        public TextLamp()
        {
            InitializeComponent();
        }
    }
}

