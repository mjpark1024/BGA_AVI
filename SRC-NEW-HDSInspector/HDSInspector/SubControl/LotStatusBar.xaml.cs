using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HDSInspector.SubWindow;
using System.Windows.Media.Imaging;

namespace HDSInspector.SubControl
{
    public partial class LotStatusBar : UserControl
    {
        public LotStatusBar()
        {
            InitializeComponent();
        }

        public void Add(bool abIsGood)
        {
            ColumnDefinition col = new ColumnDefinition();
            StripStackPanel.ColumnDefinitions.Add(col);

            Border add = new Border();
            add.Opacity = 0.5;
            
            #region Set Margin
            if (StripStackPanel.ColumnDefinitions.Count == 150)
            {
                foreach (Border border in StripStackPanel.Children)
                {
                    border.Margin = new Thickness(0.0);
                    border.CornerRadius = new CornerRadius(0);
                }
            }
            else if (StripStackPanel.ColumnDefinitions.Count < 150)
            {
                add.Margin = new Thickness(1);
                add.CornerRadius = new CornerRadius(2);
            }
            #endregion

            #region Background Color
            if (abIsGood)
                add.Background = new SolidColorBrush(Colors.Gray);
            else
                add.Background = new SolidColorBrush(Colors.Red);
            #endregion

            add.ToolTip = (StripStackPanel.ColumnDefinitions.Count).ToString();
            Grid.SetColumn(add, StripStackPanel.ColumnDefinitions.Count - 1);
            StripStackPanel.Children.Add(add);
        }

        public void ClearAll()
        {
            StripStackPanel.Children.Clear();
            StripStackPanel.ColumnDefinitions.Clear();
        }
    }
}
