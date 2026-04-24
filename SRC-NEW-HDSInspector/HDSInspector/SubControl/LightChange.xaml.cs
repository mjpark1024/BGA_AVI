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
using Microsoft.Windows.Controls;
using Common;

namespace HDSInspector.SubControl
{
    /// <summary>
    /// LightChange.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LightChange : UserControl
    {
        IntegerUpDown[] text = new IntegerUpDown[8];
        Slider[] slider = new Slider[8];
        Label[] label = new Label[8];
        public LightChange()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            txtLight1.Maximum = 255;
            txtLight2.Maximum = 255;
            txtLight3.Maximum = 255;
            txtLight4.Maximum = 255;
            txtLight5.Maximum = 255;
            txtLight6.Maximum = 255;
            txtLight7.Maximum = 255;
            txtLight8.Maximum = 255;
            sldrLight1.Maximum = 255;
            sldrLight2.Maximum = 255;
            sldrLight3.Maximum = 255;
            sldrLight4.Maximum = 255;
            sldrLight5.Maximum = 255;
            sldrLight6.Maximum = 255;
            sldrLight7.Maximum = 255;
            sldrLight8.Maximum = 255;
            text[0] = txtLight1; text[1] = txtLight2; text[2] = txtLight3; text[3] = txtLight4;
            text[4] = txtLight5; text[5] = txtLight6; text[6] = txtLight7; text[7] = txtLight8;
            slider[0] = sldrLight1; slider[1] = sldrLight2; slider[2] = sldrLight3; slider[3] = sldrLight4;
            slider[4] = sldrLight5; slider[5] = sldrLight6; slider[6] = sldrLight7; slider[7] = sldrLight8;
            label[0] = lblLight1; label[1] = lblLight2; label[2] = lblLight3; label[3] = lblLight4;
            label[4] = lblLight5; label[5] = lblLight6; label[6] = lblLight7; label[7] = lblLight8;
            this.txtLight1.ValueChanged += txtLight_ValueChanged;
            this.txtLight2.ValueChanged += txtLight_ValueChanged;
            this.txtLight3.ValueChanged += txtLight_ValueChanged;
            this.txtLight4.ValueChanged += txtLight_ValueChanged;
            this.txtLight5.ValueChanged += txtLight_ValueChanged;
            this.txtLight6.ValueChanged += txtLight_ValueChanged;
            this.txtLight7.ValueChanged += txtLight_ValueChanged;
            this.txtLight8.ValueChanged += txtLight_ValueChanged;
        }

        public void LoadLightData(int no, int DBIndex, int[,] val, string[,] Channel)
        {
            int nLight = 0;
            int num = 0;
            if (no == 0) nLight = 0;
            else if (no <= 2) nLight = 1;
            else nLight = 2;
            //if (no == 0) nLight = 0;
            //else if (no <= 2) nLight = 1;
            //else nLight = 2;
            for (int i = 0; i < 8; i++)
            {
                if (Channel[nLight, i] != "")
                {
                    label[i].Content = Channel[nLight, i];
                    label[i].Visibility = Visibility.Visible;
                    slider[i].Visibility = Visibility.Visible;
                    text[i].Visibility = Visibility.Visible;
                    text[i].Text = Convert.ToString(val[DBIndex, i]);
                    slider[i].Value = Convert.ToDouble(val[DBIndex, i]);
                }
                else
                {
                    label[i].Visibility = Visibility.Collapsed;
                    slider[i].Visibility = Visibility.Collapsed;
                    text[i].Visibility = Visibility.Collapsed;
                    text[i].Text = "0";
                    slider[i].Value = 0.0;
                }
            }
        }

        public int[] GetValues()
        {
            int[] val = new int[8];
            val[0] = (int)sldrLight1.Value;
            val[1] = (int)sldrLight2.Value;
            val[2] = (int)sldrLight3.Value;
            val[3] = (int)sldrLight4.Value;
            val[4] = (int)sldrLight5.Value;
            val[5] = (int)sldrLight6.Value;
            val[6] = (int)sldrLight7.Value;
            val[7] = (int)sldrLight8.Value;
            return val;
        }

        private void txtLight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int nChannel = int.Parse(((IntegerUpDown)sender).Tag.ToString());
            switch (nChannel)
            {
                case 1:
                    if (txtLight1.Text == "") sldrLight1.Value = 0;
                    else sldrLight1.Value = Convert.ToDouble(txtLight1.Text);
                    break;
                case 2:
                    if (txtLight2.Text == "") sldrLight2.Value = 0;
                    else sldrLight2.Value = Convert.ToDouble(txtLight2.Text);
                    break;
                case 3:
                    if (txtLight3.Text == "") sldrLight3.Value = 0;
                    else sldrLight3.Value = Convert.ToDouble(txtLight3.Text);
                    break;
                case 4:
                    if (txtLight4.Text == "") sldrLight4.Value = 0;
                    else sldrLight4.Value = Convert.ToDouble(txtLight4.Text);
                    break;
                case 5:
                    if (txtLight5.Text == "") sldrLight5.Value = 0;
                    else sldrLight5.Value = Convert.ToDouble(txtLight5.Text);
                    break;
                case 6:
                    if (txtLight6.Text == "") sldrLight6.Value = 0;
                    else sldrLight6.Value = Convert.ToDouble(txtLight6.Text);
                    break;
                case 7:
                    if (txtLight7.Text == "") sldrLight7.Value = 0;
                    else sldrLight7.Value = Convert.ToDouble(txtLight7.Text);
                    break;
                case 8:
                    if (txtLight8.Text == "") sldrLight8.Value = 0;
                    else sldrLight8.Value = Convert.ToDouble(txtLight8.Text);
                    break;
            }
        }

        private void sldLight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int nChannel = int.Parse(((Slider)sender).Tag.ToString());
            switch (nChannel)
            {
                case 1: txtLight1.Text = Convert.ToString((int)sldrLight1.Value); break;
                case 2: txtLight2.Text = Convert.ToString((int)sldrLight2.Value); break;
                case 3: txtLight3.Text = Convert.ToString((int)sldrLight3.Value); break;
                case 4: txtLight4.Text = Convert.ToString((int)sldrLight4.Value); break;
                case 5: txtLight5.Text = Convert.ToString((int)sldrLight5.Value); break;
                case 6: txtLight6.Text = Convert.ToString((int)sldrLight6.Value); break;
                case 7: txtLight7.Text = Convert.ToString((int)sldrLight7.Value); break;
                case 8: txtLight8.Text = Convert.ToString((int)sldrLight8.Value); break;
            }
        }
    }
}
