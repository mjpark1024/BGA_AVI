using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataBase;

namespace Common.Drawing.MarkingInformation
{
    

    //////////Parameter structure
    //0  MarkType           : 0 Logo 1 Num 2 ID 
    //1  PramID             : Power Select
    //2  PLTFile/FNTFile    : Files
    //3  RefNo              : Reference ID
    //4  Width              : Text Width
    //5  Height             : Text Height
    //6  Space              : Text Spacing
    //7  Select             : 1/2 Barcode Select
    //8  Barcode Type       : Barcode Type
    //9  Bar Marking Type   : 
    //10 MatrixSize         : Matrix 크기
    //11 DotSize            : Dot 크기

    #region Rail Circle Type.
    /// <summary>   Rail Circle property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class RailCircleProperty : MarkInfo
    {
        public string PLTFile;
        public int ParaNumber;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public double FirstX;
        public double FirstY;
        public double GapX;
        public double GapY;
        public RailCircleProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {
            PLTFile = hpgl.FileName;
            ParaNumber = hpgl.ParaNumber;
            Left = hpgl.DispLeft;
            Top = hpgl.DispTop;
            Width = hpgl.DispRight - hpgl.DispLeft;
            Height = hpgl.DispBottom - hpgl.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = hpgl.RotateAngle;
            FirstX = strip.FirstX;
            FirstY = strip.FirstY;
        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            hpgl.FileName = PLTFile;
            hpgl.ParaNumber = ParaNumber;
            hpgl.DispLeft = Left;
            hpgl.DispTop = Top;
            hpgl.DispRight = Left + Width; ;
            hpgl.DispBottom = Top + Height;
            hpgl.RotateAngle = Rotate;
            return 0;
        }

        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {

        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {

        }
        public override int SaveProperty(ref TEXT text)
        {
            return 0;
        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            RailCircleProperty cloneRailCircleProperty = new RailCircleProperty();
            //cloneRailCircleProperty.MarkType = this.MarkType;
            cloneRailCircleProperty.ParaNumber = this.ParaNumber;
            cloneRailCircleProperty.PLTFile = this.PLTFile;
            //cloneRailCircleProperty.FirstPoint = this.FirstPoint;
            cloneRailCircleProperty.Left = this.Left;
            cloneRailCircleProperty.Top = this.Top;
            cloneRailCircleProperty.Width = this.Width;
            cloneRailCircleProperty.Height = this.Height;
            cloneRailCircleProperty.Rotate = this.Rotate;
            cloneRailCircleProperty.FirstX = this.FirstX;
            cloneRailCircleProperty.FirstY = this.FirstY;
            cloneRailCircleProperty.GapX = this.GapX;
            cloneRailCircleProperty.GapY = this.GapY;
            return cloneRailCircleProperty;
        }
    }
    #endregion

    #region Rail Rect Type.
    /// <summary>   Rail Rect property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class RailRectProperty : MarkInfo
    {
        //public int MarkType;
        public string PLTFile;
        public int ParaNumber;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public double FirstX;
        public double FirstY;
        public double GapX;
        public double GapY;
        //public System.Windows.Point FirstPoint;
        public RailRectProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {
            PLTFile = hpgl.FileName;
            ParaNumber = hpgl.ParaNumber;
            Left = hpgl.DispLeft;
            Top = hpgl.DispTop;
            Width = hpgl.DispRight - hpgl.DispLeft;
            Height = hpgl.DispBottom - hpgl.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = hpgl.RotateAngle;
            FirstX = strip.FirstX;
            FirstY = strip.FirstY;
        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            hpgl.FileName = PLTFile;
            hpgl.ParaNumber = ParaNumber;
            hpgl.DispLeft = Left;
            hpgl.DispTop = Top;
            hpgl.DispRight = Left + Width; ;
            hpgl.DispBottom = Top + Height;
            hpgl.RotateAngle = Rotate;
            return 0;
        }

        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {

        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {

        }
        public override int SaveProperty(ref TEXT text)
        {
            return 0;
        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            RailRectProperty cloneRailRectProperty = new RailRectProperty();
            //cloneRailRectProperty.MarkType = this.MarkType;
            cloneRailRectProperty.ParaNumber = this.ParaNumber;
            cloneRailRectProperty.PLTFile = this.PLTFile;
            cloneRailRectProperty.Left = this.Left;
            cloneRailRectProperty.Top = this.Top;
            cloneRailRectProperty.Width = this.Width;
            cloneRailRectProperty.Height = this.Height;
            cloneRailRectProperty.Rotate = this.Rotate;
            cloneRailRectProperty.FirstX = this.FirstX;
            cloneRailRectProperty.FirstY = this.FirstY;
            cloneRailRectProperty.GapX = this.GapX;
            cloneRailRectProperty.GapY = this.GapY;
            // cloneRailRectProperty.FirstPoint = this.FirstPoint;
            return cloneRailRectProperty;
        }
    }
    #endregion

    #region Rail Triangle Type.
    /// <summary>   Rail Triangle property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class RailTriProperty : MarkInfo
    {
        //public int MarkType;
        public string PLTFile;
        public int ParaNumber;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public double FirstX;
        public double FirstY;
        public double GapX;
        public double GapY;
       // public System.Windows.Point FirstPoint;
        public RailTriProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {
            PLTFile = hpgl.FileName;
            ParaNumber = hpgl.ParaNumber;
            Left = hpgl.DispLeft;
            Top = hpgl.DispTop;
            Width = hpgl.DispRight - hpgl.DispLeft;
            Height = hpgl.DispBottom - hpgl.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = hpgl.RotateAngle;
            FirstX = strip.FirstX;
            FirstY = strip.FirstY;
        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            hpgl.FileName = PLTFile;
            hpgl.ParaNumber = ParaNumber;
            hpgl.DispLeft = Left;
            hpgl.DispTop = Top;
            hpgl.DispRight = Left + Width; ;
            hpgl.DispBottom = Top + Height;
            hpgl.RotateAngle = Rotate;
            return 0;
        }

        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {

        }

        public override int SaveProperty(ref TEXT text)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {

        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            RailTriProperty cloneRailTriProperty = new RailTriProperty();
            //  cloneRailTriProperty.MarkType = this.MarkType;
            cloneRailTriProperty.ParaNumber = this.ParaNumber;
            cloneRailTriProperty.PLTFile = this.PLTFile;
            cloneRailTriProperty.Left = this.Left;
            cloneRailTriProperty.Top = this.Top;
            cloneRailTriProperty.Width = this.Width;
            cloneRailTriProperty.Height = this.Height;
            cloneRailTriProperty.Rotate = this.Rotate;
            cloneRailTriProperty.FirstX = this.FirstX;
            cloneRailTriProperty.FirstY = this.FirstY;
            cloneRailTriProperty.GapX = this.GapX;
            cloneRailTriProperty.GapY = this.GapY;
            //  cloneRailTriProperty.FirstPoint = this.FirstPoint;
            return cloneRailTriProperty;
        }
    }
    #endregion

    #region Rail Special Type.
    /// <summary>   Rail Special property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class RailSpecialProperty : MarkInfo
    {
      //  public int MarkType;
        public string PLTFile;
        public int ParaNumber;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public double FirstX;
        public double FirstY;
        public double GapX;
        public double GapY;
       // public System.Windows.Point FirstPoint;
        public RailSpecialProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {
            PLTFile = hpgl.FileName;
            ParaNumber = hpgl.ParaNumber;
            Left = hpgl.DispLeft;
            Top = hpgl.DispTop;
            Width = hpgl.DispRight - hpgl.DispLeft;
            Height = hpgl.DispBottom - hpgl.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = hpgl.RotateAngle;
            FirstX = strip.FirstX;
            FirstY = strip.FirstY;
        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            hpgl.FileName = PLTFile;
            hpgl.ParaNumber = ParaNumber;
            hpgl.DispLeft = Left;
            hpgl.DispTop = Top;
            hpgl.DispRight = Left + Width; ;
            hpgl.DispBottom = Top + Height;
            hpgl.RotateAngle = Rotate;
            return 0;
        }

        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {

        }

        public override int SaveProperty(ref TEXT text)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {

        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            RailSpecialProperty cloneRailSpecialProperty = new RailSpecialProperty();
            //cloneRailSpecialProperty.MarkType = this.MarkType;
            cloneRailSpecialProperty.ParaNumber = this.ParaNumber;
            cloneRailSpecialProperty.PLTFile = this.PLTFile;
            cloneRailSpecialProperty.Left = this.Left;
            cloneRailSpecialProperty.Top = this.Top;
            cloneRailSpecialProperty.Width = this.Width;
            cloneRailSpecialProperty.Height = this.Height;
            cloneRailSpecialProperty.Rotate = this.Rotate;
            cloneRailSpecialProperty.FirstX = this.FirstX;
            cloneRailSpecialProperty.FirstY = this.FirstY;
            cloneRailSpecialProperty.GapX = this.GapX;
            cloneRailSpecialProperty.GapY = this.GapY;
            // cloneRailSpecialProperty.FirstPoint = this.FirstPoint; 
            return cloneRailSpecialProperty;
        }
    }
    #endregion

    #region Unit Circle Type.
    /// <summary>   Unit Circle property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class UnitCircleProperty : MarkInfo
    {
       // public int MarkType;
        public string PLTFile;
        public int ParaNumber;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public UnitCircleProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {
            PLTFile = hpgl.FileName;
            ParaNumber = hpgl.ParaNumber;
            Left = hpgl.DispLeft;
            Top = hpgl.DispTop;
            Width = hpgl.DispRight - hpgl.DispLeft;
            Height = hpgl.DispBottom - hpgl.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = hpgl.RotateAngle;
        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            hpgl.FileName = PLTFile;
            hpgl.ParaNumber = ParaNumber;
            hpgl.DispLeft = Left;
            hpgl.DispTop = Top;
            hpgl.DispRight = Left + Width; ;
            hpgl.DispBottom = Top + Height;
            hpgl.RotateAngle = Rotate;
            return 0;
        }

        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {

        }

        public override int SaveProperty(ref TEXT text)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {

        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            UnitCircleProperty cloneUnitCircleProperty = new UnitCircleProperty();
            //cloneUnitCircleProperty.MarkType = this.MarkType;
            cloneUnitCircleProperty.ParaNumber = this.ParaNumber;
            cloneUnitCircleProperty.PLTFile = this.PLTFile;
            cloneUnitCircleProperty.Left = this.Left;
            cloneUnitCircleProperty.Top = this.Top;
            cloneUnitCircleProperty.Width = this.Width;
            cloneUnitCircleProperty.Height = this.Height;
            cloneUnitCircleProperty.Rotate = this.Rotate;
            return cloneUnitCircleProperty;
        }
    }
    #endregion

    #region Unit Rect Type.
    /// <summary>   Unit Rect property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class UnitRectProperty : MarkInfo
    {
        //public int MarkType;
        public string PLTFile;
        public int ParaNumber;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public UnitRectProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {
            PLTFile = hpgl.FileName;
            ParaNumber = hpgl.ParaNumber;
            Left = hpgl.DispLeft;
            Top = hpgl.DispTop;
            Width = hpgl.DispRight - hpgl.DispLeft;
            Height = hpgl.DispBottom - hpgl.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = hpgl.RotateAngle;
        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            hpgl.FileName = PLTFile;
            hpgl.ParaNumber = ParaNumber;
            hpgl.DispLeft = Left;
            hpgl.DispTop = Top;
            hpgl.DispRight = Left + Width; ;
            hpgl.DispBottom = Top + Height;
            hpgl.RotateAngle = Rotate;
            return 0;
        }

        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {

        }

        public override int SaveProperty(ref TEXT text)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {

        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            UnitRectProperty cloneUnitRectProperty = new UnitRectProperty();
            //cloneUnitRectProperty.MarkType = this.MarkType;
            cloneUnitRectProperty.ParaNumber = this.ParaNumber;
            cloneUnitRectProperty.PLTFile = this.PLTFile;
            cloneUnitRectProperty.Left = this.Left;
            cloneUnitRectProperty.Top = this.Top;
            cloneUnitRectProperty.Width = this.Width;
            cloneUnitRectProperty.Height = this.Height;
            cloneUnitRectProperty.Rotate = this.Rotate;
            return cloneUnitRectProperty;
        }
    }
    #endregion

    #region Unit Triangle Type.
    /// <summary>   Unit Triangle property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class UnitTriProperty : MarkInfo
    {
        //public int MarkType;
        public string PLTFile;
        public int ParaNumber;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public UnitTriProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {
            PLTFile = hpgl.FileName;
            ParaNumber = hpgl.ParaNumber;
            Left = hpgl.DispLeft;
            Top = hpgl.DispTop;
            Width = hpgl.DispRight - hpgl.DispLeft;
            Height = hpgl.DispBottom - hpgl.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = hpgl.RotateAngle;
        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            hpgl.FileName = PLTFile;
            hpgl.ParaNumber = ParaNumber;
            hpgl.DispLeft = Left;
            hpgl.DispTop = Top;
            hpgl.DispRight = Left + Width; ;
            hpgl.DispBottom = Top + Height;
            hpgl.RotateAngle = Rotate;
            return 0;
        }

        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {

        }

        public override int SaveProperty(ref TEXT text)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {

        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            UnitTriProperty cloneUnitTriProperty = new UnitTriProperty();
            // cloneUnitTriProperty.MarkType = this.MarkType;
            cloneUnitTriProperty.ParaNumber = this.ParaNumber;
            cloneUnitTriProperty.PLTFile = this.PLTFile;
            cloneUnitTriProperty.Left = this.Left;
            cloneUnitTriProperty.Top = this.Top;
            cloneUnitTriProperty.Width = this.Width;
            cloneUnitTriProperty.Height = this.Height;
            cloneUnitTriProperty.Rotate = this.Rotate;
            return cloneUnitTriProperty;
        }
    }
    #endregion

    #region Unit Special Type.
    /// <summary>   Unit Special property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class UnitSpecialProperty : MarkInfo
    {
       // public int MarkType;
        public string PLTFile;
        public int ParaNumber;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public UnitSpecialProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {
            PLTFile = hpgl.FileName;
            ParaNumber = hpgl.ParaNumber;
            Left = hpgl.DispLeft;
            Top = hpgl.DispTop;
            Width = hpgl.DispRight - hpgl.DispLeft;
            Height = hpgl.DispBottom - hpgl.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = hpgl.RotateAngle;
        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            hpgl.FileName = PLTFile;
            hpgl.ParaNumber = ParaNumber;
            hpgl.DispLeft = Left;
            hpgl.DispTop = Top;
            hpgl.DispRight = Left + Width; ;
            hpgl.DispBottom = Top + Height;
            hpgl.RotateAngle = Rotate;
            return 0;
        }

        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {

        }

        public override int SaveProperty(ref TEXT text)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {

        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            UnitSpecialProperty cloneUnitSpecialProperty = new UnitSpecialProperty();
            // cloneUnitSpecialProperty.MarkType = this.MarkType;
            cloneUnitSpecialProperty.ParaNumber = this.ParaNumber;
            cloneUnitSpecialProperty.PLTFile = this.PLTFile;
            cloneUnitSpecialProperty.Left = this.Left;
            cloneUnitSpecialProperty.Top = this.Top;
            cloneUnitSpecialProperty.Width = this.Width;
            cloneUnitSpecialProperty.Height = this.Height;
            cloneUnitSpecialProperty.Rotate = this.Rotate;
            return cloneUnitSpecialProperty;
        }
    }
    #endregion

    #region Number Type.
    /// <summary>   Number property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class NumberProperty : MarkInfo
    {
        //public int MarkType;
        public string FNTFile;
        public int ParaNumber;
        public double CapitalHeight;
        public double CharGap;
        public double CharWidth;
        public double CharHeight;
        public double LineGap;
        public double SpaceSize;
        public int RefNumber;
        public int SpecialType;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public bool Location;
        public NumberProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {
           
        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {
            FNTFile = text.FontName;
            ParaNumber = text.ParaNumber;
            CapitalHeight = text.CapitalHeight;
            CharGap = text.CharGap;
            CharWidth = text.Width;
            CharHeight = text.Height;
            LineGap = text.LineGap;
            SpaceSize = text.SpaceSize;
            RefNumber = text.ReferenceNumber1;
            SpecialType = text.SpecialType;
            Left = text.DispLeft;
            Top = text.DispTop;
            Width = text.DispRight - text.DispLeft;
            Height = text.DispBottom - text.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = text.RotateAngle;
            Location = loc == 0 ? true : false;
        }
        public override void LoadProperty(Strip strip, TEXT text)
        {
            
        }

        public override int SaveProperty(ref TEXT text)
        {
            text.FontName = FNTFile;
            text.ParaNumber = ParaNumber;
            CapitalHeight = text.CapitalHeight;
            text.CharGap = CharGap;
            text.Width = CharWidth;
            text.Height = CharHeight;
            text.LineGap = LineGap;
            text.SpaceSize = SpaceSize;
            text.ReferenceNumber1 = RefNumber;
            text.SpecialType = SpecialType;
            text.DispLeft = Left;
            text.DispTop = Top;
            text.DispRight = Left + Width; 
            text.DispBottom = Top + Height;
            text.RotateAngle = Rotate;
            return 0;
        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            NumberProperty cloneNumProperty = new NumberProperty();
            cloneNumProperty.FNTFile = this.FNTFile;
            cloneNumProperty.ParaNumber = this.ParaNumber;
            cloneNumProperty.CapitalHeight = this.CapitalHeight;
            cloneNumProperty.CharGap = this.CharGap;
            cloneNumProperty.CharWidth = this.CharWidth;
            cloneNumProperty.CharHeight = this.CharHeight;
            cloneNumProperty.LineGap = this.LineGap;
            cloneNumProperty.SpaceSize = this.SpaceSize;
            cloneNumProperty.RefNumber = this.RefNumber;
            cloneNumProperty.SpecialType = this.SpecialType;
            cloneNumProperty.Left = this.Left;
            cloneNumProperty.Top = this.Top;
            cloneNumProperty.Width = this.Width;
            cloneNumProperty.Height = this.Height;
            cloneNumProperty.Rotate = this.Rotate;
            cloneNumProperty.Location = this.Location;
            return cloneNumProperty;
        }
    }
    #endregion

    #region Week Type.
    /// <summary>   Number property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class WeekProperty : MarkInfo
    {
        public string FNTFile;
        public int ParaNumber;
        public double CapitalHeight;
        public double CharGap;
        public double CharWidth;
        public double CharHeight;
        public double LineGap;
        public double SpaceSize;
        public int RefNumber;
        public int SpecialType;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public int Location;
        public WeekProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {

        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, TEXT text)
        {

        }

        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {
            FNTFile = text.FontName;
            ParaNumber = text.ParaNumber;
            CapitalHeight = text.CapitalHeight;
            CharGap = text.CharGap;
            CharWidth = text.Width;
            CharHeight = text.Height;
            LineGap = text.LineGap;
            SpaceSize = text.SpaceSize;
            RefNumber = text.ReferenceNumber1;
            SpecialType = text.SpecialType;
            Left = text.DispLeft;
            Top = text.DispTop;
            Width = text.DispRight - text.DispLeft;
            Height = text.DispBottom - text.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = text.RotateAngle;
            Location = loc;
        }

        public override int SaveProperty(ref TEXT text)
        {
            text.FontName = FNTFile;
            text.ParaNumber = ParaNumber;
            CapitalHeight = text.CapitalHeight;
            text.CharGap = CharGap;
            text.Width = CharWidth;
            text.Height = CharHeight;
            text.LineGap = LineGap;
            text.SpaceSize = SpaceSize;
            text.ReferenceNumber1 = RefNumber;
            text.SpecialType = SpecialType;
            text.DispLeft = Left;
            text.DispTop = Top;
            text.DispRight = Left + Width;
            text.DispBottom = Top + Height;
            text.RotateAngle = Rotate;
            return 0;
        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            WeekProperty cloneWeekProperty = new WeekProperty();
            cloneWeekProperty.FNTFile = this.FNTFile;
            cloneWeekProperty.ParaNumber = this.ParaNumber;
            cloneWeekProperty.CapitalHeight = this.CapitalHeight;
            cloneWeekProperty.CharGap = this.CharGap;
            cloneWeekProperty.CharWidth = this.CharWidth;
            cloneWeekProperty.CharHeight = this.CharHeight;
            cloneWeekProperty.LineGap = this.LineGap;
            cloneWeekProperty.SpaceSize = this.SpaceSize;
            cloneWeekProperty.RefNumber = this.RefNumber;
            cloneWeekProperty.SpecialType = this.SpecialType;
            cloneWeekProperty.Left = this.Left;
            cloneWeekProperty.Top = this.Top;
            cloneWeekProperty.Width = this.Width;
            cloneWeekProperty.Height = this.Height;
            cloneWeekProperty.Rotate = this.Rotate;
            cloneWeekProperty.Location = this.Location;
            return cloneWeekProperty;
        }
    }
    #endregion

    #region ID Mark Type.
    /// <summary>   ID Mark property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class IDMarkProperty : MarkInfo
    {
        public int MarkType;
        public string PLTFile;
        public int ParamID;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public int ParaNumber;
        public int Location;
        public IDMarkProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {

        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {
            ParaNumber = text.ParaNumber;
            Left = text.DispLeft;
            Top = text.DispTop;
            Width = text.DispRight - text.DispLeft;
            Height = text.DispBottom - text.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = text.RotateAngle;
            Location = loc;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {
        }

        public override int SaveProperty(ref TEXT text)
        {
            text.ParaNumber = ParaNumber;
            text.DispLeft = Left;
            text.DispTop = Top;
            text.DispRight = Left + Width;
            text.DispBottom = Top + Height;
            text.RotateAngle = Rotate;
            return 0;
        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            IDMarkProperty cloneWeekProperty = new IDMarkProperty();
            cloneWeekProperty.ParaNumber = this.ParaNumber;
            cloneWeekProperty.Left = this.Left;
            cloneWeekProperty.Top = this.Top;
            cloneWeekProperty.Width = this.Width;
            cloneWeekProperty.Height = this.Height;
            cloneWeekProperty.Rotate = this.Rotate;
            cloneWeekProperty.Location = this.Location;
            return cloneWeekProperty;
        }
    }
    #endregion

    #region TBD Type.
    /// <summary>   ID Mark property.  </summary>
    /// <remarks>   suoow2, 2017-01-10. </remarks>
    /// 
    public class TBDProperty : MarkInfo
    {
        public int MarkType;
        public int ParamID;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double Rotate;
        public int ParaNumber;
        public int Location;
        public TBDProperty()
        {
        }

        #region Load & Save Properties.

        public override void LoadProperty(Strip strip, HPGL hpgl)
        {

        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, IDMARK idmark)
        {
            ParaNumber = idmark.ParaNumber;
            Left = idmark.DispLeft;
            Top = idmark.DispTop;
            Width = idmark.DispRight - idmark.DispLeft;
            Height = idmark.DispBottom - idmark.DispTop;
            Width = Math.Round(Width, 3);
            Height = Math.Round(Height, 3);
            Rotate = idmark.RotateAngle;
        }

        public override int SaveProperty(ref IDMARK idmark)
        {
            idmark.ParaNumber = ParaNumber;
            idmark.DispLeft = Left;
            idmark.DispTop = Top;
            idmark.DispRight = Left + Width; ;
            idmark.DispBottom = Top + Height;
            idmark.RotateAngle = Rotate;
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {

        }

        public override int SaveProperty(ref TEXT text)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {

        }

        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            TBDProperty cloneUnitRectProperty = new TBDProperty();
            cloneUnitRectProperty.ParaNumber = this.ParaNumber;
            cloneUnitRectProperty.Left = this.Left;
            cloneUnitRectProperty.Top = this.Top;
            cloneUnitRectProperty.Width = this.Width;
            cloneUnitRectProperty.Height = this.Height;
            cloneUnitRectProperty.Rotate = this.Rotate;
            return cloneUnitRectProperty;
        }
    }
    #endregion

    #region ID Mark Type.
    /// <summary>   ID Mark property.  </summary>
    public class UnitGuideProperty : MarkInfo
    {
        public double StartX;
        public double StartY;
        public UnitGuideProperty()
        {
        }

        #region Load & Save Properties.


        public override void LoadProperty(Strip strip, HPGL hpgl)
        {

        }

        public override int SaveProperty(ref HPGL hpgl)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, IDMARK text)
        {

        }

        public override int SaveProperty(ref IDMARK text)
        {
            return 0;
        }

        public override void LoadProperty(Strip strip, TEXT text)
        {

        }

        public override int SaveProperty(ref TEXT text)
        {
            return 0;
        }
        public override void LoadProperty(Strip strip, TEXT text, int loc)
        {

        }
        #endregion
        // Deep Copy.
        public override MarkInfo Clone()
        {
            UnitGuideProperty cloneProperty = new UnitGuideProperty();
            cloneProperty.StartX = this.StartX;
            cloneProperty.StartY = this.StartY;
            return cloneProperty;
        }
    }
    #endregion

}
