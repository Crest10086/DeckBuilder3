using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using OCG.DataStructure;
using OCG.LuceneExtend;
using MyTools;

namespace OCG.CardReaders
{
    public class LuceneCardsReader
    {
        public string IndexPath { get; set; }

        public LuceneCardsReader(string indexPath) => IndexPath = indexPath;

        public Card[] Read(ProcessChangedInvoker invoker = null)
        {
            if (!System.IO.Directory.Exists(IndexPath))
                return new Card[0];

            var cards = new List<Card>();
            var searcher = MyLucene.GetIndexSearcher();
            Query query = new MatchAllDocsQuery();
            try
            {
                TopDocs topdDocs = searcher.Search(query, int.MaxValue);
                return ParseCards(searcher, topdDocs, invoker);
            }
            catch
            {
                return new Card[0];
            }
        }

        private Card[] ParseCards(IndexSearcher searcher, TopDocs topdDocs, ProcessChangedInvoker invoker)
        {
            int total = topdDocs.TotalHits;
            int count = 0;
            var cards = new List<Card>(total);
            foreach (var sdoc in topdDocs.ScoreDocs)
            {
                var doc = searcher.Doc(sdoc.Doc);
                var card = new Card
                {
                    Id = count++,
                    Name = GetFieldString(doc, "name"),
                    JapName = GetFieldString(doc, "japName"),
                    EnName = GetFieldString(doc, "enName"),
                    Effect = GetFieldString(doc, "effect"),
                    CodeList = GetFieldString(doc, "code"),
                    Code = GetFieldString(doc, "code2"),
                    AtkValue = GetFieldString(doc, "atkValue").ParseIntOrDefault()
                };
                cards.Add(card);
                invoker?.Invoke(total, count);
            }
            return cards.ToArray();
        }

        private string GetFieldString(Document doc, string fieldName, string defValue = "")
        {
            return doc.GetField(fieldName)?.GetStringValue()?.Trim() ?? defValue;
        }

        private int GetFieldInt(Document doc, string fieldName, int defValue = int.MinValue)
        {
            return doc.GetField(fieldName)?.GetInt32Value() ?? defValue;
        }
    }
}
