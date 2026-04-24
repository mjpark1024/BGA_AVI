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
    /// UnitRect.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UnitRect : UserControl, IMarkingTypeUICommands
    {
        private UnitRectProperty m_previewValue;
        public event MoveEventHandler MoveClick;
        public event SizeChangeEventHandler MarkSizeChanged;
        public event LocationChangeEventHandler LocationChanged;
        public event PenParamChanged ParamChange;
        public event SaveChanged SaveChange;
        MarkLogo logo;
        string para = "";
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
        public UnitRect()
        {
            InitializeComponent();
            logo = new MarkLogo();
            this.Loaded += UnitRect_Loaded;
            location.SetClick += location_SetClick;
            location.MoveClick += location_MoveClick;
            cbParaNumber.SelectionChanged += cbParaNumber_SelectionChanged;
        }

        void UnitRect_Loaded(object sender, RoutedEventArgs e)
        {
            cbPLT.Items.Clear();
            for (int i = 0; i < logo.UnitRect.Count; i++)
            {
                cbPLT.Items.Add(logo.UnitRect[i]);
            }
            cbPLT.SelectedItem = para;
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
            if (er!=null) er(nid, pitch, eMarkingType.eMarkingUnitRect);
        }

        public void LocationEnabled(bool IsEnable)
        {
            location.IsEnabled = IsEnable;
        }

        void location_SetClick()
        {
            SizeChangeEventHandler er = MarkSizeChanged;
            if (er != null)
            {
                er(Convert.ToDouble(location.txtLeft.Text), Convert.ToDouble(location.txtTop.Text),
                    Convert.ToDouble(location.txtWidth.Text), Convert.ToDouble(location.txtHeight.Text), 
                    Convert.ToDouble(location.txtRotate.Text), eMarkingType.eMarkingUnitRect);
            }
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
                string pLTFile = "c:\\mime\\logo\\" + cbPLT.Text;
                int paraNumber = cbParaNumber.SelectedIndex;
                double left = Convert.ToDouble(this.location.txtLeft.Text);
                double top = Convert.ToDouble(this.location.txtTop.Text);
                double width = Convert.ToDouble(this.location.txtWidth.Text);
                double height = Convert.ToDouble(this.location.txtHeight.Text);
                double rotate = Convert.ToDouble(this.location.txtRotate.Text);

                UnitRectProperty oldNum = graphic.MarkInfo.MarkInfo as UnitRectProperty;
                if (oldNum == null)
                {
                    UnitRectProperty numValue = new UnitRectProperty();
                    numValue.PLTFile = pLTFile;
                    numValue.ParaNumber = paraNumber;
                    numValue.Left = left;
                    numValue.Top = top;
                    numValue.Width = width;
                    numValue.Height = height;
                    numValue.Rotate = rotate;
                }
                else
                {
                    if (m_previewValue == null)
                        m_previewValue = new UnitRectProperty();
                    m_previewValue.PLTFile = oldNum.PLTFile;
                    m_previewValue.ParaNumber = oldNum.ParaNumber;
                    m_previewValue.Left = oldNum.Left;
                    m_previewValue.Top = oldNum.Top;
                    m_previewValue.Width = oldNum.Width;
                    m_previewValue.Height = oldNum.Height;
                    m_previewValue.Rotate = oldNum.Rotate;

                    oldNum.PLTFile = pLTFile;
                    oldNum.ParaNumber = paraNumber;
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

               // txtParaNumber.Focus();
            }
        }
        public void TryAdd(ref MarkItem item)
        {
        }
        public void SetPreviewValue()
        {
            if (m_previewValue == null)
            {
                string[] s = m_previewValue.PLTFile.Split('\\');
                this.cbPLT.SelectedItem = s[s.Length - 1]; 
                this.cbParaNumber.SelectedIndex = m_previewValue.ParaNumber;
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
            UnitRectProperty unitrectProperty = settingValue.MarkInfo.MarkInfo as UnitRectProperty;
            if (unitrectProperty != null)
            {
                string[] s = unitrectProperty.PLTFile.Split('\\');
                cbPLT.IsSynchronizedWithCurrentItem = true;
                this.cbPLT.SelectedItem = s[s.Length - 1];
                this.cbPLT.Refresh();
                para = s[s.Length - 1];
                this.cbParaNumber.SelectedIndex = unitrectProperty.ParaNumber;
                this.location.SetParam(unitrectProperty.Left, unitrectProperty.Top, unitrectProperty.Width, unitrectProperty.Height, unitrectProperty.Rotate, (int)settingValue.MarkInfo.MarkType.MarkType);
            }
        }
    }
}
