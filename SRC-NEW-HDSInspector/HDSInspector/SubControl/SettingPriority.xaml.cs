using Common;
using OpenCvSharp.Blob;
using OpenCvSharp.Flann;
using PCS;
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

namespace HDSInspector.SubControl
{
    /// <summary>
    /// SettingPriority.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingPriority : UserControl
    {
        TextBox[] txtPriority = new TextBox[1];
        Button[] btnColor = new Button[1];
        public NGInformationHelper NG;
        public SettingPriority(NGInformationHelper nginfo)
        {
            NG = nginfo.Clone();
            InitializeComponent();
            InitializeDialog(NG);            
        }

        private void InitializeDialog(NGInformationHelper nginfo)
        {
            int ColumnCount = 4; 
            ColumnDefinition temp = new ColumnDefinition();
            int RowCount = (int)(Math.Ceiling(((double)(nginfo.Size - 1)) / ColumnCount)) + 1;////양품 불량 코드 -1
            for (int i = 0; i < ColumnCount * 3; i++)
            {          
                if (i % 3 == 0) temp = new ColumnDefinition() { Width = new GridLength(9, GridUnitType.Star) };
                if (i % 3 == 1) temp = new ColumnDefinition() { Width = new GridLength(6, GridUnitType.Star) };
                if (i % 3 == 2) temp = new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) };
                GridPriority.ColumnDefinitions.Add(temp);
            }
            for (int i = 0; i < RowCount; i++) { GridPriority.RowDefinitions.Add(new RowDefinition()); }
            for (int i = 0; i < ColumnCount * 3; i++)
            {
                Label CategoryBadName = new Label() {
                    Content = "불량명", 
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.White), 
                    FontSize = 14,
                    FontWeight = FontWeights.ExtraBold,
                    Background = new SolidColorBrush(Colors.Gray),
                };
                Grid.SetRow(CategoryBadName, 0);
                Grid.SetColumn(CategoryBadName, i * 3);
                GridPriority.Children.Add(CategoryBadName);

                Label CategoryPriorityName = new Label()
                {
                    Content = "순위",
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 14,
                    FontWeight = FontWeights.ExtraBold,
                    Background = new SolidColorBrush(Colors.Gray)
                };
                Grid.SetRow(CategoryPriorityName, 0);
                Grid.SetColumn(CategoryPriorityName, i * 3 + 1);
                GridPriority.Children.Add(CategoryPriorityName);

                Label CategoryColorName = new Label()
                {
                    Content = "색상",
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 14,
                    FontWeight = FontWeights.ExtraBold,
                    Background = new SolidColorBrush(Colors.Gray)
                };
                Grid.SetRow(CategoryColorName, 0);
                Grid.SetColumn(CategoryColorName, i * 3 + 2);
                GridPriority.Children.Add(CategoryColorName);
            }


            txtPriority = new TextBox[MainWindow.NG_Info.Size];
            btnColor = new Button[MainWindow.NG_Info.Size];
            int RowArea = 1;
            int ColumArea = 0;
            for (int i = 1; i < nginfo.Size; i++)
            {
                Bad_Info info = nginfo.GetItem(i);
                int index = i - 1;
                if (RowCount == RowArea)
                {
                    RowArea = 1;
                    ColumArea++;
                }             
                Label lblName = new Label()
                {
                    Content = info.Name,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 14,
                    FontWeight = FontWeights.ExtraBold,
                    Background = new SolidColorBrush(Colors.Silver)
                };
                Grid.SetRow(lblName, RowArea);
                Grid.SetColumn(lblName, ColumArea * 3);
                GridPriority.Children.Add(lblName);

                txtPriority[index] = new TextBox()
                {
                    Text = info.Priority.ToString(),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 18,
                    FontWeight = FontWeights.ExtraBold,
                    Background = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(1, 1, 1, 1)            
                };
                Grid.SetRow(txtPriority[index], RowArea);
                Grid.SetColumn(txtPriority[index], ColumArea * 3 + 1);
                GridPriority.Children.Add(txtPriority[index]);

                btnColor[index] = new Button()
                {
                    Content = "선택",
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 14,
                    FontWeight = FontWeights.ExtraBold,
                    Background = info.Color,
                    Margin = new Thickness(1, 1, 1, 1),
                };           
                Grid.SetRow(btnColor[index], RowArea);
                Grid.SetColumn(btnColor[index], ColumArea * 3 + 2);
                GridPriority.Children.Add(btnColor[index]);
                btnColor[index].Tag = index.ToString();
                btnColor[index].Click += SettingPriority_Click;

                RowArea++;
            }            
        }

        private void SettingPriority_Click(object sender, RoutedEventArgs e)
        {
            int n = Convert.ToInt32(((Button)sender).Tag);
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Media.Color color = new System.Windows.Media.Color()
                {
                    A = dlg.Color.A,
                    R = dlg.Color.R,
                    G = dlg.Color.G,
                    B = dlg.Color.B
                };
                btnColor[n].Background = new SolidColorBrush(color);
            }
        }

        public void Save()
        {
            for (int i = 0; i < NG.Size-1; i++)
            {
                try
                {
                    int nPriority = Convert.ToInt32(txtPriority[i].Text);
                    System.Windows.Media.Brush color = btnColor[i].Background as SolidColorBrush;
                    NG.SetValue(i + 1, nPriority, color);
                }
                catch
                {

                }
            }
        }
    }
}
