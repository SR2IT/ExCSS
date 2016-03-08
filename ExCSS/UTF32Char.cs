using System;
using System.Diagnostics.Contracts;

namespace ExCSS
{
    /// <summary>
    /// UTF-32 support for <see cref="Char"/> in Silverlight and other portable targets.
    /// </summary>
    public class UTF32Char
    {
        /****************************************************************************************************
        /* Extracted from CharUnicodeInfo.cs                                                                *
        ****************************************************************************************************/
        private const char CHAR_HIGH_SURROGATE_START = '\ud800';
        private const char CHAR_HIGH_SURROGATE_END = '\udbff';
        private const char CHAR_LOW_SURROGATE_START = '\udc00';
        private const char CHAR_LOW_SURROGATE_END = '\udfff';

        /****************************************************************************************************
        /* Extracted from Char.cs                                                                           *
        ****************************************************************************************************/
        private const int UNICODE_PLANE00_END = 0x00ffff;
        // The starting codepoint for Unicode plane 1.  Plane 1 contains 0x010000 ~ 0x01ffff.
        private const int UNICODE_PLANE01_START = 0x10000;
        // The end codepoint for Unicode plane 16.  This is the maximum code point value allowed for Unicode.
        // Plane 16 contains 0x100000 ~ 0x10ffff. 
        private const int UNICODE_PLANE16_END = 0x10ffff;

        private const int HIGH_SURROGATE_START = 0x00d800;
        private const int LOW_SURROGATE_END = 0x00dfff;

        /// <summary>
        /// Indicates whether the specified <see cref="Char"/> object is a high surrogate.
        /// </summary>
        /// <param name="c">
        /// The Unicode character to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the numeric value of the <paramref name="c"/> parameter ranges from U+D800
        /// through U+DBFF; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsHighSurrogate(char c)
        {
            return ((c >= CHAR_HIGH_SURROGATE_START) && (c <= CHAR_HIGH_SURROGATE_END));
        }

        /// <summary>
        /// Indicates whether the <see cref="Char"/> object at the specified position in a string is
        /// a high surrogate.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a position within <paramref name="s"/>.
        /// </exception>
        /// <param name="s">
        /// A string.
        /// </param>
        /// <param name="index">
        /// The position of the character to evaluate in <paramref name="s"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the numeric value of the specified character in the <paramref name="s"/>
        /// parameter ranges from U+D800 through U+DBFF; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsHighSurrogate(string s, int index)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (index < 0 || index >= s.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            Contract.EndContractBlock();
            return (IsHighSurrogate(s[index]));
        }

        /// <summary>
        /// Indicates whether the specified <see cref="Char"/> object is a low surrogate.
        /// </summary>
        /// <param name="c">
        /// The Unicode character to evaluate.
        /// </param>
        /// <returns>
        /// <c>true</c> if the numeric value of the <paramref name="c"/> parameter ranges from U+DC00
        /// through U+DFFF; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsLowSurrogate(char c)
        {
            return ((c >= CHAR_LOW_SURROGATE_START) && (c <= CHAR_LOW_SURROGATE_END));
        }

