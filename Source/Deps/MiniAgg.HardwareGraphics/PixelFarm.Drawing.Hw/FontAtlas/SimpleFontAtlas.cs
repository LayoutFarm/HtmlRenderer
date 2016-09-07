//MIT,2016, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
using System.Xml;
namespace PixelFarm.Drawing.Fonts
{
    public class SimpleFontAtlas
    {
        GlyphImage totalGlyphImage;
        Dictionary<char, Rectangle> charLocations = new Dictionary<char, Rectangle>();

        public int Width { get; set; }
        public int Height { get; set; }
        public void AddGlyph(char c, Rectangle rect)
        {
            charLocations.Add(c, rect);
        }
        public GlyphImage TotalGlyph
        {
            get { return totalGlyphImage; }
            set { totalGlyphImage = value; }

        }
        public bool GetRect(char c, out Rectangle rect)
        {
            if (!charLocations.TryGetValue(c, out rect))
            {
                rect = Rectangle.Empty;
                return false;
            }
            return true;
        }

    }
    public class SimpleFontAtlasBuilder
    {
        Dictionary<char, GlyphData> glyphs = new Dictionary<char, GlyphData>();
        GlyphImage latestGenGlyphImage;
        public void AddGlyph(char c, FontGlyph fontGlyph, GlyphImage glyphImage)
        {
            var glyphData = new GlyphData(c, fontGlyph, glyphImage);
            glyphs[c] = glyphData;
        }

        public GlyphImage GetLatestGenGlyphImage()
        {
            return latestGenGlyphImage;
        }
        public GlyphImage BuildSingleImage()
        {
            //1. add to list 
            var glyphList = new List<GlyphData>(glyphs.Count);
            foreach (GlyphData glyphData in glyphs.Values)
            {
                //sort data
                glyphList.Add(glyphData);
            }
            //2. sort
            glyphList.Sort((a, b) =>
            {
                return a.glyphImage.Width.CompareTo(b.glyphImage.Width);
            });
            //3. layout

            int totalMaxLim = 800;
            int maxRowHeight = 0;
            int currentY = 0;
            int currentX = 0;
            for (int i = glyphList.Count - 1; i >= 0; --i)
            {
                GlyphData g = glyphList[i];
                if (g.glyphImage.Height > maxRowHeight)
                {
                    maxRowHeight = g.glyphImage.Height;
                }
                if (currentX + g.glyphImage.Width > totalMaxLim)
                {
                    //start new row
                    currentY += maxRowHeight;
                    currentX = 0;
                }
                //-------------------
                g.pxArea = new Rectangle(currentX, currentY, g.glyphImage.Width, g.glyphImage.Height);
                currentX += g.glyphImage.Width;
            }

            currentY += maxRowHeight;
            int imgH = currentY;



            //4. create array that can hold data
            int[] totalBuffer = new int[totalMaxLim * imgH];
            for (int i = glyphList.Count - 1; i >= 0; --i)
            {
                GlyphData g = glyphList[i];
                //copy data to totalBuffer
                GlyphImage img = g.glyphImage;
                CopyToDest(img.GetImageBuffer(), img.Width, img.Height, totalBuffer, g.pxArea.Left, g.pxArea.Top, totalMaxLim);
            }
            //------------------
            GlyphImage glyphImage = new Fonts.GlyphImage(totalMaxLim, imgH);
            glyphImage.SetImageBuffer(totalBuffer, true);
            return latestGenGlyphImage = glyphImage;
        }
        static void CopyToDest(int[] srcPixels, int srcW, int srcH, int[] targetPixels, int targetX, int targetY, int totalTargetWidth)
        {
            int srcIndex = 0;
            unsafe
            {

                for (int r = 0; r < srcH; ++r)
                {
                    //for each row 
                    int targetP = ((targetY + r) * totalTargetWidth) + targetX;
                    for (int c = 0; c < srcW; ++c)
                    {
                        targetPixels[targetP] = srcPixels[srcIndex];
                        srcIndex++;
                        targetP++;
                    }
                }
            }
        }


