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
 * @file  SectionInformation.cs
 * @brief 
 *  Section Information class.
 * 
 * @author : suoow2 <suoow.yeo@haesung.net>
 * @date : 2011.09.07
 * @version : 1.0
 * 
 * <b> Revision Histroy </b>
 * - 2011.09.07 First creation.
 */

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Xml.Serialization;
using Common.Drawing;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Globalization;
using Common;
using System.IO;
using Common.Drawing.InspectionInformation;

namespace PCS.ModelTeaching
{
    /// <summary>   Section region.  </summary>
    /// <remarks>   suoow2, 2012-02-28. </remarks>
    public class SectionRegion
    {
        public System.Drawing.Point RegionIndex;        // 스트립에 대한 유닛 위치.
        public System.Drawing.Point RegionPosition;     // 스트립에 대한 유닛 시작 위치.
        public bool IsFiducialRegion;                   // 기준 영역 여부
        public bool IsInspection;                       // 검사 여부

        public SectionRegion(int anRegionX, int anRegionY, int anRegionXPos, int anRegionYPos, bool anIsInspection, bool abIsFiducialRegion = false)
        {
            RegionIndex.X = anRegionX;
            RegionIndex.Y = anRegionY;
            RegionPosition.X = anRegionXPos;
            RegionPosition.Y = anRegionYPos;
            IsFiducialRegion = abIsFiducialRegion;
            IsInspection = anIsInspection;
        }

        // For DB Interface.
        public override string ToString()
        {
            return string.Format("X{0}Y{1}I{2}J{3}E{4}", RegionIndex.X, RegionIndex.Y, RegionPosition.X, RegionPosition.Y, IsInspection==true?1:0);
        }
    }

    /// <summary>   Information about the section.  </summary>
    public class SectionInformation
    {
        #region Constructors.
        public SectionInformation()
        {
            ROICanvas = new DrawingCanvas[3];
            Image = new AntiAliasedImage[3];
            BadRectCanvas = new Canvas[3];
            IsTempView = new bool[3];
            Initialize();
        }

        //public SectionInformation(string machineCode, string modelCode, string code, string name, SectionType type, Int32Rect region, BitmapSource image)
        //{
        //    Initialize();

        //    this.MachineCode = machineCode;
        //    this.ModelCode = modelCode;
        //    this.Code = code;
        //    this.Name = name;
        //    this.Type = type;
        //    this.Region = region;
        //    this.BitmapSource = image;
        //}
        #endregion

        #region Properties.
        [XmlIgnore]
        public GraphicsRectangle RelatedROI { get; set; }

        [XmlIgnore]
        public int LastUnitX { get; set; }

        [XmlIgnore]
        public int LastUnitY { get; set; }

        [XmlIgnore]
        public int HashID { get; set; }

        [XmlElement(ElementName = "MachineCode")]
        public string MachineCode { get; set; }

        [XmlElement(ElementName = "ModelCode")]
        public string ModelCode { get; set; }

        [XmlElement(ElementName = "SectionCode")]
        public string Code { get; set; }

        [XmlElement(ElementName = "WorkType")]
        public string WorkType { get; set; }

        [XmlIgnore]
        public int IntCode
        {
            get
            {
                return Convert.ToInt32(Code);
            }
        }

        [XmlElement(ElementName = "SectionName")]
        public string Name { get; set; }

        [XmlElement(ElementName = "SectionType")]
        public SectionType Type { get; set; }

        [XmlElement(ElementName = "Region")]
        public Int32Rect Region { get; set; }

        [XmlElement(ElementName = "Color")]
        public string RGBColor { get; set; }

        [XmlElement(ElementName = "BondPadSide")]
        public int BondPadSide { get; set; }

