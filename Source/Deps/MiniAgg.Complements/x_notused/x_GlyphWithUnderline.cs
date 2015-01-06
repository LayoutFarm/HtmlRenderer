////2014,2015 BSD,WinterDev   
////----------------------------------------------------------------------------
//// Anti-Grain Geometry - Version 2.4
////
//// C# Port port by: Lars Brubaker
////                  larsbrubaker@gmail.com
//// Copyright (C) 2007-2011
////
//// Permission to copy, use, modify, sell and distribute this software 
//// is granted provided this copyright notice appears in all copies. 
//// This software is provided "as is" without express or implied
//// warranty, and with no claim as to its suitability for any purpose.
////
////----------------------------------------------------------------------------
////
//// Class StyledTypeFace.cs
////
////----------------------------------------------------------------------------
//using System;
//using System.Collections.Generic;
//using System.Text;

//using MatterHackers.Agg;
//using MatterHackers.Agg.Transform;
//using MatterHackers.Agg.VertexSource;
//using MatterHackers.Agg.Image;

//namespace MatterHackers.Agg.Font
//{  //public class GlyphWithUnderline
//    //{

//    //    VertexStoreSnap underline;
//    //    VertexStoreSnap glyph;

//    //    public GlyphWithUnderline(VertexStore glyph, int advanceForCharacter, int Underline_position, int Underline_thickness)
//    //    {
//    //        underline = new VertexStoreSnap(
//    //            new RoundedRect(new RectangleDouble(0,
//    //                Underline_position, advanceForCharacter,
//    //                Underline_position + Underline_thickness), 0).MakeVxs());
//    //        this.glyph = new VertexStoreSnap(glyph);
//    //    }

//    //    public VertexStore MakeVxs()
//    //    {
//    //        return new VertexStore(this.GetVertexIter());             
//    //    }

//    //    public IEnumerable<VertexData> GetVertexIter()
//    //    {
//    //        ShapePath.FlagsAndCommand cmd;
//    //        double x, y;

//    //        // return all the data for the glyph
//    //        var snapIter = glyph.GetVertexSnapIter();
//    //        do
//    //        {
//    //            cmd = snapIter.GetNextVertex(out x, out y);
//    //            if (ShapePath.IsStop(cmd))
//    //            {
//    //                yield return new VertexData(cmd, x, y);
//    //                break;
//    //            }
//    //            else
//    //            {
//    //                yield return new VertexData(cmd, x, y);
//    //            }

//    //        } while (cmd != ShapePath.FlagsAndCommand.CommandStop);


//    //        snapIter = underline.GetVertexSnapIter();
//    //        do
//    //        {
//    //            cmd = snapIter.GetNextVertex(out x, out y);
//    //            yield return new VertexData(cmd, x, y);

//    //        } while (cmd != ShapePath.FlagsAndCommand.CommandStop);
//    //    }

//    //}

//}