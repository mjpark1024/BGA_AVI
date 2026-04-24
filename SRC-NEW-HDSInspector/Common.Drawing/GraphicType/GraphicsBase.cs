// http://www.codeproject.com/KB/WPF/WPF_DrawTools.aspx
// Author : Alex Fr
// This article, along with any associated source code and files, is licensed under The Code Project Open License (CPOL)

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using Common.Drawing.InspectionInformation;
using Common.Drawing.MarkingInformation;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Common.Drawing
{
    /// <summary>
    /// Base class for all graphics objects.
    /// </summary>
    public abstract class GraphicsBase : DrawingVisual, INotifyPropertyChanged
    {
        #region Class Members

        private Color[] RawColor = new Color[12] { Colors.Lime, Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green, Colors.Blue,
                                                   Colors.GreenYellow, Colors.Violet, Colors.Gold, Colors.Silver, Colors.Fuchsia, Colors.FloralWhite };

        public string roiCode;
        public Point startPoint;
        public Rect boundaryRect;
        private int unitRow = 0;
        private int unitColumn = 0;
        private int step = 0;

        private int rawRow = 0;
        
        protected string caption = string.Empty;
        protected GraphicsRegionType graphicsRegionType;
        protected Color graphicsObjectColor;

        internal double graphicsLineWidth;
        internal double graphicsActualScale;
        internal bool selected;

        private int objectId;

        private string markID;
        private int markObjID;

        private bool dummy;

        protected const double HitTestWidth = 8.0;
        protected const double HandleSize = 12.0;
        protected const double FontSize = 12.0;
        #endregion Class Members

        #region Constructor
        protected GraphicsBase()
        {
            this.objectId = this.GetHashCode();
        }
        #endregion Constructor

        #region Properties
        public int ID
        {
            get { return objectId; }
            set { objectId = value; }
        }

        public bool Dummy
        {
            get { return dummy; }
            set { dummy = value; }
        }

        public int UnitRow
        {
            get { return unitRow; }
            set { unitRow = value; }
        }

        public int UnitColumn
        {
            get { return unitColumn; }
            set { unitColumn = value; }
        }

        public int RawRow
        {
            get { return rawRow; }
            set
            {
                rawRow = value;
                ObjectColor = RawColor[rawRow - 1];
                OriginObjectColor = ObjectColor;
            }
        }

        public int Step
        {
            get { return step; }
            set { step = value; }
        }

        // 검사 설정 리스트
        public ObservableCollection<InspectionItem> InspectionList
        {
            get { return inspectionList; }
            set { inspectionList = value; }
        }
        private ObservableCollection<InspectionItem> inspectionList = new ObservableCollection<InspectionItem>();

        public MarkItem MarkInfo
        {
            get { return markItem; }
            set { markItem = value; }
        }
        private MarkItem markItem = new MarkItem();

        public GraphicsRegionType RegionType
        {
            get
            {
                return graphicsRegionType;
            }
            set
            {
                graphicsRegionType = value;
                Notify("RegionType");
            }
        }

        public bool IsSelected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                RefreshDrawing();
            }
        }

        public double LineWidth
        {
            get
            {
                return graphicsLineWidth;
            }
            set
            {
                graphicsLineWidth = value;
                RefreshDrawing();
            }
        }

        public Color OriginObjectColor { get; set; }

        public Color ObjectColor
        {
            get
            {
                return graphicsObjectColor;
            }
            set
            {
                graphicsObjectColor = value;
                RefreshDrawing();
            }
        }

        public double ActualScale
        {
            get
            {
                return graphicsActualScale;
            }

            set
            {
                graphicsActualScale = value;
                RefreshDrawing();
            }
        }

        protected double ActualLineWidth
        {
            get
            {
                return graphicsActualScale <= 0 ? graphicsLineWidth : graphicsLineWidth / graphicsActualScale;
            }
        }

        protected double LineHitTestWidth
        {
            get
            {
                // Ensure that hit test area is not too narrow
                return Math.Max(8.0, ActualLineWidth);
            }
        }

        public Point StartPoint
        {
            get { return startPoint; }
        }

        public string Caption
        {
            get { return caption; }
            set 
            { 
                caption = value;
                RefreshDrawing();
            }
        }

        public string MarkID
        {
            get { return markID; }
            set { markID = value; }
        }

        public int MarkOBJID
        {
            get { return markObjID; }
            set { markObjID = value; }
        }

        public int? SentID { get; set; }

        public GraphicsRectangle[] LocalAligns;


        [XmlIgnore]
        public GraphicsSkeletonLine[] LineSegments
        {
            get;
            set;
        }

        [XmlIgnore]
        public GraphicsSkeletonBall[] BallSegments
        {
            get;
            set;
        }
        #endregion Properties

        #region Abstract Methods and Properties
        public abstract int HandleCount
        {
            get;
        }

        public abstract bool Contains(Point point);

        public abstract PropertiesGraphicsBase CreateSerializedObject();

        public abstract Point GetHandle(int handleNumber);

        /// <summary>
        /// Hit test.
        /// Return value: -1 - no hit
        ///                0 - hit anywhere
        ///                > 1 - handle number
        /// </summary>
        public abstract int MakeHitTest(Point point);

        /// <summary>
        /// Test whether object intersects with rectangle
        /// </summary>
        public abstract bool IntersectsWith(Rect rectangle);

        public abstract void Move(double deltaX, double deltaY, double maxWidth, double maxHeight);

        // 조절점 이동.
        public abstract void MoveHandleTo(Point point, int handleNumber);

        public abstract Cursor GetHandleCursor(int handleNumber);
        #endregion Abstract Methods and Properties

        #region Virtual Methods
        /// <summary>
        /// Normalize object.
        /// Call this function in the end of object resizing,
        /// </summary>
        public virtual void Normalize()
        {
            // Empty implementation is OK for classes which don't require
            // normalization, like line.
            // Normalization is required for rectangle-based classes.
        }

        /// <summary>
        /// Implements actual drawing code.
        /// 
        /// Call GraphicsBase.Draw in the end of every derived class Draw 
        /// function to draw tracker if necessary.
        /// </summary>
        public virtual void Draw(DrawingContext drawingContext)
        {
            if (IsSelected)
            {
                DrawTracker(drawingContext);
            }
        }

        /// <summary>
        /// Draw tracker for selected object.
        /// </summary>
        public virtual void DrawTracker(DrawingContext drawingContext)
        {
            if (this is GraphicsRectangleBase)
            {
                // GraphicsRectangleBase의 HandleCount는 8.
                // 사각형 ROI 4000개에 대해 DrawTracker가 호출되면 내부적으로 288000개의 사각형을 생성해야한다.
                // 정상적인 방법으로 그려서는 속도가 나오지 않으므로 원시적으로 처리되도록 한다.
                // 
                // 2012-02-23 suoow2 Added.
                GraphicsRectangleBase rectGraphic = this as GraphicsRectangleBase;
                if (rectGraphic != null)
                {
                    double fSize = Math.Max(HandleSize / graphicsActualScale, ActualLineWidth * 1.1);
                    double fLeft = rectGraphic.Left - fSize / 2;
                    double fTop = rectGraphic.Top - fSize / 2;
                    double fRight = rectGraphic.Right - fSize / 2;
                    double fBottom = rectGraphic.Bottom - fSize / 2;
                    double fxCenter = (fRight + fLeft) / 2;
                    double fyCenter = (fBottom + fTop) / 2;
                    
                    DrawTrackerRectangle(drawingContext, new Rect(fLeft, fTop, fSize, fSize)); // 조절점 1
                    DrawTrackerRectangle(drawingContext, new Rect(fxCenter, fTop, fSize, fSize)); // 조절점 2
                    DrawTrackerRectangle(drawingContext, new Rect(fRight, fTop, fSize, fSize)); // 조절점 3
                    DrawTrackerRectangle(drawingContext, new Rect(fRight, fyCenter, fSize, fSize)); // 조절점 4
                    DrawTrackerRectangle(drawingContext, new Rect(fRight, fBottom, fSize, fSize)); // 조절점 5
                    DrawTrackerRectangle(drawingContext, new Rect(fxCenter, fBottom, fSize, fSize)); // 조절점 6
                    DrawTrackerRectangle(drawingContext, new Rect(fLeft, fBottom, fSize, fSize)); // 조절점 7
                    DrawTrackerRectangle(drawingContext, new Rect(fLeft, fyCenter, fSize, fSize)); // 조절점 8
                }
            }
            else
            {
                for (int i = 1; i <= HandleCount; i++)
                {
                    DrawTrackerRectangle(drawingContext, GetHandleRectangle(i));
                }
            }
        }

        /// <summary>
        /// Dump (for debugging)
        /// </summary>
        [Conditional("DEBUG")]
        public virtual void Dump()
        {
            Trace.WriteLine(this.GetType().Name);

            Trace.WriteLine("ID = " + objectId.ToString(CultureInfo.InvariantCulture) +
                "   Selected = " + selected.ToString(CultureInfo.InvariantCulture));

            Trace.WriteLine("objectColor = " + ColorToDisplay(graphicsObjectColor) +
                "  lineWidth = " + DoubleForDisplay(graphicsLineWidth));
        }
        #endregion Virtual Methods

        #region Other Methods
        public FormattedText CreateCaptionString()
        {
            if (graphicsActualScale == 0.0)
                graphicsActualScale = 1.0;

            Typeface typeFace = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal );
            if (Dummy) caption = string.Format("{0}:{1}:{2}", Step + 1, UnitColumn + 1, UnitRow + 1);

            FormattedText formattedText;
            if (rawRow > 0)
            {
                caption = string.Format("{0}", RawRow);
                formattedText = new FormattedText(
                caption,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeFace,
                30.0 / graphicsActualScale,
                new SolidColorBrush(ObjectColor));
            }
            else
            {
                formattedText = new FormattedText(
                    caption,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeFace,
                    FontSize / graphicsActualScale,
                    new SolidColorBrush(ObjectColor));
            }

            return formattedText;
        }

        public FormattedText CreateTBDString()
        {
            if (graphicsActualScale == 0.0)
                graphicsActualScale = 1.0;

            Typeface typeFace = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
            caption = "2D";
            FormattedText formattedText;

            formattedText = new FormattedText(
                caption,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeFace,
                FontSize / graphicsActualScale,
                new SolidColorBrush(ObjectColor));

            return formattedText;
        }

        public FormattedText CreateIDString(double height)
        {
            if (graphicsActualScale == 0.0)
                graphicsActualScale = 1.0;
            double size = 1.0;
            if (height > 0) size = height;
            Typeface typeFace = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.ExtraBold, FontStretches.Normal);

            FormattedText formattedText = new FormattedText(
                "H00A01231234",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeFace,
                size,
                new SolidColorBrush(Color.FromArgb(170, 255, 255, 255)));
            return formattedText;
        }

        public FormattedText CreateNumberString(double height)
        {
            if (graphicsActualScale == 0.0)
                graphicsActualScale = 1.0;
            double size = 1.0;
            if (height > 0) size = height;
            Typeface typeFace = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.ExtraBold, FontStretches.Normal);

            FormattedText formattedText = new FormattedText(
                "3",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeFace,
                size,
                new SolidColorBrush(Color.FromArgb(170, 255, 255, 255)));
            return formattedText;
        }

        public FormattedText CreateWeekString(double height)
        {
            if (graphicsActualScale == 0.0)
                graphicsActualScale = 1.0;
            double size = 1.0;
            if (height > 0) size = height;
            Typeface typeFace = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.ExtraBold, FontStretches.Normal);

            FormattedText formattedText = new FormattedText(
                "1234",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeFace,
                size,
                new SolidColorBrush(Color.FromArgb(170, 255, 255, 255)));

            return formattedText;
        }

        // 조절점 생성. IsSelected = True일 때 그려진다.
        static void DrawTrackerRectangle(DrawingContext drawingContext, Rect rectangle)
        {
            // External rectangle
            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)), null, rectangle);

            // Middle
            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), null,
                new Rect(rectangle.Left + rectangle.Width / 8,
                         rectangle.Top + rectangle.Height / 8,
                         rectangle.Width * 3 / 4,
                         rectangle.Height * 3 / 4));

            // Internal
            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)), null,
                new Rect(rectangle.Left + rectangle.Width / 4,
                 rectangle.Top + rectangle.Height / 4,
                 rectangle.Width / 2,
                 rectangle.Height / 2));
        }

        /// <summary>
        /// Refresh drawing.
        /// Called after change if any object property.
        /// </summary>
        public void RefreshDrawing()
        {
            DrawingContext dc = this.RenderOpen();
            Draw(dc);
            dc.Close();
        }

        public DrawingContext GetDC()
        {
            return this.RenderOpen();
        }

        /// <summary>
        /// Get handle rectangle by 1-based number
        /// </summary>
        public Rect GetHandleRectangle(int handleNumber)
        {
            Point point = GetHandle(handleNumber);

            // Handle rectangle should have constant size, except of the case
            // when line is too width.
            double size = Math.Max(HandleSize / graphicsActualScale, ActualLineWidth * 1.1);

            return new Rect(point.X - size / 2, point.Y - size / 2, size, size);
        }

        /// <summary>
        /// Helper function used for Dump
        /// </summary>
        static string DoubleForDisplay(double value)
        {
            return ((float)value).ToString("f2", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Helper function used for Dump
        /// </summary>
        static string ColorToDisplay(Color value)
        {
            //return "A:" + value.A.ToString() +
            return String.Format("R:{0} G:{1} B:{2}", value.R.ToString(CultureInfo.InvariantCulture),
                                                      value.G.ToString(CultureInfo.InvariantCulture), 
                                                      value.B.ToString(CultureInfo.InvariantCulture));
        }
        #endregion Other Methods

        #region implements INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void Notify(string strPropertyName)
        {
            PropertyChangedEventHandler p = PropertyChanged;
            if (p != null)
            {
                p(this, new PropertyChangedEventArgs(strPropertyName));
            }
        }
        #endregion
    }
}
