using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Common.Drawing
{

    public class PropertiesGraphicsSkeletonBall : PropertiesGraphicsBase
    {
        #region Private member variables.
        private Int16Rect boundaryRect;
        private float radian;
        #endregion

        public PropertiesGraphicsSkeletonBall()
        {
            // XML로의 저장을 위해 기본 생성자가 필요하다.
        }

        public PropertiesGraphicsSkeletonBall(GraphicsSkeletonBall skeletonBall)
        {
            if (skeletonBall == null)
            {
                throw new ArgumentNullException("skeletonBall");
            }

            // 중앙선 전용 property.
            this.boundaryRect = skeletonBall.BoundaryRect;
            this.radian = skeletonBall.Radian;
            this.lineWidth = skeletonBall.LineWidth;
            this.regionType = skeletonBall.RegionType;
            this.actualScale = skeletonBall.ActualScale;
            this.Id = skeletonBall.ID;
            this.selected = false;

            // this.objectColor = skeletonLine.ObjectColor; 의미가 없다.
            this.caption = string.Empty;
        }

        public override GraphicsBase CreateGraphics()
        {
            lineWidth = 2; // Default thickness.

            actualScale = (actualScale > 0) ? actualScale : 1.0;
            GraphicsBase b = new GraphicsSkeletonBall(boundaryRect, radian, actualScale);

            if (this.Id != 0)
            {
                b.ID = this.Id;
                b.IsSelected = false;
            }

            return b;
        }

        #region Properties.

        public Int16Rect BoundaryRect
        {
            get { return boundaryRect; }
            set { boundaryRect = value; }
        }

        public float Radian
        {
            get { return radian; }
            set { radian = value; }
        }

        #endregion
    }
}
