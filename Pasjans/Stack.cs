using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pasjans
{
    // Klasa z ktorej dziedzicza wszystkie stosy
    // Uwaga: wierzchnia karta w cards to ta z indeksem 0
    internal abstract class Stack
    {
        // Lista przechowujaca karty obecne na stosie
        protected List<int> cards;

        // Konstruktor pobierajacy karty poczatkowe
        public Stack(List<int> cards)
        {
            this.cards = cards;
        }

        // Funkcja odpowiedzialna za zwracanie linii gornej karty stosu do wypisania w konsoli
        public virtual List<string> GenerateDrawLines(string[] lines)
        {
            List<string> drawLines = new List<string>();

            for (int i = 0; i < 7; i++)
            {
                drawLines.Add(lines[cards[0] * 7 + i]);
            }

            return drawLines;
        }

        // Funkcja zwracajaca ilosc kart na stosie
        public int GetCardAmount()
        {
            return cards.Count;
        }

        // Funkcja zwracajaca okreslona ilosc kart ze szczytu stosu
        public virtual int[] GetTopCards(int amount)
        {
            int[] cards = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                cards[i] = this.cards[i];
            }
            return cards;
        }

        // Funkcja usuwajaca okreslona ilosc kart z gory stosu
        public virtual void DeleteTop(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                cards.RemoveAt(0);
            }
        }

        // Funkcja dodajaca okreslone karty na szczyt stosu
        public virtual void AddCards(int[] cards)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                this.cards.Insert(i, cards[i]);
            }
        }

        // Funkcja do debugowania
        public void PrintStack()
        {
            foreach (int i in cards)
            {
                Debug.Write(i + " ");
            }
            Debug.WriteLine("");
        }
    }
}
