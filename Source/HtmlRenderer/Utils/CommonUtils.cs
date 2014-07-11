//BSD 2014, WinterDev 
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
 
using System.Globalization;
using System.IO;
using System.Net;
using System.Text; 

 
namespace HtmlRenderer.Utils
{
    
    
    /// <summary>
    /// Utility methods for general stuff.
    /// </summary>
    internal static class CommonUtils
    {
        #region Fields and Consts

        /// <summary>
        /// Table to convert numbers into roman digits
        /// </summary>
        private static readonly string[,] _romanDigitsTable = new[,]
            {
                {"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"},
                {"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"},
                {"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"},
                {
                    "", "M", "MM", "MMM", "M(V)", "(V)", "(V)M",
                    "(V)MM", "(V)MMM", "M(X)"
                }
            };

        private static readonly string[,] _hebrewDigitsTable = new[,]
            {
                {"א", "ב", "ג", "ד", "ה", "ו", "ז", "ח", "ט"},
                {"י", "כ", "ל", "מ", "נ", "ס", "ע", "פ", "צ"},
                {"ק", "ר", "ש", "ת", "תק", "תר", "תש", "תת", "תתק",}
            };

        private static readonly string[,] _georgianDigitsTable = new[,]
            {
                {"ა", "ბ", "გ", "დ", "ე", "ვ", "ზ", "ჱ", "თ"},
                {"ი", "პ", "ლ", "მ", "ნ", "ჲ", "ო", "პ", "ჟ"},
                {"რ", "ს", "ტ", "ჳ", "ფ", "ქ", "ღ", "ყ", "შ"}
            };

        private static readonly string[,] _armenianDigitsTable = new[,]
            {
                {"Ա", "Բ", "Գ", "Դ", "Ե", "Զ", "Է", "Ը", "Թ"},
                {"Ժ", "Ի", "Լ", "Խ", "Ծ", "Կ", "Հ", "Ձ", "Ղ"},
                {"Ճ", "Մ", "Յ", "Ն", "Շ", "Ո", "Չ", "Պ", "Ջ"}
            };

        private static readonly string[] _hiraganaDigitsTable = new[]
            {
                "あ", "ぃ", "ぅ", "ぇ", "ぉ", "か", "き", "く", "け", "こ", "さ", "し", "す", "せ", "そ", "た", "ち", "つ", "て", "と", "な", "に", "ぬ", "ね", "の", "は", "ひ", "ふ", "へ", "ほ", "ま", "み", "む", "め", "も", "ゃ", "ゅ", "ょ", "ら", "り", "る", "れ", "ろ", "ゎ", "ゐ", "ゑ", "を", "ん"
            };

        private static readonly string[] _satakanaDigitsTable = new[]
            {
                "ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ", "サ", "シ", "ス", "セ", "ソ", "タ", "チ", "ツ", "テ", "ト", "ナ", "ニ", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ", "マ", "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ", "ル", "レ", "ロ", "ワ", "ヰ", "ヱ", "ヲ", "ン"
            };

        #endregion


        /// <summary>
        /// Check if the given char is of Asian range.
        /// </summary>
        /// <param name="ch">the character to check</param>
        /// <returns>true - Asian char, false - otherwise</returns>
        public static bool IsAsianCharecter(char ch)
        {
            return ch >= 0x4e00 && ch <= 0xFA2D;
        } 
        

