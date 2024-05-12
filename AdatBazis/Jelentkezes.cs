using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdatBazis
{
    internal class Jelentkezes
    {
        string nev;
        char nem;
        int pontszam;
        string szak;

        public Jelentkezes(string nev, char nem, int pontszam, string szak)
        {
            this.nev = nev;
            this.nem = nem;
            this.pontszam = pontszam;
            this.szak = szak;
        }

        public string Nev { get => nev; set => nev = value; }
        public char Nem { get => nem; set => nem = value; }
        public int Pontszam { get => pontszam; set => pontszam = value; }
        public string Szak { get => szak; set => szak = value; }

        public bool SzakNeveTartalmazza(string Szoveg)
        {
            return this.Szak.Contains(Szoveg);
        }

        
    }
}
