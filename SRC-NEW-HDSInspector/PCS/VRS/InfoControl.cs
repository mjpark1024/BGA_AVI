using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Input;
using Common.DataBase;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using PCS.ITS;
using Sockets;

namespace PCS.VRS
{   
    public class InfoControl
    {
        public class JobHistoryInfo
        {
            public string strProdOrder;         //양산오더
            public string strLocalOrder;           //재검시 오더 (Directory명)
            public string strItsOrder;          //ITS 오더
            public string strStdModel;          //표준 모델명
            public string strOpcode;            //공정코드
            public string strMachine;           //설비코드
            public string strLocalModel;        //설비내 모델명 - 경로 확인용
            public string strLocalGroup;        //설비내 그룹명 - 경로 확인용
            public string strCreateTime;        //검사 시작시간
            public double dYield;               //수율
            public State nState;                //검사 상태
        }

        //검사 공정 공용으로 쓰는 Enum, 함부로 변경 X - Harry
        public enum State
        {
            Idle = 0,
            Inspect = 1,
            InspectPause = 2,
            InspectDone = 3,
            OfflineVeri = 4,
            OfflineVeriDone = 5,
            OfflineVeriSkip = 6
        };

        private static ClientSyncSocket SyncSocket;

        public static bool RemoteConnect(string IP)
        {
            SyncSocket = new ClientSyncSocket();
            try
            {
                SyncSocket.Connect(IP, 14000);
            }
            catch// (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool CreateConnectDB(String astrIP, String astrPort, String astrDBName)
        {
            try
            {
                String strCon = String.Format("server={0}; user id={1}; password={2}; database={3}; port={4}; pooling=false", astrIP, "root", "mysql", astrDBName, astrPort);
                ConnectITS.CreateConnection(strCon);
                return ConnectITS.Open();
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
                return false;
            }
        }

        #region Job History Management
        //완료 처리되지 않은 로트는 강제로 완료처리한다.
        private static void JobInspCheck(string strMC)
        {
            try
            {
                String query = String.Format("SELECT IDX FROM job_info WHERE MACHINE='{0}' AND (STATE={1} OR STATE={2})", strMC, (int)State.Inspect, (int)State.InspectPause);
                List<string> list = new List<string>();

                IDataReader dataReader = ConnectITS.DBConnector().ExecuteQuery19(query);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        string idx = dataReader.GetValue(0).ToString();
                        list.Add(idx);
                    }
                    dataReader.Close();
                }

                foreach (string idx in list)
                {
                    query = String.Format("UPDATE job_info SET STATE={0} WHERE IDX={1}", (int)State.InspectDone, idx);

                    ConnectITS.DBConnector().StartTrans();
                    if (ConnectITS.DBConnector().Execute(query) > 0)
                        ConnectITS.DBConnector().Commit();
                    else
                        ConnectITS.DBConnector().Rollback();
                }

                //ConnectITS.Close();
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
            }
        }

