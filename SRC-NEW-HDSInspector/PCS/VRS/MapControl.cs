using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Common;

namespace PCS.VRS
{
    public class MapControl
    {
        //info는 SubstrateMaps에 해당하는 헤더정보, maplst는 SubstrateMap에 해당하는 Map정보(Shot: 4strip, Strip: 1strip)
        //자세한 para는 VRS_MAP_Info, VRS_MAP_Body 선언 참조
        //filename: '_TOP, _BOT, _ALL'을 파일명 마지막부분에 추가하여야 한다.
        //mapping 정보는 Unit간 경계를 ';'로 표시한다. ex."FF;FF;FF"
        public static bool WriteMap(VRS_MAP_Info info, List<VRS_MAP_Body> maplst, string mapPath, string filename)
        {
            try
            {
                //Map을 저장할 경로가 올바르지 않을 경우 False
                if (mapPath == "") return false;

                //StripRow수만큼 Map이 존재하지 않을 경우 False
                if (info.StripRow != maplst.Count) return false;

                string url = String.Format(@"{0}/{1}", mapPath, filename + ".xml");

                XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
                XNamespace ns = "urn:semi-org:xsd.E142-1.V1005.SubstrateMap";
                XElement xroot = new XElement(ns + "MapData");
                xdoc.Add(xroot);

                XElement xMaps = new XElement("SubstrateMaps",
                    new XAttribute("SubstrateType", info.SubstrateType),
                    new XAttribute("UnitNumX", info.UnitNumX),
                    new XAttribute("UnitNumY", info.UnitNumY),
                    new XAttribute("StripRow", info.StripRow),
                    new XAttribute("Orientation", info.Orientation)
                );
                xroot.Add(xMaps);

                for (int i = 0; i < info.StripRow; i++)
                {
                    XElement xMap = new XElement("SubstrateMap",
                        new XAttribute("SubstrateID", maplst[i].SubstrateID),
                        new XAttribute("SubstrateSide", maplst[i].Side),
                        new XAttribute("SubstrateAttach", maplst[i].Attach),
                        new XAttribute("InspectIndex", maplst[i].InspectIndex),
                        new XAttribute("MachineOrigin", maplst[i].MachineOrigin),
                        new XAttribute("MapOrigin", maplst[i].MapOrigin),
                        new XAttribute("IsScrap", maplst[i].IsScrap)
                    );
                    xMaps.Add(xMap);

                    XElement xlay = new XElement("Overlay",
                        new XAttribute("MapName", maplst[i].MapName),
                        new XAttribute("MapVersion", maplst[i].MapVersion)
                    );
                    xMap.Add(xlay);

                    XElement xCodemap = new XElement("BinCodeMap",
                        new XAttribute("BinType", maplst[i].BinType),
                        new XAttribute("NullBin", maplst[i].NullBin)
                    );
                    xlay.Add(xCodemap);

                    for (int j = 0; j < info.UnitNumY; j++)
                    {
                        XElement xbinCode = new XElement("BinCode");
                        xCodemap.Add(xbinCode);

                        XCData xCode = new XCData(maplst[i].Map[j]);
                        xbinCode.Add(xCode);
                    }
                }

                //기존 파일이 있는 경우, 자동으로 덮어씁니다.
                xdoc.Save(url);
                return true;
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
                return false;
            }
        }

        public static int WriteMap(VRS_MAP_Info info, VRS_MAP_Body map, string mapPath, string filename)
        {
            try
            {
                //Map을 저장할 경로가 올바르지 않을 경우 False
                if (mapPath == "") return -1;
                
                string url = String.Format(@"{0}\{1}", mapPath, filename + ".xml");

                XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
                XNamespace ns = "urn:semi-org:xsd.E142-1.V1005.SubstrateMap";
                XElement xroot = new XElement(ns + "MapData");
                xdoc.Add(xroot);

                XElement xMaps = new XElement("SubstrateMaps",
                    new XAttribute("SubstrateType", info.SubstrateType),
                    new XAttribute("UnitNumX", info.UnitNumX),
                    new XAttribute("UnitNumY", info.UnitNumY),
                    new XAttribute("StripRow", info.StripRow),
                    new XAttribute("Orientation", info.Orientation)
                );
                xroot.Add(xMaps);

                XElement xMap = new XElement("SubstrateMap",
                    new XAttribute("SubstrateID", map.SubstrateID),
                    new XAttribute("SubstrateSide", map.Side),
                    new XAttribute("SubstrateAttach", map.Attach),
                    new XAttribute("InspectIndex", map.InspectIndex),
                    new XAttribute("MachineOrigin", map.MachineOrigin),
                    new XAttribute("MapOrigin", map.MapOrigin),
                    new XAttribute("IsScrap", map.IsScrap)
                );
                xMaps.Add(xMap);

                XElement xlay = new XElement("Overlay",
                    new XAttribute("MapName", map.MapName),
                    new XAttribute("MapVersion", map.MapVersion)
                );
                xMap.Add(xlay);

                XElement xCodemap = new XElement("BinCodeMap",
                    new XAttribute("BinType", map.BinType),
                    new XAttribute("NullBin", map.NullBin)
                );
                xlay.Add(xCodemap);

                for (int j = 0; j < info.UnitNumY; j++)
                {
                    XElement xbinCode = new XElement("BinCode");
                    xCodemap.Add(xbinCode);

                    XCData xCode = new XCData(map.Map[j]);
                    xbinCode.Add(xCode);
                }
                
                //기존 파일이 있는 경우, 자동으로 덮어씁니다.
                xdoc.Save(url);
                return 0;
            }
            catch (Exception ex)
            {
                string tmp = ex.Message;
                return -2;
            }
        }

