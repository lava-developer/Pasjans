using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Pasjans
{
    internal class Program
    {
        // Preset pustego miejsca
        const string emptyPreset = "           ";

        static void Main(string[] args)
        {
            // Ustawienie enkodowania aby dzialaly znaki specjalne
            Console.OutputEncoding = Encoding.UTF8;

            // Wczytanie ascii artu kart z pliku
            string[] lines = File.ReadAllLines(@"..\..\cardArt.txt");

            // Stworzenie tablicy list do przechowywania wstepnych miejsc kart
            List<int>[] stackPlaces = new List<int>[7];
            for (int i = 0; i < 7; i++)
            {
                stackPlaces[i] = new List<int>();
            }

            // Losowanie kart w dane miejsca
            Random r = new Random();
            for (int i = 0; i < 7; i++)
            { 
                for (int j = i; j < 7; j++)
                {
                    stackPlaces[i].Add(r.Next(0, 52));
                }
            }

            // Stworzenie listy na karty poczatkowe do stosu do dobierania i wylosowanie ich
            List<int> drawStackCards = new List<int>();
            for (int i = 0; i < 24;  i++)
            {
                drawStackCards.Add(r.Next(0, 52));
            }

            // Stworzenie stosu do dobierania
            DrawStack drawStack = new DrawStack(drawStackCards);

            // Stworzenie stosow kart
            PlayStack[] stacks = new PlayStack[7];
            for (int i = 0;i < 7; i++)
            {
                stacks[i] = new PlayStack(stackPlaces[i], 1);
            }

            // Deklaracja zmiennej przechowujacej informacje ktore beda wyswietlane w konsoli
            string info = "Wykonaj ruch.";

            // Petla gry
            while (true)
            {
                // Rysowanie elementow w konsoli
                Console.Clear();
                Draw(lines, stacks, drawStack, info);

                // Pobieranie danych od gracza
                string input = Console.ReadLine();
                string[] inputElements = input.Split(new[] { ' ' },StringSplitOptions.RemoveEmptyEntries);

                info = "Niepoprawne wejście";

                // Sprawdzenie czy wejscie nie jest puste
                if (inputElements.Length != 0)
                {
                    string command = inputElements[0];

                    // Jesli wybrano komende przesuwania kart i argumenty sa poprawne wywolujemy funkcje przesuwajaca karty
                    if (command == "m" || command == "move" || command == "p" || command == "przesun")
                    {
                        if (inputElements.Length == 4 &&
                        int.TryParse(inputElements[1], out int amount) &&
                        int.TryParse(inputElements[2], out int stackOne) &&
                        int.TryParse(inputElements[3], out int stackTwo))
                        {
                            info = MoveCards(stacks, amount, stackOne - 1, stackTwo - 1);
                        }
                        // W tym przypadku pobieramy karte ze stosu
                        else if (inputElements[1] == "d" &&
                        int.TryParse(inputElements[2], out int stack))
                        {
                            info = MoveCardFromDraw(drawStack, stacks, stack - 1);
                        }
                    }
                    // Jesli wybrano komende do przekladania kart ze stosu to wywolujemy funkcje ktora to robi
                    else if (command == "d" || command == "draw" || command == "dobierz")
                    {
                        drawStack.Draw();
                        info = "Dobrano kartę.";
                    }
                    // Funkcja do debugowania
                    else if (command == "rem")
                    {
                        stacks[0].DeleteTop(1);
                        info = "Usunięto kartę.";
                    }
                    // Jesli wybrano komende wyjscia to wychodzimy z gry
                    else if (command == "e" || command == "exit" || command == "w" || command == "wyjscie")
                        break;
                }
            }
        }

        // Funkcja odpowiadajca za przesuwanie kart
        static string MoveCards(PlayStack[] stacks, int amount, int stackOne, int stackTwo)
        {
            string info = "Niedozwolony ruch.";

            
            // Przechodzimy dalej tylko jesli ilosc jest mniejsza lub rowna ilosci odlonietych (nie mozna zabrac wiecej niz widzimy)
            if (stacks[stackOne].GetExposed() >= amount)
            {
                // Pobieramy ze stosu pierwszego karty do przesuniecia
                int[] cards = (int[])stacks[stackOne].GetTopCards(amount).Clone();
                int cardOne = cards[amount - 1];
                int cardTwo = 0;
                bool isTwoEmpty = false;

                // Jesli drugi stos nie jest pusty to bierzemy z niego wierzchnia karte a jesli jest to zapisujemy to w zmiennej isTwoEmpty
                if (stacks[stackTwo].GetExposed() > 0)
                    cardTwo = ((int[])stacks[stackTwo].GetTopCards(1).Clone())[0];
                else
                    isTwoEmpty = true;

                Debug.WriteLine(cardOne);
                Debug.WriteLine(cardTwo);

                // Sprawdzamy funkcja CanMoveCard czy mozna przesunac karte biorac pod uwage nasze karty i czy stos drugi jest pusty i jesli mozna to je przesuwamy
                if (CanMoveCard(cardOne, cardTwo, isTwoEmpty))
                {
                    stacks[stackOne].DeleteTop(amount);
                    stacks[stackTwo].AddCards(cards);
                    
                    info = "Przesunięto karty.";
                }
            }

            return info;
        }

        // Funkcja sluzaca do przekladania karty ze stosu do dobierania do zwyklego stosu
        static string MoveCardFromDraw(DrawStack drawStack, PlayStack[] stacks, int stack)
        {
            string info = "Nieprawidłowy ruch.";

            // Pobieramy ze stosu do dobierania karte do przesuniecia
            int cardOne = drawStack.GetTopCards(1)[0];
            int cardTwo = 0;
            bool isTwoEmpty = false;

            // Jesli drugi stos nie jest pusty to bierzemy z niego wierzchnia karte a jesli jest to zapisujemy to w zmiennej isTwoEmpty
            if (stacks[stack].GetExposed() > 0)
                cardTwo = stacks[stack].GetTopCards(1)[0];
            else
                isTwoEmpty = true;

            // Sprawdzamy funkcja CanMoveCard czy mozna przesunac karte biorac pod uwage nasze karty i czy stos drugi jest pusty i jesli mozna to je przesuwamy
            if (CanMoveCard(cardOne, cardTwo, isTwoEmpty))
            {
                drawStack.DeleteTop(1);
                stacks[stack].AddCards(new[] { cardOne });

                info = "Przesunięto karty.";
            }

            return info;
        }

        // Funkcja sprawdzajaca czy mozna przesunac karty
        static bool CanMoveCard(int cardOne, int cardTwo, bool isTwoEmpty)
        {
            // Obliczanie wartosci kart i ich kolory na podstawie ich id
            int valueOne = cardOne / 4;
            int valueTwo = cardTwo / 4;
            int suitOne = cardOne % 4;
            int suitTwo = cardTwo % 4;

            // Zwracamy odpowiedz na podstawie warunkow
            return (valueOne == valueTwo - 1 && ((suitOne >= 2 && suitTwo < 2) || (suitTwo >= 2 && suitOne < 2))) ||
                (isTwoEmpty && valueOne == 12);
        }

        // Rysowanie w konsoli stosow
        static void Draw(string[] lines, PlayStack[] stacks, DrawStack drawStack, string info)
        {
            Console.Clear();

            // Tworzenie tablicy list do przechowywania linii do wydrukowania otrzymanych od stosow
            List<string>[] stackLines = new List<string>[7];
            for (int i = 0; i < 7; i++)
            {
                stackLines[i] = new List<string>();
            }
            
            // Pobieranie od stosu do dobierania linii do wydrukowania
            List<string> drawStackLines = drawStack.GenerateDrawLines(lines);

            // Dodawanie do linii do wydruku linii stosow ktore beda na gorze
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (j == 0)
                    {
                        stackLines[j].Add(drawStackLines[i]);
                    }
                    else if (j == 1)
                    {
                        stackLines[j].Add(lines[52 * 7 + i]);
                    }
                    else
                    {
                        stackLines[j].Add(emptyPreset);
                    }
                }
            }

            // Pobieranie od stosow linii do wyprintowania
            for (int i = 0; i < 7; i++)
            {
                foreach (string line in stacks[i].GenerateDrawLines(lines))
                    stackLines[i].Add(line);
            }

            // Printowanie linii w konsoli
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Console.Write(stackLines[j][i] + " ");
                }
                Console.WriteLine();
            }

            // Printowanie informacji dla uzytkownika
            Console.WriteLine(info);

            Console.Write("> ");
        }
    }
}
