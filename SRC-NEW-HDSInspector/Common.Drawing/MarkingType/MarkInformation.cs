using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Common.Drawing.MarkingInformation
{
    public class MarkInformation : Mark
    {
        public const int YGap = 100;
        private double ResolutionX, ResolutionY;
        public MarkInformation(string astrModel, string IP, double resx, double resy, bool railirr, int nStep, bool Debug)
        {
            m_model = astrModel;
            m_markIP = IP;
            ResolutionX = resx;
            ResolutionY = resy;
            Step = nStep;
            RailIrr = railirr;
            if (railirr)
                m_currpath = "\\\\" + IP + "\\Mime\\mrk\\" + astrModel + "_1STEP.mrk";
                
            else
                m_currpath = "\\\\" + IP + "\\Mime\\mrk\\" + astrModel + ".mrk";
            m_bDebug = Debug;
            if (Debug) m_currpath = ConvertNetTolocalPath(m_currpath, m_sDrive);
        }

        public static bool ModelExists(string astrModel, string IP)
        {
            string path = "\\\\" + IP + "\\Mime\\mrk\\" + astrModel + ".mrk";
            if (File.Exists(path)) return true;
            else return false;
        }

        public bool ModelExists()//string astrModel, Surface surfaceType, string IP)
        {
            //if (surfaceType == Surface.TOP)
            //    m_currpath = "\\\\" + IP + "\\Mime\\mrk\\" + astrModel + "_Top.mrk";
            //else m_currpath = "\\\\" + IP + "\\Mime\\mrk\\" + astrModel + "_Bot.mrk";
            if (File.Exists(m_currpath)) return true;
            else return false;
        }

        public bool LoadMarkFile(System.Windows.Point pos, System.Windows.Point upos)
        {
            if (ModelExists())
            {
                LoadMark(pos, upos);
                if (Loaded) return true;
                else return false;
            }
            else return false;
        }

        public bool SaveAll(ModelMarkInfo newmodel)
        {
            return Save(newmodel);
        }

        #region Create Mark File
        public static bool CreateNewModel(ModelMarkInfo newmodel, string IP, MarkLogo logo)
        {
            bool railirr = newmodel.RailIrr;
            if (railirr)
            {
                for (int i = 0; i < newmodel.Step; i++)
                {
                    string currpath = "\\\\" + IP + "\\Mime\\mrk\\" + newmodel.ModelName + "_" + (i + 1).ToString() + "STEP.mrk";
                    if (m_bDebug) currpath = ConvertNetTolocalPath(currpath, m_sDrive);
                    if (File.Exists(currpath))
                    {
                        try
                        {
                            File.Delete(currpath);
                        }
                        catch
                        { return false; }
                    }
                    try
                    {
                        FileStream fs = File.Create(currpath); fs.Close();
                    }
                    catch { return false; }
                    int unitmark = newmodel.Unit;
                    int railmark = newmodel.Rail;
                    int rejmark = newmodel.Week + newmodel.Count;

                    List<string> lstTemplate = new List<string>();
                    try
                    {
                        List<string> lst = new List<string>();
                        lst.Add("// MiME Mark Project File Ver0.2");
                        lst.Add("");
                        //if (unitmark > 0)
                        //{
                        lst.Add("TemplateNum=0");
                        lstTemplate.Add("c:\\Mime\\tpl\\" + newmodel.ModelName + "_Unit.tpl");
                        lst.Add("TemplateName=c:\\Mime\\tpl\\" + newmodel.ModelName + "_Unit.tpl");
                        lst.Add("APC_AutoExecuteWhenLoad=0");
                        lst.Add("APC_OpenTypeForAutoAPC=0");
                        lst.Add("APC_AutoSaveParam=0");
                        // }
                        // if (rejmark > 0)
                        // {
                        lst.Add("TemplateNum=1");
                        lstTemplate.Add("c:\\Mime\\tpl\\" + newmodel.ModelName + "_Reject.tpl");
                        lst.Add("TemplateName=c:\\Mime\\tpl\\" + newmodel.ModelName + "_Reject.tpl");
                        lst.Add("APC_AutoExecuteWhenLoad=0");
                        lst.Add("APC_OpenTypeForAutoAPC=0");
                        lst.Add("APC_AutoSaveParam=0");
                        // }
                        // if (railmark > 0)
                        // {
                        lst.Add("TemplateNum=3");
                        lstTemplate.Add("c:\\Mime\\tpl\\" + newmodel.ModelName + "_" + (i + 1).ToString() + "STEP" + "_Rail.tpl");
                        lst.Add("TemplateName=c:\\Mime\\tpl\\" + newmodel.ModelName + "_" + (i + 1).ToString() + "STEP" + "_Rail.tpl");
                        lst.Add("APC_AutoExecuteWhenLoad=0");
                        lst.Add("APC_OpenTypeForAutoAPC=0");
                        lst.Add("APC_AutoSaveParam=0");
                        // }
                        lst.Add("[PARAMETER]");
                        string path = "";
                        for (int j = 0; j < 7; j++)
                        {
                            lst.Add("Parameter" + (j + 1).ToString() + "=" + "c:\\Mime\\Pen\\" + newmodel.ModelName + "_" + (j + 1).ToString() + ".prm");
                            path = Mark.ConvertNetPath("c:\\Mime\\Pen\\" + newmodel.ModelName + "_" + (j + 1).ToString() + ".prm", IP);
                            if (m_bDebug) path = ConvertNetTolocalPath(path, m_sDrive);
                            if (i == 0) File.Copy(Directory.GetCurrentDirectory() + "\\" + "..\\Markfile\\Default_" + (j + 1).ToString() + ".prm", path, true);
                        }
                        lst.Add("[END_PARAMETER]");
                        File.WriteAllLines(currpath, lst);
                    }
                    catch { return false; }
                    if (lstTemplate.Count > 0)
                    {
                        if (!CreateTemplate(lstTemplate, newmodel, logo, IP, i + 1))
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                string currpath = "\\\\" + IP + "\\Mime\\mrk\\" + newmodel.ModelName + ".mrk";
                if (m_bDebug) currpath = ConvertNetTolocalPath(currpath, m_sDrive);
                if (File.Exists(currpath))
                {
                    try
                    {
                        File.Delete(currpath);
                    }
                    catch
                    { return false; }
                }
                try
                {
                    FileStream fs = File.Create(currpath); fs.Close();
                }
                catch { return false; }

                int unitmark = newmodel.Unit;
                int railmark = newmodel.Rail;
                int rejmark = newmodel.Week + newmodel.Count;

                List<string> lstTemplate = new List<string>();
                try
                {
                    List<string> lst = new List<string>();
                    lst.Add("// MiME Mark Project File Ver0.2");
                    lst.Add("");
                    //if (unitmark > 0)
                    //{
                    lst.Add("TemplateNum=0");
                    lstTemplate.Add("c:\\Mime\\tpl\\" + newmodel.ModelName + "_Unit.tpl");
                    lst.Add("TemplateName=c:\\Mime\\tpl\\" + newmodel.ModelName + "_Unit.tpl");
                    lst.Add("APC_AutoExecuteWhenLoad=0");
                    lst.Add("APC_OpenTypeForAutoAPC=0");
                    lst.Add("APC_AutoSaveParam=0");

                    // }
                    // if (rejmark > 0)
                    // {
                    lst.Add("TemplateNum=1");
                    lstTemplate.Add("c:\\Mime\\tpl\\" + newmodel.ModelName + "_Reject.tpl");
                    lst.Add("TemplateName=c:\\Mime\\tpl\\" + newmodel.ModelName + "_Reject.tpl");
                    lst.Add("APC_AutoExecuteWhenLoad=0");
                    lst.Add("APC_OpenTypeForAutoAPC=0");
                    lst.Add("APC_AutoSaveParam=0");

                    //  }
                    //   if (railmark > 0)
                    //  {
                    lst.Add("TemplateNum=3");
                    lstTemplate.Add("c:\\Mime\\tpl\\" + newmodel.ModelName + "_Rail.tpl");
                    lst.Add("TemplateName=c:\\Mime\\tpl\\" + newmodel.ModelName + "_Rail.tpl");
                    lst.Add("APC_AutoExecuteWhenLoad=0");
                    lst.Add("APC_OpenTypeForAutoAPC=0");
                    lst.Add("APC_AutoSaveParam=0");

                    //    }
                    lst.Add("[PARAMETER]");
                    string path = "";
                    for (int i = 0; i < 7; i++)
                    {
                        lst.Add("Parameter" + (i + 1).ToString() + "=" + "c:\\Mime\\Pen\\" + newmodel.ModelName + "_" + (i + 1).ToString() + ".prm");
                        path = Mark.ConvertNetPath("c:\\Mime\\Pen\\" + newmodel.ModelName + "_" + (i + 1).ToString() + ".prm", IP);
                        if (m_bDebug) path = ConvertNetTolocalPath(path, m_sDrive);
                        File.Copy(Directory.GetCurrentDirectory() + "\\" + "..\\Markfile\\Default_" + (i + 1).ToString() + ".prm", path, true);
                    }
                    lst.Add("[END_PARAMETER]");
                    File.WriteAllLines(currpath, lst);
                }
                catch { return false; }
                if (lstTemplate.Count > 0)
                {
                    if (!CreateTemplate(lstTemplate, newmodel, logo, IP, 0))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool CreateTemplate(List<string> lstTemplate, ModelMarkInfo newmodel, MarkLogo logo, string IP, int nStep)
        {
            for (int i = 0; i < lstTemplate.Count; i++)
            {

                if (nStep == 0)
                {
                    string path = Mark.ConvertNetPath(lstTemplate[i], IP);
                    if (m_bDebug) path = ConvertNetTolocalPath(path, m_sDrive);
                    if (File.Exists(path))
                    {
                        try
                        {
                            File.Delete(path);
                        }
                        catch
                        { return false; }
                    }
                    try
                    {
                        FileStream fs = File.Create(path); fs.Close();
                    }
                    catch { return false; }
                    try
                    {
                        List<string> lst = new List<string>();
                        lst.Add("// MiME Template File Ver0.2");
                        lst.Add("");
                        string fpath = lstTemplate[i].Replace("tpl", "stp");
                        lst.Add("StripFile=" + fpath);
                        if (fpath.Contains("_Unit.stp")) CreateStripUnit(newmodel, fpath, IP);
                        if (fpath.Contains("_Rail.stp")) CreateStripRail(newmodel, fpath, IP);
                        if (fpath.Contains("_Reject.stp")) CreateStripReject(newmodel, fpath, IP);
                        lst.Add("NumberOfSem=6");
                        lst.Add("SemNumber=0");
                        fpath = lstTemplate[i].Replace("tpl", "sem");
                        if (fpath.Contains("_Unit.sem")) CreateSemUnit(newmodel, fpath, logo, IP);
                        if (fpath.Contains("_Rail.sem")) CreateSemRail(newmodel, fpath, logo, IP);
                        if (fpath.Contains("_Reject.sem")) CreateSemReject(newmodel, fpath, logo, IP);
                        lst.Add("SemFile=" + fpath);
                        File.WriteAllLines(path, lst);
                    }
                    catch
                    { return false; }
                }
                if (nStep == 1)
                {
                    string path = Mark.ConvertNetPath(lstTemplate[i], IP);
                    if (m_bDebug) path = ConvertNetTolocalPath(path, m_sDrive);
                    if (File.Exists(path))
                    {
                        try
                        {
                            File.Delete(path);
                        }
                        catch
                        { return false; }
                    }
                    try
                    {
                        FileStream fs = File.Create(path); fs.Close();
                    }
                    catch { return false; }
                    try
                    {
                        List<string> lst = new List<string>();
                        lst.Add("// MiME Template File Ver0.2");
                        lst.Add("");
                        string fpath = lstTemplate[i].Replace("tpl", "stp");
                        lst.Add("StripFile=" + fpath);
                        if (fpath.Contains("_Unit.stp")) CreateStripUnit(newmodel, fpath, IP);
                        if (fpath.Contains("_Rail.stp")) CreateStripRail(newmodel, fpath, IP);
                        if (fpath.Contains("_Reject.stp")) CreateStripReject(newmodel, fpath, IP);
                        lst.Add("NumberOfSem=6");
                        lst.Add("SemNumber=0");
                        fpath = lstTemplate[i].Replace("tpl", "sem");
                        if (fpath.Contains("_Rail.sem"))
                        {
                            fpath = fpath.Remove(fpath.IndexOf("_1STEP"), 6);
                        }
                        if (fpath.Contains("_Unit.sem")) CreateSemUnit(newmodel, fpath, logo, IP);
                        if (fpath.Contains("_Rail.sem")) CreateSemRail(newmodel, fpath, logo, IP);
                        if (fpath.Contains("_Reject.sem")) CreateSemReject(newmodel, fpath, logo, IP);
                        lst.Add("SemFile=" + fpath);
                        File.WriteAllLines(path, lst);
                    }
                    catch
                    { return false; }
                }
                if (nStep >= 2)
                {
                    string path = Mark.ConvertNetPath(lstTemplate[i], IP);
                    if (m_bDebug) path = ConvertNetTolocalPath(path, m_sDrive);
                    string fpath = lstTemplate[i].Replace("tpl", "stp");
                    if (fpath.Contains("_Rail.stp"))
                    {
                        if (File.Exists(path))
                        {
                            try
                            {
                                File.Delete(path);
                            }
                            catch
                            { return false; }
                        }
                        try
                        {
                            FileStream fs = File.Create(path); fs.Close();
                        }
                        catch { return false; }
                        try
                        {
                            List<string> lst = new List<string>();
                            lst.Add("// MiME Template File Ver0.2");
                            lst.Add("");

                            if (fpath.Contains("_Rail.stp")) CreateStripRail(newmodel, fpath, IP);
                            else fpath = fpath.Replace("1STEP", nStep.ToString() + "STEP");
                            lst.Add("StripFile=" + fpath);
                            lst.Add("NumberOfSem=6");
                            lst.Add("SemNumber=0");
                            fpath = lstTemplate[i].Replace("tpl", "sem");
                            if (fpath.Contains("_Rail.sem"))
                            {
                                fpath = fpath.Remove(fpath.IndexOf("_" + nStep.ToString() + "STEP"), 6);
                            }
                            lst.Add("SemFile=" + fpath);
                            File.WriteAllLines(path, lst);
                        }
                        catch
                        { return false; }
                    }
                }
            }
            return true;
        }

        public static bool CreateStripUnit(ModelMarkInfo model, string path, string IP)
        {
            string Path = Mark.ConvertNetPath(path, IP);
            if (m_bDebug) Path = ConvertNetTolocalPath(Path, m_sDrive);
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(Path); fs.Close();
            }
            catch { return false; }
            try
            {
                List<string> lst = new List<string>();
                lst.Add("// MiME Strip File Ver0.3");
                lst.Add("");
                lst.Add("[Strip information]");
                lst.Add("Version=" + 0.3);
                lst.Add(" ");
                lst.Add("[base strip]");
                lst.Add("m_iStripX=" + model.StepUnits.ToString());
                lst.Add("m_iStripY=" + model.UnitRow.ToString());
                lst.Add("m_dStripSizeX=120.000000");
                lst.Add("m_dStripSizeY=120.000000");
                lst.Add("m_iStripStyle=1");
                lst.Add("// 0 = landscape, 1 = portrait");
                lst.Add("m_iMarkingSide=0");
                lst.Add("// 0 = top, 1 = back");
                lst.Add("m_dXPitch=" + model.UnitWidth.ToString("f6"));
                lst.Add("m_dYPitch=" + model.UnitHeight.ToString("f6"));
                lst.Add("m_sStripName=UNTITILED");
                lst.Add("");
                lst.Add("[Base Chip]");
                lst.Add("m_iIDPosition=0");
                lst.Add("m_iLeadNum=10");
                lst.Add("m_dUnitWidth=" + model.UnitWidth.ToString("f6"));
                lst.Add("m_dUnitHeight=" + model.UnitHeight.ToString("f6"));
                lst.Add("m_dIDRadius=1.000000");
                lst.Add("m_iIDPosition_Vision=0");
                lst.Add("m_unVisionMagnification=1000");
                lst.Add("");
                lst.Add("[Special Strip Infomation]");
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchX[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchY[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_dSpecialPitch[" + i.ToString() + "]=0.000000");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchApply[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchDir[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDX[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDY[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDPos[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDApply[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDDir[" + i.ToString() + "]=0");
                }
                lst.Add("m_dIDFOffset=0.000000");
                lst.Add("m_iIDFDirection=0");
                lst.Add("");
                lst.Add("[Marking Field Setting]");

                lst.Add("m_dFirstX=5.000000");
                if (model.ID == 1 && model.Selection > 0)
                    lst.Add("m_dFirstY=35.000000");
                else lst.Add("m_dFirstY=80.000000");
                lst.Add("m_dSecondX=0.000000");
                lst.Add("m_dSecondY=0.000000");
                lst.Add("m_dThirdX=0.000000");
                lst.Add("m_dThirdY=0.000000");
                lst.Add("m_dUpperMasterX=0.000000");
                lst.Add("m_dUpperMasterY=0.000000");
                lst.Add("m_dUpperSlaveX=0.000000");
                lst.Add("m_dUpperSlaveY=0.000000");
                lst.Add("m_dLowerMasterX=0.000000");
                lst.Add("m_dLowerMasterY=0.000000");
                lst.Add("m_dLowerSlaveX=0.000000");
                lst.Add("m_dLowerSlaveY=0.000000");
                lst.Add("m_nMarkingPos=1");
                lst.Add("m_dRotateMarkingField=0.000000");
                lst.Add("m_dLowerRotateMarkingField=0.000000");
                lst.Add("");
                lst.Add("[Selective Marking]");
                string u = "";
                string s = "";
                for (int i = 0; i < model.StepUnits * model.UnitRow; i++)
                {
                    u += "1";
                    s += "0";
                }
                lst.Add("UnitAttribute=" + u);
                lst.Add("SEMType=" + s);
                lst.Add("");
                lst.Add("[One Chip Offset]");
                lst.Add("OneChipOffset=");
                for (int i = 0; i < model.StepUnits * model.UnitRow; i++)
                {
                    lst.Add("0.000000, 0.000000, 0.000000");
                }
                lst.Add("[END One Chip Offset]");
                lst.Add("");
                lst.Add("[BiV data]");
                lst.Add("m_dlXAbsolute=0.000000");
                lst.Add("m_dlYAbsolute=0.000000");
                lst.Add("m_nXPixel=640");
                lst.Add("m_nYPixel=480");
                lst.Add("m_dlXFoV=0.000000");
                lst.Add("m_dlYFoV=0.000000");
                lst.Add("m_dlXPtoF=0.000000");
                lst.Add("m_dlYPtoF=0.000000");
                lst.Add("[END of BiV data]");
                lst.Add("");
                lst.Add("[Manual ID]");
                lst.Add("m_bUseFirstID=0");
                lst.Add("m_dID1XPos=0.000000");
                lst.Add("m_dID1YPos=0.000000");
                lst.Add("m_dID1Diameter=0.000000");
                lst.Add("m_bUseSecondID=0");
                lst.Add("m_dID2XPos=0.000000");
                lst.Add("m_dID2YPos=0.000000");
                lst.Add("m_dID2Diameter=0.000000");
                lst.Add("m_bUseThirdID=0");
                lst.Add("m_dID3XPos=0.000000");
                lst.Add("m_dID3YPos=0.000000");
                lst.Add("m_dID3Diameter=0.000000");
                lst.Add("m_bUseFourthID=0");
                lst.Add("m_dID3XPos=0.000000");
                lst.Add("m_dID3YPos=0.000000");
                lst.Add("m_dID3Diameter=0.000000");
                lst.Add("[End of Manual ID]");
                lst.Add("");
                lst.Add("[Marking Order]");
                lst.Add("//m_iMarkStartPos : 0 = TL, 1 = TR, 2 = BL, 3 = BR");
                lst.Add("m_iMarkStartPos=0");
                lst.Add("//m_iMarkMajor : 0 = Row Major, 1: Column Major");
                lst.Add("m_iMarkMajor=0");
                lst.Add("//m_iMarkType : 0 = S(Zigzag) Type, 1: 11(One direction) Type");
                lst.Add("m_iMarkType=0");
                lst.Add("[End of Marking Order]");
                lst.Add("");
                lst.Add("[Custom data]");
                lst.Add("// m_nGoldPointType : Substrate Gold Point Type Section For Motech Handler");
                lst.Add("m_nGoldPointType=-1");
                lst.Add("// m_dMarkingPositionX : Marking Position for RobotHIF");
                lst.Add("m_dMarkingPositionX=0.000000");
                lst.Add("m_dMarkingPositionY=0.000000");
                lst.Add("[END of Custom data]");
                File.WriteAllLines(Path, lst);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool CreateStripReject(ModelMarkInfo model, string path, string IP)
        {
            string Path = Mark.ConvertNetPath(path, IP);
            if (m_bDebug) Path = ConvertNetTolocalPath(Path, m_sDrive);
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(Path); fs.Close();
            }
            catch { return false; }
            try
            {
                List<string> lst = new List<string>();
                lst.Add("// MiME Strip File Ver0.3");
                lst.Add("");
                lst.Add("[Strip information]");
                lst.Add("Version=" + 0.3);
                lst.Add(" ");
                lst.Add("[base strip]");
                lst.Add("m_iStripX=1");
                lst.Add("m_iStripY=1");
                lst.Add("m_dStripSizeX=120.000000");
                lst.Add("m_dStripSizeY=120.000000");
                lst.Add("m_iStripStyle=1");
                lst.Add("// 0 = landscape, 1 = portrait");
                lst.Add("m_iMarkingSide=0");
                lst.Add("// 0 = top, 1 = back");
                lst.Add("m_dXPitch=1.000000");
                lst.Add("m_dYPitch=1.000000");
                lst.Add("m_sStripName=UNTITILED");
                lst.Add("");
                lst.Add("[Base Chip]");
                lst.Add("m_iIDPosition=0");
                lst.Add("m_iLeadNum=10");
                lst.Add("m_dUnitWidth=120.000000");
                lst.Add("m_dUnitHeight=120.000000");
                lst.Add("m_dIDRadius=1.000000");
                lst.Add("m_iIDPosition_Vision=0");
                lst.Add("m_unVisionMagnification=1000");
                lst.Add("");
                lst.Add("[Special Strip Infomation]");
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchX[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchY[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_dSpecialPitch[" + i.ToString() + "]=0.000000");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchApply[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchDir[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDX[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDY[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDPos[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDApply[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDDir[" + i.ToString() + "]=0");
                }
                lst.Add("m_dIDFOffset=0.000000");
                lst.Add("m_iIDFDirection=0");
                lst.Add("");
                lst.Add("[Marking Field Setting]");
                lst.Add("m_dFirstX=0.000000");
                lst.Add("m_dFirstY=0.000000");
                lst.Add("m_dSecondX=0.000000");
                lst.Add("m_dSecondY=0.000000");
                lst.Add("m_dThirdX=0.000000");
                lst.Add("m_dThirdY=0.000000");
                lst.Add("m_dUpperMasterX=0.000000");
                lst.Add("m_dUpperMasterY=0.000000");
                lst.Add("m_dUpperSlaveX=0.000000");
                lst.Add("m_dUpperSlaveY=0.000000");
                lst.Add("m_dLowerMasterX=0.000000");
                lst.Add("m_dLowerMasterY=0.000000");
                lst.Add("m_dLowerSlaveX=0.000000");
                lst.Add("m_dLowerSlaveY=0.000000");
                lst.Add("m_nMarkingPos=1");
                lst.Add("m_dRotateMarkingField=0.000000");
                lst.Add("m_dLowerRotateMarkingField=0.000000");
                lst.Add("");
                lst.Add("[Selective Marking]");
                lst.Add("UnitAttribute=0");
                lst.Add("SEMType=1");
                lst.Add("");
                lst.Add("[One Chip Offset]");
                lst.Add("OneChipOffset=");
                lst.Add("0.000000, 0.000000, 0.000000");
                lst.Add("[END One Chip Offset]");
                lst.Add("");
                lst.Add("[BiV data]");
                lst.Add("m_dlXAbsolute=0.000000");
                lst.Add("m_dlYAbsolute=0.000000");
                lst.Add("m_nXPixel=640");
                lst.Add("m_nYPixel=480");
                lst.Add("m_dlXFoV=0.000000");
                lst.Add("m_dlYFoV=0.000000");
                lst.Add("m_dlXPtoF=0.000000");
                lst.Add("m_dlYPtoF=0.000000");
                lst.Add("[END of BiV data]");
                lst.Add("");
                lst.Add("[Manual ID]");
                lst.Add("m_bUseFirstID=0");
                lst.Add("m_dID1XPos=0.000000");
                lst.Add("m_dID1YPos=0.000000");
                lst.Add("m_dID1Diameter=0.000000");
                lst.Add("m_bUseSecondID=0");
                lst.Add("m_dID2XPos=0.000000");
                lst.Add("m_dID2YPos=0.000000");
                lst.Add("m_dID2Diameter=0.000000");
                lst.Add("m_bUseThirdID=0");
                lst.Add("m_dID3XPos=0.000000");
                lst.Add("m_dID3YPos=0.000000");
                lst.Add("m_dID3Diameter=0.000000");
                lst.Add("m_bUseFourthID=0");
                lst.Add("m_dID3XPos=0.000000");
                lst.Add("m_dID3YPos=0.000000");
                lst.Add("m_dID3Diameter=0.000000");
                lst.Add("[End of Manual ID]");
                lst.Add("");
                lst.Add("[Marking Order]");
                lst.Add("//m_iMarkStartPos : 0 = TL, 1 = TR, 2 = BL, 3 = BR");
                lst.Add("m_iMarkStartPos=0");
                lst.Add("//m_iMarkMajor : 0 = Row Major, 1: Column Major");
                lst.Add("m_iMarkMajor=0");
                lst.Add("//m_iMarkType : 0 = S(Zigzag) Type, 1: 11(One direction) Type");
                lst.Add("m_iMarkType=0");
                lst.Add("[End of Marking Order]");
                lst.Add("");
                lst.Add("[Custom data]");
                lst.Add("// m_nGoldPointType : Substrate Gold Point Type Section For Motech Handler");
                lst.Add("m_nGoldPointType=-1");
                lst.Add("// m_dMarkingPositionX : Marking Position for RobotHIF");
                lst.Add("m_dMarkingPositionX=0.000000");
                lst.Add("m_dMarkingPositionY=0.000000");
                lst.Add("[END of Custom data]");
                File.WriteAllLines(Path, lst);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool CreateStripRail(ModelMarkInfo model, string path, string IP)
        {
            string Path = Mark.ConvertNetPath(path, IP);
            if (m_bDebug) Path = ConvertNetTolocalPath(Path, m_sDrive);
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(Path); fs.Close();
            }
            catch { return false; }
            try
            {
                List<string> lst = new List<string>();
                lst.Add("// MiME Strip File Ver0.3");
                lst.Add("");
                lst.Add("[Strip information]");
                lst.Add("Version=" + 0.3);
                lst.Add(" ");
                lst.Add("[base strip]");
                lst.Add("m_iStripX=" + model.StepUnits * model.UnitRow);
                lst.Add("m_iStripY=1");
                lst.Add("m_dStripSizeX=120.000000");
                lst.Add("m_dStripSizeY=120.000000");
                lst.Add("m_iStripStyle=1");
                lst.Add("// 0 = landscape, 1 = portrait");
                lst.Add("m_iMarkingSide=0");
                lst.Add("// 0 = top, 1 = back");
                lst.Add("m_dXPitch=0.10000");
                lst.Add("m_dYPitch=0.10000");
                lst.Add("m_sStripName=UNTITILED");
                lst.Add("");
                lst.Add("[Base Chip]");
                lst.Add("m_iIDPosition=0");
                lst.Add("m_iLeadNum=10");
                lst.Add("m_dUnitWidth=0.10000");
                lst.Add("m_dUnitHeight=0.10000");
                lst.Add("m_dIDRadius=1.000000");
                lst.Add("m_iIDPosition_Vision=0");
                lst.Add("m_unVisionMagnification=1000");
                lst.Add("");
                lst.Add("[Special Strip Infomation]");
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchX[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchY[" + i.ToString() + "]=1");
                }
                lst.Add("ms_dSpecialPitch[0]=" + (model.UnitWidth - (model.UnitRow * 0.1)).ToString("f6"));
                for (int i = 1; i < 6; i++)
                {
                    lst.Add("ms_dSpecialPitch[" + i.ToString() + "]=0.000000");
                }
                lst.Add("ms_iSpecialPitchApply[0]=" + model.UnitRow.ToString());
                for (int i = 1; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchApply[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchDir[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDX[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDY[" + i.ToString() + "]=1");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDPos[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDApply[" + i.ToString() + "]=0");
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDDir[" + i.ToString() + "]=0");
                }
                lst.Add("m_dIDFOffset=0.000000");
                lst.Add("m_iIDFDirection=0");
                lst.Add("");
                lst.Add("[Marking Field Setting]");
                lst.Add("m_dFirstX=5.000000");
                if (model.ID == 1 && model.Selection > 0)
                    lst.Add("m_dFirstY=30.000000");
                else lst.Add("m_dFirstY=91.000000");
                lst.Add("m_dSecondX=0.000000");
                lst.Add("m_dSecondY=0.000000");
                lst.Add("m_dThirdX=0.000000");
                lst.Add("m_dThirdY=0.000000");
                lst.Add("m_dUpperMasterX=0.000000");
                lst.Add("m_dUpperMasterY=0.000000");
                lst.Add("m_dUpperSlaveX=0.000000");
                lst.Add("m_dUpperSlaveY=0.000000");
                lst.Add("m_dLowerMasterX=0.000000");
                lst.Add("m_dLowerMasterY=0.000000");
                lst.Add("m_dLowerSlaveX=0.000000");
                lst.Add("m_dLowerSlaveY=0.000000");
                lst.Add("m_nMarkingPos=1");
                lst.Add("m_dRotateMarkingField=0.000000");
                lst.Add("m_dLowerRotateMarkingField=0.000000");
                lst.Add("");
                lst.Add("[Selective Marking]");
                string u = "";
                string s = "";
                for (int i = 0; i < model.StepUnits * model.UnitRow; i++)
                {
                    u += "1";
                    s += "0";
                }
                lst.Add("UnitAttribute=" + u);
                lst.Add("SEMType=" + s);
                lst.Add("");
                lst.Add("[One Chip Offset]");
                lst.Add("OneChipOffset=");
                for (int i = 0; i < model.StepUnits; i++)
                {
                    for (int j = 0; j < model.UnitRow; j++)
                    {
                        double p = 0.8 * j;
                        lst.Add(p.ToString("f6") + ", 0.000000, 0.000000");
                    }
                }
                lst.Add("[END One Chip Offset]");
                lst.Add("");
                lst.Add("[BiV data]");
                lst.Add("m_dlXAbsolute=0.000000");
                lst.Add("m_dlYAbsolute=0.000000");
                lst.Add("m_nXPixel=640");
                lst.Add("m_nYPixel=480");
                lst.Add("m_dlXFoV=0.000000");
                lst.Add("m_dlYFoV=0.000000");
                lst.Add("m_dlXPtoF=0.000000");
                lst.Add("m_dlYPtoF=0.000000");
                lst.Add("[END of BiV data]");
                lst.Add("");
                lst.Add("[Manual ID]");
                lst.Add("m_bUseFirstID=0");
                lst.Add("m_dID1XPos=0.000000");
                lst.Add("m_dID1YPos=0.000000");
                lst.Add("m_dID1Diameter=0.000000");
                lst.Add("m_bUseSecondID=0");
                lst.Add("m_dID2XPos=0.000000");
                lst.Add("m_dID2YPos=0.000000");
                lst.Add("m_dID2Diameter=0.000000");
                lst.Add("m_bUseThirdID=0");
                lst.Add("m_dID3XPos=0.000000");
                lst.Add("m_dID3YPos=0.000000");
                lst.Add("m_dID3Diameter=0.000000");
                lst.Add("m_bUseFourthID=0");
                lst.Add("m_dID3XPos=0.000000");
                lst.Add("m_dID3YPos=0.000000");
                lst.Add("m_dID3Diameter=0.000000");
                lst.Add("[End of Manual ID]");
                lst.Add("");
                lst.Add("[Marking Order]");
                lst.Add("//m_iMarkStartPos : 0 = TL, 1 = TR, 2 = BL, 3 = BR");
                lst.Add("m_iMarkStartPos=0");
                lst.Add("//m_iMarkMajor : 0 = Row Major, 1: Column Major");
                lst.Add("m_iMarkMajor=0");
                lst.Add("//m_iMarkType : 0 = S(Zigzag) Type, 1: 11(One direction) Type");
                lst.Add("m_iMarkType=0");
                lst.Add("[End of Marking Order]");
                lst.Add("");
                lst.Add("[Custom data]");
                lst.Add("// m_nGoldPointType : Substrate Gold Point Type Section For Motech Handler");
                lst.Add("m_nGoldPointType=-1");
                lst.Add("// m_dMarkingPositionX : Marking Position for RobotHIF");
                lst.Add("m_dMarkingPositionX=0.000000");
                lst.Add("m_dMarkingPositionY=0.000000");
                lst.Add("[END of Custom data]");
                File.WriteAllLines(Path, lst);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool CreateSemUnit(ModelMarkInfo model, string path, MarkLogo logo, string IP)
        {
            string Path = Mark.ConvertNetPath(path, IP);
            if (m_bDebug) Path = ConvertNetTolocalPath(Path, m_sDrive);
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(Path); fs.Close();
            }
            catch { return false; }
            try
            {
                List<string> lst = new List<string>();
                lst.Add("// MiME SemData File Ver0.5");
                lst.Add("");
                lst.Add("Unit=1");
                lst.Add("ChipXSize=" + model.UnitWidth.ToString("f3"));
                lst.Add("ChipYSize=" + model.UnitHeight.ToString("f3"));
                lst.Add("SemRotateStatus=4760534");
                //////Obects
                string file = "";
                int unitid = model.Unit;

                switch (unitid)
                {
                    case 1: file = logo.UnitTriangle[0]; break;
                    case 2: file = logo.UnitRect[4]; break;
                    case 3: file = logo.UnitTriangle[0]; break;
                    case 4: file = logo.UnitRect[4]; break;
                    case 5: file = logo.UnitRect[0]; break;
                    case 6: file = logo.UnitEllipse[0]; break;
                    case 7: file = logo.UnitCross[0]; break;
                    case 8: file = logo.UnitLine[0]; break;
                    case 9: file = logo.UnitTriangle[0]; break;
                    default: file = logo.UnitRect[0]; break;
                }
                string[] st = file.Split(' ', 'X');
                double width = Convert.ToDouble(st[1]);
                double height = Convert.ToDouble(st[2]);
                {
                    lst.Add('\t' + "[HPGL OBJECT]");
                    lst.Add('\t' + "ObjNumber=0");
                    lst.Add('\t' + "FileName=C:\\Mime\\Logo\\" + file);
                    lst.Add("\t\t" + "[Attribute]");
                    lst.Add("\t\tObjType=HPGL");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=180.000000");
                    lst.Add("\t\t\tUNIT=1");
                    lst.Add("\t\t\tDispLeft=3.610");
                    lst.Add("\t\t\tDispRight=" + (3.610 + width).ToString("f3"));
                    lst.Add("\t\t\tDispTop=2.474");
                    lst.Add("\t\t\tDispBottom=" + (2.904 + height).ToString("f3"));
                    lst.Add("\t\t\tDispRect=1971,5720,2239,5954");
                    double cpx = 2239 - (2239 - 1971) / 2.0;
                    double cpy = 5954 - (5954 - 5720) / 2.0;
                    lst.Add("\t\t\tMatrixA=0.000000");
                    lst.Add("\t\t\tMatrixB=-0.778446");
                    lst.Add("\t\t\tMatrixC=-1.279989");
                    lst.Add("\t\t\tMatrixD=0.000000");
                    lst.Add("\t\t\tMatrixX=" + cpx.ToString("f6"));
                    lst.Add("\t\t\tMatrixY=" + cpy.ToString("f6"));
                    lst.Add("\t\t\tMatrixZ=1.000000");
                    lst.Add("\t\t\tBasicMatrixA=1.000000");
                    lst.Add("\t\t\tBasicMatrixB=0.000000");
                    lst.Add("\t\t\tBasicMatrixC=0.000000");
                    lst.Add("\t\t\tBasicMatrixD=-1.000000");
                    lst.Add("\t\t\tBasicMatrixX=" + cpx.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixY=" + cpy.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixZ=1.000000");
                    lst.Add("\t\t\tRotateMatrixA=0.000000");
                    lst.Add("\t\t\tRotateMatrixB=-1.000000");
                    lst.Add("\t\t\tRotateMatrixC=1.000000");
                    lst.Add("\t\t\tRotateMatrixD=0.000000");
                    lst.Add("\t\t\tRotateMatrixX=0.003906");
                    lst.Add("\t\t\tRotateMatrixY=0.003906");
                    lst.Add("\t\t\tRotateMatrixZ=1.000000");

                    lst.Add("\t\t\tRotateAngle=270.000000");
                    lst.Add("\t\t\tParaNumber=0");
                    lst.Add("\t\t\tLockObject=0");
                    lst.Add("\t\t[END Attribute]");
                    lst.Add("\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\tSpecial Type=0");
                    lst.Add("\t\tReference Number 1=0");
                    lst.Add("\t\tReference Number 2=0");
                    lst.Add("\t\t[END_SPECIAL_TYPE]");
                    lst.Add('\t' + "LogoAlignMode=0");
                    lst.Add('\t' + "[END_ObjectHPGL]");
                    lst.Add(" ");
                }
                if (unitid == 3 || unitid == 4 || unitid == 9)
                {
                    file = logo.UnitRect[0];
                    st = file.Split(' ', 'X');
                    width = Convert.ToDouble(st[1]);
                    height = Convert.ToDouble(st[1]);
                    lst.Add('\t' + "[HPGL OBJECT]");
                    lst.Add('\t' + "ObjNumber=1");
                    lst.Add('\t' + "FileName=C:\\Mime\\Logo\\" + file);
                    lst.Add("\t\t" + "[Attribute]");
                    lst.Add("\t\tObjType=HPGL");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=180.000000");
                    lst.Add("\t\t\tUNIT=1");
                    lst.Add("\t\t\tDispLeft=6.610");
                    lst.Add("\t\t\tDispRight=" + (6.610 + width).ToString("f3"));
                    lst.Add("\t\t\tDispTop=10.474");
                    lst.Add("\t\t\tDispBottom=" + (10.904 + height).ToString("f3"));
                    lst.Add("\t\t\tDispRect=1971,5720,2239,5954");
                    double cpx = 2239 - (2239 - 1971) / 2.0;
                    double cpy = 5954 - (5954 - 5720) / 2.0;
                    lst.Add("\t\t\tMatrixA=0.000000");
                    lst.Add("\t\t\tMatrixB=-0.778433");
                    lst.Add("\t\t\tMatrixC=-1.279969");
                    lst.Add("\t\t\tMatrixD=0.000000");
                    lst.Add("\t\t\tMatrixX=" + cpx.ToString("f6"));
                    lst.Add("\t\t\tMatrixY=" + cpy.ToString("f6"));
                    lst.Add("\t\t\tMatrixZ=1.000000");
                    lst.Add("\t\t\tBasicMatrixA=1.000000");
                    lst.Add("\t\t\tBasicMatrixB=0.000000");
                    lst.Add("\t\t\tBasicMatrixC=0.000000");
                    lst.Add("\t\t\tBasicMatrixD=-1.000000");
                    lst.Add("\t\t\tBasicMatrixX=" + cpx.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixY=" + cpy.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixZ=1.000000");
                    lst.Add("\t\t\tRotateMatrixA=0.000000");
                    lst.Add("\t\t\tRotateMatrixB=-1.000000");
                    lst.Add("\t\t\tRotateMatrixC=1.000000");
                    lst.Add("\t\t\tRotateMatrixD=0.000000");
                    lst.Add("\t\t\tRotateMatrixX=0.003906");
                    lst.Add("\t\t\tRotateMatrixY=0.003906");
                    lst.Add("\t\t\tRotateMatrixZ=1.000000");

                    lst.Add("\t\t\tRotateAngle=270.000000");
                    lst.Add("\t\t\tParaNumber=1");
                    lst.Add("\t\t\tLockObject=0");
                    lst.Add("\t\t[END Attribute]");
                    lst.Add("\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\tSpecial Type=0");
                    lst.Add("\t\tReference Number 1=0");
                    lst.Add("\t\tReference Number 2=0");
                    lst.Add("\t\t[END_SPECIAL_TYPE]");
                    lst.Add('\t' + "LogoAlignMode=0");
                    lst.Add('\t' + "[END_ObjectHPGL]");
                    lst.Add(" ");
                }

                if (unitid == 9)
                {
                    file = logo.UnitRect[5];
                    st = file.Split(' ', 'X');
                    width = Convert.ToDouble(st[1]);
                    height = Convert.ToDouble(st[1]);
                    lst.Add('\t' + "[HPGL OBJECT]");
                    lst.Add('\t' + "ObjNumber=2");
                    lst.Add('\t' + "FileName=C:\\Mime\\Logo\\" + file);
                    lst.Add("\t\t" + "[Attribute]");
                    lst.Add("\t\tObjType=HPGL");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=180.000000");
                    lst.Add("\t\t\tUNIT=1");
                    lst.Add("\t\t\tDispLeft=6.610");
                    lst.Add("\t\t\tDispRight=" + (6.610 + width).ToString("f3"));
                    lst.Add("\t\t\tDispTop=10.474");
                    lst.Add("\t\t\tDispBottom=" + (10.904 + height).ToString("f3"));
                    lst.Add("\t\t\tDispRect=1971,5720,2239,5954");
                    double cpx = 2239 - (2239 - 1971) / 2.0;
                    double cpy = 5954 - (5954 - 5720) / 2.0;
                    lst.Add("\t\t\tMatrixA=0.000000");
                    lst.Add("\t\t\tMatrixB=-0.778433");
                    lst.Add("\t\t\tMatrixC=-1.279969");
                    lst.Add("\t\t\tMatrixD=0.000000");
                    lst.Add("\t\t\tMatrixX=" + cpx.ToString("f6"));
                    lst.Add("\t\t\tMatrixY=" + cpy.ToString("f6"));
                    lst.Add("\t\t\tMatrixZ=1.000000");
                    lst.Add("\t\t\tBasicMatrixA=1.000000");
                    lst.Add("\t\t\tBasicMatrixB=0.000000");
                    lst.Add("\t\t\tBasicMatrixC=0.000000");
                    lst.Add("\t\t\tBasicMatrixD=-1.000000");
                    lst.Add("\t\t\tBasicMatrixX=" + cpx.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixY=" + cpy.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixZ=1.000000");
                    lst.Add("\t\t\tRotateMatrixA=0.000000");
                    lst.Add("\t\t\tRotateMatrixB=-1.000000");
                    lst.Add("\t\t\tRotateMatrixC=1.000000");
                    lst.Add("\t\t\tRotateMatrixD=0.000000");
                    lst.Add("\t\t\tRotateMatrixX=0.003906");
                    lst.Add("\t\t\tRotateMatrixY=0.003906");
                    lst.Add("\t\t\tRotateMatrixZ=1.000000");

                    lst.Add("\t\t\tRotateAngle=270.000000");
                    lst.Add("\t\t\tParaNumber=6");
                    lst.Add("\t\t\tLockObject=0");
                    lst.Add("\t\t[END Attribute]");
                    lst.Add("\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\tSpecial Type=0");
                    lst.Add("\t\tReference Number 1=0");
                    lst.Add("\t\tReference Number 2=0");
                    lst.Add("\t\t[END_SPECIAL_TYPE]");
                    lst.Add('\t' + "LogoAlignMode=0");
                    lst.Add('\t' + "[END_ObjectHPGL]");
                    lst.Add(" ");
                }

                lst.Add('\t' + "[GUIDE LINE]");
                for (int i = 0; i < 8; i++)
                {
                    lst.Add('\t' + "ItemNum=" + i.ToString());
                    lst.Add('\t' + "ItemUse=0");
                    lst.Add('\t' + "ItemAlign=0");
                    lst.Add('\t' + "ItemPosX=0.000");
                    lst.Add('\t' + "ItemPosY=0.000");
                    lst.Add('\t' + "ItemWidth=0.000");
                    lst.Add('\t' + "ItemHeight=0.000");
                }
                lst.Add('\t' + "[END GUIDE LINE]");
                lst.Add('\t' + "[AUTO ALIGNMENT INFO]");
                lst.Add('\t' + "Use Auto Horizontal Align=0");
                lst.Add('\t' + "Use Auto Vertical Align=0");
                lst.Add('\t' + "[END AUTO ALIGNMENT INFO]");
                lst.Add("[END SemData]");
                File.WriteAllLines(Path, lst);
            }
            catch { return false; }
            return true;
        }

        public static bool CreateSemRail(ModelMarkInfo model, string path, MarkLogo logo, string IP)
        {
            string Path = Mark.ConvertNetPath(path, IP);
            if (m_bDebug) Path = ConvertNetTolocalPath(Path, m_sDrive);
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(Path); fs.Close();
            }
            catch { return false; }
            try
            {
                List<string> lst = new List<string>();
                lst.Add("// MiME SemData File Ver0.5");
                lst.Add("");
                lst.Add("Unit=1");
                lst.Add("ChipXSize=0.100");
                lst.Add("ChipYSize=0.100");
                lst.Add("SemRotateStatus=0");
                //////Obects
                string file = "";
                int unitid = model.Rail;
                bool bfc = model.FlipChip;

                switch (unitid)
                {
                    case 1:
                    case 2:
                        {
                            if (bfc) file = logo.RailEllipse[0];
                            else file = logo.RailEllipse[1];
                            break;
                        }
                    case 3: file = logo.RailRect[0]; break;
                    case 4: file = logo.RailRect[0]; break;
                    default: file = logo.RailEllipse[0]; break;
                }
                string[] st = file.Split(' ', 'X');
                double width = Convert.ToDouble(st[1]);
                double height = Convert.ToDouble(st[2]);
                {
                    lst.Add('\t' + "[HPGL OBJECT]");
                    lst.Add('\t' + "ObjNumber=0");
                    lst.Add('\t' + "FileName=C:\\Mime\\Logo\\" + file);
                    lst.Add("\t\t" + "[Attribute]");
                    lst.Add("\t\tObjType=HPGL");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=180.000000");
                    lst.Add("\t\t\tUNIT=1");
                    if (bfc && (unitid == 1 || unitid == 2))
                    {
                        lst.Add("\t\t\tDispLeft=0.000");
                        lst.Add("\t\t\tDispRight=0.600");
                        lst.Add("\t\t\tDispTop=0.000");
                        lst.Add("\t\t\tDispBottom=0.601");
                        lst.Add("\t\t\tDispRect=0,0,327,328");
                        //double cpx = 2239 - (2239 - 1971) / 2.0;
                        //double cpy = 5954 - (5954 - 5720) / 2.0;
                        lst.Add("\t\t\tMatrixA=-0.026235");
                        lst.Add("\t\t\tMatrixB=-1.496145");
                        lst.Add("\t\t\tMatrixC=-1.503027");
                        lst.Add("\t\t\tMatrixD=0.026115");
                        lst.Add("\t\t\tMatrixX=163.837500");
                        lst.Add("\t\t\tMatrixY=164.110563");
                        lst.Add("\t\t\tMatrixZ=1.000000");
                        lst.Add("\t\t\tBasicMatrixA=1.000000");
                        lst.Add("\t\t\tBasicMatrixB=0.000000");
                        lst.Add("\t\t\tBasicMatrixC=0.000000");
                        lst.Add("\t\t\tBasicMatrixD=-1.000000");
                        lst.Add("\t\t\tBasicMatrixX=163.837500");
                        lst.Add("\t\t\tBasicMatrixY=164.110563");
                        lst.Add("\t\t\tBasicMatrixZ=1.000000");
                        lst.Add("\t\t\tRotateMatrixA=-0.017452");
                        lst.Add("\t\t\tRotateMatrixB=0.999848");
                        lst.Add("\t\t\tRotateMatrixC=-0.999848");
                        lst.Add("\t\t\tRotateMatrixD=-0.017452");
                        lst.Add("\t\t\tRotateMatrixX=0.003906");
                        lst.Add("\t\t\tRotateMatrixY=0.003906");
                        lst.Add("\t\t\tRotateMatrixZ=1.000000");
                        lst.Add("\t\t\tRotateAngle=269.000000");
                    }
                    else
                    {
                        //if ((surface == "_TOP") && model.TopFlipChip)
                        // {
                        lst.Add("\t\t\tDispLeft=0.000");
                        lst.Add("\t\t\tDispRight=" + width.ToString("f3"));
                        lst.Add("\t\t\tDispTop=0.000");
                        lst.Add("\t\t\tDispBottom=" + height.ToString("f3"));
                        lst.Add("\t\t\tDispRect=1971,5720,2239,5954");
                        double cpx = 2239 - (2239 - 1971) / 2.0;
                        double cpy = 5954 - (5954 - 5720) / 2.0;
                        lst.Add("\t\t\tMatrixA=1.390546");
                        lst.Add("\t\t\tMatrixB=0.000000");
                        lst.Add("\t\t\tMatrixC=0.000000");
                        lst.Add("\t\t\tMatrixD=-1.382074");
                        lst.Add("\t\t\tMatrixX=" + cpx.ToString("f6"));
                        lst.Add("\t\t\tMatrixY=" + cpy.ToString("f6"));
                        lst.Add("\t\t\tMatrixZ=1.000000");
                        lst.Add("\t\t\tBasicMatrixA=1.000000");
                        lst.Add("\t\t\tBasicMatrixB=0.000000");
                        lst.Add("\t\t\tBasicMatrixC=0.000000");
                        lst.Add("\t\t\tBasicMatrixD=-1.000000");
                        lst.Add("\t\t\tBasicMatrixX=" + cpx.ToString("f6"));
                        lst.Add("\t\t\tBasicMatrixY=" + cpy.ToString("f6"));
                        lst.Add("\t\t\tBasicMatrixZ=1.000000");
                        lst.Add("\t\t\tRotateMatrixA=1.000000");
                        lst.Add("\t\t\tRotateMatrixB=0.000000");
                        lst.Add("\t\t\tRotateMatrixC=0.000000");
                        lst.Add("\t\t\tRotateMatrixD=1.000000");
                        lst.Add("\t\t\tRotateMatrixX=0.000000");
                        lst.Add("\t\t\tRotateMatrixY=0.000000");
                        lst.Add("\t\t\tRotateMatrixZ=1.000000");
                        lst.Add("\t\t\tRotateAngle=0.000000");
                    }
                    //}
                    //else
                    //{
                    //    width=height =0.82;
                    //    lst.Add("\t\t\tDispLeft=0.000");
                    //    lst.Add("\t\t\tDispRight=0.823");
                    //    lst.Add("\t\t\tDispTop=0.000");
                    //    lst.Add("\t\t\tDispBottom=0.821");
                    //    lst.Add("\t\t\tDispRect=0,0,449,448");
                    //    lst.Add("\t\t\tMatrixA=0.071899");
                    //    lst.Add("\t\t\tMatrixB=0.071467");
                    //    lst.Add("\t\t\tMatrixC=0.000000");
                    //    lst.Add("\t\t\tMatrixD=-1.363565");
                    //    lst.Add("\t\t\tMatrixX=224.730438");
                    //    lst.Add("\t\t\tMatrixY=224.184313");
                    //    lst.Add("\t\t\tMatrixZ=1.000000");
                    //    lst.Add("\t\t\tBasicMatrixA=1.000000");
                    //    lst.Add("\t\t\tBasicMatrixB=0.000000");
                    //    lst.Add("\t\t\tBasicMatrixC=0.000000");
                    //    lst.Add("\t\t\tBasicMatrixD=-1.000000");
                    //    lst.Add("\t\t\tBasicMatrixX=224.730438");
                    //    lst.Add("\t\t\tBasicMatrixY=224.184313");
                    //    lst.Add("\t\t\tBasicMatrixZ=1.000000");
                    //    lst.Add("\t\t\tRotateMatrixA=0.998630");
                    //    lst.Add("\t\t\tRotateMatrixB=-0.052336");
                    //    lst.Add("\t\t\tRotateMatrixC=0.052336");
                    //    lst.Add("\t\t\tRotateMatrixD=0.998630");
                    //    lst.Add("\t\t\tRotateMatrixX=0.003906");
                    //    lst.Add("\t\t\tRotateMatrixY=0.003906");
                    //    lst.Add("\t\t\tRotateMatrixZ=1.000000");
                    //    lst.Add("\t\t\tRotateAngle=3.000000");
                    //}

                    lst.Add("\t\t\tParaNumber=2");
                    lst.Add("\t\t\tLockObject=0");
                    lst.Add("\t\t[END Attribute]");
                    lst.Add("\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\tSpecial Type=0");
                    lst.Add("\t\tReference Number 1=0");
                    lst.Add("\t\tReference Number 2=0");
                    lst.Add("\t\t[END_SPECIAL_TYPE]");
                    lst.Add('\t' + "LogoAlignMode=0");
                    lst.Add('\t' + "[END_ObjectHPGL]");
                    lst.Add(" ");
                }


                lst.Add('\t' + "[GUIDE LINE]");
                for (int i = 0; i < 8; i++)
                {
                    lst.Add('\t' + "ItemNum=" + i.ToString());
                    lst.Add('\t' + "ItemUse=0");
                    lst.Add('\t' + "ItemAlign=0");
                    lst.Add('\t' + "ItemPosX=0.000");
                    lst.Add('\t' + "ItemPosY=0.000");
                    lst.Add('\t' + "ItemWidth=0.000");
                    lst.Add('\t' + "ItemHeight=0.000");
                }
                lst.Add('\t' + "[END GUIDE LINE]");
                lst.Add('\t' + "[AUTO ALIGNMENT INFO]");
                lst.Add('\t' + "Use Auto Horizontal Align=0");
                lst.Add('\t' + "Use Auto Vertical Align=0");
                lst.Add('\t' + "[END AUTO ALIGNMENT INFO]");
                lst.Add("[END SemData]");
                File.WriteAllLines(Path, lst);
            }
            catch { return false; }
            return true;
        }

        public static bool CreateSemReject(ModelMarkInfo model, string path, MarkLogo logo, string IP)
        {
            string Path = Mark.ConvertNetPath(path, IP);
            if (m_bDebug) Path = ConvertNetTolocalPath(Path, m_sDrive);
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(Path); fs.Close();
            }
            catch { return false; }
            try
            {
                List<string> lst = new List<string>();
                lst.Add("// MiME SemData File Ver0.5");
                lst.Add("");
                lst.Add("Unit=1");
                lst.Add("ChipXSize=120.000");
                lst.Add("ChipYSize=120.000");
                lst.Add("SemRotateStatus=0");
                //////Obects

                string file = logo.FontFile[0];
                string rfile = "";
                string[] st = file.Split(' ', 'X');
                double ch = 4.0 * logo.FontScale[0];
                double rch = 0.0;

                int week = model.Week;
                int count = model.Count;
                int wl = model.WeekLoc;
                int cl = model.CountLoc;
                if (model.CountRW > 0)
                {
                    rfile = logo.FontRemark[model.CountRW - 1];
                    rch = 4.0 * logo.FontRScale[model.CountRW - 1];
                }

                int nObject = 0;
                if (model.IDMark > 0)
                {
                    for (int i = 0; i < model.ID_Count + 1; i++)
                    {
                        lst.Add('\t' + "[2D_BARCODE_OBJECT]");
                        lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                        nObject++;
                        lst.Add("\t\t[BARCODE_ATTRIBUTE]");
                        lst.Add("\t\tVer=1");
                        lst.Add("\t\tContents=H19C1234123");
                        lst.Add("\t\tBarcode Type=71");
                        lst.Add("\t\tStart Mode=0");
                        lst.Add("\t\t2D Barcode Width=1.5000");
                        lst.Add("\t\t2D Barcode Height=1.5000");
                        lst.Add("\t\tDot Ratio=1.0000");
                        lst.Add("\t\tDot Line Num=4");
                        lst.Add("\t\tSelect Line Box=1");
                        lst.Add("\t\t2D Barcode Check Digit=0");
                        lst.Add("\t\tColumn and Row Index=0");
                        lst.Add("\t\tReverse Marking=0");
                        lst.Add("\t\tMake L=0");
                        lst.Add("\t\tHorizontal Line=0");
                        lst.Add("\t\tZigzag Type=0");
                        lst.Add("\t\tSampling num=2");
                        lst.Add("\t\tChange Cell Size=1");
                        lst.Add("\t\tResoultion=40");
                        lst.Add("\t\tShot Count=1");
                        lst.Add("\t\tLine Margin Ratio=0");
                        lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t\tStyle=0");
                        lst.Add("\t\t\tIncType=0");
                        lst.Add("\t\t\tStartNumber=0");
                        lst.Add("\t\t\tRangeStartNumber=0");
                        lst.Add("\t\t\tRangeEndNumber=0");
                        lst.Add("\t\t\tPreFix=");
                        lst.Add("\t\t\tPostFix=");
                        lst.Add("\t\t\tMaxCharSize=5");
                        lst.Add("\t\t\tSavedCurrentNumber=0");
                        lst.Add("\t\t\tSCHEME_Order=-1");
                        lst.Add("\t\t\tSCHEME_Position=-1");
                        lst.Add("\t\t\tSCHEME_Type=-1");
                        lst.Add("\t\t\tSCHEME_Numbering=-1");
                        lst.Add("\t\t\tSaveFileWhenUpdate=0");
                        lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                        lst.Add("\t\t\tNumericSystemType=0");
                        lst.Add("\t\t\tIncreaseLevels=1");
                        lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                        lst.Add("\t\t\tSpecial Type=2");
                        lst.Add("\t\t\tReference Number 1=2");
                        lst.Add("\t\t\tReference Number 2=0");
                        lst.Add("\t\t\t[END_SPECIAL_TYPE]");
                        lst.Add("\t\t[END_SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t[END_BARCODE_ATTRIBUTE]");
                        lst.Add("\t\t[Attribute]");
                        lst.Add("\t\tObjType=2DBARCODE");
                        lst.Add("\t\tIsSelect=0");
                        lst.Add("\t\tIsUpdate=1");
                        lst.Add("\t\t\tAxisField=180.000000");
                        lst.Add("\t\t\tUNIT=1");
                        if (i == 0)
                        {
                            if (model.IDMark == 1)
                            {
                                lst.Add("\t\t\tDispLeft=3.287");
                                lst.Add("\t\t\tDispRight=4.787");
                            }
                            else
                            {
                                lst.Add("\t\t\tDispLeft=" + (model.StepPitch - 4.787).ToString("f3"));
                                lst.Add("\t\t\tDispRight=" + (model.StepPitch - 3.287).ToString("f3"));
                            }
                            lst.Add("\t\t\tDispTop=83.364");
                            lst.Add("\t\t\tDispBottom=84.864");
                            lst.Add("\t\t\tDispRect=1795,45527,2614,46346");
                            lst.Add("\t\t\tMatrixA=0.000000");
                            lst.Add("\t\t\tMatrixB=-1.462837");
                            lst.Add("\t\t\tMatrixC=-1.462837");
                            lst.Add("\t\t\tMatrixD=-0.000000");
                            lst.Add("\t\t\tMatrixX=2204.734375");
                            lst.Add("\t\t\tMatrixY=45936.789063");
                            lst.Add("\t\t\tMatrixZ=1.000000");
                            lst.Add("\t\t\tBasicMatrixA=1.000000");
                            lst.Add("\t\t\tBasicMatrixB=0.000000");
                            lst.Add("\t\t\tBasicMatrixC=0.000000");
                            lst.Add("\t\t\tBasicMatrixD=-1.000000");
                            lst.Add("\t\t\tBasicMatrixX=2204.734375");
                            lst.Add("\t\t\tBasicMatrixY=45936.789063");
                            lst.Add("\t\t\tBasicMatrixZ=1.000000");
                            lst.Add("\t\t\tRotateMatrixA=-1.000000");
                            lst.Add("\t\t\tRotateMatrixB=-0.000000");
                            lst.Add("\t\t\tRotateMatrixC=0.000000");
                            lst.Add("\t\t\tRotateMatrixD=-1.000000");
                            lst.Add("\t\t\tRotateMatrixX=0.003906");
                            lst.Add("\t\t\tRotateMatrixY=0.003906");
                            lst.Add("\t\t\tRotateMatrixZ=1.000000");
                            lst.Add("\t\t\tRotateAngle=270.000000");
                        }
                        else
                        {
                            if (model.IDMark == 1)
                            {
                                lst.Add("\t\t\tDispLeft=3.287");
                                lst.Add("\t\t\tDispRight=4.787");
                            }
                            else
                            {
                                lst.Add("\t\t\tDispLeft=" + (model.StepPitch - 4.787).ToString("f3"));
                                lst.Add("\t\t\tDispRight=" + (model.StepPitch - 3.287).ToString("f3"));
                            }
                            lst.Add("\t\t\tDispTop=81.651");
                            lst.Add("\t\t\tDispBottom=83.151");
                            lst.Add("\t\t\tDispRect=1795,44591,2614,45410");
                            lst.Add("\t\t\tMatrixA=0.000000");
                            lst.Add("\t\t\tMatrixB=-1.462845");
                            lst.Add("\t\t\tMatrixC=-1.462845");
                            lst.Add("\t\t\tMatrixD=-0.000000");
                            lst.Add("\t\t\tMatrixX=2204.730469");
                            lst.Add("\t\t\tMatrixY=45001.277344");
                            lst.Add("\t\t\tMatrixZ=1.000000");
                            lst.Add("\t\t\tBasicMatrixA=1.000000");
                            lst.Add("\t\t\tBasicMatrixB=0.000000");
                            lst.Add("\t\t\tBasicMatrixC=0.000000");
                            lst.Add("\t\t\tBasicMatrixD=-1.000000");
                            lst.Add("\t\t\tBasicMatrixX=2204.730469");
                            lst.Add("\t\t\tBasicMatrixY=45001.277344");
                            lst.Add("\t\t\tBasicMatrixZ=1.000000");
                            lst.Add("\t\t\tRotateMatrixA=0.000000");
                            lst.Add("\t\t\tRotateMatrixB=1.000000");
                            lst.Add("\t\t\tRotateMatrixC=-1.000000");
                            lst.Add("\t\t\tRotateMatrixD=0.000000");
                            lst.Add("\t\t\tRotateMatrixX=0.003906");
                            lst.Add("\t\t\tRotateMatrixY=0.003906");
                            lst.Add("\t\t\tRotateMatrixZ=1.000000");
                            lst.Add("\t\t\tRotateAngle=270.000000");
                        }
                        lst.Add("\t\t\tParaNumber=5");
                        lst.Add("\t\t\tLockObject=0");
                        lst.Add("\t\t[END Attribute]");
                        lst.Add("\t[END_2D_BARCODE_OBJECT]");
                    }
                    if (model.ID_Text)
                    {
                        lst.Add("\t[TEXT OBJECT]");
                        lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                        nObject++;
                        lst.Add("\t\t[TEXT_ATTRIBUTE]");
                        lst.Add("\t\tContent=H19C1234123");
                        lst.Add("\t\tMaxNum=1000");
                        lst.Add("\t\tAlignChar=0");
                        lst.Add("\t\tAlignMode=1");
                        lst.Add("\t\tCapitalHeight=1.1989");
                        lst.Add("\t\tCharGap=0.1782");
                        lst.Add("\t\tFontName=C:\\MiME\\Font\\Sbga3.fnt");
                        lst.Add("\t\tHeight=1.4741");
                        lst.Add("\t\tLineGap=0.3685");
                        lst.Add("\t\tWidth=1.4248");
                        lst.Add("\t\tSpaceSize=0.9858");
                        lst.Add("\t\tUseCapitalLineGap=0");
                        lst.Add("\t\tUseFixStringLenght=0");
                        lst.Add("\t\tUseLeftGapofeachChar=0");
                        lst.Add("\t\tUseFixNumberofChar=0");
                        lst.Add("\t\tFixedStrSize=1000");
                        lst.Add("\t\tFixNumberofChar=0");
                        lst.Add("\t\tItalicAngle=0");
                        lst.Add("\t\tNTMark_WidthStype=0");
                        lst.Add("\t\tUseStringCutting=0");
                        lst.Add("\t\tFirst=0");
                        lst.Add("\t\tCount=0");
                        lst.Add("\t\tUseCriterionChar=0");
                        lst.Add("\t\tCriterionChar=W");
                        lst.Add("\t\tUseFixStringLenToCurrent=0");
                        lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t\tStyle=0");
                        lst.Add("\t\t\tIncType=0");
                        lst.Add("\t\t\tStartNumber=0");
                        lst.Add("\t\t\tRangeStartNumber=0");
                        lst.Add("\t\t\tRangeEndNumber=0");
                        lst.Add("\t\t\tPreFix=");
                        lst.Add("\t\t\tPostFix=");
                        lst.Add("\t\t\tMaxCharSize=5");
                        lst.Add("\t\t\tSavedCurrentNumber=0");
                        lst.Add("\t\t\tSCHEME_Order=-1");
                        lst.Add("\t\t\tSCHEME_Position=-1");
                        lst.Add("\t\t\tSCHEME_Type=-1");
                        lst.Add("\t\t\tSCHEME_Numbering=-1");
                        lst.Add("\t\t\tSaveFileWhenUpdate=0");
                        lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                        lst.Add("\t\t\tNumericSystemType=0");
                        lst.Add("\t\t\tIncreaseLevels=1");
                        lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                        lst.Add("\t\t\t\tSpecial Type=2");
                        lst.Add("\t\t\t\tReference Number 1=2");
                        lst.Add("\t\t\t\tReference Number 2=0");
                        lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                        lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                        lst.Add("\t\t[Attribute]");
                        lst.Add("\t\tObjType=TEXT");
                        lst.Add("\t\tIsSelect=0");
                        lst.Add("\t\tIsUpdate=1");
                        lst.Add("\t\t\tAxisField=120.000000");
                        lst.Add("\t\t\tUNIT=1");
                        lst.Add("\t\t\tDispLeft=3.349");
                        lst.Add("\t\t\tDispRight=4.549");
                        lst.Add("\t\t\tDispTop=70.707");
                        lst.Add("\t\t\tDispBottom=80.938");
                        lst.Add("\t\t\tDispRect=1829,38614,2484,44202");
                        lst.Add("\t\t\tMatrixA=0.000000");
                        lst.Add("\t\t\tMatrixB=-1.000000");
                        lst.Add("\t\t\tMatrixC=-1.000000");
                        lst.Add("\t\t\tMatrixD=-0.000000");
                        lst.Add("\t\t\tMatrixX=2156.679688");
                        lst.Add("\t\t\tMatrixY=41408.664063");
                        lst.Add("\t\t\tMatrixZ=1.000000");
                        lst.Add("\t\t\tBasicMatrixA=1.000000");
                        lst.Add("\t\t\tBasicMatrixB=0.000000");
                        lst.Add("\t\t\tBasicMatrixC=0.000000");
                        lst.Add("\t\t\tBasicMatrixD=-1.000000");
                        lst.Add("\t\t\tBasicMatrixX=2156.679688");
                        lst.Add("\t\t\tBasicMatrixY=41408.664063");
                        lst.Add("\t\t\tBasicMatrixZ=1.000000");
                        lst.Add("\t\t\tRotateMatrixA=1.000000");
                        lst.Add("\t\t\tRotateMatrixB=0.000000");
                        lst.Add("\t\t\tRotateMatrixC=-1.000000");
                        lst.Add("\t\t\tRotateMatrixD=1.000000");
                        lst.Add("\t\t\tRotateMatrixX=0.003906");
                        lst.Add("\t\t\tRotateMatrixY=0.003906");
                        lst.Add("\t\t\tRotateMatrixZ=1.000000");
                        lst.Add("\t\t\tRotateAngle=270.000000");
                        lst.Add("\t\t\tParaNumber=5");
                        lst.Add("\t\t\tLockObject=0");
                        lst.Add("\t\t[END Attribute]");
                        lst.Add("\t[END_ObjectTEXT]");
                    }
                }

                if (count == 0 && week == 0)
                {
                    lst.Add('\t' + "[TEXT OBJECT]");
                    lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                    nObject++;
                    lst.Add("\t\t[TEXT_ATTRIBUTE]");
                    lst.Add("\t\tContent= ");
                    lst.Add("\t\tMaxNum=1000");
                    lst.Add("\t\tAlignChar=1");
                    lst.Add("\t\tAlignMode=0");
                    lst.Add("\t\tCapitalHeight=1.9520");
                    lst.Add("\t\tCharGap=0.0400");
                    lst.Add("\t\tFontName=C:\\Mime\\Font\\" + file);
                    lst.Add("\t\tHeight=2.5");
                    lst.Add("\t\tLineGap=1.0017");
                    lst.Add("\t\tWidth=1.2000");
                    lst.Add("\t\tSpaceSize=2.9998");
                    lst.Add("\t\tUseCapitalLineGap=0");
                    lst.Add("\t\tUseFixStringLenght=0");
                    lst.Add("\t\tUseLeftGapofeachChar=0");
                    lst.Add("\t\tUseFixNumberofChar=0");
                    lst.Add("\t\tFixedStrSize=1000");
                    lst.Add("\t\tFixNumberofChar=0");
                    lst.Add("\t\tItalicAngle=0");
                    lst.Add("\t\tNTMark_WidthStype=0");
                    lst.Add("\t\tUseStringCutting=0");
                    lst.Add("\t\tFirst=0");
                    lst.Add("\t\tCount=0");
                    lst.Add("\t\tUseCriterionChar=0");
                    lst.Add("\t\tCriterionChar=W");
                    lst.Add("\t\tUseFixStringLenToCurrent=0");
                    lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t\tStyle=0");
                    lst.Add("\t\t\tIncType=0");
                    lst.Add("\t\t\tStartNumber=0");
                    lst.Add("\t\t\tRangeStartNumber=0");
                    lst.Add("\t\t\tRangeEndNumber=0");
                    lst.Add("\t\t\tPreFix=");
                    lst.Add("\t\t\tPostFix=");
                    lst.Add("\t\t\tMaxCharSize=5");
                    lst.Add("\t\t\tSavedCurrentNumber=0");
                    lst.Add("\t\t\tSCHEME_Order=0");
                    lst.Add("\t\t\tSCHEME_Position=0");
                    lst.Add("\t\t\tSCHEME_Type=1");
                    lst.Add("\t\t\tSCHEME_Numbering=0");
                    lst.Add("\t\t\tSaveFileWhenUpdate=0");
                    lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                    lst.Add("\t\t\tNumericSystemType=0");
                    lst.Add("\t\t\tIncreaseLevels=1");
                    lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\t\t\tSpecial Type=2");
                    lst.Add("\t\t\t\tReference Number 1=1");
                    lst.Add("\t\t\t\tReference Number 2=0");
                    lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                    lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                    lst.Add("\t\t[Attribute]");
                    lst.Add("\t\tObjType=TEXT");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=120.000000");
                    lst.Add("\t\t\tUNIT=1");

                    lst.Add("\t\t\tDispLeft=0.184");
                    lst.Add("\t\t\tDispRight=3.184");

                    lst.Add("\t\t\tDispTop=50.700");
                    lst.Add("\t\t\tDispBottom=53.100");

                    lst.Add("\t\t\tDispRect=1738,29367,3376,31061");
                    double cpx = 3376 - (3376 - 1738) / 2.0;
                    double cpy = 31061 - (31061 - 29367) / 2.0;
                    lst.Add("\t\t\tMatrixA=1.000000");
                    lst.Add("\t\t\tMatrixB=0.000000");
                    lst.Add("\t\t\tMatrixC=0.000000");
                    lst.Add("\t\t\tMatrixD=-1.000000");
                    lst.Add("\t\t\tMatrixX=" + cpx.ToString("f6"));
                    lst.Add("\t\t\tMatrixY=" + cpy.ToString("f6"));
                    lst.Add("\t\t\tMatrixZ=1.000000");
                    lst.Add("\t\t\tBasicMatrixA=1.000000");
                    lst.Add("\t\t\tBasicMatrixB=0.000000");
                    lst.Add("\t\t\tBasicMatrixC=0.000000");
                    lst.Add("\t\t\tBasicMatrixD=-1.000000");
                    lst.Add("\t\t\tBasicMatrixX=" + cpx.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixY=" + cpy.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixZ=1.000000");
                    lst.Add("\t\t\tRotateMatrixA=1.000000");
                    lst.Add("\t\t\tRotateMatrixB=0.000000");
                    lst.Add("\t\t\tRotateMatrixC=0.000000");
                    lst.Add("\t\t\tRotateMatrixD=1.000000");
                    lst.Add("\t\t\tRotateMatrixX=0.003906");
                    lst.Add("\t\t\tRotateMatrixY=0.003906");
                    lst.Add("\t\t\tRotateMatrixZ=1.000000");

                    lst.Add("\t\t\tRotateAngle=0.000000");

                    lst.Add("\t\t\tParaNumber=4");
                    lst.Add("\t\t\tLockObject=0");
                    lst.Add("\t\t[END Attribute]");
                    lst.Add('\t' + "[END_ObjectTEXT]");
                    lst.Add(" ");
                }
                if (count > 0)
                {
                    if (model.Mode == 1)
                    {
                        lst.Add('\t' + "[TEXT OBJECT]");
                        lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                        nObject++;
                        lst.Add("\t\t[TEXT_ATTRIBUTE]");
                        lst.Add("\t\tContent= ");
                        lst.Add("\t\tMaxNum=1000");
                        lst.Add("\t\tAlignChar=1");
                        lst.Add("\t\tAlignMode=0");
                        lst.Add("\t\tCapitalHeight=" + rch.ToString("0.0000"));
                        lst.Add("\t\tCharGap=0.0400");
                        lst.Add("\t\tFontName=C:\\Mime\\Font\\" + rfile);
                        lst.Add("\t\tHeight=4.0000");
                        lst.Add("\t\tLineGap=1.0017");
                        lst.Add("\t\tWidth=1.0999");
                        lst.Add("\t\tSpaceSize=2.9998");
                        lst.Add("\t\tUseCapitalLineGap=0");
                        lst.Add("\t\tUseFixStringLenght=0");
                        lst.Add("\t\tUseLeftGapofeachChar=0");
                        lst.Add("\t\tUseFixNumberofChar=0");
                        lst.Add("\t\tFixedStrSize=1000");
                        lst.Add("\t\tFixNumberofChar=0");
                        lst.Add("\t\tItalicAngle=0");
                        lst.Add("\t\tNTMark_WidthStype=0");
                        lst.Add("\t\tUseStringCutting=0");
                        lst.Add("\t\tFirst=0");
                        lst.Add("\t\tCount=0");
                        lst.Add("\t\tUseCriterionChar=0");
                        lst.Add("\t\tCriterionChar=W");
                        lst.Add("\t\tUseFixStringLenToCurrent=0");
                        lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t\tStyle=0");
                        lst.Add("\t\t\tIncType=0");
                        lst.Add("\t\t\tStartNumber=0");
                        lst.Add("\t\t\tRangeStartNumber=0");
                        lst.Add("\t\t\tRangeEndNumber=0");
                        lst.Add("\t\t\tPreFix=");
                        lst.Add("\t\t\tPostFix=");
                        lst.Add("\t\t\tMaxCharSize=5");
                        lst.Add("\t\t\tSavedCurrentNumber=0");
                        lst.Add("\t\t\tSCHEME_Order=0");
                        lst.Add("\t\t\tSCHEME_Position=0");
                        lst.Add("\t\t\tSCHEME_Type=1");
                        lst.Add("\t\t\tSCHEME_Numbering=0");
                        lst.Add("\t\t\tSaveFileWhenUpdate=0");
                        lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                        lst.Add("\t\t\tNumericSystemType=0");
                        lst.Add("\t\t\tIncreaseLevels=1");
                        lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                        lst.Add("\t\t\t\tSpecial Type=2");
                        lst.Add("\t\t\t\tReference Number 1=1");
                        lst.Add("\t\t\t\tReference Number 2=0");
                        lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                        lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                        lst.Add("\t\t[Attribute]");
                        lst.Add("\t\tObjType=TEXT");
                        lst.Add("\t\tIsSelect=0");
                        lst.Add("\t\tIsUpdate=0");
                        lst.Add("\t\t\tAxisField=120.000000");
                        lst.Add("\t\t\tUNIT=1");
                        if (cl == 0)
                        {
                            lst.Add("\t\t\tDispLeft=0.184");
                            lst.Add("\t\t\tDispRight=3.184");
                        }
                        else
                        {
                            lst.Add("\t\t\tDispLeft=" + (model.StepPitch - 3.184).ToString("f3"));
                            lst.Add("\t\t\tDispRight=" + (model.StepPitch - 0.184).ToString("f3"));
                        }
                        lst.Add("\t\t\tDispTop=50.700");
                        lst.Add("\t\t\tDispBottom=53.100");
                        lst.Add("\t\t\tDispRect=1738,29367,3376,31061");
                        lst.Add("\t\t\tMatrixA=-1.000000");
                        lst.Add("\t\t\tMatrixB=0.000000");
                        lst.Add("\t\t\tMatrixC=0.000000");
                        lst.Add("\t\t\tMatrixD=1.000000");
                        lst.Add("\t\t\tMatrixX=2557.816406");
                        lst.Add("\t\t\tMatrixY=30214.402344");
                        lst.Add("\t\t\tMatrixZ=1.000000");
                        lst.Add("\t\t\tBasicMatrixA=1.000000");
                        lst.Add("\t\t\tBasicMatrixB=0.000000");
                        lst.Add("\t\t\tBasicMatrixC=0.000000");
                        lst.Add("\t\t\tBasicMatrixD=-1.000000");
                        lst.Add("\t\t\tBasicMatrixX=2557.816406");
                        lst.Add("\t\t\tBasicMatrixY=30214.402344");
                        lst.Add("\t\t\tBasicMatrixZ=1.000000");
                        lst.Add("\t\t\tRotateMatrixA=-1.000000");
                        lst.Add("\t\t\tRotateMatrixB=0.000000");
                        lst.Add("\t\t\tRotateMatrixC=0.000000");
                        lst.Add("\t\t\tRotateMatrixD=-1.000000");
                        lst.Add("\t\t\tRotateMatrixX=0.003906");
                        lst.Add("\t\t\tRotateMatrixY=0.003906");
                        lst.Add("\t\t\tRotateMatrixZ=1.000000");
                        if (count == 1) lst.Add("\t\t\tRotateAngle=0.000000");
                        if (count == 2) lst.Add("\t\t\tRotateAngle=90.000000");
                        if (count == 3) lst.Add("\t\t\tRotateAngle=180.000000");
                        if (count == 4) lst.Add("\t\t\tRotateAngle=270.000000");
                        lst.Add("\t\t\tParaNumber=4");
                        lst.Add("\t\t\tLockObject=0");
                        lst.Add("\t\t[END Attribute]");
                        lst.Add('\t' + "[END_ObjectTEXT]");
                        lst.Add(" ");
                    }
                    else if (model.Mode == 2)
                    {
                        lst.Add('\t' + "[TEXT OBJECT]");
                        lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                        nObject++;
                        lst.Add("\t\t[TEXT_ATTRIBUTE]");
                        lst.Add("\t\tContent= ");
                        lst.Add("\t\tMaxNum=1000");
                        lst.Add("\t\tAlignChar=1");
                        lst.Add("\t\tAlignMode=0");
                        lst.Add("\t\tCapitalHeight=" + rch.ToString("0.0000"));
                        lst.Add("\t\tCharGap=0.0400");
                        lst.Add("\t\tFontName=C:\\Mime\\Font\\" + rfile);
                        lst.Add("\t\tHeight=4.0000");
                        lst.Add("\t\tLineGap=1.0017");
                        lst.Add("\t\tWidth=1.0999");
                        lst.Add("\t\tSpaceSize=2.9998");
                        lst.Add("\t\tUseCapitalLineGap=0");
                        lst.Add("\t\tUseFixStringLenght=0");
                        lst.Add("\t\tUseLeftGapofeachChar=0");
                        lst.Add("\t\tUseFixNumberofChar=0");
                        lst.Add("\t\tFixedStrSize=1000");
                        lst.Add("\t\tFixNumberofChar=0");
                        lst.Add("\t\tItalicAngle=0");
                        lst.Add("\t\tNTMark_WidthStype=0");
                        lst.Add("\t\tUseStringCutting=0");
                        lst.Add("\t\tFirst=0");
                        lst.Add("\t\tCount=0");
                        lst.Add("\t\tUseCriterionChar=0");
                        lst.Add("\t\tCriterionChar=W");
                        lst.Add("\t\tUseFixStringLenToCurrent=0");
                        lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t\tStyle=0");
                        lst.Add("\t\t\tIncType=0");
                        lst.Add("\t\t\tStartNumber=0");
                        lst.Add("\t\t\tRangeStartNumber=0");
                        lst.Add("\t\t\tRangeEndNumber=0");
                        lst.Add("\t\t\tPreFix=");
                        lst.Add("\t\t\tPostFix=");
                        lst.Add("\t\t\tMaxCharSize=5");
                        lst.Add("\t\t\tSavedCurrentNumber=0");
                        lst.Add("\t\t\tSCHEME_Order=0");
                        lst.Add("\t\t\tSCHEME_Position=0");
                        lst.Add("\t\t\tSCHEME_Type=1");
                        lst.Add("\t\t\tSCHEME_Numbering=0");
                        lst.Add("\t\t\tSaveFileWhenUpdate=0");
                        lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                        lst.Add("\t\t\tNumericSystemType=0");
                        lst.Add("\t\t\tIncreaseLevels=1");
                        lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                        lst.Add("\t\t\t\tSpecial Type=2");
                        lst.Add("\t\t\t\tReference Number 1=2");
                        lst.Add("\t\t\t\tReference Number 2=0");
                        lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                        lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                        lst.Add("\t\t[Attribute]");
                        lst.Add("\t\tObjType=TEXT");
                        lst.Add("\t\tIsSelect=0");
                        lst.Add("\t\tIsUpdate=0");
                        lst.Add("\t\t\tAxisField=120.000000");
                        lst.Add("\t\t\tUNIT=1");
                        if (cl == 0)
                        {
                            lst.Add("\t\t\tDispLeft=0.184");
                            lst.Add("\t\t\tDispRight=3.184");
                        }
                        else
                        {
                            lst.Add("\t\t\tDispLeft=" + (model.StepPitch - 3.184).ToString("f3"));
                            lst.Add("\t\t\tDispRight=" + (model.StepPitch - 0.184).ToString("f3"));
                        }
                        lst.Add("\t\t\tDispTop=70.700");
                        lst.Add("\t\t\tDispBottom=73.100");
                        lst.Add("\t\t\tDispRect=1738,29367,3376,31061");
                        lst.Add("\t\t\tMatrixA=-1.000000");
                        lst.Add("\t\t\tMatrixB=0.000000");
                        lst.Add("\t\t\tMatrixC=0.000000");
                        lst.Add("\t\t\tMatrixD=1.000000");
                        lst.Add("\t\t\tMatrixX=2557.816406");
                        lst.Add("\t\t\tMatrixY=30214.402344");
                        lst.Add("\t\t\tMatrixZ=1.000000");
                        lst.Add("\t\t\tBasicMatrixA=1.000000");
                        lst.Add("\t\t\tBasicMatrixB=0.000000");
                        lst.Add("\t\t\tBasicMatrixC=0.000000");
                        lst.Add("\t\t\tBasicMatrixD=-1.000000");
                        lst.Add("\t\t\tBasicMatrixX=2557.816406");
                        lst.Add("\t\t\tBasicMatrixY=30214.402344");
                        lst.Add("\t\t\tBasicMatrixZ=1.000000");
                        lst.Add("\t\t\tRotateMatrixA=-1.000000");
                        lst.Add("\t\t\tRotateMatrixB=0.000000");
                        lst.Add("\t\t\tRotateMatrixC=0.000000");
                        lst.Add("\t\t\tRotateMatrixD=-1.000000");
                        lst.Add("\t\t\tRotateMatrixX=0.003906");
                        lst.Add("\t\t\tRotateMatrixY=0.003906");
                        lst.Add("\t\t\tRotateMatrixZ=1.000000");
                        if (count == 1) lst.Add("\t\t\tRotateAngle=0.000000");
                        if (count == 2) lst.Add("\t\t\tRotateAngle=90.000000");
                        if (count == 3) lst.Add("\t\t\tRotateAngle=180.000000");
                        if (count == 4) lst.Add("\t\t\tRotateAngle=270.000000");
                        lst.Add("\t\t\tParaNumber=4");
                        lst.Add("\t\t\tLockObject=0");
                        lst.Add("\t\t[END Attribute]");
                        lst.Add('\t' + "[END_ObjectTEXT]");
                        lst.Add(" ");
                    }
                    lst.Add('\t' + "[TEXT OBJECT]");
                    lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                    nObject++;
                    lst.Add("\t\t[TEXT_ATTRIBUTE]");
                    lst.Add("\t\tContent= ");
                    lst.Add("\t\tMaxNum=1000");
                    lst.Add("\t\tAlignChar=1");
                    lst.Add("\t\tAlignMode=0");
                    lst.Add("\t\tCapitalHeight=" + ch.ToString("0.0000"));
                    lst.Add("\t\tCharGap=0.0400");
                    lst.Add("\t\tFontName=C:\\Mime\\Font\\" + file);
                    lst.Add("\t\tHeight=4.0000");
                    lst.Add("\t\tLineGap=1.0017");
                    lst.Add("\t\tWidth=1.0999");
                    lst.Add("\t\tSpaceSize=2.9998");
                    lst.Add("\t\tUseCapitalLineGap=0");
                    lst.Add("\t\tUseFixStringLenght=0");
                    lst.Add("\t\tUseLeftGapofeachChar=0");
                    lst.Add("\t\tUseFixNumberofChar=0");
                    lst.Add("\t\tFixedStrSize=1000");
                    lst.Add("\t\tFixNumberofChar=0");
                    lst.Add("\t\tItalicAngle=0");
                    lst.Add("\t\tNTMark_WidthStype=0");
                    lst.Add("\t\tUseStringCutting=0");
                    lst.Add("\t\tFirst=0");
                    lst.Add("\t\tCount=0");
                    lst.Add("\t\tUseCriterionChar=0");
                    lst.Add("\t\tCriterionChar=W");
                    lst.Add("\t\tUseFixStringLenToCurrent=0");
                    lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t\tStyle=0");
                    lst.Add("\t\t\tIncType=0");
                    lst.Add("\t\t\tStartNumber=0");
                    lst.Add("\t\t\tRangeStartNumber=0");
                    lst.Add("\t\t\tRangeEndNumber=0");
                    lst.Add("\t\t\tPreFix=");
                    lst.Add("\t\t\tPostFix=");
                    lst.Add("\t\t\tMaxCharSize=5");
                    lst.Add("\t\t\tSavedCurrentNumber=0");
                    lst.Add("\t\t\tSCHEME_Order=0");
                    lst.Add("\t\t\tSCHEME_Position=0");
                    lst.Add("\t\t\tSCHEME_Type=1");
                    lst.Add("\t\t\tSCHEME_Numbering=0");
                    lst.Add("\t\t\tSaveFileWhenUpdate=0");
                    lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                    lst.Add("\t\t\tNumericSystemType=0");
                    lst.Add("\t\t\tIncreaseLevels=1");
                    lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\t\t\tSpecial Type=2");
                    if (model.Mode == 0)
                        lst.Add("\t\t\t\tReference Number 1=1");
                    else if (model.Mode == 1)
                        lst.Add("\t\t\t\tReference Number 1=2");
                    else
                        lst.Add("\t\t\t\tReference Number 1=3");
                    lst.Add("\t\t\t\tReference Number 2=0");
                    lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                    lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                    lst.Add("\t\t[Attribute]");
                    lst.Add("\t\tObjType=TEXT");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=120.000000");
                    lst.Add("\t\t\tUNIT=1");
                    if (cl == 0)
                    {
                        lst.Add("\t\t\tDispLeft=0.184");
                        lst.Add("\t\t\tDispRight=3.184");
                    }
                    else
                    {
                        lst.Add("\t\t\tDispLeft=" + (model.StepPitch - 3.184).ToString("f3"));
                        lst.Add("\t\t\tDispRight=" + (model.StepPitch - 0.184).ToString("f3"));
                    }
                    if (model.Mode == 0)
                    {
                        lst.Add("\t\t\tDispTop=50.700");
                        lst.Add("\t\t\tDispBottom=53.100");
                    }
                    else if (model.Mode == 1)
                    {
                        lst.Add("\t\t\tDispTop=70.700");
                        lst.Add("\t\t\tDispBottom=73.100");
                    }
                    else
                    {
                        lst.Add("\t\t\tDispTop=50.700");
                        lst.Add("\t\t\tDispBottom=53.100");
                    }
                    lst.Add("\t\t\tDispRect=1738,29367,3376,31061");
                    lst.Add("\t\t\tMatrixA=-1.000000");
                    lst.Add("\t\t\tMatrixB=0.000000");
                    lst.Add("\t\t\tMatrixC=0.000000");
                    lst.Add("\t\t\tMatrixD=1.000000");
                    lst.Add("\t\t\tMatrixX=2557.816406");
                    lst.Add("\t\t\tMatrixY=30214.402344");
                    lst.Add("\t\t\tMatrixZ=1.000000");
                    lst.Add("\t\t\tBasicMatrixA=1.000000");
                    lst.Add("\t\t\tBasicMatrixB=0.000000");
                    lst.Add("\t\t\tBasicMatrixC=0.000000");
                    lst.Add("\t\t\tBasicMatrixD=-1.000000");
                    lst.Add("\t\t\tBasicMatrixX=2557.816406");
                    lst.Add("\t\t\tBasicMatrixY=30214.402344");
                    lst.Add("\t\t\tBasicMatrixZ=1.000000");
                    lst.Add("\t\t\tRotateMatrixA=-1.000000");
                    lst.Add("\t\t\tRotateMatrixB=0.000000");
                    lst.Add("\t\t\tRotateMatrixC=0.000000");
                    lst.Add("\t\t\tRotateMatrixD=-1.000000");
                    lst.Add("\t\t\tRotateMatrixX=0.003906");
                    lst.Add("\t\t\tRotateMatrixY=0.003906");
                    lst.Add("\t\t\tRotateMatrixZ=1.000000");
                    if (count == 1) lst.Add("\t\t\tRotateAngle=0.000000");
                    if (count == 2) lst.Add("\t\t\tRotateAngle=90.000000");
                    if (count == 3) lst.Add("\t\t\tRotateAngle=180.000000");
                    if (count == 4) lst.Add("\t\t\tRotateAngle=270.000000");
                    lst.Add("\t\t\tParaNumber=4");
                    lst.Add("\t\t\tLockObject=0");
                    lst.Add("\t\t[END Attribute]");
                    lst.Add('\t' + "[END_ObjectTEXT]");
                    lst.Add(" ");
                }

                if (week > 0)
                {
                    lst.Add('\t' + "[TEXT OBJECT]");
                    lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                    lst.Add("\t\t[TEXT_ATTRIBUTE]");
                    lst.Add("\t\tContent= ");
                    lst.Add("\t\tMaxNum=1000");
                    lst.Add("\t\tAlignChar=1");
                    lst.Add("\t\tAlignMode=0");
                    lst.Add("\t\tCapitalHeight=" + ch.ToString("0.0000"));
                    lst.Add("\t\tCharGap=0.0400");
                    lst.Add("\t\tFontName=C:\\Mime\\Font\\" + file);
                    lst.Add("\t\tHeight=4.0000");
                    lst.Add("\t\tLineGap=1.0017");
                    lst.Add("\t\tWidth=1.0999");
                    lst.Add("\t\tSpaceSize=2.9998");
                    lst.Add("\t\tUseCapitalLineGap=0");
                    lst.Add("\t\tUseFixStringLenght=0");
                    lst.Add("\t\tUseLeftGapofeachChar=0");
                    lst.Add("\t\tUseFixNumberofChar=0");
                    lst.Add("\t\tFixedStrSize=1000");
                    lst.Add("\t\tFixNumberofChar=0");
                    lst.Add("\t\tItalicAngle=0");
                    lst.Add("\t\tNTMark_WidthStype=0");
                    lst.Add("\t\tUseStringCutting=0");
                    lst.Add("\t\tFirst=0");
                    lst.Add("\t\tCount=0");
                    lst.Add("\t\tUseCriterionChar=0");
                    lst.Add("\t\tCriterionChar=W");
                    lst.Add("\t\tUseFixStringLenToCurrent=0");
                    lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t\tStyle=0");
                    lst.Add("\t\t\tIncType=0");
                    lst.Add("\t\t\tStartNumber=0");
                    lst.Add("\t\t\tRangeStartNumber=0");
                    lst.Add("\t\t\tRangeEndNumber=0");
                    lst.Add("\t\t\tPreFix=");
                    lst.Add("\t\t\tPostFix=");
                    lst.Add("\t\t\tMaxCharSize=5");
                    lst.Add("\t\t\tSavedCurrentNumber=0");
                    lst.Add("\t\t\tSCHEME_Order=0");
                    lst.Add("\t\t\tSCHEME_Position=0");
                    lst.Add("\t\t\tSCHEME_Type=1");
                    lst.Add("\t\t\tSCHEME_Numbering=0");
                    lst.Add("\t\t\tSaveFileWhenUpdate=0");
                    lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                    lst.Add("\t\t\tNumericSystemType=0");
                    lst.Add("\t\t\tIncreaseLevels=1");
                    lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\t\t\tSpecial Type=2");
                    lst.Add("\t\t\t\tReference Number 1=20");
                    lst.Add("\t\t\t\tReference Number 2=0");
                    lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                    lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                    lst.Add("\t\t[Attribute]");
                    lst.Add("\t\tObjType=TEXT");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=120.000000");
                    lst.Add("\t\t\tUNIT=1");
                    if (wl == 1)
                    {
                        lst.Add("\t\t\tDispLeft=" + (model.StepPitch - 3.184).ToString("f3"));
                        lst.Add("\t\t\tDispRight=" + (model.StepPitch - 1.184).ToString("f3"));
                        lst.Add("\t\t\tDispTop=50.700");
                        lst.Add("\t\t\tDispBottom=53.100");
                    }
                    else if (wl == 2)
                    {
                        lst.Add("\t\t\tDispLeft=" + (model.StepPitch / 2 - 3.184).ToString("f3"));
                        lst.Add("\t\t\tDispRight=" + (model.StepPitch / 2 - 1.184).ToString("f3"));
                        lst.Add("\t\t\tDispTop=20.133");
                        lst.Add("\t\t\tDispBottom=22.134");
                    }
                    else
                    {
                        lst.Add("\t\t\tDispLeft=1.184");
                        lst.Add("\t\t\tDispRight=3.184");
                        lst.Add("\t\t\tDispTop=50.700");
                        lst.Add("\t\t\tDispBottom=53.100");
                    }
                    lst.Add("\t\t\tDispRect=1738,29367,3376,31061");
                    lst.Add("\t\t\tMatrixA=-1.000000");
                    lst.Add("\t\t\tMatrixB=0.000000");
                    lst.Add("\t\t\tMatrixC=0.000000");
                    lst.Add("\t\t\tMatrixD=1.000000");
                    lst.Add("\t\t\tMatrixX=2557.816406");
                    lst.Add("\t\t\tMatrixY=30214.402344");
                    lst.Add("\t\t\tMatrixZ=1.000000");
                    lst.Add("\t\t\tBasicMatrixA=1.000000");
                    lst.Add("\t\t\tBasicMatrixB=0.000000");
                    lst.Add("\t\t\tBasicMatrixC=0.000000");
                    lst.Add("\t\t\tBasicMatrixD=-1.000000");
                    lst.Add("\t\t\tBasicMatrixX=2557.816406");
                    lst.Add("\t\t\tBasicMatrixY=30214.402344");
                    lst.Add("\t\t\tBasicMatrixZ=1.000000");
                    lst.Add("\t\t\tRotateMatrixA=-1.000000");
                    lst.Add("\t\t\tRotateMatrixB=0.000000");
                    lst.Add("\t\t\tRotateMatrixC=0.000000");
                    lst.Add("\t\t\tRotateMatrixD=-1.000000");
                    lst.Add("\t\t\tRotateMatrixX=0.003906");
                    lst.Add("\t\t\tRotateMatrixY=0.003906");
                    lst.Add("\t\t\tRotateMatrixZ=1.000000");
                    if (wl == 2)
                    {
                        if (week == 1) lst.Add("\t\t\tRotateAngle=180.000000");
                        if (week == 2) lst.Add("\t\t\tRotateAngle=0.000000");
                    }
                    else
                    {
                        if (week == 1) lst.Add("\t\t\tRotateAngle=270.000000");
                        if (week == 2) lst.Add("\t\t\tRotateAngle=90.000000");
                    }
                    lst.Add("\t\t\tParaNumber=4");
                    lst.Add("\t\t\tLockObject=0");
                    lst.Add("\t\t[END Attribute]");
                    lst.Add('\t' + "[END_ObjectTEXT]");
                    lst.Add(" ");
                }


                lst.Add('\t' + "[GUIDE LINE]");
                for (int i = 0; i < 8; i++)
                {
                    lst.Add('\t' + "ItemNum=" + i.ToString());
                    lst.Add('\t' + "ItemUse=0");
                    lst.Add('\t' + "ItemAlign=0");
                    lst.Add('\t' + "ItemPosX=0.000");
                    lst.Add('\t' + "ItemPosY=0.000");
                    lst.Add('\t' + "ItemWidth=0.000");
                    lst.Add('\t' + "ItemHeight=0.000");
                }
                lst.Add('\t' + "[END GUIDE LINE]");
                lst.Add('\t' + "[AUTO ALIGNMENT INFO]");
                lst.Add('\t' + "Use Auto Horizontal Align=0");
                lst.Add('\t' + "Use Auto Vertical Align=0");
                lst.Add('\t' + "[END AUTO ALIGNMENT INFO]");
                lst.Add("[END SemData]");
                File.WriteAllLines(Path, lst);
            }
            catch { return false; }
            return true;
        }

        public bool CopyAndPasteStrip(string path)
        {
            //Base Info 외 모두 카피하자.
            return true;
        }

        public bool CopyAndPasteSem()
        {

            return true;
        }
        #endregion


        #region CopyTo Model MarkFile()
        public static bool CopyTo(string SrcModel, string DstModel, string IP, ModelMarkInfo model, MarkLogo logo)
        {
            bool railirr = model.RailIrr;
            if (railirr)
            {
                for (int n = 0; n < model.Step; n++)
                {
                    string SrcPath = "\\\\" + IP + "\\Mime\\mrk\\" + SrcModel + "_" + (n + 1).ToString() + "STEP.mrk";
                    string DstPath = "\\\\" + IP + "\\Mime\\mrk\\" + DstModel + "_" + (n + 1).ToString() + "STEP.mrk";
                    if (!File.Exists(SrcPath)) return false;
                    if (File.Exists(DstPath))
                    {
                        try
                        {
                            File.Delete(DstPath);
                        }
                        catch
                        { return false; }
                    }
                    try
                    {
                        FileStream fs = File.Create(DstPath); fs.Close();
                    }
                    catch { return false; }
                    if (File.Exists(SrcPath))
                    {
                        string[] strs = File.ReadAllLines(SrcPath);
                        for (int i = 0; i < strs.Length; i++)
                        {
                            string[] slt = strs[i].Split('=');
                            if (slt.Length > 1)
                            {
                                if (slt[0] == "TemplateName")
                                {
                                    strs[i] = strs[i].Replace(SrcModel, DstModel);
                                }
                                if (slt[0] == "Parameter1" || slt[0] == "Parameter2" || slt[0] == "Parameter3" || slt[0] == "Parameter4" || slt[0] == "Parameter5" || slt[0] == "Parameter6" || slt[0] == "Parameter7")
                                {
                                    strs[i] = strs[i].Replace(SrcModel, DstModel);
                                }
                            }
                        }
                        File.WriteAllLines(DstPath, strs);
                    }
                    try
                    {
                        if (n == 0)
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                string path1 = "\\\\" + IP + "\\Mime\\Pen\\" + SrcModel + "_" + (i + 1).ToString() + ".prm";
                                string path2 = "\\\\" + IP + "\\Mime\\Pen\\" + DstModel + "_" + (i + 1).ToString() + ".prm";

                                if (File.Exists(path1))
                                {
                                    File.Copy(path1, path2, true);
                                }
                            }
                        }
                    }
                    catch { return false; }
                    CopyToTemplate(SrcModel, DstModel, IP, model, logo);
                }
            }
            else
            {
                string SrcPath = "\\\\" + IP + "\\Mime\\mrk\\" + SrcModel + ".mrk";
                string DstPath = "\\\\" + IP + "\\Mime\\mrk\\" + DstModel + ".mrk";
                if (!File.Exists(SrcPath)) return false;
                if (File.Exists(DstPath))
                {
                    try
                    {
                        File.Delete(DstPath);
                    }
                    catch
                    { return false; }
                }
                try
                {
                    FileStream fs = File.Create(DstPath); fs.Close();
                }
                catch { return false; }
                if (File.Exists(SrcPath))
                {
                    string[] strs = File.ReadAllLines(SrcPath);
                    for (int i = 0; i < strs.Length; i++)
                    {
                        string[] slt = strs[i].Split('=');
                        if (slt.Length > 1)
                        {
                            if (slt[0] == "TemplateName")
                            {
                                strs[i] = strs[i].Replace(SrcModel, DstModel);
                            }
                            if (slt[0] == "Parameter1" || slt[0] == "Parameter2" || slt[0] == "Parameter3" || slt[0] == "Parameter4" || slt[0] == "Parameter5" || slt[0] == "Parameter6" || slt[0] == "Parameter7")
                            {
                                strs[i] = strs[i].Replace(SrcModel, DstModel);
                            }
                        }
                    }
                    File.WriteAllLines(DstPath, strs);
                }
                try
                {
                    for (int i = 0; i < 7; i++)
                    {
                        string path1 = "\\\\" + IP + "\\Mime\\Pen\\" + SrcModel + "_" + (i + 1).ToString() + ".prm";
                        string path2 = "\\\\" + IP + "\\Mime\\Pen\\" + DstModel + "_" + (i + 1).ToString() + ".prm";
                        if (File.Exists(path1))
                        {
                            File.Copy(path1, path2, true);
                        }
                    }
                }
                catch { return false; }
                CopyToTemplate(SrcModel, DstModel, IP, model, logo);
            }
            return true;
        }
        public static void CopyToTemplate(string SrcModel, string DstModel, string IP, ModelMarkInfo model, MarkLogo logo)
        {
            bool railirr = model.RailIrr;
            if (railirr)
            {
                string SrcPath = "\\\\" + IP + "\\Mime\\tpl\\" + SrcModel + "_Unit.tpl";
                string DstPath = "\\\\" + IP + "\\Mime\\tpl\\" + DstModel + "_Unit.tpl";
                if (File.Exists(SrcPath))
                {
                    if (File.Exists(DstPath))
                    {
                        try
                        {
                            File.Delete(DstPath);
                        }
                        catch
                        { }
                    }
                    try
                    {
                        FileStream fs = File.Create(DstPath); fs.Close();
                    }
                    catch { }
                    string[] strs = File.ReadAllLines(SrcPath);
                    for (int i = 0; i < strs.Length; i++)
                    {
                        string[] slt = strs[i].Split('=');
                        if (slt.Length > 1)
                        {
                            if (slt[0] == "StripFile" || slt[0] == "SemFile")
                            {
                                strs[i] = strs[i].Replace(SrcModel, DstModel);
                            }
                        }
                    }
                    File.WriteAllLines(DstPath, strs);

                }

                SrcPath = "\\\\" + IP + "\\Mime\\tpl\\" + SrcModel + "_Reject.tpl";
                DstPath = "\\\\" + IP + "\\Mime\\tpl\\" + DstModel + "_Reject.tpl";
                if (File.Exists(SrcPath))
                {
                    if (File.Exists(DstPath))
                    {
                        try
                        {
                            File.Delete(DstPath);
                        }
                        catch
                        { }
                    }
                    try
                    {
                        FileStream fs = File.Create(DstPath); fs.Close();
                    }
                    catch { }
                    string[] strs = File.ReadAllLines(SrcPath);
                    for (int i = 0; i < strs.Length; i++)
                    {
                        string[] slt = strs[i].Split('=');
                        if (slt.Length > 1)
                        {
                            if (slt[0] == "StripFile" || slt[0] == "SemFile")
                            {
                                strs[i] = strs[i].Replace(SrcModel, DstModel);
                            }
                        }
                    }
                    File.WriteAllLines(DstPath, strs);
                }
                for (int n = 0; n < model.Step; n++)
                {
                    SrcPath = "\\\\" + IP + "\\Mime\\tpl\\" + SrcModel + "_" + (n + 1).ToString() + "STEP_Rail.tpl";
                    DstPath = "\\\\" + IP + "\\Mime\\tpl\\" + DstModel + "_" + (n + 1).ToString() + "STEP_Rail.tpl";
                    if (File.Exists(SrcPath))
                    {
                        if (File.Exists(DstPath))
                        {
                            try
                            {
                                File.Delete(DstPath);
                            }
                            catch
                            { }
                        }
                        try
                        {
                            FileStream fs = File.Create(DstPath); fs.Close();
                        }
                        catch { }
                        string[] strs = File.ReadAllLines(SrcPath);
                        for (int i = 0; i < strs.Length; i++)
                        {
                            string[] slt = strs[i].Split('=');
                            if (slt.Length > 1)
                            {
                                if (slt[0] == "StripFile" || slt[0] == "SemFile")
                                {
                                    strs[i] = strs[i].Replace(SrcModel, DstModel);
                                }
                            }
                        }
                        File.WriteAllLines(DstPath, strs);
                    }

                }
                CopyToStrip(SrcModel, DstModel, IP, model, true);
                CopyToSem(SrcModel, DstModel, IP, model, logo);
            }
            else
            {
                string SrcPath = "\\\\" + IP + "\\Mime\\tpl\\" + SrcModel + "_Unit.tpl";
                string DstPath = "\\\\" + IP + "\\Mime\\tpl\\" + DstModel + "_Unit.tpl";
                if (File.Exists(SrcPath))
                {
                    if (File.Exists(DstPath))
                    {
                        try
                        {
                            File.Delete(DstPath);
                        }
                        catch
                        { }
                    }
                    try
                    {
                        FileStream fs = File.Create(DstPath); fs.Close();
                    }
                    catch { }
                    string[] strs = File.ReadAllLines(SrcPath);
                    for (int i = 0; i < strs.Length; i++)
                    {
                        string[] slt = strs[i].Split('=');
                        if (slt.Length > 1)
                        {
                            if (slt[0] == "StripFile" || slt[0] == "SemFile")
                            {
                                strs[i] = strs[i].Replace(SrcModel, DstModel);
                            }
                        }
                    }
                    File.WriteAllLines(DstPath, strs);

                }

                SrcPath = "\\\\" + IP + "\\Mime\\tpl\\" + SrcModel + "_Reject.tpl";
                DstPath = "\\\\" + IP + "\\Mime\\tpl\\" + DstModel + "_Reject.tpl";
                if (File.Exists(SrcPath))
                {
                    if (File.Exists(DstPath))
                    {
                        try
                        {
                            File.Delete(DstPath);
                        }
                        catch
                        { }
                    }
                    try
                    {
                        FileStream fs = File.Create(DstPath); fs.Close();
                    }
                    catch { }
                    string[] strs = File.ReadAllLines(SrcPath);
                    for (int i = 0; i < strs.Length; i++)
                    {
                        string[] slt = strs[i].Split('=');
                        if (slt.Length > 1)
                        {
                            if (slt[0] == "StripFile" || slt[0] == "SemFile")
                            {
                                strs[i] = strs[i].Replace(SrcModel, DstModel);
                            }
                        }
                    }
                    File.WriteAllLines(DstPath, strs);
                }
                SrcPath = "\\\\" + IP + "\\Mime\\tpl\\" + SrcModel + "_Rail.tpl";
                DstPath = "\\\\" + IP + "\\Mime\\tpl\\" + DstModel + "_Rail.tpl";
                if (File.Exists(SrcPath))
                {
                    if (File.Exists(DstPath))
                    {
                        try
                        {
                            File.Delete(DstPath);
                        }
                        catch
                        { }
                    }
                    try
                    {
                        FileStream fs = File.Create(DstPath); fs.Close();
                    }
                    catch { }
                    string[] strs = File.ReadAllLines(SrcPath);
                    for (int i = 0; i < strs.Length; i++)
                    {
                        string[] slt = strs[i].Split('=');
                        if (slt.Length > 1)
                        {
                            if (slt[0] == "StripFile" || slt[0] == "SemFile")
                            {
                                strs[i] = strs[i].Replace(SrcModel, DstModel);
                            }
                        }
                    }
                    File.WriteAllLines(DstPath, strs);
                }
                CopyToStrip(SrcModel, DstModel, IP, model, false);
                CopyToSem(SrcModel, DstModel, IP, model, logo);
            }
        }

        public static void CopyToStrip(string SrcModel, string DstModel, string IP, ModelMarkInfo model, bool railirr)
        {
            string SrcPath = "\\\\" + IP + "\\Mime\\stp\\" + SrcModel + "_Unit.stp";
            string DstPath = "\\\\" + IP + "\\Mime\\stp\\" + DstModel + "_Unit.stp";
            if (File.Exists(SrcPath))
            {
                if (File.Exists(DstPath))
                {
                    try
                    {
                        File.Delete(DstPath);
                    }
                    catch
                    { }
                }
                try
                {
                    FileStream fs = File.Create(DstPath); fs.Close();
                }
                catch { }
                string[] strs = File.ReadAllLines(SrcPath);
                for (int i = 0; i < strs.Length; i++)
                {
                    string[] slt = strs[i].Split('=');
                    if (slt.Length > 1)
                    {
                        if (slt[0] == "m_iStripX")
                            strs[i] = "m_iStripX=" + model.StepUnits.ToString();
                        if (slt[0] == "m_iStripY")
                            strs[i] = "m_iStripY=" + model.UnitRow.ToString();
                        if (slt[0] == "m_dXPitch")
                            strs[i] = "m_dXPitch=" + model.UnitWidth.ToString("f6");
                        if (slt[0] == "m_dYPitch")
                            strs[i] = "m_dYPitch=" + model.UnitHeight.ToString("f6");
                        if (slt[0] == "m_dUnitWidth")
                            strs[i] = "m_dUnitWidth=" + model.UnitWidth.ToString("f6");
                        if (slt[0] == "m_dUnitHeight")
                            strs[i] = "m_dUnitHeight=" + model.UnitHeight.ToString("f6");
                        if (slt[0] == "UnitAttribute")
                        {
                            strs[i] = "UnitAttribute=";
                            for (int k = 0; k < model.StepUnits * model.UnitRow; k++)
                            {
                                strs[i] += "1";
                            }
                        }
                        if (slt[0] == "SEMType")
                        {
                            strs[i] = "SEMType=";
                            for (int k = 0; k < model.StepUnits * model.UnitRow; k++)
                            {
                                strs[i] += "0";
                            }
                        }
                    }
                }
                List<string> str = strs.OfType<string>().ToList();
                int sp = 0;
                int ep = 0;
                for (int i = 0; i < str.Count; i++)
                {
                    if (str[i] == "OneChipOffset=")
                    {
                        sp = i + 1;
                    }
                    if (str[i] == "[END One Chip Offset]")
                    {
                        ep = i - 1;
                    }
                }
                str.RemoveRange(sp, (ep - sp));
                for (int k = 0; k < model.StepUnits * model.UnitRow; k++)
                {
                    str.Insert(sp, "0.000000, 0.000000, 0.000000");
                }
                File.WriteAllLines(DstPath, str);
            }

            SrcPath = "\\\\" + IP + "\\Mime\\stp\\" + SrcModel + "_Reject.stp";
            DstPath = "\\\\" + IP + "\\Mime\\stp\\" + DstModel + "_Reject.stp";
            if (File.Exists(SrcPath))
            {
                if (File.Exists(DstPath))
                {
                    try
                    {
                        File.Delete(DstPath);
                    }
                    catch
                    { }
                }
                try
                {
                    FileStream fs = File.Create(DstPath); fs.Close();
                }
                catch { }
                string[] strs = File.ReadAllLines(SrcPath);
                //for (int i = 0; i < strs.Length; i++)
                //{
                //    string[] slt = strs[i].Split('=');
                //    if (slt.Length > 1)
                //    {
                //        if (slt[0] == "UnitAttribute")
                //        {
                //            strs[i] = "UnitAttribute=";
                //            for (int k = 0; k < model.Mark.StepUnits * model.Strip.UnitColumn; k++)
                //            {
                //                strs[i] += "1";
                //            }
                //        }
                //        if (slt[0] == "SEMType")
                //        {
                //            strs[i] = "SEMType=";
                //            for (int k = 0; k < model.Mark.StepUnits * model.Strip.UnitColumn; k++)
                //            {
                //                strs[i] += "0";
                //            }
                //        }
                //    }
                //}
                File.WriteAllLines(DstPath, strs);
            }
            if (railirr)
            {
                for (int n = 0; n < model.Step; n++)
                {
                    SrcPath = "\\\\" + IP + "\\Mime\\stp\\" + SrcModel + "_" + (n + 1).ToString() + "STEP_Rail.stp";
                    DstPath = "\\\\" + IP + "\\Mime\\stp\\" + DstModel + "_" + (n + 1).ToString() + "STEP_Rail.stp";
                    if (File.Exists(SrcPath))
                    {
                        if (File.Exists(DstPath))
                        {
                            try
                            {
                                File.Delete(DstPath);
                            }
                            catch
                            { }
                        }
                        try
                        {
                            FileStream fs = File.Create(DstPath); fs.Close();
                        }
                        catch { }
                        string[] strs = File.ReadAllLines(SrcPath);
                        for (int i = 0; i < strs.Length; i++)
                        {
                            string[] slt = strs[i].Split('=');
                            if (slt.Length > 1)
                            {
                                if (slt[0] == "m_iStripX")
                                    strs[i] = "m_iStripX=" + (model.StepUnits * model.UnitRow).ToString();
                                if (slt[0] == "m_dXPitch")
                                    strs[i] = "m_dXPitch=0.000000";
                                if (slt[0] == "m_dYPitch")
                                    strs[i] = "m_dYPitch=0.000000";
                                if (slt[0] == "m_dUnitWidth")
                                    strs[i] = "m_dUnitWidth=0.100000";
                                if (slt[0] == "m_dUnitHeight")
                                    strs[i] = "m_dUnitHeight=0.100000";
                                if (slt[0] == "ms_dSpecialPitch[0]")
                                    strs[i] = "ms_dSpecialPitch[0]=" + model.UnitWidth.ToString("f6");
                                if (slt[0] == "ms_iSpecialPitchApply[0]")
                                    strs[i] = "ms_iSpecialPitchApply[0]=" + model.UnitRow.ToString();
                                if (slt[0] == "UnitAttribute")
                                {
                                    strs[i] = "UnitAttribute=";
                                    for (int k = 0; k < model.StepUnits * model.UnitRow; k++)
                                    {
                                        strs[i] += "1";
                                    }
                                }
                                if (slt[0] == "SEMType")
                                {
                                    strs[i] = "SEMType=";
                                    for (int k = 0; k < model.StepUnits * model.UnitRow; k++)
                                    {
                                        strs[i] += "0";
                                    }
                                }
                            }
                        }
                        List<string> str = strs.OfType<string>().ToList();
                        int sp = 0;
                        int ep = 0;
                        for (int i = 0; i < str.Count; i++)
                        {
                            if (str[i] == "OneChipOffset=")
                            {
                                sp = i + 1;
                            }
                            if (str[i] == "[END One Chip Offset]")
                            {
                                ep = i;
                            }
                        }
                        str.RemoveRange(sp, (ep - sp));
                        for (int k = 0; k < model.StepUnits; k++)
                        {
                            for (int i = model.UnitRow - 1; i >= 0; i--)
                            {
                                double dd = i * 0.5;
                                str.Insert(sp, dd.ToString("f6") + ", 0.000000, 0.000000");
                            }
                        }
                        File.WriteAllLines(DstPath, str);
                    }
                }
            }
            else
            {
                SrcPath = "\\\\" + IP + "\\Mime\\stp\\" + SrcModel + "_Rail.stp";
                DstPath = "\\\\" + IP + "\\Mime\\stp\\" + DstModel + "_Rail.stp";
                if (File.Exists(SrcPath))
                {
                    if (File.Exists(DstPath))
                    {
                        try
                        {
                            File.Delete(DstPath);
                        }
                        catch
                        { }
                    }
                    try
                    {
                        FileStream fs = File.Create(DstPath); fs.Close();
                    }
                    catch { }
                    string[] strs = File.ReadAllLines(SrcPath);
                    for (int i = 0; i < strs.Length; i++)
                    {
                        string[] slt = strs[i].Split('=');
                        if (slt.Length > 1)
                        {
                            if (slt[0] == "m_iStripX")
                                strs[i] = "m_iStripX=" + (model.StepUnits * model.UnitRow).ToString();
                            if (slt[0] == "m_dXPitch")
                                strs[i] = "m_dXPitch=0.000000";
                            if (slt[0] == "m_dYPitch")
                                strs[i] = "m_dYPitch=0.000000";
                            if (slt[0] == "m_dUnitWidth")
                                strs[i] = "m_dUnitWidth=0.100000";
                            if (slt[0] == "m_dUnitHeight")
                                strs[i] = "m_dUnitHeight=0.100000";
                            if (slt[0] == "ms_dSpecialPitch[0]")
                                strs[i] = "ms_dSpecialPitch[0]=" + model.UnitWidth.ToString("f6");
                            if (slt[0] == "ms_iSpecialPitchApply[0]")
                                strs[i] = "ms_iSpecialPitchApply[0]=" + model.UnitRow.ToString();
                            if (slt[0] == "UnitAttribute")
                            {
                                strs[i] = "UnitAttribute=";
                                for (int k = 0; k < model.StepUnits * model.UnitRow; k++)
                                {
                                    strs[i] += "1";
                                }
                            }
                            if (slt[0] == "SEMType")
                            {
                                strs[i] = "SEMType=";
                                for (int k = 0; k < model.StepUnits * model.UnitRow; k++)
                                {
                                    strs[i] += "0";
                                }
                            }
                        }
                    }
                    List<string> str = strs.OfType<string>().ToList();
                    int sp = 0;
                    int ep = 0;
                    for (int i = 0; i < str.Count; i++)
                    {
                        if (str[i] == "OneChipOffset=")
                        {
                            sp = i + 1;
                        }
                        if (str[i] == "[END One Chip Offset]")
                        {
                            ep = i;
                        }
                    }
                    str.RemoveRange(sp, (ep - sp));
                    for (int k = 0; k < model.StepUnits; k++)
                    {
                        for (int i = model.UnitRow - 1; i >= 0; i--)
                        {
                            double dd = i * 0.5;
                            str.Insert(sp, dd.ToString("f6") + ", 0.000000, 0.000000");
                        }
                    }
                    File.WriteAllLines(DstPath, str);
                }
            }
        }

        public static void CopyToSem(string SrcModel, string DstModel, string IP, ModelMarkInfo model, MarkLogo logo)
        {
            string SrcPath = "\\\\" + IP + "\\Mime\\sem\\" + SrcModel + "_Unit.sem";
            string DstPath = "\\\\" + IP + "\\Mime\\sem\\" + DstModel + "_Unit.sem";
            if (File.Exists(SrcPath))
            {
                if (File.Exists(DstPath))
                {
                    try
                    {
                        File.Delete(DstPath);
                    }
                    catch
                    { }
                }
                try
                {
                    FileStream fs = File.Create(DstPath); fs.Close();
                }
                catch { }
                string[] strs = File.ReadAllLines(SrcPath);
                for (int i = 0; i < strs.Length; i++)
                {
                    string[] slt = strs[i].Split('=');
                    if (slt.Length > 1)
                    {
                        if (slt[0] == "ChipXSize")
                            strs[i] = "ChipXSize=" + model.UnitWidth.ToString("f3");
                        if (slt[0] == "ChipYSize")
                            strs[i] = "ChipYSize=" + model.UnitHeight.ToString("f3");
                    }
                }
                File.WriteAllLines(DstPath, strs);

            }

            SrcPath = "\\\\" + IP + "\\Mime\\sem\\" + SrcModel + "_Reject.sem";
            DstPath = "\\\\" + IP + "\\Mime\\sem\\" + DstModel + "_Reject.sem";
            if (File.Exists(SrcPath))
            {
                if (File.Exists(DstPath))
                {
                    try
                    {
                        File.Delete(DstPath);
                    }
                    catch
                    { }
                }
                try
                {
                    FileStream fs = File.Create(DstPath); fs.Close();
                }
                catch { }
                List<string> strs = UpdateRejSem(model, logo);
                File.WriteAllLines(DstPath, strs);
            }
            SrcPath = "\\\\" + IP + "\\Mime\\sem\\" + SrcModel + "_Rail.sem";
            DstPath = "\\\\" + IP + "\\Mime\\sem\\" + DstModel + "_Rail.sem";
            if (File.Exists(SrcPath))
            {
                if (File.Exists(DstPath))
                {
                    try
                    {
                        File.Delete(DstPath);
                    }
                    catch
                    { }
                }
                try
                {
                    FileStream fs = File.Create(DstPath); fs.Close();
                }
                catch { }
                string[] strs = File.ReadAllLines(SrcPath);
                for (int i = 0; i < strs.Length; i++)
                {
                    string[] slt = strs[i].Split('=');
                    if (slt.Length > 1)
                    {
                        if (slt[0] == "ChipXSize")
                            strs[i] = "ChipXSize=0.100";
                        if (slt[0] == "ChipYSize")
                            strs[i] = "ChipYSize=0.100";
                    }
                }
                File.WriteAllLines(DstPath, strs);
            }
        }

        private static List<string> UpdateRejSem(ModelMarkInfo model, MarkLogo logo)
        {
            List<string> lst = new List<string>();
            try
            {
                lst.Add("// MiME SemData File Ver0.5");
                lst.Add("");
                lst.Add("Unit=1");
                lst.Add("ChipXSize=120.000");
                lst.Add("ChipYSize=120.000");
                lst.Add("SemRotateStatus=0");
                //////Obects

                string file = logo.FontFile[0];
                string[] st = file.Split(' ', 'X');
                int week = model.Week;
                int count = model.Count;
                int wl = model.WeekLoc;
                int cl = model.CountLoc;
                string rfile = logo.FontRemark[model.CountRW - 1];

                int nObject = 0;
                if (count > 0)
                {
                    if (model.Mode == 1)
                    {
                        lst.Add('\t' + "[TEXT OBJECT]");
                        lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                        nObject++;
                        lst.Add("\t\t[TEXT_ATTRIBUTE]");
                        lst.Add("\t\tContent=");
                        lst.Add("\t\tMaxNum=1000");
                        lst.Add("\t\tAlignChar=1");
                        lst.Add("\t\tAlignMode=0");
                        lst.Add("\t\tCapitalHeight=3.1585");
                        lst.Add("\t\tCharGap=0.0400");
                        lst.Add("\t\tFontName=C:\\Mime\\Logo\\" + rfile);
                        lst.Add("\t\tHeight=4.0076");
                        lst.Add("\t\tLineGap=1.0017");
                        lst.Add("\t\tWidth=1.0999");
                        lst.Add("\t\tSpaceSize=2.9998");
                        lst.Add("\t\tUseCapitalLineGap=0");
                        lst.Add("\t\tUseFixStringLenght=0");
                        lst.Add("\t\tUseLeftGapofeachChar=0");
                        lst.Add("\t\tUseFixNumberofChar=0");
                        lst.Add("\t\tFixedStrSize=1000");
                        lst.Add("\t\tFixNumberofChar=0");
                        lst.Add("\t\tItalicAngle=0");
                        lst.Add("\t\tNTMark_WidthStype=0");
                        lst.Add("\t\tUseStringCutting=0");
                        lst.Add("\t\tFirst=0");
                        lst.Add("\t\tCount=0");
                        lst.Add("\t\tUseCriterionChar=0");
                        lst.Add("\t\tCriterionChar=W");
                        lst.Add("\t\tUseFixStringLenToCurrent=0");
                        lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t\tStyle=0");
                        lst.Add("\t\t\tIncType=0");
                        lst.Add("\t\t\tStartNumber=0");
                        lst.Add("\t\t\tRangeStartNumber=0");
                        lst.Add("\t\t\tRangeEndNumber=0");
                        lst.Add("\t\t\tPreFix=");
                        lst.Add("\t\t\tPostFix=");
                        lst.Add("\t\t\tMaxCharSize=5");
                        lst.Add("\t\t\tSavedCurrentNumber=0");
                        lst.Add("\t\t\tSCHEME_Order=0");
                        lst.Add("\t\t\tSCHEME_Position=0");
                        lst.Add("\t\t\tSCHEME_Type=1");
                        lst.Add("\t\t\tSCHEME_Numbering=0");
                        lst.Add("\t\t\tSaveFileWhenUpdate=0");
                        lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                        lst.Add("\t\t\tNumericSystemType=0");
                        lst.Add("\t\t\tIncreaseLevels=1");
                        lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                        lst.Add("\t\t\t\tSpecial Type=2");
                        lst.Add("\t\t\t\tReference Number 1=1");
                        lst.Add("\t\t\t\tReference Number 2=0");
                        lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                        lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                        lst.Add("\t\t[Attribute]");
                        lst.Add("\t\tObjType=TEXT");
                        lst.Add("\t\tIsSelect=0");
                        lst.Add("\t\tIsUpdate=0");
                        lst.Add("\t\t\tAxisField=120.000000");
                        lst.Add("\t\t\tUNIT=1");
                        if (cl == 0)
                        {
                            lst.Add("\t\t\tDispLeft=0.184");
                            lst.Add("\t\t\tDispRight=3.184");
                        }
                        else
                        {
                            lst.Add("\t\t\tDispLeft=" + (model.StepPitch - 3.184).ToString("f3"));
                            lst.Add("\t\t\tDispRight=" + (model.StepPitch - 0.184).ToString("f3"));
                        }
                        lst.Add("\t\t\tDispTop=50.700");
                        lst.Add("\t\t\tDispBottom=53.100");
                        lst.Add("\t\t\tDispRect=1738,29367,3376,31061");
                        lst.Add("\t\t\tMatrixA=-1.000000");
                        lst.Add("\t\t\tMatrixB=0.000000");
                        lst.Add("\t\t\tMatrixC=0.000000");
                        lst.Add("\t\t\tMatrixD=1.000000");
                        lst.Add("\t\t\tMatrixX=2557.816406");
                        lst.Add("\t\t\tMatrixY=30214.402344");
                        lst.Add("\t\t\tMatrixZ=1.000000");
                        lst.Add("\t\t\tBasicMatrixA=1.000000");
                        lst.Add("\t\t\tBasicMatrixB=0.000000");
                        lst.Add("\t\t\tBasicMatrixC=0.000000");
                        lst.Add("\t\t\tBasicMatrixD=-1.000000");
                        lst.Add("\t\t\tBasicMatrixX=2557.816406");
                        lst.Add("\t\t\tBasicMatrixY=30214.402344");
                        lst.Add("\t\t\tBasicMatrixZ=1.000000");
                        lst.Add("\t\t\tRotateMatrixA=-1.000000");
                        lst.Add("\t\t\tRotateMatrixB=0.000000");
                        lst.Add("\t\t\tRotateMatrixC=0.000000");
                        lst.Add("\t\t\tRotateMatrixD=-1.000000");
                        lst.Add("\t\t\tRotateMatrixX=0.003906");
                        lst.Add("\t\t\tRotateMatrixY=0.003906");
                        lst.Add("\t\t\tRotateMatrixZ=1.000000");
                        if (count == 1) lst.Add("\t\t\tRotateAngle=0.000000");
                        if (count == 2) lst.Add("\t\t\tRotateAngle=90.000000");
                        if (count == 3) lst.Add("\t\t\tRotateAngle=180.000000");
                        if (count == 4) lst.Add("\t\t\tRotateAngle=270.000000");
                        lst.Add("\t\t\tParaNumber=4");
                        lst.Add("\t\t\tLockObject=0");
                        lst.Add("\t\t[END Attribute]");
                        lst.Add('\t' + "[END_ObjectTEXT]");
                        lst.Add(" ");
                    }
                    else if (model.Mode == 2)
                    {
                        lst.Add('\t' + "[TEXT OBJECT]");
                        lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                        nObject++;
                        lst.Add("\t\t[TEXT_ATTRIBUTE]");
                        lst.Add("\t\tContent=");
                        lst.Add("\t\tMaxNum=1000");
                        lst.Add("\t\tAlignChar=1");
                        lst.Add("\t\tAlignMode=0");
                        lst.Add("\t\tCapitalHeight=3.1585");
                        lst.Add("\t\tCharGap=0.0400");
                        lst.Add("\t\tFontName=C:\\Mime\\Logo\\" + rfile);
                        lst.Add("\t\tHeight=4.0076");
                        lst.Add("\t\tLineGap=1.0017");
                        lst.Add("\t\tWidth=1.0999");
                        lst.Add("\t\tSpaceSize=2.9998");
                        lst.Add("\t\tUseCapitalLineGap=0");
                        lst.Add("\t\tUseFixStringLenght=0");
                        lst.Add("\t\tUseLeftGapofeachChar=0");
                        lst.Add("\t\tUseFixNumberofChar=0");
                        lst.Add("\t\tFixedStrSize=1000");
                        lst.Add("\t\tFixNumberofChar=0");
                        lst.Add("\t\tItalicAngle=0");
                        lst.Add("\t\tNTMark_WidthStype=0");
                        lst.Add("\t\tUseStringCutting=0");
                        lst.Add("\t\tFirst=0");
                        lst.Add("\t\tCount=0");
                        lst.Add("\t\tUseCriterionChar=0");
                        lst.Add("\t\tCriterionChar=W");
                        lst.Add("\t\tUseFixStringLenToCurrent=0");
                        lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t\tStyle=0");
                        lst.Add("\t\t\tIncType=0");
                        lst.Add("\t\t\tStartNumber=0");
                        lst.Add("\t\t\tRangeStartNumber=0");
                        lst.Add("\t\t\tRangeEndNumber=0");
                        lst.Add("\t\t\tPreFix=");
                        lst.Add("\t\t\tPostFix=");
                        lst.Add("\t\t\tMaxCharSize=5");
                        lst.Add("\t\t\tSavedCurrentNumber=0");
                        lst.Add("\t\t\tSCHEME_Order=0");
                        lst.Add("\t\t\tSCHEME_Position=0");
                        lst.Add("\t\t\tSCHEME_Type=1");
                        lst.Add("\t\t\tSCHEME_Numbering=0");
                        lst.Add("\t\t\tSaveFileWhenUpdate=0");
                        lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                        lst.Add("\t\t\tNumericSystemType=0");
                        lst.Add("\t\t\tIncreaseLevels=1");
                        lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                        lst.Add("\t\t\t\tSpecial Type=2");
                        lst.Add("\t\t\t\tReference Number 1=2");
                        lst.Add("\t\t\t\tReference Number 2=0");
                        lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                        lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                        lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                        lst.Add("\t\t[Attribute]");
                        lst.Add("\t\tObjType=TEXT");
                        lst.Add("\t\tIsSelect=0");
                        lst.Add("\t\tIsUpdate=0");
                        lst.Add("\t\t\tAxisField=120.000000");
                        lst.Add("\t\t\tUNIT=1");
                        if (cl == 0)
                        {
                            lst.Add("\t\t\tDispLeft=0.184");
                            lst.Add("\t\t\tDispRight=3.184");
                        }
                        else
                        {
                            lst.Add("\t\t\tDispLeft=" + (model.StepPitch - 3.184).ToString("f3"));
                            lst.Add("\t\t\tDispRight=" + (model.StepPitch - 0.184).ToString("f3"));
                        }
                        lst.Add("\t\t\tDispTop=70.700");
                        lst.Add("\t\t\tDispBottom=73.100");
                        lst.Add("\t\t\tDispRect=1738,29367,3376,31061");
                        lst.Add("\t\t\tMatrixA=-1.000000");
                        lst.Add("\t\t\tMatrixB=0.000000");
                        lst.Add("\t\t\tMatrixC=0.000000");
                        lst.Add("\t\t\tMatrixD=1.000000");
                        lst.Add("\t\t\tMatrixX=2557.816406");
                        lst.Add("\t\t\tMatrixY=30214.402344");
                        lst.Add("\t\t\tMatrixZ=1.000000");
                        lst.Add("\t\t\tBasicMatrixA=1.000000");
                        lst.Add("\t\t\tBasicMatrixB=0.000000");
                        lst.Add("\t\t\tBasicMatrixC=0.000000");
                        lst.Add("\t\t\tBasicMatrixD=-1.000000");
                        lst.Add("\t\t\tBasicMatrixX=2557.816406");
                        lst.Add("\t\t\tBasicMatrixY=30214.402344");
                        lst.Add("\t\t\tBasicMatrixZ=1.000000");
                        lst.Add("\t\t\tRotateMatrixA=-1.000000");
                        lst.Add("\t\t\tRotateMatrixB=0.000000");
                        lst.Add("\t\t\tRotateMatrixC=0.000000");
                        lst.Add("\t\t\tRotateMatrixD=-1.000000");
                        lst.Add("\t\t\tRotateMatrixX=0.003906");
                        lst.Add("\t\t\tRotateMatrixY=0.003906");
                        lst.Add("\t\t\tRotateMatrixZ=1.000000");
                        if (count == 1) lst.Add("\t\t\tRotateAngle=0.000000");
                        if (count == 2) lst.Add("\t\t\tRotateAngle=90.000000");
                        if (count == 3) lst.Add("\t\t\tRotateAngle=180.000000");
                        if (count == 4) lst.Add("\t\t\tRotateAngle=270.000000");
                        lst.Add("\t\t\tParaNumber=4");
                        lst.Add("\t\t\tLockObject=0");
                        lst.Add("\t\t[END Attribute]");
                        lst.Add('\t' + "[END_ObjectTEXT]");
                        lst.Add(" ");
                    }
                    lst.Add('\t' + "[TEXT OBJECT]");
                    lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                    nObject++;
                    lst.Add("\t\t[TEXT_ATTRIBUTE]");
                    lst.Add("\t\tContent=");
                    lst.Add("\t\tMaxNum=1000");
                    lst.Add("\t\tAlignChar=1");
                    lst.Add("\t\tAlignMode=0");
                    lst.Add("\t\tCapitalHeight=3.1585");
                    lst.Add("\t\tCharGap=0.0400");
                    lst.Add("\t\tFontName=C:\\Mime\\Logo\\" + file);
                    lst.Add("\t\tHeight=4.0076");
                    lst.Add("\t\tLineGap=1.0017");
                    lst.Add("\t\tWidth=1.0999");
                    lst.Add("\t\tSpaceSize=2.9998");
                    lst.Add("\t\tUseCapitalLineGap=0");
                    lst.Add("\t\tUseFixStringLenght=0");
                    lst.Add("\t\tUseLeftGapofeachChar=0");
                    lst.Add("\t\tUseFixNumberofChar=0");
                    lst.Add("\t\tFixedStrSize=1000");
                    lst.Add("\t\tFixNumberofChar=0");
                    lst.Add("\t\tItalicAngle=0");
                    lst.Add("\t\tNTMark_WidthStype=0");
                    lst.Add("\t\tUseStringCutting=0");
                    lst.Add("\t\tFirst=0");
                    lst.Add("\t\tCount=0");
                    lst.Add("\t\tUseCriterionChar=0");
                    lst.Add("\t\tCriterionChar=W");
                    lst.Add("\t\tUseFixStringLenToCurrent=0");
                    lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t\tStyle=0");
                    lst.Add("\t\t\tIncType=0");
                    lst.Add("\t\t\tStartNumber=0");
                    lst.Add("\t\t\tRangeStartNumber=0");
                    lst.Add("\t\t\tRangeEndNumber=0");
                    lst.Add("\t\t\tPreFix=");
                    lst.Add("\t\t\tPostFix=");
                    lst.Add("\t\t\tMaxCharSize=5");
                    lst.Add("\t\t\tSavedCurrentNumber=0");
                    lst.Add("\t\t\tSCHEME_Order=0");
                    lst.Add("\t\t\tSCHEME_Position=0");
                    lst.Add("\t\t\tSCHEME_Type=1");
                    lst.Add("\t\t\tSCHEME_Numbering=0");
                    lst.Add("\t\t\tSaveFileWhenUpdate=0");
                    lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                    lst.Add("\t\t\tNumericSystemType=0");
                    lst.Add("\t\t\tIncreaseLevels=1");
                    lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\t\t\tSpecial Type=2");
                    if (model.Mode == 0)
                        lst.Add("\t\t\t\tReference Number 1=1");
                    else if (model.Mode == 1)
                        lst.Add("\t\t\t\tReference Number 1=2");
                    else
                        lst.Add("\t\t\t\tReference Number 1=3");
                    lst.Add("\t\t\t\tReference Number 2=0");
                    lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                    lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                    lst.Add("\t\t[Attribute]");
                    lst.Add("\t\tObjType=TEXT");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=120.000000");
                    lst.Add("\t\t\tUNIT=1");
                    if (cl == 0)
                    {
                        lst.Add("\t\t\tDispLeft=0.184");
                        lst.Add("\t\t\tDispRight=3.184");
                    }
                    else
                    {
                        lst.Add("\t\t\tDispLeft=" + (model.StepPitch - 3.184).ToString("f3"));
                        lst.Add("\t\t\tDispRight=" + (model.StepPitch - 0.184).ToString("f3"));
                    }
                    if (model.Mode == 0)
                    {
                        lst.Add("\t\t\tDispTop=50.700");
                        lst.Add("\t\t\tDispBottom=53.100");
                    }
                    else if (model.Mode == 1)
                    {
                        lst.Add("\t\t\tDispTop=70.700");
                        lst.Add("\t\t\tDispBottom=73.100");
                    }
                    else
                    {
                        lst.Add("\t\t\tDispTop=90.700");
                        lst.Add("\t\t\tDispBottom=93.100");
                    }
                    lst.Add("\t\t\tDispRect=1738,29367,3376,31061");
                    lst.Add("\t\t\tMatrixA=-1.000000");
                    lst.Add("\t\t\tMatrixB=0.000000");
                    lst.Add("\t\t\tMatrixC=0.000000");
                    lst.Add("\t\t\tMatrixD=1.000000");
                    lst.Add("\t\t\tMatrixX=2557.816406");
                    lst.Add("\t\t\tMatrixY=30214.402344");
                    lst.Add("\t\t\tMatrixZ=1.000000");
                    lst.Add("\t\t\tBasicMatrixA=1.000000");
                    lst.Add("\t\t\tBasicMatrixB=0.000000");
                    lst.Add("\t\t\tBasicMatrixC=0.000000");
                    lst.Add("\t\t\tBasicMatrixD=-1.000000");
                    lst.Add("\t\t\tBasicMatrixX=2557.816406");
                    lst.Add("\t\t\tBasicMatrixY=30214.402344");
                    lst.Add("\t\t\tBasicMatrixZ=1.000000");
                    lst.Add("\t\t\tRotateMatrixA=-1.000000");
                    lst.Add("\t\t\tRotateMatrixB=0.000000");
                    lst.Add("\t\t\tRotateMatrixC=0.000000");
                    lst.Add("\t\t\tRotateMatrixD=-1.000000");
                    lst.Add("\t\t\tRotateMatrixX=0.003906");
                    lst.Add("\t\t\tRotateMatrixY=0.003906");
                    lst.Add("\t\t\tRotateMatrixZ=1.000000");
                    if (count == 1) lst.Add("\t\t\tRotateAngle=0.000000");
                    if (count == 2) lst.Add("\t\t\tRotateAngle=90.000000");
                    if (count == 3) lst.Add("\t\t\tRotateAngle=180.000000");
                    if (count == 4) lst.Add("\t\t\tRotateAngle=270.000000");
                    lst.Add("\t\t\tParaNumber=4");
                    lst.Add("\t\t\tLockObject=0");
                    lst.Add("\t\t[END Attribute]");
                    lst.Add('\t' + "[END_ObjectTEXT]");
                    lst.Add(" ");
                }

                if (week > 0)
                {
                    lst.Add('\t' + "[TEXT OBJECT]");
                    lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                    lst.Add("\t\t[TEXT_ATTRIBUTE]");
                    lst.Add("\t\tContent=");
                    lst.Add("\t\tMaxNum=1000");
                    lst.Add("\t\tAlignChar=1");
                    lst.Add("\t\tAlignMode=0");
                    lst.Add("\t\tCapitalHeight=3.1585");
                    lst.Add("\t\tCharGap=0.0400");
                    lst.Add("\t\tFontName=C:\\Mime\\Logo\\" + file);
                    lst.Add("\t\tHeight=4.0076");
                    lst.Add("\t\tLineGap=1.0017");
                    lst.Add("\t\tWidth=1.0999");
                    lst.Add("\t\tSpaceSize=2.9998");
                    lst.Add("\t\tUseCapitalLineGap=0");
                    lst.Add("\t\tUseFixStringLenght=0");
                    lst.Add("\t\tUseLeftGapofeachChar=0");
                    lst.Add("\t\tUseFixNumberofChar=0");
                    lst.Add("\t\tFixedStrSize=1000");
                    lst.Add("\t\tFixNumberofChar=0");
                    lst.Add("\t\tItalicAngle=0");
                    lst.Add("\t\tNTMark_WidthStype=0");
                    lst.Add("\t\tUseStringCutting=0");
                    lst.Add("\t\tFirst=0");
                    lst.Add("\t\tCount=0");
                    lst.Add("\t\tUseCriterionChar=0");
                    lst.Add("\t\tCriterionChar=W");
                    lst.Add("\t\tUseFixStringLenToCurrent=0");
                    lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t\tStyle=0");
                    lst.Add("\t\t\tIncType=0");
                    lst.Add("\t\t\tStartNumber=0");
                    lst.Add("\t\t\tRangeStartNumber=0");
                    lst.Add("\t\t\tRangeEndNumber=0");
                    lst.Add("\t\t\tPreFix=");
                    lst.Add("\t\t\tPostFix=");
                    lst.Add("\t\t\tMaxCharSize=5");
                    lst.Add("\t\t\tSavedCurrentNumber=0");
                    lst.Add("\t\t\tSCHEME_Order=0");
                    lst.Add("\t\t\tSCHEME_Position=0");
                    lst.Add("\t\t\tSCHEME_Type=1");
                    lst.Add("\t\t\tSCHEME_Numbering=0");
                    lst.Add("\t\t\tSaveFileWhenUpdate=0");
                    lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                    lst.Add("\t\t\tNumericSystemType=0");
                    lst.Add("\t\t\tIncreaseLevels=1");
                    lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\t\t\tSpecial Type=2");
                    lst.Add("\t\t\t\tReference Number 1=4");
                    lst.Add("\t\t\t\tReference Number 2=0");
                    lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                    lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                    lst.Add("\t\t[Attribute]");
                    lst.Add("\t\tObjType=TEXT");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=120.000000");
                    lst.Add("\t\t\tUNIT=1");
                    if (wl == 1)
                    {
                        lst.Add("\t\t\tDispLeft=" + (model.StepPitch - 3.184).ToString("f3"));
                        lst.Add("\t\t\tDispRight=" + (model.StepPitch - 1.184).ToString("f3"));
                        lst.Add("\t\t\tDispTop=50.700");
                        lst.Add("\t\t\tDispBottom=53.100");
                    }
                    else if (wl == 2)
                    {
                        lst.Add("\t\t\tDispLeft=" + (model.StepPitch / 2 - 3.184).ToString("f3"));
                        lst.Add("\t\t\tDispRight=" + (model.StepPitch / 2 - 1.184).ToString("f3"));
                        lst.Add("\t\t\tDispTop=20.133");
                        lst.Add("\t\t\tDispBottom=22.134");
                    }
                    else
                    {
                        lst.Add("\t\t\tDispLeft=1.184");
                        lst.Add("\t\t\tDispRight=3.184");
                        lst.Add("\t\t\tDispTop=50.700");
                        lst.Add("\t\t\tDispBottom=53.100");
                    }
                    lst.Add("\t\t\tDispRect=1738,29367,3376,31061");
                    lst.Add("\t\t\tMatrixA=-1.000000");
                    lst.Add("\t\t\tMatrixB=0.000000");
                    lst.Add("\t\t\tMatrixC=0.000000");
                    lst.Add("\t\t\tMatrixD=1.000000");
                    lst.Add("\t\t\tMatrixX=2557.816406");
                    lst.Add("\t\t\tMatrixY=30214.402344");
                    lst.Add("\t\t\tMatrixZ=1.000000");
                    lst.Add("\t\t\tBasicMatrixA=1.000000");
                    lst.Add("\t\t\tBasicMatrixB=0.000000");
                    lst.Add("\t\t\tBasicMatrixC=0.000000");
                    lst.Add("\t\t\tBasicMatrixD=-1.000000");
                    lst.Add("\t\t\tBasicMatrixX=2557.816406");
                    lst.Add("\t\t\tBasicMatrixY=30214.402344");
                    lst.Add("\t\t\tBasicMatrixZ=1.000000");
                    lst.Add("\t\t\tRotateMatrixA=-1.000000");
                    lst.Add("\t\t\tRotateMatrixB=0.000000");
                    lst.Add("\t\t\tRotateMatrixC=0.000000");
                    lst.Add("\t\t\tRotateMatrixD=-1.000000");
                    lst.Add("\t\t\tRotateMatrixX=0.003906");
                    lst.Add("\t\t\tRotateMatrixY=0.003906");
                    lst.Add("\t\t\tRotateMatrixZ=1.000000");
                    if (wl == 2)
                    {
                        if (week == 1) lst.Add("\t\t\tRotateAngle=180.000000");
                        if (week == 2) lst.Add("\t\t\tRotateAngle=0.000000");
                    }
                    else
                    {
                        if (week == 1) lst.Add("\t\t\tRotateAngle=270.000000");
                        if (week == 2) lst.Add("\t\t\tRotateAngle=90.000000");
                    }
                    lst.Add("\t\t\tParaNumber=4");
                    lst.Add("\t\t\tLockObject=0");
                    lst.Add("\t\t[END Attribute]");
                    lst.Add('\t' + "[END_ObjectTEXT]");
                    lst.Add(" ");
                }


                lst.Add('\t' + "[GUIDE LINE]");
                for (int i = 0; i < 8; i++)
                {
                    lst.Add('\t' + "ItemNum=" + i.ToString());
                    lst.Add('\t' + "ItemUse=0");
                    lst.Add('\t' + "ItemAlign=0");
                    lst.Add('\t' + "ItemPosX=0.000");
                    lst.Add('\t' + "ItemPosY=0.000");
                    lst.Add('\t' + "ItemWidth=0.000");
                    lst.Add('\t' + "ItemHeight=0.000");
                }
                lst.Add('\t' + "[END GUIDE LINE]");
                lst.Add('\t' + "[AUTO ALIGNMENT INFO]");
                lst.Add('\t' + "Use Auto Horizontal Align=0");
                lst.Add('\t' + "Use Auto Vertical Align=0");
                lst.Add('\t' + "[END AUTO ALIGNMENT INFO]");
                lst.Add("[END SemData]");
            }
            catch { return null; }
            return lst;
        }
        #endregion

        #region Create ROI
        public void AddRailDummy(DrawingCanvas anRoiCanvas, GraphicsBase graphic, System.Windows.Point[] sa, Strip strip, ModelMarkInfo model)
        {
            double itx = 0.0;
            double ity = 0.0;
            double top, left, right, bottom;
            double ox = 0.0;
            double ax = 0.0;
            Color graphicObjectColor = Colors.Yellow;
            if (sa[0].Y > 0 && sa[1].Y > 0)
            {
                ox = ((sa[1].Y - sa[0].Y) / (sa[1].X - sa[0].X));
            }
            int nPos = 0;
            for (int s = 0; s < model.Step; s++)
            {
                nPos = 0;
                double StepY = (model.StepPitch * 1000.0 / ResolutionY) * s;
                for (int i = 0; i < model.StepUnits; i++)
                {
                    double UnitY = StepY + ((model.UnitWidth * 1000.0) / ResolutionY * i);
                    for (int j = 0; j < model.UnitRow; j++)
                    {
                        if (j == 0 && i == 0 && s == 0)
                        {
                            nPos++;
                            continue;
                        }
                        ity = UnitY + (strip.OneChipOffset[nPos].X) * 1000.0 / ResolutionY;
                        itx = -(strip.OneChipOffset[nPos].Y) * 1000.0 / ResolutionX;
                        eMarkingType t = graphic.MarkInfo.MarkType.MarkType;
                        switch (t)
                        {
                            case eMarkingType.eMarkingRailCircle:
                                GraphicsEllipseBase ge = graphic as GraphicsEllipseBase;
                                GraphicsBase gc = null;
                                top = ge.Top + ity;
                                bottom = ge.Bottom + ity;

                                ax = (top - ge.Top) / ox;

                                left = ge.Left + itx + ax;
                                right = ge.Right + itx + ax;
                                double cpx = left + (right - left) / 2.0;
                                double cpy = top + (bottom - top) / 2.0;
                                GraphicsEllipseMark graphicse = new GraphicsEllipseMark(new Circle(new System.Windows.Point(cpx, cpy), (right - left) / 2.0), anRoiCanvas.LineWidth, anRoiCanvas.ActualScale, true, true, graphic.MarkID, GraphicsRegionType.MarkingRail);
                                gc = graphicse;
                                gc.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gc.MarkInfo.MarkType = graphic.MarkInfo.MarkType.Clone();
                                RailCircleProperty rc = gc.MarkInfo.MarkInfo as RailCircleProperty;
                                rc.GapX = strip.OneChipOffset[nPos].X;
                                rc.GapY = strip.OneChipOffset[nPos].Y;

                                gc.Step = s;
                                gc.UnitColumn = i;
                                gc.UnitRow = j;
                                // gc.MarkInfo = null;
                                if (gc != null)
                                {
                                    gc.RegionType = GraphicsRegionType.MarkingRail;
                                    anRoiCanvas.GraphicsList.Add(gc);
                                    gc.RefreshDrawing();
                                }
                                break;
                            case eMarkingType.eMarkingRailRect:
                                GraphicsBase gr = null;
                                GraphicsRectangleBase gr1 = graphic as GraphicsRectangleBase;
                                top = gr1.Top + ity;
                                bottom = gr1.Bottom;

                                ax = (top - gr1.Top) / ox;
                                bottom = gr1.Bottom + ity;
                                left = gr1.Left + itx + ax;
                                right = gr1.Right + itx + ax;
                                GraphicsRectangleMark graphicsr = new GraphicsRectangleMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, anRoiCanvas.ActualScale, true, true, graphic.MarkID, GraphicsRegionType.MarkingRail);
                                gr = graphicsr;
                                gr.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gr.MarkInfo.MarkType = graphic.MarkInfo.MarkType.Clone();
                                RailRectProperty rr = gr.MarkInfo.MarkInfo as RailRectProperty;
                                rr.GapX = strip.OneChipOffset[nPos].X;
                                rr.GapY = strip.OneChipOffset[nPos].Y;
                                gr.Step = s;
                                gr.UnitColumn = i;
                                gr.UnitRow = j;
                                // gr.MarkInfo = null;
                                if (gr != null)
                                {
                                    gr.RegionType = GraphicsRegionType.MarkingRail;
                                    anRoiCanvas.GraphicsList.Add(gr);
                                    gr.RefreshDrawing();
                                }
                                break;
                            case eMarkingType.eMarkingRailSpecial:
                                GraphicsBase gs = null;
                                GraphicsSpecialMark gr2 = graphic as GraphicsSpecialMark;
                                top = gr2.Top + ity;
                                bottom = gr2.Bottom + ity;
                                ax = (top - gr2.Top) / ox;
                                bottom = gr2.Bottom + ity;
                                left = gr2.Left + itx + ax;
                                right = gr2.Right + itx + ax;
                                GraphicsSpecialMark graphicss = new GraphicsSpecialMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, graphic.MarkID, anRoiCanvas.ActualScale, true, true, gr2.Shape, GraphicsRegionType.MarkingRail);
                                gs = graphicss;
                                gs.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gs.MarkInfo.MarkType = graphic.MarkInfo.MarkType.Clone();
                                RailSpecialProperty rs = gs.MarkInfo.MarkInfo as RailSpecialProperty;
                                rs.GapX = strip.OneChipOffset[nPos].X;
                                rs.GapY = strip.OneChipOffset[nPos].Y;
                                gs.Step = s;
                                gs.UnitColumn = i;
                                gs.UnitRow = j;
                                //  gs.MarkInfo = null;
                                if (gs != null)
                                {
                                    gs.RegionType = GraphicsRegionType.MarkingRail;
                                    anRoiCanvas.GraphicsList.Add(gs);
                                    gs.RefreshDrawing();
                                }
                                break;
                            case eMarkingType.eMarkingRailTri:
                                GraphicsBase gt = null;
                                GraphicsTriangleMark gr3 = graphic as GraphicsTriangleMark;
                                top = gr3.Top + ity;
                                bottom = gr3.Bottom + ity;
                                ax = (top - gr3.Top) / ox;
                                bottom = gr3.Bottom + ity;
                                left = gr3.Left + itx + ax;
                                right = gr3.Right + itx + ax;
                                GraphicsTriangleMark graphicst = new GraphicsTriangleMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, anRoiCanvas.ActualScale, true, true, graphic.MarkID, gr3.Rotate, GraphicsRegionType.MarkingRail);
                                gt = graphicst;
                                gt.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gt.MarkInfo.MarkType = graphic.MarkInfo.MarkType.Clone();
                                RailTriProperty rt = gt.MarkInfo.MarkInfo as RailTriProperty;
                                rt.GapX = strip.OneChipOffset[nPos].X;
                                rt.GapY = strip.OneChipOffset[nPos].Y;
                                gt.Step = s;
                                gt.UnitColumn = i;
                                gt.UnitRow = j;
                                //  gt.MarkInfo = null;
                                if (gt != null)
                                {
                                    gt.RegionType = GraphicsRegionType.MarkingRail;
                                    anRoiCanvas.GraphicsList.Add(gt);
                                    gt.RefreshDrawing();
                                }
                                break;
                        }
                        nPos++;
                    }
                }
            }
        }

        public void AddRailStepDummy(DrawingCanvas anRoiCanvas, GraphicsBase graphic, System.Windows.Point[] sa, Strip strip, ModelMarkInfo model, int nStep)
        {
            double itx = 0.0;
            double ity = 0.0;
            double top, left, right, bottom;
            double ox = 0.0;
            double ax = 0.0;
            Color graphicObjectColor = Colors.Yellow;
            if (sa[0].Y > 0 && sa[1].Y > 0)
            {
                ox = ((sa[1].Y - sa[0].Y) / (sa[1].X - sa[0].X));
            }

            int nPos = 0;
            int s = nStep;
            nPos = 0;
            double StepY = (model.StepPitch * 1000.0 / ResolutionY) * s;
            for (int i = 0; i < model.StepUnits; i++)
            {
                //double UnitY = StepY + ((strip.SpecialPitch[0] * 1000.0) / Resolution * i);
                double UnitY = StepY + ((model.UnitWidth * 1000.0) / ResolutionY * i);
                for (int j = 0; j < model.UnitRow; j++)
                {
                    if (j == 0 && i == 0 && s == 0)
                    {
                        nPos++;
                        continue;
                    }
                    ity = UnitY + (strip.OneChipOffset[nPos].X) * 1000.0 / ResolutionY;
                    itx = -(strip.OneChipOffset[nPos].Y) * 1000.0 / ResolutionX;
                    eMarkingType t = graphic.MarkInfo.MarkType.MarkType;
                    switch (t)
                    {
                        case eMarkingType.eMarkingRailCircle:
                            GraphicsEllipseBase ge = graphic as GraphicsEllipseBase;
                            GraphicsBase gc = null;
                            top = ge.Top + ity;
                            bottom = ge.Bottom + ity;

                            ax = (top - ge.Top) / ox;

                            left = ge.Left + itx + ax;
                            right = ge.Right + itx + ax;
                            double cpx = left + (right - left) / 2.0;
                            double cpy = top + (bottom - top) / 2.0;
                            GraphicsEllipseMark graphicse = new GraphicsEllipseMark(new Circle(new System.Windows.Point(cpx, cpy), (right - left) / 2.0), anRoiCanvas.LineWidth, anRoiCanvas.ActualScale, true, true, graphic.MarkID, GraphicsRegionType.MarkingRail);
                            gc = graphicse;
                            gc.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                            gc.MarkInfo.MarkType = graphic.MarkInfo.MarkType.Clone();
                            RailCircleProperty rc = gc.MarkInfo.MarkInfo as RailCircleProperty;
                            rc.GapX = strip.OneChipOffset[nPos].X;
                            rc.GapY = strip.OneChipOffset[nPos].Y;

                            gc.Step = s;
                            gc.UnitColumn = i;
                            gc.UnitRow = j;
                            // gc.MarkInfo = null;
                            if (gc != null)
                            {
                                gc.RegionType = GraphicsRegionType.MarkingRail;
                                anRoiCanvas.GraphicsList.Add(gc);
                                gc.RefreshDrawing();
                            }
                            break;
                        case eMarkingType.eMarkingRailRect:
                            GraphicsBase gr = null;
                            GraphicsRectangleBase gr1 = graphic as GraphicsRectangleBase;
                            top = gr1.Top + ity;
                            bottom = gr1.Bottom;

                            ax = (top - gr1.Top) / ox;
                            bottom = gr1.Bottom + ity;
                            left = gr1.Left + itx + ax;
                            right = gr1.Right + itx + ax;
                            GraphicsRectangleMark graphicsr = new GraphicsRectangleMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, anRoiCanvas.ActualScale, true, true, graphic.MarkID, GraphicsRegionType.MarkingRail);
                            gr = graphicsr;
                            gr.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                            gr.MarkInfo.MarkType = graphic.MarkInfo.MarkType.Clone();
                            RailRectProperty rr = gr.MarkInfo.MarkInfo as RailRectProperty;
                            rr.GapX = strip.OneChipOffset[nPos].X;
                            rr.GapY = strip.OneChipOffset[nPos].Y;
                            gr.Step = s;
                            gr.UnitColumn = i;
                            gr.UnitRow = j;
                            // gr.MarkInfo = null;
                            if (gr != null)
                            {
                                gr.RegionType = GraphicsRegionType.MarkingRail;
                                anRoiCanvas.GraphicsList.Add(gr);
                                gr.RefreshDrawing();
                            }
                            break;
                        case eMarkingType.eMarkingRailSpecial:
                            GraphicsBase gs = null;
                            GraphicsSpecialMark gr2 = graphic as GraphicsSpecialMark;
                            top = gr2.Top + ity;
                            bottom = gr2.Bottom + ity;
                            ax = (top - gr2.Top) / ox;
                            bottom = gr2.Bottom + ity;
                            left = gr2.Left + itx + ax;
                            right = gr2.Right + itx + ax;
                            GraphicsSpecialMark graphicss = new GraphicsSpecialMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, graphic.MarkID, anRoiCanvas.ActualScale, true, true, gr2.Shape, GraphicsRegionType.MarkingRail);
                            gs = graphicss;
                            gs.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                            gs.MarkInfo.MarkType = graphic.MarkInfo.MarkType.Clone();
                            RailSpecialProperty rs = gs.MarkInfo.MarkInfo as RailSpecialProperty;
                            rs.GapX = strip.OneChipOffset[nPos].X;
                            rs.GapY = strip.OneChipOffset[nPos].Y;
                            gs.Step = s;
                            gs.UnitColumn = i;
                            gs.UnitRow = j;
                            //  gs.MarkInfo = null;
                            if (gs != null)
                            {
                                gs.RegionType = GraphicsRegionType.MarkingRail;
                                anRoiCanvas.GraphicsList.Add(gs);
                                gs.RefreshDrawing();
                            }
                            break;
                        case eMarkingType.eMarkingRailTri:
                            GraphicsBase gt = null;
                            GraphicsTriangleMark gr3 = graphic as GraphicsTriangleMark;
                            top = gr3.Top + ity;
                            bottom = gr3.Bottom + ity;
                            ax = (top - gr3.Top) / ox;
                            bottom = gr3.Bottom + ity;
                            left = gr3.Left + itx + ax;
                            right = gr3.Right + itx + ax;
                            GraphicsTriangleMark graphicst = new GraphicsTriangleMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, anRoiCanvas.ActualScale, true, true, graphic.MarkID, gr3.Rotate, GraphicsRegionType.MarkingRail);
                            gt = graphicst;
                            gt.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                            gt.MarkInfo.MarkType = graphic.MarkInfo.MarkType.Clone();
                            RailTriProperty rt = gt.MarkInfo.MarkInfo as RailTriProperty;
                            rt.GapX = strip.OneChipOffset[nPos].X;
                            rt.GapY = strip.OneChipOffset[nPos].Y;
                            gt.Step = s;
                            gt.UnitColumn = i;
                            gt.UnitRow = j;
                            //  gt.MarkInfo = null;
                            if (gt != null)
                            {
                                gt.RegionType = GraphicsRegionType.MarkingRail;
                                anRoiCanvas.GraphicsList.Add(gt);
                                gt.RefreshDrawing();
                            }
                            break;
                    }
                    nPos++;
                }
            }
        }

        public void AddUnitDummy(DrawingCanvas anRoiCanvas, GraphicsBase graphic, System.Windows.Point[] sa, ModelMarkInfo model)
        {
            double itx = 0.0;
            double ity = 0.0;
            double top, left, right, bottom;
            double ox = 0.000001;
            double ax = 0.000001;
            if (sa[0].Y > 0 && sa[1].Y > 0)
            {
                ox = ((sa[1].Y - sa[0].Y) / (sa[1].X - sa[0].X));
            }
            double oy = 0.00000;
            double ay = 0.00000;
            if (sa[0].X > 0 && sa[2].X > 0)
            {
                oy = (sa[2].X - sa[0].X) / (sa[0].Y - sa[2].Y);
            }

            Color graphicObjectColor = Colors.Yellow;
            for (int s = 0; s < model.Step; s++)
            {
                double StepY = (model.StepPitch * 1000.0 / ResolutionY) * s;
                for (int i = 0; i < model.StepUnits; i++)
                {
                    ity = StepY + (model.UnitWidth * 1000.0 / ResolutionY) * i;
                    itx = 0.0;
                    for (int j = 0; j < model.UnitRow; j++)
                    {
                        if (i == 0 && j == 0 && s == 0) continue;
                        itx = -(model.UnitHeight * 1000.0 / ResolutionX) * j;
                        eMarkingType t = graphic.MarkInfo.MarkType.MarkType;
                        switch (t)
                        {
                            case eMarkingType.eMarkingUnitCircle:
                                GraphicsEllipseBase ge = graphic as GraphicsEllipseBase;
                                GraphicsBase gc = null;
                                top = ge.Top + ity;
                                ax = (top - ge.Top) / ox;
                                bottom = ge.Bottom + ity;
                                left = ge.Left + itx + ax;
                                right = ge.Right + itx + ax;
                                ay = (ge.Left - left) / oy;
                                top += ay;
                                bottom += ay;
                                double cpx = left + (right - left) / 2.0;
                                double cpy = top + (bottom - top) / 2.0;
                                GraphicsEllipseMark graphicse = new GraphicsEllipseMark(new Circle(new System.Windows.Point(cpx, cpy), (right - left) / 2.0), anRoiCanvas.LineWidth, anRoiCanvas.ActualScale, true, true, graphic.MarkID, GraphicsRegionType.MarkingUnit);
                                gc = graphicse;
                                gc.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gc.MarkInfo.MarkType.MarkType = eMarkingType.eMarkingUnitCircle;
                                gc.Step = s;
                                gc.UnitColumn = i;
                                gc.UnitRow = model.UnitRow - j - 1;
                                //  gc.MarkInfo = null;
                                if (gc != null)
                                {
                                    gc.RegionType = GraphicsRegionType.MarkingUnit;
                                    anRoiCanvas.GraphicsList.Add(gc);
                                    gc.RefreshDrawing();
                                }
                                break;
                            case eMarkingType.eMarkingUnitRect:
                                GraphicsBase gr = null;
                                GraphicsRectangleBase gr1 = graphic as GraphicsRectangleBase;
                                top = gr1.Top + ity;
                                ax = (top - gr1.Top) / ox;
                                bottom = gr1.Bottom + ity;
                                left = gr1.Left + itx + ax;
                                right = gr1.Right + itx + ax;
                                ay = (gr1.Left - left) / oy;
                                top += ay;
                                bottom += ay;
                                GraphicsRectangleMark graphicsr = new GraphicsRectangleMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, anRoiCanvas.ActualScale, true, true, graphic.MarkID, GraphicsRegionType.MarkingUnit);
                                gr = graphicsr;
                                gr.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gr.MarkInfo.MarkType.MarkType = eMarkingType.eMarkingUnitRect;
                                gr.Step = s;
                                gr.UnitColumn = i;
                                gr.UnitRow = model.UnitRow - j - 1;
                                // gr.MarkInfo = null;
                                if (gr != null)
                                {
                                    gr.RegionType = GraphicsRegionType.MarkingUnit;
                                    anRoiCanvas.GraphicsList.Add(gr);
                                    gr.RefreshDrawing();
                                }
                                break;
                            case eMarkingType.eMarkingUnitSpecial:
                                GraphicsBase gs = null;
                                GraphicsSpecialMark gr2 = graphic as GraphicsSpecialMark;
                                top = gr2.Top + ity;
                                ax = (top - gr2.Top) / ox;
                                bottom = gr2.Bottom + ity;
                                left = gr2.Left + itx + ax;
                                right = gr2.Right + itx + ax;
                                ay = (gr2.Left - left) / oy;
                                top += ay;
                                bottom += ay;
                                GraphicsSpecialMark graphicss = new GraphicsSpecialMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, graphic.MarkID, anRoiCanvas.ActualScale, true, true, gr2.Shape, GraphicsRegionType.MarkingUnit);
                                gs = graphicss;
                                gs.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gs.MarkInfo.MarkType.MarkType = eMarkingType.eMarkingUnitSpecial;
                                gs.Step = s;
                                gs.UnitColumn = i;
                                gs.UnitRow = model.UnitRow - j - 1;
                                //  gs.MarkInfo = null;
                                if (gs != null)
                                {
                                    gs.RegionType = GraphicsRegionType.MarkingUnit;
                                    anRoiCanvas.GraphicsList.Add(gs);
                                    gs.RefreshDrawing();
                                }
                                break;
                            case eMarkingType.eMarkingUnitTri:
                                GraphicsBase gt = null;
                                GraphicsTriangleMark gr3 = graphic as GraphicsTriangleMark;
                                top = gr3.Top + ity;
                                ax = (top - gr3.Top) / ox;
                                bottom = gr3.Bottom + ity;
                                left = gr3.Left + itx + ax;
                                right = gr3.Right + itx + ax;
                                ay = (gr3.Left - left) / oy;
                                top += ay;
                                bottom += ay;
                                GraphicsTriangleMark graphicst = new GraphicsTriangleMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, anRoiCanvas.ActualScale, true, true, graphic.MarkID, gr3.Rotate, GraphicsRegionType.MarkingUnit);
                                gt = graphicst;
                                gt.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gt.MarkInfo.MarkType.MarkType = eMarkingType.eMarkingUnitTri;
                                gt.Step = s;
                                gt.UnitColumn = i;
                                gt.UnitRow = model.UnitRow - j - 1;
                                //  gt.MarkInfo = null;
                                if (gt != null)
                                {
                                    gt.RegionType = GraphicsRegionType.MarkingUnit;
                                    anRoiCanvas.GraphicsList.Add(gt);
                                    gt.RefreshDrawing();
                                }
                                break;
                        }
                    }
                }
            }

        }

        public void AddUnitDummyReverse(DrawingCanvas anRoiCanvas, GraphicsBase graphic, System.Windows.Point[] sa, ModelMarkInfo model)
        {
            double itx = 0.0;
            double ity = 0.0;
            double top, left, right, bottom;
            double ox = 0.000001;
            double ax = 0.000001;
            if (sa[0].Y > 0 && sa[1].Y > 0)
            {
                ox = (sa[1].Y - sa[0].Y) / (sa[1].X - sa[0].X);
            }
            double oy = 0.00000;
            double ay = 0.00000;
            if (sa[0].X > 0 && sa[2].X > 0)
            {
                oy = (sa[2].X - sa[0].X) / (sa[0].Y - sa[2].Y);
            }

            Color graphicObjectColor = Colors.Yellow;
            for (int s = 0; s < model.Step; s++)
            {
                double StepY = (model.StepPitch * 1000.0 / ResolutionY) * s;
                for (int i = 0; i < model.StepUnits; i++)
                {
                    ity = StepY + (model.UnitWidth * 1000.0 / ResolutionY) * i;
                    itx = 0.0;
                    for (int j = 0; j < model.UnitRow; j++)
                    {
                        if (i == 0 && j == 0 && s == 0) continue;
                        itx = (model.UnitHeight * 1000.0 / ResolutionX) * j;
                        eMarkingType t = graphic.MarkInfo.MarkType.MarkType;
                        switch (t)
                        {
                            case eMarkingType.eMarkingUnitCircle:
                                GraphicsEllipseBase ge = graphic as GraphicsEllipseBase;
                                GraphicsBase gc = null;
                                top = ge.Top + ity;
                                ax = (top - ge.Top) / ox;
                                bottom = ge.Bottom + ity;
                                left = ge.Left + itx + ax;
                                right = ge.Right + itx + ax;
                                ay = (ge.Left - left) / oy;
                                top += ay;
                                bottom += ay;
                                double cpx = left + (right - left) / 2.0;
                                double cpy = top + (bottom - top) / 2.0;
                                GraphicsEllipseMark graphicse = new GraphicsEllipseMark(new Circle(new System.Windows.Point(cpx, cpy), (right - left) / 2.0), anRoiCanvas.LineWidth, anRoiCanvas.ActualScale, true, true, graphic.MarkID, GraphicsRegionType.MarkingUnit);
                                gc = graphicse;
                                gc.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gc.MarkInfo.MarkType.MarkType = eMarkingType.eMarkingUnitCircle;
                                gc.Step = s;
                                gc.UnitColumn = i;
                                gc.UnitRow = model.UnitRow - j - 1;
                                //  gc.MarkInfo = null;
                                if (gc != null)
                                {
                                    gc.RegionType = GraphicsRegionType.MarkingUnit;
                                    anRoiCanvas.GraphicsList.Add(gc);
                                    gc.RefreshDrawing();
                                }
                                break;
                            case eMarkingType.eMarkingUnitRect:
                                GraphicsBase gr = null;
                                GraphicsRectangleBase gr1 = graphic as GraphicsRectangleBase;
                                top = gr1.Top + ity;
                                ax = (top - gr1.Top) / ox;
                                bottom = gr1.Bottom + ity;
                                left = gr1.Left + itx + ax;
                                right = gr1.Right + itx + ax;
                                ay = (gr1.Left - left) / oy;
                                top += ay;
                                bottom += ay;
                                GraphicsRectangleMark graphicsr = new GraphicsRectangleMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, anRoiCanvas.ActualScale, true, true, graphic.MarkID, GraphicsRegionType.MarkingUnit);
                                gr = graphicsr;
                                gr.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gr.MarkInfo.MarkType.MarkType = eMarkingType.eMarkingUnitRect;
                                gr.Step = s;
                                gr.UnitColumn = i;
                                gr.UnitRow = model.UnitRow - j - 1;
                                // gr.MarkInfo = null;
                                if (gr != null)
                                {
                                    gr.RegionType = GraphicsRegionType.MarkingUnit;
                                    anRoiCanvas.GraphicsList.Add(gr);
                                    gr.RefreshDrawing();
                                }
                                break;
                            case eMarkingType.eMarkingUnitSpecial:
                                GraphicsBase gs = null;
                                GraphicsSpecialMark gr2 = graphic as GraphicsSpecialMark;
                                top = gr2.Top + ity;
                                ax = (top - gr2.Top) / ox;
                                bottom = gr2.Bottom + ity;
                                left = gr2.Left + itx + ax;
                                right = gr2.Right + itx + ax;
                                ay = (gr2.Left - left) / oy;
                                top += ay;
                                bottom += ay;
                                GraphicsSpecialMark graphicss = new GraphicsSpecialMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, graphic.MarkID, anRoiCanvas.ActualScale, true, true, gr2.Shape, GraphicsRegionType.MarkingUnit);
                                gs = graphicss;
                                gs.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gs.MarkInfo.MarkType.MarkType = eMarkingType.eMarkingUnitSpecial;
                                gs.Step = s;
                                gs.UnitColumn = i;
                                gs.UnitRow = model.UnitRow - j - 1;
                                //  gs.MarkInfo = null;
                                if (gs != null)
                                {
                                    gs.RegionType = GraphicsRegionType.MarkingUnit;
                                    anRoiCanvas.GraphicsList.Add(gs);
                                    gs.RefreshDrawing();
                                }
                                break;
                            case eMarkingType.eMarkingUnitTri:
                                GraphicsBase gt = null;
                                GraphicsTriangleMark gr3 = graphic as GraphicsTriangleMark;
                                top = gr3.Top + ity;
                                ax = (top - gr3.Top) / ox;
                                bottom = gr3.Bottom + ity;
                                left = gr3.Left + itx + ax;
                                right = gr3.Right + itx + ax;
                                ay = (gr3.Left - left) / oy;
                                top += ay;
                                bottom += ay;
                                GraphicsTriangleMark graphicst = new GraphicsTriangleMark(left, top, right, bottom, anRoiCanvas.LineWidth, graphicObjectColor, anRoiCanvas.ActualScale, true, true, graphic.MarkID, gr3.Rotate, GraphicsRegionType.MarkingUnit);
                                gt = graphicst;
                                gt.MarkInfo = new MarkItem(new MarkingType(), graphic.MarkInfo.MarkInfo);
                                gt.MarkInfo.MarkType.MarkType = eMarkingType.eMarkingUnitTri;
                                gt.Step = s;
                                gt.UnitColumn = i;
                                gt.UnitRow = model.UnitRow - j - 1;
                                //  gt.MarkInfo = null;
                                if (gt != null)
                                {
                                    gt.RegionType = GraphicsRegionType.MarkingUnit;
                                    anRoiCanvas.GraphicsList.Add(gt);
                                    gt.RefreshDrawing();
                                }
                                break;
                        }
                    }
                }
            }

        }

        public void LoadROI(DrawingCanvas anRoiCanvas, System.Windows.Point[] sa, double MarkCenterY, ModelMarkInfo model, MarkLogo logo)
        {
            //anRoiCanvas.GraphicsList.Clear();

            List<string> roiCodeList = new List<string>();
            List<int> markTypeList = new List<int>();
            List<GraphicsBase> roiList = new List<GraphicsBase>();
            GraphicsBase graphic = null;
            CreateGuideGraphicsToCanvas(null, anRoiCanvas, 0, MarkCenterY, model);
            int uc = model.Unit;
            int rc = model.Rail;
            int cc = model.Count;
            int wc = model.Week;
            int cl = model.CountLoc;
            int wl = model.WeekLoc;
            int idl = model.IDMark;
            try
            {
                #region Unit
                if (UnitMark != null && uc > 0)
                {

                    Sem sem = UnitMark.template.Sems;
                    Strip strip = UnitMark.template.Strips[0];
                    if (UnitMark.template.Section == eMarkSectionType.eSecTypeUnit)
                    {
                        CreateGuideGraphicsToCanvas(strip, anRoiCanvas, 1, MarkCenterY, model);
                        for (int j = 0; j < sem.lstHPGL.Count; j++)
                        {
                            graphic = CreateUnitGraphicsToCanvas(strip, sem.lstHPGL[j], anRoiCanvas); // Drawing Canvas에 ROI를 그린다.
                            if (graphic != null)
                            {
                                graphic.MarkOBJID = j;
                                if (anRoiCanvas.ID == 0) AddUnitDummyReverse(anRoiCanvas, graphic, sa, model);
                                else
                                {
                                    if (model.Selection > 0)
                                        AddUnitDummy(anRoiCanvas, graphic, sa, model);
                                    else AddUnitDummyReverse(anRoiCanvas, graphic, sa, model);
                                }
                            }
                        }
                    }
                }
                #endregion Unit
                #region Rail
                if (RailMark != null && rc > 0)
                {
                    Sem sem = RailMark.template.Sems;
                    bool railirr = model.RailIrr;
                    if (railirr)
                    {

                        Strip strip = RailMark.template.Strips[0];
                        if (RailMark.template.Section == eMarkSectionType.eSecTypeRail)
                        {
                            for (int j = 0; j < sem.lstHPGL.Count; j++)
                            {
                                //sem.lstHPGL[j].DispRight;// -= sem.lstHPGL[j].DispLeft;
                                //sem.lstHPGL[j].DispBottom;// -= sem.lstHPGL[j].DispTop;
                                sem.lstHPGL[j].DispLeft = sem.lstHPGL[j].DispTop = 0.0;
                                graphic = CreateRailGraphicsToCanvas(strip, sem.lstHPGL[j], anRoiCanvas); // Drawing Canvas에 ROI를 그린다.
                                if (graphic != null)
                                {
                                    graphic.MarkOBJID = j;
                                    for (int k = 0; k < model.Step; k++)
                                    {
                                        Strip s = RailMark.template.Strips[k];
                                        AddRailStepDummy(anRoiCanvas, graphic, sa, s, model, k);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Strip strip = RailMark.template.Strips[0];
                        if (RailMark.template.Section == eMarkSectionType.eSecTypeRail)
                        {
                            for (int j = 0; j < sem.lstHPGL.Count; j++)
                            {
                                //sem.lstHPGL[j].DispRight;// -= sem.lstHPGL[j].DispLeft;
                                //sem.lstHPGL[j].DispBottom;// -= sem.lstHPGL[j].DispTop;
                                sem.lstHPGL[j].DispLeft = sem.lstHPGL[j].DispTop = 0.0;
                                graphic = CreateRailGraphicsToCanvas(strip, sem.lstHPGL[j], anRoiCanvas); // Drawing Canvas에 ROI를 그린다.
                                if (graphic != null)
                                {
                                    graphic.MarkOBJID = j;
                                    AddRailDummy(anRoiCanvas, graphic, sa, strip, model);
                                }
                            }
                        }
                    }
                }
                #endregion Rail
                if (RejMark != null && (wc + cc) > 0)
                {
                    Sem sem = RejMark.template.Sems;
                    Strip strip = RejMark.template.Strips[0];
                    if (RejMark.template.Section == eMarkSectionType.eSecTypeNum)
                    {
                        for (int j = 0; j < sem.lstText.Count; j++)
                        {
                            graphic = CreateRejGraphicsToCanvas(strip, sem.lstText[j], anRoiCanvas, cl, wl, idl, model.StepPitch, logo); // Drawing Canvas에 ROI를 그린다.
                            if (graphic != null)
                            {
                                graphic.MarkOBJID = j;
                            }
                        }
                    }
                }

                if (RejMark != null && (idl) > 0)
                {
                    Sem sem = RejMark.template.Sems;
                    Strip strip = RejMark.template.Strips[0];
                    if (RejMark.template.Section == eMarkSectionType.eSecTypeNum)
                    {
                        for (int j = 0; j < sem.lstIDMark.Count; j++)
                        {
                            graphic = CreateRejGraphicsToCanvas(strip, sem.lstIDMark[j], anRoiCanvas, idl, model.StepPitch); // Drawing Canvas에 ROI를 그린다.
                            graphic.MarkOBJID = j;
                        }
                    }
                }
            }
            catch
            {
                anRoiCanvas.GraphicsList.Clear();
            }
        }

        public GraphicsBase CreateGuideGraphicsToCanvas(Strip strip, DrawingCanvas roiCanvas, int Type, double MarkCenterY, ModelMarkInfo model)
        {
            GraphicsBase graphic = null;
            Color graphicObjectColor = Colors.Yellow;
            double top = YGap;
            double left = 0;
            double bottom = 120.0 * 1000.0 / ResolutionY + YGap;// model.StepPitch * 1000.0 / ResolutionY + YGap; // 
            double right = 3000;
            double cpx = 1300;
            double Gap = model.StepPitch * 1000.0 / ResolutionY;
            cpx += (60.0 - MarkCenterY) / 2.0 * 1000.0 / ResolutionX;
            if (Type == 0)
            {
                for (int i = 1; i < model.Step; i++)
                {
                    GraphicsMarkGuide gm1 = new GraphicsMarkGuide(top + (Gap * i), bottom + (Gap * i), cpx, roiCanvas.LineWidth, Colors.Red, roiCanvas.ActualScale, i);
                    GraphicsBase g = gm1;
                    roiCanvas.GraphicsList.Add(g);
                    g.RefreshDrawing();
                    roiCanvas.StripRejectGuidePoint[model.Step - i] = new System.Windows.Point(cpx, top + (Gap * i));
                }
                GraphicsMarkGuide gm = new GraphicsMarkGuide(top, bottom, cpx, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, 0);
                graphic = gm;
                roiCanvas.StripGuidePoint = new System.Windows.Point(cpx, top);
                roiCanvas.StripRejectGuidePoint[0] = new System.Windows.Point(cpx, top);
            }
            if (Type == 1)
            {
                top = YGap + (strip.FirstX) * 1000.0 / ResolutionY;
                bottom = YGap + ((strip.FirstX) + model.UnitWidth) * 1000.0 / ResolutionY;
                right = cpx + (60.0 - strip.FirstY) * 1000.0 / ResolutionX;
                left = right - (model.UnitHeight) * 1000.0 / ResolutionX;
                GraphicsUnitGuide gu = new GraphicsUnitGuide(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale);
                graphic = gu;
                roiCanvas.UnitGuidePoint = new System.Windows.Point(right, top);
                graphic.MarkInfo = MarkingItemHelper.GetMarkingItem(strip.FirstX, strip.FirstY);
            }
            if (graphic != null)
            {
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }

            return null;
        }

        public GraphicsBase CreateUnitGraphicsToCanvas(Strip strip, HPGL hpgl, DrawingCanvas roiCanvas)
        {
            if (hpgl.UNIT < 0)
            {
                return null;
            }

            GraphicsBase graphic = null;
            Color graphicObjectColor = Colors.Yellow;
            double top = hpgl.DispLeft * 1000.0 / ResolutionY + roiCanvas.UnitGuidePoint.Y;
            double left = roiCanvas.UnitGuidePoint.X - hpgl.DispTop * 1000.0 / ResolutionX;
            double bottom = hpgl.DispRight * 1000.0 / ResolutionY + roiCanvas.UnitGuidePoint.Y;
            double right = roiCanvas.UnitGuidePoint.X - hpgl.DispBottom * 1000.0 / ResolutionX;
            bool white = true;
            bool dummy = false;
            string id = roiCanvas.GetMarkID();
            eMarkingType type = eMarkingType.eMarkingNone;
            if (hpgl.FileName.Contains("RU"))
            {
                GraphicsRectangleMark graphicsr = new GraphicsRectangleMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, white, dummy, id, GraphicsRegionType.MarkingUnit);
                graphic = graphicsr;
                type = eMarkingType.eMarkingUnitRect;

            }
            else if (hpgl.FileName.Contains("CU"))
            {
                double cpx = left + (right - left) / 2.0;
                double cpy = top + (bottom - top) / 2.0;
                GraphicsEllipseMark graphicse = new GraphicsEllipseMark(new Circle(new System.Windows.Point(cpx, cpy), (right - left) / 2.0), roiCanvas.LineWidth, roiCanvas.ActualScale, white, dummy, id, GraphicsRegionType.MarkingUnit);
                graphic = graphicse;
                type = eMarkingType.eMarkingUnitCircle;
            }
            else if (hpgl.FileName.Contains("TU"))
            {
                GraphicsTriangleMark graphicst = new GraphicsTriangleMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, white, dummy, id, (int)hpgl.RotateAngle, GraphicsRegionType.MarkingUnit);
                graphic = graphicst;
                type = eMarkingType.eMarkingUnitTri;
            }
            else if (hpgl.FileName.Contains("LU"))
            {
                GraphicsSpecialMark graphicss = new GraphicsSpecialMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, id, roiCanvas.ActualScale, white, dummy, 1, GraphicsRegionType.MarkingUnit);
                graphic = graphicss;
                type = eMarkingType.eMarkingUnitSpecial;
            }
            else if (hpgl.FileName.Contains("XU"))
            {
                GraphicsSpecialMark graphicss = new GraphicsSpecialMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, id, roiCanvas.ActualScale, white, dummy, 2, GraphicsRegionType.MarkingUnit);
                graphic = graphicss;
                type = eMarkingType.eMarkingUnitSpecial;
            }
            if (graphic != null)
            {
                graphic.Step = 0;
                graphic.UnitColumn = 0;
                graphic.UnitRow = 0;

                graphic.MarkInfo = MarkingItemHelper.GetMarkingItem(strip, hpgl, type);


                graphic.RegionType = GraphicsRegionType.MarkingUnit;
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }

        public GraphicsBase CreateRailGraphicsToCanvas(Strip strip, HPGL hpgl, DrawingCanvas roiCanvas)
        {
            if (hpgl.UNIT < 0)
            {
                return null;
            }

            GraphicsBase graphic = null;
            Color graphicObjectColor = Colors.Yellow;
            double top = (hpgl.DispLeft + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            double right = roiCanvas.StripGuidePoint.X + (60 - (strip.FirstY + hpgl.DispTop)) * 1000.0 / ResolutionX;
            double bottom = (hpgl.DispRight + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            double left = roiCanvas.StripGuidePoint.X + (60 - (strip.FirstY + hpgl.DispBottom)) * 1000.0 / ResolutionX;
            bool white = true;
            bool dummy = false;
            string id = roiCanvas.GetMarkID();
            eMarkingType type = eMarkingType.eMarkingNone;
            if (hpgl.FileName.Contains("RR"))
            {
                GraphicsRectangleMark graphicsr = new GraphicsRectangleMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, white, dummy, id, GraphicsRegionType.MarkingRail);
                graphic = graphicsr;
                type = eMarkingType.eMarkingRailRect;
            }
            else if (hpgl.FileName.Contains("CR"))
            {
                double cpx = left + (right - left) / 2.0;
                double cpy = top + (bottom - top) / 2.0;
                GraphicsEllipseMark graphicse = new GraphicsEllipseMark(new Circle(new System.Windows.Point(cpx, cpy), (right - left) / 2.0), roiCanvas.LineWidth, roiCanvas.ActualScale, white, dummy, id, GraphicsRegionType.MarkingRail);
                graphic = graphicse;
                type = eMarkingType.eMarkingRailCircle;
            }
            else if (hpgl.FileName.Contains("TR"))
            {
                GraphicsTriangleMark graphicst = new GraphicsTriangleMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, white, dummy, id, (int)hpgl.RotateAngle, GraphicsRegionType.MarkingRail);
                graphic = graphicst;
                type = eMarkingType.eMarkingRailTri;
            }
            else if (hpgl.FileName.Contains("LR"))
            {
                GraphicsSpecialMark graphicss = new GraphicsSpecialMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, id, roiCanvas.ActualScale, white, dummy, 1, GraphicsRegionType.MarkingRail);
                graphic = graphicss;
                type = eMarkingType.eMarkingUnitSpecial;
            }
            if (graphic != null)
            {
                graphic.Step = 0;
                graphic.UnitColumn = 0;
                graphic.UnitRow = 0;

                graphic.MarkInfo = MarkingItemHelper.GetMarkingItem(strip, hpgl, type);


                graphic.RegionType = GraphicsRegionType.MarkingRail;
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }

        public GraphicsBase CreateRejGraphicsToCanvas(Strip strip, HPGL hpgl, DrawingCanvas roiCanvas)
        {
            if (hpgl.UNIT < 0)
            {
                return null;
            }

            GraphicsBase graphic = null;
            Color graphicObjectColor = Colors.Yellow;
            double top = hpgl.DispLeft * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            double right = roiCanvas.StripGuidePoint.X + (60 - hpgl.DispTop) * 1000.0 / ResolutionX;
            double bottom = hpgl.DispRight * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            double left = roiCanvas.StripGuidePoint.X + (60 - hpgl.DispBottom) * 1000.0 / ResolutionX;
            bool white = true;
            bool dummy = false;
            string id = roiCanvas.GetMarkID();
            eMarkingType type = eMarkingType.eMarkingNone;
            if (hpgl.FileName.Contains("BOX"))
            {
                GraphicsRectangleMark graphicsr = new GraphicsRectangleMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, white, dummy, id, GraphicsRegionType.MarkingRail);
                graphic = graphicsr;
                type = eMarkingType.eMarkingRailRect;
            }
            else if (hpgl.FileName.Contains("0.6 line 1um-easymarker"))
            {
                double cpx = left + (right - left) / 2.0;
                double cpy = top + (bottom - top) / 2.0;
                GraphicsEllipseMark graphicse = new GraphicsEllipseMark(new Circle(new System.Windows.Point(cpx, cpy), (right - left) / 2.0), roiCanvas.LineWidth, roiCanvas.ActualScale, white, dummy, id, GraphicsRegionType.MarkingRail);
                graphic = graphicse;
                type = eMarkingType.eMarkingRailCircle;
            }
            else if (hpgl.FileName.Contains("0.6 line 1um-easymarker"))
            {
                GraphicsTriangleMark graphicst = new GraphicsTriangleMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, white, dummy, id, (int)hpgl.RotateAngle, GraphicsRegionType.MarkingRail);
                graphic = graphicst;
                type = eMarkingType.eMarkingRailTri;
            }
            else if (hpgl.FileName.Contains("0.6 line 1um-easymarker"))
            {
                GraphicsSpecialMark graphicss = new GraphicsSpecialMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, id, roiCanvas.ActualScale, white, dummy, (int)hpgl.RotateAngle, GraphicsRegionType.MarkingRail);
                graphic = graphicss;
                type = eMarkingType.eMarkingUnitSpecial;
            }
            graphic.Step = 0;
            graphic.UnitColumn = 0;
            graphic.UnitRow = 0;

            graphic.MarkInfo = MarkingItemHelper.GetMarkingItem(strip, hpgl, type);

            if (graphic != null)
            {
                graphic.RegionType = GraphicsRegionType.MarkingRail;
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }

        public GraphicsBase CreateRejGraphicsToCanvas(Strip strip, IDMARK idmark, DrawingCanvas roiCanvas, int IDLoc, double pitch)
        {
            if (idmark.UNIT < 0)
            {
                return null;
            }

            GraphicsBase graphic = null;
            Color graphicObjectColor = Colors.Yellow;
            double p = pitch * 2.0;
            double top = 0;
            double bottom = 0;
            if (idmark.ReferenceNumber1 == 2 && IDLoc > 0)
            {
                top = (idmark.DispLeft + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripRejectGuidePoint[IDLoc - 1].Y;
                bottom = (idmark.DispRight + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripRejectGuidePoint[IDLoc - 1].Y;
            }

            double right = roiCanvas.StripRejectGuidePoint[IDLoc - 1].X + (60 - idmark.DispTop + strip.FirstY) * 1000.0 / ResolutionX;
            double left = roiCanvas.StripRejectGuidePoint[IDLoc - 1].X + (60 - idmark.DispBottom + strip.FirstY) * 1000.0 / ResolutionX;
            string id = roiCanvas.GetMarkID();
            eMarkingType type = eMarkingType.eMarkingNone;
            if (idmark.ReferenceNumber1 == 2 && IDLoc > 0)
            {
                GraphicsTBD graphicw = new GraphicsTBD(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, idmark.RotateAngle, id, GraphicsRegionType.MarkingReject);
                graphic = graphicw;
                type = eMarkingType.eMarkingTBD;

            }

            graphic.Step = 0;
            graphic.UnitColumn = 0;
            graphic.UnitRow = 0;

            graphic.MarkInfo = MarkingItemHelper.GetMarkingItem(strip, idmark, type);
            graphic.MarkInfo.Resolution = ResolutionX;
            if (graphic != null)
            {

                if (type == eMarkingType.eMarkingTBD)
                {
                    TBDProperty idp = graphic.MarkInfo.MarkInfo as TBDProperty;
                    GraphicsTBD g = ((GraphicsTBD)graphic);
                    if (idmark.RotateAngle == 0 || idmark.RotateAngle == 180)
                    {
                        g.Top = (idp.Left + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripRejectGuidePoint[IDLoc - 1].Y;
                        g.Left = roiCanvas.StripRejectGuidePoint[IDLoc - 1].X + (60.0 - (idp.Top + idp.Height) - strip.FirstY) * 1000.0 / ResolutionX;
                        g.Bottom = g.Top + (idp.Width * 1000.0 / ResolutionY);
                        g.Right = g.Left + (idp.Height * 1000.0 / ResolutionX);
                    }
                    else
                    {
                        g.Top = (idp.Left + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripRejectGuidePoint[IDLoc - 1].Y;
                        g.Left = roiCanvas.StripRejectGuidePoint[IDLoc - 1].X + (60.0 - (idp.Top + idp.Width) - strip.FirstY) * 1000.0 / ResolutionX;
                        g.Bottom = g.Top + (idp.Height * 1000.0 / ResolutionY);
                        g.Right = g.Left + (idp.Width * 1000.0 / ResolutionX);
                    }
                }

                graphic.RegionType = GraphicsRegionType.MarkingReject;
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }

        public GraphicsBase CreateRejGraphicsToCanvas(Strip strip, TEXT text, DrawingCanvas roiCanvas, int CountLoc, int WeekLoc, int IDLoc, double pitch, MarkLogo logo)
        {
            if (text.UNIT < 0)
            {
                return null;
            }

            GraphicsBase graphic = null;
            Color graphicObjectColor = Colors.Yellow;
            //double p = pitch * 2.0;
            double top = 0;
            double bottom = 0;
            //if (text.ReferenceNumber1 >= 4)
            //{
            //    if (WeekLoc == 2)
            //    {
            //        top = (text.DispLeft + p + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //        bottom = (text.DispRight + p + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //    }
            //    else
            //    {
            //        top = (text.DispLeft + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //        bottom = (text.DispRight + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //    }
            //}
            //else if (text.ReferenceNumber1 == 2 && IDLoc > 0)
            //{
            //    if (IDLoc == 2)
            //    {
            //        top = (text.DispLeft + p + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //        bottom = (text.DispRight + p + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //    }
            //    else
            //    {
            //        top = (text.DispLeft + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //        bottom = (text.DispRight + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //    }
            //
            //}
            //else if (text.ReferenceNumber1 < 4)
            //{
            //    if (CountLoc == 2)
            //    {
            //        top = (text.DispLeft + p + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //        bottom = (text.DispRight + p + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //    }
            //    else
            //    {
            //        top = (text.DispLeft + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //        bottom = (text.DispRight + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripGuidePoint.Y;
            //    }
            //
            //}


            double right = roiCanvas.StripGuidePoint.X + (60 - text.DispTop + strip.FirstY) * 1000.0 / ResolutionX;
            double left = roiCanvas.StripGuidePoint.X + (60 - text.DispBottom + strip.FirstY) * 1000.0 / ResolutionX;
            string id = roiCanvas.GetMarkID();
            eMarkingType type = eMarkingType.eMarkingNone;
            if (text.ReferenceNumber1 >= 4)
            {
                GraphicsWeekMark graphicw = new GraphicsWeekMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, text.RotateAngle, id, GraphicsRegionType.MarkingReject);
                graphic = graphicw;
                type = eMarkingType.eMarkingWeek;

            }
            else if (text.ReferenceNumber1 == 2 && IDLoc > 0)
            {
                GraphicsIDMark graphicn = new GraphicsIDMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, text.RotateAngle, id, GraphicsRegionType.MarkingReject);
                graphic = graphicn;
                type = eMarkingType.eMarkingIDMark;
            }
            else if (text.ReferenceNumber1 < 4)
            {
                GraphicsNumberMark graphicn = new GraphicsNumberMark(left, top, right, bottom, roiCanvas.LineWidth, graphicObjectColor, roiCanvas.ActualScale, text.RotateAngle, id, GraphicsRegionType.MarkingReject, logo);
                graphic = graphicn;
                type = eMarkingType.eMarkingNumber;
            }

            graphic.Step = 0;
            graphic.UnitColumn = 0;
            graphic.UnitRow = 0;

            graphic.MarkInfo = MarkingItemHelper.GetMarkingItem(strip, text, type, IDLoc, CountLoc, WeekLoc);
            graphic.MarkInfo.Resolution = ResolutionX;
            if (graphic != null)
            {
                if (type == eMarkingType.eMarkingNumber)
                {
                    NumberProperty num = graphic.MarkInfo.MarkInfo as NumberProperty;
                    GraphicsNumberMark g = ((GraphicsNumberMark)graphic);
                    if (text.RotateAngle == 0 || text.RotateAngle == 180)
                    {
                        g.Top = (num.Left + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripRejectGuidePoint[CountLoc].Y;
                        g.Left = roiCanvas.StripRejectGuidePoint[CountLoc].X + (60.0 - (num.Top + num.Height) - strip.FirstY) * 1000.0 / ResolutionX;
                        g.Bottom = g.Top + (num.Width * 1000.0 / ResolutionY);
                        g.Right = g.Left + (num.Height * 1000.0 / ResolutionX);
                    }
                    else
                    {
                        g.Top = (num.Left + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripRejectGuidePoint[CountLoc].Y;
                        g.Left = roiCanvas.StripRejectGuidePoint[CountLoc].X + (60.0 - (num.Top + num.Width) - strip.FirstY) * 1000.0 / ResolutionX;
                        g.Bottom = g.Top + (num.Height * 1000.0 / ResolutionY);
                        g.Right = g.Left + (num.Width * 1000.0 / ResolutionX);
                    }
                }
                if (type == eMarkingType.eMarkingWeek)
                {
                    WeekProperty num = graphic.MarkInfo.MarkInfo as WeekProperty;
                    GraphicsWeekMark g = ((GraphicsWeekMark)graphic);
                    if (text.RotateAngle == 0 || text.RotateAngle == 180)
                    {
                        g.Top = (num.Left + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripRejectGuidePoint[WeekLoc].Y;
                        g.Left = roiCanvas.StripRejectGuidePoint[WeekLoc].X + (60.0 - (num.Top + num.Height) - strip.FirstY) * 1000.0 / ResolutionX;
                        g.Bottom = g.Top + (num.Width * 1000.0 / ResolutionY);
                        g.Right = g.Left + (num.Height * 1000.0 / ResolutionX);
                    }
                    else
                    {                        
                        g.Top = (num.Left + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripRejectGuidePoint[WeekLoc].Y;
                        g.Left = roiCanvas.StripRejectGuidePoint[WeekLoc].X + (60.0 - (num.Top + num.Width) - strip.FirstY) * 1000.0 / ResolutionX;
                        g.Bottom = g.Top + (num.Height * 1000.0 / ResolutionY);
                        g.Right = g.Left + (num.Width * 1000.0 / ResolutionX);
                    }
                }

                if (type == eMarkingType.eMarkingIDMark)
                {
                    IDMarkProperty idp = graphic.MarkInfo.MarkInfo as IDMarkProperty;
                    GraphicsIDMark g = ((GraphicsIDMark)graphic);
                    if (text.RotateAngle == 0 || text.RotateAngle == 180)
                    {
                        g.Top = (idp.Left + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripRejectGuidePoint[IDLoc - 1].Y;
                        g.Left = roiCanvas.StripRejectGuidePoint[IDLoc - 1].X + (60.0 - (idp.Top + idp.Height) - strip.FirstY) * 1000.0 / ResolutionX;
                        g.Bottom = g.Top + (idp.Width * 1000.0 / ResolutionY);
                        g.Right = g.Left + (idp.Height * 1000.0 / ResolutionX);
                    }
                    else
                    {
                        g.Top = (idp.Left + strip.FirstX) * 1000.0 / ResolutionY + roiCanvas.StripRejectGuidePoint[IDLoc - 1].Y;
                        g.Left = roiCanvas.StripRejectGuidePoint[IDLoc - 1].X + (60.0 - (idp.Top + idp.Width) - strip.FirstY) * 1000.0 / ResolutionX;
                        g.Bottom = g.Top + (idp.Height * 1000.0 / ResolutionY);
                        g.Right = g.Left + (idp.Width * 1000.0 / ResolutionX);
                    }
                }

                graphic.RegionType = GraphicsRegionType.MarkingReject;
                roiCanvas.GraphicsList.Add(graphic);
                graphic.RefreshDrawing();
                return graphic;
            }
            return null;
        }
        #endregion
    }

    public class Mark
    {
        public MarkFile UnitMark = new MarkFile();
        public MarkFile RailMark = new MarkFile();
        public MarkFile RejMark = new MarkFile();
        //public List<MarkFile> lstMark = new List<MarkFile>();
        public PenParam[] pen = new PenParam[7];
        public string m_currpath;
        public string m_markIP;
        public string m_model;
        public int Step;
        public bool RailIrr;
        public bool Loaded;
        public Point UnitFirst = new Point(0, 0);
        public Point RejectFirst = new Point(0, 0);
        public Point RailFirst = new Point(0, 0);
        public static string m_sDrive = "e";////회사 보안 정책으로 공유 폴더 사용 불가하여 디버그시 로컬경로 변경 추가
        public static bool m_bDebug = false;
        #region Construtor
        protected void LoadMark(Point pos, Point upos)
        {
            Loaded = false;
            //m_model = model;
            // surface = surfaceType;
            //if (surfaceType == Surface.TOP) m_Surface = "_TOP"; else m_Surface= "_BOT";
            //m_currpath = "\\\\" + markIP + "\\Mime\\mrk\\" + model + m_Surface + ".mrk";
            //m_markIP = markIP;
            if (File.Exists(m_currpath))
            {
                UnitMark = null;
                RailMark = null;
                RejMark = null;
                string[] lstStr = File.ReadAllLines(m_currpath);
                for (int i = 0; i < lstStr.Length; i++)
                {
                    string[] slt = lstStr[i].Split('=');
                    if (slt.Length < 1) continue;
                    if (slt[0] == "TemplateNum")
                    {
                        MarkFile mf = new MarkFile();
                        mf.Num = Convert.ToInt32(slt[1]);
                        slt = lstStr[i + 1].Split('=');
                        if (slt[0] == "TemplateName")
                        {
                            mf.Name = slt[1];
                        }
                        if (!mf.LoadTemplate(m_markIP, Step, RailIrr, pos, upos))
                        {
                            MessageBox.Show(mf.Name + "파일을 로드 할 수 없습니다.");
                        }
                        if (mf.template.Section == eMarkSectionType.eSecTypeUnit)
                        {
                            UnitMark = mf;
                            UnitFirst = new Point(mf.template.Strips[0].FirstX, mf.template.Strips[0].FirstY);
                        }
                        else if (mf.template.Section == eMarkSectionType.eSecTypeRail)
                        {
                            RailMark = mf;
                            //for (int k = 0; k < RailMark.template.Strips.Count; k++)
                            RailFirst = new Point(mf.template.Strips[0].FirstX, mf.template.Strips[0].FirstY);
                        }
                        if (mf.template.Section == eMarkSectionType.eSecTypeNum)
                        {
                            RejMark = mf;
                            RejectFirst = new Point(mf.template.Strips[0].FirstX, mf.template.Strips[0].FirstY);
                        }
                    }
                    if (slt[0] == "[PARAMETER]")
                    {
                        for (int j = i + 1; j < lstStr.Length; j++)
                        {
                            slt = lstStr[j].Split('=');
                            if (slt.Length < 1) continue;
                            try
                            {
                                if (slt[0].Contains("Parameter"))
                                {
                                    int no = Convert.ToInt32(slt[0].Substring(9, 1));
                                    pen[no - 1] = new PenParam(Mark.ConvertNetPath(slt[1], m_markIP));
                                    if (!pen[no - 1].Loaded)
                                    {
                                        MessageBox.Show(slt[1] + "파일을 로드 할 수 없습니다.");
                                    }
                                }
                            }
                            catch { }
                            if (slt[0] == "[END_PARAMETER]") break;
                        }
                        break;
                    }
                }
                for (int i = 0; i < 7; i++)
                {
                    if (pen[i] == null) pen[i] = new PenParam();
                }
                Loaded = true;
            }
        }
        #endregion

        #region Save Inform
        protected bool Save(ModelMarkInfo newmodel)
        {
            if (File.Exists(m_currpath))
            {
                try
                {
                    File.Delete(m_currpath);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(m_currpath); fs.Close();
            }
            catch { return false; }
            #region MarkFile
            try
            {
                List<string> lst = new List<string>();
                lst.Add("// MiME Mark Project File Ver0.2");
                lst.Add("");
                if (UnitMark != null)
                {
                    lst.Add("TemplateNum=0");
                    lst.Add("TemplateName=" + UnitMark.Name);
                    lst.Add("APC_AutoExecuteWhenLoad=0");
                    lst.Add("APC_OpenTypeForAutoAPC=0");
                    lst.Add("APC_AutoSaveParam=0");
                    lst.Add("CustomInt1=0");
                    lst.Add("CustomInt2=");
                    lst.Add("CustomStr1=");
                    lst.Add("CustomStr2=");
                }
                if (RejMark != null)
                {
                    lst.Add("TemplateNum=1");
                    lst.Add("TemplateName=" + RejMark.Name);
                    lst.Add("APC_AutoExecuteWhenLoad=0");
                    lst.Add("APC_OpenTypeForAutoAPC=0");
                    lst.Add("APC_AutoSaveParam=0");
                    lst.Add("CustomInt1=0");
                    lst.Add("CustomInt2=");
                    lst.Add("CustomStr1=");
                    lst.Add("CustomStr2=");
                }
                if (RailMark != null)
                {
                    lst.Add("TemplateNum=3");
                    lst.Add("TemplateName=" + RailMark.Name);
                    lst.Add("APC_AutoExecuteWhenLoad=0");
                    lst.Add("APC_OpenTypeForAutoAPC=0");
                    lst.Add("APC_AutoSaveParam=0");
                    lst.Add("CustomInt1=0");
                    lst.Add("CustomInt2=");
                    lst.Add("CustomStr1=");
                    lst.Add("CustomStr2=");
                }
                lst.Add("[PARAMETER]");
                for (int i = 0; i < 7; i++)
                {
                    lst.Add("Parameter" + (i + 1).ToString() + "=" + "c:\\Mime\\Pen\\" + m_model + "_" + (i + 1).ToString() + ".prm");
                }
                lst.Add("[END_PARAMETER]");
                File.WriteAllLines(m_currpath, lst);
            }
            catch { return false; }

            if (this.RailIrr)
            {
                for (int n = 1; n < this.Step; n++)
                {
                    string path = m_currpath.Replace("1STEP", (n + 1).ToString() + "STEP");
                    if (File.Exists(path))
                    {
                        try
                        {
                            File.Delete(path);
                        }
                        catch
                        { return false; }
                    }
                    try
                    {
                        FileStream fs = File.Create(path); fs.Close();
                    }
                    catch { return false; }

                    try
                    {
                        List<string> lst = new List<string>();
                        lst.Add("// MiME Mark Project File Ver0.2");
                        lst.Add("");
                        if (UnitMark != null)
                        {
                            lst.Add("TemplateNum=0");
                            lst.Add("TemplateName=" + UnitMark.Name);
                            lst.Add("APC_AutoExecuteWhenLoad=0");
                            lst.Add("APC_OpenTypeForAutoAPC=0");
                            lst.Add("APC_AutoSaveParam=0");
                            lst.Add("CustomInt1=0");
                            lst.Add("CustomInt2=");
                            lst.Add("CustomStr1=");
                            lst.Add("CustomStr2=");
                        }
                        if (RejMark != null)
                        {
                            lst.Add("TemplateNum=1");
                            lst.Add("TemplateName=" + RejMark.Name);
                            lst.Add("APC_AutoExecuteWhenLoad=0");
                            lst.Add("APC_OpenTypeForAutoAPC=0");
                            lst.Add("APC_AutoSaveParam=0");
                            lst.Add("CustomInt1=0");
                            lst.Add("CustomInt2=");
                            lst.Add("CustomStr1=");
                            lst.Add("CustomStr2=");
                        }
                        if (RailMark != null)
                        {
                            lst.Add("TemplateNum=3");
                            lst.Add("TemplateName=" + (RailMark.Name.Replace("1STEP", (n + 1).ToString() + "STEP")));
                            lst.Add("APC_AutoExecuteWhenLoad=0");
                            lst.Add("APC_OpenTypeForAutoAPC=0");
                            lst.Add("APC_AutoSaveParam=0");
                            lst.Add("CustomInt1=0");
                            lst.Add("CustomInt2=");
                            lst.Add("CustomStr1=");
                            lst.Add("CustomStr2=");
                        }
                        lst.Add("[PARAMETER]");
                        for (int i = 0; i < 7; i++)
                        {
                            lst.Add("Parameter" + (i + 1).ToString() + "=" + "c:\\Mime\\Pen\\" + m_model + "_" + (i + 1).ToString() + ".prm");
                        }
                        lst.Add("[END_PARAMETER]");
                        File.WriteAllLines(path, lst);
                    }
                    catch { return false; }
                }
            }
            #endregion
            if (UnitMark != null) UnitMark.template.Save(newmodel);
            if (RailMark != null) RailMark.template.Save(newmodel);
            if (RejMark != null) RejMark.template.Save(newmodel);
            return true;
        }


        public bool SavePenParam()
        {
            for (int i = 0; i < 7; i++)
            {
                if (!this.pen[i].Save())
                {
                    MessageBox.Show("Pen Paramater 를 저장 할 수 없습니다.");
                }
            }
            return false;
        }
        #endregion
        public static string ConvertNetPath(string path, string IP)
        {
            if (path.Length < 5) return "";
            string tmppath = path.Remove(0, 2);
            tmppath = "\\\\" + IP + tmppath;
            return tmppath;
        }
        ////회사 보안 정책으로 공유 폴더 사용 불가. TCP/IP 주소를 로컬로 변경시키는 함수 추가
        public static string ConvertNetTolocalPath(string path, string Drive)
        {            
            if (path.Length < 3) return "";
            path = path.Replace('/', '\\');
            string[] parts = path.Split('\\');
            string Path = Drive + ":\\";
            for (int i = 0; i < parts.Length - 3; i++)
            {       
                Path += parts[i + 3];
                if (i < parts.Length - 4)
                    Path += "\\";
            }
            return Path;
        }
    }

    public class MarkFile
    {
        private int num;
        private string name;
        private int paramID;
        private string paramPath;
        public int APC_AutoExecuteWhenLoad = 0;
        public int APC_OpenTypeForAutoAPC = 0;
        public int APC_AutoSaveParam = 0;
        public int CustomInt1 = 0;
        public int CustomInt2 = 0;
        public string CustomStr1 = "";
        public string CustomStr2 = "";

        public Template template = new Template();
        public MarkFile()
        {

        }
        public MarkFile(int anNum, string astrPath, int anParamid, string astrParampath)
        {
            this.num = anNum;
            this.name = astrPath;
            this.paramID = anParamid;
            this.paramPath = astrParampath;
        }
        public bool LoadTemplate(string IP, int nstep, bool brailirr, Point pos, Point upos)
        {
            template = new Template(name, IP, nstep, brailirr, pos, upos);
            return template.Loaded;
        }
        public bool LoadParam(string IP)
        {

            return false;
        }
        #region properties
        public int Num
        {
            get { return num; }
            set { num = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int ParamID
        {
            get { return paramID; }
            set { paramID = value; }
        }
        public string ParamPath
        {
            get { return paramPath; }
            set { paramPath = value; }
        }
        #endregion
    }

    public class Template
    {
        public List<string> StripFile = new List<string>();
        public string SemFile;
        public int NumberOfSem;
        public int SemNumber;
        public List<Strip> Strips = new List<Strip>();
        public Sem Sems = new Sem();
        public bool Loaded = false;
        public eMarkSectionType Section;
        public bool IsUnitMark = false;
        private string Path;
        public Template()
        {
        }
        public Template(string strpath, string IP, int nstep, bool railirr, Point pos, Point upos)
        {
            Loaded = true;

            string path = Mark.ConvertNetPath(strpath, IP);
            if (Mark.m_bDebug) path = Mark.ConvertNetTolocalPath(path, Mark.m_sDrive);
            Path = path;
            if (File.Exists(path))
            {
                try
                {
                    if (path.Contains("_Unit"))
                    {
                        Section = eMarkSectionType.eSecTypeUnit;
                    }
                    else if (path.Contains("_Rail"))
                        Section = eMarkSectionType.eSecTypeRail;
                    else if (path.Contains("_Reject"))
                        Section = eMarkSectionType.eSecTypeNum;
                    else Section = eMarkSectionType.eSecTypeNone;

                    string[] lstStr = File.ReadAllLines(path);
                    Strips.Clear();
                    StripFile.Clear();
                    foreach (string str in lstStr)
                    {
                        string[] slt = str.Split('=');
                        if (slt.Length < 2) continue;
                        if (slt[0] == "StripFile")
                        {
                            StripFile.Add(slt[1]);
                            Strip s = new Strip();
                            string sPath = Mark.ConvertNetPath(StripFile[0], IP);
                            if (Mark.m_bDebug)sPath = Mark.ConvertNetTolocalPath(sPath, Mark.m_sDrive);
                            if (!s.Load(sPath, Section, pos, upos))
                            {
                                MessageBox.Show(StripFile[0] + "파일을 로드 할 수 없습니다.");
                            }
                            Strips.Add(s);
                            if (Section == eMarkSectionType.eSecTypeRail)
                            {
                                if (railirr)
                                {
                                    for (int n = 1; n < nstep; n++)
                                    {
                                        StripFile.Add(slt[1].Replace("1STEP", (n + 1).ToString() + "STEP"));
                                        Strip ss = new Strip();
                                        sPath = Mark.ConvertNetPath(StripFile[0], IP);
                                        if (Mark.m_bDebug) sPath = Mark.ConvertNetTolocalPath(sPath, Mark.m_sDrive);
                                        if (!ss.Load(Mark.ConvertNetPath(StripFile[StripFile.Count - 1], IP), Section, pos, upos))
                                        {
                                            MessageBox.Show(StripFile[StripFile.Count - 1] + "파일을 로드 할 수 없습니다.");
                                        }
                                        Strips.Add(ss);
                                    }
                                }
                            }
                        }
                        else if (slt[0] == "SemFile")
                        {
                            SemFile = slt[1];
                            string sPath = Mark.ConvertNetPath(SemFile, IP);
                            if (Mark.m_bDebug) sPath = Mark.ConvertNetTolocalPath(sPath, Mark.m_sDrive);
                            if (!Sems.Load(sPath))
                            {
                                MessageBox.Show(SemFile + "파일을 로드 할 수 없습니다.");
                            }
                        }
                        else if (slt[0] == "NumberOfSem") NumberOfSem = Convert.ToInt32(slt[1]);
                        else if (slt[0] == "SemNumber") SemNumber = Convert.ToInt32(slt[1]);
                    }
                }
                catch
                {
                    Loaded = false;
                    return;
                }
            }
            else Loaded = false;
        }
        public Template(string astrStrip, string astrSem, int anNSem, int anSem)
        {
            // StripFile = astrStrip;
            SemFile = astrSem;
            NumberOfSem = anNSem;
            SemNumber = anSem;
        }

        public bool Save(ModelMarkInfo newmodel)
        {
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(Path); fs.Close();
            }
            catch { return false; }
            try
            {
                List<string> lst = new List<string>();
                lst.Add("// MiME Template File Ver0.2");
                lst.Add("");
                lst.Add("StripFile=" + StripFile[0]);
                lst.Add("NumberOfSem=" + NumberOfSem.ToString());
                lst.Add("SemNumber=" + SemNumber.ToString());
                lst.Add("SemFile=" + SemFile);
                File.WriteAllLines(Path, lst);
            }
            catch
            { return false; }

            if (StripFile.Count > 1)
            {
                for (int n = 1; n < StripFile.Count; n++)
                {
                    string path = Path.Replace("1STEP", (n + 1).ToString() + "STEP");
                    if (File.Exists(path))
                    {
                        try
                        {
                            File.Delete(path);
                        }
                        catch
                        { return false; }
                    }
                    try
                    {
                        FileStream fs = File.Create(path); fs.Close();
                    }
                    catch { return false; }
                    try
                    {
                        List<string> lst = new List<string>();
                        lst.Add("// MiME Template File Ver0.2");
                        lst.Add("");
                        lst.Add("StripFile=" + StripFile[n]);
                        lst.Add("NumberOfSem=" + NumberOfSem.ToString());
                        lst.Add("SemNumber=" + SemNumber.ToString());
                        lst.Add("SemFile=" + SemFile);
                        File.WriteAllLines(path, lst);
                    }
                    catch
                    { return false; }
                }
            }

            for (int i = 0; i < Strips.Count; i++)
            {
                if (i > 0)
                {
                    Strips[i].m_tmpFirstX = Strips[0].FirstX;
                    Strips[i].m_tmpFirstY = Strips[0].FirstY;
                }
                Strips[i].Save(newmodel, i);
            }
            Sems.Save();
            return true;
        }
    }

    public class Strip
    {
        public string Version;
        public int StripX;
        public int StripY;
        public double StripSizeX;
        public double StripSizeY;
        public int StripStyle = 1;
        public int MarkingSide = 0;
        public double XPitch;
        public double YPitch;
        public string StripName;
        public int IDPosition = 0;
        public int LeadNum;
        public double UnitWidth;
        public double UnitHeight;
        public double IDRadius;
        public int IDPosition_Vision = 0;
        public int VisionMfc = 1000;
        #region Special Pitch
        public int[] SpecialPitchX = new int[6];
        public int[] SpecialPitchY = new int[6];
        public double[] SpecialPitch = new double[6];
        public int[] SpecialApply = new int[6];
        public int[] SpecialDir = new int[6];
        public int[] SpecialDX = new int[6];
        public int[] SpecialDY = new int[6];
        public int[] SpecialDPos = new int[6];
        public int[] SpecialDApply = new int[6];
        public int[] SpecialDDir = new int[6];
        public double DFOffset = 0.0;
        public int DFDirection = 0;
        #endregion
        #region Marking Field
        public double FirstX = 24.620000;
        public double FirstY = 90.830000;
        public double SecondX = 0.000000;
        public double SecondY = 0.000000;
        public double ThirdX = 0.000000;
        public double ThirdY = 0.000000;
        public double UpperMasterX = 0.000000;
        public double UpperMasterY = 0.000000;
        public double UpperSlaveX = 0.000000;
        public double UpperSlaveY = 0.000000;
        public double LowerMasterX = 0.000000;
        public double LowerMasterY = 0.000000;
        public double LowerSlaveX = 0.000000;
        public double LowerSlaveY = 0.000000;
        public int MarkingPos = 1;
        public double RotateMarkingField = 0.0000;
        public double LowerRotateMarkingField = 0.000000;
        #endregion
        public string UnitAttribute;
        public string SEMType;
        public Point[] OneChipOffset;

        #region BiV data
        public double lXAbsolute = 0.0000000;
        public double lYAbsolute = 0.0000000;
        public int XPixel = 640;
        public int YPixel = 480;
        public double lXFoV = 0.0000000;
        public double lYFoV = 0.0000000;
        public double lXPtoF = 0.0000000;
        public double lYPtoF = 0.0000000;
        #endregion

        #region [Manual ID]
        public int UseFirstID = 0;
        public double ID1XPos = 0.0000000;
        public double ID1YPos = 0.0000000;
        public double ID1Diameter = 0.0000000;
        public int UseSecondID = 0;
        public double ID2XPos = 0.0000000;
        public double ID2YPos = 0.0000000;
        public double ID2Diameter = 0.0000000;
        public int UseThirdID = 0;
        public double ID3XPos = 0.0000000;
        public double ID3YPos = 0.0000000;
        public double ID3Diameter = 0.0000000;
        public int UseFourthID = 0;
        public double ID4XPos = 0.0000000;
        public double ID4YPos = 0.0000000;
        public double ID4Diameter = 0.0000000;
        #endregion

        #region [Marking Order]
        //m_iMarkStartPos : 0 = TL, 1 = TR, 2 = BL, 3 = BR
        public int MarkStartPos = 0;
        //m_iMarkMajor : 0 = Row Major, 1: Column Major
        public int MarkMajor = 0;
        //m_iMarkType : 0 = S(Zigzag) Type, 1: 11(One direction) Type
        public int MarkType = 0;
        #endregion

        #region [Custom data]
        // m_nGoldPointType : Substrate Gold Point Type Section For Motech Handler
        public int GoldPointType = -1;
        // m_dMarkingPositionX : Marking Position for RobotHIF
        public double MarkingPositionX = 0.0000000;
        public double MarkingPositionY = 0.0000000;
        #endregion

        private string Path;
        eMarkSectionType m_Section;
        public double m_tmpFirstX;
        public double m_tmpFirstY;
        public bool Load(string path, eMarkSectionType section, Point anpos, Point anupos)
        {
            m_Section = section;
            if (File.Exists(path))
            {
                Path = path;
                string[] str = File.ReadAllLines(path);
                int pos = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    string[] slt = str[i].Split('=');
                    if (slt.Length > 0)
                    {
                        if (slt[0] == "[Special Strip Infomation]")
                        {
                            pos = i;
                            break;
                        }
                    }
                    if (slt.Length == 2)
                    {
                        switch (slt[0])
                        {
                            case "Version": Version = slt[1]; break;
                            case "m_iStripX": StripX = Convert.ToInt32(slt[1]); break;
                            case "m_iStripY": StripY = Convert.ToInt32(slt[1]); break;
                            case "m_dStripSizeX": StripSizeX = Convert.ToDouble(slt[1]); break;
                            case "m_dStripSizeY": StripSizeY = Convert.ToDouble(slt[1]); break;
                            case "m_iStripStyle": StripStyle = Convert.ToInt32(slt[1]); break;
                            case "m_iMarkingSide": MarkingSide = Convert.ToInt32(slt[1]); break;
                            case "m_dXPitch": XPitch = Convert.ToDouble(slt[1]); break;
                            case "m_dYPitch": YPitch = Convert.ToDouble(slt[1]); break;
                            case "m_sStripName": StripName = slt[1]; break;

                            case "m_iIDPosition": IDPosition = Convert.ToInt32(slt[1]); break;
                            case "m_iLeadNum": LeadNum = Convert.ToInt32(slt[1]); break;
                            case "m_dUnitWidth": UnitWidth = Convert.ToDouble(slt[1]); break;
                            case "m_dUnitHeight": UnitHeight = Convert.ToDouble(slt[1]); break;
                            case "m_iIDPosition_Vision": IDPosition_Vision = Convert.ToInt32(slt[1]); break;
                            case "m_unVisionMagnification": VisionMfc = Convert.ToInt32(slt[1]); break;
                        }
                        OneChipOffset = new Point[StripX * StripY];
                    }
                }
                if (section == eMarkSectionType.eSecTypeRail)
                {
                    XPitch = 0;
                    YPitch = 0;
                    UnitWidth = 0.1;
                    UnitHeight = 0.1;
                }
                if (pos > 0)
                {
                    int npos = 0;
                    #region
                    for (int i = pos + 1; i < str.Length; i++)
                    {
                        string[] slt = str[i].Split('=');
                        if (slt.Length == 1)
                        {
                            if (slt[0] == "[Marking Field Setting]")
                            {
                                npos = i;
                                break;
                            }
                        }
                        if (slt.Length == 2)
                        {
                            if (slt[0] == "m_dIDFOffset") DFOffset = Convert.ToDouble(slt[1]);
                            else if (slt[0] == "m_iIDFDirection") DFDirection = Convert.ToInt32(slt[1]);
                            else
                            {
                                string s = slt[0].Substring(0, slt[0].Length - 3);
                                int l = s.Length + 1;
                                switch (s)
                                {
                                    case "ms_iSpecialPitchX":
                                        try
                                        {
                                            SpecialPitchX[Convert.ToInt32(slt[0].Substring(l, 1))] = Convert.ToInt32(slt[1]);
                                        }
                                        catch { }
                                        break;
                                    case "ms_iSpecialPitchY":
                                        try
                                        {
                                            SpecialPitchY[Convert.ToInt32(slt[0].Substring(l, 1))] = Convert.ToInt32(slt[1]);
                                        }
                                        catch { }
                                        break;
                                    case "ms_dSpecialPitch":
                                        try
                                        {
                                            SpecialPitch[Convert.ToInt32(slt[0].Substring(l, 1))] = Convert.ToDouble(slt[1]);
                                        }
                                        catch { }
                                        break;
                                    case "ms_iSpecialPitchApply":
                                        try
                                        {
                                            SpecialApply[Convert.ToInt32(slt[0].Substring(l, 1))] = Convert.ToInt32(slt[1]);
                                        }
                                        catch { }
                                        break;
                                    case "ms_iSpecialPitchDir":
                                        try
                                        {
                                            SpecialDir[Convert.ToInt32(slt[0].Substring(l, 1))] = Convert.ToInt32(slt[1]);
                                        }
                                        catch { }
                                        break;
                                    case "ms_iSpecialDX":
                                        try
                                        {
                                            SpecialDX[Convert.ToInt32(slt[0].Substring(l, 1))] = Convert.ToInt32(slt[1]);
                                        }
                                        catch { }
                                        break;
                                    case "ms_iSpecialDY":
                                        try
                                        {
                                            SpecialDY[Convert.ToInt32(slt[0].Substring(l, 1))] = Convert.ToInt32(slt[1]);
                                        }
                                        catch { }
                                        break;
                                    case "ms_iSpecialDPos":
                                        try
                                        {
                                            SpecialDPos[Convert.ToInt32(slt[0].Substring(l, 1))] = Convert.ToInt32(slt[1]);
                                        }
                                        catch { }
                                        break;
                                    case "ms_iSpecialDApply":
                                        try
                                        {
                                            SpecialDApply[Convert.ToInt32(slt[0].Substring(l, 1))] = Convert.ToInt32(slt[1]);
                                        }
                                        catch { }
                                        break;
                                    case "ms_iSpecialDDir":
                                        try
                                        {
                                            SpecialDDir[Convert.ToInt32(slt[0].Substring(l, 1))] = Convert.ToInt32(slt[1]);
                                        }
                                        catch { }
                                        break;
                                }
                            }
                        }

                    }
                    #endregion
                    if (npos > pos)
                    {
                        #region
                        pos = 0;
                        for (int i = npos + 1; i < str.Length; i++)
                        {
                            string[] slt = str[i].Split('=');
                            if (slt.Length == 1)
                            {
                                if (slt[0] == "[One Chip Offset]")
                                {
                                    pos = i;
                                    break;
                                }
                            }
                            if (slt.Length == 2)
                            {
                                switch (slt[0])
                                {
                                    case "m_dFirstX":
                                        if (section == eMarkSectionType.eSecTypeUnit)
                                            FirstX = Convert.ToDouble(slt[1]) - anpos.X - anupos.X;
                                        else FirstX = Convert.ToDouble(slt[1]) - anpos.X;
                                        break;
                                    case "m_dFirstY":
                                        if (section == eMarkSectionType.eSecTypeUnit)
                                            FirstY = Convert.ToDouble(slt[1]) - anpos.Y - anupos.Y;
                                        else
                                            FirstY = Convert.ToDouble(slt[1]) - anpos.Y; break;
                                    case "m_dSecondX": SecondX = Convert.ToDouble(slt[1]); break;
                                    case "m_dSecondY": SecondY = Convert.ToDouble(slt[1]); break;
                                    case "m_dThirdX": ThirdX = Convert.ToDouble(slt[1]); break;
                                    case "m_dThirdY": ThirdY = Convert.ToDouble(slt[1]); break;
                                    case "m_dUpperMasterX": UpperMasterX = Convert.ToDouble(slt[1]); break;
                                    case "m_dUpperMasterY": UpperMasterY = Convert.ToDouble(slt[1]); break;
                                    case "m_dUpperSlaveX": UpperSlaveX = Convert.ToDouble(slt[1]); break;
                                    case "m_dUpperSlaveY": UpperSlaveY = Convert.ToDouble(slt[1]); break;
                                    case "m_dLowerMasterX": LowerMasterX = Convert.ToDouble(slt[1]); break;
                                    case "m_dLowerMasterY": LowerMasterY = Convert.ToDouble(slt[1]); break;
                                    case "m_dLowerSlaveX": LowerSlaveX = Convert.ToDouble(slt[1]); break;
                                    case "m_dLowerSlaveY": LowerSlaveY = Convert.ToDouble(slt[1]); break;
                                    case "m_nMarkingPos": MarkingPos = Convert.ToInt32(slt[1]); break;
                                    case "m_dRotateMarkingField": RotateMarkingField = Convert.ToDouble(slt[1]); break;
                                    case "m_dLowerRotateMarkingField": LowerRotateMarkingField = Convert.ToDouble(slt[1]); break;
                                    case "UnitAttribute": UnitAttribute = slt[1]; break;
                                    case "SEMType": SEMType = slt[1]; break;
                                }
                            }
                        }
                        #endregion
                        if (pos > npos)
                        {
                            #region
                            npos = 0;
                            for (int i = pos; i < str.Length; i++)
                            {
                                string[] slt = str[i].Split('=');
                                if (slt.Length >= 1)
                                {
                                    if (slt[0] == "[BiV data]")
                                    {
                                        npos = i;
                                        break;
                                    }
                                    if (slt[0] == "OneChipOffset")
                                    {
                                        int n = 0;
                                        for (int j = i + 1; j < str.Length; j++)
                                        {
                                            slt = str[j].Split(',');
                                            if (slt.Length <= 0) continue;
                                            if (slt[0] == "[END One Chip Offset]")
                                            {
                                                break;
                                            }
                                            if (slt.Length >= 3)
                                            {
                                                try
                                                {
                                                    if (eMarkSectionType.eSecTypeRail == section)
                                                    {
                                                        int nn = n % SpecialApply[0];
                                                        if (nn == 0)
                                                            OneChipOffset[n] = new Point(Convert.ToDouble(slt[0]), Convert.ToDouble(slt[1]));
                                                        else OneChipOffset[n] = new Point(Convert.ToDouble(slt[0]) + (0.1 * nn), Convert.ToDouble(slt[1]));
                                                    }
                                                    else OneChipOffset[n] = new Point(Convert.ToDouble(slt[0]), Convert.ToDouble(slt[1]));
                                                    n++;
                                                }
                                                catch { }
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                            if (npos > pos)
                            {
                                #region
                                for (int i = npos + 1; i < str.Length; i++)
                                {
                                    string[] slt = str[i].Split('=');
                                    if (slt.Length == 2)
                                    {
                                        switch (slt[0])
                                        {
                                            case "m_dlXAbsolute": lXAbsolute = Convert.ToDouble(slt[1]); break;
                                            case "m_dlYAbsolute": lYAbsolute = Convert.ToDouble(slt[1]); break;
                                            case "m_nXPixel": XPixel = Convert.ToInt32(slt[1]); break;
                                            case "m_nYPixel": YPixel = Convert.ToInt32(slt[1]); break;
                                            case "m_dlXFoV": lXFoV = Convert.ToDouble(slt[1]); break;
                                            case "m_dlYFoV": lYFoV = Convert.ToDouble(slt[1]); break;
                                            case "m_dlXPtoF": lXPtoF = Convert.ToDouble(slt[1]); break;
                                            case "m_dlYPtoF": lYPtoF = Convert.ToDouble(slt[1]); break;

                                            case "m_bUseFirstID": UseFirstID = Convert.ToInt32(slt[1]); break;
                                            case "m_dID1XPos": ID1XPos = Convert.ToDouble(slt[1]); break;
                                            case "m_dID1YPos": ID1YPos = Convert.ToDouble(slt[1]); break;
                                            case "m_dID1Diameter": ID1Diameter = Convert.ToDouble(slt[1]); break;
                                            case "m_bUseSecondID": UseSecondID = Convert.ToInt32(slt[1]); break;
                                            case "m_dID2XPos": ID2XPos = Convert.ToDouble(slt[1]); break;
                                            case "m_dID2YPos": ID2YPos = Convert.ToDouble(slt[1]); break;
                                            case "m_dID2Diameter": ID2Diameter = Convert.ToDouble(slt[1]); break;
                                            case "m_bUseThirdID": UseThirdID = Convert.ToInt32(slt[1]); break;
                                            case "m_dID3XPos": ID3XPos = Convert.ToDouble(slt[1]); break;
                                            case "m_dID3YPos": ID3YPos = Convert.ToDouble(slt[1]); break;
                                            case "m_dID3Diameter": ID3Diameter = Convert.ToDouble(slt[1]); break;
                                            case "m_bUseFourthID": UseFourthID = Convert.ToInt32(slt[1]); break;
                                            case "m_dID4XPos": ID4XPos = Convert.ToDouble(slt[1]); break;
                                            case "m_dID4YPos": ID4YPos = Convert.ToDouble(slt[1]); break;
                                            case "m_dID4Diameter": ID4Diameter = Convert.ToDouble(slt[1]); break;

                                            case "m_iMarkStartPos": MarkStartPos = Convert.ToInt32(slt[1]); break;
                                            case "m_iMarkMajor": MarkMajor = Convert.ToInt32(slt[1]); break;
                                            case "m_iMarkType": MarkType = Convert.ToInt32(slt[1]); break;

                                            case "m_nGoldPointType": GoldPointType = Convert.ToInt32(slt[1]); break;
                                            case "m_dMarkingPositionX": MarkingPositionX = Convert.ToDouble(slt[1]); break;
                                            case "m_dMarkingPositionY": MarkingPositionY = Convert.ToDouble(slt[1]); break;
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public bool Save(ModelMarkInfo newmodel, int nStep)
        {

            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(Path); fs.Close();
            }
            catch { return false; }

            if (Path.Contains("_Unit"))
            {
                m_Section = eMarkSectionType.eSecTypeUnit;
            }
            else if (Path.Contains("_Rail"))
                m_Section = eMarkSectionType.eSecTypeRail;
            else if (Path.Contains("_Reject"))
                m_Section = eMarkSectionType.eSecTypeNum;
            else m_Section = eMarkSectionType.eSecTypeNone;

            try
            {
                List<string> lst = new List<string>();
                lst.Add("// MiME Strip File Ver" + Version);
                lst.Add("");
                lst.Add("[Strip information]");
                lst.Add("Version=" + Version);
                lst.Add(" ");
                lst.Add("[base strip]");
                if (m_Section == eMarkSectionType.eSecTypeUnit)
                {
                    lst.Add("m_iStripX=" + newmodel.StepUnits.ToString());
                    lst.Add("m_iStripY=" + newmodel.UnitRow.ToString());
                }
                else if (m_Section == eMarkSectionType.eSecTypeRail)
                {
                    lst.Add("m_iStripX=" + (newmodel.StepUnits * newmodel.UnitRow).ToString());
                    lst.Add("m_iStripY=1");
                }
                else
                {
                    lst.Add("m_iStripX=" + StripX.ToString());
                    lst.Add("m_iStripY=" + StripY.ToString());
                }
                lst.Add("m_dStripSizeX=" + StripSizeX.ToString("f6"));
                lst.Add("m_dStripSizeY=" + StripSizeY.ToString("f6"));
                lst.Add("m_iStripStyle=" + StripStyle.ToString());
                lst.Add("// 0 = landscape, 1 = portrait");
                lst.Add("m_iMarkingSide=" + MarkingSide.ToString());
                lst.Add("// 0 = top, 1 = back");
                if (m_Section == eMarkSectionType.eSecTypeRail)
                {
                    lst.Add("m_dXPitch=0.1");
                    lst.Add("m_dYPitch=0.1");
                }
                else if (m_Section == eMarkSectionType.eSecTypeUnit)
                {
                    lst.Add("m_dXPitch=" + newmodel.UnitWidth.ToString("f6"));
                    if (newmodel.ID == 1 && newmodel.Selection > 0)
                        lst.Add("m_dYPitch=" + newmodel.UnitHeight.ToString("f6"));
                    else lst.Add("m_dYPitch=0");
                }
                else
                {
                    lst.Add("m_dXPitch=" + XPitch.ToString("f6"));
                    lst.Add("m_dYPitch=" + YPitch.ToString("f6"));
                }
                lst.Add("m_sStripName=" + StripName);
                lst.Add("");
                lst.Add("[Base Chip]");
                lst.Add("m_iIDPosition=" + IDPosition.ToString());
                lst.Add("m_iLeadNum=" + LeadNum.ToString());
                if (m_Section == eMarkSectionType.eSecTypeUnit)
                {
                    UnitWidth = newmodel.UnitWidth;
                    UnitHeight = newmodel.UnitHeight;
                    lst.Add("m_dUnitWidth=" + newmodel.UnitWidth.ToString("f6"));
                    lst.Add("m_dUnitHeight=" + newmodel.UnitHeight.ToString("f6"));
                }
                else
                {
                    lst.Add("m_dUnitWidth=" + UnitWidth.ToString("f6"));
                    lst.Add("m_dUnitHeight=" + UnitHeight.ToString("f6"));
                }
                lst.Add("m_dIDRadius=" + IDRadius.ToString("f6"));
                lst.Add("m_iIDPosition_Vision=" + IDPosition_Vision.ToString());
                lst.Add("m_unVisionMagnification=" + VisionMfc.ToString());
                lst.Add("");
                lst.Add("[Special Strip Infomation]");
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchX[" + i.ToString() + "]=" + SpecialPitchX[i].ToString());
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchY[" + i.ToString() + "]=" + SpecialPitchY[i].ToString());
                }
                for (int i = 0; i < 6; i++)
                {
                    if (m_Section == eMarkSectionType.eSecTypeRail)
                    {
                        if (i == 0)
                        {
                            lst.Add("ms_dSpecialPitch[" + i.ToString() + "]=" + (newmodel.UnitWidth - (newmodel.UnitRow * 0.1)).ToString());
                            SpecialPitch[i] = (newmodel.UnitWidth - (newmodel.UnitRow * 0.1));
                        }
                        else
                        {
                            lst.Add("ms_dSpecialPitch[" + i.ToString() + "]=0");
                            SpecialPitch[i] = 0;
                        }
                    }
                    else lst.Add("ms_dSpecialPitch[" + i.ToString() + "]=" + SpecialPitch[i].ToString("f6"));
                }
                for (int i = 0; i < 6; i++)
                {
                    if (m_Section == eMarkSectionType.eSecTypeRail)
                    {
                        if (i == 0)
                        {
                            lst.Add("ms_iSpecialPitchApply[" + i.ToString() + "]=" + newmodel.UnitRow.ToString());
                            SpecialApply[i] = newmodel.UnitRow;
                        }
                        else
                        {
                            lst.Add("ms_iSpecialPitchApply[" + i.ToString() + "]=0");
                            SpecialApply[i] = 0;
                        }
                    }
                    else
                        lst.Add("ms_iSpecialPitchApply[" + i.ToString() + "]=" + SpecialApply[i].ToString());
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialPitchDir[" + i.ToString() + "]=" + SpecialDir[i].ToString());
                }
                if (m_Section == eMarkSectionType.eSecTypeNum)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        lst.Add("ms_iSpecialIDX[" + i.ToString() + "]=1");
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        lst.Add("ms_iSpecialIDY[" + i.ToString() + "]=1");
                    }
                }
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        lst.Add("ms_iSpecialIDX[" + i.ToString() + "]=" + SpecialDX[i].ToString());
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        lst.Add("ms_iSpecialIDY[" + i.ToString() + "]=" + SpecialDY[i].ToString());
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDPos[" + i.ToString() + "]=" + SpecialDPos[i].ToString());
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDApply[" + i.ToString() + "]=" + SpecialDApply[i].ToString());
                }
                for (int i = 0; i < 6; i++)
                {
                    lst.Add("ms_iSpecialIDDir[" + i.ToString() + "]=" + SpecialDDir[i].ToString());
                }
                lst.Add("m_dIDFOffset=" + DFOffset.ToString("f6"));
                lst.Add("m_iIDFDirection=" + DFDirection.ToString());
                lst.Add("");
                lst.Add("[Marking Field Setting]");
                if (nStep > 0)
                {
                    FirstX = m_tmpFirstX;
                    FirstY = m_tmpFirstY;
                }
                if (m_Section == eMarkSectionType.eSecTypeUnit)
                {
                    lst.Add("m_dFirstX=" + (FirstX + newmodel.Pos.X + newmodel.UPos.X).ToString("f6"));
                    lst.Add("m_dFirstY=" + (FirstY + newmodel.Pos.Y + newmodel.UPos.Y).ToString("f6"));
                }
                else
                {
                    lst.Add("m_dFirstX=" + (FirstX + newmodel.Pos.X).ToString("f6"));
                    lst.Add("m_dFirstY=" + (FirstY + newmodel.Pos.Y).ToString("f6"));

                }
                lst.Add("m_dSecondX=" + SecondX.ToString("f6"));
                lst.Add("m_dSecondY=" + SecondY.ToString("f6"));
                lst.Add("m_dThirdX=" + ThirdX.ToString("f6"));
                lst.Add("m_dThirdY=" + ThirdY.ToString("f6"));
                lst.Add("m_dUpperMasterX=" + UpperMasterX.ToString("f6"));
                lst.Add("m_dUpperMasterY=" + UpperMasterY.ToString("f6"));
                lst.Add("m_dUpperSlaveX=" + UpperSlaveX.ToString("f6"));
                lst.Add("m_dUpperSlaveY=" + UpperSlaveY.ToString("f6"));
                lst.Add("m_dLowerMasterX=" + LowerMasterX.ToString("f6"));
                lst.Add("m_dLowerMasterY=" + LowerMasterY.ToString("f6"));
                lst.Add("m_dLowerSlaveX=" + LowerSlaveX.ToString("f6"));
                lst.Add("m_dLowerSlaveY=" + LowerSlaveY.ToString("f6"));
                lst.Add("m_nMarkingPos=" + MarkingPos.ToString(""));
                lst.Add("m_dRotateMarkingField=" + RotateMarkingField.ToString("f6"));
                lst.Add("m_dLowerRotateMarkingField=" + LowerRotateMarkingField.ToString("f6"));
                lst.Add("");
                lst.Add("[Selective Marking]");
                string u = "";
                string s = "";
                for (int i = 0; i < StripX * StripY; i++)
                {
                    u += "1";
                    s += "0";
                }
                if (m_Section == eMarkSectionType.eSecTypeNum)
                {
                    lst.Add("UnitAttribute=0");
                    lst.Add("SEMType=1");
                }
                else
                {
                    lst.Add("UnitAttribute=" + u);
                    lst.Add("SEMType=" + s);
                }
                lst.Add("");
                lst.Add("[One Chip Offset]");
                lst.Add("OneChipOffset=");
                if (newmodel.ID == 1 && newmodel.Selection > 0)
                {
                    for (int i = 0; i < StripX * StripY; i++)
                    {
                        if (i >= OneChipOffset.Length)
                        {
                            lst.Add("0.000000, 0.000000, 0.000000");
                        }
                        else
                        {
                            if (m_Section == eMarkSectionType.eSecTypeRail)
                            {
                                int n = i % newmodel.UnitRow;
                                if (n == 0)
                                    lst.Add(OneChipOffset[i].X.ToString("f6") + ", " + OneChipOffset[i].Y.ToString("f6") + ", 0.000000");
                                else lst.Add((OneChipOffset[i].X - (0.1 * n)).ToString("f6") + ", " + OneChipOffset[i].Y.ToString("f6") + ", 0.000000");
                            }
                            else
                                lst.Add(OneChipOffset[i].X.ToString("f6") + ", " + OneChipOffset[i].Y.ToString("f6") + ", 0.000000");
                        }
                    }
                }
                else
                {
                    if (m_Section == eMarkSectionType.eSecTypeUnit)
                    {
                        for (int i = 0; i < StripY; i++)
                        {
                            for (int j = 0; j < StripX; j++)
                            {
                                lst.Add(string.Format("0.000000, {0}, 0.000000", (-(UnitHeight * ((StripY - 1) - i))).ToString("0.000000")));
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < StripX * StripY; i++)
                        {

                            if (i >= OneChipOffset.Length)
                            {
                                lst.Add("0.000000, 0.000000, 0.000000");
                            }
                            else
                            {
                                if (m_Section == eMarkSectionType.eSecTypeRail)
                                {
                                    int n = i % newmodel.UnitRow;
                                    if (n == 0)
                                        lst.Add(OneChipOffset[i].X.ToString("f6") + ", " + OneChipOffset[i].Y.ToString("f6") + ", 0.000000");
                                    else lst.Add((OneChipOffset[i].X - (0.1 * n)).ToString("f6") + ", " + OneChipOffset[i].Y.ToString("f6") + ", 0.000000");
                                }
                                else
                                    lst.Add(OneChipOffset[i].X.ToString("f6") + ", " + OneChipOffset[i].Y.ToString("f6") + ", 0.000000");
                            }

                        }
                    }
                }
                lst.Add("[END One Chip Offset]");
                lst.Add("");
                lst.Add("[BiV data]");
                lst.Add("m_dlXAbsolute=" + lXAbsolute.ToString("f6"));
                lst.Add("m_dlYAbsolute=" + lYAbsolute.ToString("f6"));
                lst.Add("m_nXPixel=" + XPixel.ToString(""));
                lst.Add("m_nYPixel=" + YPixel.ToString(""));
                lst.Add("m_dlXFoV=" + lXFoV.ToString("f6"));
                lst.Add("m_dlYFoV=" + lYFoV.ToString("f6"));
                lst.Add("m_dlXPtoF=" + lXPtoF.ToString("f6"));
                lst.Add("m_dlYPtoF=" + lYPtoF.ToString("f6"));
                lst.Add("[END of BiV data]");
                lst.Add("");
                lst.Add("[Manual ID]");
                lst.Add("m_bUseFirstID=" + UseFirstID.ToString());
                lst.Add("m_dID1XPos=" + ID1XPos.ToString("f6"));
                lst.Add("m_dID1YPos=" + ID1XPos.ToString("f6"));
                lst.Add("m_dID1Diameter=" + ID1Diameter.ToString("f6"));
                lst.Add("m_bUseSecondID=" + UseSecondID.ToString());
                lst.Add("m_dID2XPos=" + ID2XPos.ToString("f6"));
                lst.Add("m_dID2YPos=" + ID2XPos.ToString("f6"));
                lst.Add("m_dID2Diameter=" + ID2Diameter.ToString("f6"));
                lst.Add("m_bUseThirdID=" + UseThirdID.ToString());
                lst.Add("m_dID3XPos=" + ID3XPos.ToString("f6"));
                lst.Add("m_dID3YPos=" + ID3XPos.ToString("f6"));
                lst.Add("m_dID3Diameter=" + ID3Diameter.ToString("f6"));
                lst.Add("m_bUseFourthID=" + UseFourthID.ToString());
                lst.Add("m_dID3XPos=" + ID3XPos.ToString("f6"));
                lst.Add("m_dID3YPos=" + ID3XPos.ToString("f6"));
                lst.Add("m_dID3Diameter=" + ID3Diameter.ToString("f6"));
                lst.Add("[End of Manual ID]");
                lst.Add("");
                lst.Add("[Marking Order]");
                lst.Add("//m_iMarkStartPos : 0 = TL, 1 = TR, 2 = BL, 3 = BR");
                lst.Add("m_iMarkStartPos=" + MarkStartPos.ToString());
                lst.Add("//m_iMarkMajor : 0 = Row Major, 1: Column Major");
                lst.Add("m_iMarkMajor=" + MarkMajor.ToString());
                lst.Add("//m_iMarkType : 0 = S(Zigzag) Type, 1: 11(One direction) Type");
                lst.Add("m_iMarkType=" + MarkType.ToString());
                lst.Add("[End of Marking Order]");
                lst.Add("");
                lst.Add("[Custom data]");
                lst.Add("// m_nGoldPointType : Substrate Gold Point Type Section For Motech Handler");
                lst.Add("m_nGoldPointType=" + GoldPointType.ToString());
                lst.Add("// m_dMarkingPositionX : Marking Position for RobotHIF");
                lst.Add("m_dMarkingPositionX=" + MarkingPositionX.ToString("f6"));
                lst.Add("m_dMarkingPositionY=" + MarkingPositionY.ToString("f6"));
                lst.Add("[END of Custom data]");
                File.WriteAllLines(Path, lst);
            }
            catch
            {
                return false;
            }
            return true;

        }
    }

    public class Sem
    {
        public const double Res = 120.0 / 65536;
        public int Unit = 1;
        public double ChipXSize = 77.500;
        public double ChipYSize = 77.500;
        public int SemRotateStatus = 0;  //////////////////////////이거뭔지 모르게따

        #region [GUIDE LINE]
        public GuideLine[] Guideline = new GuideLine[8];
        #endregion
        private int UseAutoHorizontalAlign = 0;
        private int UseAutoVerticalAlign = 0;
        public List<HPGL> lstHPGL = new List<HPGL>();
        public List<TEXT> lstText = new List<TEXT>();
        public List<IDMARK> lstIDMark = new List<IDMARK>();

        public string Path;

        public bool Load(string path)
        {
            lstHPGL.Clear();
            lstText.Clear();
            lstIDMark.Clear();
            if (File.Exists(path))
            {
                Path = path;
                string[] str = File.ReadAllLines(path);
                int spos = 0;
                int epos = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    string[] slt = str[i].Split('=', '\t');
                    var val = slt.Where(item => !string.IsNullOrEmpty(item));
                    slt = val.ToArray<string>();
                    if (slt.Length > 0)
                    {
                        if (slt[0] == "[GUIDE LINE]")
                        {
                            epos = i;
                            break;
                        }
                    }
                    if (slt.Length == 2)
                    {
                        if (slt[0] == "Unit") Unit = Convert.ToInt32(slt[1]);
                        else if (slt[0] == "ChipXSize") ChipXSize = Convert.ToDouble(slt[1]);
                        else if (slt[0] == "ChipYSize") ChipYSize = Convert.ToDouble(slt[1]);
                        else if (slt[0] == "SemRotateStatus")
                        {
                            SemRotateStatus = Convert.ToInt32(slt[1]);
                            spos = i + 1;
                        }
                    }
                }
                for (int i = spos; i < epos; i++)
                {
                    string[] slt = str[i].Split('=', '\t');
                    var val = slt.Where(item => !string.IsNullOrEmpty(item));
                    slt = val.ToArray<string>();
                    if (slt.Length > 0)
                    {
                        if (slt[0] == "[TEXT OBJECT]")
                        {
                            for (int j = i + 1; j < epos; j++)
                            {
                                slt = str[j].Split('=', '\t');
                                val = slt.Where(item => !string.IsNullOrEmpty(item));
                                slt = val.ToArray<string>();
                                if (slt.Length > 0)
                                    if (slt[0] == "[END_ObjectTEXT]")
                                    {
                                        string[] strTEXT = new string[j - i + 1];

                                        Array.Copy(str, i, strTEXT, 0, strTEXT.Length);
                                        TEXT text = new TEXT(strTEXT);
                                        lstText.Add(text);
                                        i = j;
                                        break;
                                    }
                            }
                        }
                        if (slt[0] == "[2D_BARCODE_OBJECT]")
                        {
                            for (int j = i + 1; j < epos; j++)
                            {
                                slt = str[j].Split('=', '\t');
                                val = slt.Where(item => !string.IsNullOrEmpty(item));
                                slt = val.ToArray<string>();
                                if (slt.Length > 0)
                                    if (slt[0] == "[END_2D_BARCODE_OBJECT]")
                                    {
                                        string[] strTEXT = new string[j - i + 1];

                                        Array.Copy(str, i, strTEXT, 0, strTEXT.Length);
                                        IDMARK text = new IDMARK(strTEXT);
                                        lstIDMark.Add(text);
                                        i = j;
                                        break;
                                    }
                            }
                        }
                        if (slt[0] == "[HPGL OBJECT]")
                        {
                            for (int j = i + 1; j < epos; j++)
                            {
                                slt = str[j].Split('=', '\t');
                                val = slt.Where(item => !string.IsNullOrEmpty(item));
                                slt = val.ToArray<string>();
                                if (slt.Length > 0)
                                    if (slt[0] == "[END_ObjectHPGL]")
                                    {
                                        string[] strHPGL = new string[j - i + 1];
                                        Array.Copy(str, i, strHPGL, 0, strHPGL.Length);
                                        HPGL hpgl = new HPGL(strHPGL);
                                        lstHPGL.Add(hpgl);
                                        i = j;
                                        break;
                                    }
                            }
                        }
                    }
                }
                int npos = 0;
                for (int i = epos + 1; i < str.Length; i++)
                {
                    string[] slt = str[i].Split('=', '\t');
                    var val = slt.Where(item => !string.IsNullOrEmpty(item));
                    slt = val.ToArray<string>();
                    if (slt.Length > 0)
                    {
                        if (slt[0] == "[END GUIDE LINE]")
                        {
                            npos = i;
                            break;
                        }
                        if (slt[0] == "ItemNum")
                        {
                            GuideLine gl = new GuideLine();
                            gl.ItemNum = Convert.ToInt32(slt[1]);
                            slt = str[i + 1].Split('=');
                            gl.ItemUse = Convert.ToInt32(slt[1]);
                            slt = str[i + 2].Split('=');
                            gl.ItemAlign = Convert.ToInt32(slt[1]);
                            slt = str[i + 3].Split('=');
                            gl.ItemPosX = Convert.ToDouble(slt[1]);
                            slt = str[i + 4].Split('=');
                            gl.ItemPosY = Convert.ToDouble(slt[1]);
                            slt = str[i + 5].Split('=');
                            gl.ItemWidth = Convert.ToDouble(slt[1]);
                            slt = str[i + 6].Split('=');
                            gl.ItemHeight = Convert.ToDouble(slt[1]);
                            Guideline[gl.ItemNum] = gl;
                        }
                    }
                }
                for (int i = npos + 1; i < str.Length; i++)
                {
                    string[] slt = str[i].Split('=', '\t');
                    var val = slt.Where(item => !string.IsNullOrEmpty(item));
                    slt = val.ToArray<string>();
                    if (slt.Length > 0)
                    {
                        if (slt[0] == "[END AUTO ALIGNMENT INFO]")
                        {
                            break;
                        }
                        if (slt[0] == "Use Auto Horizontal Align") UseAutoHorizontalAlign = Convert.ToInt32(slt[1]);
                        if (slt[0] == "Use Auto Vertical Align") UseAutoVerticalAlign = Convert.ToInt32(slt[1]);
                    }
                }

                return true;
            }
            return false;
        }

        public bool Save()
        {
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(Path); fs.Close();
            }
            catch { return false; }
            try
            {
                List<string> lst = new List<string>();
                lst.Add("// MiME SemData File Ver0.5");
                lst.Add("");
                lst.Add("Unit=" + Unit.ToString());
                lst.Add("ChipXSize=" + ChipXSize.ToString("f3"));
                lst.Add("ChipYSize=" + ChipYSize.ToString("f3"));
                lst.Add("SemRotateStatus=" + SemRotateStatus.ToString());
                //////Obects
                int nObject = 0;
                for (int i = 0; i < lstIDMark.Count; i++)
                {
                    lst.Add('\t' + "[2D_BARCODE_OBJECT]");
                    lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                    nObject++;
                    lst.Add("\t\t[BARCODE_ATTRIBUTE]");
                    lst.Add("\t\tVer=1");
                    lst.Add("\t\tContents=H19C1234123");
                    lst.Add("\t\tBarcode Type=71");
                    lst.Add("\t\tStart Mode=0");
                    lst.Add("\t\t2D Barcode Width=1.5000");
                    lst.Add("\t\t2D Barcode Height=1.5000");
                    lst.Add("\t\tDot Ratio=1.0000");
                    lst.Add("\t\tDot Line Num=4");
                    lst.Add("\t\tSelect Line Box=1");
                    lst.Add("\t\t2D Barcode Check Digit=0");
                    lst.Add("\t\tColumn and Row Index=0");
                    lst.Add("\t\tReverse Marking=0");
                    lst.Add("\t\tMake L=0");
                    lst.Add("\t\tHorizontal Line=0");
                    lst.Add("\t\tZigzag Type=0");
                    lst.Add("\t\tSampling num=2");
                    lst.Add("\t\tChange Cell Size=1");
                    lst.Add("\t\tResoultion=40");
                    lst.Add("\t\tShot Count=1");
                    lst.Add("\t\tLine Margin Ratio=0");
                    lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t\tStyle=0");
                    lst.Add("\t\t\tIncType=0");
                    lst.Add("\t\t\tStartNumber=0");
                    lst.Add("\t\t\tRangeStartNumber=0");
                    lst.Add("\t\t\tRangeEndNumber=0");
                    lst.Add("\t\t\tPreFix=");
                    lst.Add("\t\t\tPostFix=");
                    lst.Add("\t\t\tMaxCharSize=5");
                    lst.Add("\t\t\tSavedCurrentNumber=0");
                    lst.Add("\t\t\tSCHEME_Order=-1");
                    lst.Add("\t\t\tSCHEME_Position=-1");
                    lst.Add("\t\t\tSCHEME_Type=-1");
                    lst.Add("\t\t\tSCHEME_Numbering=-1");
                    lst.Add("\t\t\tSaveFileWhenUpdate=0");
                    lst.Add("\t\t\tUpdateStartNumAsCurrNum=0");
                    lst.Add("\t\t\tNumericSystemType=0");
                    lst.Add("\t\t\tIncreaseLevels=1");
                    lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\t\tSpecial Type=2");
                    lst.Add("\t\t\tReference Number 1=2");
                    lst.Add("\t\t\tReference Number 2=0");
                    lst.Add("\t\t\t[END_SPECIAL_TYPE]");
                    lst.Add("\t\t[END_SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t[END_BARCODE_ATTRIBUTE]");
                    lst.Add("\t\t[Attribute]");
                    lst.Add("\t\tObjType=2DBARCODE");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=1");
                    lst.Add("\t\t\tAxisField=180.000000");
                    lst.Add("\t\t\tUNIT=1");

                    lst.Add("\t\t\tDispLeft=" + lstIDMark[i].DispLeft.ToString("f3"));
                    lst.Add("\t\t\tDispRight=" + lstIDMark[i].DispRight.ToString("f3"));
                    lst.Add("\t\t\tDispTop=" + lstIDMark[i].DispTop.ToString("f3"));
                    lst.Add("\t\t\tDispBottom=" + lstIDMark[i].DispBottom.ToString("f3"));
                    lst.Add("\t\t\tDispRect=" + ((int)(lstIDMark[i].DispLeft / Res)).ToString() + "," +
                                                              ((int)(lstIDMark[i].DispTop / Res)).ToString() + "," +
                                                              ((int)(lstIDMark[i].DispRight / Res)).ToString() + "," +
                                                              ((int)(lstIDMark[i].DispBottom / Res)).ToString());

                    Matrix mat = new Matrix((int)lstIDMark[i].RotateAngle, lstIDMark[i].DispLeft, lstIDMark[i].DispTop, lstIDMark[i].DispRight, lstIDMark[i].DispBottom);
                    lst.Add("\t\t\tMatrixA=" + lstIDMark[i].MatrixA.ToString("f6"));
                    lst.Add("\t\t\tMatrixB=" + lstIDMark[i].MatrixB.ToString("f6"));
                    lst.Add("\t\t\tMatrixC=" + lstIDMark[i].MatrixC.ToString("f6"));
                    lst.Add("\t\t\tMatrixD=" + lstIDMark[i].MatrixD.ToString("f6"));
                    lst.Add("\t\t\tMatrixX=" + mat.MatrixX.ToString("f6"));
                    lst.Add("\t\t\tMatrixY=" + mat.MatrixY.ToString("f6"));
                    lst.Add("\t\t\tMatrixZ=" + lstIDMark[i].MatrixZ.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixA=" + lstIDMark[i].BasicMatrixA.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixB=" + lstIDMark[i].BasicMatrixB.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixC=" + lstIDMark[i].BasicMatrixC.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixD=" + lstIDMark[i].BasicMatrixD.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixX=" + mat.BasicMatrixX.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixY=" + mat.BasicMatrixY.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixZ=" + lstIDMark[i].BasicMatrixZ.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixA=" + lstIDMark[i].RotateMatrixA.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixB=" + lstIDMark[i].RotateMatrixB.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixC=" + lstIDMark[i].RotateMatrixC.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixD=" + lstIDMark[i].RotateMatrixD.ToString("f6"));

                    lst.Add("\t\t\tRotateMatrixX=" + lstIDMark[i].RotateMatrixX.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixY=" + lstIDMark[i].RotateMatrixY.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixZ=" + lstIDMark[i].RotateMatrixZ.ToString("f6"));


                    lst.Add("\t\t\tRotateAngle=" + lstIDMark[i].RotateAngle.ToString("f6"));

                    lst.Add("\t\t\tParaNumber=5");
                    lst.Add("\t\t\tLockObject=0");
                    lst.Add("\t\t[END Attribute]");
                    lst.Add("\t[END_2D_BARCODE_OBJECT]");
                }
                for (int i = 0; i < lstHPGL.Count; i++)
                {
                    lst.Add('\t' + "[HPGL OBJECT]");
                    lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                    lst.Add('\t' + "FileName=" + lstHPGL[i].FileName.ToString());
                    nObject++;
                    lst.Add("\t\t" + "[Attribute]");
                    lst.Add("\t\tObjType=HPGL");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=180.000000");
                    lst.Add("\t\t\tUNIT=" + lstHPGL[i].UNIT.ToString());
                    lst.Add("\t\t\tDispLeft=" + lstHPGL[i].DispLeft.ToString("f3"));
                    lst.Add("\t\t\tDispRight=" + lstHPGL[i].DispRight.ToString("f3"));
                    lst.Add("\t\t\tDispTop=" + lstHPGL[i].DispTop.ToString("f3"));
                    lst.Add("\t\t\tDispBottom=" + lstHPGL[i].DispBottom.ToString("f3"));
                    lst.Add("\t\t\tDispRect=" + ((int)(lstHPGL[i].DispLeft / Res)).ToString() + "," +
                                                              ((int)(lstHPGL[i].DispTop / Res)).ToString() + "," +
                                                              ((int)(lstHPGL[i].DispRight / Res)).ToString() + "," +
                                                              ((int)(lstHPGL[i].DispBottom / Res)).ToString());

                    Matrix mat = new Matrix((int)lstHPGL[i].RotateAngle, lstHPGL[i].DispLeft, lstHPGL[i].DispTop, lstHPGL[i].DispRight, lstHPGL[i].DispBottom);
                    lst.Add("\t\t\tMatrixA=" + lstHPGL[i].MatrixA.ToString("f6"));
                    lst.Add("\t\t\tMatrixB=" + lstHPGL[i].MatrixB.ToString("f6"));
                    lst.Add("\t\t\tMatrixC=" + lstHPGL[i].MatrixC.ToString("f6"));
                    lst.Add("\t\t\tMatrixD=" + lstHPGL[i].MatrixD.ToString("f6"));
                    lst.Add("\t\t\tMatrixX=" + mat.MatrixX.ToString("f6"));
                    lst.Add("\t\t\tMatrixY=" + mat.MatrixY.ToString("f6"));
                    lst.Add("\t\t\tMatrixZ=" + lstHPGL[i].MatrixZ.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixA=" + lstHPGL[i].BasicMatrixA.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixB=" + lstHPGL[i].BasicMatrixB.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixC=" + lstHPGL[i].BasicMatrixC.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixD=" + lstHPGL[i].BasicMatrixD.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixX=" + mat.BasicMatrixX.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixY=" + mat.BasicMatrixY.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixZ=" + lstHPGL[i].BasicMatrixZ.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixA=" + lstHPGL[i].RotateMatrixA.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixB=" + lstHPGL[i].RotateMatrixB.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixC=" + lstHPGL[i].RotateMatrixC.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixD=" + lstHPGL[i].RotateMatrixD.ToString("f6"));

                    lst.Add("\t\t\tRotateMatrixX=" + lstHPGL[i].RotateMatrixX.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixY=" + lstHPGL[i].RotateMatrixY.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixZ=" + lstHPGL[i].RotateMatrixZ.ToString("f6"));


                    lst.Add("\t\t\tRotateAngle=" + lstHPGL[i].RotateAngle.ToString("f6"));
                    lst.Add("\t\t\tParaNumber=" + lstHPGL[i].ParaNumber.ToString());
                    lst.Add("\t\t\tLockObject=" + lstHPGL[i].LockObject.ToString());
                    lst.Add("\t\t[END Attribute]");
                    lst.Add("\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\tSpecial Type=" + lstHPGL[i].SpecialType.ToString());
                    lst.Add("\t\tReference Number 1=" + lstHPGL[i].ReferenceNumber1.ToString());
                    lst.Add("\t\tReference Number 2=" + lstHPGL[i].ReferenceNumber2.ToString());
                    lst.Add("\t\t[END_SPECIAL_TYPE]");
                    lst.Add('\t' + "LogoAlignMode=" + lstHPGL[i].LogoAlignMode.ToString());
                    lst.Add('\t' + "[END_ObjectHPGL]");
                    lst.Add(" ");
                }

                for (int i = 0; i < lstText.Count; i++)
                {
                    lst.Add('\t' + "[TEXT OBJECT]");
                    lst.Add('\t' + "ObjNumber=" + nObject.ToString());
                    nObject++;
                    lst.Add("\t\t[TEXT_ATTRIBUTE]");
                    if (lstText[i].Content == "" || lstText[i].Content == " ")
                        lst.Add("\t\tContent= ");
                    else lst.Add("\t\tContent=" + lstText[i].Content);
                    lst.Add("\t\tMaxNum=" + lstText[i].MaxNum.ToString());
                    lst.Add("\t\tAlignChar=" + lstText[i].AlignChar.ToString());
                    lst.Add("\t\tAlignMode=" + lstText[i].AlignMode.ToString());
                    lst.Add("\t\tCapitalHeight=" + lstText[i].CapitalHeight.ToString("f4"));
                    lst.Add("\t\tCharGap=" + lstText[i].CharGap.ToString("f4"));
                    lst.Add("\t\tFontName=" + lstText[i].FontName);
                    lst.Add("\t\tHeight=" + lstText[i].Height.ToString("f4"));
                    lst.Add("\t\tLineGap=" + lstText[i].LineGap.ToString("f4"));
                    lst.Add("\t\tWidth=" + lstText[i].Width.ToString("f4"));
                    lst.Add("\t\tSpaceSize=" + lstText[i].SpaceSize.ToString("f4"));
                    lst.Add("\t\tUseCapitalLineGap=" + lstText[i].UseCapitalLineGap.ToString());
                    lst.Add("\t\tUseFixStringLenght=" + lstText[i].UseFixStringLenght.ToString());
                    lst.Add("\t\tUseLeftGapofeachChar=" + lstText[i].UseLeftGapofeachChar.ToString());
                    lst.Add("\t\tUseFixNumberofChar=" + lstText[i].UseFixNumberofChar.ToString());
                    lst.Add("\t\tFixedStrSize=" + lstText[i].FixedStrSize.ToString());
                    lst.Add("\t\tFixNumberofChar=" + lstText[i].FixNumberofChar.ToString());
                    lst.Add("\t\tItalicAngle=" + lstText[i].ItalicAngle.ToString());
                    lst.Add("\t\tNTMark_WidthStype=" + lstText[i].NTMark_WidthStype.ToString());
                    lst.Add("\t\tUseStringCutting=" + lstText[i].UseStringCutting.ToString());
                    lst.Add("\t\tFirst=" + lstText[i].First.ToString());
                    lst.Add("\t\tCount=" + lstText[i].Count.ToString());
                    lst.Add("\t\tUseCriterionChar=" + lstText[i].UseCriterionChar.ToString());
                    lst.Add("\t\tCriterionChar=" + lstText[i].CriterionChar);
                    lst.Add("\t\tUseFixStringLenToCurrent=" + lstText[i].UseFixStringLenToCurrent.ToString());
                    lst.Add("\t\t\t[SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t\tStyle=" + lstText[i].Style.ToString());
                    lst.Add("\t\t\tIncType=" + lstText[i].IncType.ToString());
                    lst.Add("\t\t\tStartNumber=" + lstText[i].StartNumber.ToString());
                    lst.Add("\t\t\tRangeStartNumber=" + lstText[i].RangeStartNumber.ToString());
                    lst.Add("\t\t\tRangeEndNumber=" + lstText[i].RangeEndNumber.ToString());
                    lst.Add("\t\t\tPreFix=" + lstText[i].PreFix);
                    lst.Add("\t\t\tPostFix=" + lstText[i].PostFix);
                    lst.Add("\t\t\tMaxCharSize=" + lstText[i].MaxCharSize.ToString());
                    lst.Add("\t\t\tSavedCurrentNumber=" + lstText[i].SavedCurrentNumber.ToString());
                    lst.Add("\t\t\tSCHEME_Order=" + lstText[i].SCHEME_Order.ToString());
                    lst.Add("\t\t\tSCHEME_Position=" + lstText[i].SCHEME_Position.ToString());
                    lst.Add("\t\t\tSCHEME_Type=" + lstText[i].SCHEME_Type.ToString());
                    lst.Add("\t\t\tSCHEME_Numbering=" + lstText[i].SCHEME_Numbering.ToString());
                    lst.Add("\t\t\tSaveFileWhenUpdate=" + lstText[i].SaveFileWhenUpdate.ToString());
                    lst.Add("\t\t\tUpdateStartNumAsCurrNum=" + lstText[i].UpdateStartNumAsCurrNum.ToString());
                    lst.Add("\t\t\tNumericSystemType=" + lstText[i].NumericSystemType.ToString());
                    lst.Add("\t\t\tIncreaseLevels=" + lstText[i].IncreaseLevels.ToString());
                    lst.Add("\t\t\t\t[SPECIAL_TYPE]");
                    lst.Add("\t\t\t\tSpecial Type=" + lstText[i].SpecialType.ToString());
                    lst.Add("\t\t\t\tReference Number 1=" + lstText[i].ReferenceNumber1.ToString());
                    lst.Add("\t\t\t\tReference Number 2=" + lstText[i].ReferenceNumber2.ToString());
                    lst.Add("\t\t\t\t[END_SPECIAL_TYPE]");
                    lst.Add("\t\t\t[END_SERIAL_ATTRIBUTE]");
                    lst.Add("\t\t[END_TEXT_ATTRIBUTE]");
                    lst.Add("\t\t[Attribute]");
                    lst.Add("\t\tObjType=TEXT");
                    lst.Add("\t\tIsSelect=0");
                    lst.Add("\t\tIsUpdate=0");
                    lst.Add("\t\t\tAxisField=120.000000");
                    lst.Add("\t\t\tUNIT=" + lstText[i].UNIT.ToString());
                    lst.Add("\t\t\tDispLeft=" + lstText[i].DispLeft.ToString("f3"));
                    lst.Add("\t\t\tDispRight=" + lstText[i].DispRight.ToString("f3"));
                    lst.Add("\t\t\tDispTop=" + lstText[i].DispTop.ToString("f3"));
                    lst.Add("\t\t\tDispBottom=" + lstText[i].DispBottom.ToString("f3"));
                    lst.Add("\t\t\tDispRect=" + ((int)(lstText[i].DispLeft / Res)).ToString() + "," +
                                                ((int)(lstText[i].DispTop / Res)).ToString() + "," +
                                                ((int)(lstText[i].DispRight / Res)).ToString() + "," +
                                                ((int)(lstText[i].DispBottom / Res)).ToString());

                    Matrix mat = new Matrix((int)lstText[i].RotateAngle, lstText[i].DispLeft, lstText[i].DispTop, lstText[i].DispRight, lstText[i].DispBottom);
                    lst.Add("\t\t\tMatrixA=" + mat.MatrixA.ToString("f6"));
                    lst.Add("\t\t\tMatrixB=" + mat.MatrixB.ToString("f6"));
                    lst.Add("\t\t\tMatrixC=" + mat.MatrixC.ToString("f6"));
                    lst.Add("\t\t\tMatrixD=" + mat.MatrixD.ToString("f6"));
                    lst.Add("\t\t\tMatrixX=" + mat.MatrixX.ToString("f6"));
                    lst.Add("\t\t\tMatrixY=" + mat.MatrixY.ToString("f6"));
                    lst.Add("\t\t\tMatrixZ=" + mat.MatrixZ.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixA=" + mat.BasicMatrixA.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixB=" + mat.BasicMatrixB.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixC=" + mat.BasicMatrixC.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixD=" + mat.BasicMatrixD.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixX=" + mat.BasicMatrixX.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixY=" + mat.BasicMatrixY.ToString("f6"));
                    lst.Add("\t\t\tBasicMatrixZ=" + mat.BasicMatrixZ.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixA=" + mat.RotateMatrixA.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixB=" + mat.RotateMatrixB.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixC=" + mat.RotateMatrixC.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixD=" + mat.RotateMatrixD.ToString("f6"));

                    lst.Add("\t\t\tRotateMatrixX=" + mat.RotateMatrixX.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixY=" + mat.RotateMatrixY.ToString("f6"));
                    lst.Add("\t\t\tRotateMatrixZ=" + mat.RotateMatrixZ.ToString("f6"));

                    lst.Add("\t\t\tRotateAngle=" + lstText[i].RotateAngle.ToString("f6"));
                    lst.Add("\t\t\tParaNumber=" + lstText[i].ParaNumber.ToString());
                    lst.Add("\t\t\tLockObject=" + lstText[i].LockObject.ToString());
                    lst.Add("\t\t[END Attribute]");
                    lst.Add('\t' + "[END_ObjectTEXT]");
                    lst.Add(" ");
                }

                lst.Add('\t' + "[GUIDE LINE]");
                for (int i = 0; i < 8; i++)
                {
                    lst.Add('\t' + "ItemNum=" + i.ToString());
                    lst.Add('\t' + "ItemUse=" + Guideline[i].ItemUse.ToString());
                    lst.Add('\t' + "ItemAlign=" + Guideline[i].ItemAlign.ToString());
                    lst.Add('\t' + "ItemPosX=" + Guideline[i].ItemPosX.ToString("f3"));
                    lst.Add('\t' + "ItemPosY=" + Guideline[i].ItemPosY.ToString("f3"));
                    lst.Add('\t' + "ItemWidth=" + Guideline[i].ItemWidth.ToString("f3"));
                    lst.Add('\t' + "ItemHeight=" + Guideline[i].ItemHeight.ToString("f3"));
                }
                lst.Add('\t' + "[END GUIDE LINE]");
                lst.Add('\t' + "[AUTO ALIGNMENT INFO]");
                lst.Add('\t' + "Use Auto Horizontal Align=" + UseAutoHorizontalAlign.ToString());
                lst.Add('\t' + "Use Auto Vertical Align=" + UseAutoVerticalAlign.ToString());
                lst.Add('\t' + "[END AUTO ALIGNMENT INFO]");
                lst.Add("[END SemData]");
                File.WriteAllLines(Path, lst);
            }
            catch { return false; }
            return true;
        }
    }

    public class GuideLine
    {
        public int ItemNum;
        public int ItemUse;
        public int ItemAlign;
        public double ItemPosX;
        public double ItemPosY;
        public double ItemWidth;
        public double ItemHeight;
        public GuideLine()
        {
            ItemNum = 0;
            ItemUse = 0;
            ItemAlign = 0;
            ItemPosX = 0.000;
            ItemPosY = 0.000;
            ItemWidth = 0.000;
            ItemHeight = 0.000;
        }
    }

    public class PenParam
    {
        //public int DrawSpeed = 1000;
        //public int JumpSpeed = 1000;
        //public int WobbleDiameter = 0;
        //public int WobbleOverlap = 0;
        //public int BroadwayIn = 0;
        //public int BroadwayOut = 0;
        //public int SlowJump = 0;
        public int DrawStep = 20;
        public int JumpStep = 50;
        public int StepPeriod = 41;
        public int CornerDelay = 10;
        public int LaserOnDelay = 200;
        public int LaserOffDelay = 200;
        public int FPSDelay = 185;
        public int JumpDelay = 400;
        public int LineDelay = 350;
        public int Frequency = 20000;
        public int PulseDuty = 10;
        public double LampCurrent = 42.000000;
        public int AACValue = 0;
        public double SettingPower = 0.0; ///////0.0일때 Not Defined 로 저장
        public int ModeLampCurrent = 1;
        public int NumMark = 1;
        public int FirstLaserOnDelay = 0;
        public int Custom1 = 99;
        public int Custom2 = 0;
        public int Custom3 = 0;
        public int Custom4 = 0;
        public int ThermalTrack = 0;
        // public double CurrPower = 0.0;///////0.0일때 Not Defined 로 저장
        // public int LowerThreshold = 0;
        public bool Loaded = false;
        private string Path;
        public PenParam()
        {
            //DrawSpeed = 0;
            //JumpSpeed = 0;
            //WobbleDiameter = 0;
            //WobbleOverlap = 0;
            //BroadwayIn = 0;
            //BroadwayOut = 0;
            //SlowJump = 0;
            DrawStep = 0;
            JumpStep = 0;
            StepPeriod = 0;
            CornerDelay = 0;
            LaserOnDelay = 0;
            LaserOffDelay = 0;
            FPSDelay = 0;
            JumpDelay = 0;
            LineDelay = 0;
            Frequency = 0;
            PulseDuty = 0;
            LampCurrent = 0.0;
            AACValue = 0;
            SettingPower = 0.0;
            ModeLampCurrent = 1;
            NumMark = 1;
            FirstLaserOnDelay = 0;
            Custom1 = 99;
            Custom2 = 0;
            Custom3 = 0;
            Custom4 = 0;
            ThermalTrack = 0;
            // CurrPower = 0.0;
            // LowerThreshold = 0;
        }
        public PenParam(string path)
        {
            if (File.Exists(path))
            {
                Path = path;
                string[] lstStr = File.ReadAllLines(path);
                for (int i = 0; i < lstStr.Length; i++)
                {
                    string[] slt = lstStr[i].Split('=', '\t');
                    if (slt.Length < 1) continue;
                    switch (slt[0])
                    {
                        //case "DrawSpeed": try
                        //    { DrawSpeed = Convert.ToInt32(slt[1]); }
                        //    catch { DrawSpeed = 0; } break;
                        //case "JumpSpeed": try
                        //    { JumpSpeed = Convert.ToInt32(slt[1]); }
                        //    catch { JumpSpeed = 0; } break;
                        //case "WobbleDiameter": try
                        //    { WobbleDiameter = Convert.ToInt32(slt[1]); }
                        //    catch { WobbleDiameter = 0; } break;
                        //case "WobbleOverlap": try
                        //    { WobbleOverlap = Convert.ToInt32(slt[1]); }
                        //    catch { WobbleOverlap = 0; } break;
                        //case "BroadwayIn": try
                        //    { BroadwayIn = Convert.ToInt32(slt[1]); }
                        //    catch { BroadwayIn = 0; } break;
                        //case "BroadwayOut": try
                        //    { BroadwayOut = Convert.ToInt32(slt[1]); }
                        //    catch { BroadwayOut = 0; } break;
                        //case "SlowJump": try
                        //    { SlowJump = Convert.ToInt32(slt[1]); }
                        //    catch { SlowJump = 0; } break;
                        case "DrawStep": try
                            { DrawStep = Convert.ToInt32(slt[1]); }
                            catch { DrawStep = 0; } break;
                        case "JumpStep": try
                            { JumpStep = Convert.ToInt32(slt[1]); }
                            catch { JumpStep = 0; } break;
                        case "StepPeriod": try
                            { StepPeriod = Convert.ToInt32(slt[1]); }
                            catch { StepPeriod = 0; } break;
                        case "CornerDelay": try
                            { CornerDelay = Convert.ToInt32(slt[1]); }
                            catch { CornerDelay = 0; } break;
                        case "LaserOnDelay": try
                            { LaserOnDelay = Convert.ToInt32(slt[1]); }
                            catch { LaserOnDelay = 0; } break;
                        case "LaserOffDelay": try
                            { LaserOffDelay = Convert.ToInt32(slt[1]); }
                            catch { LaserOffDelay = 0; } break;
                        case "FPSDelay": try
                            { FPSDelay = Convert.ToInt32(slt[1]); }
                            catch { FPSDelay = 0; } break;
                        case "JumpDelay": try
                            { JumpDelay = Convert.ToInt32(slt[1]); }
                            catch { JumpDelay = 0; } break;
                        case "LineDelay": try
                            { LineDelay = Convert.ToInt32(slt[1]); }
                            catch { LineDelay = 0; } break;
                        case "Frequency": try
                            { Frequency = Convert.ToInt32(slt[1]); }
                            catch { Frequency = 0; } break;
                        case "PulseDuty": try
                            { PulseDuty = Convert.ToInt32(slt[1]); }
                            catch { PulseDuty = 0; } break;
                        case "LampCurrent": try
                            { LampCurrent = Convert.ToDouble(slt[1]); }
                            catch { LampCurrent = 0; } break;
                        case "AACValue": try
                            { AACValue = Convert.ToInt32(slt[1]); }
                            catch { AACValue = 0; } break;
                        case "SettingPower": try
                            { SettingPower = Convert.ToDouble(slt[1]); }
                            catch { SettingPower = 0; } break;
                        case "ModeLampCurrent": try
                            { ModeLampCurrent = Convert.ToInt32(slt[1]); }
                            catch { ModeLampCurrent = 0; } break;
                        case "NumMark": try
                            { NumMark = Convert.ToInt32(slt[1]); }
                            catch { NumMark = 0; } break;
                        case "FirstLaserOnDelay": try
                            { FirstLaserOnDelay = Convert.ToInt32(slt[1]); }
                            catch { FirstLaserOnDelay = 0; } break;
                        case "Custom1": try
                            { Custom1 = Convert.ToInt32(slt[1]); }
                            catch { Custom1 = 0; } break;
                        case "Custom2": try
                            { Custom2 = Convert.ToInt32(slt[1]); }
                            catch { Custom2 = 0; } break;
                        case "Custom3": try
                            { Custom3 = Convert.ToInt32(slt[1]); }
                            catch { Custom3 = 0; } break;
                        case "Custom4": try
                            { Custom4 = Convert.ToInt32(slt[1]); }
                            catch { Custom4 = 0; } break;
                        case "ThermalTrack": try
                            { ThermalTrack = Convert.ToInt32(slt[1]); }
                            catch { ThermalTrack = 0; } break;
                        //case "CurrPower": try
                        //    { CurrPower = Convert.ToDouble(slt[1]); }
                        //    catch { CurrPower = 0; } break;
                        //case "LowerThreshold": try
                        //    { LowerThreshold = Convert.ToInt32(slt[1]); }
                        //    catch { LowerThreshold = 0; } break;
                    }
                }
                Loaded = true;
            }
            else
            {
                Path = path;
                //DrawSpeed = 0;
                //JumpSpeed = 0;
                //WobbleDiameter = 0;
                //WobbleOverlap = 0;
                //BroadwayIn = 0;
                //BroadwayOut = 0;
                //SlowJump = 0;
                DrawStep = 0;
                JumpStep = 0;
                StepPeriod = 0;
                CornerDelay = 0;
                LaserOnDelay = 0;
                LaserOffDelay = 0;
                FPSDelay = 0;
                JumpDelay = 0;
                LineDelay = 0;
                Frequency = 0;
                PulseDuty = 0;
                LampCurrent = 0.0;
                AACValue = 0;
                SettingPower = 0.0;
                ModeLampCurrent = 1;
                NumMark = 1;
                FirstLaserOnDelay = 0;
                Custom1 = 99;
                Custom2 = 0;
                Custom3 = 0;
                Custom4 = 0;
                ThermalTrack = 0;
                //CurrPower = 0.0;
                //LowerThreshold = 0;
                Loaded = true;
            }
        }

        public bool Save()
        {
            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch
                { return false; }
            }
            try
            {
                FileStream fs = File.Create(Path); fs.Close();
            }
            catch { return false; }
            try
            {
                string[] str = new string[23];
                str[0] = "// MiME Parameter File Ver0.2";
                //str[1] = "DrawSpeed=1000";
                //str[2] = "JumpSpeed=1000";
                //str[3] = "WobbleDiameter=0";
                //str[4] = "WobbleOverlap=0";
                //str[5] = "BroadwayIn=0";
                //str[6] = "BroadwayOut=0";
                //str[7] = "SlowJump=0";
                str[1] = "DrawStep=" + DrawStep.ToString();
                str[2] = "JumpStep=" + JumpStep.ToString();
                str[3] = "StepPeriod=" + StepPeriod.ToString();
                str[4] = "CornerDelay=" + CornerDelay.ToString();
                str[5] = "LaserOnDelay=" + LaserOnDelay.ToString();
                str[6] = "LaserOffDelay=" + LaserOffDelay.ToString();
                str[7] = "FPSDelay=185";
                str[8] = "JumpDelay=" + JumpDelay.ToString();
                str[9] = "LineDelay=" + LineDelay.ToString();
                str[10] = "Frequency=" + Frequency.ToString();
                str[11] = "PulseDuty=" + PulseDuty.ToString();
                str[12] = "LampCurrent=" + LampCurrent.ToString("f6");
                str[13] = "AACValue=0";
                if (SettingPower == 0.0)
                    str[14] = "SettingPower=Not Defined";
                else
                    str[14] = "SettingPower=" + SettingPower.ToString();
                str[15] = "ModeLampCurrent=1";
                str[16] = "NumMark=1";
                str[17] = "FirstLaserOnDelay=0";
                str[18] = "Custom1=0";
                str[19] = "Custom2=0";
                str[20] = "Custom3=0";
                str[21] = "Custom4=0";
                str[22] = "ThermalTrack=0";
                //if (CurrPower == 0.0)
                //    str[30] = "CurrPower=Not Defined";
                //else str[30] = "CurrPower=3.1";
                //str[31] = "LowerThreshold=0";
                File.WriteAllLines(Path, str);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }

    public class IDMARK
    {
        public string Contents;
        private const string ObjType = "2DBARCODE";
        private int IsSelect = 0;
        private int IsUpdate = 1;
        private double AxisField = 180.000000;
        public int UNIT = 1;
        public double DispLeft = -0.070;
        public double DispRight = 0.530;
        public double DispTop = -0.070;
        public double DispBottom = 0.530;
        public Int32Rect DispRect = new Int32Rect(-38, -38, 289, 289);
        public double MatrixA = 0.000000;
        public double MatrixB = 1.496539;
        public double MatrixC = 1.503404;
        public double MatrixD = 0.000000;
        public double MatrixX = 125.605469;
        public double MatrixY = 125.605469;
        public double MatrixZ = 1.000000;
        public double BasicMatrixA = 1.000000;
        public double BasicMatrixB = 0.000000;
        public double BasicMatrixC = 0.000000;
        public double BasicMatrixD = -1.000000;
        public double BasicMatrixX = 125.605469;
        public double BasicMatrixY = 125.605469;
        public double BasicMatrixZ = 1.000000;
        public double RotateMatrixA = 0.000000;
        public double RotateMatrixB = -1.000000;
        public double RotateMatrixC = 1.000000;
        public double RotateMatrixD = 0.000000;
        public double RotateMatrixX = 0.003906;
        public double RotateMatrixY = 0.003906;
        public double RotateMatrixZ = 1.000000;
        public double RotateAngle = 90.000000;
        public int ParaNumber = 2;
        public int LockObject = 0;

        public int SpecialType = 0;
        public int ReferenceNumber1 = 0;
        public int ReferenceNumber2 = 0;
        public int LogoAlignMode = 0;
        public IDMARK(string[] astr)
        {
            //ObjType = "HPGL";
            for (int i = 0; i < astr.Length; i++)
            {
                string[] slt = astr[i].Split('=', '\t');
                var val = slt.Where(item => !string.IsNullOrEmpty(item));
                slt = val.ToArray<string>();
                if (slt.Length > 0)
                {
                    switch (slt[0])
                    {
                        case "Contents": Contents = slt[1]; break;
                        case "IsSelect": IsSelect = Convert.ToInt32(slt[1]); break;
                        case "IsUpdate": IsUpdate = Convert.ToInt32(slt[1]); break;
                        case "AxisField": AxisField = Convert.ToDouble(slt[1]); break;
                        case "UNIT": UNIT = Convert.ToInt32(slt[1]); break;
                        case "DispLeft": DispLeft = Convert.ToDouble(slt[1]); break;
                        case "DispRight": DispRight = Convert.ToDouble(slt[1]); break;
                        case "DispTop": DispTop = Convert.ToDouble(slt[1]); break;
                        case "DispBottom": DispBottom = Convert.ToDouble(slt[1]); break;
                        case "DispRect":
                            string[] ttt = slt[1].Split(',');
                            if (ttt.Length >= 4)
                            {
                                DispRect = new Int32Rect(Convert.ToInt32(ttt[0]), Convert.ToInt32(ttt[1]), Convert.ToInt32(ttt[2]), Convert.ToInt32(ttt[3]));
                            }
                            else DispRect = new Int32Rect(1829, 2239, 2375, 3877);
                            break;
                        case "MatrixA": MatrixA = Convert.ToDouble(slt[1]); break;
                        case "MatrixB": MatrixB = Convert.ToDouble(slt[1]); break;
                        case "MatrixC": MatrixC = Convert.ToDouble(slt[1]); break;
                        case "MatrixD": MatrixD = Convert.ToDouble(slt[1]); break;
                        case "MatrixX": MatrixX = Convert.ToDouble(slt[1]); break;
                        case "MatrixY": MatrixY = Convert.ToDouble(slt[1]); break;
                        case "MatrixZ": MatrixZ = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixA": BasicMatrixA = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixB": BasicMatrixB = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixC": BasicMatrixC = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixD": BasicMatrixD = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixX": BasicMatrixX = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixY": BasicMatrixY = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixZ": BasicMatrixZ = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixA": RotateMatrixA = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixB": RotateMatrixB = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixC": RotateMatrixC = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixD": RotateMatrixD = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixX": RotateMatrixX = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixY": RotateMatrixY = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixZ": RotateMatrixZ = Convert.ToDouble(slt[1]); break;
                        case "RotateAngle": RotateAngle = Convert.ToDouble(slt[1]); break;
                        case "ParaNumber": ParaNumber = Convert.ToInt32(slt[1]); break;
                        case "LockObject": LockObject = Convert.ToInt32(slt[1]); break;
                        case "Special Type": SpecialType = Convert.ToInt32(slt[1]); break;
                        case "Reference Number 1": ReferenceNumber1 = Convert.ToInt32(slt[1]); break;
                        case "Reference Number 2": ReferenceNumber2 = Convert.ToInt32(slt[1]); break;
                        case "LogoAlignMode": LogoAlignMode = Convert.ToInt32(slt[1]); break;
                    }
                }
            }
        }
        public IDMARK()
        {

        }
    }

    public class HPGL
    {
        public string FileName;
        private const string ObjType = "HPGL";
        private int IsSelect = 0;
        private int IsUpdate = 1;
        private double AxisField = 180.000000;
        public int UNIT = 1;
        public double DispLeft = -0.070;
        public double DispRight = 0.530;
        public double DispTop = -0.070;
        public double DispBottom = 0.530;
        public Int32Rect DispRect = new Int32Rect(-38, -38, 289, 289);
        public double MatrixA = 0.000000;
        public double MatrixB = 1.496539;
        public double MatrixC = 1.503404;
        public double MatrixD = 0.000000;
        public double MatrixX = 125.605469;
        public double MatrixY = 125.605469;
        public double MatrixZ = 1.000000;
        public double BasicMatrixA = 1.000000;
        public double BasicMatrixB = 0.000000;
        public double BasicMatrixC = 0.000000;
        public double BasicMatrixD = -1.000000;
        public double BasicMatrixX = 125.605469;
        public double BasicMatrixY = 125.605469;
        public double BasicMatrixZ = 1.000000;
        public double RotateMatrixA = 0.000000;
        public double RotateMatrixB = -1.000000;
        public double RotateMatrixC = 1.000000;
        public double RotateMatrixD = 0.000000;
        public double RotateMatrixX = 0.003906;
        public double RotateMatrixY = 0.003906;
        public double RotateMatrixZ = 1.000000;
        public double RotateAngle = 90.000000;
        public int ParaNumber = 2;
        public int LockObject = 0;

        public int SpecialType = 0;
        public int ReferenceNumber1 = 0;
        public int ReferenceNumber2 = 0;
        public int LogoAlignMode = 0;
        public HPGL(string[] astr)
        {
            //ObjType = "HPGL";
            for (int i = 0; i < astr.Length; i++)
            {
                string[] slt = astr[i].Split('=', '\t');
                var val = slt.Where(item => !string.IsNullOrEmpty(item));
                slt = val.ToArray<string>();
                if (slt.Length > 0)
                {
                    switch (slt[0])
                    {
                        case "FileName": FileName = slt[1]; break;
                        case "IsSelect": IsSelect = Convert.ToInt32(slt[1]); break;
                        case "IsUpdate": IsUpdate = Convert.ToInt32(slt[1]); break;
                        case "AxisField": AxisField = Convert.ToDouble(slt[1]); break;
                        case "UNIT": UNIT = Convert.ToInt32(slt[1]); break;
                        case "DispLeft": DispLeft = Convert.ToDouble(slt[1]); break;
                        case "DispRight": DispRight = Convert.ToDouble(slt[1]); break;
                        case "DispTop": DispTop = Convert.ToDouble(slt[1]); break;
                        case "DispBottom": DispBottom = Convert.ToDouble(slt[1]); break;
                        case "DispRect":
                            string[] ttt = slt[1].Split(',');
                            if (ttt.Length >= 4)
                            {
                                DispRect = new Int32Rect(Convert.ToInt32(ttt[0]), Convert.ToInt32(ttt[1]), Convert.ToInt32(ttt[2]), Convert.ToInt32(ttt[3]));
                            }
                            else DispRect = new Int32Rect(1829, 2239, 2375, 3877);
                            break;
                        case "MatrixA": MatrixA = Convert.ToDouble(slt[1]); break;
                        case "MatrixB": MatrixB = Convert.ToDouble(slt[1]); break;
                        case "MatrixC": MatrixC = Convert.ToDouble(slt[1]); break;
                        case "MatrixD": MatrixD = Convert.ToDouble(slt[1]); break;
                        case "MatrixX": MatrixX = Convert.ToDouble(slt[1]); break;
                        case "MatrixY": MatrixY = Convert.ToDouble(slt[1]); break;
                        case "MatrixZ": MatrixZ = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixA": BasicMatrixA = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixB": BasicMatrixB = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixC": BasicMatrixC = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixD": BasicMatrixD = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixX": BasicMatrixX = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixY": BasicMatrixY = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixZ": BasicMatrixZ = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixA": RotateMatrixA = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixB": RotateMatrixB = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixC": RotateMatrixC = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixD": RotateMatrixD = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixX": RotateMatrixX = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixY": RotateMatrixY = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixZ": RotateMatrixZ = Convert.ToDouble(slt[1]); break;
                        case "RotateAngle": RotateAngle = Convert.ToDouble(slt[1]); break;
                        case "ParaNumber": ParaNumber = Convert.ToInt32(slt[1]); break;
                        case "LockObject": LockObject = Convert.ToInt32(slt[1]); break;
                        case "Special Type": SpecialType = Convert.ToInt32(slt[1]); break;
                        case "Reference Number 1": ReferenceNumber1 = Convert.ToInt32(slt[1]); break;
                        case "Reference Number 2": ReferenceNumber2 = Convert.ToInt32(slt[1]); break;
                        case "LogoAlignMode": LogoAlignMode = Convert.ToInt32(slt[1]); break;
                    }
                }
            }
        }
        public HPGL()
        {

        }
    }

    public class TEXT
    {
        public string Content = "";
        public int MaxNum = 1000;
        public int AlignChar = 1;
        public int AlignMode = 0;
        public double CapitalHeight = 3.1942;
        public double CharGap = 0.0400;
        public string FontName = "";
        public double Height = 4.0528;
        public double LineGap = 1.0131;
        public double Width = 1.1000;
        public double SpaceSize = 3.0000;
        public int UseCapitalLineGap = 0;
        public int UseFixStringLenght = 0;
        public int UseLeftGapofeachChar = 0;
        public int UseFixNumberofChar = 0;
        public int FixedStrSize = 1000;
        public int FixNumberofChar = 0;
        public int ItalicAngle = 0;
        public int NTMark_WidthStype = 0;
        public int UseStringCutting = 0;
        public int First = 0;
        public int Count = 0;
        public int UseCriterionChar = 0;
        public char CriterionChar = 'W';
        public int UseFixStringLenToCurrent = 0;
        /// [SERIAL_ATTRIBUTE]
        public int Style = 0;
        public int IncType = 0;
        public int StartNumber = 0;
        public int RangeStartNumber = 0;
        public int RangeEndNumber = 0;
        public string PreFix = "";
        public string PostFix = "";
        public int MaxCharSize = 5;
        public int SavedCurrentNumber = 0;
        public int SCHEME_Order = 0;
        public int SCHEME_Position = 0;
        public int SCHEME_Type = 1;
        public int SCHEME_Numbering = 0;
        public int SaveFileWhenUpdate = 0;
        public int UpdateStartNumAsCurrNum = 0;
        public int NumericSystemType = 0;
        public int IncreaseLevels = 1;
        //		[SPECIAL_TYPE]
        public int SpecialType = 2;
        public int ReferenceNumber1 = 3;
        public int ReferenceNumber2 = 0;
        //[Attribute]
        private const string ObjType = "TEXT";
        public int IsSelect = 0;
        public int IsUpdate = 0;
        public double AxisField = 120.000000;
        public int UNIT = 1;
        public double DispLeft = 3.184;
        public double DispRight = 6.184;
        public double DispTop = 27.274;
        public double DispBottom = 30.360;
        public Int32Rect DispRect = new Int32Rect(1738, 14895, 3377, 16580);
        public double MatrixA = -1.000000;
        public double MatrixB = 0.000000;
        public double MatrixC = 0.000000;
        public double MatrixD = 1.000000;
        public double MatrixX = 2558.074219;
        public double MatrixY = 15737.699219;
        public double MatrixZ = 1.000000;
        public double BasicMatrixA = 1.000000;
        public double BasicMatrixB = 0.000000;
        public double BasicMatrixC = 0.000000;
        public double BasicMatrixD = -1.000000;
        public double BasicMatrixX = 2558.074219;
        public double BasicMatrixY = 15737.699219;
        public double BasicMatrixZ = 1.000000;
        public double RotateMatrixA = -1.000000;
        public double RotateMatrixB = 0.000000;
        public double RotateMatrixC = -0.000000;
        public double RotateMatrixD = -1.000000;
        public double RotateMatrixX = 0.003906;
        public double RotateMatrixY = 0.003906;
        public double RotateMatrixZ = 1.000000;
        public double RotateAngle = 180.000000;
        public int ParaNumber = 4;
        public int LockObject = 0;
        public TEXT(string[] astr)
        {
            //ObjType = "TEXT";
            for (int i = 0; i < astr.Length; i++)
            {
                string[] slt = astr[i].Split('=', '\t');
                var val = slt.Where(item => !string.IsNullOrEmpty(item));
                slt = val.ToArray<string>();
                if (slt.Length > 0)
                {
                    switch (slt[0])
                    {
                        case "Content":
                            try
                            {
                                Content = slt[1];
                            }
                            catch { Content = " "; }
                            break;
                        case "MaxNum": MaxNum = Convert.ToInt32(slt[1]); break;
                        case "AlignChar": AlignChar = Convert.ToInt32(slt[1]); break;
                        case "AlignMode": AlignMode = Convert.ToInt32(slt[1]); break;
                        case "CapitalHeight":
                            CapitalHeight = Convert.ToDouble(slt[1]); break;
                        case "CharGap": CharGap = Convert.ToDouble(slt[1]); break;
                        case "FontName": FontName = slt[1]; break;
                        case "Height":
                            Height = Convert.ToDouble(slt[1]); break;
                        case "LineGap": LineGap = Convert.ToDouble(slt[1]); break;
                        case "Width": Width = Convert.ToDouble(slt[1]); break;
                        case "SpaceSize": SpaceSize = Convert.ToDouble(slt[1]); break;
                        case "UseCapitalLineGap": UseCapitalLineGap = Convert.ToInt32(slt[1]); break;
                        case "UseFixStringLenght": UseFixStringLenght = Convert.ToInt32(slt[1]); break;
                        case "UseLeftGapofeachChar": UseLeftGapofeachChar = Convert.ToInt32(slt[1]); break;
                        case "UseFixNumberofChar": UseFixNumberofChar = Convert.ToInt32(slt[1]); break;
                        case "FixedStrSize": FixedStrSize = Convert.ToInt32(slt[1]); break;
                        case "FixNumberofChar": FixNumberofChar = Convert.ToInt32(slt[1]); break;
                        case "ItalicAngle": ItalicAngle = Convert.ToInt32(slt[1]); break;
                        case "NTMark_WidthStype": NTMark_WidthStype = Convert.ToInt32(slt[1]); break;
                        case "UseStringCutting": UseStringCutting = Convert.ToInt32(slt[1]); break;
                        case "First": First = Convert.ToInt32(slt[1]); break;
                        case "Count": Count = Convert.ToInt32(slt[1]); break;
                        case "UseCriterionChar": UseCriterionChar = Convert.ToInt32(slt[1]); break;
                        case "CriterionChar": CriterionChar = slt[1][0]; break;
                        case "UseFixStringLenToCurrent": UseFixStringLenToCurrent = Convert.ToInt32(slt[1]); break;
                        case "Style": Style = Convert.ToInt32(slt[1]); break;
                        case "IncType": IncType = Convert.ToInt32(slt[1]); break;
                        case "StartNumber": StartNumber = Convert.ToInt32(slt[1]); break;
                        case "RangeStartNumber": RangeStartNumber = Convert.ToInt32(slt[1]); break;
                        case "RangeEndNumber": RangeEndNumber = Convert.ToInt32(slt[1]); break;
                        case "PreFix":
                            try
                            {
                                PreFix = slt[1];
                            }
                            catch { PreFix = ""; }
                            break;
                        case "PostFix":
                            try
                            {
                                PostFix = slt[1];
                            }
                            catch { PostFix = ""; }
                            break;
                        case "MaxCharSize": MaxCharSize = Convert.ToInt32(slt[1]); break;
                        case "SavedCurrentNumber": SavedCurrentNumber = Convert.ToInt32(slt[1]); break;
                        case "SCHEME_Order": SCHEME_Order = Convert.ToInt32(slt[1]); break;
                        case "SCHEME_Position": SCHEME_Position = Convert.ToInt32(slt[1]); break;
                        case "SCHEME_Type": SCHEME_Type = Convert.ToInt32(slt[1]); break;
                        case "SCHEME_Numbering": SCHEME_Numbering = Convert.ToInt32(slt[1]); break;
                        case "SaveFileWhenUpdate": SaveFileWhenUpdate = Convert.ToInt32(slt[1]); break;
                        case "UpdateStartNumAsCurrNum": UpdateStartNumAsCurrNum = Convert.ToInt32(slt[1]); break;
                        case "NumericSystemType": NumericSystemType = Convert.ToInt32(slt[1]); break;
                        case "IncreaseLevels": IncreaseLevels = Convert.ToInt32(slt[1]); break;
                        case "Special Type": SpecialType = Convert.ToInt32(slt[1]); break;
                        case "Reference Number 1": ReferenceNumber1 = Convert.ToInt32(slt[1]); break;
                        case "Reference Number 2": ReferenceNumber2 = Convert.ToInt32(slt[1]); break;

                        case "IsSelect": IsSelect = Convert.ToInt32(slt[1]); break;
                        case "IsUpdate": IsUpdate = Convert.ToInt32(slt[1]); break;
                        case "IsSeAxisFieldlect": AxisField = Convert.ToDouble(slt[1]); break;
                        case "UNIT": UNIT = Convert.ToInt32(slt[1]); break;
                        case "DispLeft": DispLeft = Convert.ToDouble(slt[1]); break;
                        case "DispRight": DispRight = Convert.ToDouble(slt[1]); break;
                        case "DispTop": DispTop = Convert.ToDouble(slt[1]); break;
                        case "DispBottom": DispBottom = Convert.ToDouble(slt[1]); break;
                        case "DispRect": DispRect = new Int32Rect(1738, 14895, 3377, 16580); break;
                        case "MatrixA": MatrixA = Convert.ToDouble(slt[1]); break;
                        case "MatrixB": MatrixB = Convert.ToDouble(slt[1]); break;
                        case "MatrixC": MatrixC = Convert.ToDouble(slt[1]); break;
                        case "MatrixD": MatrixD = Convert.ToDouble(slt[1]); break;
                        case "MatrixX": MatrixX = Convert.ToDouble(slt[1]); break;
                        case "MatrixY": MatrixY = Convert.ToDouble(slt[1]); break;
                        case "MatrixZ": MatrixZ = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixA": BasicMatrixA = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixB": BasicMatrixB = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixC": BasicMatrixC = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixD": BasicMatrixD = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixX": BasicMatrixX = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixY": BasicMatrixY = Convert.ToDouble(slt[1]); break;
                        case "BasicMatrixZ": BasicMatrixZ = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixA": RotateMatrixA = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixB": RotateMatrixB = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixC": RotateMatrixC = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixD": RotateMatrixD = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixX": RotateMatrixX = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixY": RotateMatrixY = Convert.ToDouble(slt[1]); break;
                        case "RotateMatrixZ": RotateMatrixZ = Convert.ToDouble(slt[1]); break;
                        case "RotateAngle": RotateAngle = Convert.ToDouble(slt[1]); break;
                        case "ParaNumber": ParaNumber = Convert.ToInt32(slt[1]); break;
                        case "LockObject": LockObject = Convert.ToInt32(slt[1]); break;
                    }
                }
            }
        }
        public TEXT()
        {
        }
    }

    public class MarkParams
    {
        public string filename;
        public int Mode;
        public int Selection;
        public bool RailIrr;
        public bool ZeroMark;
        public bool FlipChip;
        public int Step;
        public double StepPitch;
        public int StepUnits;
        public int UnitRow;
        public int UnitColumn;
        public double UnitWidth;
        public double UnitHeight;
        public int Block;
        public double BlockGap;

        public int Unit;
        public int Rail;
        public int Week;
        public int WeekLoc;
        public int Count;
        public int CountLoc;
        public int CountRW;
        public int ID_Count;
        public bool ID_Text;
        public int WeekMarkType;
        public double Shift1;
        public double Shift2;

        public MarkParams()
        {

        }
    }

    public class ModelMarkInfo
    {
        public int ID;
        public string ModelName;
        public int Mode;
        public int Selection;
        public bool RailIrr;
        public bool FlipChip;
        public int Step;
        public double StepPitch;
        public int StepUnits;
        public int UnitRow;
        public int UnitColumn;
        public double UnitWidth;
        public double UnitHeight;
        public int Block;
        public double BlockGap;

        public int Unit;
        public int Rail;
        public int Week;
        public int WeekLoc;
        public int Count;
        public int CountLoc;
        public int CountRW;
        public int IDMark;
        public int ID_Count;
        public bool ID_Text;
        public int WeekMarkType;

        public Point Pos;
        public Point UPos;
        public double Shift1;
        public double Shift2;


        public ModelMarkInfo(int step, double steppitch, int stepunits, int unitrow, int unitcol, double unitwidth, double unitheight, int block, double blockgap)
        {
            Step = step;
            StepPitch = steppitch;
            StepUnits = stepunits;
            UnitRow = unitrow;
            UnitColumn = unitcol;
            UnitWidth = unitwidth;
            UnitHeight = unitheight;
            Block = block;
            BlockGap = blockgap;
        }
        public ModelMarkInfo()
        {
        }
    }

    public class MarkLogo
    {
        string path;
        public List<string> UnitRect = new List<string>();
        public List<string> RailRect = new List<string>();
        public List<string> UnitEllipse = new List<string>();
        public List<string> RailEllipse = new List<string>();
        public List<string> UnitTriangle = new List<string>();
        public List<string> UnitCross = new List<string>();
        public List<string> UnitLine = new List<string>();
        public List<string> RailLine = new List<string>();
        public List<string> FontFile = new List<string>();
        public List<string> FontRemark = new List<string>();
        public List<double> FontScale = new List<double>();
        public List<double> FontRScale = new List<double>();
        public MarkLogo()
        {
            path = System.IO.Directory.GetCurrentDirectory() + "\\..\\MarkFile\\PLT_INFO.ini";
            IniFile ini = new IniFile(path);
            string str = "11111";
            int i = 1;
            UnitRect.Clear();
            while (str != "")
            {
                str = ini.ReadString("UNIT RECT", "UR" + i.ToString(), "");
                i++;
                if (str == "") continue;
                UnitRect.Add(str);
            }
            i = 1;
            str = "11111";
            RailRect.Clear();
            while (str != "")
            {
                str = ini.ReadString("RAIL RECT", "RR" + i.ToString(), "");
                i++;

                if (str == "") continue;
                RailRect.Add(str);
            }
            i = 1;
            str = "11111";
            UnitEllipse.Clear();
            while (str != "")
            {
                str = ini.ReadString("UNIT ELLIPSE", "UE" + i.ToString(), "");
                i++;
                if (str == "") continue;
                UnitEllipse.Add(str);
            }
            i = 1;
            str = "11111";
            RailEllipse.Clear();
            while (str != "")
            {
                str = ini.ReadString("RAIL ELLIPSE", "RE" + i.ToString(), "");
                i++;
                if (str == "") continue;
                RailEllipse.Add(str);
            }
            i = 1;
            str = "11111";
            UnitTriangle.Clear();
            while (str != "")
            {
                str = ini.ReadString("UNIT TRIANGLE", "UT" + i.ToString(), "");
                i++;
                if (str == "") continue;
                UnitTriangle.Add(str);
            }
            i = 1;
            str = "11111";
            UnitCross.Clear();
            while (str != "")
            {
                str = ini.ReadString("UNIT CROSS", "UC" + i.ToString(), "");
                i++;
                if (str == "") continue;
                UnitCross.Add(str);
            }
            i = 1;
            str = "11111";
            UnitLine.Clear();
            while (str != "")
            {
                str = ini.ReadString("UNIT LINE", "UL" + i.ToString(), "");
                i++;
                if (str == "") continue;
                UnitLine.Add(str);
            }
            i = 1;
            str = "11111";
            RailLine.Clear();
            while (str != "")
            {
                str = ini.ReadString("UNIT LINE", "RR" + i.ToString(), "");
                i++;
                if (str == "") continue;
                RailLine.Add(str);
            }
            i = 1;
            str = "11111";
            FontFile.Clear();
            FontScale.Clear();
            double a = 0.0;
            while (str != "")
            {
                str = ini.ReadString("FONT", "FT" + i.ToString(), "");
                a = ini.ReadDouble("FONT", "FS" + i.ToString(), 0.0);
                i++;
                if (str == "") continue;
                FontFile.Add(str);
                FontScale.Add(a);
            }
            i = 1;
            str = "11111";
            FontRemark.Clear();
            while (str != "")
            {
                str = ini.ReadString("FONT REMARK", "FR" + i.ToString(), "");
                a = ini.ReadDouble("FONT REMARK", "FS" + i.ToString(), 0.0);
                i++;
                if (str == "") continue;
                FontRemark.Add(str);
                FontRScale.Add(a);
            }
        }
        public void Save()
        {
            path = System.IO.Directory.GetCurrentDirectory() + "\\..\\MarkFile\\PLT_INFO.ini";
            IniFile ini = new IniFile(path);
            for (int i = 0; i < UnitRect.Count; i++)
                ini.WriteString("UNIT RECT", "UR" + (i + 1).ToString(), UnitRect[i]);
            for (int i = 0; i < RailRect.Count; i++)
                ini.WriteString("RAIL RECT", "RR" + (i + 1).ToString(), RailRect[i]);
            for (int i = 0; i < UnitEllipse.Count; i++)
                ini.WriteString("UNIT ELLIPSE", "UE" + (i + 1).ToString(), UnitEllipse[i]);
            for (int i = 0; i < RailEllipse.Count; i++)
                ini.WriteString("RAIL ELLIPSE", "RE" + (i + 1).ToString(), RailEllipse[i]);
            for (int i = 0; i < UnitTriangle.Count; i++)
                ini.WriteString("UNIT TRIANGLE", "UT" + (i + 1).ToString(), UnitTriangle[i]);
            for (int i = 0; i < UnitCross.Count; i++)
                ini.WriteString("UNIT CROSS", "UC" + (i + 1).ToString(), UnitCross[i]);
            for (int i = 0; i < UnitLine.Count; i++)
                ini.WriteString("UNIT LINE", "UL" + (i + 1).ToString(), UnitLine[i]);
            for (int i = 0; i < RailLine.Count; i++)
                ini.WriteString("RAIL LINE", "RL" + (i + 1).ToString(), RailLine[i]);
            for (int i = 0; i < FontFile.Count; i++)
                ini.WriteString("FONT", "FT" + (i + 1).ToString(), FontFile[i]);
            for (int i = 0; i < FontRemark.Count; i++)
                ini.WriteString("FONT REMARK", "FR" + (i + 1).ToString(), FontRemark[i]);
        }
    }

    public class Matrix
    {
        public double MatrixA = 0.000000;
        public double MatrixB = 1.496539;
        public double MatrixC = 1.503404;
        public double MatrixD = 0.000000;
        public double MatrixX = 125.605469;
        public double MatrixY = 125.605469;
        public double MatrixZ = 1.000000;
        public double BasicMatrixA = 1.000000;
        public double BasicMatrixB = 0.000000;
        public double BasicMatrixC = 0.000000;
        public double BasicMatrixD = -1.000000;
        public double BasicMatrixX = 125.605469;
        public double BasicMatrixY = 125.605469;
        public double BasicMatrixZ = 1.000000;
        public double RotateMatrixA = 0.000000;
        public double RotateMatrixB = -1.000000;
        public double RotateMatrixC = 1.000000;
        public double RotateMatrixD = 0.000000;
        public double RotateMatrixX = 0.003906;
        public double RotateMatrixY = 0.003906;
        public double RotateMatrixZ = 1.000000;
        public Matrix(int angle, double left, double top, double right, double bottom)
        {
            double cpx = right - (right - left) / 2.0;
            double cpy = bottom - (bottom - top) / 2.0;
            cpy = cpy / (120.0 / 65535);
            cpx = cpx / (120.0 / 65535);
            switch (angle)
            {
                case 0:
                    MatrixA = 1.000000;
                    MatrixB = 0.000000;
                    MatrixC = 0.000000;
                    MatrixD = -1.000000;
                    MatrixX = cpx;
                    MatrixY = cpy;
                    MatrixZ = 1.000000;
                    BasicMatrixA = 1.000000;
                    BasicMatrixB = 0.000000;
                    BasicMatrixC = 0.000000;
                    BasicMatrixD = -1.000000;
                    BasicMatrixX = cpx;
                    BasicMatrixY = cpy;
                    BasicMatrixZ = 1.000000;
                    RotateMatrixA = 0.000000;
                    RotateMatrixB = 1.000000;
                    RotateMatrixC = -1.000000;
                    RotateMatrixD = 0.000000;
                    RotateMatrixX = 0.0039060;
                    RotateMatrixY = 0.0039060;
                    RotateMatrixZ = 1.000000;
                    break;
                case 90:
                    MatrixA = 0.000000;
                    MatrixB = 1.000000;
                    MatrixC = 1.000000;
                    MatrixD = -0.000000;
                    MatrixX = cpx;
                    MatrixY = cpy;
                    MatrixZ = 1.000000;
                    BasicMatrixA = 1.000000;
                    BasicMatrixB = 0.000000;
                    BasicMatrixC = 0.000000;
                    BasicMatrixD = -1.000000;
                    BasicMatrixX = cpx;
                    BasicMatrixY = cpy;
                    BasicMatrixZ = 1.000000;
                    RotateMatrixA = 0.000000;
                    RotateMatrixB = 1.000000;
                    RotateMatrixC = -1.000000;
                    RotateMatrixD = 0.000000;
                    RotateMatrixX = 0.0039060;
                    RotateMatrixY = 0.0039060;
                    RotateMatrixZ = 1.000000;
                    break;
                case 180:
                    MatrixA = -1.000000;
                    MatrixB = 0.000000;
                    MatrixC = 0.000000;
                    MatrixD = 1.000000;
                    MatrixX = cpx;
                    MatrixY = cpy;
                    MatrixZ = 1.000000;
                    BasicMatrixA = 1.000000;
                    BasicMatrixB = 0.000000;
                    BasicMatrixC = 0.000000;
                    BasicMatrixD = -1.000000;
                    BasicMatrixX = cpx;
                    BasicMatrixY = cpy;
                    BasicMatrixZ = 1.000000;
                    RotateMatrixA = -1.000000;
                    RotateMatrixB = 0.000000;
                    RotateMatrixC = 0.000000;
                    RotateMatrixD = -1.000000;
                    RotateMatrixX = 0.003906;
                    RotateMatrixY = 0.003906;
                    RotateMatrixZ = 1.000000;
                    //RotateMatrixA = 0.000000;
                    //RotateMatrixB = 1.000000;
                    //RotateMatrixC = -1.000000;
                    //RotateMatrixD = 0.000000;
                    //RotateMatrixX = 0.0039060;
                    //RotateMatrixY = 0.0039060;
                    //RotateMatrixZ = 1.000000;
                    break;
                case 270:
                    MatrixA = -0.000000;
                    MatrixB = -1.000000;
                    MatrixC = -1.000000;
                    MatrixD = 0.000000;
                    MatrixX = cpx;
                    MatrixY = cpy;
                    MatrixZ = 1.000000;
                    BasicMatrixA = 1.000000;
                    BasicMatrixB = 0.000000;
                    BasicMatrixC = 0.000000;
                    BasicMatrixD = -1.000000;
                    BasicMatrixX = cpx;
                    BasicMatrixY = cpy;
                    BasicMatrixZ = 1.000000;
                    RotateMatrixA = -1.000000;
                    RotateMatrixB = 0.000000;
                    RotateMatrixC = 0.000000;
                    RotateMatrixD = -1.000000;
                    RotateMatrixX = 0.003906;
                    RotateMatrixY = 0.003906;
                    RotateMatrixZ = 1.000000;
                    //RotateMatrixA = 0.000000;
                    //RotateMatrixB = -1.000000;
                    //RotateMatrixC = 1.000000;
                    //RotateMatrixD = -0.000000;
                    //RotateMatrixX = 0.0039060;
                    //RotateMatrixY = 0.0039060;
                    //RotateMatrixZ = 1.000000;
                    break;
            }
        }
    }

}
