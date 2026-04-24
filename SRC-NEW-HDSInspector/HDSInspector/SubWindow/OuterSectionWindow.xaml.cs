/*********************************************************************************
 * Copyright(c) 2015 by Haesung DS.
 * 
 * This software is copyrighted by, and is the sole property of Haesung DS.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Haesung DS. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Haesung DS reserves the right to modify this 
 * software without notice.
 *
 * Haesung DS.
 * KOREA 
 * http://www.HaesungDS.com
 *********************************************************************************/
/**
 * @file  TeachingHelperWindow.xaml.cs
 * @brief 
 *  Interaction logic for TeachingHelperWindow.xaml.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2015.05.27
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2015.05.27 First creation.
 */

using System;
using System.Windows;
using System.Windows.Input;
using PCS.ELF.AVI;
using Common.Drawing;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using Common;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// OuterSectionWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OuterSectionWindow : Window
    {
        #region Private Member Variables.
        private TeachingViewerCtrl m_ptrParentWindow;
        private ModelInformation m_currentModel;
        private DrawingCanvas m_currentCanvas;

        public SymmetryType SectionSymmetryType { get; private set; }
        public GraphicsRectangle CurrentROI { get; private set; }
        public int nType = 0;
        #endregion

        public OuterSectionWindow(TeachingViewerCtrl parentWindow, ModelInformation currentModel, DrawingCanvas currentCanvas, CategorySurface CategorySurface)
        {
            m_ptrParentWindow = parentWindow;
            m_currentModel = currentModel;
            m_currentCanvas = currentCanvas;
            if (m_currentCanvas != null && m_currentCanvas.SelectedGraphic != null)
            {
                CurrentROI = m_currentCanvas.SelectedGraphic as GraphicsRectangle;
                Debug.Assert(CurrentROI != null);
            }

            InitializeComponent();
            InitializeDialog();
            InitializeEvent();

            if (CategorySurface == CategorySurface.BP) this.rbColumn.Visibility = Visibility.Hidden;
            else this.rbColumn.Visibility = Visibility.Visible;

        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void InitializeDialog()
        {
            if (CurrentROI == null)
            {
                return;
            }
            else
            {
                // One Block 모델인 경우, 반복 설정 창에 Block 관련 내용을 담지 않는다.
                if (m_currentModel.Strip.Block <= 1)
                {
                    this.pnlBlockSetting.Visibility = Visibility.Hidden;
                    this.pnlBlockSetting.IsEnabled = false;
                    this.pnlBlockSetting.Height = 0;
                    this.Height -= 60;

                    txtUnitXPitch.Focus();
                }
                else
                {
                    txtBlockGap.Focus();
                }
                InitializeValue();
                rbNone.IsChecked = true;
            }
        }

        private void InitializeValue()
        {
            // 유의사항.
            // IS는 취득된 영상을 90도 회전하여 반환한다. 따라서 본 메서드에서는 모델 스펙 입력값에서의 Row를 Column으로, Column을 Row로 적용시킨다.
            lblUnitPitchX.Text = m_currentModel.Strip.UnitHeight.ToString("F2");
            lblUnitPitchY.Text = m_currentModel.Strip.UnitWidth.ToString("F2");
            lblBlockGap.Text = m_currentModel.Strip.BlockGap.ToString("F2");

            txtBlockGap.Text = string.Format("{0:f4}", m_currentModel.Strip.BlockGap);
            txtUnitXPitch.Text = string.Format("{0:F4}", m_currentModel.Strip.UnitHeight);
            txtUnitYPitch.Text = string.Format("{0:F4}", m_currentModel.Strip.UnitWidth);
        }

        private void InitializeEvent()
        {
            this.KeyDown += new KeyEventHandler(OuterSectionWindow_KeyDown);

            this.btnOK.Click += (s, e) => CloseWithOK();
            this.btnCancel.Click += (s, e) => CloseWithCancel();
            this.Closing += (s, e) => CloseWithCancel();
            this.txtBlockGap.TextInput += InputBox_TextInput;
            this.txtUnitXPitch.TextInput += InputBox_TextInput;
            this.txtUnitYPitch.TextInput += InputBox_TextInput;
        }

        #region Close Window.

        void OuterSectionWindow_KeyDown(object sender, KeyEventArgs e)
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

        private void CloseWithOK()
        {
            try
            {
                if ((bool)rbNone.IsChecked) nType = 0;
                if ((bool)rbColumn.IsChecked) nType = 1;
                if ((bool)rbRow.IsChecked) nType = 2;
                if (nType == 0)
                {
                    DialogResult = true;
                }
                if (!IsValidTextBoxValue(txtBlockGap) || !IsValidTextBoxValue(txtUnitXPitch) || !IsValidTextBoxValue(txtUnitXPitch))
                {
                    return;
                }
                double fBlockgap = Convert.ToDouble(txtBlockGap.Text) / m_ptrParentWindow.CamResolutionY * 1000 * m_ptrParentWindow.ReferenceImageScale;
                double fUnitXPitch = Convert.ToDouble(txtUnitXPitch.Text) / m_ptrParentWindow.CamResolutionX * 1000 * m_ptrParentWindow.ReferenceImageScale;
                double fUnitYPitch = Convert.ToDouble(txtUnitYPitch.Text) / m_ptrParentWindow.CamResolutionY * 1000 * m_ptrParentWindow.ReferenceImageScale;
                StripInformation currentStripInfo = m_currentModel.Strip;
                Debug.Assert(currentStripInfo != null);

                #region Check Block
                if (currentStripInfo.Block != 1 && fBlockgap <= 0.0)
                {
                    ShowExclamation("Block이 2개 이상인 경우 Pitch는 0보다 커야 합니다.");
                    txtBlockGap.Focus();
                    return;
                }

                #endregion

                #region Check Unit
                if (currentStripInfo.UnitColumn != 1 && fUnitXPitch <= 0.0)
                {
                    ShowExclamation("Unit이 2개 이상인 경우 Pitch는 0보다 커야 합니다.");
                    txtUnitXPitch.Focus();
                    return;
                }
                if (currentStripInfo.UnitRow != 1 && fUnitYPitch <= 0.0)
                {
                    ShowExclamation("Unit이 2개 이상인 경우 Pitch는 0보다 커야 합니다.");
                    txtUnitYPitch.Focus();
                    return;
                }
                #endregion

                CurrentROI.BlockIterationValue = new IterationInformation(currentStripInfo.Block, 1, 1, fBlockgap, 0, 0);
                CurrentROI.IterationValue = new IterationInformation(currentStripInfo.UnitRow, currentStripInfo.UnitColumn, fUnitXPitch, fUnitYPitch);

                int nResult = m_ptrParentWindow.BasedROICanvas.ValidIterationValue(CurrentROI, m_ptrParentWindow.TeachingImageSource.PixelWidth, m_ptrParentWindow.TeachingImageSource.PixelHeight);
                if (nResult < 0)
                {
                    if (nResult == -1 && nType == 1)
                    {
                        ShowExclamation("입력된 값이 기준 영상의 가로 길이를 벗어납니다.\nPitch 값을 낮추어 설정 바랍니다.");
                        if (pnlBlockSetting.Visibility == Visibility.Visible)
                        {
                            txtBlockGap.Focus();
                        }
                        else
                        {
                            txtUnitXPitch.Focus();
                        }
                        return;
                    }
                    else if (nResult == -2 && nType == 2)
                    {
                        ShowExclamation("입력된 값이 기준 영상의 세로 길이를 벗어납니다.\nPitch 값을 낮추어 설정 바랍니다.");
                        if (pnlBlockSetting.Visibility == Visibility.Visible)
                        {
                            txtBlockGap.Focus();
                        }
                        else
                        {
                            txtUnitXPitch.Focus();
                        }
                        return;
                    }

                    
                }

                
                CurrentROI.IsValidRegion = true;
                CurrentROI.IsFiducialRegion = true;
                CurrentROI.IterationXPosition = 0;
                CurrentROI.IterationYPosition = 0;
                CurrentROI.SymmetryValue.StartX = 1;
                CurrentROI.SymmetryValue.StartY = 1;
                switch (nType)
                {
                    case 0:
                        CurrentROI.SymmetryValue.JumpX = 0;
                        CurrentROI.SymmetryValue.JumpY = 0;
                        break;
                    case 1:
                        CurrentROI.SymmetryValue.JumpX = 1;
                        CurrentROI.SymmetryValue.JumpY = 0;
                        break;
                    case 2:
                        CurrentROI.SymmetryValue.JumpX = 0;
                        CurrentROI.SymmetryValue.JumpY = 1;
                        break;
                    default:
                        CurrentROI.SymmetryValue.JumpX = 0;
                        CurrentROI.SymmetryValue.JumpY = 0;
                        break;
                }

                fUnitXPitch = Convert.ToDouble(txtUnitXPitch.Text);
                fUnitYPitch = Convert.ToDouble(txtUnitYPitch.Text);
                fBlockgap = Convert.ToDouble(txtBlockGap.Text);
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
                ShowExclamation("잘못된 값을 입력하셨습니다. 확인 바랍니다.");
            }
        }

        private void CloseWithCancel()
        {
            if (DialogResult == null || !(bool)DialogResult)
            {
                if (CurrentROI != null &&
                    m_currentCanvas.GraphicsList.Contains(CurrentROI))
                {
                    m_currentCanvas.GraphicsList.Remove(CurrentROI);
                }
                DialogResult = false;
            }
        }
        #endregion

        #region About Exclamation.
        private void ShowExclamation(string aszWarnMessage)
        {
            pnlExclamation.Visibility = Visibility.Visible;
            txtWarnMessage.Text = aszWarnMessage;
        }

        private void HideExclamation()
        {
            pnlExclamation.Visibility = Visibility.Hidden;
        }

        private void InputBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            HideExclamation();
        }

        private bool IsValidTextBoxValue(TextBox textBox)
        {
            if (textBox == null || !(textBox is TextBox))
            {
                return false;
            }

            if (string.IsNullOrEmpty(textBox.Text))
            {
                ShowExclamation("값을 입력 바랍니다.");
                textBox.Focus();
                return false;
            }
            return true;
        }
        #endregion
    }
}
