using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DCS.Light;
using Common;
using Microsoft.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Generic;
using Common.DataBase;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace DCS
{
    /// <summary>   Light control.  </summary>
    public partial class LightControl : UserControl
    {
        private LightDevice m_LightDevice = new LightDevice();

        public bool IsOpen { get; set; }
        public int[] LightValue = new int[8];
        public int LightNO = 0;
        IntegerUpDown[] text = new IntegerUpDown[8];
        Slider[] slider = new Slider[8];
        Label[] label = new Label[8];
        // ctor.
        public LightControl()
        {
            InitializeComponent();
            InitializeEvent();
        }

        // declare events.
        private void InitializeEvent()
        {
            this.txtLight1.ValueChanged += txtLight_ValueChanged;
            this.txtLight2.ValueChanged += txtLight_ValueChanged;
            this.txtLight3.ValueChanged += txtLight_ValueChanged;
            this.txtLight4.ValueChanged += txtLight_ValueChanged;
            this.txtLight5.ValueChanged += txtLight_ValueChanged;
            this.txtLight6.ValueChanged += txtLight_ValueChanged;
            this.txtLight7.ValueChanged += txtLight_ValueChanged;
            this.txtLight8.ValueChanged += txtLight_ValueChanged;
            text[0] = txtLight1; text[1] = txtLight2; text[2] = txtLight3; text[3] = txtLight4;
            text[4] = txtLight5; text[5] = txtLight6; text[6] = txtLight7; text[7] = txtLight8;
            slider[0] = sldrLight1; slider[1] = sldrLight2; slider[2] = sldrLight3; slider[3] = sldrLight4;
            slider[4] = sldrLight5; slider[5] = sldrLight6; slider[6] = sldrLight7; slider[7] = sldrLight8;
            label[0] = lblLight1; label[1] = lblLight2; label[2] = lblLight3; label[3] = lblLight4;
            label[4] = lblLight5; label[5] = lblLight6; label[6] = lblLight7; label[7] = lblLight8;
        }

        public void LoadDevice(LightDevice alightDevice, int[] anLightType, int totalScanCount)
        {
            // Load Device 순서 
            // 1. 객체를 받아와서 Null 이 아닌 경우 초기화
            // 2. 그 다음에는 값이 있으면 특정 값으로 설정
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
            if (alightDevice != null)
            {
                m_LightDevice = alightDevice;
                IsOpen = m_LightDevice.IsOpen;

            }
            else
            { 
                m_LightDevice = new LightDevice();
            }
        }

        private void txtLight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IsOpen)
            {
                int nChannel = int.Parse(((IntegerUpDown)sender).Tag.ToString());

                switch (nChannel)
                {
                    case 1:
                        if (txtLight1.Text == "") sldrLight1.Value = 0;
                        else sldrLight1.Value = Convert.ToDouble(txtLight1.Text);
                        m_LightDevice.SetBrightness(LightNO, nChannel, (int)sldrLight1.Value);
                        break;
                    case 2:
                        if (txtLight2.Text == "") sldrLight2.Value = 0;
                        else sldrLight2.Value = Convert.ToDouble(txtLight2.Text);
                        m_LightDevice.SetBrightness(LightNO, nChannel, (int)sldrLight2.Value);
                        break;
                    case 3:
                        if (txtLight3.Text == "") sldrLight3.Value = 0;
                        else sldrLight3.Value = Convert.ToDouble(txtLight3.Text);
                        m_LightDevice.SetBrightness(LightNO, nChannel, (int)sldrLight3.Value);
                        break;
                    case 4:
                        if (txtLight4.Text == "") sldrLight4.Value = 0;
                        else sldrLight4.Value = Convert.ToDouble(txtLight4.Text);
                        m_LightDevice.SetBrightness(LightNO, nChannel, (int)sldrLight4.Value);
                        break;
                    case 5:
                        if (txtLight5.Text == "") sldrLight5.Value = 0;
                        else sldrLight5.Value = Convert.ToDouble(txtLight5.Text);
                        m_LightDevice.SetBrightness(LightNO, nChannel, (int)sldrLight5.Value);
                        break;
                    case 6:
                        if (txtLight6.Text == "") sldrLight6.Value = 0;
                        else sldrLight6.Value = Convert.ToDouble(txtLight6.Text);
                        m_LightDevice.SetBrightness(LightNO, nChannel, (int)sldrLight6.Value);
                        break;
                    case 7:
                        if (txtLight7.Text == "") sldrLight7.Value = 0;
                        else sldrLight7.Value = Convert.ToDouble(txtLight7.Text);
                        m_LightDevice.SetBrightness(LightNO, nChannel, (int)sldrLight7.Value);
                        break;
                    case 8:
                        if (txtLight8.Text == "") sldrLight8.Value = 0;
                        else sldrLight8.Value = Convert.ToDouble(txtLight8.Text);
                        m_LightDevice.SetBrightness(LightNO, nChannel, (int)sldrLight8.Value);
                        break;
                }
            }
        }

        private void sldLight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsOpen)
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

        public void LightOn(bool abIsOn)
        {
            if (m_LightDevice != null)
            {
                m_LightDevice.SetOnOffEx(LightNO, abIsOn);
            }
        }

        public void Destroy()
        {
            m_LightDevice.ClosePortAll();
            IsOpen = false;
        }

        public int[] GetChannelValues()
        {
            int[] channelvalues = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            try
            {
                if (string.IsNullOrEmpty(txtLight1.Text)) txtLight1.Text = "0";
                if (string.IsNullOrEmpty(txtLight2.Text)) txtLight2.Text = "0";
                if (string.IsNullOrEmpty(txtLight3.Text)) txtLight3.Text = "0";
                if (string.IsNullOrEmpty(txtLight4.Text)) txtLight4.Text = "0";
                if (string.IsNullOrEmpty(txtLight5.Text)) txtLight5.Text = "0";
                if (string.IsNullOrEmpty(txtLight6.Text)) txtLight6.Text = "0";
                if (string.IsNullOrEmpty(txtLight7.Text)) txtLight7.Text = "0";
                if (string.IsNullOrEmpty(txtLight8.Text)) txtLight8.Text = "0";

                channelvalues[0] = Convert.ToInt32(txtLight1.Text);
                channelvalues[1] = Convert.ToInt32(txtLight2.Text);
                channelvalues[2] = Convert.ToInt32(txtLight3.Text);
                channelvalues[3] = Convert.ToInt32(txtLight4.Text);
                channelvalues[4] = Convert.ToInt32(txtLight5.Text);
                channelvalues[5] = Convert.ToInt32(txtLight6.Text);
                channelvalues[6] = Convert.ToInt32(txtLight7.Text);
                channelvalues[7] = Convert.ToInt32(txtLight8.Text);
            }
            catch { }
            return channelvalues;
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

        public void SetValues(int[,] val, int no, int Index, string[,] Channel)
        {
            LightNO = no;
            int[] value = new int[8];
            int ch = 0;
            if (no == 0) ch = 0;
            else if (no == 1 || no == 2) ch = 1;
            else ch = 2;
            for(int i = 0; i < 8; i++)
            {
                if(Channel[ch, i] != "")
                {
                    label[i].Content = Channel[ch, i];
                    label[i].Visibility = Visibility.Visible;
                    slider[i].Visibility = Visibility.Visible;
                    text[i].Visibility = Visibility.Visible;
                    text[i].Text = Convert.ToString(val[Index, i]);
                    slider[i].Value = Convert.ToDouble(val[Index, i]);
                    value[i] = val[Index, i];
                }
                else
                {
                    label[i].Visibility = Visibility.Hidden;
                    slider[i].Visibility = Visibility.Hidden;
                    text[i].Visibility = Visibility.Hidden;
                    text[i].Text = "0";
                    slider[i].Value = 0.0;
                    value[i] = 0;
                }
            }
            m_LightDevice.SetLightValue(no, Index, value);
        }
    }
}
