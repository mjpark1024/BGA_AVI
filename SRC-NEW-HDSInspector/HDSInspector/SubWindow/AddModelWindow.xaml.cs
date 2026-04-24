using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HDSInspector.SubWindow
{
    /// <summary>
    /// AddModelWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddModelWindow : Window
    {
        public string MdoelName;
        public string Code;
        private List<string> m_listStoredName;
        public AddModelWindow(List<string> models)
        {
            InitializeComponent();
            if (models != null)
            {
                int nLength = models.Count;
                for (int i = 0; i < nLength; i++)
                {
                    models[i] = GetSpaceDeletedText(models[i]);
                }
                m_listStoredName = models;
            }
            else
            {
                m_listStoredName = new List<string>();
            }
            this.btnOK.Click += BtnOK_Click;
            this.btnCancel.Click += BtnCancel_Click;
            btnOK.IsEnabled = false;
        }

        public string GetSpaceDeletedText(string strText)
        {
            strText = strText.TrimStart(' ');
            strText = strText.TrimEnd(' ');

            return strText;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            bool IsDuplicate = false;
            Name = GetSpaceDeletedText(txtModelName.Text);
            Code = txtModelCode.Text.Trim();
            if (string.IsNullOrEmpty(Name))
            {
                this.btnOK.IsEnabled = false;
                txtModelName.Focus();
                return;
            }
            else // 문자열 리스트로부터 입력된 문자열이 중복 값인지, 아닌지를 판정한다.
            {
                foreach (string str in m_listStoredName)
                {
                    if (Name.ToLower() == str.ToLower())
                    {
                        IsDuplicate = true;
                        break;
                    }
                }

                if (IsDuplicate) // 입력 문자열이 중복된 경우 등록할 수 없다.
                {
                    MessageBox.Show("이미 등록된 모델입니다.");

                    this.btnOK.IsEnabled = false;
                    txtModelName.Focus();
                    return;
                }
                else if (Name == "." || Name == ".." || IsContainsSpecialChars() || IsSystemVariable())
                {
                    MessageBox.Show("사용 할 수 없는 문자가 존재 합니다.");
                    this.btnOK.IsEnabled = false;
                    txtModelName.Focus();
                    return;
                }
            }
            this.DialogResult = true;
        }

        public bool IsSystemVariable()
        {
            switch (txtModelName.Text)
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
            if (txtModelName.Text.IndexOfAny(new char[] { '/', ':', '*', '?', '"', '<', '>', '|' }) != -1)
            {
                return true; // invalid DIR-name.
            }
            else if (txtModelName.Text.IndexOf(@"\") != -1 || txtModelName.Text.IndexOf("'") != -1)
            {
                return true; // invalid DIR-name.
            }

            return false;
        }
    }
}
