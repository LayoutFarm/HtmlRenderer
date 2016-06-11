////2014,2015 BSD,WinterDev   
//using System;
//using System.Collections.Generic; 
//using System.Text;

//namespace PixelFarm.Agg.Font
//{
//    abstract public class TextWrapping
//    {
//        protected StyledTypeFace styledTypeFace;


//        public TextWrapping(StyledTypeFace styledTypeFace)
//        {
//            this.styledTypeFace = styledTypeFace;
//        }

//        public string InsertCRs(string textToWrap, double maxPixelWidth)
//        {
//            StringBuilder textWithCRs = new StringBuilder();
//            string[] lines = WrapText(textToWrap, maxPixelWidth);
//            for (int i = 0; i < lines.Length; i++)
//            {
//                string line = lines[i];
//                if (i == 0)
//                {
//                    textWithCRs.Append(line);
//                }
//                else
//                {
//                    textWithCRs.Append("\n" + line);
//                }
//            }

//            return textWithCRs.ToString();
//        }

//        public string[] WrapText(string textToWrap, double maxPixelWidth)
//        {
//            List<string> finalLines = new List<string>();
//            string[] splitOnNL = textToWrap.Split('\n');
//            foreach (string line in splitOnNL)
//            {
//                string[] linesFromWidth = WrapSingleLineOnWidth(line, maxPixelWidth);
//                if (linesFromWidth.Length == 0)
//                {
//                    finalLines.Add("");
//                }
//                else
//                {
//                    finalLines.AddRange(linesFromWidth);
//                }
//            }

//            return finalLines.ToArray();
//        }

//        abstract public string[] WrapSingleLineOnWidth(string originalTextToWrap, double maxPixelWidth);
//    }

//    public class EnglishTextWrapping : TextWrapping
//    {
//        public EnglishTextWrapping(StyledTypeFace styledTypeFace)
//            : base(styledTypeFace)
//        {
//        }

//        public EnglishTextWrapping(double pointSize)
//            : base(new StyledTypeFace(LiberationSansFont.Instance, pointSize))
//        {
//        }

//        public override string[] WrapSingleLineOnWidth(string originalTextToWrap, double maxPixelWidth)
//        {
//            List<string> lines = new List<string>();

//            if (maxPixelWidth > 0)
//            {
//                string textToWrap = originalTextToWrap;
//                while (textToWrap.Length > 0)
//                {
//                    TypeFacePrinter printer = new TypeFacePrinter(textToWrap);
//                    int remainingLength = textToWrap.Length;
//                    while (printer.GetSize().x > maxPixelWidth)
//                    {
//                        remainingLength--;
//                        while (remainingLength > 1
//                            && textToWrap.Substring(0, remainingLength).Contains(" ")
//                            && textToWrap[remainingLength] != ' ')
//                        {
//                            remainingLength--;
//                        }

//                        printer.Text = textToWrap.Substring(0, remainingLength);
//                    }

//                    if (remainingLength >= 0)
//                    {
//                        lines.Add(textToWrap.Substring(0, remainingLength));
//                    }


//                    // check if we wrapped because of to long or a '\n'. If '\n' we only trim a leading space if to long.
//                    if (remainingLength > 1 // we have more than 2 charecters left
//                        && textToWrap.Length > remainingLength // we are longer than the remaining text
//                        && textToWrap[remainingLength] == ' ' // the first new character is a space
//                        && textToWrap[remainingLength - 1] != '\n') // the character before the space was not a cr (wrapped because of length)
//                    {
//                        textToWrap = textToWrap.Substring(remainingLength + 1);
//                    }
//                    else
//                    {
//                        textToWrap = textToWrap.Substring(remainingLength);
//                    }
//                }
//            }
//            else
//            {
//                lines.Add(originalTextToWrap);
//            }

//            return lines.ToArray();
//        }
//    }
//}
