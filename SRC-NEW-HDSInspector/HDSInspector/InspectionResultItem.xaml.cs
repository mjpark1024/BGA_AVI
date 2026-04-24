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
    /// InspectionResultItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InspectionResultItem : UserControl
    {
        public InspectionResultItem(string BadName, System.Windows.Media.Brush BadColor, Binding BadCount, bool Yield = false)
        {
            DataContext = this;
            InitializeComponent();
            badColor.Background = BadColor;
            badName.Content = BadName;
            badName.Margin = new Thickness(-5, -2, -5, -2);
            badCount.FontSize = 13;
            badCount.Margin = new Thickness(-5, -2, -5, -2);
            if (Yield)
            {
                badName.FontSize = 15;
                badName.Foreground = new SolidColorBrush(Colors.Blue);
                badName.FontWeight = FontWeights.ExtraBold;
                badCount.FontSize = 15;
                badCount.FontWeight = FontWeights.ExtraBold;
            }
            badCount.SetBinding(TextBlock.TextProperty, BadCount);
        }
    }
}
