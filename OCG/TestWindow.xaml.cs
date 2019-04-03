using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OCG.DataStructure;
using OCG.CardReaders;
using OCG.CardSavers;
using OCG.Search;
using OCG.LuceneExtend;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.QueryParsers.Classic;

namespace OCG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".cdb",
                Filter = "YGOPro数据库|*.cdb"
            };
            if (ofd.ShowDialog() == true)
            {
                var ygoReader = new YGOProCardsReader(ofd.FileName);
                var cards = ygoReader.Read();
                var cardLibrary = new CardLibrary(cards);
                var preader = new OurOCGPatchReader(Global.PatchFile);
                preader.PatchCards(cardLibrary);
                cardLibrary.SortCards();

                var luceneSaver = new LuceneCardsSaver(Global.IndexPath);
                luceneSaver.Save(cardLibrary.GetCards());
                var cards3 = cardLibrary.GetCards();

                //var luceneReader = new LuceneCardsReader(Global.IndexPath);
                //var cards2 = luceneReader.Read();
       

                ObservableCollection<Card> cardList = new ObservableCollection<Card>(cards3);
                listview1.ItemsSource = cardList;
            }
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {

            /*
            Stream receviceStream = null;
            StreamReader readerOfStream = null;

            try
            {
                Uri uri = new Uri("https://www.ourocg.cn/search/89631139");
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
                myReq.Method = "HEAD";
                myReq.AllowAutoRedirect = false;
                myReq.Proxy = null;

                HttpWebResponse result = (HttpWebResponse)myReq.GetResponse();

                string newurl = result.GetResponseHeader("Location");
                result.Close();
                myReq.Abort();

                uri = new Uri(newurl);
                myReq = (HttpWebRequest)WebRequest.Create(uri);
                myReq.Method = "GET";
                myReq.Accept = "applicaton/json";
                myReq.Proxy = null;
                result = (HttpWebResponse)myReq.GetResponse();
                receviceStream = result.GetResponseStream();
                readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("UTF-8"));
                string strHTML = readerOfStream.ReadToEnd();             

                string jsontext = System.Text.RegularExpressions.Regex.Unescape(strHTML);
                RichText1.Document.Blocks.Clear();
                //RichText1.AppendText(jsontext);
                var card = ParseCardByJson(jsontext);
                RichText1.AppendText(card.Name + "/r/n" + card.Effect);

            }
            catch (Exception ex)
            {
                throw new Exception("采集指定网址异常，" + ex.Message);
            }
            finally
            {
                readerOfStream?.Close();
            }
            */
        }


        private Card ParseCardByJson(string jsontext)
        {
            JObject obj = Newtonsoft.Json.Linq.JObject.Parse(jsontext);
            Card card = new Card()
            {
                Name = obj["name"].ToString(),
                //sCardType = obj["sCardType"].ToString(),
                CodeList = obj["password"].ToString(),
                Effect = obj["desc"].ToString()
            };

            return card;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RichText1.Document.Blocks.Clear();

            /*
            int testTimes = 100000000;
            int[] arr = new int[testTimes];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = i;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            watch.Reset();
            watch.Start();
            //代码
            int s = 0;
            int n = arr.Length * 2 ;
            for (int i = 0; i < n; i++)
            {
                if (i >= 0 && i < arr.Length)
                {
                    s += arr[i];
                }
            }

            watch.Stop();

            RichText1.AppendText("方法1：" + watch.ElapsedMilliseconds.ToString() + Environment.NewLine + Environment.NewLine);

            watch.Reset();
            watch.Start();
            //代码

            s = 0;
            int m = n < arr.Length ? n : arr.Length;
            for (int i = 0; i < m; i++)
            {
                    s += arr[i];
            }

            watch.Stop();
            RichText1.AppendText("方法2：" + watch.ElapsedMilliseconds.ToString() + Environment.NewLine + Environment.NewLine);
            */

            //CardType ct = new CardType();
            //ct.FullType = (FullCardTypes)8225;

            /*
            Query query2 = NumericRangeQuery.NewInt32Range("level", 0, 4, false, true);
            QueryParser parser = new QueryParser(MyLucene.LuceneVersion, "level", MyLucene.GetCardAnalyzer());//new MultiFieldQueryParser(MyLucene.LuceneVersion, MyLucene.GetSearchField(), MyLucene.GetCardAnalyzer(), MyLucene.GetFieldBoosts());
            var query = parser.Parse(query2.ToString());
            var searcher = OCG.LuceneExtend.MyLucene.GetIndexSearcher();
            TopDocs topDocs = searcher.Search(query, int.MaxValue);
            var total = topDocs.TotalHits;
            */

            string s = MyTools.PinYinConverter.GetTotalAndFirstPingYinText("青眼白龙");
            RichText1.AppendText(s);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var cards = Global.CardLibrary.Search(QueryStringTools.Format(SearchText.Text));
            ObservableCollection<Card> cardList = new ObservableCollection<Card>(cards);
            listview1.ItemsSource = cardList;
        }

        private void Listview1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Card card = (Card)listview1.SelectedItem;
            if (card != null)
            {
                RichText1.Document.Blocks.Clear();
                RichText1.AppendText(card.Text);
            }
        }

        private void SearchText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click_2(null, null);
        }
    }
}
