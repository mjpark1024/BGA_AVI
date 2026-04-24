using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Common.Drawing;

namespace HDSInspector
{
    /// <summary>   Values that represent TeachingType.  </summary>
    /// <remarks>   suoow2, 2014-09-22. </remarks>
    public enum TeachingType
    {
        NONE = 0,
        Entire = 1,
        StripAlign = 2,
        OuterRegion = 3,
        UnitRegion = 4,
        RawRegion = 5,
        PsrRegion = 6
    }

    public delegate void TeachingToolChangeEventHandler(string strToolType);

    public partial class TeachingSubMenu : UserControl
    {
        public event TeachingToolChangeEventHandler TeachingToolChangeEvent;

        public TeachingSubMenu()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnPointer.Click += btnTeachingTool_Click;
            this.btnPannning.Click += btnTeachingTool_Click;
            this.btnRawmetrial.Click += btnTeachingTool_Click;
            this.btnPSROdd.Click += btnTeachingTool_Click;
            this.btnRectangle.Click += btnTeachingTool_Click;
            this.btnOuter.Click += btnTeachingTool_Click;
            this.btnEllipse.Click += btnTeachingTool_Click;
            this.btnPolyLine.Click += btnTeachingTool_Click;
            this.btnAlign.Click += btnTeachingTool_Click;
            this.btnStripAlign.Click += btnTeachingTool_Click;
            this.btnLine.Click += btnTeachingTool_Click;
            this.btnIDMark.Click += btnTeachingTool_Click;
            this.btnGuideLine.Click += btnTeachingTool_Click;
            this.btnWPShift.Click += btnTeachingTool_Click;
            this.btnCopyAndPaste.Click += btnTeachingTool_Click;
        }

