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
    /// UnitCircle.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UnitCircle : UserControl, IMarkingTypeUICommands
    {
        private UnitCircleProperty m_previewValue;
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
        public UnitCircle()
        {
            InitializeComponent();
            logo = new MarkLogo();
            this.Loaded += UnitCircle_Loaded;
            location.SetClick += location_SetClick;
            location.MoveClick += location_MoveClick;
            cbParaNumber.SelectionChanged += cbParaNumber_SelectionChanged;
        }

        void UnitCircle_Loaded(object sender, RoutedEventArgs e)
        {
            cbPLT.Items.Clear();
            for (int i = 0; i < logo.UnitEllipse.Count; i++)
            {
                cbPLT.Items.Add(logo.UnitEllipse[i]);
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
            if (er != null) er(nid, pitch, m_enumMarkType);
        }

        void location_SetClick()
        {
            SizeChangeEventHandler er = MarkSizeChanged;
            if (er != null)
            {
                er(Convert.ToDouble(location.txtLeft.Text), Convert.ToDouble(location.txtTop.Text),
                    Convert.ToDouble(location.txtRadian.Text), Convert.ToDouble(location.txtRadian.Text),
                    Convert.ToDouble(location.txtRotate.Text), eMarkingType.eMarkingUnitCircle);
            }
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
                string pLTFile = "c:\\mime\\logo\\" + cbPLT.Text;
                int paraNumber = cbParaNumber.SelectedIndex;
                double left = Convert.ToDouble(this.location.txtLeft.Text);
                double top = Convert.ToDouble(this.location.txtTop.Text);
                double radian = Convert.ToDouble(this.location.txtRadian.Text);
                double rotate = Convert.ToDouble(this.location.txtRotate.Text);

                UnitCircleProperty oldNum = graphic.MarkInfo.MarkInfo as UnitCircleProperty;
                if (oldNum == null)
                {
                    UnitCircleProperty numValue = new UnitCircleProperty();
                    numValue.PLTFile = pLTFile;
                    numValue.ParaNumber = paraNumber;
                    numValue.Left = left;
                    numValue.Top = top;
                    numValue.Width = radian;
                    numValue.Height = radian;
                    numValue.Rotate = rotate;
                }
                else
                {
                    if (m_previewValue == null)
                        m_previewValue = new UnitCircleProperty();
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
                    oldNum.Width = Width;
                    oldNum.Height = radian;
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
            UnitCircleProperty unitcircleProperty = settingValue.MarkInfo.MarkInfo as UnitCircleProperty;
            if (unitcircleProperty != null)
            {
                string[] s = unitcircleProperty.PLTFile.Split('\\');
                this.cbPLT.SelectedItem = s[s.Length - 1];
                this.cbPLT.Refresh();
                para = s[s.Length - 1];
                this.cbParaNumber.SelectedIndex = unitcircleProperty.ParaNumber;
                this.location.SetParam(unitcircleProperty.Left, unitcircleProperty.Top, unitcircleProperty.Width, unitcircleProperty.Height, unitcircleProperty.Rotate, (int)settingValue.MarkInfo.MarkType.MarkType);
            }
        }
    }
}
