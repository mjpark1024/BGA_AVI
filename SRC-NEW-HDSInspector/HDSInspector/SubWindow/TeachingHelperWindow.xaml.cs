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
 * @date : 2012.02.27
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2012.02.27 First creation.
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
using PCS;

namespace HDSInspector.SubWindow
{
    /// <summary>   Form for viewing the teaching helper.  </summary>
    /// <remarks>   suoow2, 2012-02-27. </remarks>
    public partial class TeachingHelperWindow : Window
    {
        #region Private Member Variables.
        private TeachingViewerCtrl m_ptrParentWindow;
        private ModelInformation m_currentModel;
        private DrawingCanvas m_currentCanvas;
        private int ID = 0;

        public SymmetryType SectionSymmetryType { get; private set; }
        public GraphicsRectangle CurrentROI { get; private set; }
        public bool Align = false;
        #endregion

        public TeachingHelperWindow(TeachingViewerCtrl parentWindow, ModelInformation currentModel, DrawingCanvas currentCanvas, int id)
        {
            m_ptrParentWindow = parentWindow;
            m_currentModel = currentModel;
            m_currentCanvas = currentCanvas;
            ID = id;
            if (m_currentCanvas != null && m_currentCanvas.SelectedGraphic != null)
            {
                CurrentROI = m_currentCanvas.SelectedGraphic as GraphicsRectangle;
                Debug.Assert(CurrentROI != null);
            }

            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
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
                SetSymmetry(TeachingWindow.Symmetry);
            }
            cbAlign.IsChecked = false;
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
            this.KeyDown += TeachingHelperWindow_KeyDown;

            this.btnMatrix.Click += btnSymmetry_Click;
            this.btnXFlip.Click += btnSymmetry_Click;
            this.btnYFlip.Click += btnSymmetry_Click;
            this.btnXYFlip.Click += btnSymmetry_Click;
            this.btnOtherPattern.Click += btnSymmetry_Click;

            this.btnOK.Click += (s, e) => CloseWithOK();
            this.btnCancel.Click += (s, e) => CloseWithCancel();
            this.Closing += (s, e) => CloseWithCancel();
            this.txtBlockGap.TextInput += InputBox_TextInput;
            this.txtUnitXPitch.TextInput += InputBox_TextInput;
            this.txtUnitYPitch.TextInput += InputBox_TextInput;
        }

