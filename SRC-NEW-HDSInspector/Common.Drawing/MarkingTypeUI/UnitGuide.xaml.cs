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
    /// UnitGuide.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UnitGuide : UserControl, IMarkingTypeUICommands
    {
        private UnitGuideProperty m_previewValue;
        public event MoveEventHandler MoveClick;
        public event SizeChangeEventHandler MarkSizeChanged;
        public event LocationChangeEventHandler LocationChanged;
        public event PenParamChanged ParamChange;
        public event SaveChanged SaveChange;
        private double Resolution;
        public UnitGuide()
        {
            InitializeComponent();
            location.SetClick += location_SetClick;
            location.MoveClick += location_MoveClick;
        }
        void location_MoveClick(int nid, double pitch)
        {
            MoveEventHandler er = MoveClick;
            if (er != null) er(nid, pitch, eMarkingType.eUnitGuide);
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
                er(Convert.ToDouble(location.txtX.Text), Convert.ToDouble(location.txtY.Text),
                   0, 0, 0, eMarkingType.eUnitGuide);
            }
        }

        public void SetDialog(double resolution, eMarkingType markType)
        {
            this.Resolution = resolution;
        }
        public void TrySave(GraphicsBase graphic)
        {
            try
            {
                double x = Convert.ToDouble(this.location.txtX.Text);
                double y = Convert.ToDouble(this.location.txtY.Text);

                UnitGuideProperty oldNum = graphic.MarkInfo.MarkInfo as UnitGuideProperty;
                if (oldNum == null)
                {
                    UnitGuideProperty numValue = new UnitGuideProperty();
                    numValue.StartX = x;
                    numValue.StartY = y;
                }
                else
                {
                    if (m_previewValue == null)
                        m_previewValue = new UnitGuideProperty();
                    m_previewValue.StartX = oldNum.StartX;
                    m_previewValue.StartY = oldNum.StartY;

                    oldNum.StartX = x;
                    oldNum.StartY = y;
                }
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
                this.location.txtX.Text = m_previewValue.StartX.ToString();
                this.location.txtY.Text = m_previewValue.StartY.ToString();
            }
        }
        public void SetDefaultValue()
        {
        }

        public void Display(GraphicsBase settingValue, bool isdraw = false)
        {
            UnitGuideProperty unitguideProperty = settingValue.MarkInfo.MarkInfo as UnitGuideProperty;
            if (unitguideProperty != null)
            {
                this.location.Set(unitguideProperty.StartX, unitguideProperty.StartY);
            }
        }
    }
}
