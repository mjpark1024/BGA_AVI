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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using Common.DataBase;
using System.Data;
using RMS.Generic.EquipmentManagement;

namespace ResultManagement
{
    /// <summary>   Result data management.  </summary>
    public partial class ResultDataManagement : UserControl
    {
        /// <summary>   Information about the delete.  </summary>
        public struct DelInfo
        {
            public string strMachineCode;
            public string strMachineName;
            public string strModelCode;
            public string strLotNo;
        }

        /// <summary> Information describing the list delete </summary>
        private List<DelInfo> m_listDelInfo = new List<DelInfo>();

        public ResultDataManagement()
        {
            InitializeComponent();
            InitializeDialog();
            InitializeEvent();
        }
        private void InitializeDialog()
        {
            this.txtDateStart.SelectedDate = DateTime.Today.AddDays(-7);
            this.txtDateEnd.SelectedDate = DateTime.Today;
            EquipmentManager equipMgr = new EquipmentManager();
            this.cmbMachine.ItemsSource = equipMgr.LoadAllEquipmentList();
            this.cmbMachine.DisplayMemberPath = "Name";
            this.cmbMachine.SelectedValuePath = "Code";
            this.cmbMachine.SelectedIndex = 0;
        }

        private void InitializeEvent()
        {
            this.btnDeleteAll.Click += new RoutedEventHandler(btnDeleteAll_Click);
            this.btnDeleteImage.Click += new RoutedEventHandler(btnDeleteImage_Click);

            this.txtDateStart.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(txtDateStart_SelectedDateChanged);
            this.txtDateEnd.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(txtDateEnd_SelectedDateChanged);
        }

