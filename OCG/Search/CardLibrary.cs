﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers.Classic;
using OCG.DataStructure;
using OCG.CardReaders;
using OCG.Search;
using OCG.LuceneExtend;

namespace OCG.Search
{
    public class CardLibrary
    {
        private Card[] cards = new Card[0];
        private Dictionary<string, Card> htcards = new Dictionary<string, Card>();

        public Card this[int index] => GetCardByIndex(index);
        public Card this[string name] => GetCardByName(name);
        public int Count => cards.Length;

        public CardLibrary(string indexPath)
        {
            LoadCards(indexPath);
        }

        private void LoadCards(string indexPath)
        {
            var reader = new LuceneCardsReader(indexPath);
            cards = reader.Read();
            htcards.Clear();
            for (int i = 0; i < cards.Length; i++)
                htcards.Add(cards[i].Name, cards[i]);
        }

        public Card[] Search(string queryString = null, SorterBuilder sorterBuilder = null)
        {
            Query query = null;
            if (string.IsNullOrWhiteSpace(queryString))
                query = new MatchAllDocsQuery();
            else
            {
                var parser = new MultiFieldQueryParser(MyLucene.LuceneVersion, MyLucene.GetSearchField(), MyLucene.GetCardAnalyzer(), MyLucene.GetFieldBoosts());
                query = parser.Parse(queryString);
            }
            try
            {
                var searcher = MyLucene.GetIndexSearcher();
                TopDocs topDocs = sorterBuilder == null
                    ? searcher.Search(query, int.MaxValue)
                    : searcher.Search(query, int.MaxValue, sorterBuilder.ToLuceneSort());
                var total = topDocs.TotalHits;
                var cards = new Card[total];
                for (int i = 0; i < total; i++)
                    cards[i] = GetCardByIndex(topDocs.ScoreDocs[i].Doc);

                return cards;
            }
            catch
            {
                return new Card[0];
            }
        }

        /// <summary>
        /// 根据索引返回卡片。考虑到性能，这里不做索引越界检查，调用方需自行处理
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>卡片</returns>
        public Card GetCardByIndex(int index) => cards[index];


        /// <summary>
        /// 根据卡片名返回卡片
        /// </summary>
        /// <param name="name">卡片名</param>
        /// <returns>卡片</returns>
        public Card GetCardByName(string name) => htcards[name]; 
    }
}