        //mapPath는 확장자를 포함하지 않은상태로 넘어와야 한다.
        public static List<VRS_MAP_Body> ReadMap(string filename, string mapPath, ref VRS_MAP_Info info)
        {
            List<VRS_MAP_Body> maplst = new List<VRS_MAP_Body>();

            try
            {
                //Map을 읽을 경로가 올바르지 않을 경우 False
                if (mapPath == "") return null;
                
                string[] mapFile = Directory.GetFiles(mapPath + "\\", filename + "*.xml");
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(mapFile[0]);

                XmlElement xroot = xdoc.DocumentElement;
                XmlNode xMaps = xroot.ChildNodes[0];

                info.SubstrateType = xMaps.Attributes["SubstrateType"].Value;
                info.UnitNumX = Convert.ToInt32(xMaps.Attributes["UnitNumX"].Value);
                info.UnitNumY = Convert.ToInt32(xMaps.Attributes["UnitNumY"].Value);
                info.StripRow = Convert.ToInt32(xMaps.Attributes["StripRow"].Value);
                info.Orientation = Convert.ToInt32(xMaps.Attributes["Orientation"].Value);

                maplst.Clear();
                XmlNodeList xMaplst = xMaps.ChildNodes;
                for (int i = 0; i < info.StripRow; i++)
                {
                    XmlNode xMap = xMaplst[i];

                    VRS_MAP_Body map = new VRS_MAP_Body();
                    map.SubstrateID = xMap.Attributes["SubstrateID"].Value;
                    map.Side = xMap.Attributes["SubstrateSide"].Value;
                    map.Attach = xMap.Attributes["SubstrateAttach"].Value;
                    map.InspectIndex = Convert.ToInt32(xMap.Attributes["InspectIndex"].Value);
                    map.MachineOrigin = xMap.Attributes["MachineOrigin"].Value;
                    map.MapOrigin = xMap.Attributes["MapOrigin"].Value;
                    map.IsScrap = (xMap.Attributes["IsScrap"] == null) ? false : Convert.ToBoolean(xMap.Attributes["IsScrap"].Value);

                    XmlNode xlay = xMap.ChildNodes[0];
                    map.MapName = xlay.Attributes["MapName"].Value;
                    map.MapVersion = xlay.Attributes["MapVersion"].Value;

                    XmlNode xCodeMap = xlay.ChildNodes[0];
                    map.BinType = xCodeMap.Attributes["BinType"].Value;
                    map.NullBin = xCodeMap.Attributes["NullBin"].Value;

                    XmlNodeList codes = xCodeMap.ChildNodes;
                    map.Map = new string[info.UnitNumY];
                    for (int j = 0; j < info.UnitNumY; j++)
                    {
                        XmlNode xbincode = codes[j];
                        XmlNodeList bincodes = xbincode.ChildNodes;

                        XmlNode xcode = bincodes[0];
                        map.Map[j] = xcode.Value;
                    }

                    maplst.Add(map);
                }

                return maplst;
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
                return null;
            }
        }

        /*RectFill로 구성된 Map(int[,])을 입력하면, BinCode로 변환된 List<string> 배열을 반환합니다. int to string list
         int[,] 배열은 (Column, Row) ex.(30, 6) 순서로 구성되어야 합니다.*/
        public static bool MapConvITSL(int[,] iMap, ref List<string> sMap, List<MapConverter> lstConv, bool yFlip = false, bool xFlip = false)
        {
            try
            {
                int nX = iMap.GetLength(0);
                int nY = iMap.GetLength(1);
                sMap.Clear();

                for (int y = 0; y < nY; y++)
                {
                    string str = "";

                    for (int x = 0; x < nX; x++)
                    {
                        int rect;
                        if (yFlip)
                            rect = TempFuncConvertRectFill(iMap[x, nY - y - 1]);
                        else
                            rect = TempFuncConvertRectFill(iMap[x, y]);

                        string bin = lstConv.Single(s => s.RectFill == rect).binCode;

                        str += bin;
                        if (x != nX - 1)
                            str += ";";
                    }
                    sMap.Add(str);
                }

                if (xFlip) sMap.Reverse();

                return true;
            }
            catch (Exception ex)
            {
                string tmp = ex.Message;
                return false;
            }
        }

