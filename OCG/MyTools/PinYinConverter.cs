using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.International.Converters.PinYinConverter;

namespace MyTools
{
    public class PingYinModel
    {
        public PingYinModel()
        {
            TotalPingYin = new List<string>();
            FirstPingYin = new List<string>();
        }

        //全拼
        public List<string> TotalPingYin { get; set; }

        //首拼
        public List<string> FirstPingYin { get; set; }
    }

    public class PinYinConverter
    {

        /// <summary>
        /// 把汉字转换成拼音(全拼)，支持多音字
        /// </summary>
        /// <param name="str">汉字字符串</param>
        /// <param name="charSeparator">每个单字拼音间的分隔符，默认为空格</param>
        /// <returns>转换后的拼音(全拼)字符串列表</returns>
        public static PingYinModel GetTotalPingYin(string str, string charSeparator = " ")
        {
            var chs = str.ToCharArray();
            //记录每个汉字的全拼
            var totalPingYins = new Dictionary<int, List<string>>();
            for (int i = 0; i < chs.Length; i++)
            {
                var pinyins = new List<string>();
                var ch = chs[i];
                //是否是有效的汉字
                if (ChineseChar.IsValidChar(ch))
                {
                    ChineseChar cc = new ChineseChar(ch);
                    pinyins = cc.Pinyins.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
                }
                else
                {
                    pinyins.Add(ch.ToString());
                }

                //去除声调，转小写
                pinyins = pinyins.ConvertAll(p => Regex.Replace(p, @"\d", "").ToLower());
                //去重
                pinyins = pinyins.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
                if (pinyins.Any())
                {
                    totalPingYins[i] = pinyins;
                }
            }
            var result = new PingYinModel();
            foreach (var pinyins in totalPingYins)
            {
                var items = pinyins.Value;
                if (result.TotalPingYin.Count <= 0)
                {
                    result.TotalPingYin = items;
                    result.FirstPingYin = items.ConvertAll(p => p.Substring(0, 1)).Distinct().ToList();
                }
                else
                {
                    //全拼循环匹配
                    var newTotalPingYins = new List<string>();
                    foreach (var totalPingYin in result.TotalPingYin)
                    {
                        newTotalPingYins.AddRange(items.Select(item => totalPingYin + charSeparator + item));
                    }
                    newTotalPingYins = newTotalPingYins.Distinct().ToList();
                    result.TotalPingYin = newTotalPingYins;

                    //首字母循环匹配
                    var newFirstPingYins = new List<string>();
                    foreach (var firstPingYin in result.FirstPingYin)
                    {
                        newFirstPingYins.AddRange(items.Select(item => firstPingYin + charSeparator + item.Substring(0, 1)));
                    }
                    newFirstPingYins = newFirstPingYins.Distinct().ToList();
                    result.FirstPingYin = newFirstPingYins;
                }
            }
            return result;
        }

        /// <summary>
        /// 把汉字转换成拼音(全拼)，支持多音字
        /// </summary>
        /// <param name="str">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串，多种拼音默认以逗号分隔</returns>
        public static string GetTotalPingYinText(string str, string charSeparator = " ", string wordSeparator = " 囧 ")
        {
            return string.Join(wordSeparator, GetTotalPingYin(str, charSeparator).TotalPingYin);
        }

        /// <summary>
        /// 把汉字转换成拼音(首字母)，支持多音字
        /// </summary>
        /// <param name="str">汉字字符串</param>
        /// <returns>转换后的拼音(首字母)字符串，多种拼音默认以逗号分隔</returns>
        public static string GetFirstPingYinText(string str, string charSeparator = " ", string wordSeparator = " 囧 ")
        {
            return string.Join(wordSeparator, GetTotalPingYin(str, charSeparator).FirstPingYin);
        }

        /// <summary>
        /// 把汉字转换成拼音(全拼和首字母)，支持多音字
        /// </summary>
        /// <param name="str">汉字字符串</param>
        /// <returns>转换后的拼音(全拼和首字母)字符串，多种拼音默认以逗号分隔</returns>
        public static string GetTotalAndFirstPingYinText(string str, string charSeparator = " ", string wordSeparator = " 囧 ")
        {
            var py = GetTotalPingYin(str, charSeparator);
            return string.Join(wordSeparator, py.TotalPingYin) + wordSeparator + string.Join(wordSeparator, py.FirstPingYin);
        }
    }
}