        void txtDateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.txtDateEnd.SelectedDate != null)
            {
                int result = DateTime.Compare((DateTime)this.txtDateEnd.SelectedDate, DateTime.Today);
                if (result > 0)
                {
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("R008"), "Information", MessageBoxButton.OK);
                    this.txtDateEnd.SelectedDate = null;
                    return;
                }
            }

            if (this.txtDateStart.SelectedDate != null && this.txtDateEnd.SelectedDate != null)
            {
                int result = DateTime.Compare((DateTime)this.txtDateStart.SelectedDate, (DateTime)this.txtDateEnd.SelectedDate);
                if (result > 0)
                {
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("R008"), "Information", MessageBoxButton.OK);
                    this.txtDateEnd.SelectedDate = null;
                    return;
                }
            }
        }

        void txtDateStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.txtDateStart.SelectedDate != null)
            {
                int result = DateTime.Compare((DateTime)this.txtDateStart.SelectedDate, DateTime.Today);
                if (result > 0)
                {
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("R008"), "Information", MessageBoxButton.OK);
                    this.txtDateEnd.SelectedDate = null;
                    return;
                }
            }

            if (this.txtDateEnd.SelectedDate != null && this.txtDateStart.SelectedDate != null)
            {
                int result = DateTime.Compare((DateTime)this.txtDateStart.SelectedDate, (DateTime)this.txtDateEnd.SelectedDate);
                if (result > 0)
                {
                    MessageBox.Show(ResourceStringHelper.GetInformationMessage("R008"), "Information", MessageBoxButton.OK);
                    this.txtDateStart.SelectedDate = null;
                    return;
                }
            }
        }

        void btnDeleteImage_Click(object sender, RoutedEventArgs e)
        {

            bool bRet = true;
            if (this.txtDateEnd.SelectedDate == null)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("R007"), "Information", MessageBoxButton.OK);
                return;
            }

            string strMachine = cmbMachine.SelectedValue.ToString();
            string strDateStart = this.txtDateStart.Text + " 00:00:00";
            string strDateEnd = this.txtDateEnd.Text + " 23:59:59";

            SearchListLotNo(strMachine, strDateStart, strDateEnd);

            String strBasePath = Settings.GetSettings().General.ResultPath;
            string strPath = @"//" + Settings.GetSettings().SubSystem.FileServerIP + @"/";
            if (Settings.GetSettings().SubSystem.FileServerIP.Equals("127.0.0.1")
                    || Settings.GetSettings().SubSystem.FileServerIP.Equals("localhost"))
            {
                strPath = strBasePath;
            }
            else
            {
                int index = strBasePath.IndexOf(":/");

                if (index < 0)
                {
                    index = strBasePath.IndexOf(":");
                    if (index < 0)
                    {
                        strPath += "ImagePath";
                    }
                    else
                    {
                        strPath += strBasePath.Substring(index + 1, strBasePath.Length - (index + 1));
                    }
                }
                else
                {
                    strPath += strBasePath.Substring(index + 2, strBasePath.Length - (index + 2));
                }
            }
            foreach (DelInfo delInfo in m_listDelInfo)
            {
                string strImageFolerPath = strPath + @"/" +
                                            delInfo.strMachineName + @"/" +
                                            delInfo.strModelCode + @"/" +
                                            delInfo.strLotNo;

                bRet = DirectoryManager.DeleteDirectory(strImageFolerPath);
            }

            MessageBox.Show(ResourceStringHelper.GetInformationMessage("C002"), "Information", MessageBoxButton.OK);

        }

        void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            bool bRet = true;
            if (this.txtDateEnd.SelectedDate == null)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("R007"), "Information", MessageBoxButton.OK);
                return;
            }

            //이미지도 함께 삭제 하도록 한다....정보 삭제 되면 해당일자의 lot를 찾을 수 없으므로
            // 이미지도 삭제 됩니다 계속 하시겠습니까?
            if (MessageBox.Show(ResourceStringHelper.GetInformationMessage("R009"), "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.No)
                return;

            string strDateStart = this.txtDateStart.Text + " 00:00:00";
            string strDateEnd = this.txtDateEnd.Text + " 23:59:59";
            string strMachine = cmbMachine.SelectedValue.ToString();
            SearchListLotNo(strMachine, strDateStart, strDateEnd);

            String strBasePath = Settings.GetSettings().General.ResultPath;
            string strPath = @"//" + Settings.GetSettings().SubSystem.FileServerIP + @"/";
            if (Settings.GetSettings().SubSystem.FileServerIP.Equals("127.0.0.1")
                    || Settings.GetSettings().SubSystem.FileServerIP.Equals("localhost"))
            {
                strPath = strBasePath;
            }
            else
            {
                int index = strBasePath.IndexOf(":/");

                if (index < 0)
                {
                    index = strBasePath.IndexOf(":");
                    if (index < 0)
                    {
                        strPath += "ImagePath";
                    }
                    else
                    {
                        strPath += strBasePath.Substring(index + 1, strBasePath.Length - (index + 1));
                    }
                }
                else
                {
                    strPath += strBasePath.Substring(index + 2, strBasePath.Length - (index + 2));
                }
            }

            foreach (DelInfo delInfo in m_listDelInfo)
            {
                string strImageFolerPath = strPath + @"/" +
                                          delInfo.strMachineName + @"/" +
                                          delInfo.strModelCode + @"/" +
                                          delInfo.strLotNo;

                bRet = DirectoryManager.DeleteDirectory(strImageFolerPath);
            }
            if (ConnectFactory.DBConnector() != null)
                ConnectFactory.DBConnector().StartTrans();

            if (DeleteDefectResult(strDateStart, strDateEnd) >= 0)
            {
                if (DeleteInspectResultDetail(strDateStart, strDateEnd) >= 0)
                {
                    if (DeleteInspectResult(strDateStart, strDateEnd) >= 0)
                    {
                        if (DeleteLot(strDateStart, strDateEnd) >= 0)
                        {
                            ConnectFactory.DBConnector().Commit();
                            MessageBox.Show(ResourceStringHelper.GetInformationMessage("C002"), "Information", MessageBoxButton.OK);
                            return;
                        }
                        else
                            bRet = false;

                    }
                    else
                        bRet = false;
                }
                else
                    bRet = false;
            }
            else
                bRet = false;

            if (bRet == false)
            {
                MessageBox.Show(ResourceStringHelper.GetInformationMessage("C003"), "Information", MessageBoxButton.OK);
                ConnectFactory.DBConnector().Rollback();
            }
        }

        public int DeleteDefectResult(string strDateStart, string strDateEnd)
        {
            int nRet = 0;

            if (ConnectFactory.DBConnector() != null)
            {
                foreach (DelInfo delInfo in m_listDelInfo)
                {

                    string strQuery = String.Format(" DELETE FROM defect_result "
                    + "    WHERE machine_code = '{0}' "
                    + "    AND  model_code = '{1}' "
                    + "    AND  result_code in ( SELECT DISTINCT result_code FROM inspect_result "
                    + "                               WHERE machine_code = '{2}' and model_code ='{3}' and lot_no = '{4}') "
                   , delInfo.strMachineCode, delInfo.strModelCode, delInfo.strMachineCode, delInfo.strModelCode, delInfo.strLotNo);

                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return nRet;
        }

        public int DeleteInspectResultDetail(string strDateStart, string strDateEnd)
        {
            int nRet = 0;

            if (ConnectFactory.DBConnector() != null)
            {

                foreach (DelInfo delInfo in m_listDelInfo)
                {

                    string strQuery = String.Format(" DELETE FROM inspect_result_detail "
                    + "    WHERE machine_code = '{0}' "
                    + "    AND  model_code = '{1}' "
                    + "    AND  result_code in ( SELECT DISTINCT result_code FROM inspect_result "
                    + "                               WHERE machine_code = '{2}' and model_code ='{3}' and lot_no = '{4}') "
                   , delInfo.strMachineCode, delInfo.strModelCode, delInfo.strMachineCode, delInfo.strModelCode, delInfo.strLotNo);

                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return nRet;
        }

        public int DeleteInspectResult(string strDateStart, string strDateEnd)
        {
            int nRet = 0;

            if (ConnectFactory.DBConnector() != null)
            {


                foreach (DelInfo delInfo in m_listDelInfo)
                {

                    string strQuery = String.Format(" DELETE FROM inspect_result "
                    + "    WHERE machine_code = '{0}' "
                    + "    AND  model_code = '{1}' "
                    + "    AND  lot_no = '{2}' "
                   , delInfo.strMachineCode, delInfo.strModelCode, delInfo.strLotNo);

                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return nRet;
        }

        public int DeleteLot(string strDateStart, string strDateEnd)
        {
            int nRet = 0;

            if (ConnectFactory.DBConnector() != null)
            {
                foreach (DelInfo delInfo in m_listDelInfo)
                {

                    string strQuery = String.Format(" DELETE FROM lot_work "
                    + "    WHERE machine_code = '{0}' "
                    + "    AND  model_code = '{1}' "
                    + "    AND  lot_no = '{2}' "
                   , delInfo.strMachineCode, delInfo.strModelCode, delInfo.strLotNo);

                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }

            if (nRet >= 0)
            {
                foreach (DelInfo delInfo in m_listDelInfo)
                {

                    string strQuery = String.Format(" DELETE FROM lot_info "
                    + "    WHERE machine_code = '{0}' "
                    + "    AND  model_code = '{1}' "
                    + "    AND  lot_no = '{2}' "
                   , delInfo.strMachineCode, delInfo.strModelCode, delInfo.strLotNo);

                    nRet = ConnectFactory.DBConnector().Execute(strQuery);
                }
            }
            return nRet;
        }

        public void SearchListLotNo(string strMachine, string strDateStart, string strDateEnd)
        {

            DelInfo delInfo = new DelInfo();
            m_listDelInfo.Clear();
            if (ConnectFactory.DBConnector() != null)
            {

                string strQuery = String.Format(" SELECT a.machine_code, b.machine_name,a.model_code, a.lot_no FROM lot_info a, machine_info b "
                            + "             WHERE ( a.machine_code = '{0}' or '*' = '{1}') and ( a.reg_date BETWEEN '{2}' AND '{3}' ) AND a.machine_code = b.machine_code ",
                            strMachine, strMachine, strDateStart, strDateEnd);

                IDataReader dataReader = ConnectFactory.DBConnector().ExecuteQuery19(strQuery);

                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        delInfo.strMachineCode = dataReader.GetValue(0).ToString();
                        delInfo.strMachineName = dataReader.GetValue(1).ToString();
                        delInfo.strModelCode = dataReader.GetValue(2).ToString();
                        delInfo.strLotNo = dataReader.GetValue(3).ToString();

                        m_listDelInfo.Add(delInfo);
                    }
                    dataReader.Close();
                }
            }
        }
    }
}
