using System;
using System.Collections.Generic;

namespace Pasjans
{
    // Klasa stosow gornych (koncowych)
    internal class TopStack : Stack
    {
        // Zmienna przechowujaca ktory w kolejnosci to stos (0-3)
        int stackID;

        // Konstruktor sczytujący ID
        public TopStack(int stackID) : base(new List<int>()) 
        {
            this.stackID = stackID;
        }

        // Funkcja odpowiedzialna za zwracanie linii do wypisania w konsoli
        public override List<string> GenerateDrawLines(string[] lines)
        {
            List<string> drawLines = new List<string>();

            // Jesli stos jest pusty to zwracamy linie odpowiedniego tla z pliku
            if (cards.Count == 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    drawLines.Add(lines[(54 + stackID) * 7 + i]);
                }
            }
            // Jesli nie jest to zwracamy linie wierzchniej karty
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    drawLines.Add(lines[cards[0] * 7 + i]);
                }
            }

            return drawLines;
        }

        // Funkcja zwracajaca okreslona ilosc kart z gory stosu
        public override int[] GetTopCards(int amount)
        {
            if (cards.Count == 0)
                return new[] { -1 };
            else
                return base.GetTopCards(amount);
        }

        // Funkcja zwracajaca ID stosu
        public int GetID()
        {
            return stackID;
        }
    }
}
