using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Lucene.Net;
using Lucene.Net.Documents;
using OCG.DataStructure;
using OCG.LuceneExtend;
using MyTools;

namespace OCG.CardSavers
{
    public class LuceneCardsSaver
    {
        public string IndexPath { get; set; }

        public LuceneCardsSaver(string indexPath) => IndexPath = indexPath;

        public bool Save(Card[] cards, ProcessChangedInvoker invoker = null)
        {
            try
            {              
                var writer = MyLucene.GetIndexReWriter();
                int total = cards.Length;
                int count = 0;
                foreach (var card in cards)
                {
                    var doc = BuildDocument(card);
                    writer.AddDocument(doc);
                    count++;
                    invoker?.Invoke(total, count);
                }

                writer.Flush(true, true);
                writer.Commit();
            }
            catch
            {
                return false;
            }

            return true;
        }

        private Document BuildDocument(Card card)
        {
            var doc = new Document()
            {
                new NumericDocValuesField("id", card.Id),
                new TextField("name", card.Name??"", Field.Store.YES),
                new TextField("japName", card.JapName??"", Field.Store.YES),
                new TextField("enName", card.EnName??"", Field.Store.YES),
                new TextField("effect", card.Effect??"", Field.Store.YES),
                new TextField("code", card.CodeList??"", Field.Store.YES),
                new StoredField("code2", card.Code??""),
                new StringField("rule", card.CardRule.GetDesc<CardRule>(), Field.Store.YES),
                new StringField("alias", card.Alias??"", Field.Store.YES),
                new StringField("level", card.Level < 0 ? card.Level.ToString() : card.Level.ToString().PadLeft(2, '0'), Field.Store.YES),
                new StringField("pendL", card.PendulumL < 0 ? card.PendulumL.ToString() : card.PendulumL.ToString().PadLeft(2, '0'), Field.Store.YES),
                new StringField("pendR", card.PendulumR < 0 ? card.PendulumR.ToString() : card.PendulumR.ToString().PadLeft(2, '0'), Field.Store.YES),
                new StringField("attr", card.Attribute??"", Field.Store.YES),
                new StringField("race", card.Race??"", Field.Store.YES),
                new TextField("cardType", card.CardType.FullText??"", Field.Store.NO),
                new StoredField("cardType2", ((int)card.CardType.FullType).ToString()),
                new StringField("atk", card.AtkValue < 0 ? card.AtkValue.ToString() : card.AtkValue.ToString().PadLeft(4, '0'), Field.Store.YES),
                new StoredField("atk2", card.Atk??""),
                new StringField("def", card.DefValue < 0 ? card.DefValue.ToString() : card.DefValue.ToString().PadLeft(4, '0'), Field.Store.YES),
                new StoredField("def2", card.Def??""),
                new TextField("link", card.Link??"", Field.Store.YES),
            };
            return doc;
        }
    }
}
