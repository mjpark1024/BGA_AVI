using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Common.Drawing.MarkingInformation;
using Common.DataBase;
using System.Text;

namespace Common.Drawing.MarkingTypeUI
{
    /// <summary>
    /// Week.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Week : UserControl, IMarkingTypeUICommands
    {
        private WeekProperty m_previewValue;
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
        #endregion
        public Week()
        {
            InitializeComponent();
            location.SetClick += location_SetClick;
            location.LocationSetClick += location_SetClick;
            location.MoveClick += location_MoveClick;
            cbParaNumber.SelectionChanged += cbParaNumber_SelectionChanged;
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
                    Convert.ToDouble(location.txtRotate.Text), eMarkingType.eMarkingWeek);
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
                double charWidth = Convert.ToDouble(txtWidth.Text);
                double charHeight = Convert.ToDouble(txtHeight.Text);
                double lineGap = Convert.ToDouble(txtLineGap.Text);
                double spaceSize = Convert.ToDouble(txtSpaceSize.Text);
                int refNumber = Convert.ToInt32(txtRefNo.Text);
                int specialType = Convert.ToInt32(txtSpecialType.Text);
                double left = Convert.ToDouble(this.location.txtLeft.Text);
                double top = Convert.ToDouble(this.location.txtTop.Text);
                double width = Convert.ToDouble(this.location.txtWidth.Text);
                double height = Convert.ToDouble(this.location.txtHeight.Text);
                double rotate = Convert.ToDouble(this.location.txtRotate.Text);


                WeekProperty oldNum = graphic.MarkInfo.MarkInfo as WeekProperty;
                if (oldNum == null)
                {
                    WeekProperty numValue = new WeekProperty();
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
                        m_previewValue = new WeekProperty();
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
        public void Display(GraphicsBase settingValue)
        {
            
        }
        public void Display(GraphicsBase settingValue, bool isdraw = false)
        {
            WeekProperty weekProperty = settingValue.MarkInfo.MarkInfo as WeekProperty;
            if (weekProperty != null)
            {
                string[] f = weekProperty.FNTFile.Split('\\');
                this.txtFontFile.Text = f[f.Length - 1];
                this.cbParaNumber.SelectedIndex = weekProperty.ParaNumber;
                this.txtCHeight.Text = weekProperty.CapitalHeight.ToString();
                this.txtCGap.Text = weekProperty.CharGap.ToString();
                this.txtWidth.Text = weekProperty.CharWidth.ToString();
                this.txtHeight.Text = weekProperty.CharHeight.ToString();
                this.txtLineGap.Text = weekProperty.LineGap.ToString();
                this.txtSpaceSize.Text = weekProperty.SpaceSize.ToString();
                this.txtRefNo.Text = weekProperty.RefNumber.ToString();
                this.txtSpecialType.Text = weekProperty.SpecialType.ToString();

                if (isdraw)
                {
                    if (weekProperty.Location == 0)
                    {
                        this.location.rdLeft.IsChecked = true;
                        this.location.rdCenter.IsChecked = false;
                        this.location.rdRight.IsChecked = false;

                    }
                    else if (weekProperty.Location == 1)
                    {
                        this.location.rdLeft.IsChecked = false;
                        this.location.rdCenter.IsChecked = false;
                        this.location.rdRight.IsChecked = true;

                    }
                    else
                    {
                        this.location.rdRight.IsChecked = false;
                        this.location.rdCenter.IsChecked = true;
                        this.location.rdLeft.IsChecked = false;
                    }
                }
                this.location.SetParam(weekProperty.Left, weekProperty.Top, weekProperty.Width, weekProperty.Height, weekProperty.Rotate, (int)settingValue.MarkInfo.MarkType.MarkType, true, true);
            }
        }
    }
}
