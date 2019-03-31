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
                new StoredField("code2", card.Code??"")
            };
            return doc;
        }
    }
}
