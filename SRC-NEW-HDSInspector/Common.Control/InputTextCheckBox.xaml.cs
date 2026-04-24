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
 * @file  InputTextCheckBox.xaml.cs
 * @brief
 *  behind code of InputTextCheckBox.xaml
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.08.02
 * @version : 1.1
 * 
 * <b> Revision Histroy </b>
 * - 2011.08.02 First creation.
 * - 2011.08.10 Some functions added.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Common.Control
{
    /// <summary>   Input text check box.  </summary>
    /// <remarks>   suoow2, 2014-08-02. </remarks>
    public partial class InputTextCheckBox : Window
    {
        #region Member variables.
        private SolidColorBrush m_redBrush = new SolidColorBrush(Colors.Red);
        private SolidColorBrush m_blueBrush = new SolidColorBrush(Colors.Blue);
        private List<string> m_listStoredName;
        public string m_strInputMessage = string.Empty;
        #endregion

        #region Constructors.
        public InputTextCheckBox()
            : this(string.Empty, string.Empty, string.Empty, null) { }

        public InputTextCheckBox(string strTitle, string strCaption, string strInputText, List<string> listStoredName)
        {
            // listStoredName : 중복 문자열을 비교하는 대상입니다.
            // 기존에 저장되어 있던 문자열 중에서 'Inspector'가 포함되어 있었다면 사용자는 'Inspector'를 입력할 수 없습니다.
            if (listStoredName != null)
            {
                int nLength = listStoredName.Count;
                for (int i = 0; i < nLength; i++)
                {
                    listStoredName[i] = GetSpaceDeletedText(listStoredName[i]);
                }
                m_listStoredName = listStoredName;
            }
            else
            {
                m_listStoredName = new List<string>();
            }

            InitializeComponent();
            InitializeDialog(strTitle, strCaption, strInputText);
            InitializeEvent();
        }
        #endregion

        #region Initialize Dialog & Events.
        private void InitializeDialog(string strTitle, string strCaption, string strInputText)
        {
            this.Title = strTitle;
            this.lblCaption.Content = strCaption;
            this.btnOK.IsEnabled = false;
            this.txtInputBox.Focus();

            if (!string.IsNullOrEmpty(strInputText))
            {
                this.txtInputBox.Text = strInputText;
                CheckDuplicateInputText();
            }
        }

        /// <summary>   Initializes the event. </summary>
        /// <remarks>   suoow2, 2014-08-02. </remarks>
        private void InitializeEvent()
        {
            this.btnOK.Click += btnOK_Click;
            this.btnCancel.Click += btnCancel_Click;
            this.txtInputBox.TextChanged += txtInputBox_TextChanged;
            this.KeyDown += InputTextCheckBox_KeyDown;
        }

        private void CheckDuplicateInputText()
        {
            bool IsDuplicate = false;

            string inputText = GetSpaceDeletedText(txtInputBox.Text);

            // 입력 값이 비어있는 경우 확인 버튼을 비활성 상태로 바꾼다.
            if (string.IsNullOrEmpty(inputText))
            {
                lblHelpMessage.Content = string.Empty;
                this.btnOK.IsEnabled = false;
            }
            else // 문자열 리스트로부터 입력된 문자열이 중복 값인지, 아닌지를 판정한다.
            {
                foreach (string str in m_listStoredName)
                {
                    if (inputText.ToLower() == str.ToLower())
                    {
                        IsDuplicate = true;
                        break;
                    }
                }

                if (IsDuplicate) // 입력 문자열이 중복된 경우 등록할 수 없다.
                {
                    lblHelpMessage.Foreground = m_redBrush;
                    lblHelpMessage.Content = "시스템에 등록되어 있는 이름입니다.";
                    this.btnOK.IsEnabled = false;
                }
                else if (txtInputBox.Text == "." || txtInputBox.Text == "..")
                {
                    lblHelpMessage.Foreground = m_redBrush;
                    lblHelpMessage.Content = ResourceStringHelper.GetInformationMessage("M021");
                    this.btnOK.IsEnabled = false;
                }
                else if (IsContainsSpecialChars())
                {
                    lblHelpMessage.Foreground = m_redBrush;
                    lblHelpMessage.Content = ResourceStringHelper.GetInformationMessage("M062");
                    this.btnOK.IsEnabled = false;
                }
                else if (IsSystemVariable())
                {
                    lblHelpMessage.Foreground = m_redBrush;
                    lblHelpMessage.Content = ResourceStringHelper.GetInformationMessage("M063");
                    this.btnOK.IsEnabled = false;
                }
                else
                {
                    lblHelpMessage.Foreground = m_blueBrush;
                    lblHelpMessage.Content = "사용할 수 있습니다.";
                    this.btnOK.IsEnabled = true;
                }
            }
        }
        #endregion

        #region Event handlers.
        private void txtInputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckDuplicateInputText();
        }

        private void InputTextCheckBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter || e.Key == Key.Space) && btnOK.IsEnabled)
            {
                // DialogResult = true;
                CloseWindowWithInputMessage();
            }
            else if (e.Key == Key.Escape)
            {
                // DialogResult = false;
                CloseWindow();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            CloseWindowWithInputMessage();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
        #endregion

        #region Close functions.
        private void CloseWindowWithInputMessage()
        {
            m_strInputMessage = GetSpaceDeletedText(this.txtInputBox.Text);
            this.DialogResult = true;
        }

        private void CloseWindow()
        {
            this.DialogResult = false;
        }
        #endregion

        #region Validation rules.
        /// <summary>   Query if this object is not system variable. </summary>
        /// <remarks>   suoow2, 2014-09-14. </remarks>
        /// <returns>   true if not system variable, false if not. </returns>
        public bool IsSystemVariable()
        {
            switch(txtInputBox.Text)
            {
                case "CON":
                case "PRN":
                case "AUX":
                case "NUL":
                case "COM1":
                case "COM2":
                case "COM3":
                case "COM4":
                case "COM5":
                case "COM6":
                case "COM7":
                case "COM8":
                case "COM9":
                case "LPT1":
                case "LPT2":
                case "LPT3":
                case "LPT4":
                case "LPT5":
                case "LPT6":
                case "LPT7":
                case "LPT8":
                case "LPT9": return true;
            }
            return false;
        }

        /// <summary>   Query if this object is contains special characters. </summary>
        /// <remarks>   suoow2, 2014-09-14. </remarks>
        /// <returns>   true if contains special characters, false if not. </returns>
        public bool IsContainsSpecialChars()
        {
            if (txtInputBox.Text.IndexOfAny(new char[] { '/', ':', '*', '?', '"', '<', '>', '|' }) != -1)
            {
                return true; // invalid DIR-name.
            }
            else if (txtInputBox.Text.IndexOf(@"\") != -1 || txtInputBox.Text.IndexOf("'") != -1)
            {
                return true; // invalid DIR-name.
            }

            return false;
        }

        /// <summary>   Gets a space deleted text. </summary>
        /// <remarks>   suoow2, 2014-10-16. </remarks>
        /// <param name="strText">  The string text. </param>
        /// <returns>   The space deleted text. </returns>
        public string GetSpaceDeletedText(string strText)
        {
            strText = strText.TrimStart(' ');
            strText = strText.TrimEnd(' ');

            return strText;
        }
        #endregion
    }
}
