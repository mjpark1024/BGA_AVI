using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common.Drawing.MarkingInformation;
using Common.DataBase;

namespace Common.Drawing.MarkingTypeUI
{
    /// <summary>
    /// Number.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Number : UserControl, IMarkingTypeUICommands
    {
        private NumberProperty m_previewValue;
        public event MoveEventHandler MoveClick;
        public event SizeChangeEventHandler MarkSizeChanged;
        public event LocationChangeEventHandler LocationChanged;
        public event PenParamChanged ParamChange;
        public event SaveChanged SaveChange;
        #region Inspect Type
        public eMarkingType MarkType
        {
            get
            {
                return m_enumMarkType;
            }
        }
        private eMarkingType m_enumMarkType;
        private double Resolution;
        private double scale;
        #endregion
        public Number()
        {
            InitializeComponent();
            location.SetClick += location_SetClick;
            location.LocationSetClick += location_SetClick;
            location.MoveClick += location_MoveClick;
            cbParaNumber.SelectionChanged += cbParaNumber_SelectionChanged;
            this.txtCHeight.TextChanged += txtCHeight_TextChanged;
            this.txtHeight.TextChanged += txtHeight_TextChanged;
        }

        void txtHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double c = Convert.ToDouble(txtHeight.Text) * scale * 10000.0;
                double v = Math.Round(c) / 10000.0;
                txtCHeight.Text = v.ToString("0.0000");
            }
            catch { }
        }

        void txtCHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double c = Convert.ToDouble(txtCHeight.Text) / scale * 10000.0;
                double v = Math.Round(c) / 10000.0;
                txtHeight.Text = v.ToString("0.0000");
            }
            catch { }
        }

        void cbParaNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PenParamChanged er = ParamChange;
            if (er != null)
            {
                er(cbParaNumber.SelectedIndex);
            }
        }

        void location_MoveClick(int nid, double pitch)
        {
            MoveEventHandler er = MoveClick;
            if (er != null) er(nid, pitch, m_enumMarkType);
        }

        void location_SetClick()
        {
            SizeChangeEventHandler er = MarkSizeChanged;
            if (er != null)
            {
                er(Convert.ToDouble(location.txtLeft.Text), Convert.ToDouble(location.txtTop.Text),
                    Convert.ToDouble(location.txtWidth.Text), Convert.ToDouble(location.txtHeight.Text),
                    Convert.ToDouble(location.txtRotate.Text), eMarkingType.eMarkingNumber);
            }
        }
        void location_SetClick(int Location)
        {
            LocationChangeEventHandler er = LocationChanged;
            if (er != null)
            {
                er(Location);
            }
            location_SetClick();
        }
        public void SetDialog(double resolution, eMarkingType markType)
        {
            this.m_enumMarkType = markType;
            this.Resolution = resolution;
        }

        public void LocationEnabled(bool IsEnable)
        {
            location.IsEnabled = IsEnable;
        }

        public void TrySave(GraphicsBase graphic)
        {
            try
            {
                string fNTFile = "c:\\mime\\font\\" + txtFontFile.Text;
                int paraNumber = cbParaNumber.SelectedIndex;
                double capitalHeight = Convert.ToDouble(txtCHeight.Text);
                double charGap = Convert.ToDouble(txtCGap.Text);
                double charWidth = Convert.ToDouble(txtWidth.Text) ;
                double charHeight = Convert.ToDouble(txtHeight.Text) ;
                double lineGap = Convert.ToDouble(txtLineGap.Text);
                double spaceSize = Convert.ToDouble(txtSpaceSize.Text);
                int refNumber = Convert.ToInt32(txtRefNo.Text);
                int specialType = Convert.ToInt32(txtSpecialType.Text);
                double left = Convert.ToDouble(this.location.txtLeft.Text);
                double top = Convert.ToDouble(this.location.txtTop.Text);
                double width = Convert.ToDouble(this.location.txtWidth.Text);
                double height = Convert.ToDouble(this.location.txtHeight.Text);
                double rotate = Convert.ToDouble(this.location.txtRotate.Text);
                

                NumberProperty oldNum = graphic.MarkInfo.MarkInfo as NumberProperty;
                if (oldNum == null)
                {
                    NumberProperty numValue = new NumberProperty();
                    numValue.FNTFile = fNTFile;
                    numValue.ParaNumber = paraNumber;
                    numValue.CapitalHeight = capitalHeight;
                    numValue.CharGap = charGap;
                    numValue.CharWidth = charWidth;
                    numValue.CharHeight = charHeight;
                    numValue.LineGap = lineGap;
                    numValue.SpaceSize = spaceSize;
                    numValue.RefNumber = refNumber;
                    numValue.SpecialType = specialType;
                    numValue.Left = left;
                    numValue.Top = top;
                    numValue.Width = width;
                    numValue.Height = height;
                    numValue.Rotate = rotate;
                }
                else
                {
                    if (m_previewValue == null)
                        m_previewValue = new NumberProperty();
                    m_previewValue.FNTFile = oldNum.FNTFile;
                    m_previewValue.ParaNumber = oldNum.ParaNumber;
                    m_previewValue.CapitalHeight = oldNum.CapitalHeight;
                    m_previewValue.CharGap = oldNum.CharGap;
                    m_previewValue.CharWidth = oldNum.CharWidth;
                    m_previewValue.CharHeight = oldNum.CharHeight;
                    m_previewValue.LineGap = oldNum.LineGap;
                    m_previewValue.SpaceSize = oldNum.SpaceSize;
                    m_previewValue.RefNumber = oldNum.RefNumber;
                    m_previewValue.SpecialType = oldNum.SpecialType;
                    m_previewValue.Left = oldNum.Left;
                    m_previewValue.Top = oldNum.Top;
                    m_previewValue.Width = oldNum.Width;
                    m_previewValue.Height = oldNum.Height;
                    m_previewValue.Rotate = oldNum.Rotate;

                    oldNum.FNTFile = fNTFile;
                    oldNum.ParaNumber = paraNumber;
                    oldNum.CapitalHeight = capitalHeight;
                    oldNum.CharGap = charGap;
                    oldNum.CharWidth = charWidth;
                    oldNum.CharHeight = charHeight;
                    oldNum.LineGap = lineGap;
                    oldNum.SpaceSize = spaceSize;
                    oldNum.RefNumber = refNumber;
                    oldNum.SpecialType = specialType;
                    oldNum.Left = left;
                    oldNum.Top = top;
                    oldNum.Width = width;
                    oldNum.Height = height;
                    oldNum.Rotate = rotate;
                }
                SaveChanged er = SaveChange;
                if (er != null) er();
            }
            catch 
            {
                MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");

                txtRefNo.Focus();
            }
        }
        public void TryAdd(ref MarkItem item)
        {
            //try
            //{
            //    string fNTFile = txtFontFile.Text;
            //    int paraNumber = Convert.ToInt32(txtParamNo.Text);
            //    double capitalHeight = Convert.ToDouble(txtCHeight.Text);
            //    double charGap = Convert.ToDouble(txtCGap.Text);
            //    double charWidth = Convert.ToDouble(txtWidth.Text);
            //    double charHeight = Convert.ToDouble(txtHeight.Text);
            //    double lineGap = Convert.ToDouble(txtLineGap.Text);
            //    double spaceSize = Convert.ToDouble(txtSpaceSize.Text);
            //    int refNumber = Convert.ToInt32(txtRefNo.Text);
            //    int specialType = Convert.ToInt32(txtSpecialType.Text);
            //    double left = Convert.ToDouble(this.location.txtLeft.Text);
            //    double top = Convert.ToDouble(this.location.txtTop.Text);
            //    double width = Convert.ToDouble(this.location.txtWidth.Text);
            //    double height = Convert.ToDouble(this.location.txtHeight.Text);



            //    NumberProperty oldNum = item.MarkInfo as NumberProperty;
            //    if (oldNum == null)
            //    {
            //        NumberProperty numValue = new NumberProperty();
            //        numValue.FNTFile = fNTFile;
            //        numValue.ParaNumber = paraNumber;
            //        numValue.CapitalHeight = capitalHeight;
            //        numValue.CharGap = charGap;
            //        numValue.CharWidth = charWidth;
            //        numValue.CharHeight = charHeight;
            //        numValue.LineGap = lineGap;
            //        numValue.SpaceSize = spaceSize;
            //        numValue.RefNumber = refNumber;
            //        numValue.SpecialType = specialType;
            //        numValue.Left = left;
            //        numValue.Top = top;
            //        numValue.Width = width;
            //        numValue.Height = height;
            //    }
            //    else
            //    {
            //        if (m_previewValue == null)
            //            m_previewValue = new NumberProperty();
            //        m_previewValue.FNTFile = oldNum.FNTFile;
            //        m_previewValue.ParaNumber = oldNum.ParaNumber;
            //        m_previewValue.CapitalHeight = oldNum.CapitalHeight;
            //        m_previewValue.CharGap = oldNum.CharGap;
            //        m_previewValue.CharWidth = oldNum.CharWidth;
            //        m_previewValue.CharHeight = oldNum.CharHeight;
            //        m_previewValue.LineGap = oldNum.LineGap;
            //        m_previewValue.SpaceSize = oldNum.SpaceSize;
            //        m_previewValue.RefNumber = oldNum.RefNumber;
            //        m_previewValue.SpecialType = oldNum.SpecialType;
            //        m_previewValue.Left = oldNum.Left;
            //        m_previewValue.Top = oldNum.Top;
            //        m_previewValue.Width = oldNum.Width;
            //        m_previewValue.Height = oldNum.Height;

            //        oldNum.FNTFile = fNTFile;
            //        oldNum.ParaNumber = paraNumber;
            //        oldNum.CapitalHeight = capitalHeight;
            //        oldNum.CharGap = charGap;
            //        oldNum.CharWidth = charWidth;
            //        oldNum.CharHeight = charHeight;
            //        oldNum.LineGap = lineGap;
            //        oldNum.SpaceSize = spaceSize;
            //        oldNum.RefNumber = refNumber;
            //        oldNum.SpecialType = specialType;
            //        oldNum.Left = left;
            //        oldNum.Top = top;
            //        oldNum.Width = width;
            //        oldNum.Height = height;

            //    }
            //}
            //catch
            //{
            //    MessageBox.Show("잘못 입력된 값이 존재합니다. 확인 바랍니다.", "Information");

            //    txtRefNo.Focus();
            //}
        }
        public void SetPreviewValue()
        {
            if (m_previewValue == null)
            {
                this.txtFontFile.Text = m_previewValue.FNTFile;
                this.cbParaNumber.SelectedIndex = m_previewValue.ParaNumber;
                this.txtCHeight.Text = m_previewValue.CapitalHeight.ToString();
                this.txtCGap.Text = m_previewValue.CharGap.ToString();
                this.txtWidth.Text = m_previewValue.CharWidth.ToString();
                this.txtHeight.Text = m_previewValue.CharHeight.ToString();
                this.txtLineGap.Text = m_previewValue.LineGap.ToString();
                this.txtSpaceSize.Text = m_previewValue.SpaceSize.ToString();
                this.txtRefNo.Text = m_previewValue.RefNumber.ToString();
                this.txtSpecialType.Text = m_previewValue.SpecialType.ToString();
                this.location.txtLeft.Text = m_previewValue.Left.ToString();
                this.location.txtTop.Text = m_previewValue.Top.ToString();
                this.location.txtWidth.Text = m_previewValue.Width.ToString();
                this.location.txtHeight.Text = m_previewValue.Height.ToString();
                this.location.txtRotate.Text = m_previewValue.Rotate.ToString();
            }
        }
        public void SetDefaultValue()
        {
        }
        public void Display(GraphicsBase settingValue, bool isdraw = false)
        {
            NumberProperty numProperty = settingValue.MarkInfo.MarkInfo as NumberProperty;
            if (numProperty != null)
            {
                scale = numProperty.CapitalHeight / numProperty.CharHeight;
                string[] f = numProperty.FNTFile.Split('\\');
                this.txtFontFile.Text = f[f.Length - 1];
                this.cbParaNumber.SelectedIndex = numProperty.ParaNumber;
                this.txtCHeight.Text = numProperty.CapitalHeight.ToString();
                this.txtCGap.Text = numProperty.CharGap.ToString();
                this.txtWidth.Text = numProperty.CharWidth.ToString();
                this.txtHeight.Text = numProperty.CharHeight.ToString();
                this.txtLineGap.Text = numProperty.LineGap.ToString();
                this.txtSpaceSize.Text = numProperty.SpaceSize.ToString();
                this.txtRefNo.Text = numProperty.RefNumber.ToString();
                this.txtSpecialType.Text = numProperty.SpecialType.ToString();

                if (isdraw)
                {
                    if (numProperty.Location)
                    {
                        this.location.rdLeft.IsChecked = true;
                        this.location.rdRight.IsChecked = false;

                    }
                    else
                    {
                        this.location.rdLeft.IsChecked = false;
                        this.location.rdRight.IsChecked = true;

                    }
                }
                this.location.SetParam(numProperty.Left, numProperty.Top, numProperty.Width, numProperty.Height, numProperty.Rotate, (int)settingValue.MarkInfo.MarkType.MarkType, true);
            }
        }       
    }
}
