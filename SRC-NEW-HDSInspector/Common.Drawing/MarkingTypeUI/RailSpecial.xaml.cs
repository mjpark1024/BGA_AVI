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
    /// RailSpecial.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RailSpecial : UserControl, IMarkingTypeUICommands
    {
        private RailSpecialProperty m_previewValue;
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
        private bool isDummy;
        //string para = "";
        public RailSpecial()
        {
            InitializeComponent();
            location.SetClick += location_SetClick;
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
                if (isDummy)
                    er(Convert.ToDouble(location.txtGX.Text), Convert.ToDouble(location.txtGY.Text), 0, 0, 0, eMarkingType.eMarkingRailSpecial);
                else
                    er(Convert.ToDouble(location.txtFX.Text), Convert.ToDouble(location.txtFY.Text),
                        Convert.ToDouble(location.txtWidth.Text), Convert.ToDouble(location.txtWidth.Text),
                        Convert.ToDouble(location.txtRotate.Text), eMarkingType.eMarkingRailSpecial);
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
                string pLTFile = "c:\\mime\\logo\\" + txtFontFile.Text;
                int paraNumber = cbParaNumber.SelectedIndex;
                double fx = Convert.ToDouble(this.location.txtFX.Text);
                double fy = Convert.ToDouble(this.location.txtFY.Text);
                double width = Convert.ToDouble(this.location.txtWidth.Text);
                double height = Convert.ToDouble(this.location.txtHeight.Text);
                double rotate = Convert.ToDouble(this.location.txtRotate.Text);
                int unitrow = Convert.ToInt32(txtUnitRow.Text);
                double gx, gy;
                if (isDummy)
                {
                    gx = Convert.ToDouble(this.location.txtGX.Text);
                    gy = Convert.ToDouble(this.location.txtGY.Text);
                }
                else
                {
                    gx = gy = 0;
                }
                RailSpecialProperty oldNum = graphic.MarkInfo.MarkInfo as RailSpecialProperty;
                if (oldNum == null)
                {
                    RailSpecialProperty numValue = new RailSpecialProperty();
                    numValue.PLTFile = pLTFile;
                    numValue.ParaNumber = paraNumber;
                    numValue.Left = 0;
                    numValue.Top = 0;
                    numValue.Width = width;
                    numValue.Height = height;
                    numValue.Rotate = rotate;
                    numValue.FirstX = fx;
                    numValue.FirstY = fy;
                    numValue.GapX = gx;
                    numValue.GapY = gy;
                }
                else
                {
                    if (m_previewValue == null)
                        m_previewValue = new RailSpecialProperty();
                    m_previewValue.PLTFile = oldNum.PLTFile;
                    m_previewValue.ParaNumber = oldNum.ParaNumber;
                    m_previewValue.Left = oldNum.Left;
                    m_previewValue.Top = oldNum.Top;
                    m_previewValue.Width = oldNum.Width;
                    m_previewValue.Height = oldNum.Height;
                    m_previewValue.Rotate = oldNum.Rotate;
                    m_previewValue.FirstX = oldNum.FirstX;
                    m_previewValue.FirstY = oldNum.FirstY;
                    m_previewValue.GapX = oldNum.GapX;
                    m_previewValue.GapY = oldNum.GapY;

                    oldNum.PLTFile = pLTFile;
                    oldNum.ParaNumber = paraNumber;
                    oldNum.Left = 0;
                    oldNum.Top = 0;
                    oldNum.Width = width;
                    oldNum.Height = height;
                    oldNum.Rotate = rotate;
                    oldNum.FirstX = fx;
                    oldNum.FirstY = fy;
                    oldNum.GapX = gx;
                    oldNum.GapY = gy;
                }
                graphic.UnitRow = unitrow;
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
                this.txtFontFile.Text = m_previewValue.PLTFile;
                this.cbParaNumber.SelectedIndex = m_previewValue.ParaNumber;
                this.location.txtFX.Text = m_previewValue.FirstX.ToString("f3");
                this.location.txtFY.Text = m_previewValue.FirstY.ToString("f3");
                this.location.txtWidth.Text = m_previewValue.Width.ToString("f3");
                this.location.txtHeight.Text = m_previewValue.Height.ToString("f3");
                this.location.txtRotate.Text = m_previewValue.Rotate.ToString("f3");
                this.location.txtGX.Text = m_previewValue.GapX.ToString("f3");
                this.location.txtGY.Text = m_previewValue.GapY.ToString("f3");
            }
        }
        public void SetDefaultValue()
        {
        }
        public void Display(GraphicsBase settingValue, bool isdraw = false)
        {
            isDummy = settingValue.Dummy;
            RailSpecialProperty railspecialProperty = settingValue.MarkInfo.MarkInfo as RailSpecialProperty;
            if (railspecialProperty != null)
            {
                string[] f = railspecialProperty.PLTFile.Split('\\');
                this.txtFontFile.Text = f[f.Length - 1];
                this.cbParaNumber.SelectedIndex = railspecialProperty.ParaNumber;
                if (isDummy)
                    this.location.SetParamDummy(railspecialProperty.GapX, railspecialProperty.GapY);
                else this.location.SetParamFirst(railspecialProperty.FirstX, railspecialProperty.FirstY, railspecialProperty.Width, railspecialProperty.Height, railspecialProperty.Rotate, false);
            }
            this.txtUnitRow.Text = settingValue.UnitRow.ToString();
            this.txtUnitCol.Text = settingValue.UnitColumn.ToString();
        }     
    }
}
