using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Xml;
using System.Xml.Linq;

namespace IGS.Classes
{
    public class StandardManager
    {
        ///BGA 제품 Unit 좌표계 정의 
        ///TOP: 몰드게이트가 있는 면, BOT: 몰드게이트가 없는 면, Map 방향은 몰드게이트가 상면, Unit 상단에 있는 상태 기준으로 좌표를 계산한다.
        ///BOC 제품군은 몰드게이트가 없지만, 리드가 있는 면이 TOP이며, 레일부의 반대편을 몰드게이트 위치로 가정한다.
        ///BOT 좌표도 TOP 좌표와 동일하다 (ex. TOP의 (1,1) Unit과 BOT의 (1,1) Unit은 동일한 Unit을 나타낸다.)

        ///LF 제품 Unit 좌표계 정의 필요


        #region Member Variables.

        public STD_MAP_HEADER CUR_HEADER = new STD_MAP_HEADER();    //현재 LOT HEADER 정보
        public string MAP_DIR_PATH = "";                            //.xml 파일이 기록될 Directory 경로

        public STD_MAP_DATA[] CUR_MAP = new STD_MAP_DATA[4];        //STRIP/SHOT MAP 정보 (0: ALL, 1: TOP, 2: BOT, 3: BACK)

        public string LOCAL_GROUP_NAME { get { return CUR_HEADER.LOCAL_GROUP_NAME; } set { CUR_HEADER.LOCAL_GROUP_NAME = value; } }         //(필수)설비 내부 그룹명
        public string STD_MODEL_NAME { get { return CUR_HEADER.STD_MODEL_NAME; } set { CUR_HEADER.STD_MODEL_NAME = value; } }               //표준 모델명 (MES.ITEM_CD)
        public string LOCAL_MODEL_NAME { get { return CUR_HEADER.LOCAL_MODEL_NAME; } set { CUR_HEADER.LOCAL_MODEL_NAME = value; } }         //(필수)설비 내부 모델명
        public string STD_ORDER { get { return CUR_HEADER.STD_ORDER; } set { CUR_HEADER.STD_ORDER = value; } }                              //(필수)표준 오더명 (9자리)
        public string ITS_ORDER { get { return CUR_HEADER.ITS_ORDER; } set { CUR_HEADER.ITS_ORDER = value; } }                              //ITS 오더명
        public string LOCAL_ORDER { get { return CUR_HEADER.LOCAL_ORDER; } set { CUR_HEADER.LOCAL_ORDER = value; } }                        //(필수)설비 내부 오더명 (ex. ~J01, ~1회차, etc)
        public string OP_CODE { get { return CUR_HEADER.OP_CODE; } set { CUR_HEADER.OP_CODE = value; } }                                    //(필수)공정코드
        public string PROD_TYPE { get { return CUR_HEADER.PROD_TYPE; } set { CUR_HEADER.PROD_TYPE = value; } }                              //(필수)제품 형태 (SHOT/STRIP)
        public int UNIT_X { get { return CUR_HEADER.UNIT_X; } set { CUR_HEADER.UNIT_X = value; } }                                          //(필수)Strip 길이방향 Unit 수
        public int UNIT_Y { get { return CUR_HEADER.UNIT_Y; } set { CUR_HEADER.UNIT_Y = value; } }                                          //(필수)Strip 폭방향 Unit 수
        public string MOLD_SURFACE { get { return CUR_HEADER.MOLD_SURFACE; } set { CUR_HEADER.MOLD_SURFACE = value; } }                     //BGA MOLD면 (TOP, BOT)
        public string MOLD_LOCATION { get { return CUR_HEADER.MOLD_LOCATION; } set { CUR_HEADER.MOLD_LOCATION = value; } }                  //BGA MOLD위치 (Unit 기준 - 상단, 하단)
        #endregion

        #region Initializer
        public StandardManager()
        {
            for (int i = 0; i < CUR_MAP.Length; i++)
                CUR_MAP[i] = new STD_MAP_DATA();
        }

        public void AllClear()
        {
            CUR_HEADER = new STD_MAP_HEADER();
            MAP_DIR_PATH = "";

            for (int i = 0; i < CUR_MAP.Length; i++)
                CUR_MAP[i] = new STD_MAP_DATA();
        }

        public void MapClear(STD_SIDE side)
        {
            CUR_MAP[(int)side] = new STD_MAP_DATA();
        }

        public void DirectoryClear()
        {
            try
            {
                string strDirPath = string.Format(@"{0}/{1}/{2}/{3}_{4}", MAP_DIR_PATH, LOCAL_GROUP_NAME, LOCAL_MODEL_NAME, LOCAL_ORDER, OP_CODE);
                if (Directory.Exists(strDirPath))
                {
                    string[] files = Directory.GetFiles(strDirPath);
                    foreach (string file in files)
                        File.Delete(file);

                    Directory.Delete(strDirPath);
                }
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs DirectoryClear Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs DirectoryClear Exception: {0}", ex.Message));
            }
        }
        #endregion Initializer

        #region ADD HEADER DATA
        public bool AddSapCodePriority(string strSapCode, int nPriority)
        {
            try
            {
                if (CUR_HEADER.SAP_PRIORITY == null)
                    CUR_HEADER.SAP_PRIORITY = new List<SAP_CODE_PRIORITY>();

                SAP_CODE_PRIORITY sap = new SAP_CODE_PRIORITY();
                sap.SAP_CODE = strSapCode;
                sap.PRIORITY = nPriority;

                CUR_HEADER.SAP_PRIORITY.Add(sap);
                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddSapCodePriority Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddSapCodePriority Exception: {0}", ex.Message));
                return false;
            }
        }
        #endregion ADD HEADER DATA

        #region ADD MAP DATA
        public bool SetStripInfo(STD_SIDE side, int ProdID, int ItsID = 0, int nReloadCount = 0, string strBarcode = "")
        {
            try
            {
                CUR_MAP[(int)side].PROD_ID = ProdID;
                CUR_MAP[(int)side].ITS_ID = ItsID;
                CUR_MAP[(int)side].RE_LOAD_COUNT = nReloadCount;
                CUR_MAP[(int)side].BARCODE_STR = strBarcode;

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddStripInfo Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddStripInfo Exception: {0}", ex.Message));
                return false;
            }
        }

        #region ADD BINCODE MAP
        public bool AddStripMap(STD_SIDE side, string[,] sapCodeMap, int nScanCount = 0, STD_DISPOSE_CODE disposeCode = STD_DISPOSE_CODE.NONE, STD_YN outerNG = STD_YN.N, string stripRow = "")
        {
            try
            {
                if (sapCodeMap == null)
                    return false;

                BIN_CODE_MAP binMap = new BIN_CODE_MAP();

                binMap.SCAN_COUNT = nScanCount;
                binMap.DISPOSE_CODE = disposeCode;
                binMap.OUTER_NG = outerNG;
                binMap.STRIP_ROW = stripRow;

                for (int i = 0; i < sapCodeMap.GetLength(0); i++)
                {
                    BIN_CODE_DATA mapData = new BIN_CODE_DATA();
                    mapData.MAP_CONTENT = " ";

                    for (int j = 0; j < sapCodeMap.GetLength(1); j++)
                        mapData.MAP_CONTENT += String.Format("{0} ", string.IsNullOrEmpty(sapCodeMap[i, j]) ? "----" : sapCodeMap[i, j]);

                    binMap.BIN_LIST.Add(mapData.Clone());
                }
                CUR_MAP[(int)side].BIN_MAP.Add(binMap);

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddStripMap Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddStripMap Exception: {0}", ex.Message));
                return false;
            }
        }
        #endregion ADD BINCODE MAP