        /// <summary>
        /// Get Uri object for the given path if it is valid uri path.
        /// </summary>
        /// <param name="path">the path to get uri for</param>
        /// <returns>uri or null if not valid</returns>
        public static Uri TryGetUri(string path)
        {
            try
            {
                if (Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
                {
                    return new Uri(path);
                }
            }
            catch
            { }

            return null;
        }

        /// <summary>
        /// Get the first value in the given dictionary.
        /// </summary>
        /// <typeparam name="TKey">the type of dictionary key</typeparam>
        /// <typeparam name="TValue">the type of dictionary value</typeparam>
        /// <param name="dic">the dictionary</param>
        /// <param name="defaultValue">optional: the default value to return of no elements found in dictionary </param>
        /// <returns>first element or default value</returns>
        public static TValue GetFirstValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dic, TValue defaultValue = default(TValue))
        {
            if (dic != null)
            {
                foreach (var value in dic)
                {
                    return value.Value;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Get file info object for the given path if it is valid file path.
        /// </summary>
        /// <param name="path">the path to get file info for</param>
        /// <returns>file info or null if not valid</returns>
        public static FileInfo TryGetFileInfo(string path)
        {
            try
            {
                return new FileInfo(path);
            }
            catch
            { }

            return null;
        }

        /// <summary>
        /// Get web client response content type.
        /// </summary>
        /// <param name="client">the web client to get the response content type from</param>
        /// <returns>response content type or null</returns>
        public static string GetResponseContentType(WebClient client)
        {
            foreach (string header in client.ResponseHeaders)
            {
                if (header.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                    return client.ResponseHeaders[header];

            }
            return null;
        }

        /// <summary>
        /// Gets the representation of the online uri on the local disk.
        /// </summary>
        /// <param name="imageUri">The online image uri.</param>
        /// <returns>The path of the file on the disk.</returns>
        public static FileInfo GetLocalfileName(Uri imageUri)
        {
            StringBuilder fileNameBuilder = new StringBuilder();
            string absoluteUri = imageUri.AbsoluteUri;
            int lastSlash = absoluteUri.LastIndexOf('/');
            if (lastSlash == -1)
            {
                return null;
            }

            string uriUntilSlash = absoluteUri.Substring(0, lastSlash);
            fileNameBuilder.Append(uriUntilSlash.GetHashCode().ToString());
            fileNameBuilder.Append('_');

            string restOfUri = absoluteUri.Substring(lastSlash + 1);
            int indexOfParams = restOfUri.IndexOf('?');
            if (indexOfParams == -1)
            {
                string ext = ".cache";
                int indexOfDot = restOfUri.IndexOf('.');
                if (indexOfDot > -1)
                {
                    ext = restOfUri.Substring(indexOfDot);
                    restOfUri = restOfUri.Substring(0, indexOfDot);
                }

                fileNameBuilder.Append(restOfUri);
                fileNameBuilder.Append(ext);
            }
            else
            {
                int indexOfDot = restOfUri.IndexOf('.');
                if (indexOfDot == -1 || indexOfDot > indexOfParams)
                {
                    //The uri is not for a filename
                    fileNameBuilder.Append(restOfUri);
                    fileNameBuilder.Append(".cache");
                }
                else if (indexOfParams > indexOfDot)
                {
                    //Adds the filename without extension.
                    fileNameBuilder.Append(restOfUri, 0, indexOfDot);
                    //Adds the parameters
                    fileNameBuilder.Append(restOfUri, indexOfParams, restOfUri.Length - indexOfParams);
                    //Adds the filename extension.
                    fileNameBuilder.Append(restOfUri, indexOfDot, indexOfParams - indexOfDot);
                }
            }

            var validFileName = GetValidFileName(fileNameBuilder.ToString());
            if (validFileName.Length > 25)
            {
                validFileName = validFileName.Substring(0, 24) + validFileName.Substring(24).GetHashCode() + Path.GetExtension(validFileName);
            }

            return new FileInfo(Path.Combine(Path.GetTempPath(), validFileName));
        }

        /// <summary>
        /// Get substring seperated by whitespace starting from the given idex.
        /// </summary>
        /// <param name="str">the string to get substring in</param>
        /// <param name="idx">the index to start substring search from</param>
        /// <param name="length">return the length of the found string</param>
        /// <returns>the index of the substring, -1 if no valid sub-string found</returns>
        public static int GetNextSubString(string str, int idx, out int length)
        {
            while (idx < str.Length && Char.IsWhiteSpace(str[idx]))
                idx++;
            if (idx < str.Length)
            {
                var endIdx = idx + 1;
                while (endIdx < str.Length && !Char.IsWhiteSpace(str[endIdx]))
                    endIdx++;
                length = endIdx - idx;
                return idx;
            }
            length = 0;
            return -1;
        }

        /// <summary>
        /// Compare that the substring of <paramref name="str"/> is equal to <paramref name="str"/>
        /// Assume given substring is not empty and all indexes are valid!<br/>
        /// </summary>
        /// <returns>true - equals, false - not equals</returns>
        public static bool SubStringEquals(string str, int idx, int length, string str2)
        {
            if (length == str2.Length && idx + length <= str.Length)
            {
                for (int i = 0; i < length; i++)
                {
                    if (Char.ToLowerInvariant(str[idx + i]) != Char.ToLowerInvariant(str2[i]))
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Replaces invalid filename chars to '_'
        /// </summary>
        /// <param name="source">The possibly-not-valid filename</param>
        /// <returns>A valid filename.</returns>
        private static string GetValidFileName(string source)
        {
            string retVal = source;
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (var invalidFileNameChar in invalidFileNameChars)
            {
                retVal = retVal.Replace(invalidFileNameChar, '_');
            }
            return retVal;
        }

        /// <summary>
        /// Convert number to alpha numeric system by the requested style (UpperAlpha, LowerRoman, Hebrew, etc.).
        /// </summary>
        /// <param name="number">the number to convert</param>
        /// <param name="style">the css style to convert by</param>
        /// <returns>converted string</returns>
        public static string ConvertToAlphaNumber(int number, HtmlRenderer.Css.CssListStyleType style) // =   string style = CssConstants.UpperAlpha)
        {
            if (number == 0)
                return string.Empty;
            switch (style)
            {
                case Css.CssListStyleType.LowerGreek:
                    return ConvertToGreekNumber(number);
                case Css.CssListStyleType.LowerRoman:
                    return ConvertToRomanNumbers(number, true);
                case Css.CssListStyleType.UpperRoman:
                    return ConvertToRomanNumbers(number, false);
                case Css.CssListStyleType.Armenian:
                    return ConvertToSpecificNumbers(number, _armenianDigitsTable);
                case Css.CssListStyleType.Georgian:
                    return ConvertToSpecificNumbers(number, _georgianDigitsTable);
                case Css.CssListStyleType.Hebrew:
                      return ConvertToSpecificNumbers(number, _hebrewDigitsTable);
                case Css.CssListStyleType.Hiragana:
                case Css.CssListStyleType.HiraganaIroha:
                    return ConvertToSpecificNumbers2(number, _hiraganaDigitsTable);
                case Css.CssListStyleType.Katakana:
                case Css.CssListStyleType.KatakanaIroha:
                   return ConvertToSpecificNumbers2(number, _satakanaDigitsTable);
                case Css.CssListStyleType.LowerAlpha:
                case Css.CssListStyleType.LowerLatin:
                    return ConvertToEnglishNumber(number, true);
                default:
                    return ConvertToEnglishNumber(number, false);
            }
             
        }

        /// <summary>
        /// Convert the given integer into alphabetic numeric format (D, AU, etc.)
        /// </summary>
        /// <param name="number">the number to convert</param>
        /// <param name="lowercase">is to use lowercase</param>
        /// <returns>the roman number string</returns>
        private static string ConvertToEnglishNumber(int number, bool lowercase)
        {
            var sb = string.Empty;
            int alphStart = lowercase ? 97 : 65;
            while (number > 0)
            {
                var n = number % 26 - 1;
                if (n >= 0)
                {
                    sb = (Char)(alphStart + n) + sb;
                    number = number / 26;
                }
                else
                {
                    sb = (Char)(alphStart + 25) + sb;
                    number = (number - 1) / 26;
                }
            }

            return sb;
        }

        /// <summary>
        /// Convert the given integer into alphabetic numeric format (alpha, AU, etc.)
        /// </summary>
        /// <param name="number">the number to convert</param>
        /// <returns>the roman number string</returns>
        private static string ConvertToGreekNumber(int number)
        {
            var sb = string.Empty;
            while (number > 0)
            {
                var n = number % 24 - 1;
                if (n > 16)
                    n++;
                if (n >= 0)
                {
                    sb = (Char)(945 + n) + sb;
                    number = number / 24;
                }
                else
                {
                    sb = (Char)(945 + 24) + sb;
                    number = (number - 1) / 25;
                }
            }

            return sb;
        }

        /// <summary>
        /// Convert the given integer into roman numeric format (II, VI, IX, etc.)
        /// </summary>
        /// <param name="number">the number to convert</param>
        /// <param name="lowercase">if to use lowercase letters for roman digits</param>
        /// <returns>the roman number string</returns>
        private static string ConvertToRomanNumbers(int number, bool lowercase)
        {
            var sb = string.Empty;
            for (int i = 1000, j = 3; i > 0; i /= 10, j--)
            {
                int digit = number / i;
                sb += string.Format(_romanDigitsTable[j, digit]);
                number -= digit * i;
            }
            return lowercase ? sb.ToLower() : sb;
        }

        /// <summary>
        /// Convert the given integer into given alphabet numeric system.
        /// </summary>
        /// <param name="number">the number to convert</param>
        /// <param name="alphabet">the alphabet system to use</param>
        /// <returns>the number string</returns>
        private static string ConvertToSpecificNumbers(int number, string[,] alphabet)
        {
            int level = 0;
            var sb = string.Empty;
            while (number > 0 && level < alphabet.GetLength(0))
            {
                var n = number % 10;
                if (n > 0)
                    sb = alphabet[level, number % 10 - 1].ToString(CultureInfo.InvariantCulture) + sb;
                number /= 10;
                level++;
            }
            return sb;
        }

        /// <summary>
        /// Convert the given integer into given alphabet numeric system.
        /// </summary>
        /// <param name="number">the number to convert</param>
        /// <param name="alphabet">the alphabet system to use</param>
        /// <returns>the number string</returns>
        private static string ConvertToSpecificNumbers2(int number, string[] alphabet)
        {
            for (int i = 20; i > 0; i--)
            {
                if (number > 49 * i - i + 1)
                    number++;
            }

            var sb = string.Empty;
            while (number > 0)
            {


                sb = alphabet[Math.Max(0, number % 49 - 1)].ToString(CultureInfo.InvariantCulture) + sb;
                number /= 49;
            }
            return sb;
        }
    }
}
