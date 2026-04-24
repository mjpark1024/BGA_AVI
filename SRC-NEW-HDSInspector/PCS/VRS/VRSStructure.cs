using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace PCS.VRS
{
    public  class VRS_Model_Info
    {
        /*제품 길이 mm*/
        public double Height { get; set; }
        /*제품 폭 mm*/
        public double Width { get; set; }
        /*제품 블록의 수 */
        public int BlockNum { get; set; }
        /*제품 블록에서 다음블록 까지의 길이 mm*/
        public double BlockPitch { get; set; }
        /*제품 길이방향 유닛의 수 */
        public int UnitNumX { get; set; }
        /*제품 폭방향 유닛의 수 */
        public int UnitNumY { get; set; }
        /*제품 유닛의 길이방향 피치 mm*/
        public double UnitPitchX { get; set; }
        /*제품 유닛의 폭방향 피치 mm*/
        public double UnitPitchY { get; set; }
        /*검사기에서의 제품 시작위치로 부터 섹션까지의 거리 mm*/
        /*카메라 순서대로 저장 할 것*/
        public Point[] SectionPos { get; set; }
        public Point[] IDSectionPos { get; set; }
        /*2D Barcode Length*/
        public int TD_Length { get; set; }
        /*Strip X Out*/
        public int X_Out { get; set; }
        /*Strip X Axis Xout*/
        public int X_Continue { get; set; }
        /*Strip Y Axis Xout*/
        public int Y_Continue { get; set; }
        /*모델별 Unit 기준 이미지가 저장된 경로*/
        public string Unit_Path { get; set; }

        public VRS_Model_Info()
        {
            Height = 0;
            Width = 0;
            BlockNum = 0;
            BlockPitch = 0;
            UnitNumX = 0;
            UnitNumY = 0;
            UnitPitchX = 0;
            UnitPitchX = 0;
            SectionPos = new Point[10];
            IDSectionPos = new Point[10];
        }
        public VRS_Model_Info(PCS.ELF.AVI.ModelInformation model_info, Point[] p_arr, Point[] p1_arr)
        {
            Height = model_info.Strip.Height;
            Width = model_info.Strip.Width;
            BlockNum = model_info.Strip.Block;
            BlockPitch = model_info.Strip.BlockGap;
            UnitNumX = model_info.Strip.UnitColumn;
            UnitNumY = model_info.Strip.UnitRow;
            UnitPitchX = model_info.Strip.UnitHeight;
            UnitPitchY = model_info.Strip.UnitWidth;
            X_Out = model_info.AutoNG;
            SectionPos = new Point[model_info.ListALLUseSur.Count()];
            Array.Copy(p_arr, SectionPos, p_arr.Length);
            IDSectionPos = new Point[model_info.ListALLUseSur.Count()];
            Array.Copy(p1_arr, IDSectionPos, p1_arr.Length);
        }
    }

    public class VRS_LOT_Info
    {
        /*해당 작업 노트 번호*/
        public string LotNo { get; set; }
        /*해당 작업 DataMatix Lot 번호*/
        public string ITS_LotNo { get; set; }
        /*해당 작업 설비 코드*/
        public string MC_Name { get; set; }
        /*해당 작업 그룹 명*/
        public string Group { get; set; }
        /*해당 작업 모델 명*/
        public string Model { get; set; }
        /*해당 작업 결과가 저장된 경로*/
        public string ResultPath { get; set; }
        /*해당 작업 맵이 저장된 경로*/
        public string Map_Path { get; set; }
        /*작업을 시작한 일시*/
        public DateTime RegDate { get; set; }

        public string ManagementCode { get; set; }

        public VRS_LOT_Info()
        {
            LotNo = "";
            MC_Name = "";
            Group = "";
            Model = "";
            ResultPath = "";
            Map_Path = "";
            ManagementCode = "";
        }
    }

    public class VRS_MAP_Info
    {
        /*제품 Type - Shot / Strip*/
        public string SubstrateType { get; set; }
        /*제품 길이방향 유닛의 수*/
        public int UnitNumX { get; set; }
        /*제품 폭방향 유닛의 수*/
        public int UnitNumY { get; set; }
        /*Panel의 경우, 한 Panel 내부 Strip의 수*/
        public int StripRow { get; set; }
        /*제품의 투입방향 - Gerber대비 정방향 : 0 / 역방향 : 1*/
        public int Orientation { get; set; }

        public VRS_MAP_Info()
        {
            SubstrateType = "";
            UnitNumX = 0;
            UnitNumY = 0;
            StripRow = 0;
            Orientation = 0;
        }

        public VRS_MAP_Info(string type, int nUnitNumX, int nUnitNumY, int nStripRow, int nOrientation, string path)
        {
            SubstrateType = type;
            UnitNumX = nUnitNumX;
            UnitNumY = nUnitNumY;
            StripRow = nStripRow;
            Orientation = nOrientation;
        }
    }

    public class VRS_MAP_Body
    {
        /*Strip ID - Lot No(9자리) + Panel ID(3자리) + Strip ID(1자리)*/
        public string SubstrateID { get; set; }
        /*SubstrateSide - 상부검사(Top) / 하부검사(Bot) / 전체Map(All)*/
        public string Side { get; set; }
        /*SubstrateAttach - 제품의 검사면 (CA / BA)*/
        public string Attach { get; set; }
        /*검사 설비에서 진행한 Shot/Strip Index를 나타낸다*/
        public int InspectIndex { get; set; }
        /*검사 설비에서의 원점 ( Left / Right )*/
        public string MachineOrigin { get; set; }
        /*Map File에서의 원점 ( Left / Right )*/
        public string MapOrigin { get; set; }
        /*폐기 Strip 여부*/
        public bool IsScrap { get; set; }
        /*MapName*/
        public string MapName { get; set; }
        /*MapVersion - 불량 항목 변경에 따라 Version 변경 가능성이 있기 때문에, Setting파일에서 읽어오는걸 추천*/
        public string MapVersion { get; set; }
        /*BinCodeType*/
        public string BinType { get; set; }
        /*양품 BinCode*/
        public string NullBin { get; set; }
        /*Mapping 정보 - Unit단위로 ";"필요, ex)FF;FF;FF;*/
        public string[] Map { get; set; }

        public VRS_MAP_Body()
        {
            SubstrateID = "";
            Side = "";
            Attach = "";
            InspectIndex = 0;
            MachineOrigin = "Left";
            MapOrigin = "Left";
            MapName = "BinCodeMap";
            MapVersion = "1.0";
            BinType = "HexaDecimal";
            NullBin = "FF";
        }

        public VRS_MAP_Body(string ID, string side, string attach, int index, string ver, string[] map)
        {
            SubstrateID = ID;
            Side = side;
            Attach = attach;
            InspectIndex = index;
            MapVersion = ver;
            Map = map;

            MachineOrigin = "Left";
            MapOrigin = "Left";
            MapName = "BinCodeMap";
            BinType = "HexaDecimal";
            NullBin = "FF";
        }

        public VRS_MAP_Body Clone()
        {
            VRS_MAP_Body res = new VRS_MAP_Body();
            res.SubstrateID = this.SubstrateID;
            res.Side = this.Side;
            res.Attach = this.Attach;
            res.InspectIndex = this.InspectIndex;
            res.MachineOrigin = this.MachineOrigin;
            res.MapOrigin = this.MapOrigin;
            res.MapName = this.MapName;
            res.MapVersion = this.MapVersion;
            res.BinType = this.BinType;
            res.NullBin = this.NullBin;
            res.Map = new string[this.Map.Count()];
            for (int i = 0; i < this.Map.Count(); i++)
                res.Map[i] = this.Map[i];

            return res;
        }
    }

    public class MapConverter
    {
        /*MES에서 사용되는 불량코드*/
        public int RectFill { get; set; }
        /*불량항목 명*/
        public string badName { get; set; }
        /*XML파일에서 사용되는 Bin코드*/
        public string binCode { get; set; }
    }

    public class VRS_Result
    {
        /*검사가 시작된 시간*/
        public DateTime StartTime { get; set; }
        /*검사가 최종적으로 끝난 시간*/
        public DateTime EndTime { get; set; }
        /*제품의 Type - (Shot / Strip)*/
        public string Type { get; set; }
        /*InspectIndex - 검사를 진행한 Shot/Strip 수량*/
        public int nCount { get; set; }
        /*한 Panel에 들어있는 Strip 수량 - Shot 단위에서만 사용*/
        public int nStripRow { get; set; }
        /*최종 수율*/
        public double Yield { get; set; }
        /*열별 최종 수율 - Shot 단위에서만 사용*/
        public List<double> Row_Yield { get; set; }
        /*불량항목 수*/
        public int codeCnt { get; set; }
        /*불량코드별 불량 수량 - 한 Unit내 여러 불량 발생시 대표 불량만 Count*/
        public List<KeyValuePair<string, int>> lstNgCount { get; set; }
        /*각 열별 - 불량코드별 불량 수량 - 한 Unit내 여러 불량 발생시 대표 불량만 Count - Shot 단위에서만 사용*/
        public List<List<KeyValuePair<string, int>>> row_lstNgCount { get; set; }

        public VRS_Result()
        {
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            Type = "";
            nCount = 0;
            nStripRow = 4;
            Yield = 0;
            Row_Yield = new List<double>();
            codeCnt = 0;
            lstNgCount = new List<KeyValuePair<string, int>>();
            row_lstNgCount = new List<List<KeyValuePair<string, int>>>();
        }
    }
}