        /// <summary>
        /// save font info into xml document
        /// </summary>
        /// <param name="filename"></param>
        public void SaveFontInfo(string filename)
        {
            //save font info as xml 
            //save position of each font
            XmlDocument xmldoc = new XmlDocument();
            XmlElement root = xmldoc.CreateElement("font");
            xmldoc.AppendChild(root);

            if (latestGenGlyphImage == null)
            {
                BuildSingleImage();
            }

            {

                //total img element
                XmlElement totalImgElem = xmldoc.CreateElement("total_img");
                totalImgElem.SetAttribute("w", latestGenGlyphImage.Width.ToString());
                totalImgElem.SetAttribute("h", latestGenGlyphImage.Height.ToString());
                totalImgElem.SetAttribute("compo", "4");
                root.AppendChild(totalImgElem);
            }

            foreach (GlyphData g in glyphs.Values)
            {
                XmlElement gElem = xmldoc.CreateElement("glyph");
                //convert char to hex
                string unicode = ("0x" + ((int)g.character).ToString("X")); //accept unicode char
                Rectangle area = g.pxArea;
                gElem.SetAttribute("uc", unicode);//unicode char
                gElem.SetAttribute("ltwh",
                    area.Left + " " + area.Top + " " + area.Width + " " + area.Height
                    );
                gElem.SetAttribute("offset",
                    g.glyphImage.OffsetX + " " + g.glyphImage.OffsetY
                    );
                if (g.character > 50)
                {
                    gElem.SetAttribute("example", g.character.ToString());
                }
                root.AppendChild(gElem);
            }



            //if (embededGlyphsImage)
            //{
            //    XmlElement glyphImgElem = xmldoc.CreateElement("msdf_img");
            //    glyphImgElem.SetAttribute("w", latestGenGlyphImage.Width.ToString());
            //    glyphImgElem.SetAttribute("h", latestGenGlyphImage.Height.ToString());
            //    int[] imgBuffer = latestGenGlyphImage.GetImageBuffer();
            //    glyphImgElem.SetAttribute("buff_len", (imgBuffer.Length * 4).ToString());
            //    //----------------------------------------------------------------------
            //    glyphImgElem.AppendChild(
            //        xmldoc.CreateTextNode(ConvertToBase64(imgBuffer)));
            //    //----------------------------------------------------------------------
            //    root.AppendChild(glyphImgElem);
            //    latestGenGlyphImage.GetImageBuffer();
            //}
            xmldoc.Save(filename);
        }
        static string ConvertToBase64(int[] buffer)
        {
            byte[] copy1 = new byte[buffer.Length * 4];
            unsafe
            {
                fixed (void* buff_h = &buffer[0])
                {
                    System.Runtime.InteropServices.Marshal.Copy(
                         new IntPtr(buff_h), copy1, 0, copy1.Length);
                }
            }
            return Convert.ToBase64String(copy1);
        }



        //read font info from xml document
        public SimpleFontAtlas LoadFontInfo(string filename)
        {
            SimpleFontAtlas simpleFontAtlas = new Fonts.SimpleFontAtlas();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);
            //read
            int total_W = 0;
            int total_H = 0;
            {

                foreach (XmlElement xmlelem in xmldoc.GetElementsByTagName("total_img"))
                {
                    simpleFontAtlas.Width = total_W = int.Parse(xmlelem.GetAttribute("w"));
                    simpleFontAtlas.Height = total_H = int.Parse(xmlelem.GetAttribute("h"));
                    //only 1...

                    break;
                }
            }
            foreach (XmlElement glyphElem in xmldoc.GetElementsByTagName("glyph"))
            {
                //read
                string unicodeHex = glyphElem.GetAttribute("uc");
                char c = (char)int.Parse(unicodeHex.Substring(2), System.Globalization.NumberStyles.HexNumber);
                Rectangle area = ParseRect(glyphElem.GetAttribute("ltwh"));

                area.Y += area.Height;//***

                simpleFontAtlas.AddGlyph(c, area);
            }
            return simpleFontAtlas;
        }
        static Rectangle ParseRect(string str)
        {
            string[] ltwh = str.Split(' ');
            return new Rectangle(
                int.Parse(ltwh[0]),
                int.Parse(ltwh[1]),
                int.Parse(ltwh[2]),
                int.Parse(ltwh[3]));

        }
    }
    class GlyphData
    {
        public FontGlyph fontGlyph;
        public GlyphImage glyphImage;
        public Rectangle pxArea;
        public char character;
        public GlyphData(char c, FontGlyph fontGlyph, GlyphImage glyphImage)
        {
            this.character = c;
            this.fontGlyph = fontGlyph;
            this.glyphImage = glyphImage;

        }
    }

}