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
    // Klasa stosow na ktorych znajduja sie karty
    // Wazne: w kartach na stosie w liscie places karta najbardziej na wierzchu to ta z indeksem 0
    internal class Stack
    {
        // Tworzenie listy na przechowywanie kart jakie sa na stosie
        public List<int> places = new List<int>();

        // Zmienna ktora przechowuje ilosc kart widocznych dla gracza (odslonietych)
        public int exposed;

        // Presety do rysowania kart
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
            
            // Jesli stos jest pusty zwracamy 25 pustych linii
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
            // Potem jesli jest wiecej kart odslonietych niz tylko ta na samym wierzchu dodajemy ich wystajace gory z wartoscia pobrana z pliku
            if ( exposed > 1)
                for (int i = 0; i < exposed - 1; i++)
                {
                    drawLines.Add(topPreset);
                    drawLines.Add(lines[places[exposed - 1 - i] * 7 + 1]);
                }
            // Potem dajemy linie karty ktora jest na samym wierzchu pobrane z pliku
            for (int i = 0; i < 7; i++)
            {
                drawLines.Add(lines[places[0] * 7 + i]);
            }
            // Reszte linii dajemy puste aby razem bylo 25
            for (int i = 0; i < 25 - (places.Count + 5 + exposed); i++)
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

        // Funkcja zwracajaca okreslona ilosc kart ze szczytu stosu
        public int[] GetTopCards(int amount)
        {
            int[] cards = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                cards[i] = places[i];
            }
            return cards;
        }

        // Funkcja usuwajaca okreslona ilosc kart z gory stosu
        public void DeleteTop(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                places.RemoveAt(0);
            }

            exposed -= amount;
            if (exposed < 1 && places.Count > 0)
                exposed = 1;
        }

        // Funkcja dodajaca okreslone karty na szczyt stosu
        public void AddCards(int[] cards)
        {
            exposed += cards.Length;

            for (int i = 0; i < cards.Length; i++)
            {
                places.Insert(i, cards[i]); 
            }
        }

        // Funkcja do debugowania
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
