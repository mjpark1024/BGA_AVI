using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PCS.ELF.AVI;
using Common;
using RMS.Generic.UserManagement;
using Common.DataBase;
using System.Data;

namespace HDSInspector.SubWindow
{
    public partial class GoldenLoadWindow : Window
    {
        public PCS.ELF.AVI.ModelManager GoldenModelManager = new PCS.ELF.AVI.ModelManager();
        private List<string> m_listStoredModelName = new List<string>();
        private string m_szMachineType;
        public bool bExistModel = false;

        public GoldenLoadWindow(string aszMachineType)
        {
            InitializeComponent();
            InitializeEvent();
            InitializeDialog(aszMachineType);
        }

        private void InitializeDialog(string aszMachineType)
        {
            m_szMachineType = aszMachineType;

            lbGroup.DataContext = GoldenModelManager.Groups;
            lbModel.DataContext = GoldenModelManager.Models;
            lbHistory.DataContext = GoldenModelManager.History;

            GoldenModelManager.LoadGoldenGroup();
        }

        private void InitializeEvent()
        {
            this.btnOK.Click += btnOK_Click;
            this.btnCancel.Click += btnCancel_Click;

            lbGroup.SelectionChanged += lbGroup_SelectionChanged;
            lbModel.SelectionChanged += lbModel_SelectionChanged;
        }

        private void ChangeModel()
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
            ModelInformation selectedModel = lbModel.SelectedItem as ModelInformation;

            if (selectedGroup == null)
            {
                return;
            }

            if (selectedModel == null)
            {
                this.txtModelName.Text = "선택된 모델이 없습니다.";
                this.pnlModelSpec.DataContext = new ModelInformation();
            }
            else
            {
                this.txtModelName.Text = selectedModel.Name;
                this.pnlModelSpec.DataContext = selectedModel;
            }
        }

        void lbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;
            if (selectedGroup == null)
            {
                return;
            }
            else
            {
               // GoldenModelManager.LoadGoldenModel(selectedGroup.Name);
                if (GoldenModelManager.Models.Count > 0)
                {
                    this.lbModel.DataContext = GoldenModelManager.Models;
                    this.lbModel.SelectedIndex = 0;
                }
            }
        }

        void lbModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModelInformation selectedModel = lbModel.SelectedItem as ModelInformation;
            if (selectedModel == null)
            {
                if (lbModel.HasItems)
                {
                    lbModel.SelectedIndex = 0;
                    ChangeModel();
                }
            }
            else
            {
                ChangeModel();

                // History 가져오기                
                if (m_szMachineType != "")
                    GoldenModelManager.LoadGoldenHistory(m_szMachineType, selectedModel.Name);

                if (GoldenModelManager.History.Count > 0)
                {
                    lbHistory.SelectedIndex = GoldenModelManager.History.Count - 1;
                    txtNoResult.Visibility = Visibility.Hidden;
                    txtNoResultSub.Visibility = Visibility.Hidden;
                }
                else
                {
                    txtNoResult.Visibility = Visibility.Visible;
                    txtNoResultSub.Visibility = Visibility.Visible;
                }
            }
        }

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Group selectedGroup = lbGroup.SelectedItem as Group;            
            ModelInformation selectedModel = lbModel.SelectedItem as ModelInformation;

            if (selectedGroup == null || selectedModel == null)
            {
                MessageBox.Show("선택된 모델이 없습니다.", "Information");
                return;
            }
            else
            {
                #region 1.모델이 이미 존재하는지 확인
                PCS.ELF.AVI.ModelManager.LoadUsedModelNames(ref m_listStoredModelName);
                bExistModel = false;
                foreach (string modelName in m_listStoredModelName)
                {
                    if (selectedModel.Name == modelName)
                    {
                        bExistModel = true;
                        break;
                    }
                }
                #endregion

                if (bExistModel)
                {
                    // 2.있으면 덮어쓸지 확인 
                    if (MessageBox.Show("선택한 모델이 이미 존재합니다. 덮어쓰시겠습니까?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        #region 3. 로컬에 XML로 해당모델 티칭정보 백업
                        MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                        mainWindow.TeachingDlg.InitializeDialog();
                        string szBackupInfo = string.Format("Setter-{0}, (골든 덮어쓰기 백업-{1})", UserManager.CurrentUser.Name, DateTime.Now.ToString().Replace(':', '-'));
                        if (!DirectoryManager.ValidateDirectoryName(szBackupInfo))
                            szBackupInfo = DateTime.Now.ToString().Replace(':', '-');

                        for (int nIndex = 0; nIndex < mainWindow.TeachingDlg.TeachingViewers.Length; nIndex++)
                        {
                            mainWindow.TeachingDlg.TeachingViewers[nIndex].ExportTeachingData(DirectoryManager.GetModelBackupPath(selectedGroup.Name, selectedModel.Name, szBackupInfo), false);
                        }
                        #endregion
                    }
                    else
                        return;
                }
                
                // 4. Download function 쿼리 실행
                if(!DownloadGoldenModel(selectedModel.Name))
                {
                    MessageBox.Show("골든 모델을 불러오는데 실패하였습니다.", "Error");
                    return;
                }

                // 5. Group use_yn = 1
                PCS.ELF.AVI.ModelManager.SetGroupNameUse(selectedGroup.Name);

                this.DialogResult = true;
                this.Close();
            }
        }

        private bool DownloadGoldenModel(string aszModelName)
        {
            if (ConnectFactory.DBConnector() != null)
            {
                String strQuery = String.Format("Select bgadb.`fn_Download`('{0}', '{1}', now(), '{2}')",
                                  MainWindow.Setting.General.MachineCode, aszModelName, UserManager.CurrentUser.ID);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);
                if (dataReader == null)
                    return false;

                if (dataReader.Read())
                {
                    int Ret = Convert.ToInt32(dataReader.GetValue(0).ToString());
                    dataReader.Close();

                    if (Ret > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
    }
}
