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

namespace Common.Control
{
    public partial class SimpleGageBar : UserControl
    {
        private int m_Percent;

        public SimpleGageBar(double afTotal, double afValue)
        {
            m_Percent = (int)(afValue / afTotal * 100.0);
            
            InitializeComponent();
            Loaded += (s, e) => InitializeGage();
        }

        public void SetGage(double afTotal, double afValue)
        {
            m_Percent = (int)(afValue / afTotal * 100.0);

            // Update Gage.
            InitializeGage();
        }

        private void InitializeGage()
        {
            if (GagePanel.Children.Count > 0)
            {
                GagePanel.Children.Clear();
                GagePanel.ColumnDefinitions.Clear();
            }

            int nRectCount = m_Percent / 10;
            for (int i = 0; i < 10; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                GagePanel.ColumnDefinitions.Add(col);

                Border border = new Border();
                border.Margin = new Thickness(1);
                border.CornerRadius = new CornerRadius(2);
                border.Opacity = 0.9;

                if (i < nRectCount)
                    border.Background = new SolidColorBrush(Colors.White);
                else
                    border.Background = new SolidColorBrush(Colors.Transparent);

                Grid.SetColumn(border, GagePanel.ColumnDefinitions.Count - 1);
                GagePanel.Children.Add(border);
            }
            txtGage.Text = m_Percent.ToString() + "%";
        }
    }
}
