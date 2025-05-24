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
    internal class Stack
    {
        // Tworzenie listy na przechowywanie kart jakie sa na stosie
        public List<int> places = new List<int>();

        public int exposed;

        // Preset pustego miejsca
        const string emptyPreset = "           ";
        const string topPreset = "┌─────────┐";

        // Konstruktor, pobieranie miejsc przy tworzeniu stosu
        public Stack(List<int> places, int exposed) 
        {
            this.places = places;
            this.exposed = exposed;
        }

        // Funkcja zwracajaca linie jakie nalezy wydrukowac w konsoli
        public List<string> GenerateDrawLines(string[] lines)
        {
            // Zmienna do przechowywania linii
            List<string> drawLines = new List<string>();
            
            // Jesli stos jest pusty zwracamy 20 pustych linii
            if (places.Count == 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    drawLines.Add(emptyPreset);
                }
                return drawLines;
            }

            // Najpierw dajemy linie wystajace gory kart zakrytych
            for (int i = 0; i < places.Count - exposed; i++)
            {
                drawLines.Add(topPreset);
            }
            for (int i = 0; i < exposed - 1; i++)
            {
                drawLines.Add(topPreset);
                drawLines.Add(lines[places[exposed - 1 + i] * 7 + 1]);
            }
            // Potem dajemy linie karty ktora jest odslonieta pobrane z pliku
            for (int i = 0; i < 7; i++)
            {
                drawLines.Add(lines[places[0] * 7 + i]);
            }
            // Reszte linii dajemy puste aby razem bylo 20
            for (int i = 0; i < 25 - (places.Count + 5 + exposed); i++)
            {
                drawLines.Add(emptyPreset);
            }

            return drawLines;
        }

        public int GetExposed()
        {
            return exposed;
        }

        public int[] GetTopCards(int amount)
        {
            int[] cards = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                cards[i] = places[i];
            }
            return cards;
        }

        // Funkcja usuwajaca gorna karte stosu
        public void DeleteTop(int amount)
        {
            if (exposed > amount)
                exposed -= amount;
            for (int i = 0; i < amount; i++)
            {
                places.RemoveAt(0);
            }
        }

        public void AddCards(int[] cards)
        {

            for (int i = cards.Length - 1; i >= 0; i--)
            {
                places.Insert(0, cards[i]); 
            }

            Debug.WriteLine("Dodaję do talii:");
            foreach (var c in cards)
                Debug.Write(c + " ");
            Debug.WriteLine("");
        }

        public void PrintStack()
        {
            foreach (int i in places)
            {
                Debug.Write(i + " ");
            }
            Debug.WriteLine("");
        }
    }
}
