using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace OCG.LuceneExtend
{
    public class MyLucene
    {
        private static PerFieldAnalyzerWrapper analyzer = null;
        private static IndexWriter indexReWriter = null;
        private static IndexWriter indexWriter = null;
        private static IndexSearcher indexSearcher = null;
        private static Dictionary<string, float> fieldBoosts = null;
        private static string[] fields = null;

        public static LuceneVersion LuceneVersion => LuceneVersion.LUCENE_48;

        public static Analyzer GetCardAnalyzer()
        {
            if (analyzer == null)
            {

                Dictionary<string, Analyzer> analyzers = new Dictionary<string, Analyzer>
                {
                    { "name", new StandardAnalyzer(LuceneVersion) },
                    { "japName", new StandardAnalyzer(LuceneVersion) },
                    { "enName", new StandardAnalyzer(LuceneVersion) },
                    { "effect", new StandardAnalyzer(LuceneVersion) },
                    { "code", new StandardAnalyzer(LuceneVersion) },
                    { "cardType", new StandardAnalyzer(LuceneVersion) },
                    { "link", new StandardAnalyzer(LuceneVersion) }
                };
                analyzer = new PerFieldAnalyzerWrapper(new StandardAnalyzer(LuceneVersion), analyzers);
            }
            return analyzer;
        }

        public static IndexWriter GetIndexWriter()
        {
            if (indexWriter == null)
            {
                if (!System.IO.Directory.Exists(Global.IndexPath))
                    System.IO.Directory.CreateDirectory(Global.IndexPath);

                var config = new IndexWriterConfig(MyLucene.LuceneVersion, MyLucene.GetCardAnalyzer())
                {
                    OpenMode = OpenMode.CREATE_OR_APPEND,
                    RAMBufferSizeMB = 256,
                    MaxBufferedDocs = 20000
                };
                indexWriter = new IndexWriter(FSDirectory.Open(Global.IndexPath), config);
            }
            return indexWriter;
        }

        public static IndexWriter GetIndexReWriter()
        {
            if (indexReWriter == null)
            {
                if (!System.IO.Directory.Exists(Global.IndexPath))
                    System.IO.Directory.CreateDirectory(Global.IndexPath);

                var config = new IndexWriterConfig(MyLucene.LuceneVersion, MyLucene.GetCardAnalyzer())
                {
                    OpenMode = OpenMode.CREATE,
                    RAMBufferSizeMB = 256,
                    MaxBufferedDocs = 20000
                };
                indexReWriter = new IndexWriter(FSDirectory.Open(Global.IndexPath), config);
            }
            return indexReWriter;
        }

        public static IndexSearcher GetIndexSearcher()
        {
            if (indexSearcher == null)
            {
                IndexReader reader = DirectoryReader.Open(FSDirectory.Open(Global.IndexPath));
                indexSearcher = new IndexSearcher(reader);
            }
            return indexSearcher;
        }

        private static void InitFieldBoosts()
        {
            if (fieldBoosts == null)
                fieldBoosts = new Dictionary<string, float>
                {
                    { "name", 2 },
                    { "japName", 2 },
                    { "EnName", 2 },
                    { "effect", 0.5f },
                    { "level", 1 },
                };
        }

        public static string[] GetSearchField()
        {
            InitFieldBoosts();
            if (fields == null)
            {
                List<string> sl = new List<string>(fieldBoosts.Count);
                foreach (var s in fieldBoosts.Keys)
                    sl.Add(s);
                fields = sl.ToArray();
            }
            return fields;
        }

        public static Dictionary<string, float> GetFieldBoosts()
        {
            InitFieldBoosts();
            return fieldBoosts;
        }
    }
}