        private void btnTeachingTool_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton)
            {
                TeachingToolChanged(((ToggleButton)sender).Tag.ToString());
            }
            else if (sender is Button)
            {
                TeachingToolChanged(((Button)sender).Tag.ToString());
            }
        }

        private void TeachingToolChanged(string aszTag)
        {
            if (!string.IsNullOrEmpty(aszTag))
            {
                TeachingToolChangeEventHandler eventRunner = TeachingToolChangeEvent;
                if (eventRunner != null)
                {
                    eventRunner(aszTag);
                }
            }
        }

        public void ToolTypeChanged(ToolType aToolType)
        {
            #region Disable tool-type buttons.
            this.btnPointer.IsChecked = false;
            this.btnPannning.IsChecked = false;
            this.btnEllipse.IsChecked = false;
            this.btnRectangle.IsChecked = false;
            this.btnPSROdd.IsChecked = false;
            this.btnOuter.IsChecked = false;
            this.btnPolyLine.IsChecked = false;
            this.btnAlign.IsChecked = false;
            this.btnStripAlign.IsChecked = false;
            this.btnLine.IsChecked = false;
            this.btnGuideLine.IsChecked = false;
            this.btnIDMark.IsChecked = false;
            this.btnRawmetrial.IsChecked = false;
            this.btnWPShift.IsChecked = false;
            this.btnCopyAndPaste.IsChecked = false;
            #endregion

            switch (aToolType)
            {
                case ToolType.Pointer:
                    this.btnPointer.IsChecked = true;
                    break;
                case ToolType.Move:
                    this.btnPannning.IsChecked = true;
                    break;
                case ToolType.Ellipse:
                    this.btnEllipse.IsChecked = true;
                    break;
                case ToolType.Rectangle:
                    this.btnRectangle.IsChecked = true;
                    break;
                case ToolType.Outer:
                    this.btnOuter.IsChecked = true;
                    break;
                case ToolType.Rawmetrial:
                    this.btnRawmetrial.IsChecked = true;
                    break;
                case ToolType.PSROdd:
                    this.btnPSROdd.IsChecked = true;
                    break;
                case ToolType.PolyLine:
                    this.btnPolyLine.IsChecked = true;
                    break;
                case ToolType.AlignPattern:
                    this.btnAlign.IsChecked = true;
                    break;
                case ToolType.StripOrigin:
                    this.btnStripAlign.IsChecked = true;
                    break;
                case ToolType.WPShift:
                    this.btnWPShift.IsChecked = true;
                    break;
                case ToolType.Line:
                    this.btnLine.IsChecked = true;
                    break;
                case ToolType.GuideLine:
                    this.btnGuideLine.IsChecked = true;
                    break;
                case ToolType.IDMark:
                    this.btnIDMark.IsChecked = true;
                    break;
                case ToolType.CopyAndPaste:
                    this.btnCopyAndPaste.IsChecked = true;
                    break;
            }
        }

        // Section 선택에 따른 티칭 툴 활성 / 비활성화
        public void ChangeTeachingMode(TeachingType aTeachingType, bool PSR_odd = true)
        {
            switch (aTeachingType)
            {
                case TeachingType.Entire:           // 전체 영상 티칭
                    lblRectangle.Content = "유닛";
                    btnOuter.Visibility = Visibility.Visible;
                    btnStripAlign.Visibility = Visibility.Visible;
                    btnAlign.Visibility = Visibility.Hidden;
                    btnPolyLine.Visibility = Visibility.Hidden;
                    btnLine.Visibility = Visibility.Visible;
                    btnWPShift.Visibility = Visibility.Hidden;
                    btnIDMark.Visibility = Visibility.Visible;
                    btnGuideLine.Visibility = Visibility.Hidden;
                    btnCopyAndPaste.Visibility = Visibility.Hidden;
                    btnRawmetrial.Visibility = Visibility.Visible;
                    btnEllipse.Visibility = Visibility.Hidden;
                    if(PSR_odd) btnPSROdd.Visibility = Visibility.Visible;
                    else btnPSROdd.Visibility = Visibility.Hidden;
                    btnPolyLine.IsEnabled = false;
                    btnEllipse.IsEnabled = false;
                    break;

                case TeachingType.StripAlign:       // Strip Align 섹션 티칭
                    lblRectangle.Content = "사각형";
                    btnOuter.Visibility = Visibility.Hidden;
                    btnAlign.Visibility = Visibility.Visible;
                    btnStripAlign.Visibility = Visibility.Hidden;
                    btnPolyLine.Visibility = Visibility.Visible;
                    btnLine.Visibility = Visibility.Visible;
                    btnWPShift.Visibility = Visibility.Hidden;
                    btnIDMark.Visibility = Visibility.Hidden;
                    btnGuideLine.Visibility = Visibility.Hidden;
                    btnCopyAndPaste.Visibility = Visibility.Hidden;
                    btnRawmetrial.Visibility = Visibility.Hidden;
                    btnPSROdd.Visibility = Visibility.Hidden;
                    btnEllipse.Visibility = Visibility.Visible;
                    btnPolyLine.IsEnabled = false;
                    btnEllipse.IsEnabled = false;
                    break;

                case TeachingType.UnitRegion:       // Unit 섹션 티칭
                case TeachingType.PsrRegion:
                case TeachingType.OuterRegion:      // Outer 섹션 티칭
                case TeachingType.RawRegion:
                    lblRectangle.Content = "사각형";
                    btnOuter.Visibility = Visibility.Hidden;
                    btnAlign.Visibility = Visibility.Visible;
                    btnStripAlign.Visibility = Visibility.Hidden;
                    btnPolyLine.Visibility = Visibility.Visible;
                    btnLine.Visibility = Visibility.Hidden;
                    btnWPShift.Visibility = Visibility.Visible;
                    btnIDMark.Visibility = Visibility.Visible;
                    btnGuideLine.Visibility = Visibility.Visible;
                    btnCopyAndPaste.Visibility = Visibility.Visible;
                    btnRawmetrial.Visibility = Visibility.Hidden;
                    btnPSROdd.Visibility = Visibility.Hidden;
                    btnEllipse.Visibility = Visibility.Visible;
                    btnPolyLine.IsEnabled = true;
                    btnEllipse.IsEnabled = true;
                    break;
            }
        }
    }
}
