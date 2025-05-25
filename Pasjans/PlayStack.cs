using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasjans
{
    // Klasa stosow na ktorych znajduja sie karty
    // Uwaga: wierzchnia karta w places to ta z indeksem 0
    internal class PlayStack : Stack
    {
        // Zmienna ktora przechowuje ilosc kart widocznych dla gracza (odslonietych)
        public int exposed = 1;

        // Presety do rysowania kart
        const string emptyPreset = "           ";
        const string topPreset = "┌─────────┐";

        // Konstruktor z klasy bazowej
        public PlayStack(List<int> cards) : base(cards) { }

        // Funkcja zwracajaca linie jakie nalezy wydrukowac w konsoli
        public override List<string> GenerateDrawLines(string[] lines)
        {
            // Zmienna do przechowywania linii
            List<string> drawLines = new List<string>();

            // Jesli stos jest pusty zwracamy 25 pustych linii
            if (cards.Count == 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    drawLines.Add(emptyPreset);
                }
                return drawLines;
            }

            // Najpierw dajemy linie wystajace gory kart zakrytych
            for (int i = 0; i < cards.Count - exposed; i++)
            {
                drawLines.Add(topPreset);
            }
            // Potem jesli jest wiecej kart odslonietych niz tylko ta na samym wierzchu dodajemy ich wystajace gory z wartoscia pobrana z pliku
            if (exposed > 1)
                for (int i = 0; i < exposed - 1; i++)
                {
                    drawLines.Add(topPreset);
                    drawLines.Add(lines[cards[exposed - 1 - i] * 7 + 1]);
                }
            // Potem dajemy linie karty ktora jest na samym wierzchu pobrane z pliku
            for (int i = 0; i < 7; i++)
            {
                drawLines.Add(lines[cards[0] * 7 + i]);
            }
            // Reszte linii dajemy puste aby razem bylo 25
            for (int i = 0; i < 25 - (cards.Count + 5 + exposed); i++)
            {
                drawLines.Add(emptyPreset);
            }

            return drawLines;
        }

        // Funkcja zwracajaca ilosc odslonietych kart
        public int GetExposed()
        {
            return exposed;
        }

        // Funkcja usuwajaca okreslona ilosc kart z gory stosu
        public override void DeleteTop(int amount)
        {
            base.DeleteTop(amount);

            exposed -= amount;
            if (exposed < 1 && cards.Count > 0)
                exposed = 1;
        }

        // Funkcja dodajaca okreslone karty na szczyt stosu
        public override void AddCards(int[] cards)
        {
            exposed += cards.Length;

            base.AddCards(cards);
        }
    }
}
