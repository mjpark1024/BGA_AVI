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
    /// IDMark.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IDMark : UserControl, IMarkingTypeUICommands
    {
        private IDMarkProperty m_previewValue;
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
        public IDMark()
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
                    Convert.ToDouble(location.txtRotate.Text), eMarkingType.eMarkingIDMark);
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
        public void TrySave(GraphicsBase graphic)
        {
            try
            {
                int paraNumber = cbParaNumber.SelectedIndex;
                double left = Convert.ToDouble(this.location.txtLeft.Text);
                double top = Convert.ToDouble(this.location.txtTop.Text);
                double width = Convert.ToDouble(this.location.txtWidth.Text);
                double height = Convert.ToDouble(this.location.txtHeight.Text);
                double rotate = Convert.ToDouble(this.location.txtRotate.Text);


                IDMarkProperty oldNum = graphic.MarkInfo.MarkInfo as IDMarkProperty;
                if (oldNum == null)
                {
                    IDMarkProperty numValue = new IDMarkProperty();
                    numValue.Left = left;
                    numValue.Top = top;
                    numValue.Width = width;
                    numValue.Height = height;
                    numValue.Rotate = rotate;
                }
                else
                {
                    if (m_previewValue == null)
                        m_previewValue = new IDMarkProperty();
                    m_previewValue.Left = oldNum.Left;
                    m_previewValue.Top = oldNum.Top;
                    m_previewValue.Width = oldNum.Width;
                    m_previewValue.Height = oldNum.Height;
                    m_previewValue.Rotate = oldNum.Rotate;
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
            }
        }

        public void LocationEnabled(bool IsEnable)
        {
            location.IsEnabled = IsEnable;
        }

        public void TryAdd(ref MarkItem item)
        {

        }
        public void SetPreviewValue()
        {
            if (m_previewValue == null)
            {
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
            IDMarkProperty weekProperty = settingValue.MarkInfo.MarkInfo as IDMarkProperty;
            if (weekProperty != null)
            {
                if (isdraw)
                {
                    if (weekProperty.Location == 1)
                    {
                        this.location.rdLeft.IsChecked = true;
                        this.location.rdRight.IsChecked = false;

                    }
                    else if (weekProperty.Location == 2)
                    {
                        this.location.rdRight.IsChecked = false;
                        this.location.rdLeft.IsChecked = true;
                    }
                }
                this.location.SetParam(weekProperty.Left, weekProperty.Top, weekProperty.Width, weekProperty.Height, weekProperty.Rotate, (int)settingValue.MarkInfo.MarkType.MarkType);
            }
        }
    }
}