        [XmlIgnore]
        public SolidColorBrush RGBBrush
        {
            get
            {
                if (RGBColor != null && RGBColor.Length == 6)
                {
                    return new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(int.Parse(RGBColor.Substring(0, 2), NumberStyles.AllowHexSpecifier)),
                                                                   Convert.ToByte(int.Parse(RGBColor.Substring(2, 2), NumberStyles.AllowHexSpecifier)),
                                                                   Convert.ToByte(int.Parse(RGBColor.Substring(4, 2), NumberStyles.AllowHexSpecifier))));
                }
                else return new SolidColorBrush(Colors.Transparent);
            }
        }

        [XmlElement(ElementName = "IterationXPosition")]
        public int IterationXPosition { get; set; }

        [XmlElement(ElementName = "IterationYPosition")]
        public int IterationYPosition { get; set; }

        [XmlElement(ElementName = "IterationStartX")]
        public int IterationStartX { get; set; }

        [XmlElement(ElementName = "IterationStartY")]
        public int IterationStartY { get; set; }

        [XmlElement(ElementName = "IterationCountX")]
        public int IterationCountX { get; set; }

        [XmlElement(ElementName = "IterationCountY")]
        public int IterationCountY { get; set; }

        [XmlElement(ElementName = "IterationJumpX")]
        public int IterationJumpX { get; set; }

        [XmlElement(ElementName = "IterationJumpY")]
        public int IterationJumpY { get; set; }

        [XmlElement(ElementName = "IterationPitchX")]
        public double IterationPitchX { get; set; }

        [XmlElement(ElementName = "IterationPitchY")]
        public double IterationPitchY { get; set; }

        [XmlElement(ElementName = "BlockCount")]
        public int BlockCount { get; set; }
        
        [XmlElement(ElementName = "BlockCountX")]
        public int BlockCountX { get; set; }

        [XmlElement(ElementName = "BlockCountY")]
        public int BlockCountY { get; set; }

        [XmlElement(ElementName = "BlockPitch")]
        public double BlockPitch { get; set; }

        [XmlElement(ElementName = "BlockPitchX")]
        public double BlockPitchX { get; set; }

        [XmlElement(ElementName = "BlockPitchY")]
        public double BlockPitchY { get; set; }

        [XmlIgnore, DefaultValue(false)]
        public bool[] IsTempView { get; set; }

        [XmlIgnore] // TODO : XML 대응시 처리되어야 함.
        public List<SectionRegion> SectionRegionList
        {
            get
            {
                return m_sectionRegionList;
            }
            set
            {
                m_sectionRegionList = value;
            }
        }
        #endregion

        /// <summary>   Gets or sets the image. </summary>
        /// <value> The image. </value>
        [XmlIgnore]
        public BitmapSource[] BitmapSource
        {
            get
            {
                return m_BitmapSource;
            }
        }
        //    set
        //    {
        //        m_BitmapSource = value;
        //        if (m_BitmapSource != null)
        //        {
        //            Image.Width = m_BitmapSource.PixelWidth;
        //            Image.Height = m_BitmapSource.PixelHeight;
        //            Image.Source = m_BitmapSource;
        //            BadRectCanvas.Width = ROICanvas.Width = Image.Width;
        //            BadRectCanvas.Height = ROICanvas.Height = Image.Height;

        //            IsTempView = false;
        //        }
        //    }
        //}

        public void SetBitmapSource(BitmapSource value, int anChannel)
        {
            m_BitmapSource[anChannel] = value;
            if (m_BitmapSource[anChannel] != null)
            {
                Image[anChannel].Width = m_BitmapSource[anChannel].PixelWidth;
                Image[anChannel].Height = m_BitmapSource[anChannel].PixelHeight;
                Image[anChannel].Source = m_BitmapSource[anChannel];
                BadRectCanvas[anChannel].Width = ROICanvas[anChannel].Width = Image[anChannel].Width;
                BadRectCanvas[anChannel].Height = ROICanvas[anChannel].Height = Image[anChannel].Height;

                IsTempView[anChannel] = false;
            }
        }

        public void SetTempBitmapSource(BitmapSource value, int anChannel)
        {
            m_TempBitmapSource[anChannel] = value;
            if (m_TempBitmapSource[anChannel] != null)
            {
                Image[anChannel].Width = m_TempBitmapSource[anChannel].PixelWidth;
                Image[anChannel].Height = m_TempBitmapSource[anChannel].PixelHeight;
                Image[anChannel].Source = m_TempBitmapSource[anChannel];

                //debug
                //using (var fileStream = new FileStream("d:\\test"+anChannel+".jpg", FileMode.Create))
                //{
                //    BitmapEncoder encoder = new JpegBitmapEncoder();
                //    encoder.Frames.Add(BitmapFrame.Create(m_TempBitmapSource[anChannel]));
                //    encoder.Save(fileStream);
                //}

                BadRectCanvas[anChannel].Width = ROICanvas[anChannel].Width = Image[anChannel].Width;
                BadRectCanvas[anChannel].Height = ROICanvas[anChannel].Height = Image[anChannel].Height;

                IsTempView[anChannel] = true;
            }
        }

        public void SetTempBitmapSource_channel(int anChannel)
        {
            if (m_TempBitmapSource[anChannel] != null)
            {
                Image[anChannel].Width = m_TempBitmapSource[anChannel].PixelWidth;
                Image[anChannel].Height = m_TempBitmapSource[anChannel].PixelHeight;
                Image[anChannel].Source = m_TempBitmapSource[anChannel];

                //debug
                //using (var fileStream = new FileStream("d:\\test"+anChannel+".jpg", FileMode.Create))
                //{
                //    BitmapEncoder encoder = new JpegBitmapEncoder();
                //    encoder.Frames.Add(BitmapFrame.Create(m_TempBitmapSource[anChannel]));
                //    encoder.Save(fileStream);
                //}

                BadRectCanvas[anChannel].Width = ROICanvas[anChannel].Width = Image[anChannel].Width;
                BadRectCanvas[anChannel].Height = ROICanvas[anChannel].Height = Image[anChannel].Height;

                IsTempView[anChannel] = true;
            }
        }

        [XmlIgnore]
        public BitmapSource[] TempBitmapSource // 개별 유닛 보기 요청시 Display되는 BitmapSource
        {
            get
            {
                return m_TempBitmapSource;
            }
           
        }

        public BitmapSource GetBitmapSource(int anChannel)
        {
            
             if (!IsTempView[anChannel])
             {
                return m_BitmapSource[anChannel];
             }
             else
             {

                return m_TempBitmapSource[anChannel];
             }
        }

        public BitmapSource GetBitmapSource2(int anChannel)
        {

            if (!IsTempView[anChannel])
            {
                if (this.Type.Code == SectionTypeCode.OUTER_REGION)
                {
                    return BitmapHelper.ResizeBitmapSource(m_BitmapSource[anChannel], 4);
                }
                else return m_BitmapSource[anChannel];
            }
            else
            {
                if (this.Type.Code == SectionTypeCode.OUTER_REGION)
                {
                    return BitmapHelper.ResizeBitmapSource(m_TempBitmapSource[anChannel], 4);
                }
                return m_TempBitmapSource[anChannel];
            }
        }

        [XmlIgnore]
        public Canvas[] BadRectCanvas
        {
            get;
            set;
        }

        #region Private member variables.
        private BitmapSource[] m_BitmapSource = new BitmapSource[3];
        private BitmapSource[] m_TempBitmapSource = new BitmapSource[3];
        private Rectangle[] m_BadRect = new Rectangle[3];

        private List<SectionRegion> m_sectionRegionList = new List<SectionRegion>();
        #endregion

        #region Initialize Helper Controls.
        /// <summary>   Initializes this object. </summary>
        /// <remarks>   suoow2, 2014-11-10. </remarks>
        private void Initialize()
        {
            CreateImage();
            CreateROICanvas();
            CreateBadRectCanvas();
        }

        /// <summary>   Creates the image canvas. </summary>
        /// <remarks>   suoow2, 2014-11-10. </remarks>
        private void CreateImage()
        {
            for (int i = 0; i < 3; i++)
            {
                this.Image[i] = new AntiAliasedImage();
                this.Image[i].Source = null;
                this.Image[i].HorizontalAlignment = HorizontalAlignment.Left;
                this.Image[i].VerticalAlignment = VerticalAlignment.Top;
                this.Image[i].Width = 0;
                this.Image[i].Height = 0;
            }
        }

        /// <summary>   Creates the roi canvas. </summary>
        /// <remarks>   suoow2, 2014-11-10. </remarks>
        private void CreateROICanvas()
        {
            for (int i = 0; i < 3; i++)
            {
                this.ROICanvas[i] = new DrawingCanvas(false, false);
                this.ROICanvas[i].Background = new SolidColorBrush(Colors.Transparent);
                this.ROICanvas[i].HorizontalAlignment = HorizontalAlignment.Left;
                this.ROICanvas[i].VerticalAlignment = VerticalAlignment.Top;
                this.ROICanvas[i].Width = 0;
                this.ROICanvas[i].Height = 0;
            }
        }

        private void CreateBadRectCanvas()
        {
            for (int i = 0; i < 3; i++)
            {
                this.BadRectCanvas[i] = new Canvas();
                this.BadRectCanvas[i].Background = new SolidColorBrush(Colors.Transparent);
                this.BadRectCanvas[i].HorizontalAlignment = HorizontalAlignment.Left;
                this.BadRectCanvas[i].VerticalAlignment = VerticalAlignment.Top;
                this.BadRectCanvas[i].Width = 0;
                this.BadRectCanvas[i].Height = 0;

                m_BadRect[i] = new Rectangle
                {
                    Stroke = new SolidColorBrush(Colors.DeepPink),
                    StrokeThickness = 4
                };

                BadRectCanvas[i].Children.Add(m_BadRect[i]);
            }
        }

        public void DrawBadRect(int anChanel, double afRectWidth, double afRectHeight, double afLeftMargin, double afTopMargin)
        {
            m_BadRect[anChanel].Width = afRectWidth;
            m_BadRect[anChanel].Height = afRectHeight;
            m_BadRect[anChanel].StrokeThickness = Math.Min(Math.Min(afRectWidth, afRectHeight) / 35, 8.0);
            if (m_BadRect[anChanel].StrokeThickness < 4.0)
                m_BadRect[anChanel].StrokeThickness = 4.0;

            Canvas.SetLeft(m_BadRect[anChanel], afLeftMargin);
            Canvas.SetTop(m_BadRect[anChanel], afTopMargin);
        }
        #endregion

        #region Helper controls.
        [XmlIgnore]
        public AntiAliasedImage[] Image
        {
            get;
            set;
        }

        [XmlIgnore]
        public DrawingCanvas[] ROICanvas
        {
            get;
            set;
        }
        #endregion
    }
}
