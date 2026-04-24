using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Common.Drawing;

namespace HDSInspector
{
    /// <summary>
    /// MarkSubMenuCtrl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MarkSubMenuCtrl : UserControl
    {
        public event TeachingToolChangeEventHandler TeachingToolChangeEvent;
        public MarkSubMenuCtrl()
        {
            InitializeComponent();
            InitializeEvent();
        }

        private void InitializeEvent()
        {
            this.btnStripAlign.Click += btnTeachingTool_Click;

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
            this.btnStripAlign.IsChecked = false;

            #endregion

            switch (aToolType)
            {
                case ToolType.MarkStripAlign:
                    this.btnStripAlign.IsChecked = true;
                    break;

            }
        }

        // Section 선택에 따른 티칭 툴 활성 / 비활성화
        public void ChangeTeachingMode(TeachingType aTeachingType)
        {
            this.btnStripAlign.IsEnabled = true;

        }
    }
}