        /// <summary>
        /// Indicates whether the <see cref="Char"/> object at the specified position in a string is
        /// a low surrogate.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a position within <paramref name="s"/>.
        /// </exception>
        /// <param name="s">
        /// A string.
        /// </param>
        /// <param name="index">
        /// The position of the character to evaluate in <paramref name="s"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the numeric value of the specified character in the <paramref name="s"/>
        /// parameter ranges from U+DC00 through U+DFFF; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsLowSurrogate(string s, int index)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (index < 0 || index >= s.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            Contract.EndContractBlock();
            return (IsLowSurrogate(s[index]));
        }


        /// <summary>
        /// Converts the specified Unicode code point into a UTF-16 encoded string.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="utf32"/> is not a valid 21-bit Unicode code point ranging from U+0 through
        /// U+10FFFF, excluding the surrogate pair range from U+D800 through U+DFFF.
        /// </exception>
        /// <param name="utf32">
        /// A 21-bit Unicode code point.
        /// </param>
        /// <returns>
        /// A string consisting of one <see cref="Char"/> object or a surrogate pair of <see cref="Char"/>
        /// objects equivalent to the code point specified by the <paramref name="utf32"/> parameter.
        /// </returns>
        public static string ConvertFromUtf32(int utf32)
        {
            // For UTF32 values from U+00D800 ~ U+00DFFF, we should throw.  They 
            // are considered as irregular code unit sequence, but they are not illegal.
            if ((utf32 < 0 || utf32 > UNICODE_PLANE16_END) || (utf32 >= HIGH_SURROGATE_START && utf32 <= LOW_SURROGATE_END))
            {
                throw new ArgumentOutOfRangeException("utf32", global::ExCSS.Properties.Resources.ArgumentOutOfRange_InvalidUTF32);
            }
            Contract.EndContractBlock();

            if (utf32 < UNICODE_PLANE01_START)
            {
                // This is a BMP character.
                return (Char.ToString((char)utf32));
            }
            // This is a sumplementary character.  Convert it to a surrogate pair in UTF-16.
            utf32 -= UNICODE_PLANE01_START;
            char[] surrogate = new char[2];
            surrogate[0] = (char)((utf32 / 0x400) + (int)CHAR_HIGH_SURROGATE_START);
            surrogate[1] = (char)((utf32 % 0x400) + (int)CHAR_LOW_SURROGATE_START);
            return (new string(surrogate));
        }

        /// <summary>
        /// Converts the value of a UTF-16 encoded surrogate pair into a Unicode code point.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="highSurrogate"/> is not in the range U+D800 through U+DBFF, or <paramref name="lowSurrogate"/>
        /// is not in the range U+DC00 through U+DFFF.
        /// </exception>
        /// <param name="highSurrogate">
        /// A high surrogate code unit (that is, a code unit ranging from U+D800 through U+DBFF).
        /// </param>
        /// <param name="lowSurrogate">
        /// A low surrogate code unit (that is, a code unit ranging from U+DC00 through U+DFFF).
        /// </param>
        /// <returns>
        /// The 21-bit Unicode code point represented by the <paramref name="highSurrogate"/> and <paramref name="lowSurrogate"/>
        /// parameters.
        /// </returns>
        public static int ConvertToUtf32(char highSurrogate, char lowSurrogate)
        {
            if (!IsHighSurrogate(highSurrogate))
            {
                throw new ArgumentOutOfRangeException("highSurrogate", global::ExCSS.Properties.Resources.ArgumentOutOfRange_InvalidHighSurrogate);
            }
            if (!IsLowSurrogate(lowSurrogate))
            {
                throw new ArgumentOutOfRangeException("lowSurrogate", global::ExCSS.Properties.Resources.ArgumentOutOfRange_InvalidLowSurrogate);
            }
            Contract.EndContractBlock();
            return (((highSurrogate - CHAR_HIGH_SURROGATE_START) * 0x400) + (lowSurrogate - CHAR_LOW_SURROGATE_START) + UNICODE_PLANE01_START);
        }

        /// <summary>
        /// Converts the value of a UTF-16 encoded character or surrogate pair at a specified position
        /// in a string into a Unicode code point.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a position within <paramref name="s"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The specified index position contains a surrogate pair, and either the first character in
        /// the pair is not a valid high surrogate or the second character in the pair is not a valid
        /// low surrogate.
        /// </exception>
        /// <param name="s">
        /// A string that contains a character or surrogate pair.
        /// </param>
        /// <param name="index">
        /// The index position of the character or surrogate pair in <paramref name="s"/>.
        /// </param>
        /// <returns>
        /// The 21-bit Unicode code point represented by the character or surrogate pair at the position
        /// in the <paramref name="s"/> parameter specified by the <paramref name="index"/> parameter.
        /// </returns>
        public static int ConvertToUtf32(String s, int index)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            if (index < 0 || index >= s.Length)
            {
                throw new ArgumentOutOfRangeException("index", global::ExCSS.Properties.Resources.ArgumentOutOfRange_Index);
            }
            Contract.EndContractBlock();
            // Check if the character at index is a high surrogate. 
            int temp1 = (int)s[index] - CHAR_HIGH_SURROGATE_START;
            if (temp1 >= 0 && temp1 <= 0x7ff)
            {
                // Found a surrogate char.
                if (temp1 <= 0x3ff)
                {
                    // Found a high surrogate.
                    if (index < s.Length - 1)
                    {
                        int temp2 = (int)s[index + 1] - CHAR_LOW_SURROGATE_START;
                        if (temp2 >= 0 && temp2 <= 0x3ff)
                        {
                            // Found a low surrogate. 
                            return ((temp1 * 0x400) + temp2 + UNICODE_PLANE01_START);
                        }
                        else {
                            throw new ArgumentException(string.Format(global::ExCSS.Properties.Resources.Argument_InvalidHighSurrogate, index), "s");
                        }
                    }
                    else {
                        // Found a high surrogate at the end of the string. 
                        throw new ArgumentException(string.Format(global::ExCSS.Properties.Resources.Argument_InvalidHighSurrogate, index), "s");
                    }
                }
                else {
                    // Find a low surrogate at the character pointed by index.
                    throw new ArgumentException(string.Format(global::ExCSS.Properties.Resources.Argument_InvalidLowSurrogate, index), "s");
                }
            }
            // Not a high-surrogate or low-surrogate. Genereate the UTF32 value for the BMP characters.
            return ((int)s[index]);
        }
    }
}