        //검사 이력 정보에 로트 시작 정보를 등록한다.
        public static bool UpdateJobInfoStart(JobHistoryInfo job)
        {
            try
            {
                if (ConnectITS.Open())
                {
                    bool bRes = false;

                    String query = String.Format("SELECT IDX FROM job_info WHERE LOCAL_LOT='{0}' AND MACHINE='{1}' AND LOCAL_MODEL='{2}'", job.strLocalOrder, job.strMachine, job.strLocalModel);
                    int nIdx = -1;

                    IDataReader dataReader = ConnectITS.DBConnector().ExecuteQuery19(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            nIdx = Convert.ToInt32(dataReader.GetValue(0).ToString());
                            break;
                        }
                        dataReader.Close();
                    }

                    if (nIdx != -1)
                        query = String.Format("UPDATE job_info SET STATE='{0}' WHERE IDX='{1}'", (int)State.Inspect, nIdx);
                    else
                    {
                        JobInspCheck(job.strMachine);

                        query = String.Format("INSERT INTO job_info(LOT, ITS_LOT, STD_MODEL, OP_CODE, MACHINE, LOCAL_MODEL, LOCAL_GROUP, CREATE_TIME, STATE, LOCAL_LOT) " +
                            "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', now(), {7}, '{8}')", job.strProdOrder, job.strItsOrder, job.strStdModel, job.strOpcode,
                            job.strMachine, job.strLocalModel, job.strLocalGroup, (int)State.Inspect, job.strLocalOrder);
                    }

                    ConnectITS.DBConnector().StartTrans();
                    if (ConnectITS.DBConnector().Execute(query) > 0)
                    {
                        ConnectITS.DBConnector().Commit();
                        bRes = true;
                    }
                    else
                        ConnectITS.DBConnector().Rollback();

                    ConnectITS.Close();

                    return bRes;
                }
                else
                    return false;
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
                return false;
            }
        }
        
        //검사 이력 정보에 로트 완료 정보를 등록한다.
        public static bool UpdateJobInfoEnd(JobHistoryInfo job)
        {
            try
            {
                if (ConnectITS.Open())
                {
                    bool bRes = false;

                    String query = String.Format("SELECT IDX FROM job_info WHERE LOCAL_LOT='{0}' AND MACHINE='{1}' AND LOCAL_MODEL='{2}' AND LOCAL_GROUP='{3}'", 
                        job.strLocalOrder, job.strMachine, job.strLocalModel, job.strLocalGroup);
                    int nIdx = -1;

                    IDataReader dataReader = ConnectITS.DBConnector().ExecuteQuery19(query);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            nIdx = Convert.ToInt32(dataReader.GetValue(0).ToString());
                            break;
                        }
                        dataReader.Close();
                    }

                    if (nIdx != -1)
                        query = String.Format("UPDATE job_info SET STATE={0} WHERE IDX={1}", (int)State.InspectDone, nIdx);
                    else
                        query = String.Format("INSERT INTO job_info(LOT, ITS_LOT, STD_MODEL, OP_CODE, MACHINE, LOCAL_MODEL, LOCAL_GROUP, STATE, LOCAL_LOT) " +
                            "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}, '{8}')", job.strProdOrder, job.strItsOrder, job.strStdModel, job.strOpcode,
                            job.strMachine, job.strLocalModel, job.strLocalGroup, (int)State.Inspect, job.strLocalOrder);

                    ConnectITS.DBConnector().StartTrans();
                    if (ConnectITS.DBConnector().Execute(query) > 0)
                    {
                        ConnectITS.DBConnector().Commit();
                        bRes = true;
                    }
                    else
                        ConnectITS.DBConnector().Rollback();

                    ConnectITS.Close();

                    return bRes;
                }
                else
                    return false;
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
                return false;
            }
        }
        #endregion

        #region VRS Information
        //kmj
        /*검사기의 로트 정보를 DataBase에 기록 합니다.*/
        public static bool CreateLotInfo(VRS_LOT_Info LotInfo, VRS_Model_Info ModelInfo, string TableName, string ScanCountInfo)
        {
            if (ConnectITS.Open())
            {
                //string str = string.Format("delete from {0} where Lot_No = '{1}'", TableName, LotInfo.LotNo);
                //string str = string.Format("DELETE FROM {0} WHERE Lot_No = '{1}' AND ITS_Lot_No = '{2}' AND Model_Name = '{3}' AND AVI_Code = '{4}'",
                //                            TableName, LotInfo.LotNo, LotInfo.ITS_LotNo, LotInfo.Model, LotInfo.MC_Name);
                //ConnectITS.DBConnector().Execute(str);
                string model = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};",
                                              ModelInfo.Height,
                                              ModelInfo.Width,
                                              ModelInfo.BlockNum,
                                              ModelInfo.BlockPitch,
                                              ModelInfo.UnitNumX,
                                              ModelInfo.UnitNumY,
                                              ModelInfo.UnitPitchX,
                                              ModelInfo.UnitPitchY);
                for (int i = 0; i < ModelInfo.SectionPos.Length; i++)
                {
                    model += String.Format("{0};{1};", ModelInfo.SectionPos[i].X, ModelInfo.SectionPos[i].Y); 
                }
                for (int i = 0; i < ModelInfo.SectionPos.Length; i++)
                {
                    model += String.Format("{0};{1};", ModelInfo.IDSectionPos[i].X, ModelInfo.IDSectionPos[i].Y);
                }

                string str = string.Format("REPLACE INTO {0}(Lot_No, ITS_Lot_No, Group_Name, Model_Name, AVI_Code, Model_Info, AVI_Time, TD_Length, X_out, X_Continue, Y_Continue, SurfaceScanCount, ManagementCode) "
                                     + "values('{1}','{2}','{3}','{4}','{5}','{6}', now(), '{7}', {8}, {9}, {10}, '{11}', '{12}')"

                     , TableName, LotInfo.LotNo, LotInfo.ITS_LotNo, LotInfo.Group, LotInfo.Model, LotInfo.MC_Name, model, ModelInfo.TD_Length, ModelInfo.X_Out, ModelInfo.X_Continue, ModelInfo.Y_Continue, ScanCountInfo, LotInfo.ManagementCode);
                if (ConnectITS.DBConnector().Execute(str) < 1) return false;

                ConnectITS.Close();
                return true;
            }
            else return false;
        }

        /*VRS의 로트 정보를 DataBase에 기록 합니다.*/
        public static bool CreateVRSInfo(string LotNo, string MC_Code, string TableName)
        {
            if (ConnectITS.Open())
            {
                string str = string.Format("delete from {0} where Lot_No = '{1}'", TableName, LotNo);
                ConnectITS.DBConnector().Execute(str);

                str = string.Format("insert into {0}(Lot_No, MC_Code, Reg_Date) values('{1}', '{2}', now())", TableName, LotNo, MC_Code);
                if (ConnectITS.DBConnector().Execute(str) < 1) return false;

                ConnectITS.Close();
                return true;
            }
            else
                return false;
        }

        /*검사기에서 기록한 로트 정보를 불러 옵니다.*/
        public static bool ReadLotInfo(string Lot, ref VRS_LOT_Info LotInfo, ref VRS_Model_Info ModelInfo, string TableName)
        {
            LotInfo = new VRS_LOT_Info();
            ModelInfo = new VRS_Model_Info();
            if (ConnectITS.Open())
            {
                string str = string.Format("select * from {0} where Lot_No = '{1}'", TableName, Lot);
                IDataReader dataReader = ConnectITS.DBConnector().ExecuteQuery19(str);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        LotInfo.LotNo = dataReader.GetValue(0).ToString();
                        LotInfo.Group = dataReader.GetValue(1).ToString();
                        LotInfo.Model = dataReader.GetValue(2).ToString();
                        LotInfo.MC_Name = dataReader.GetValue(3).ToString();
                        LotInfo.RegDate = Convert.ToDateTime(dataReader.GetValue(5).ToString());
                        string[] strmodel = dataReader.GetValue(4).ToString().Split(';');
                        if(strmodel.Length >= 18)
                        {
                            ModelInfo.Height = Convert.ToDouble(strmodel[0]);
                            ModelInfo.Width = Convert.ToDouble(strmodel[1]);
                            ModelInfo.BlockNum = Convert.ToInt32(strmodel[2]);
                            ModelInfo.BlockPitch = Convert.ToDouble(strmodel[3]);
                            ModelInfo.UnitNumX = Convert.ToInt32(strmodel[4]);
                            ModelInfo.UnitNumY = Convert.ToInt32(strmodel[5]);
                            ModelInfo.UnitPitchX = Convert.ToDouble(strmodel[6]);
                            ModelInfo.UnitPitchY = Convert.ToDouble(strmodel[7]);
                            for (int i = 0; i < ModelInfo.SectionPos.Length; i++)
                            {
                                ModelInfo.SectionPos[i].X = Convert.ToDouble(strmodel[7 + (i * 2) + 1]);
                                ModelInfo.SectionPos[i].Y = Convert.ToDouble(strmodel[7 + (i * 2) + 2]);
                            }
                        }
                        ModelInfo.TD_Length = Convert.ToInt32(dataReader.GetValue(6));
                        ModelInfo.X_Out = Convert.ToInt32(dataReader.GetValue(7));
                        ModelInfo.X_Continue = Convert.ToInt32(dataReader.GetValue(8));
                        ModelInfo.Y_Continue = Convert.ToInt32(dataReader.GetValue(9));

                        dataReader.Close();
                        ConnectITS.Close();
                        return true;
                    }
                    dataReader.Close();
                    ConnectITS.Close();
                }
                else ConnectITS.Close();
            }
            return false;
        }

        /*설비 정보를 불러옵니다.*/
        public static bool ReadMachineInfo(ref VRS_LOT_Info LotInfo, ref VRS_Model_Info ModelInfo, ref List<double> lstResolution, string TableName)
        {
            if (ConnectITS.Open())
            {
                string str = string.Format("select Image_Path, Map_Path, Model_Path, Resolution from {0} where MC_Code = '{1}'", TableName, LotInfo.MC_Name);
                IDataReader dataReader = ConnectITS.DBConnector().ExecuteQuery19(str);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        LotInfo.ResultPath = dataReader.GetValue(0).ToString();
                        LotInfo.Map_Path = dataReader.GetValue(1).ToString();
                        ModelInfo.Unit_Path = dataReader.GetValue(2).ToString();
                        string[] res = dataReader.GetValue(3).ToString().Split(';');
                        for(int i=0; i<res.Count(); i++)
                        {
                            lstResolution.Add(Convert.ToDouble(res[i]));
                        }
                        
                        dataReader.Close();
                        ConnectITS.Close();
                        return true;
                    }
                    dataReader.Close();
                    ConnectITS.Close();
                }
                else ConnectITS.Close();
            }
            return false;
        }
        
        /*RectFill에 해당하는 불량명, BinCode를 읽어 옵니다.*/
        public static bool ReadMapConv(ref List<MapConverter> lstConv, string TableName)
        {
            if(ConnectITS.Open())
            {
                string str = string.Format("select * from {0}", TableName);
                IDataReader dataReader = ConnectITS.DBConnector().ExecuteQuery19(str);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        MapConverter conv = new MapConverter();
                        conv.RectFill = Convert.ToInt32(dataReader.GetValue(0).ToString());
                        conv.badName = dataReader.GetValue(1).ToString();
                        conv.binCode = dataReader.GetValue(2).ToString();

                        lstConv.Add(conv);
                    }
                    dataReader.Close();
                    ConnectITS.Close();

                    return true;
                }
                else
                    ConnectITS.Close();
            }
            return false;
        }
        #endregion

        #region Not Used
        public static bool CreateITSModel(ITSModelInfo model)
        {
            if (ConnectITS.Open())
            {
                string str = string.Format("insert into itsdb_bga.ITS_Model(Name, Use_ITS, LeftID, InnerAOI, OuterAOI, BBT, SKIPDATA, Length, Width, PitchX, PitchY, UnitNumX, UnitNumY) "
                                     + "values('{0}',{1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12})"

                     , model.ModelName, model.UseITS, model.LeftID, model.Length, model.InnerAOI, model.OuterAOI, model.BBT, model.SKIPDATA, model.Length, model.Width, 
                       model.PitchX, model.PitchY, model.UnitNumX, model.UnitNumY);
                if (ConnectITS.DBConnector().Execute(str) < 1) return false;

                ConnectITS.Close();
                return true;
            }
            else return false;
        }
        //No Use
        public static ITSModelInfo LoadModel(string strModelCode)
        {
            string ModelName = strModelCode;
            if(strModelCode.Length > 4)
            {
                ModelName = strModelCode.Substring(0, 4);
            }
            if (ConnectITS.Open())
            {
                string str = string.Format("select * from itsdb_bga.ITS_Model");
                IDataReader dataReader = ConnectITS.DBConnector().ExecuteQuery19(str);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        ITSModelInfo model = new ITSModelInfo();
                        
                        model.ModelName = dataReader.GetValue(0).ToString();
                        if (model.ModelName.Contains(ModelName))
                        {
                            model.UseITS = dataReader.GetValue(1).ToString() == "1" ? true : false;
                            model.LeftID = dataReader.GetValue(2).ToString() == "1" ? true : false;
                            model.InnerAOI = dataReader.GetValue(3).ToString() == "1" ? true : false;
                            model.OuterAOI = dataReader.GetValue(4).ToString() == "1" ? true : false;
                            model.BBT = dataReader.GetValue(5).ToString() == "1" ? true : false;
                            model.SKIPDATA = dataReader.GetValue(6).ToString() == "1" ? true : false;
                            model.Length = Convert.ToDouble(dataReader.GetValue(7).ToString());
                            model.Width = Convert.ToDouble(dataReader.GetValue(8).ToString());
                            model.PitchX = Convert.ToDouble(dataReader.GetValue(9).ToString());
                            model.PitchY = Convert.ToDouble(dataReader.GetValue(10).ToString());
                            model.UnitNumX = Convert.ToInt32(dataReader.GetValue(11).ToString());
                            model.UnitNumY = Convert.ToInt32(dataReader.GetValue(1).ToString());

                            dataReader.Close();
                            ConnectITS.Close();
                            return model;
                        }
                    }
                    dataReader.Close();
                    ConnectITS.Close();
                }
                else ConnectITS.Close();
            }
            return null;
        }
        //NO USE
        public static List<ITSModelInfo> LoadModels()
        {
            List<ITSModelInfo> models = new List<ITSModelInfo>();
            if (ConnectITS.Open())
            {
                string str = string.Format("select * from itsdb_bga.ITS_Model");
                IDataReader dataReader = ConnectITS.DBConnector().ExecuteQuery19(str);
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        ITSModelInfo model = new ITSModelInfo();
                        model.ModelName = dataReader.GetValue(0).ToString();
                        model.UseITS = dataReader.GetValue(1).ToString() == "0" ? true : false;
                        model.LeftID = dataReader.GetValue(2).ToString() == "0" ? true : false;
                        model.InnerAOI = dataReader.GetValue(3).ToString() == "0" ? true : false;
                        model.OuterAOI = dataReader.GetValue(4).ToString() == "0" ? true : false;
                        model.BBT = dataReader.GetValue(5).ToString() == "0" ? true : false;
                        model.SKIPDATA = dataReader.GetValue(6).ToString() == "1" ? true : false;
                        model.Length = Convert.ToDouble(dataReader.GetValue(7).ToString());
                        model.Width = Convert.ToDouble(dataReader.GetValue(8).ToString());
                        model.PitchX = Convert.ToDouble(dataReader.GetValue(9).ToString());
                        model.PitchY = Convert.ToDouble(dataReader.GetValue(10).ToString());
                        model.UnitNumX = Convert.ToInt32(dataReader.GetValue(11).ToString());
                        model.UnitNumY = Convert.ToInt32(dataReader.GetValue(12).ToString());
                        models.Add(model);
                        
                    }
                    dataReader.Close();
                    ConnectITS.Close();
                }
                else ConnectITS.Close();
            }
            return models;
        }

        public static bool DeleteModel(string strModelCode)
        {
            if (ConnectITS.Open())
            {
                string str = string.Format("delete from itsdb_bga.ITS_Model where Code = '{0}'", strModelCode);
                ConnectITS.DBConnector().Execute(str);
                ConnectITS.Close();
            }
            return true;
        }
        #endregion
    }
}
