using System;
using System.Collections.Generic;
using Common.Drawing.MarkingInformation;
using System.Windows.Media;

// Modify & Comment by suoow2.

namespace Common.Drawing
{
    public class PropertiesGraphicsEllipseMark : PropertiesGraphicsBase
    {
        #region Member variables.
        private double left;
        private double top;
        private double right;
        private double bottom;
        private Circle circle;
        private bool white;
        private bool dummy;
        private string motherID;
        private GraphicsRegionType regiontype;
        #endregion

        #region Ctor.
        // For XmlSerializer
        public PropertiesGraphicsEllipseMark()
        {
            // XML로의 저장을 위해 기본 생성자가 필요하다.
        }

        public PropertiesGraphicsEllipseMark(GraphicsEllipseMark ellipse)
        {
            if (ellipse == null)
            {
                throw new ArgumentNullException("ellipsemark");
            }
            this.CircleInfo = ellipse.CircleInfo;
            this.left = ellipse.Left;
            this.top = ellipse.Top;
            this.right = ellipse.Right;
            this.bottom = ellipse.Bottom;
            this.white = ellipse.white;
            this.dummy = ellipse.Dummy;
            this.lineWidth = ellipse.LineWidth;
            this.regionType = ellipse.RegionType;
            this.objectColor = ellipse.ObjectColor;
            this.actualScale = ellipse.ActualScale;
            this.Id = ellipse.ID;
            this.motherID = ellipse.MarkID;
            this.selected = ellipse.IsSelected;
            this.caption = ellipse.Caption;
            this.inspectionItems = null;
            this.markInfo = ellipse.MarkInfo;
            this.regiontype = ellipse.RegionType;
        }
        #endregion

        public override GraphicsBase CreateGraphics()
        {
            lineWidth = 2; // Default thickness.
            MarkItem markinfo = new MarkItem();
            markinfo = MarkingItemHelper.CopyMarkItem(this.markInfo);
            actualScale = (actualScale > 0) ? actualScale : 1.0;
            GraphicsBase b = new GraphicsEllipseMark(circle, lineWidth, actualScale, white, dummy, caption, markinfo, motherID, regiontype);
            if (this.Id != 0)
            {
                b.ID = this.Id;
                b.IsSelected = this.selected;
            }

            return b;
        }

        #region Properties
        public Circle CircleInfo
        {
            get { return circle; }
            set
            {
                circle = value;
                left = circle.Position.X - circle.Radian;
                top = circle.Position.Y - circle.Radian;
                right = circle.Position.X + circle.Radian;
                bottom = circle.Position.Y + circle.Radian;
            }
        }

        public GraphicsRegionType RegionType
        {
            get { return regiontype; }
        }

        public double Left
        {
            get { return left; }
           // set { left = value; }
        }

        public double Top
        {
            get { return top; }
          //  set { top = value; }
        }

        public double Right
        {
            get { return right; }
          //  set { right = value; }
        }

        public double Bottom
        {
            get { return bottom; }
          //  set { bottom = value; }
        }

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        public Color ObjectColor
        {
            get { return objectColor; }
            set { objectColor = value; }
        }

        public bool White
        {
            get { return white; }
            set { white = value; }
        }

        public bool Dummy
        {
            get { return dummy; }
            set { dummy = value; }
        }

        public string MotherROIID
        {
            get { return motherID; }
            set { motherID = value; }
        }

        public MarkItem MarkInfo
        {
            get { return markInfo; }
            set { markInfo = value; }
        }
        #endregion Properties
    }
}
