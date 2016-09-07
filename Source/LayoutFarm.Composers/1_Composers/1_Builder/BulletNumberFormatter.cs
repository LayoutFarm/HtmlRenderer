//BSD, 2014-2016, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

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
using System.Globalization;
namespace LayoutFarm.Composers
{
    static class BulletNumberFormatter
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
        /// Convert number to alpha numeric system by the requested style (UpperAlpha, LowerRoman, Hebrew, etc.).
        /// </summary>
        /// <param name="number">the number to convert</param>
        /// <param name="style">the css style to convert by</param>
        /// <returns>converted string</returns>
        public static string ConvertToAlphaNumber(int number, LayoutFarm.Css.CssListStyleType style) // =   string style = CssConstants.UpperAlpha)
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
                    //TODO: review here
                    sb = Convert.ToString(alphabet[level, number % 10 - 1], CultureInfo.InvariantCulture) + sb;
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
                sb = Convert.ToString(alphabet[Math.Max(0, number % 49 - 1)], CultureInfo.InvariantCulture) + sb;
                number /= 49;
            }
            return sb;
        }
    }
}
