namespace Common.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 修正UTF7変換クラス
    /// IMAP4のメールボックスを扱うのに使う。
    /// </summary>
    public static class ModifiedUTF7
    {
        public static string ToModifiedUTF7(string str)
        {
            var encoded = new StringBuilder();
            int shiftFrom = -1;

            for (int index = 0; index < str.Length; index++)
            {
                char c = str[index];

                if ((0x20 <= c && c <= 0x7e))
                {
                    if (0 <= shiftFrom)
                    {
                        // string -> modified UTF7
                        encoded.Append('&');
                        encoded.Append(ToModifiedBase64(str.Substring(shiftFrom, index - shiftFrom)));
                        encoded.Append('-');

                        shiftFrom = -1;
                    }

                    // printable US-ASCII characters
                    if (c == 0x26)
                    {
                        // except for "&"
                        encoded.Append("&-");
                    }
                    else
                    {
                        encoded.Append(c);
                    }
                }
                else
                {
                    if (shiftFrom == -1)
                    {
                        shiftFrom = index;
                    }
                }
            }

            if (0 <= shiftFrom)
            {
                // string -> modified UTF7
                encoded.Append('&');
                encoded.Append(ToModifiedBase64(str.Substring(shiftFrom)));
                encoded.Append('-');
            }

            return encoded.ToString();
        }

        private static string ToModifiedBase64(string str)
        {
            byte[] bytes = Encoding.BigEndianUnicode.GetBytes(str);
            string base64 = Convert.ToBase64String(bytes).Replace('/', ',');
            int padding = base64.IndexOf('=');

            if (padding < 0)
            {
                return base64;
            }
            else
            {
                return base64.Substring(0, padding);
            }
        }

        public static string FromModifiedUTF7(string str)
        {
            if (!str.Contains("&"))
            {
                return str;
            }

            byte[] bytes = Encoding.ASCII.GetBytes(str);
            var decoded = new StringBuilder();

            for (int index = 0; index < bytes.Length; index++)
            {
                // In modified UTF-7, printable US-ASCII characters, except for "&",
                // represent themselves
                // "&" is used to shift to modified BASE64
                if (bytes[index] != 0x26)
                { // '&'
                    decoded.Append((char)bytes[index]);
                    continue;
                }

                if (bytes.Length <= ++index)
                {
                    // incorrect form
                    throw new FormatException("incorrect form");
                }

                if (bytes[index] == 0x2d)
                { // '-'
                    // The character "&" (0x26) is represented by the two-octet sequence "&-".
                    decoded.Append('&');
                    continue;
                }

                var nonPrintable = new StringBuilder();

                for (; index < bytes.Length; index++)
                {
                    if (bytes[index] == 0x2d) // '-'
                    {
                        // "-" is used to shift back to US-ASCII
                        break;
                    }

                    nonPrintable.Append((char)bytes[index]);
                }

                // modified UTF7 -> string
                decoded.Append(FromModifiedBase64(nonPrintable.ToString()));
            }

            return decoded.ToString();
        }

        private static string FromModifiedBase64(string str)
        {
            byte[] buf = null;

            // "," is used instead of "/"
            str = str.Replace(',', '/');

            int padding = 4 - str.Length & 3;

            if (padding == 3)
            {
                throw new FormatException("incorrect form");
            }
            else if (padding == 4)
            {
                buf = Convert.FromBase64String(str);
            }
            else
            {
                buf = Convert.FromBase64String(str + (new string('=', padding)));
            }

            return Encoding.BigEndianUnicode.GetString(buf);
        }
    }
}
