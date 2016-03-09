using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarteTPLibrary
{
    public class Carte
    {
        public string SourceFile;
        public string SourcePage;
        public string Serial;
        public string Pli;
        public string BarCod;
        public string Index;

        public Carte()
        { }
        public Carte(string barcod)
        {
            BarCod = barcod;
            Serial = barcod.Substring(10);
            Pli = barcod.Substring(2, 8).TrimStart('0');
            Index = barcod.Substring(0, 1);
        }
    }
}