        //AVI to VRS 불량매칭 임시 조치 코드
        //추후 VRS 수정필요 -> AVI불량 정보 개수만큼 VRS에 추가조치 필요함.
        private static int TempFuncConvertRectFill(int code)
        {
            switch (code)
            {
                case 8:
                case 12:
                    return 1;
                case 9:
                case 10:
                    return 2;
                case 13:
                    return 3;
                case 14:
                    return 4;
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                    return 5;
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                    return 6;
                case 37:
                    return 7;
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                    return 8;
                case 43:
                    return 9;
                case 44:
                    return 10;
                case 4:
                    return 11;
                case 36:
                    return 12;
                case 45:
                    return 13;
                default:
                    return 14;
                case 1:
                    return 15;
                case 2:
                    return 16;
                case 6:
                    return 17;
                case 3:
                    return 18;
                case 5:       //고정
                case 7:       //Skip
                case 11:    //BBT
                case 46:    //검사과검
                case 47:    //티칭에러
                    return 20;
            }
        }


        /*RectFill로 구성된 Map(int[,])을 입력하면, BinCode로 변환된 List<string> 배열을 반환합니다. int to string array
         int[,] 배열은 (Column, Row) ex.(30, 6) 순서로 구성되어야 합니다.*/
        public static bool MapConvITSA(int[,] iMap, ref string[] sMap, List<MapConverter> lstConv, bool yFlip = false)
        {
            try
            {
                int nY = iMap.GetLength(1);
                sMap = new string[nY];

                List<string> tmplst = new List<string>();
                if (!MapConvITSL(iMap, ref tmplst, lstConv, yFlip))
                    return false;
                if (tmplst.Count != nY) return false;

                sMap = tmplst.ToArray();
                return true;
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
                return false;
            }
        }

        /*BinCode로 구성된 string[] 배열을 입력하면, RectFill로 구성된 int[,]배열을 반환합니다. string array to int
         int[,] 배열은 (Column, Row), (X, Y) ex.(30, 6) 순서로 구성됩니다.*/
        public static bool MapConvSATI(string[] sMap, ref int[,] iMap, List<MapConverter> lstConv, bool xFlip = false, bool yFlip = false)
        {
            try
            {
                int nY = sMap.Length;
                int nX = sMap[0].Split(';').Length;
                iMap = new int[nX, nY];

                for(int i=0; i<nY; i++)
                {
                    string[] str = sMap[i].Split(';');
                    if (str.Length != nX) return false;

                    for(int j=0; j<nX; j++)
                    {
                        string bin = str[j];
                        iMap[j, i] = lstConv.Single(s => s.binCode == bin).RectFill;
                    }
                }

                if(xFlip)
                {
                    int[,] tmp = new int[nX, nY];
                    tmp = (int[,])iMap.Clone();

                    for(int i=0; i<nY; i++)
                    {
                        for (int j = 0; j < nX; j++)
                            iMap[j, i] = tmp[nX - j - 1, i];
                    }
                }

                if(yFlip)
                {
                    int[,] tmp = new int[nX, nY];
                    tmp = (int[,])iMap.Clone();

                    for(int i=0; i<nY; i++)
                    {
                        for (int j = 0; j < nX; j++)
                            iMap[j, i] = tmp[j, nY - i - 1];
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                string tmp = ex.Message;
                return false;
            }
        }
        public static bool MapConvITSL(int[,] iMap, ref List<string> sMap, NGInformationHelper NG_Info,  bool yFlip = false, bool xFlip = false)
        {
            try
            {
                int nX = iMap.GetLength(0);
                int nY = iMap.GetLength(1);
                sMap.Clear();

                for (int y = 0; y < nY; y++)
                {
                    string str = "";

                    for (int x = 0; x < nX; x++)
                    {
                        string UnitCode;
                        if (yFlip)
                            UnitCode = NG_Info.GetMapString(iMap[x, nY - y - 1]);
                        else
                            UnitCode = NG_Info.GetMapString(iMap[x, y]);

                        str += UnitCode;
                        if (x != nX - 1)
                            str += ";";
                    }
                    sMap.Add(str);
                }

                if (xFlip) sMap.Reverse();

                return true;
            }
            catch (Exception ex)
            {
                string tmp = ex.Message;
                return false;
            }
        }
    }
}
