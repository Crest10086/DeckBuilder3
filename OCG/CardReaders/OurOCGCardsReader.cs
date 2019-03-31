using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OCG.DataStructure;

namespace OCG.CardReaders
{
    class OurOCGCardsReader
    {
        private readonly string requesturl = "https://www.ourocg.cn/search/";

        public Card[] Read(string filename)
        {
            if (!File.Exists(filename))
                return new Card[0];

            List<(string cheatcode, string name)> list1 = new List<(string cheatcode, string name)>();

            //连接数据库
            SQLiteConnection con = new SQLiteConnection($"Provider=Microsoft.Jet.OLEDB.4.0;Data source={filename};Persist Security Info=False;");
            con.Open();

            SQLiteCommand dcc = new SQLiteCommand(@"select t1.id, t2.name from datas t1 left join texts t2 on t1.id = t2.id", con);
            SQLiteDataReader creader = dcc.ExecuteReader();

            while (creader.Read())
            {
                string scd = GetFieldString(creader, "id");
                string sname = GetFieldString(creader, "name");
                list1.Add((scd, sname));
            }

            creader.Close();
            con.Close();

            
            //var cdlist1 = (from c in list1 select c.cheatcode).Distinct<string>();
            var list2 = from c in list1
                    group c by c.name into g
                    let clist = g.Select(b => b.cheatcode).ToArray()
                    select new { name = g.Key, cheatcodelist = string.Join(",", clist) };
            var cdlist = from c in list1 join d in list2 on c.name equals d.name select new RequestNode() { cheatcode=c.cheatcode, cheatcodelist=d.cheatcodelist };

            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            var requestlist = new ConcurrentQueue<RequestNode>(cdlist);
            var cardlist = new ConcurrentQueue<Card>();
            Task[] tasks = new Task[1];
            for(int i=0; i<tasks.Length; i++)
            {
                //tasks[i] = new Task(() =>
                {
                    while(!requestlist.IsEmpty && cardlist.Count < 100)
                    {
                        if (requestlist.TryDequeue(out var node))
                        {
                           var (issucc, card) = GetCard(node);
                            if (issucc)
                            {
                                if (card != null)
                                {
                                    cardlist.Enqueue(card);
                                }
                            }
                            else
                            {
                                requestlist.Enqueue(node);
                                //GC.Collect();
                                System.Threading.Thread.Sleep(500);
                            }
                        }
                    }
                }
                    //);
                //tasks[i].Start();
            }
            //Task.WaitAll(tasks);

            var cards = cardlist.OrderBy(card => card.CreateTime).ToArray<Card>();

            return cards;
        }

        private (bool issuccess, Card card) GetCard(RequestNode node)
        {
            HttpWebRequest myReq = null;
            HttpWebRequest myReq2 = null;
            HttpWebResponse result = null;
            HttpWebResponse result2 = null;
            Stream receviceStream = null;
            StreamReader readerOfStream = null;
            Card card = null;

            try
            {
                Uri uri = new Uri(requesturl + node.cheatcode);
                myReq = (HttpWebRequest)WebRequest.Create(uri);
                myReq.Method = "HEAD";
                myReq.AllowAutoRedirect = false;

                result = (HttpWebResponse)myReq.GetResponse();

                string newurl = result.GetResponseHeader("Location");
                myReq.Abort();

                if (newurl == null || newurl == "")
                    return (true, null);

                uri = new Uri(newurl);
                myReq2 = (HttpWebRequest)WebRequest.Create(uri);
                myReq2.Method = "GET";
                myReq2.AllowAutoRedirect = false;
                myReq2.Accept = "applicaton/json";
                result2 = (HttpWebResponse)myReq2.GetResponse();
                receviceStream = result2.GetResponseStream();
                readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("UTF-8"));
                string strHTML = readerOfStream.ReadToEnd();
                card = ParseCardByJson(strHTML);
            }
            catch //(Exception ex)
            {
                //throw new Exception("采集指定网址异常，" + ex.Message);
                return (false, null);
            }
            finally
            {
                readerOfStream?.Close();
                result?.Close();
                result2?.Close();
                myReq?.Abort();
                myReq2?.Abort();
            }

            return (true, card);
        }
        
        private Card ParseCardByJson(string jsontext)
        {
            JObject obj = Newtonsoft.Json.Linq.JObject.Parse(jsontext);
            Card card = new Card()
            {
                Name = obj["name"].ToString(),
                //sCardType = obj["sCardType"].ToString(),
                CodeList = obj["password"].ToString(),
                Effect = obj["desc"].ToString(),
                CreateTime = DateTime.Parse(obj["created_at"].ToString())
            };

            return card;
        }

        private Card[] GetCards(List<string> codelist)
        {
            return null;
        }

        private string GetFieldString(SQLiteDataReader reader, string fieldname)
        {
            return reader[fieldname]?.ToString().Trim() ?? "";
        }
    }

    class RequestNode
    {
        public string cheatcode;
        public string cheatcodelist;
    }
}
