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
                    CardRule = GetFieldString(doc, "rule").ParseEnumOrDefault<CardRule>(CardRule.TYPE_OT),
                    Alias = GetFieldString(doc, "alias"),
                    Level = GetFieldInt(doc, "level"),
                    PendulumL = GetFieldInt(doc, "pendL"),
                    PendulumR = GetFieldInt(doc, "pendR"),
                    Attribute = GetFieldString(doc, "attr"),
                    Race = GetFieldString(doc, "race"),
                    CardType = new CardType() { FullType = (FullCardTypes)GetFieldInt(doc, "cardType2") },
                    AtkValue = GetFieldInt(doc, "atk"),
                    Atk = GetFieldString(doc, "atk2"),
                    DefValue = GetFieldInt(doc, "def"),
                    Def = GetFieldString(doc, "def2"),
                    Link = GetFieldString(doc, "link"),
                    Infrequence = GetFieldString(doc, "infrequence"),
                    Package = GetFieldString(doc, "package"),
                    Adjust = GetFieldString(doc, "adjust"),
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
            return doc.GetField(fieldName)?.GetStringValue()?.Trim()?.ParseIntOrDefault() ?? defValue;
        }
    }
}