        #region ADD IMAGE MAP
        public bool AddImageMap(STD_SIDE side, IMAGE_DATA image)
        {
            try
            {
                if (image == null)
                    return false;

                if (CUR_MAP[(int)side].IMG_MAP == null)
                    CUR_MAP[(int)side].IMG_MAP = new IMAGE_MAP();

                image.IMAGE_INDEX = CUR_MAP[(int)side].IMG_MAP.IMG_LIST.Count + 1;
                CUR_MAP[(int)side].IMG_MAP.IMG_LIST.Add(image.Clone());

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddImageMap Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddImageMap Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddImageMap(STD_SIDE side, List<IMAGE_DATA> imgList)
        {
            try
            {
                if (imgList == null)
                    return false;

                if (imgList.Count == 0)
                    return true;

                if (CUR_MAP[(int)side].IMG_MAP == null)
                    CUR_MAP[(int)side].IMG_MAP = new IMAGE_MAP();

                foreach (IMAGE_DATA img in imgList)
                {
                    img.IMAGE_INDEX = CUR_MAP[(int)side].IMG_MAP.IMG_LIST.Count + 1;
                    CUR_MAP[(int)side].IMG_MAP.IMG_LIST.Add(img.Clone());
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddImageMap_List Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddImageMap_List Exception: {0}", ex.Message));
                return false;
            }
        }
        #endregion ADD IMAGE MAP

        #region ADD VERIFY MAP
        public bool AddVerifyMap_Strip(STD_SIDE side, string strDecision)
        {
            try
            {
                CUR_MAP[(int)side].VERIFY_MAP = new VERIFY_MAP();
                CUR_MAP[(int)side].VERIFY_MAP.BASE = "STRIP";

                CUR_MAP[(int)side].VERIFY_MAP.STRIP_DATA = new VERIFY_STRIP_DATA();
                CUR_MAP[(int)side].VERIFY_MAP.STRIP_DATA.DECISION = strDecision;

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddVerifyMap_Strip Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddVerifyMap_Strip Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddVerifyMap_Unit(STD_SIDE side, string nRow, int nUnitX, int nUnitY, string strDecision)
        {
            try
            {
                if (CUR_MAP[(int)side].VERIFY_MAP == null)
                    CUR_MAP[(int)side].VERIFY_MAP = new VERIFY_MAP();

                CUR_MAP[(int)side].VERIFY_MAP.BASE = "UNIT";

                VERIFY_UNIT_DATA unitData = new VERIFY_UNIT_DATA();
                unitData.ROW = nRow;
                unitData.X = nUnitX;
                unitData.Y = nUnitY;
                unitData.DECISION = strDecision;

                CUR_MAP[(int)side].VERIFY_MAP.UNIT_DATA.Add(unitData);
                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddVerifyMap_Unit Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddVerifyMap_Unit Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddVerifyMap_Unit(STD_SIDE side, int nUnitX, int nUnitY, string strDecision)
        {
            try
            {
                if (CUR_MAP[(int)side].VERIFY_MAP == null)
                    CUR_MAP[(int)side].VERIFY_MAP = new VERIFY_MAP();

                CUR_MAP[(int)side].VERIFY_MAP.BASE = "UNIT";

                VERIFY_UNIT_DATA unitData = new VERIFY_UNIT_DATA();
                unitData.ROW = "";
                unitData.X = nUnitX;
                unitData.Y = nUnitY;
                unitData.DECISION = strDecision;

                CUR_MAP[(int)side].VERIFY_MAP.UNIT_DATA.Add(unitData);
                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddVerifyMap_Unit Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddVerifyMap_Unit Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddVerifyMap_Unit(STD_SIDE side, VERIFY_UNIT_DATA unitData)
        {
            try
            {
                if (CUR_MAP[(int)side].VERIFY_MAP == null)
                    CUR_MAP[(int)side].VERIFY_MAP = new VERIFY_MAP();

                CUR_MAP[(int)side].VERIFY_MAP.BASE = "UNIT";

                CUR_MAP[(int)side].VERIFY_MAP.UNIT_DATA.Add(unitData.Clone());
                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddVerifyMap_Unit Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddVerifyMap_Unit Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddVerifyMap_Unit(STD_SIDE side, List<VERIFY_UNIT_DATA> unitList)
        {
            try
            {
                if (CUR_MAP[(int)side].VERIFY_MAP == null)
                    CUR_MAP[(int)side].VERIFY_MAP = new VERIFY_MAP();

                CUR_MAP[(int)side].VERIFY_MAP.BASE = "UNIT";

                foreach (VERIFY_UNIT_DATA unit in unitList)
                    CUR_MAP[(int)side].VERIFY_MAP.UNIT_DATA.Add(unit.Clone());

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddVerifyMap_Unit Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddVerifyMap_Unit Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddVerifyMap_Image(STD_SIDE side, string camID, int nIndex, string strDecision)
        {
            try
            {
                if (CUR_MAP[(int)side].VERIFY_MAP == null)
                    CUR_MAP[(int)side].VERIFY_MAP = new VERIFY_MAP();

                CUR_MAP[(int)side].VERIFY_MAP.BASE = "IMAGE";

                VERIFY_IMAGE_DATA imgData = new VERIFY_IMAGE_DATA();
                imgData.CAM_ID = camID;
                imgData.INDEX = nIndex;
                imgData.DECISION = strDecision;

                CUR_MAP[(int)side].VERIFY_MAP.IMG_DATA.Add(imgData);
                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddVerifyMap_Image Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddVerifyMap_Image Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddVerifyMap_Image(STD_SIDE side, VERIFY_IMAGE_DATA imgData)
        {
            try
            {
                if (CUR_MAP[(int)side].VERIFY_MAP == null)
                    CUR_MAP[(int)side].VERIFY_MAP = new VERIFY_MAP();

                CUR_MAP[(int)side].VERIFY_MAP.BASE = "IMAGE";

                CUR_MAP[(int)side].VERIFY_MAP.IMG_DATA.Add(imgData.Clone());
                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddVerifyMap_Image Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddVerifyMap_Image Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddVerifyMap_Image(STD_SIDE side, List<VERIFY_IMAGE_DATA> imgList)
        {
            try
            {
                if (CUR_MAP[(int)side].VERIFY_MAP == null)
                    CUR_MAP[(int)side].VERIFY_MAP = new VERIFY_MAP();

                CUR_MAP[(int)side].VERIFY_MAP.BASE = "IMAGE";

                foreach (VERIFY_IMAGE_DATA img in imgList)
                    CUR_MAP[(int)side].VERIFY_MAP.IMG_DATA.Add(img.Clone());

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddVerifyMap_Image Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddVerifyMap_Image Exception: {0}", ex.Message));
                return false;
            }
        }
        #endregion ADD VERIFY MAP

        #region ADD TRANSFER MAP
        public bool AddTransferMap(STD_SIDE side, string fromID, string fromRow, string toID, string custID)
        {
            try
            {
                if (CUR_MAP[(int)side].TRANS_MAP == null)
                    CUR_MAP[(int)side].TRANS_MAP = new TRANSFER_MAP();

                TRANSFER_DATA trans = new TRANSFER_DATA();
                trans.FROM_ID = fromID;
                trans.FROM_ROW = fromRow;
                trans.TO_ID = toID;
                trans.CUST_ID = custID;

                CUR_MAP[(int)side].TRANS_MAP.TRANS_LIST.Add(trans);
                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddTransferMap Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddTransferMap Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddTransferMap(STD_SIDE side, TRANSFER_DATA transData)
        {
            try
            {
                if (transData == null)
                    return false;

                if (CUR_MAP[(int)side].TRANS_MAP == null)
                    CUR_MAP[(int)side].TRANS_MAP = new TRANSFER_MAP();

                CUR_MAP[(int)side].TRANS_MAP.TRANS_LIST.Add(transData.Clone());
                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddTransferMap Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddTransferMap Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddTransferMap(STD_SIDE side, List<TRANSFER_DATA> transList)
        {
            try
            {
                if (transList == null)
                    return false;

                if (CUR_MAP[(int)side].TRANS_MAP == null)
                    CUR_MAP[(int)side].TRANS_MAP = new TRANSFER_MAP();

                foreach (TRANSFER_DATA trans in transList)
                    CUR_MAP[(int)side].TRANS_MAP.TRANS_LIST.Add(trans.Clone());

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddTransferMap Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddTransferMap Exception: {0}", ex.Message));
                return false;
            }
        }
        #endregion ADD TRANSFER MAP

        #region ADD TIME STAMP
        public bool AddTimestamp(STD_SIDE side, int nStep, string strDesc)
        {
            try
            {
                if (nStep < 1)
                    return false;

                if (CUR_MAP[(int)side].TIMESTAMPS == null)
                    CUR_MAP[(int)side].TIMESTAMPS = new Dictionary<int, TIMESTAMP_DATA>();

                TIMESTAMP_DATA time = new TIMESTAMP_DATA();
                time.DESCRIPTION = strDesc;
                time.TIME_STAMP = DateTime.Now;

                if (CUR_MAP[(int)side].TIMESTAMPS.ContainsKey(nStep))
                    CUR_MAP[(int)side].TIMESTAMPS[nStep] = time;
                else
                    CUR_MAP[(int)side].TIMESTAMPS.Add(nStep, time);

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddTimestamp Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddTimestamp Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool AddTimestamp(STD_SIDE side, int nStep, string strDesc, DateTime timeStamp)
        {
            try
            {
                if (nStep < 1)
                    return false;

                if (CUR_MAP[(int)side].TIMESTAMPS == null)
                    CUR_MAP[(int)side].TIMESTAMPS = new Dictionary<int, TIMESTAMP_DATA>();

                TIMESTAMP_DATA time = new TIMESTAMP_DATA();
                time.DESCRIPTION = strDesc;
                time.TIME_STAMP = timeStamp;

                if (CUR_MAP[(int)side].TIMESTAMPS.ContainsKey(nStep))
                    CUR_MAP[(int)side].TIMESTAMPS[nStep] = time;
                else
                    CUR_MAP[(int)side].TIMESTAMPS.Add(nStep, time);

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs AddTimestamp Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs AddTimestamp Exception: {0}", ex.Message));
                return false;
            }
        }
        #endregion ADD TIME STAMP

        #endregion ADD MAP DATA

        #region EXPORT XML FILE

        public bool WriteHeader()
        {
            try
            {
                if (string.IsNullOrEmpty(MAP_DIR_PATH))
                    return false;

                string headerFilePath = string.Format(@"{0}/MapHeader.xml", DirectoryCheck());

                XDocument xDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
                XNamespace xNameSpace = string.Format("urn:xsd.H142-V{0}.SubstrateMap", CUR_MAP[0].MAP_VERSION);

                XElement xMapData = new XElement(xNameSpace + "MapHeader");
                xDoc.Add(xMapData);

                XElement xParameters = new XElement(xNameSpace + "Parameters", new XAttribute("HeaderVersion", CUR_HEADER.HEADER_VERSION));
                xMapData.Add(xParameters);

                //Local Group Name
                XElement xLocGroupName = new XElement(xNameSpace + "LocalGroupName", new XAttribute("Description", "설비 내부 그룹명"));
                xParameters.Add(xLocGroupName);
                XText xLocGroupNameData = new XText(CUR_HEADER.LOCAL_GROUP_NAME);
                xLocGroupName.Add(xLocGroupNameData);

                //Standard Model Name
                XElement xStdModelName = new XElement(xNameSpace + "StdModelName", new XAttribute("Description", "표준 모델명"));
                xParameters.Add(xStdModelName);
                XText xStdModelNameData = new XText(CUR_HEADER.STD_MODEL_NAME);
                xStdModelName.Add(xStdModelNameData);

                //Local Model Name
                XElement xLocModelName = new XElement(xNameSpace + "LocalModelName", new XAttribute("Description", "설비 내부 모델명"));
                xParameters.Add(xLocModelName);
                XText xLocModelNameData = new XText(CUR_HEADER.LOCAL_MODEL_NAME);
                xLocModelName.Add(xLocModelNameData);

                //Standard Order
                XElement xStdOrder = new XElement(xNameSpace + "StdOrder", new XAttribute("Description", "표준 오더명"));
                xParameters.Add(xStdOrder);
                XText xStdOrderData = new XText(CUR_HEADER.STD_ORDER);
                xStdOrder.Add(xStdOrderData);

                //ITS Order
                XElement xITSOrder = new XElement(xNameSpace + "ItsOrder", new XAttribute("Description", "ITS 오더명"));
                xParameters.Add(xITSOrder);
                XText xITSOrderData = new XText(CUR_HEADER.ITS_ORDER);
                xITSOrder.Add(xITSOrderData);

                //Local Order
                XElement xLocOrder = new XElement(xNameSpace + "LocalOrder", new XAttribute("Description", "설비 내부 오더명"));
                xParameters.Add(xLocOrder);
                XText xLocOrderData = new XText(CUR_HEADER.LOCAL_ORDER);
                xLocOrder.Add(xLocOrderData);

                //Operation Code
                XElement xOpcode = new XElement(xNameSpace + "OpCode", new XAttribute("Description", "공정코드"));
                xParameters.Add(xOpcode);
                XText xOpcodeData = new XText(CUR_HEADER.OP_CODE);
                xOpcode.Add(xOpcodeData);

                //Product Type
                XElement xProdType = new XElement(xNameSpace + "ProdType", new XAttribute("Description", "제품형태"));
                xParameters.Add(xProdType);
                XText xProdTypeData = new XText(CUR_HEADER.PROD_TYPE);
                xProdType.Add(xProdTypeData);

                //Unit X Count
                XElement xUnitX = new XElement(xNameSpace + "UnitColumnCount", new XAttribute("Description", "길이방향 Unit수"));
                xParameters.Add(xUnitX);
                XText xUnitXData = new XText(CUR_HEADER.UNIT_X.ToString());
                xUnitX.Add(xUnitXData);

                //Unit Y Count
                XElement xUnitY = new XElement(xNameSpace + "UnitRowCount", new XAttribute("Description", "폭방향 Unit수"));
                xParameters.Add(xUnitY);
                XText xUnitYData = new XText(CUR_HEADER.UNIT_Y.ToString());
                xUnitY.Add(xUnitYData);

                //Image Header
                XElement xImageHeader = new XElement(xNameSpace + "ImageHeader", new XAttribute("Separator", ","));
                xParameters.Add(xImageHeader);
                XElement xTagSequence = new XElement(xNameSpace + "TagSequence");
                xImageHeader.Add(xTagSequence);

                CUR_HEADER.IMAGE_HEADER = IMAGE_DATA.GetTagSequence();
                XText xTagData = new XText(CUR_HEADER.IMAGE_HEADER);
                xTagSequence.Add(xTagData);

                //Sap Code Priority
                XElement xSapPriority = new XElement(xNameSpace + "SapCodePriority");
                xParameters.Add(xSapPriority);

                CUR_HEADER.SAP_PRIORITY.OrderBy(s => s.PRIORITY);
                foreach (SAP_CODE_PRIORITY prio in CUR_HEADER.SAP_PRIORITY)
                {
                    XElement xSapCode = new XElement(xNameSpace + "SapCode", new XAttribute("Priority", prio.PRIORITY.ToString()));
                    xSapPriority.Add(xSapCode);

                    XText xSapData = new XText(prio.SAP_CODE);
                    xSapCode.Add(xSapData);
                }

                //Moldgate Information
                if (!string.IsNullOrEmpty(MOLD_SURFACE) && !string.IsNullOrEmpty(MOLD_LOCATION))
                {
                    XElement xMoldSurface = new XElement(xNameSpace + "MoldSurface", new XAttribute("Description", "몰드게이트 위치 (검사부)"));
                    xParameters.Add(xMoldSurface);
                    XText xMoldSurfData = new XText(CUR_HEADER.MOLD_SURFACE.ToString());
                    xMoldSurface.Add(xMoldSurfData);

                    XElement xMoldLocation = new XElement(xNameSpace + "MoldLocation", new XAttribute("Description", "몰드게이트 위치 (제품)"));
                    xParameters.Add(xMoldLocation);
                    XText xMoldLocData = new XText(CUR_HEADER.MOLD_LOCATION.ToString());
                    xMoldLocation.Add(xMoldLocData);
                }

                xDoc.Save(headerFilePath, SaveOptions.OmitDuplicateNamespaces);
                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs WriteHeader Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs WriteHeader Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool WriteMap(STD_SIDE side)
        {
            try
            {
                if (string.IsNullOrEmpty(MAP_DIR_PATH))
                    return false;

                STD_MAP_DATA WRITE_MAP = CUR_MAP[(int)side];
                string mapFilePath = string.Format(@"{0}/ID[{1}]_{2}_Map.xml", DirectoryCheck(), WRITE_MAP.PROD_ID.ToString("D5"), side.ToString());

                XDocument xDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
                XNamespace xNameSpace = string.Format("urn:xsd.H142-V{0}.SubstrateMap", WRITE_MAP.MAP_VERSION);

                XElement xMapData = new XElement(xNameSpace + "MapData");
                xDoc.Add(xMapData);

                XElement xSubstrateMaps = new XElement(xNameSpace + "SubstrateMaps", WRITE_MAP.GetXMLAttributes());
                xMapData.Add(xSubstrateMaps);

                //BIN CODE MAP
                if (WRITE_MAP.BIN_MAP.Count > 0)
                {
                    XElement xSubstrateMap = new XElement(xNameSpace + "SubstrateMap");
                    xSubstrateMaps.Add(xSubstrateMap);

                    foreach (BIN_CODE_MAP map in WRITE_MAP.BIN_MAP)
                    {
                        XElement xBinMap = new XElement(xNameSpace + "BinCodeMap", map.GetXMLAttributes());
                        xSubstrateMap.Add(xBinMap);

                        foreach (BIN_CODE_DATA data in map.BIN_LIST)
                        {
                            XElement xBinCode = new XElement(xNameSpace + "BinCode");
                            xBinMap.Add(xBinCode);

                            XText xBinData = new XText(data.MAP_CONTENT);
                            xBinCode.Add(xBinData);
                        }
                    }
                }

                //IMAGE MAP
                if (WRITE_MAP.IMG_MAP != null && WRITE_MAP.IMG_MAP.IMG_LIST.Count > 0)
                {
                    XElement xSubstrateMap = new XElement(xNameSpace + "SubstrateMap");
                    xSubstrateMaps.Add(xSubstrateMap);

                    XElement xImageMap = new XElement(xNameSpace + "ImageMap");
                    xSubstrateMap.Add(xImageMap);

                    foreach (IMAGE_DATA data in WRITE_MAP.IMG_MAP.IMG_LIST)
                    {
                        XElement xImage = new XElement(xNameSpace + "I");
                        xImageMap.Add(xImage);

                        XText xImageData = new XText(data.GetDataSequence());
                        xImage.Add(xImageData);
                    }
                }

                //VERIFY MAP
                if (WRITE_MAP.VERIFY_MAP != null)
                {
                    if (WRITE_MAP.VERIFY_MAP.BASE == "STRIP" && WRITE_MAP.VERIFY_MAP.STRIP_DATA != null)
                    {
                        XElement xSubstrateMap = new XElement(xNameSpace + "SubstrateMap");
                        xSubstrateMaps.Add(xSubstrateMap);

                        XElement xVerifyMap = new XElement(xNameSpace + "VerifyMap", new XAttribute("Base", WRITE_MAP.VERIFY_MAP.BASE));
                        xSubstrateMap.Add(xVerifyMap);

                        XElement xStrip = new XElement(xNameSpace + "SV");
                        xVerifyMap.Add(xStrip);

                        XText xDecision = new XText(WRITE_MAP.VERIFY_MAP.STRIP_DATA.DECISION);
                        xStrip.Add(xDecision);
                    }
                    else if (WRITE_MAP.VERIFY_MAP.BASE == "UNIT" && WRITE_MAP.VERIFY_MAP.UNIT_DATA.Count > 0)
                    {
                        XElement xSubstrateMap = new XElement(xNameSpace + "SubstrateMap");
                        xSubstrateMaps.Add(xSubstrateMap);

                        XElement xVerifyMap = new XElement(xNameSpace + "VerifyMap", new XAttribute("Base", WRITE_MAP.VERIFY_MAP.BASE));
                        xSubstrateMap.Add(xVerifyMap);

                        foreach (VERIFY_UNIT_DATA unit in WRITE_MAP.VERIFY_MAP.UNIT_DATA)
                        {
                            XElement xUnit = new XElement(xNameSpace + "UV", unit.GetXMLAttributes());
                            xVerifyMap.Add(xUnit);

                            XText xDecision = new XText(unit.DECISION);
                            xUnit.Add(xDecision);
                        }
                    }
                    else if (WRITE_MAP.VERIFY_MAP.BASE == "IMAGE" && WRITE_MAP.VERIFY_MAP.IMG_DATA.Count > 0)
                    {
                        XElement xSubstrateMap = new XElement(xNameSpace + "SubstrateMap");
                        xSubstrateMaps.Add(xSubstrateMap);

                        XElement xVerifyMap = new XElement(xNameSpace + "VerifyMap", new XAttribute("Base", WRITE_MAP.VERIFY_MAP.BASE));
                        xSubstrateMap.Add(xVerifyMap);

                        foreach (VERIFY_IMAGE_DATA image in WRITE_MAP.VERIFY_MAP.IMG_DATA)
                        {
                            XElement xImage = new XElement(xNameSpace + "IV", image.GetXMLAttributes());
                            xVerifyMap.Add(xImage);

                            XText xDecision = new XText(image.DECISION);
                            xImage.Add(xDecision);
                        }
                    }
                }

                //TRANSFER MAP
                if (WRITE_MAP.TRANS_MAP != null && WRITE_MAP.TRANS_MAP.TRANS_LIST.Count > 0)
                {
                    XElement xSubstrateMap = new XElement(xNameSpace + "SubstrateMap");
                    xSubstrateMaps.Add(xSubstrateMap);

                    XElement xTransMap = new XElement(xNameSpace + "TransferMap", WRITE_MAP.TRANS_MAP.GetXMLAttributes());
                    xSubstrateMap.Add(xTransMap);

                    foreach (TRANSFER_DATA trans in WRITE_MAP.TRANS_MAP.TRANS_LIST)
                    {
                        XElement xTrans = new XElement(xNameSpace + "T", trans.GetXMLAttributes());
                        xTransMap.Add(xTrans);
                    }
                }

                //Timestamps
                if (WRITE_MAP.TIMESTAMPS != null && WRITE_MAP.TIMESTAMPS.Count > 0)
                {
                    XElement xSubstrateMap = new XElement(xNameSpace + "SubstrateMap");
                    xSubstrateMaps.Add(xSubstrateMap);

                    XElement xTimestamp = new XElement(xNameSpace + "Timestamps");
                    xSubstrateMap.Add(xTimestamp);

                    List<int> steps = WRITE_MAP.TIMESTAMPS.Keys.ToList();
                    steps.Sort();

                    foreach (int step in steps)
                    {
                        XElement xStep = new XElement(xNameSpace + string.Format("Step{0}", step), new XAttribute("Description", WRITE_MAP.TIMESTAMPS[step].DESCRIPTION));
                        xTimestamp.Add(xStep);

                        XText xTimeData = new XText(WRITE_MAP.TIMESTAMPS[step].TIME_STAMP.ToString("yyyy-MM-dd HH:mm:ss:fffff"));
                        xStep.Add(xTimeData);
                    }
                }

                xDoc.Save(mapFilePath);

                //Save Image Maching List
                if (WRITE_MAP.IMG_MAP != null && WRITE_MAP.IMG_MAP.IMG_LIST.Count > 0)
                    WriteImageMatchList(side);

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs WriteMap Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs WriteMap Exception: {0}", ex.Message));
                return false;
            }
        }

        ///// Image명을 바로 표준화 할 수 없으니, 표준정보에 대한 이미지명 매칭 리스트 파일을 별도로 만들자 - WriteMap안에서 임시로 호출하자
        private bool WriteImageMatchList(STD_SIDE side)
        {
            try
            {
                List<string> matchList = new List<string>();
                foreach (IMAGE_DATA image in CUR_MAP[(int)side].IMG_MAP.IMG_LIST)
                {
                    string strLine = string.Format("{0}={1}", image.GetFileName(), image.LOCAL_IMG_FILE_NAME);
                    matchList.Add(strLine);
                }

                if (matchList.Count > 0)
                {
                    string strMatchFilePath = string.Format(@"{0}/ImageMatchList_{1}.txt", DirectoryCheck(), side == STD_SIDE.TOP ? "TOP" : "BOT");
                    File.AppendAllLines(strMatchFilePath, matchList);
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs WriteImageMatchList Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs WriteImageMatchList Exception: {0}", ex.Message));
                return false;
            }
        }

        public string DirectoryCheck()
        {
            string strDirPath = string.Format(@"{0}", MAP_DIR_PATH);

            if (!Directory.Exists(strDirPath))
                Directory.CreateDirectory(strDirPath);

            strDirPath = string.Format(@"{0}/{1}", strDirPath, LOCAL_GROUP_NAME);
            if (!Directory.Exists(strDirPath))
                Directory.CreateDirectory(strDirPath);

            strDirPath = string.Format(@"{0}/{1}", strDirPath, LOCAL_MODEL_NAME);
            if (!Directory.Exists(strDirPath))
                Directory.CreateDirectory(strDirPath);

            strDirPath = string.Format(@"{0}/{1}_{2}", strDirPath, LOCAL_ORDER, OP_CODE);
            if (!Directory.Exists(strDirPath))
                Directory.CreateDirectory(strDirPath);

            return strDirPath;
        }
        #endregion EXPORT XML FILE

        #region READ XML FILE

        public bool ReadHeader(string strFilePath)
        {
            try
            {
                if (!File.Exists(strFilePath) || !strFilePath.EndsWith(".xml"))
                    return false;

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(strFilePath);

                XmlElement xMapHeader = xDoc.DocumentElement;
                XmlNode xParameters = xMapHeader.ChildNodes[0];

                XmlNodeList paramList = xParameters.ChildNodes;
                foreach (XmlNode param in paramList)
                {
                    switch (param.Name)
                    {
                        case "LocalGroupName":
                            CUR_HEADER.LOCAL_GROUP_NAME = param.InnerText;
                            break;
                        case "StdModelName":
                            CUR_HEADER.STD_MODEL_NAME = param.InnerText;
                            break;
                        case "LocalModelName":
                            CUR_HEADER.LOCAL_MODEL_NAME = param.InnerText;
                            break;
                        case "StdOrder":
                            CUR_HEADER.STD_ORDER = param.InnerText;
                            break;
                        case "ItsOrder":
                            CUR_HEADER.ITS_ORDER = param.InnerText;
                            break;
                        case "LocalOrder":
                            CUR_HEADER.LOCAL_ORDER = param.InnerText;
                            break;
                        case "OpCode":
                            CUR_HEADER.OP_CODE = param.InnerText;
                            break;
                        case "ProdType":
                            CUR_HEADER.PROD_TYPE = param.InnerText;
                            break;
                        case "UnitColumnCount":
                            CUR_HEADER.UNIT_X = Convert.ToInt32(param.InnerText);
                            break;
                        case "UnitRowCount":
                            CUR_HEADER.UNIT_Y = Convert.ToInt32(param.InnerText);
                            break;
                        case "ImageHeader":
                            XmlNode xTagSeq = param.ChildNodes[0];
                            if (xTagSeq.Name == "TagSequence")
                                CUR_HEADER.IMAGE_HEADER = xTagSeq.InnerText;
                            break;
                        case "SapCodePriority":
                            XmlNodeList sapCodes = param.ChildNodes;
                            foreach (XmlNode code in sapCodes)
                            {
                                SAP_CODE_PRIORITY prio = new SAP_CODE_PRIORITY();
                                prio.SAP_CODE = code.InnerText;
                                prio.PRIORITY = Convert.ToInt32(code.Attributes["Priority"].Value);

                                CUR_HEADER.SAP_PRIORITY.Add(prio);
                            }
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs ReadHeader Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs ReadHeader Exception: {0}", ex.Message));
                return false;
            }
        }

        //Map 읽을 때, 불량이 0개인 제품의 경우 ImageList에 해당 ID에 대한 이미지가 있으면 지워줘야 한다. (재검 잔재)
        public bool ReadMap(STD_SIDE side, int nProdID)
        {
            try
            {
                if (string.IsNullOrEmpty(MAP_DIR_PATH))
                    return false;

                if (string.IsNullOrEmpty(LOCAL_GROUP_NAME) || string.IsNullOrEmpty(LOCAL_MODEL_NAME) || string.IsNullOrEmpty(LOCAL_ORDER) || string.IsNullOrEmpty(OP_CODE))
                    return false;

                string strFilePath = string.Format(@"{0}/{1}/{2}/{3}_{4}/ID[{5}]_{6}_Map.xml", MAP_DIR_PATH, LOCAL_GROUP_NAME, LOCAL_MODEL_NAME,
                    LOCAL_ORDER, OP_CODE, nProdID.ToString("D5"), side.ToString());
                if (!File.Exists(strFilePath))
                    return false;

                CUR_MAP[(int)side] = new STD_MAP_DATA();

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(strFilePath);

                XmlElement xMapData = xDoc.DocumentElement;
                XmlNode xSubstrateMaps = xMapData.ChildNodes[0];

                XmlAttributeCollection xMapsAttributes = xSubstrateMaps.Attributes;
                if (!CUR_MAP[(int)side].SetXMLAttributes(xMapsAttributes))
                    return false;

                XmlNodeList xSubstrateMapList = xSubstrateMaps.ChildNodes;
                foreach (XmlNode xSubstrateMap in xSubstrateMapList)
                {
                    XmlNode xMap = xSubstrateMap.ChildNodes[0];
                    switch (xMap.Name)
                    {
                        case "BinCodeMap":
                            XmlNodeList xBinCodeMaps = xSubstrateMap.ChildNodes;
                            foreach (XmlNode xBinCodeMap in xBinCodeMaps)
                            {
                                BIN_CODE_MAP binCodeMap = new BIN_CODE_MAP();

                                if (!binCodeMap.SetXMLAttributes(xBinCodeMap.Attributes))
                                    return false;

                                XmlNodeList xBinCodes = xBinCodeMap.ChildNodes;
                                foreach (XmlNode xBinCode in xBinCodes)
                                {
                                    if (xBinCode.Name == "BinCode")
                                    {
                                        BIN_CODE_DATA binCode = new BIN_CODE_DATA();
                                        binCode.MAP_CONTENT = xBinCode.InnerText;

                                        binCodeMap.BIN_LIST.Add(binCode);
                                    }
                                }

                                CUR_MAP[(int)side].BIN_MAP.Add(binCodeMap);
                            }
                            break;
                        case "ImageMap":
                            if (CUR_MAP[(int)side].IMG_MAP == null)
                                CUR_MAP[(int)side].IMG_MAP = new IMAGE_MAP();

                            XmlNodeList xImageInfos = xMap.ChildNodes;
                            foreach (XmlNode xImageInfo in xImageInfos)
                            {
                                if (xImageInfo.Name == "I")
                                {
                                    IMAGE_DATA image = new IMAGE_DATA();
                                    if (!image.SetData(xImageInfo.InnerText, CUR_HEADER.IMAGE_HEADER))
                                        return false;

                                    CUR_MAP[(int)side].IMG_MAP.IMG_LIST.Add(image);
                                }
                            }
                            break;
                        case "VerifyMap":
                            if (CUR_MAP[(int)side].VERIFY_MAP == null)
                                CUR_MAP[(int)side].VERIFY_MAP = new VERIFY_MAP();

                            string strBase = xMap.Attributes["Base"].Value.ToUpper();
                            if (strBase == "STRIP")
                            {
                                CUR_MAP[(int)side].VERIFY_MAP.STRIP_DATA = new VERIFY_STRIP_DATA();
                                CUR_MAP[(int)side].VERIFY_MAP.STRIP_DATA.DECISION = xMap.ChildNodes[0].InnerText;
                            }
                            else if (strBase == "UNIT")
                            {
                                CUR_MAP[(int)side].VERIFY_MAP.UNIT_DATA = new List<VERIFY_UNIT_DATA>();
                                XmlNodeList xUnitList = xMap.ChildNodes;
                                foreach (XmlNode xUnit in xUnitList)
                                {
                                    VERIFY_UNIT_DATA unitData = new VERIFY_UNIT_DATA();

                                    if (!unitData.SetXMLAttributes(xUnit.Attributes))
                                        return false;

                                    unitData.DECISION = xUnit.InnerText;

                                    CUR_MAP[(int)side].VERIFY_MAP.UNIT_DATA.Add(unitData);
                                }
                            }
                            else if (strBase == "IMAGE")
                            {
                                CUR_MAP[(int)side].VERIFY_MAP.IMG_DATA = new List<VERIFY_IMAGE_DATA>();
                                XmlNodeList xImageList = xMap.ChildNodes;
                                foreach (XmlNode xImage in xImageList)
                                {
                                    VERIFY_IMAGE_DATA imageData = new VERIFY_IMAGE_DATA();

                                    if (!imageData.SetXMLAttributes(xImage.Attributes))
                                        return false;

                                    imageData.DECISION = xImage.InnerText;

                                    CUR_MAP[(int)side].VERIFY_MAP.IMG_DATA.Add(imageData);
                                }
                            }
                            break;
                        case "TransferMap":
                            if (CUR_MAP[(int)side].TRANS_MAP == null)
                                CUR_MAP[(int)side].TRANS_MAP = new TRANSFER_MAP();

                            CUR_MAP[(int)side].TRANS_MAP.TRANSFER_NAME = xMap.Attributes["TransferName"] == null ? "" : xMap.Attributes["TransferName"].Value;
                            XmlNodeList xTrans = xMap.ChildNodes;
                            foreach (XmlNode xData in xTrans)
                            {
                                TRANSFER_DATA transData = new TRANSFER_DATA();

                                if (!transData.SetXMLAttributes(xData.Attributes))
                                    return false;

                                CUR_MAP[(int)side].TRANS_MAP.TRANS_LIST.Add(transData);
                            }
                            break;
                        case "Timestamps":
                            if (CUR_MAP[(int)side].TIMESTAMPS == null)
                                CUR_MAP[(int)side].TIMESTAMPS = new Dictionary<int, TIMESTAMP_DATA>();

                            XmlNodeList xStamps = xMap.ChildNodes;
                            foreach (XmlNode xStamp in xStamps)
                            {
                                int nStep = Convert.ToInt32(xStamp.Name.Remove(0, 4));

                                TIMESTAMP_DATA stepData = new TIMESTAMP_DATA();
                                stepData.DESCRIPTION = xStamp.Attributes["Description"].Value;
                                stepData.TIME_STAMP = DateTime.ParseExact(xStamp.InnerText, "yyyy-MM-dd HH:mm:ss:fffff", null);

                                CUR_MAP[(int)side].TIMESTAMPS.Add(nStep, stepData);
                            }
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs ReadMap Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs ReadMap Exception: {0}", ex.Message));
                return false;
            }
        }

        public bool ReadImageMatchList(STD_SIDE side, out Dictionary<string, string> matchList)
        {
            matchList = null;

            try
            {
                if (string.IsNullOrEmpty(MAP_DIR_PATH))
                    return false;

                if (string.IsNullOrEmpty(LOCAL_GROUP_NAME) || string.IsNullOrEmpty(LOCAL_MODEL_NAME) || string.IsNullOrEmpty(LOCAL_ORDER) || string.IsNullOrEmpty(OP_CODE))
                    return false;

                string strFilePath = string.Format(@"{0}/{1}/{2}/{3}_{4}/ImageMatchList_{5}.txt", MAP_DIR_PATH, LOCAL_GROUP_NAME, LOCAL_MODEL_NAME, LOCAL_ORDER, OP_CODE,
                    side == STD_SIDE.TOP ? "TOP" : "BOT");

                if (!File.Exists(strFilePath))
                    return false;

                matchList = new Dictionary<string, string>();

                string[] lines = File.ReadAllLines(strFilePath);
                foreach (string line in lines)
                {
                    int nPos = line.IndexOf("=");

                    string strStand = line.Substring(0, nPos);
                    string strOld = line.Substring(nPos + 1);

                    if (strStand.Contains("NO[1]"))
                    {
                        string strID = strStand.Split('_')[0];
                        List<string> keyList = matchList.Where(m => m.Key.StartsWith(strID)).Select(m => m.Key).ToList();
                        foreach (string key in keyList)
                            matchList.Remove(key);
                    }

                    matchList.Add(strStand, strOld);
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs ReadImageMatchList Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs ReadImageMatchList Exception: {0}", ex.Message));
                return false;
            }
        }

        #endregion READ XML FILE
    }

    #region STANDARD CLASS INFORMATION
    public class STD_MAP_HEADER
    {
        public string LOCAL_GROUP_NAME;                                                 //설비 내부 그룹명
        public string STD_MODEL_NAME;                                                   //표준 모델명 (= MES.ITEM_CD)
        public string LOCAL_MODEL_NAME;                                                 //설비 내부 모델명
        public string STD_ORDER;                                                        //표준 오더명
        public string ITS_ORDER;                                                        //ITS 오더명
        public string LOCAL_ORDER;                                                      //설비 내부 오더명
        public string OP_CODE;                                                          //공정코드
        public string PROD_TYPE;                                                        //제품 형태 (SHOT / STRIP)
        public string IMAGE_HEADER;                                                     //ImageMap Tag Sequence
        public int UNIT_X;                                                              //Strip 길이방향 Unit 수
        public int UNIT_Y;                                                              //Strip 폭방향 Unit 수
        public List<SAP_CODE_PRIORITY> SAP_PRIORITY = new List<SAP_CODE_PRIORITY>();    //SapCode 우선순위
        public string MOLD_SURFACE;                                                     //BGA MOLD면 (TOP, BOT)
        public string MOLD_LOCATION;                                                    //BGA MOLD위치 (Unit 기준 - 상단, 하단)
        public readonly string HEADER_VERSION = "1.0";

        public STD_MAP_HEADER Clone()
        {
            STD_MAP_HEADER data = new STD_MAP_HEADER();

            data.LOCAL_GROUP_NAME = this.LOCAL_GROUP_NAME;
            data.STD_MODEL_NAME = this.STD_MODEL_NAME;
            data.LOCAL_MODEL_NAME = this.LOCAL_MODEL_NAME;
            data.STD_ORDER = this.STD_ORDER;
            data.ITS_ORDER = this.ITS_ORDER;
            data.LOCAL_ORDER = this.LOCAL_ORDER;
            data.OP_CODE = this.OP_CODE;
            data.PROD_TYPE = this.PROD_TYPE;
            data.IMAGE_HEADER = this.IMAGE_HEADER;
            data.UNIT_X = this.UNIT_X;
            data.UNIT_Y = this.UNIT_Y;
            data.SAP_PRIORITY = this.SAP_PRIORITY.ConvertAll(s => s.Clone());
            data.MOLD_SURFACE = this.MOLD_SURFACE;
            data.MOLD_LOCATION = this.MOLD_LOCATION;

            return data;
        }
    }

    public class STD_MAP_DATA
    {
        public int PROD_ID;                                                     //양산 진행 ID (= PLC ID)
        public int ITS_ID;                                                      //2D Barcode ID
        public int RE_LOAD_COUNT;                                               //재투입 횟수 (폐기 재투입 등)
        public string BARCODE_STR;                                              //2D Barcode Reading 내용
        public string MAP_VERSION = "1.0";                             //표준 버전

        public List<BIN_CODE_MAP> BIN_MAP = new List<BIN_CODE_MAP>();           //BinCodeMap
        public IMAGE_MAP IMG_MAP;                                               //ImageMap
        public VERIFY_MAP VERIFY_MAP;                                           //VerifyMap
        public TRANSFER_MAP TRANS_MAP;                                          //TransferMap
        public Dictionary<int, TIMESTAMP_DATA> TIMESTAMPS;                      //Timestamps

        public STD_MAP_DATA Clone()
        {
            STD_MAP_DATA data = new STD_MAP_DATA();

            data.PROD_ID = this.PROD_ID;
            data.ITS_ID = this.ITS_ID;
            data.RE_LOAD_COUNT = this.RE_LOAD_COUNT;
            data.BARCODE_STR = this.BARCODE_STR;

            data.BIN_MAP = this.BIN_MAP.ConvertAll(b => b.Clone());
            if (this.IMG_MAP != null)
                data.IMG_MAP = this.IMG_MAP.Clone();
            if (this.VERIFY_MAP != null)
                data.VERIFY_MAP = this.VERIFY_MAP.Clone();
            if (this.TRANS_MAP != null)
                data.TRANS_MAP = this.TRANS_MAP.Clone();
            if (this.TIMESTAMPS != null)
                data.TIMESTAMPS = new Dictionary<int, TIMESTAMP_DATA>(this.TIMESTAMPS);

            return data;
        }

        public object[] GetXMLAttributes()
        {
            try
            {
                List<object> objs = new List<object>();

                objs.Add(new XAttribute("ProdId", PROD_ID.ToString("D5")));

                if (ITS_ID > 0)
                    objs.Add(new XAttribute("ItsId", ITS_ID.ToString("D5")));

                if (RE_LOAD_COUNT > 0)
                    objs.Add(new XAttribute("ReLoad", RE_LOAD_COUNT));

                if (!string.IsNullOrEmpty(BARCODE_STR))
                    objs.Add(new XAttribute("Barcode", BARCODE_STR));

                objs.Add(new XAttribute("MapVersion", MAP_VERSION));

                return objs.ToArray();
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs STD_MAP_DATA GetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs STD_MAP_DATA GetXMLAttributes Exception: {0}", ex.Message));
                return null;
            }
        }

        public bool SetXMLAttributes(XmlAttributeCollection xAttributes)
        {
            try
            {
                foreach (XmlAttribute attr in xAttributes)
                {
                    switch (attr.Name)
                    {
                        case "ProdId":
                            PROD_ID = Convert.ToInt32(attr.Value);
                            break;
                        case "ItsId":
                            ITS_ID = Convert.ToInt32(attr.Value);
                            break;
                        case "ReLoad":
                            RE_LOAD_COUNT = Convert.ToInt32(attr.Value);
                            break;
                        case "Barcode":
                            BARCODE_STR = attr.Value;
                            break;
                        case "MapVeresion":
                            MAP_VERSION = attr.Value;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs STD_MAP_DATA SetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs STD_MAP_DATA SetXMLAttributes Exception: {0}", ex.Message));
                return false;
            }
        }
    }

    public class BIN_CODE_MAP
    {
        public string STRIP_ROW;                                                //현재 Strip의 열 정보
        public int SCAN_COUNT = 0;                                              //검사 횟수 (광학조건 차이)
        public STD_DISPOSE_CODE DISPOSE_CODE;                                   //폐기 사유 코드
        public STD_YN OUTER_NG;                                                 //외곽 불량 발생 여부
        public STD_BIN_TYPE BIN_TYPE = STD_BIN_TYPE.SAP_CODE;                   //BinCode 표현 형태
        public string NULL_BIN = "----";                                        //양품의 BinCode        
        public List<BIN_CODE_DATA> BIN_LIST = new List<BIN_CODE_DATA>();        //현재 Strip의 Map 정보

        public BIN_CODE_MAP Clone()
        {
            BIN_CODE_MAP data = new BIN_CODE_MAP();

            data.STRIP_ROW = this.STRIP_ROW;
            data.SCAN_COUNT = this.SCAN_COUNT;
            data.DISPOSE_CODE = this.DISPOSE_CODE;
            data.OUTER_NG = this.OUTER_NG;
            data.BIN_TYPE = this.BIN_TYPE;
            data.NULL_BIN = this.NULL_BIN;
            data.BIN_LIST = this.BIN_LIST.ConvertAll(b => b);

            return data;
        }

        public object[] GetXMLAttributes()
        {
            try
            {
                List<object> objs = new List<object>();

                if (!string.IsNullOrEmpty(STRIP_ROW))
                    objs.Add(new XAttribute("StripRow", STRIP_ROW));

                if (SCAN_COUNT > 0)
                    objs.Add(new XAttribute("ScanCount", SCAN_COUNT));

                if (DISPOSE_CODE > 0)
                    objs.Add(new XAttribute("DisposeCode", DISPOSE_CODE));

                if (OUTER_NG == STD_YN.Y)
                    objs.Add(new XAttribute("OuterNG", OUTER_NG));

                objs.Add(new XAttribute("BinType", BIN_TYPE));
                objs.Add(new XAttribute("NullBin", NULL_BIN));

                return objs.ToArray();
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs BIN_CODE_MAP GetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs BIN_CODE_MAP GetXMLAttributes Exception: {0}", ex.Message));
                return null;
            }
        }

        public bool SetXMLAttributes(XmlAttributeCollection xAttributes)
        {
            try
            {
                foreach (XmlAttribute attr in xAttributes)
                {
                    switch (attr.Name)
                    {
                        case "StripRow":
                            STRIP_ROW = attr.Value;
                            break;
                        case "ScanCount":
                            SCAN_COUNT = Convert.ToInt32(attr.Value);
                            break;
                        case "DisposeCode":
                            DISPOSE_CODE = (STD_DISPOSE_CODE)Enum.Parse(typeof(STD_DISPOSE_CODE), attr.Value);
                            break;
                        case "OuterNG":
                            OUTER_NG = (STD_YN)Enum.Parse(typeof(STD_YN), attr.Value);
                            break;
                        case "BinType":
                            BIN_TYPE = (STD_BIN_TYPE)Enum.Parse(typeof(STD_BIN_TYPE), attr.Value);
                            break;
                        case "NullBin":
                            NULL_BIN = attr.Value;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs BIN_CODE_MAP SetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs BIN_CODE_MAP SetXMLAttributes Exception: {0}", ex.Message));
                return false;
            }
        }
    }

    //DATA의 확장성을 위하여 별도의 Class로 생성
    public class BIN_CODE_DATA
    {
        public string MAP_CONTENT;                                              //MAP 내역

        public BIN_CODE_DATA Clone()
        {
            BIN_CODE_DATA data = new BIN_CODE_DATA();

            data.MAP_CONTENT = this.MAP_CONTENT;

            return data;
        }
    }
    public class TRANSFER_MAP
    {
        public string TRANSFER_NAME = "ShotToStrip";                            //변환 종류
        public List<TRANSFER_DATA> TRANS_LIST = new List<TRANSFER_DATA>();      //변환 내용

        public TRANSFER_MAP Clone()
        {
            TRANSFER_MAP data = new TRANSFER_MAP();

            data.TRANSFER_NAME = this.TRANSFER_NAME;
            data.TRANS_LIST = this.TRANS_LIST.ConvertAll(t => t);

            return data;
        }

        public object[] GetXMLAttributes()
        {
            try
            {
                List<object> objs = new List<object>();

                objs.Add(new XAttribute("TransferName", TRANSFER_NAME));

                return objs.ToArray();
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs TRANSFER_MAP GetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs TRANSFER_MAP GetXMLAttributes Exception: {0}", ex.Message));
                return null;
            }
        }
    }

    public class TRANSFER_DATA
    {
        public string FROM_ID;                                                  //인식한 ShotID
        public string FROM_ROW;                                                 //제품의 열 정보
        public string TO_ID;                                                    //각인한 사내 표준 2D ID
        public string CUST_ID;                                                  //고객사별 지정 사양에 따른 각인한 2D ID

        public TRANSFER_DATA Clone()
        {
            TRANSFER_DATA data = new TRANSFER_DATA();

            data.FROM_ID = this.FROM_ID;
            data.FROM_ROW = this.FROM_ROW;
            data.TO_ID = this.TO_ID;

            data.CUST_ID = this.CUST_ID;

            return data;
        }

        public object[] GetXMLAttributes()
        {
            try
            {
                List<object> objs = new List<object>();

                objs.Add(new XAttribute("FromId", FROM_ID));
                objs.Add(new XAttribute("FromRow", FROM_ROW));
                objs.Add(new XAttribute("ToId", TO_ID));
                objs.Add(new XAttribute("CustId", CUST_ID));

                return objs.ToArray();
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs TRANSFER_DATA GetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs TRANSFER_DATA GetXMLAttributes Exception: {0}", ex.Message));
                return null;
            }
        }

        public bool SetXMLAttributes(XmlAttributeCollection xAttributes)
        {
            try
            {
                foreach (XmlAttribute attr in xAttributes)
                {
                    switch (attr.Name)
                    {
                        case "FromId":
                            this.FROM_ID = attr.Value;
                            break;
                        case "FromRow":
                            this.FROM_ROW = attr.Value;
                            break;
                        case "ToId":
                            this.TO_ID = attr.Value;
                            break;
                        case "CustId":
                            this.CUST_ID = attr.Value;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs TRANSFER_DATA SetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs TRANSFER_DATA SetXMLAttributes Exception: {0}", ex.Message));
                return false;
            }
        }
    }

    public class IMAGE_MAP
    {
        public List<IMAGE_DATA> IMG_LIST = new List<IMAGE_DATA>();

        public IMAGE_MAP Clone()
        {
            IMAGE_MAP data = new IMAGE_MAP();

            data.IMG_LIST = this.IMG_LIST.ConvertAll(i => i);

            return data;
        }
    }

    //데이터간 구분자로 콤마(',')를 사용하기 때문에, 데이터 값에 콤마가 들어가지 않도록 주의 필요
    public class IMAGE_DATA
    {
        public int PRODUCT_ID;                                                  //(필수)ShotID, StripID
        public string CAM_ID;                                                   //(필수)Stage 구분명 (ex. TOP, BOT, 11, 12)        
        public int IMAGE_INDEX;                                                 //(필수)Shot/Strip 내 불량 Index - AddImage할 때 자동으로 Increment
        public string STRIP_ROW;                                                //Shot 내 열 정보 (몇번째 Strip 인가)
        public int UNIT_X;                                                      //(필수)길이방향 Unit 위치
        public int UNIT_Y;                                                      //(필수)폭방향 Unit 위치
        public int POS_X;                                                       //(필수)기준 Unit 내 불량 중심 위치 (Pixel 단위)
        public int POS_Y;                                                       //(필수)기준 Unit 내 불량 중심 위치 (Pixel 단위)
        public int COLOR_IMAGE;                                                 //Mono(0), Color(1), Red(2), Green(3), Blue(4)
        public double DEFECT_SIZE;                                              //길이 조건 Defect (um)
        public double DEFECT_PIXEL;                                             //길이 조건 Defect (pixel)
        public double DEFECT_PERC;                                              //비율 조건 Defect (%)
        public double DEFECT_AREA;                                              //면적 조건 Defect (pixel)
        public int TEACHING_TAB_ID;                                             //검출된 Teaching Tab ID
        public int ROI_ID;                                                      //검출된 ROI ID
        public int INSPECT_CODE;                                                //검출된 알고리즘 코드
        public int VISION_CODE;                                                 //검출된 Vision Defect 코드
        public string BAD_NAME;                                                 //(필수)검출 불량명
        public string SAP_CODE;                                                 //(필수)불량에 대한 SAP CODE
        public string COLOR_CHANNEL;                                            //Color 검사에서 Defect된 Channel (R,G,B,RG ...)
        public int BLOB_WIDTH;                                                  //불량 Blob Width (pixel)
        public int BLOB_HEIGHT;                                                 //불량 Blob Height (pixel)
        public int BLOB_SUM;                                                    //불량 Blob Sum (pixel)
        public double AVG_RED;                                                  //Red Channel Blob 평균
        public double AVG_GREEN;                                                //Green Channel Blob 평균
        public double AVG_BLUE;                                                 //Blue Channel Blob 평균
        public double AVG_HUE;                                                  //색상 평균
        public double AVG_SATURATION;                                           //채도 평균
        public double AVG_VALUE;                                                //명도 평균
        public int THRESHOLD;                                                   //Binary Threshold 값
        public string AI_VERIFY_RESULT;                                         //AI Verify 판정 결과

        public string LOCAL_IMG_FILE_NAME;                                      //(필수)표준화 미적용 이미지 파일명 - 이미지에 대하여 표준 전환 완료 후 삭제 예정 (임시조치)

        public IMAGE_DATA Clone()
        {
            IMAGE_DATA data = new IMAGE_DATA();

            data.PRODUCT_ID = this.PRODUCT_ID;
            data.CAM_ID = this.CAM_ID;
            data.IMAGE_INDEX = this.IMAGE_INDEX;
            data.STRIP_ROW = this.STRIP_ROW;
            data.UNIT_X = this.UNIT_X;
            data.UNIT_Y = this.UNIT_Y;
            data.POS_X = this.POS_X;
            data.POS_Y = this.POS_Y;
            data.COLOR_IMAGE = this.COLOR_IMAGE;
            data.DEFECT_SIZE = this.DEFECT_SIZE;
            data.DEFECT_PIXEL = this.DEFECT_PIXEL;
            data.DEFECT_PERC = this.DEFECT_PERC;
            data.DEFECT_AREA = this.DEFECT_AREA;
            data.TEACHING_TAB_ID = this.TEACHING_TAB_ID;
            data.ROI_ID = this.ROI_ID;
            data.INSPECT_CODE = this.INSPECT_CODE;
            data.VISION_CODE = this.VISION_CODE;
            data.BAD_NAME = this.BAD_NAME;
            data.SAP_CODE = this.SAP_CODE;
            data.COLOR_CHANNEL = this.COLOR_CHANNEL;
            data.BLOB_WIDTH = this.BLOB_WIDTH;
            data.BLOB_HEIGHT = this.BLOB_HEIGHT;
            data.BLOB_SUM = this.BLOB_SUM;
            data.AVG_RED = this.AVG_RED;
            data.AVG_GREEN = this.AVG_GREEN;
            data.AVG_BLUE = this.AVG_BLUE;
            data.AVG_HUE = this.AVG_HUE;
            data.AVG_SATURATION = this.AVG_SATURATION;
            data.AVG_VALUE = this.AVG_VALUE;
            data.THRESHOLD = this.THRESHOLD;
            data.AI_VERIFY_RESULT = this.AI_VERIFY_RESULT;

            data.LOCAL_IMG_FILE_NAME = this.LOCAL_IMG_FILE_NAME;

            return data;
        }

        public static string GetTagSequence()
        {
            string strHeader = string.Format("PRODUCT_ID,CAM_ID,IMAGE_INDEX,STRIP_ROW,UNIT_X,UNIT_Y,POS_X,POS_Y,COLOR_IMAGE,DEFECT_SIZE,DEFECT_PIXEL," +
                "DEFECT_PERC,DEFECT_AREA,TEACHING_TAB_ID,ROI_ID,INSPECT_CODE,VISION_CODE,BAD_NAME,SAP_CODE,COLOR_CHANNEL,BLOB_WIDTH,BLOB_HEIGHT,BLOB_SUM," +
                "AVG_RED,AVG_GREEN,AVG_BLUE,AVG_HUE,AVG_SATURATION,AVG_VALUE,THRESHOLD,AI_VERIFY_RESULT");

            return strHeader;
        }

        public string GetDataSequence()
        {
            string strSep = ",";

            string strData = GetData(0);
            for (int i = 1; i < 31; i++)
                strData += string.Format("{0}{1}", strSep, GetData(i));

            return strData;
        }

        public string GetFileName()
        {
            return string.Format("ID[{0}]_CAM[{1}]_NO[{2}]", PRODUCT_ID.ToString("D5"), CAM_ID, IMAGE_INDEX);
        }

        private string GetData(int nIndex)
        {
            switch (nIndex)
            {
                case 0: return this.PRODUCT_ID.ToString();
                case 1: return this.CAM_ID;
                case 2: return this.IMAGE_INDEX.ToString();
                case 3: return this.STRIP_ROW;
                case 4: return this.UNIT_X.ToString();
                case 5: return this.UNIT_Y.ToString();
                case 6: return this.POS_X.ToString();
                case 7: return this.POS_Y.ToString();
                case 8: return this.COLOR_IMAGE.ToString();
                case 9: return this.DEFECT_SIZE.ToString();
                case 10: return this.DEFECT_PIXEL.ToString();
                case 11: return this.DEFECT_PERC.ToString();
                case 12: return this.DEFECT_AREA.ToString();
                case 13: return this.TEACHING_TAB_ID.ToString();
                case 14: return this.ROI_ID.ToString();
                case 15: return this.INSPECT_CODE.ToString();
                case 16: return this.VISION_CODE.ToString();
                case 17: return this.BAD_NAME;
                case 18: return this.SAP_CODE;
                case 19: return this.COLOR_CHANNEL;
                case 20: return this.BLOB_WIDTH.ToString();
                case 21: return this.BLOB_HEIGHT.ToString();
                case 22: return this.BLOB_SUM.ToString();
                case 23: return this.AVG_RED.ToString();
                case 24: return this.AVG_GREEN.ToString();
                case 25: return this.AVG_BLUE.ToString();
                case 26: return this.AVG_HUE.ToString();
                case 27: return this.AVG_SATURATION.ToString();
                case 28: return this.AVG_VALUE.ToString();
                case 29: return this.THRESHOLD.ToString();
                case 30: return this.AI_VERIFY_RESULT;

                default: return "";
            }
        }

        public bool SetData(string strData, string strTagSequence)
        {
            try
            {
                string[] sptData = strData.Split(',');
                string[] sptTag = strTagSequence.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (sptData.Length != sptTag.Length)
                    return false;

                for (int i = 0; i < sptData.Length; i++)
                {
                    switch (sptTag[i])
                    {
                        case "PRODUCT_ID":
                            PRODUCT_ID = Convert.ToInt32(sptData[i]);
                            break;
                        case "CAM_ID":
                            CAM_ID = sptData[i];
                            break;
                        case "IMAGE_INDEX":
                            IMAGE_INDEX = Convert.ToInt32(sptData[i]);
                            break;
                        case "UNIT_ROW":
                            STRIP_ROW = sptData[i];
                            break;
                        case "UNIT_X":
                            UNIT_X = Convert.ToInt32(sptData[i]);
                            break;
                        case "UNIT_Y":
                            UNIT_Y = Convert.ToInt32(sptData[i]);
                            break;
                        case "POS_X":
                            POS_X = Convert.ToInt32(sptData[i]);
                            break;
                        case "POS_Y":
                            POS_Y = Convert.ToInt32(sptData[i]);
                            break;
                        case "COLOR_IMAGE":
                            COLOR_IMAGE = Convert.ToInt32(sptData[i]);
                            break;
                        case "DEFECT_SIZE":
                            DEFECT_SIZE = Convert.ToDouble(sptData[i]);
                            break;
                        case "DEFECT_PIXEL":
                            DEFECT_PIXEL = Convert.ToDouble(sptData[i]);
                            break;
                        case "DEFECT_PERC":
                            DEFECT_PERC = Convert.ToDouble(sptData[i]);
                            break;
                        case "DEFECT_AREA":
                            DEFECT_AREA = Convert.ToDouble(sptData[i]);
                            break;
                        case "TEACHING_TAB_ID":
                            TEACHING_TAB_ID = Convert.ToInt32(sptData[i]);
                            break;
                        case "ROI_ID":
                            ROI_ID = Convert.ToInt32(sptData[i]);
                            break;
                        case "INSPECT_CODE":
                            INSPECT_CODE = Convert.ToInt32(sptData[i]);
                            break;
                        case "VISION_CODE":
                            VISION_CODE = Convert.ToInt32(sptData[i]);
                            break;
                        case "BAD_NAME":
                            BAD_NAME = sptData[i];
                            break;
                        case "SAP_CODE":
                            SAP_CODE = sptData[i];
                            break;
                        case "COLOR_CHANNEL":
                            COLOR_CHANNEL = sptData[i];
                            break;
                        case "BLOB_WIDTH":
                            BLOB_WIDTH = Convert.ToInt32(sptData[i]);
                            break;
                        case "BLOB_HEIGHT":
                            BLOB_HEIGHT = Convert.ToInt32(sptData[i]);
                            break;
                        case "BLOB_SUM":
                            BLOB_SUM = Convert.ToInt32(sptData[i]);
                            break;
                        case "AVG_RED":
                            AVG_RED = Convert.ToDouble(sptData[i]);
                            break;
                        case "AVG_GREEN":
                            AVG_GREEN = Convert.ToDouble(sptData[i]);
                            break;
                        case "AVG_BLUE":
                            AVG_BLUE = Convert.ToDouble(sptData[i]);
                            break;
                        case "AVG_HUE":
                            AVG_HUE = Convert.ToDouble(sptData[i]);
                            break;
                        case "AVG_SATURATION":
                            AVG_SATURATION = Convert.ToDouble(sptData[i]);
                            break;
                        case "AVG_VALUE":
                            AVG_VALUE = Convert.ToDouble(sptData[i]);
                            break;
                        case "THRESHOLD":
                            THRESHOLD = Convert.ToInt32(sptData[i]);
                            break;
                        case "AI_VERIFY_RESULT":
                            AI_VERIFY_RESULT = sptData[i];
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs IMAGE_DATA SetData Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs IMAGE_DATA SetData Exception: {0}", ex.Message));
                return false;
            }
        }
    }

    public class VERIFY_MAP
    {
        public string BASE;                                                         //판정 단위 (STRIP, UNIT, IMAGE)

        public VERIFY_STRIP_DATA STRIP_DATA;                                        //Strip Verify 판정 데이터
        public List<VERIFY_UNIT_DATA> UNIT_DATA = new List<VERIFY_UNIT_DATA>();     //Unit Verify 판정 데이터
        public List<VERIFY_IMAGE_DATA> IMG_DATA = new List<VERIFY_IMAGE_DATA>();    //Image Verify 판정 데이터

        public VERIFY_MAP Clone()
        {
            VERIFY_MAP data = new VERIFY_MAP();

            data.BASE = this.BASE;
            if (this.STRIP_DATA != null)
                data.STRIP_DATA = this.STRIP_DATA;
            data.UNIT_DATA = this.UNIT_DATA.ConvertAll(u => u);
            data.IMG_DATA = this.IMG_DATA.ConvertAll(i => i);

            return data;
        }
    }

    public class VERIFY_STRIP_DATA
    {
        public string DECISION;                                                 //판정 결과

        public VERIFY_STRIP_DATA Clone()
        {
            VERIFY_STRIP_DATA data = new VERIFY_STRIP_DATA();

            data.DECISION = this.DECISION;

            return data;
        }
    }

    public class VERIFY_UNIT_DATA
    {
        public string ROW;                                                      //Unit 열 정보
        public int X;                                                           //Unit X 위치 정보 (제품 길이방향)
        public int Y;                                                           //Unit Y 위치 정보 (제품 폭방향) 
        public string DECISION;                                                 //판정 결과

        public VERIFY_UNIT_DATA Clone()
        {
            VERIFY_UNIT_DATA data = new VERIFY_UNIT_DATA();

            data.ROW = this.ROW;
            data.X = this.X;
            data.Y = this.Y;
            data.DECISION = this.DECISION;

            return data;
        }

        public object[] GetXMLAttributes()
        {
            try
            {
                List<object> objs = new List<object>();

                if (!string.IsNullOrEmpty(ROW))
                    objs.Add(new XAttribute("Row", ROW));

                objs.Add(new XAttribute("X", X));
                objs.Add(new XAttribute("Y", Y));

                return objs.ToArray();
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs VERIFY_UNIT_DATA GetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs VERIFY_UNIT_DATA GetXMLAttributes Exception: {0}", ex.Message));
                return null;
            }
        }

        public bool SetXMLAttributes(XmlAttributeCollection xAttributes)
        {
            try
            {
                foreach (XmlAttribute attr in xAttributes)
                {
                    switch (attr.Name)
                    {
                        case "Row":
                            ROW = attr.Value;
                            break;
                        case "X":
                            X = Convert.ToInt32(attr.Value);
                            break;
                        case "Y":
                            Y = Convert.ToInt32(attr.Value);
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs VERIFY_UNIT_DATA SetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs VERIFY_UNIT_DATA SetXMLAttributes Exception: {0}", ex.Message));
                return false;
            }
        }
    }

    public class VERIFY_IMAGE_DATA
    {
        public string CAM_ID;                                                   //불량 이미지 CamID (TOP, BOT, 11, 12, ...)
        public int INDEX;                                                       //불량 이미지 Index
        public string DECISION;                                                 //판정 결과

        public VERIFY_IMAGE_DATA Clone()
        {
            VERIFY_IMAGE_DATA data = new VERIFY_IMAGE_DATA();

            data.CAM_ID = this.CAM_ID;
            data.INDEX = this.INDEX;
            data.DECISION = this.DECISION;

            return data;
        }

        public object[] GetXMLAttributes()
        {
            try
            {
                List<object> objs = new List<object>();

                objs.Add(new XAttribute("CamID", CAM_ID));
                objs.Add(new XAttribute("Index", INDEX));

                return objs.ToArray();
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs VERIFY_IMAGE_DATA GetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs VERIFY_IMAGE_DATA GetXMLAttributes Exception: {0}", ex.Message));
                return null;
            }
        }

        public bool SetXMLAttributes(XmlAttributeCollection xAttributes)
        {
            try
            {
                foreach (XmlAttribute attr in xAttributes)
                {
                    switch (attr.Name)
                    {
                        case "CamID":
                            CAM_ID = attr.Value;
                            break;
                        case "Index":
                            INDEX = Convert.ToInt32(attr.Value);
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ClientManager.Log(string.Format("StandardManager.cs VERIFY_IMAGE_DATA SetXMLAttributes Exception: {0}", ex.Message));
                Debug.WriteLine(string.Format("StandardManager.cs VERIFY_IMAGE_DATA SetXMLAttributes Exception: {0}", ex.Message));
                return false;
            }
        }
    }

    public class TIMESTAMP_DATA
    {
        public string DESCRIPTION;
        public DateTime TIME_STAMP;

        public TIMESTAMP_DATA Clone()
        {
            TIMESTAMP_DATA data = new TIMESTAMP_DATA();

            data.DESCRIPTION = this.DESCRIPTION;
            data.TIME_STAMP = this.TIME_STAMP;

            return data;
        }
    }

    public class SAP_CODE_PRIORITY
    {
        public string SAP_CODE;
        public int PRIORITY;

        public SAP_CODE_PRIORITY Clone()
        {
            SAP_CODE_PRIORITY data = new SAP_CODE_PRIORITY();

            data.SAP_CODE = this.SAP_CODE;
            data.PRIORITY = this.PRIORITY;

            return data;
        }
    }
    #endregion STANDARD CLASS INFORMATION

    #region Enum Definition
    public enum STD_SIDE
    {
        ALL = 0,
        TOP = 1,
        BOT = 2,
        BACK = 3
    }

    public enum STD_YN
    {
        Y = 0,
        N = 1
    }

    public enum STD_VERIFY_TYPE
    {
        STRIP = 0,
        UNIT = 1,
        IMAGE = 2,
        AI = 3
    }

    public enum STD_DISPOSE_CODE
    {
        NONE = 0,
        OUTER = 1,
        PSR_SHIFT = 2,
        BLOCK_CONTI = 3,
        BLOCK_XOUT = 4,
        STRIP_CONTI = 5,
        STRIP_XOUT = 6,
        BARCODE_FAIL = 7,
        CRITICAL = 8,
        DEFECT = 9
    }

    public enum STD_STRIP_ROW
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
    }

    public enum STD_BIN_TYPE
    {
        ASCII = 0,
        DECIMAL = 1,
        HEXA_DECIMAL = 2,
        SAP_CODE = 3
    }
    #endregion Enum Definition
}
