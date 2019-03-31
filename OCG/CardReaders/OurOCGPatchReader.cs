using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCG.DataStructure;
using OCG.Search;
using Newtonsoft.Json.Linq;

namespace OCG.CardReaders
{
    public class OurOCGPatchReader
    {
        private readonly string fileName = null;

        public OurOCGPatchReader(string fileName) => this.fileName = fileName;

        public int PatchCards(CardLibrary cardLibrary, ProcessChangedInvoker invoker = null)
        {
            if (!File.Exists(fileName))
                return 0;

            var ss = File.ReadAllLines(fileName, Encoding.GetEncoding("UTF-8"));
            int total = ss.Length;
            int count = 0;

            foreach (var s in ss)
            {
                JObject obj = Newtonsoft.Json.Linq.JObject.Parse(s);

                var code = obj["password"].ToString()?.Trim();
                var name = obj["name"].ToString();
                var card = cardLibrary.GetCardByCode(code);

                if (card != null)
                {
                    card.JapName = obj["japName"].ToString();
                    card.EnName = obj["enName"].ToString();
                    card.Infrequence = obj["infrequence"].ToString();
                    card.Package = obj["package"].ToString();
                    card.Adjust = obj["adjust"].ToString();
                    card.CreateTime = DateTime.Parse(obj["created_at"].ToString());
                    card.UpdateTime = DateTime.Parse(obj["updated_at"].ToString());
                }
                else
                    throw new ArgumentNullException("name", "未找到卡片!");

                    //sCardType = obj["sCardType"].ToString(),


                count++;
                invoker?.Invoke(total, count);
            }

            return count;
        }
    }
}
