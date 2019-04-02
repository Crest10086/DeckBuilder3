using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;
using Mozilla.NUniversalCharDet;

namespace MyTools
{
    public class CharSetTools
    {
        /*
        private static char[] HANKAKU_KATAKANA = { '｡', '｢', '｣', '､', '･',
        'ｦ', 'ｧ', 'ｨ', 'ｩ', 'ｪ', 'ｫ', 'ｬ', 'ｭ', 'ｮ', 'ｯ', 'ｰ', 'ｱ', 'ｲ',
        'ｳ', 'ｴ', 'ｵ', 'ｶ', 'ｷ', 'ｸ', 'ｹ', 'ｺ', 'ｻ', 'ｼ', 'ｽ', 'ｾ', 'ｿ',
        'ﾀ', 'ﾁ', 'ﾂ', 'ﾃ', 'ﾄ', 'ﾅ', 'ﾆ', 'ﾇ', 'ﾈ', 'ﾉ', 'ﾊ', 'ﾋ', 'ﾌ',
        'ﾍ', 'ﾎ', 'ﾏ', 'ﾐ', 'ﾑ', 'ﾒ', 'ﾓ', 'ﾔ', 'ﾕ', 'ﾖ', 'ﾗ', 'ﾘ', 'ﾙ',
        'ﾚ', 'ﾛ', 'ﾜ', 'ﾝ', 'ﾞ', 'ﾟ' };

        private static char[] ZENKAKU_KATAKANA = { '。', '「', '」', '、', '・',
        'ヲ', 'ァ', 'ィ', 'ゥ', 'ェ', 'ォ', 'ャ', 'ュ', 'ョ', 'ッ', 'ー', 'ア', 'イ',
        'ウ', 'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ',
        'タ', 'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'ヒ', 'フ',
        'ヘ', 'ホ', 'マ', 'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル',
        'レ', 'ロ', 'ワ', 'ン', '゛', '゜' };
         */

        private static Dictionary<char, char> dbcDict = new Dictionary<char, char>();
        private static Dictionary<char, char> SBCDict = new Dictionary<char, char>();
        private static bool inited = false;

        private static void AddDict(char c1, char c2)
        {
            dbcDict.Add(c1, c2);
            SBCDict.Add(c2, c1);
        }

        private static void InitCharDict()
        {
            if (!inited)
            {
                AddDict('·', '・');
                AddDict(' ', '　');
                //addmap('－', '-');
                inited = true;
            }         
        }

        /// <summary>
        /// 日文半角转全角
        /// </summary>
        /// <param name="source">需要转换的字符串</param>
        /// <returns>转换结果</returns>
        public static string JPDBCToSBC(string source)
        {
            InitCharDict();

            var s = DBCToSBC(source);
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (dbcDict.ContainsKey(s[i]))
                    sb.Append(dbcDict[s[i]]);
                else
                    sb.Append(s[i]);
            }

            return sb.ToString();
        }


        /// <summary>
        /// 日文全角转半角
        /// </summary>
        /// <param name="source">待转换的字符串</param>
        /// <returns>转换结果</returns>
        public static string JPSBCToDBC(string source)
        {
            InitCharDict();

            var s = source;
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (SBCDict.ContainsKey(s[i]))
                    sb.Append(SBCDict[s[i]]);
                else
                    sb.Append(s[i]);
            }

            s = sb.ToString();
            return SBCToDBC(s);
        }

        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="source">需要转换的字符串</param>
        /// <returns>转换结果</returns>
        public static string DBCToSBC(string source)
        {
            return Strings.StrConv(source, VbStrConv.Wide, 1);

            /*
            //半角转全角：
            char[] c = source.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
             */

            /*
            char[] c=source.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
             byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
             if (b.Length == 2)
             {
                 if (b[1] == 0)
                 {
                     b[0] = (byte)(b[0] - 32);
                     b[1] = 255;
                     c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                 }
             }
            }*/
        }

        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="source">需要转换的字符串</param>
        /// <returns>转换结果</returns>
        public static string SBCToDBC(string source)
        {
            //return Strings.StrConv(source, VbStrConv.Narrow, 1);
            //这里不可以直接用Strings.StrConv，会有识别问题
            
            if (string.IsNullOrWhiteSpace(source))
                return "";

            var sb = new StringBuilder(source.Length, source.Length);
            var len = source.Length;
            for (int i = 0; i < len; i++)
            {
                if (source[i] >= 65281 && source[i] <= 65373)
                {
                    sb.Append((char)(source[i] - 65248));
                }
                else if (source[i] == 12288)
                {
                    sb.Append(' ');
                }
                else
                {
                    sb.Append(source[i]);
                }
            }
            return sb.ToString();
            
        }

        /// <summary>
        /// 繁体转简体
        /// </summary>
        /// <param name="source">需要转换的字符串</param>
        /// <returns>转换结果</returns>
        public static string BIG5ToGB(string source)
        {
            return Strings.StrConv(source, VbStrConv.SimplifiedChinese, 0);
        }


        /// <summary>
        /// 识别文本文件字符集
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static string GetCharSet(string fileName)
        {
            try
            {
                byte[] pReadByte = new byte[0];
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(fs);
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                UniversalDetector Det = new UniversalDetector(null);
                Det.HandleData(pReadByte, 0, pReadByte.Length);
                Det.DataEnd();
                return Det.GetDetectedCharset();
            }
            catch
            {
                return null;
            }
        }
    }
}