        private void btnSymmetry_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton)
            {
                string szTag = ((ToggleButton)sender).Tag.ToString();
                if (!string.IsNullOrEmpty(szTag))
                {
                    btnMatrix.IsChecked = false;
                    btnXFlip.IsChecked = false;
                    btnYFlip.IsChecked = false;
                    btnXYFlip.IsChecked = false;
                    btnOtherPattern.IsChecked = false;

                    btnMatrix.Background = new SolidColorBrush(Colors.White);
                    btnXFlip.Background = new SolidColorBrush(Colors.White);
                    btnYFlip.Background = new SolidColorBrush(Colors.White);
                    btnXYFlip.Background = new SolidColorBrush(Colors.White);
                    btnOtherPattern.Background = new SolidColorBrush(Colors.White);

                    ButtonBorder1.Opacity = 0.6;
                    ButtonBorder2.Opacity = 0.6;
                    ButtonBorder3.Opacity = 0.6;
                    ButtonBorder4.Opacity = 0.6;
                    ButtonBorder5.Opacity = 0.6;

                    switch (szTag)
                    {
                        case "Matrix":
                            btnMatrix.IsChecked = true;
                            ButtonBorder1.Opacity = 0.9;
                            SectionSymmetryType = SymmetryType.Matrix;
                            btnMatrix.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                            break;
                        case "XFlip":
                            btnXFlip.IsChecked = true;
                            ButtonBorder2.Opacity = 0.9;
                            SectionSymmetryType = SymmetryType.XFlip;
                            btnXFlip.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                            break;
                        case "YFlip":
                            btnYFlip.IsChecked = true;
                            ButtonBorder3.Opacity = 0.9;
                            SectionSymmetryType = SymmetryType.YFlip;
                            btnYFlip.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                            break;
                        case "XYFlip":
                            btnXYFlip.IsChecked = true;
                            ButtonBorder4.Opacity = 0.9;
                            SectionSymmetryType = SymmetryType.XYFlip;
                            btnXYFlip.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                            break;
                        case "Unknown":
                            btnOtherPattern.IsChecked = true;
                            ButtonBorder5.Opacity = 0.9;
                            SectionSymmetryType = SymmetryType.Unknown;
                            btnOtherPattern.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                            break;
                        default:
                            btnMatrix.IsChecked = true;
                            ButtonBorder1.Opacity = 0.9;
                            SectionSymmetryType = SymmetryType.Matrix;
                            btnMatrix.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                            break;
                    }
                }
            }
        }

        private void SetSymmetry(SymmetryType anSymmetry)
        {
            switch (anSymmetry)
            {
                case SymmetryType.Matrix:
                    btnMatrix.IsChecked = true;
                    ButtonBorder1.Opacity = 0.9;
                    SectionSymmetryType = SymmetryType.Matrix;
                    btnMatrix.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                    break;
                case SymmetryType.XFlip:
                    btnXFlip.IsChecked = true;
                    ButtonBorder2.Opacity = 0.9;
                    SectionSymmetryType = SymmetryType.XFlip;
                    btnXFlip.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                    break;
                case SymmetryType.YFlip:
                    btnYFlip.IsChecked = true;
                    ButtonBorder3.Opacity = 0.9;
                    SectionSymmetryType = SymmetryType.YFlip;
                    btnYFlip.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                    break;
                case SymmetryType.XYFlip:
                    btnXYFlip.IsChecked = true;
                    ButtonBorder4.Opacity = 0.9;
                    SectionSymmetryType = SymmetryType.XYFlip;
                    btnXYFlip.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                    break;
                case SymmetryType.Unknown:
                    btnOtherPattern.IsChecked = true;
                    ButtonBorder5.Opacity = 0.9;
                    SectionSymmetryType = SymmetryType.Unknown;
                    btnOtherPattern.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                    break;
                default:
                    btnMatrix.IsChecked = true;
                    ButtonBorder1.Opacity = 0.9;
                    SectionSymmetryType = SymmetryType.Matrix;
                    btnMatrix.Background = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
                    break;
            }
        }

        #region Close Window.
        private void TeachingHelperWindow_KeyDown(object sender, KeyEventArgs e)
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

                int nRow = (int)Math.Floor(currentStripInfo.UnitRow / 2.0);
                if (ID == VID.BP1)
                {

                }
                else if (ID == (VID.BP1+1))
                {
                    if (currentStripInfo.UnitRow % 2 == 1)
                        nRow++;
                }
                else
                    nRow = currentStripInfo.UnitRow;

                CurrentROI.BlockIterationValue = new IterationInformation(currentStripInfo.Block, 1, 1, fBlockgap, 0, 0);
                CurrentROI.IterationValue = new IterationInformation(nRow, currentStripInfo.UnitColumn, fUnitXPitch, fUnitYPitch);

                int nResult = m_ptrParentWindow.BasedROICanvas.ValidIterationValue(CurrentROI, m_ptrParentWindow.TeachingImageSource.PixelWidth, m_ptrParentWindow.TeachingImageSource.PixelHeight);
                if (nResult < 0)
                {
                    if (nResult == -1)
                        ShowExclamation("입력된 값이 기준 영상의 가로 길이를 벗어납니다.\nPitch 값을 낮추어 설정 바랍니다.");
                    else if (nResult == -2)
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

                CurrentROI.IsValidRegion = true;
                CurrentROI.IsFiducialRegion = true;
                CurrentROI.IterationXPosition = 0;
                CurrentROI.IterationYPosition = 0;
                CurrentROI.SymmetryValue.StartX = 1;
                CurrentROI.SymmetryValue.StartY = 1;
                switch (SectionSymmetryType)
                {
                    case SymmetryType.Matrix:
                        CurrentROI.SymmetryValue.JumpX = 1;
                        CurrentROI.SymmetryValue.JumpY = 1;
                        TeachingWindow.Symmetry = SymmetryType.Matrix;
                        break;
                    case SymmetryType.XFlip:
                        CurrentROI.SymmetryValue.JumpX = 2;
                        CurrentROI.SymmetryValue.JumpY = 1;
                        break;
                    case SymmetryType.YFlip:
                        CurrentROI.SymmetryValue.JumpX = 1;
                        CurrentROI.SymmetryValue.JumpY = 2;
                        break;
                    case SymmetryType.XYFlip:
                        CurrentROI.SymmetryValue.JumpX = 2;
                        CurrentROI.SymmetryValue.JumpY = 2;
                        break;
                    case SymmetryType.Unknown:
                    default:
                        CurrentROI.SymmetryValue.JumpX = 1;
                        CurrentROI.SymmetryValue.JumpY = 1;
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
                Align = !(bool)cbAlign.IsChecked;
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
