using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PCS.ELF.AVI;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// PitchSetWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PitchSetWindow : Window
    {
        private TeachingViewerCtrl m_ptrParentWindow;
        private ModelInformation m_currentModel;
        public PitchSetWindow(TeachingViewerCtrl parentWindow, ModelInformation currentModel, Point p, bool unit, int id)
        {
            InitializeComponent();
            m_ptrParentWindow = parentWindow;
            m_currentModel = currentModel;
            lblPitchX.Content = currentModel.Strip.UnitHeight.ToString("F2");
            lblPitchY.Content = currentModel.Strip.UnitWidth.ToString("F2");
            lblGap.Content = currentModel.Strip.BlockGap.ToString("F2");

            int nRow = (int)Math.Floor(currentModel.Strip.UnitRow / 2.0);

            if (id == 3)
            {
                if (currentModel.Strip.UnitRow % 2 == 1)
                    nRow++;
            }
            else
                nRow = currentModel.Strip.UnitRow;

            if (unit)
            {
                double PitchY = p.Y / m_ptrParentWindow.ReferenceImageScale * m_ptrParentWindow.CamResolutionY / 1000.0 / ((currentModel.Strip.UnitColumn / currentModel.Strip.Block) - 1);
                double PitchX = p.X / m_ptrParentWindow.ReferenceImageScale * m_ptrParentWindow.CamResolutionX / 1000.0 / (nRow - 1);
                lblPitchX2.Content = PitchX.ToString("F2");
                lblPitchY2.Content = PitchY.ToString("F2");
                lblGap2.Content = currentModel.Strip.BlockGap.ToString("F2");
            }
            else
            {
                double gap = p.Y / m_ptrParentWindow.ReferenceImageScale * m_ptrParentWindow.CamResolutionY / 1000.0 - currentModel.Strip.UnitWidth;
                lblPitchX2.Content = currentModel.Strip.UnitHeight.ToString("F2");
                lblPitchY2.Content = currentModel.Strip.UnitWidth.ToString("F2");
                lblGap2.Content = gap.ToString("F2");
            }
            this.btnOK.Click += new RoutedEventHandler(btnOK_Click);
            this.btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
            this.KeyDown += new KeyEventHandler(PitchSetWindow_KeyDown);
        }

        void PitchSetWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CloseWithOK();
            }
            else if (e.Key == Key.Escape)
            {
                CloseWithCancel();
            }
        }

        private void CloseWithCancel()
        {
            if (DialogResult == null || !(bool)DialogResult)
            {
                DialogResult = false;
            }
        }

        private void CloseWithOK()
        {
            try
            {
                double fUnitXPitch = Convert.ToDouble(lblPitchX2.Content);
                double fUnitYPitch = Convert.ToDouble(lblPitchY2.Content);
                double fBlockgap = Convert.ToDouble(lblGap2.Content);
                StripInformation currentStripInfo = m_currentModel.Strip;
                if (currentStripInfo.BlockGap != fBlockgap || currentStripInfo.UnitWidth != fUnitYPitch || currentStripInfo.UnitHeight != fUnitXPitch)
                {
                    // 90 degree Flip.
                    m_currentModel.Strip.BlockGap = fBlockgap;
                    m_currentModel.Strip.UnitWidth = fUnitYPitch;
                    m_currentModel.Strip.UnitHeight = fUnitXPitch;

                    if (m_ptrParentWindow.m_ptrTeachingWindow.IsOnLine) // Offline 모드에서는 실제 모델 데이터를 변경시키지 않도록 한다.
                    {
                        PCS.ELF.AVI.ModelManager.UpdateModelPitch(m_currentModel);
                    }
                }
                DialogResult = true;
            }
            catch
            {
                MessageBox.Show("잘못된 값이 있습니다.");
            }
        }


        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWithCancel();
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            CloseWithOK();
        }
    }
}
