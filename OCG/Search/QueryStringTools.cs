using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyTools;

namespace OCG.Search
{
    public class QueryStringTools
    {
        private static Dictionary<Regex, string> nameDict = null;

        private static void InitNameDict()
        {
            if (nameDict == null)
            {
                nameDict = new Dictionary<Regex, string>
                {
                    { new Regex(@" (&&?)|(and) ", RegexOptions.Compiled | RegexOptions.IgnoreCase), " AND " },
                    { new Regex(@" (\|\|?)|(or) ", RegexOptions.Compiled | RegexOptions.IgnoreCase), " OR " },
                    { new Regex(@"\b(中文|卡)名:", RegexOptions.Compiled), "name:" },
                    { new Regex(@"\b日文名:", RegexOptions.Compiled), "japName:" },
                    { new Regex(@"\b(旧卡|曾用)名:", RegexOptions.Compiled), "oldName:" },
                    { new Regex(@"\b(卡种|卡片种类):", RegexOptions.Compiled), "cardType:" },
                    { new Regex(@"\b种族:", RegexOptions.Compiled), "race:" },
                    { new Regex(@"\b属性:", RegexOptions.Compiled), "attr:" },
                    { new Regex(@"\b效果(说明)?:", RegexOptions.Compiled), "effect:" },
                    { new Regex(@"\b调整:", RegexOptions.Compiled), "adjust:" },
                    { new Regex(@"\b卡包:", RegexOptions.Compiled), "package:" },
                    { new Regex(@"\b(罕见|稀有)度:", RegexOptions.Compiled), "Infrequence:" },
                    { new Regex(@"\b攻(击力?)?:", RegexOptions.Compiled), "atk:" },
                    { new Regex(@"\b防(御力?)?:", RegexOptions.Compiled), "def:" },
                    { new Regex(@"\b(星(级|数)?|等级):", RegexOptions.Compiled), "level:" },
                    { new Regex(@"\b禁(限数(|量))?:", RegexOptions.Compiled), "limit:" },
                    { new Regex(@"\b(简称|俗称|缩写):", RegexOptions.Compiled), "shortName:" },
                    { new Regex(@"\b(效果)?种类:", RegexOptions.Compiled), "effectType:" },
                    { new Regex(@"\b((卡片)?密码|cardpass):", RegexOptions.Compiled), "code:" }
                };
            }
        }

        private static string FormatName(string queryString)
        {
            InitNameDict();
            var s = queryString;
            foreach (var regex in nameDict.Keys)
            {
                var name = nameDict[regex];
                s = regex.Replace(s, name);
            }

            return s;
        }

        static Regex regexNum = new Regex(@"\b(?<field>(?:atk)|(?:def)|(?:level)|(?:pendL)|(?:pendR)):(?<num1>\d+)(?:-(?<num2>\d+))?", RegexOptions.Compiled);
        private static string FormatNum(string queryString)
        {
            var s = queryString;
            var matches = regexNum.Matches(s);
            int count = matches.Count;
            for (int i = 0; i < count; i++)
            {
                var match = matches[count - 1 - i];
                var numlength = "";
                switch (match.Groups["field"].Value)
                {
                    case "atk":
                    case "def":
                        numlength = "4";
                        break;
                    case "level":
                    case "pendL":
                    case "pendR":
                        numlength = "2";
                        break;
                }
                string s2 = null;
                int n1 = 0;
                int n2 = 9999;

                if (match.Groups["num2"].Success)
                {
                    try
                    {
                        n1 = int.Parse(match.Groups["num1"].Value);
                        n2 = int.Parse(match.Groups["num2"].Value);
                    }
                    catch
                    {
                    }
                    s2 = string.Format("{0}:[{1:D" + numlength + "} TO {2:D" + numlength + "}]", match.Groups["field"].Value, n1, n2);
                }
                else
                {
                    try
                    {
                        n1 = int.Parse(match.Groups["num1"].Value);
                    }
                    catch
                    {
                    }
                    s2 = string.Format("{0}:{1:D" + numlength + "}", match.Groups["field"].Value, n1);
                }

                if (match.Index > 0)
                    s = s.Substring(0, match.Index) + s2 + s.Substring(match.Index + match.Length, s.Length - match.Index - match.Length);
                else
                    s = s2 + s.Substring(match.Index + match.Length, s.Length - match.Index - match.Length);
            }

            return s;
        }

        static Regex regexQuote = new Regex(@"\b(?<field>\w+:)?""?((\d+\-\d+)|(\[\d+ TO \d+\])|(?<string>\w+))""?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static string FormatQuote(string queryString)
        {
            var s = queryString;
            var matches = regexNum.Matches(s);
            int count = matches.Count;
            for (int i = 0; i < count; i++)
            {
                var match = matches[count - 1 - i];
                var numlength = "";
                if (match.Groups["string"].Success)
                {

                }
            }
                    

            return null;
        }

        public static string Format(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
                return "";

            var s = CharSetTools.SBCToDBC(queryString.Trim());
            s = FormatName(s);
            s = FormatNum(s);

            return s;
        }
    }
}
