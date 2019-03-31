using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTools;
using OCG.Search;

namespace OCG
{
    public class Global
    {
        public static string AppPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
        public static string IndexPath { get; } = AppPath + "\\Index";

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
                    cardLibrary = new CardLibrary(IndexPath);
                return cardLibrary;
            }
        }
    }
}
