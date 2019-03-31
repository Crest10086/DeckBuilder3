using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTools;
using OCG.Search;
using OCG.CardReaders;

namespace OCG
{
    public class Global
    {
        public static string AppPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
        public static string IndexPath { get; } = AppPath + "\\Index";
        public static string PatchFile { get; } = AppPath + "\\Data\\card.json";

        private static IniConfig config = null;
        public static IniConfig Config
        {
            get
            {
                if (config == null)
                    config = new IniConfig(AppPath + "\\Settings.ini", "DeckBuilder3");
                return config;
            }
        }

        private static CardLibrary cardLibrary = null;
        public static CardLibrary CardLibrary
        {
            get
            {
                if (cardLibrary == null)
                {
                    var reader = new LuceneCardsReader(IndexPath);
                    var cards = reader.Read();
                    cardLibrary = new CardLibrary(cards);                   
                }
                return cardLibrary;
            }
        }
    }
}
