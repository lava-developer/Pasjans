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

            TopStack[] topStacks = new TopStack[4];
            for (int i = 0; i < 4; i++)
            {
                topStacks[i] = new TopStack(i);
            }

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
                Draw(lines, topStacks, drawStack, stacks, info);

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
                            stackOne >= 1 && stackOne <= 7 &&
                            int.TryParse(inputElements[3], out int stackTwo) &&
                            stackTwo >= 1 && stackTwo <= 7)
                        {
                            info = MoveCards(stacks, amount, stackOne - 1, stackTwo - 1);
                        }
                        // W tym przypadku pobieramy karte ze stosu
                        else if (inputElements[1] == "d" &&
                            int.TryParse(inputElements[2], out int stackDraw))
                        {
                            info = MoveCardFromDraw(drawStack, stacks[stackDraw - 1]);
                        }
                        else if (inputElements[1][0] == 't' &&
                            int.TryParse(inputElements[1][1].ToString(), out int topStackFrom) &&
                            topStackFrom >= 1 && topStackFrom <= 4 &&
                            int.TryParse(inputElements[2], out int stackTopFrom) &&
                            stackTopFrom >= 1 && stackTopFrom <= 7)
                        {
                            info = MoveCardTop(topStacks[topStackFrom - 1], stacks[stackTopFrom - 1], true);
                        }
                        else if (int.TryParse(inputElements[1], out int stackTopDep) &&
                            stackTopDep >= 1 && stackTopDep <= 7 &&
                            inputElements[2][0] == 't' &&
                            int.TryParse(inputElements[2][1].ToString(), out int topStackDep) &&
                            topStackDep >= 1 && topStackDep <= 4)
                        {
                            info = MoveCardTop(topStacks[topStackDep - 1], stacks[stackTopDep - 1], false);
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

                // Sprawdzamy funkcja CanMoveCard czy mozna przesunac karte biorac pod uwage nasze karty i czy stos drugi jest pusty i jesli mozna to je przesuwamy
                if (CanMoveCard(cardOne, cardTwo, isTwoEmpty, false))
                {
                    stacks[stackOne].DeleteTop(amount);
                    stacks[stackTwo].AddCards(cards);
                    
                    info = "Przesunięto karty.";
                }
            }

            return info;
        }

        // Funkcja sluzaca do przekladania karty ze stosu do dobierania do zwyklego stosu
        static string MoveCardFromDraw(DrawStack drawStack, PlayStack stack)
        {
            string info = "Nieprawidłowy ruch.";

            // Pobieramy ze stosu do dobierania karte do przesuniecia
            int cardOne = drawStack.GetTopCards(1)[0];
            int cardTwo = 0;
            bool isTwoEmpty = false;

            // Jesli drugi stos nie jest pusty to bierzemy z niego wierzchnia karte a jesli jest to zapisujemy to w zmiennej isTwoEmpty
            if (stack.GetExposed() > 0)
                cardTwo = stack.GetTopCards(1)[0];
            else
                isTwoEmpty = true;

            // Sprawdzamy funkcja CanMoveCard czy mozna przesunac karte biorac pod uwage nasze karty i czy stos drugi jest pusty i jesli mozna to je przesuwamy
            if (CanMoveCard(cardOne, cardTwo, isTwoEmpty, false))
            {
                drawStack.DeleteTop(1);
                stack.AddCards(new[] { cardOne });

                info = "Przesunięto karty.";
            }

            return info;
        }

        static string MoveCardTop(TopStack topStack, PlayStack stack, bool isFrom)
        {
            string info = "Nieprawidłowy ruch.";

            Debug.WriteLine("a");

            // Pobieramy ze stosu do dobierania karte do przesuniecia
            int cardOne = 0;
            int cardTwo = 0;
            bool isTwoEmpty = false;

            if (isFrom)
            {
                cardOne = topStack.GetTopCards(1)[0];
                // Jesli drugi stos nie jest pusty to bierzemy z niego wierzchnia karte a jesli jest to zapisujemy to w zmiennej isTwoEmpty
                if (stack.GetExposed() > 0)
                    cardTwo = stack.GetTopCards(1)[0];
                else
                    isTwoEmpty = true;

                if (CanMoveCard(cardOne, cardTwo, isTwoEmpty, false))
                {
                    topStack.DeleteTop(1);
                    stack.AddCards(new[] { cardOne });
                }
            }
            else
            {
                Debug.WriteLine("b");
                cardOne = stack.GetTopCards(1)[0];
                // Jesli drugi stos nie jest pusty to bierzemy z niego wierzchnia karte a jesli jest to zapisujemy to w zmiennej isTwoEmpty
                if (stack.GetExposed() > 0)
                    cardTwo = topStack.GetTopCards(1)[0];
                else
                    isTwoEmpty = true;

                if (CanMoveCard(cardOne, cardTwo, isTwoEmpty, true))
                {
                    Debug.WriteLine("c");
                    stack.DeleteTop(1);
                    topStack.AddCards(new[] { cardOne });
                }
            }

            return info;
        }

        // Funkcja sprawdzajaca czy mozna przesunac karty
        static bool CanMoveCard(int cardOne, int cardTwo, bool isTwoEmpty, bool isDeposit)
        {
            // Obliczanie wartosci kart i ich kolory na podstawie ich id
            int valueOne = cardOne / 4;
            int valueTwo = cardTwo / 4;
            int suitOne = cardOne % 4;
            int suitTwo = cardTwo % 4;

            if (isDeposit)
            {
                return valueOne == valueTwo + 1 && suitOne == suitTwo;
            }

            // Zwracamy odpowiedz na podstawie warunkow
            return (valueOne == valueTwo - 1 && ((suitOne >= 2 && suitTwo < 2) || (suitTwo >= 2 && suitOne < 2))) ||
                (isTwoEmpty && valueOne == 12);
        }

        // Rysowanie w konsoli stosow
        static void Draw(string[] lines, TopStack[] topStacks, DrawStack drawStack, PlayStack[] stacks, string info)
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

            List<string>[] topStackLines = new List<string>[4];
            for (int i = 0; i < 4; i++)
            {
                topStackLines[i] = topStacks[i].GenerateDrawLines(lines);
            }

            // Dodawanie do linii do wydruku linii stosow ktore beda na gorze
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    switch (j)
                    {
                        case 0:
                            stackLines[j].Add(drawStackLines[i]); break;
                        case 1:
                            stackLines[j].Add(lines[52 * 7 + i]); break;
                        case 2:
                            stackLines[j].Add(emptyPreset); break;
                        default:
                            stackLines[j].Add(topStackLines[j - 3][i]); break;
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
