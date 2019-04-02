using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using OCG.DataStructure;
using MyTools;

namespace OCG.CardReaders
{
    class YGOProCardsReader
    {
        public string FileName { get; set; }

        public YGOProCardsReader(string fileName) => FileName = fileName;

        public Card[] Read(ProcessChangedInvoker invoker = null)
        {
            if (!File.Exists(FileName))
                return new Card[0];

            var cards = new List<Card>();
            var ht = new Dictionary<string, Card>();
            var total = 0;
            var count = 0;

            using (SQLiteConnection con = new SQLiteConnection($"Provider=Microsoft.Jet.OLEDB.4.0;Data source={FileName};Persist Security Info=False;"))
            {
                //连接数据库
                con.Open();

                //如果有进度回调，统计记录数
                if (invoker != null)
                {
                    using (SQLiteCommand dcc = new SQLiteCommand("Select Count(*) as iCount FROM [Datas]", con))
                    {
                        using (SQLiteDataReader reader = dcc.ExecuteReader())
                        {
                            reader.Read();
                            total = GetFieldInt(reader, "iCount");
                        }
                    }
                }

                using (SQLiteCommand dcc = new SQLiteCommand(@"select * from datas t1 left join texts t2 on t1.id = t2.id", con))
                {
                    using (SQLiteDataReader reader = dcc.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            Card card = ParseCard(reader, cards);
                            if (!ht.ContainsKey(card.Name))
                            {
                                cards.Add(card);
                                ht.Add(card.Name, card);
                            }
                            else
                            {
                                Card cd = ht[card.Name];
                                cd.CodeList = $"{cd.CodeList},{card.Code}";
                            }
                        }
                        count++;
                        invoker?.Invoke(total, count);
                    }
                }
            }
            return cards.ToArray<Card>();
        }

        private Card ParseCard(SQLiteDataReader reader, List<Card> cards)
        {
            var card = new Card
            {
                Id = cards.Count + 1,
                Name = GetFieldString(reader, "name"),
                Effect = GetFieldString(reader, "desc"),
                CardRule = (CardRule)GetFieldInt(reader, "ot")
            };

            card.Alias = card.CodeList = card.Code = GetFieldString(reader, "id").PadLeft(8, '0');

            var s = GetFieldString(reader, "alias");
            if (!string.IsNullOrEmpty(s))
                 card.Alias = s.PadLeft(8, '0');

            var i = GetFieldInt(reader, "level");
            if (i <= 12)
                card.Level = i;
            else
            {
                card.Level = (int)(i & 0xffff);
                card.PendulumR = (int)(i >> 16 & 0xff);
                card.PendulumL = (int)(i >> 24 & 0xff);
            }

            var ce = (CardElement)GetFieldInt(reader, "attribute");
            card.Attribute = ce.GetDesc<CardElement>() ?? "";

            var cr = (CardRace)GetFieldInt(reader, "race");
            card.Race = cr.GetDesc<CardRace>() ?? "";

            int cardType = GetFieldInt(reader, "type");
            card.CardType.FullType = (FullCardTypes)cardType;

            if (card.CardType.BaseType == BaseCardTypes.TYPE_MONSTER)
            {
                //如果是怪兽卡，解析攻击防御
                card.AtkValue = GetFieldInt(reader, "atk", -1);
                card.Atk = card.AtkValue >= 0 ? card.AtkValue.ToString() : "?";

                if (card.CardType.SubType == SubCardTypes.TYPE_LINK)
                {
                    //如果是LINK怪，防御力为空，防御值设为-8排序用
                    card.Def = "";
                    card.DefValue = -8;

                    //从def字段解析LINK数据
                    var lm = (LINK_MARK)GetFieldInt(reader, "def", 0);
                    var ss = lm.GetDescList<LINK_MARK>();
                    card.Link = string.Join("", ss);
                }
                else
                {
                    //如果是非LINK怪，解析防御力
                    card.DefValue = GetFieldInt(reader, "def", -1);
                    card.Def = card.DefValue >= 0 ? card.DefValue.ToString() : "?";
                }
            }
            else
            {
                //如果是魔陷卡，攻防为空，攻防值设为-9排序用
                card.Atk = "";
                card.AtkValue = -9;
                card.Def = "";
                card.DefValue = -9;
            }

            return card;
        }

        private string GetFieldString(SQLiteDataReader reader, string fieldName, string defValue = "")
        {
            return reader[fieldName]?.ToString().Trim() ?? defValue;
        }

        private int GetFieldInt(SQLiteDataReader reader, string fieldName, int defValue = int.MinValue)
        {
            return reader[fieldName]?.ToString().Trim().ParseIntOrDefault(defValue) ?? defValue;
        }
    }
}
