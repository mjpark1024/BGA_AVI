/*********************************************************************************
 * Copyright(c) 2011,2012,2013 by Samsung Techwin.
 * 
 * This software is copyrighted by, and is the sole property of Samsung Techwin.
 * All rigths, title, ownership, or other interests in the software remain the
 * property of Samsung Techwin. This software may only be used in accordance with
 * the corresponding license agreement. Any unauthorized use, duplication, 
 * transmission, distribution, or disclosure of this software is expressly 
 * forbidden.
 *
 * This Copyright notice may not be removed or modified without prior written
 * consent of Samsung Techwin.Samsung Techwin reserves the right to modify this 
 * software without notice.
 *
 * Samsung Techwin.
 * KOREA 
 * http://www.samsungtechwin.co.kr
 *********************************************************************************/
/**
 * @file  TeachingIterationInfoWindow.xaml.cs
 * @brief 
 *  Interaction logic for TeachingIterationInfoWindow.xaml.
 * 
 * @author : Minseok Hwang <h.min-suck@samsung.com>
 * @date : 2011.09.23
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.09.23 First creation.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PCS.ELF.AVI;
using Common.Drawing;
using Common;

namespace STWInspector
{
    /// <summary>   Interaction logic for TeachingIterationInfoWindow.xaml. </summary>
    /// <remarks>   Minseok, Hwang, 2011-09-23. </remarks>
    public partial class TeachingIterationInfoWindow : Window
    {
        #region Private member variables.
        private TeachingViewerCtrl m_ptrParentWindow;
        private ModelInformation m_currentModel;
        private DrawingCanvas m_currentCanvas;
        public GraphicsRectangle CurrentROI
        {
            get;
            private set;
        }

        private IterationInformation m_oldIterationValue;
        private IterationInformation m_oldBlockIterationValue;
        #endregion

        public TeachingIterationInfoWindow(TeachingViewerCtrl parentWindow, ModelInformation currentModel, DrawingCanvas currentCanvas)
        {
            m_ptrParentWindow = parentWindow;
            m_currentModel = currentModel;
            m_currentCanvas = currentCanvas;

            GraphicsRectangle graphic = currentCanvas.SelectedGraphic as GraphicsRectangle;
            CurrentROI = (graphic.MotherROI == null) ? graphic : graphic.MotherROI as GraphicsRectangle;
            
            if (CurrentROI != null)
            {
                if (m_ptrParentWindow.SectionManager.Sections != null)
                {
                    if (m_ptrParentWindow.m_ptrTeachingViewer.SelectedViewer.BasedROICanvas.GraphicsList.Count != 0)
                    {
                        // 섹션정보에 저장되어 있던 값을 가져온다.
                        foreach (PCS.ModelTeaching.SectionInformation section in m_ptrParentWindow.SectionManager.Sections)
                        {
                            if (CurrentROI.ID == section.HashID)
                            {
                                if (CurrentROI.IsValidRegion)
                                {
                                    m_oldIterationValue = CurrentROI.IterationValue;
                                    m_oldBlockIterationValue = CurrentROI.BlockIterationValue;
                                }
                                else
                                {
                                    m_oldIterationValue = new IterationInformation(
                                        section.IterationCountX,
                                        section.IterationCountY,
                                        section.IterationPitchX,
                                        section.IterationPitchY);

                                    m_oldBlockIterationValue = new IterationInformation(
                                        section.BlockCountX,
                                        section.BlockCountY,
                                        section.BlockPitchX,
                                        section.BlockPitchY);

                                    CurrentROI.SymmetryValue.StartX = section.IterationStartX;
                                    CurrentROI.SymmetryValue.StartY = section.IterationStartY;
                                    CurrentROI.SymmetryValue.JumpX = section.IterationJumpX;
                                    CurrentROI.SymmetryValue.JumpY = section.IterationJumpY;
                                    CurrentROI.IsValidRegion = true;
                                }
                            }
                        }
                    }
                }

                if (m_oldIterationValue == null)
                {
                    m_oldIterationValue = CurrentROI.IterationValue;
                    m_oldBlockIterationValue = CurrentROI.BlockIterationValue;
                }
            }

            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.KeyDown += TeachingIterationInfoWindow_KeyDown;
            this.Loaded += TeachingIterationInfoWindow_Loaded;

            this.txtBlockColumn.TextInput += InputBox_TextInput;
            this.txtBlockRow.TextInput += InputBox_TextInput;
            this.txtBlockXPitch.TextInput += InputBox_TextInput;
            this.txtBlockYPitch.TextInput += InputBox_TextInput;
            this.txtUnitColumn.TextInput += InputBox_TextInput;
            this.txtUnitRow.TextInput += InputBox_TextInput;
            this.txtUnitXPitch.TextInput += InputBox_TextInput;
            this.txtUnitYPitch.TextInput += InputBox_TextInput;

            this.txtUnitStartX.ValueChanged += InputBox_ValueChanged;
            this.txtUnitStartY.ValueChanged += InputBox_ValueChanged;
            this.txtUnitJumpX.ValueChanged += InputBox_ValueChanged;
            this.txtUnitJumpY.ValueChanged += InputBox_ValueChanged;

            this.btnOK.Click += (s, e) => ApplyWindow();
            this.btnCancel.Click += (s, e) => CloseWindow();
            this.Closing += (s, e) =>
                {
                    CurrentROI.RegionType = GraphicsRegionType.UnitRegion;
                    CurrentROI.IsFiducialRegion = true;
                };
        }

        private void InputBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            HideExclamation();
        }

        private void InputBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            HideExclamation();
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
                if (m_currentModel.Strip.BlockColumn <= 1 && m_currentModel.Strip.BlockRow <= 1)
                {
                    this.pnlBlockSetting.Visibility = Visibility.Hidden;
                    this.pnlBlockSetting.IsEnabled = false;
                    this.pnlBlockSetting.Height = 0;
                    this.Height -= 120;

                    txtUnitColumn.Focus();
                }
                else
                {
                    txtBlockColumn.Focus();
                }
                InitializeValue();
            }
        }

        /// <summary>   Initializes the value. </summary>
        /// <remarks>   Minseok, Hwang, 2011-10-18. </remarks>
        private void InitializeValue()
        {
            // 유의사항.
            // IS는 취득된 영상을 90도 회전하여 반환한다. 따라서 본 메서드에서는 모델 스펙 입력값에서의 Row를 Column으로, Column을 Row로 적용시킨다.

            lblBlockColumn.Text = string.Format("{0}개", m_currentModel.Strip.BlockRow);
            lblBlockRow.Text = string.Format("{0}개", m_currentModel.Strip.BlockColumn);
            lblUnitColumn.Text = string.Format("{0}개", m_currentModel.Strip.UnitRow);
            lblUnitRow.Text = string.Format("{0}개", m_currentModel.Strip.UnitColumn);

            lblUnitPitchX.Text = m_currentModel.Strip.UnitHeight.ToString("F2");
            lblUnitPitchY.Text = m_currentModel.Strip.UnitWidth.ToString("F2");
            lblBlockPitchX.Text = m_currentModel.Strip.BlockHeight.ToString("F2");
            lblBlockPitchY.Text = m_currentModel.Strip.BlockWidth.ToString("F2");

            txtUnitColumn.MaxValue = m_currentModel.Strip.UnitRow;
            txtUnitRow.MaxValue = m_currentModel.Strip.UnitColumn;
            txtBlockColumn.MaxValue = m_currentModel.Strip.BlockRow;
            txtBlockRow.MaxValue = m_currentModel.Strip.BlockColumn;

            txtUnitStartX.Text = CurrentROI.SymmetryValue.StartX.ToString();
            txtUnitStartY.Text = CurrentROI.SymmetryValue.StartY.ToString();
            txtUnitJumpX.Text = CurrentROI.SymmetryValue.JumpX.ToString();
            txtUnitJumpY.Text = CurrentROI.SymmetryValue.JumpY.ToString();

            if (!CurrentROI.IsValidRegion) // Iteration Teaching이 완료되지 않은 ROI는 모델 스펙 값을 기본값으로 보여지게 한다.
            {
                txtBlockColumn.Text = m_currentModel.Strip.BlockRow.ToString();
                txtBlockRow.Text = m_currentModel.Strip.BlockColumn.ToString();
                txtBlockXPitch.Text = string.Format("{0:f4}", m_currentModel.Strip.BlockHeight);
                txtBlockYPitch.Text = string.Format("{0:f4}", m_currentModel.Strip.BlockWidth);
                
                txtUnitColumn.Text = m_currentModel.Strip.UnitRow.ToString();
                txtUnitRow.Text = m_currentModel.Strip.UnitColumn.ToString();
                txtUnitXPitch.Text = string.Format("{0:F4}", m_currentModel.Strip.UnitHeight);
                txtUnitYPitch.Text = string.Format("{0:F4}", m_currentModel.Strip.UnitWidth);
            }
            else
            {
                txtBlockColumn.Text = m_oldBlockIterationValue.Column.ToString();
                txtBlockRow.Text = m_oldBlockIterationValue.Row.ToString();
                txtBlockXPitch.Text = string.Format("{0:f4}", m_oldBlockIterationValue.XPitch * m_ptrParentWindow.CamResolutionX / 1000 / m_ptrParentWindow.ReferenceImageScale);
                txtBlockYPitch.Text = string.Format("{0:f4}", m_oldBlockIterationValue.YPitch * m_ptrParentWindow.CamResolutionY / 1000 / m_ptrParentWindow.ReferenceImageScale);

                txtUnitColumn.Text = m_oldIterationValue.Column.ToString();
                txtUnitRow.Text = m_oldIterationValue.Row.ToString();
                txtUnitXPitch.Text = string.Format("{0:f4}", m_oldIterationValue.XPitch * m_ptrParentWindow.CamResolutionX / 1000 / m_ptrParentWindow.ReferenceImageScale);
                txtUnitYPitch.Text = string.Format("{0:f4}", m_oldIterationValue.YPitch * m_ptrParentWindow.CamResolutionY / 1000 / m_ptrParentWindow.ReferenceImageScale);
            }
        }

        #region Event handlers.
        private void TeachingIterationInfoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentROI == null)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void TeachingIterationInfoWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                ApplyWindow();
            }
            else if (e.Key == Key.Escape)
            {
                CloseWindow();
            }
        }
        #endregion

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

        private bool IsValidTextBoxValue(Microsoft.Windows.Controls.IntegerUpDown upDownBox)
        {
            if (upDownBox == null || !(upDownBox is Microsoft.Windows.Controls.IntegerUpDown))
            {
                return false;
            }

            if (string.IsNullOrEmpty(upDownBox.Text))
            {
                ShowExclamation("값을 입력 바랍니다.");
                upDownBox.Focus();
                return false;
            }
            return true;
        }

        private void ApplyWindow()
        {
            try
            {
                if (IsValidTextBoxValue(txtUnitStartX) == false ||
                    IsValidTextBoxValue(txtUnitStartY) == false ||
                    IsValidTextBoxValue(txtUnitJumpX) == false ||
                    IsValidTextBoxValue(txtUnitJumpY) == false ||
                    IsValidTextBoxValue(txtUnitColumn) == false ||
                    IsValidTextBoxValue(txtUnitRow) == false ||
                    IsValidTextBoxValue(txtUnitXPitch) == false ||
                    IsValidTextBoxValue(txtUnitYPitch) == false ||
                    IsValidTextBoxValue(txtBlockColumn) == false ||
                    IsValidTextBoxValue(txtBlockRow) == false ||
                    IsValidTextBoxValue(txtBlockXPitch) == false ||
                    IsValidTextBoxValue(txtBlockYPitch) == false)
                {
                    return;
                }

                int nUnitStartX = Convert.ToInt32(txtUnitStartX.Text);
                int nUnitStartY = Convert.ToInt32(txtUnitStartY.Text);
                int nUnitJumpX = Convert.ToInt32(txtUnitJumpX.Text);
                int nUnitJumpY = Convert.ToInt32(txtUnitJumpY.Text);

                int nUnitColumn = Convert.ToInt32(txtUnitColumn.Text);
                int nUnitRow = Convert.ToInt32(txtUnitRow.Text);
                double fUnitXPitch = Convert.ToDouble(txtUnitXPitch.Text) / m_ptrParentWindow.CamResolutionX * 1000 * m_ptrParentWindow.ReferenceImageScale;
                double fUnitYPitch = Convert.ToDouble(txtUnitYPitch.Text) / m_ptrParentWindow.CamResolutionY * 1000 * m_ptrParentWindow.ReferenceImageScale;

                int nBlockColumn = Convert.ToInt32(txtBlockColumn.Text);
                int nBlockRow = Convert.ToInt32(txtBlockRow.Text);
                double fBlockXPitch = Convert.ToDouble(txtBlockXPitch.Text) / m_ptrParentWindow.CamResolutionX * 1000 * m_ptrParentWindow.ReferenceImageScale;
                double fBlockYPitch = Convert.ToDouble(txtBlockYPitch.Text) / m_ptrParentWindow.CamResolutionY * 1000 * m_ptrParentWindow.ReferenceImageScale;

                #region Check block iteration values.
                if (nBlockColumn > nUnitColumn)
                {
                    // M064 : Block 열 개수는 Unit 열의 개수를 초과할 수 없습니다.
                    ShowExclamation(ResourceStringHelper.GetInformationMessage("M064"));
                    txtBlockColumn.Focus();
                    return;
                }

                if (nBlockRow > nUnitRow)
                {
                    // M065 : Block 행 개수는 Unit 행의 개수를 초과할 수 없습니다.
                    ShowExclamation(ResourceStringHelper.GetInformationMessage("M065"));
                    txtBlockColumn.Focus();
                    return;
                }

                if (nBlockColumn <= 0)
                {
                    // M034 : Block은 최소 1개 이상이어야 합니다. (원맵 타입인 경우 1로 지정해 주십시오.)
                    ShowExclamation(ResourceStringHelper.GetInformationMessage("M034"));
                    txtBlockColumn.Focus();
                    return;
                }

                if (nBlockRow <= 0)
                {
                    // M034 : Block은 최소 1개 이상이어야 합니다. (원맵 타입인 경우 1로 지정해 주십시오.)
                    ShowExclamation(ResourceStringHelper.GetInformationMessage("M034"));
                    txtBlockRow.Focus();
                    return;
                }

                if (nBlockColumn != 1 && fBlockXPitch <= 0.0)
                {
                    ShowExclamation("Block이 2개 이상인 경우 Pitch는 0보다 커야 합니다.");
                    txtBlockXPitch.Focus();
                    return;
                }

                if (nBlockRow != 1 && fBlockYPitch <= 0.0)
                {
                    ShowExclamation("Block이 2개 이상인 경우 Pitch는 0보다 커야 합니다.");
                    txtBlockYPitch.Focus();
                    return;
                }
                #endregion

                CurrentROI.SymmetryValue = new IterationSymmetryInformation(nUnitStartX, nUnitStartY, nUnitJumpX, nUnitJumpY);
                CurrentROI.BlockIterationValue = new IterationInformation(nBlockColumn, nBlockRow, fBlockXPitch, fBlockYPitch);
                CurrentROI.IterationValue = new IterationInformation(nUnitColumn, nUnitRow, fUnitXPitch, fUnitYPitch);

                #region Check unit values.
                // 2011-11-01. HMS
                if (nUnitStartX <= 0)
                {
                    ShowExclamation("Unit 시작 열 번호에 1 이상의 값을 지정 바랍니다.");
                    txtUnitStartX.Focus();
                    return;
                }

                if (nUnitStartY <= 0)
                {
                    ShowExclamation("Unit 시작 행 번호에 1 이상의 값을 지정 바랍니다.");
                    txtUnitStartY.Focus();
                    return;
                }

                if (nUnitJumpX <= 0)
                {
                    ShowExclamation("Unit 가로 건너뛰기 횟수에 1 이상의 값을 지정 바랍니다.");
                    txtUnitJumpX.Focus();
                    return;
                }

                if (nUnitJumpY <= 0)
                {
                    ShowExclamation("Unit 세로 건너뛰기 횟수에 1 이상의 값을 지정 바랍니다.");
                    txtUnitJumpY.Focus();
                    return;
                }
                // End of modify.

                if (nUnitColumn <= 0)
                {
                    // M039 : Unit은 최소 1개 이상이어야 합니다.
                    ShowExclamation(ResourceStringHelper.GetInformationMessage("M039"));
                    txtUnitColumn.Focus();
                    return;
                }

                if (nUnitRow <= 0)
                {
                    // M039 : Unit은 최소 1개 이상이어야 합니다.
                    ShowExclamation(ResourceStringHelper.GetInformationMessage("M039"));
                    txtUnitRow.Focus();
                    return;
                }

                if (nUnitColumn != 1 && fUnitXPitch <= 0.0)
                {
                    ShowExclamation("Unit이 2개 이상인 경우 Pitch는 0보다 커야 합니다.");
                    txtUnitXPitch.Focus();
                    return;
                }

                if (nUnitRow != 1 && fUnitYPitch <= 0.0)
                {
                    ShowExclamation("Unit이 2개 이상인 경우 Pitch는 0보다 커야 합니다.");
                    txtUnitYPitch.Focus();
                    return;
                }


                // 2011-11.03 HMS Added.
                if (nUnitStartX + nUnitJumpX * (nUnitColumn - 1) > MainWindow.CurrentModel.Strip.UnitRow)
                {
                    ShowExclamation("설정된 Unit 가로 축 배열 개수가 모델 입력 값을 초과합니다.");
                    txtUnitStartX.Focus();
                    return;
                }

                if (nUnitStartY + nUnitJumpY * (nUnitRow - 1) > MainWindow.CurrentModel.Strip.UnitColumn)
                {
                    ShowExclamation("설정된 Unit 세로 축 배열 개수가 모델 입력 값을 초과합니다.");
                    txtUnitStartY.Focus();
                    return;
                }

                CurrentROI.BlockIterationValue = new IterationInformation(nBlockColumn, nBlockRow, fBlockXPitch, fBlockYPitch);
                CurrentROI.IterationValue = new IterationInformation(nUnitColumn, nUnitRow, fUnitXPitch, fUnitYPitch);

                int nResult = m_ptrParentWindow.BasedROICanvas.ValidIterationValue(CurrentROI, m_ptrParentWindow.TeachingImageSource.PixelWidth, m_ptrParentWindow.TeachingImageSource.PixelHeight);
                if (nResult < 0)
                {
                    if (nResult == -1)
                        ShowExclamation("입력된 값이 기준 영상의 가로 길이를 벗어납니다.\nPitch 값을 낮추어 설정 바랍니다.");
                    else if (nResult == -2)
                        ShowExclamation("입력된 값이 기준 영상의 세로 길이를 벗어납니다.\nPitch 값을 낮추어 설정 바랍니다.");

                    CurrentROI.IterationValue = m_oldIterationValue.Clone();
                    CurrentROI.BlockIterationValue = m_oldBlockIterationValue.Clone();

                    InitializeValue();
                    txtBlockColumn.Focus();

                    return;
                }
                #endregion

                // Minseok, Hwang. 2011-11-23 added.
                string strErr = IsOccupiedPosition(CurrentROI.SymmetryValue, CurrentROI.IterationValue);
                if (!string.IsNullOrEmpty(strErr))
                {
                    ShowExclamation(strErr);
                    txtUnitStartX.Focus();
                    return;
                }

                CurrentROI.IsValidRegion = true;

                fUnitXPitch = Convert.ToDouble(txtUnitXPitch.Text);
                fUnitYPitch = Convert.ToDouble(txtUnitYPitch.Text);
                fBlockXPitch = Convert.ToDouble(txtBlockXPitch.Text);
                fBlockYPitch = Convert.ToDouble(txtBlockYPitch.Text);
                if (m_currentModel.Strip.BlockWidth != fBlockYPitch ||
                    m_currentModel.Strip.BlockHeight != fBlockXPitch ||
                    m_currentModel.Strip.UnitWidth != fUnitYPitch ||
                    m_currentModel.Strip.UnitHeight != fUnitXPitch)
                {
                    // 90 degree Flip.
                    m_currentModel.Strip.BlockWidth = fBlockYPitch;
                    m_currentModel.Strip.BlockHeight = fBlockXPitch;
                    m_currentModel.Strip.UnitWidth = fUnitYPitch;
                    m_currentModel.Strip.UnitHeight = fUnitXPitch;

                    PCS.ELF.AVI.ModelManager.UpdateModelPitch(m_currentModel);
                }

                this.DialogResult = true;
                this.Close();

            }
            catch
            {
                ShowExclamation("잘못된 값을 입력하셨습니다. 확인 바랍니다.");
            }
        }

        /// <summary>   Query if Unit(X, Y) is occupied position. </summary>
        /// <remarks>   Minseok. Hwang, 2011-11-23. </remarks>
        private string IsOccupiedPosition(IterationSymmetryInformation symmetryValue, IterationInformation iterationValue)
        {
            // Section 반복 설정의 경우, 작업자의 입력에 의존하기 때문에 잘못된 설정 값 입력시
            // 동일 Unit(X,Y)가 중복되어 잘못된 결과가 리포팅 되는 경우가 발생할 수 있습니다.
            // 본 함수에서 그러한 오류를 미연에 필터링 합니다.

            int _row = 0;
            int _col = 0;
            int nUnitX = m_currentModel.Strip.UnitRow;
            int nUnitY = m_currentModel.Strip.UnitColumn;            
            bool[,] entireMap = new bool[nUnitY, nUnitX];
            bool[,] currentMap = new bool[nUnitY, nUnitX];

            // 0. 새로 추가하려는 Section에 의해 점유될 위치값을 기록한다.
            for (int row = 0; row < iterationValue.Row; row++)
            {
                for (int column = 0; column < iterationValue.Column; column++)
                {
                    _row = symmetryValue.StartY - 1 + (symmetryValue.JumpY * row);
                    _col = symmetryValue.StartX - 1 + (symmetryValue.JumpX * column);
                    currentMap[_row, _col] = true;
                }
            }

            foreach (GraphicsRectangle g in m_ptrParentWindow.BasedROICanvas.GraphicsRectangleList)
            {
                if (g.RegionType == GraphicsRegionType.UnitRegion)
                {
                    if (g.IsFiducialRegion) // 기준 Unit으로 설정된 ROI이면,
                    {
                        GraphicsRectangle rectGraphic = g; // iterator에 직접 access 불가하므로, 임시 변수 할당.
                        rectGraphic = (rectGraphic.MotherROI == null) ? rectGraphic : rectGraphic.MotherROI as GraphicsRectangle;
                        if (CurrentROI == rectGraphic) 
                            continue;

                        // 1. 기존의 Section과 새로 추가하려는 Section의 시작점이 같은 경우 오류를 리포팅한다.
                        if (rectGraphic.SymmetryValue.StartX == symmetryValue.StartX &&
                            rectGraphic.SymmetryValue.StartY == symmetryValue.StartY)
                        {
                            // 동일한 시작 위치를 갖는 Section이 존재합니다.
                            return ResourceStringHelper.GetErrorMessage("MT008");
                        }
                        else
                        {
                            // 2. 기존의 Section에 의해 점유당한 위치 값을 기록한다.
                            for (int row = 0; row < rectGraphic.IterationValue.Row; row++)
                            {
                                for (int column = 0; column < rectGraphic.IterationValue.Column; column++)
                                {
                                    _row = rectGraphic.SymmetryValue.StartY - 1 + (rectGraphic.SymmetryValue.JumpY * row);
                                    _col = rectGraphic.SymmetryValue.StartX - 1 + (rectGraphic.SymmetryValue.JumpX * column);
                                    entireMap[_row, _col] = true;
                                }
                            }
                        }
                    }
                }
            }

            // 3. 시작점이 아닌, 간격 값에 의해 발생한 위치 중복 오류를 리포팅한다.
            for (int row = 0; row < nUnitY; row++)
            {
                for (int column = 0; column < nUnitX; column++)
                {
                    if (entireMap[row, column] && currentMap[row, column])
                    {
                        // 동일한 위치를 점유하는 Section이 존재합니다. 1. 기존 Section의 반복 설정이 잘못 되었을 수 있습니다. 2. 현재 Section의 건너뛰기 설정이 잘못 되었을 수 있습니다.
                        return ResourceStringHelper.GetErrorMessage("MT009");
                    }
                }
            }

            return null; // Is not occupied position, can create unit region here.
        }

        private void ShowExclamation(string aszWarnMessage)
        {
            pnlExclamation.Visibility = Visibility.Visible;
            txtWarnMessage.Text = aszWarnMessage;
        }

        private void HideExclamation()
        {
            pnlExclamation.Visibility = Visibility.Hidden;
        }

        private void CloseWindow()
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
