using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasjans
{
    // Klasa stosu z ktorego dobiera sie karty
    // Uwaga: wierzchnia karta w cards to ta z indeksem 0
    internal class DrawStack
    {
        // Zadeklarowanie listy przechowujacej karty obecne na stosie
        List<int> cards = new List<int>();

        // Konstruktor pobierajacy karty poczatkowe
        public DrawStack(List<int> cards)
        {
            this.cards = cards;
        }

        // Funkcja odpowiedzialna za zwracanie linii do wypisania w konsoli
        public List<string> GenerateDrawLines(string[] lines)
        {
            List<string> drawLines = new List<string>();

            for (int i = 0; i < 7; i++)
            {
                drawLines.Add(lines[cards[0] * 7 + i]);
            }

            return drawLines;
        }

        // Funkcja odpowiedzialna za przekladanie karty z gory stosu na dol
        public void Draw()
        {
            int card = cards[0];
            cards.RemoveAt(0);
            cards.Insert(cards.Count, card);
        }

        // Funkcja zwracajaca gorna karte stosu
        public int GetDrawCard()
        {
            return cards[0];
        }

        // Funkcja usuwajaca gorna karte stosu
        public void DeleteDrawCard()
        {
            cards.RemoveAt(0);
        }

    }
}
