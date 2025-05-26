using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasjans
{
    // Klasa stosu z ktorego dobiera sie karty
    internal class DrawStack : Stack
    {
        List<int> sideCards = new List<int>(); // Lista kart bocznych, ktore zostaly odlozone na bok

        // Konstruktor z klasy bazowej
        public DrawStack(List<int> cards) : base(cards) { }

        // Funkcja odpowiedzialna za zwracanie linii do wypisania w konsoli
        public override List<string> GenerateDrawLines(string[] lines)
        {
            List<string> drawLines = new List<string>();

            // Jesli stos jest pusty to zwracamy puste linie
            if (cards.Count == 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    drawLines.Add(emptyPreset);
                }
            }
            // Jesli nie jest pusty to zwracamy linie wierzchniej karty
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    drawLines.Add(lines[cards[0] * 7 + i]);
                }
            }

            // Jesli nie ma kart bocznych to zwracamy puste linie
            if (sideCards.Count == 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    //drawLines.Add(lines[53 * 7 + i]);
                    drawLines.Add(emptyPreset);
                }
            }
            // Jesli sa karty boczne to zwracamy linie tylu karty
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    drawLines.Add(lines[52 * 7 + i]);
                }
            }

            return drawLines;
        }

        // Funkcja odpowiedzialna za przekladanie karty z gory stosu na dol
        public void Draw()
        {
            // Jesli nie ma kart na stosie to bierzemy karty boczne i tasujemy je
            if (cards.Count == 0)
            {
                Shuffle();
            }
            // W przeciwnym razie bierzemy wierzchnia karte i przekladamy ja na karty boczne
            else
            {
                sideCards.Add(cards[0]);
                cards.RemoveAt(0);
            }
        }

        // Funkcja odpowiedzialna za przekladanie kart ze stosu bocznego na glowny i ich tasowanie
        void Shuffle()
        {
            // Przekladamy karty z bocznego stosu na glowny
            cards = sideCards;
            sideCards = new List<int>();

            // Tasujemy karty na glownym stosie
            Random rand = new Random();
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                int temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }
    }
}
