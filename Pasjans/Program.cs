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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Data;

namespace Pasjans
{
    internal class Program
    {
        // Preset pustej linii do rysowania stosu
        const string emptyPreset = "           ";

        // Zmienna przechwujaca karty
        static int[] cards = new int[52];

        // Stworzenie tablicy list do przechowywania wstepnych miejsc kart
        static List<int>[] stackPlaces = new List<int>[7];

        // Stworzenie listy na karty poczatkowe do stosu do dobierania i wylosowanie ich
        static List<int> drawStackCards = new List<int>();

        // Stworzenie stosu do dobierania
        static DrawStack drawStack;

        static TopStack[] topStacks = new TopStack[4];

        // Stworzenie stosow kart
        static PlayStack[] stacks = new PlayStack[7];

        static string info;

        static void Main(string[] args)
        {
            // Ustawienie enkodowania aby dzialaly znaki specjalne
            Console.OutputEncoding = Encoding.UTF8;

            // Maksymalizacja okna konsoli
            ShowWindow(GetConsoleWindow(), SW_MAXIMIZE);

            // Ustawienie wielkosci czcionki w konsoli
            SetConsoleFontSize(20); // Ustawienie rozmiaru czcionki konsoli

            // Wczytanie ascii artu kart z pliku
            string[] lines = File.ReadAllLines(@"..\..\cardArt.txt");

            // Wywolanie funkcji ktora przygotowuje gre
            Restart();

            info = "Wykonaj ruch.";

            // Petla gry
            while (true)
            {
                // Rysowanie w konsoli elementow
                Draw(lines, topStacks, drawStack, stacks, info);

                // Pobieranie danych od gracza
                string input = Console.ReadLine();
                string[] inputElements = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                info = "Niepoprawne wejście";

                // Sprawdzenie czy wejscie nie jest puste
                if (inputElements.Length != 0)
                {
                    string command = inputElements[0];

                    // Jesli wybrano komende przesuwania kart sprawdzamy czy argumenty sa poprawne i wywolujemy funkcje od przesuwania kart
                    if (inputElements.Length >= 2)
                    {
                        // Sprawdzanie czy uzyto odpowiednich argumentow do przesuwania kart
                        bool elemOneParse = int.TryParse(inputElements[0], out int elemOne);
                        bool elemTwoParse = int.TryParse(inputElements[1], out int elemTwo);
                        bool elemThreeParse = false;
                        int elemThree = 0;
                        if (inputElements.Length >= 3)
                            elemThreeParse = int.TryParse(inputElements[2], out elemThree);
                        // W tym przypadku przesuwamy okreslona ilosc kart ze stosu na stos
                        if (inputElements.Length == 3 &&
                            elemOneParse &&
                            elemTwoParse &&
                            IsValidStackIndex(elemTwo) &&
                            elemThreeParse &&
                            IsValidStackIndex(elemThree))
                        {
                            info = MoveCards(stacks[elemTwo - 1], stacks[elemThree - 1], elemOne);
                        }
                        // W tym przypadku przesuwamy jedna karte ze stosu na stos
                        if (inputElements.Length == 2 &&
                            elemOneParse &&
                            IsValidStackIndex(elemOne) &&
                            elemTwoParse &&
                            IsValidStackIndex(elemTwo))
                        {
                            info = MoveCards(stacks[elemOne - 1], stacks[elemTwo - 1], 1);
                        }
                        // W tym przypadku pobieramy karte ze stosu do pobierania na jeden ze zwyklych stosow
                        else if (inputElements[0] == "d" && elemTwoParse)
                        {
                            info = MoveCards(drawStack, stacks[elemTwo - 1], 1);
                        }
                        // W tym przypadku pobieramy karte ze stosu do pobierania na jeden ze stosow koncowych
                        else if (inputElements[0] == "d" &&
                            inputElements[1][0] == 'e' &&
                            int.TryParse(inputElements[1][1].ToString(), out int topStackDrawDep) &&
                            topStackDrawDep >= 1 && topStackDrawDep <= 4)
                        {
                            info = MoveCards(drawStack, topStacks[topStackDrawDep - 1], 1);
                        }
                        // W tym przypadku przesuwamy karte z jednego ze stosow koncowych na zwykly stos
                        else if (inputElements[0][0] == 'e' &&
                            int.TryParse(inputElements[0][1].ToString(), out int topStackFrom) &&
                            topStackFrom >= 1 && topStackFrom <= 4 &&
                            elemTwoParse &&
                            IsValidStackIndex(elemTwo))
                        {
                            info = MoveCards(topStacks[topStackFrom - 1], stacks[elemTwo - 1], 1);
                        }
                        // W tym przypadku przesuwamy karte ze zwyklego stosu na stos koncowy
                        else if (elemOneParse &&
                            IsValidStackIndex(elemOne) &&
                            inputElements[1][0] == 'e' &&
                            int.TryParse(inputElements[1][1].ToString(), out int topStackDep) &&
                            topStackDep >= 1 && topStackDep <= 4)
                        {
                            info = MoveCards(stacks[elemOne - 1], topStacks[topStackDep - 1], 1);
                        }

                        if (topStacks.All(stack => stack.GetCardAmount() == 13))
                        {
                            info = "Gratulacje, wygrałeś! Aby zagrać ponownie, wciśnij jakikolwiek przycisk.";
                            Draw(lines, topStacks, drawStack, stacks, info);
                            Console.ReadKey();
                            Restart();
                            info = "Zrestartowano grę. Wykonaj ruch.";
                        }
                    }
                    // Jesli wybrano komende do przekladania kart ze stosu to wywolujemy funkcje ktora to robi
                    else if (command == "d" || command == "draw" || command == "dobierz")
                    {
                        drawStack.Draw();
                        info = "Dobrano kartę.";
                    }
                    // Jesli wybrano odpowiednia komende to restartujemy gre
                    else if (command == "r" || command == "restart")
                    {
                        Restart();
                        info = "Zrestartowano grę. Wykonaj ruch.";
                    }
                    // Jesli wybrano komende do wyswietlania pomocy to otwieramy plik z pomoca
                    else if (command == "h" || command == "help" || command == "p" || command == "pomoc")
                    {
                        Process.Start(@"..\..\..\README.txt");
                        info = "Otwarto plik pomocy.";
                    }
                    // Jesli wybrano komende wyjscia to wychodzimy z gry
                    else if (command == "e" || command == "exit" || command == "w" || command == "wyjscie")
                        break;
                }
            }
        }

        // Funkcja odpowiadajaca za restartowanie gry
        static void Restart()
        {
            // Inicjalizacja tablicy kart
            cards = new int[52];

            // Wypelnienie tablicy kartami (0-51)
            for (int i = 0; i < 52; i++)
            {
                cards[i] = i;
            }

            // Tasowanie kart alorytmem Fishera-Yates'a
            Random r = new Random();
            for (int i = cards.Length - 1; i > 0; i--)
            {
                int j = r.Next(i + 1);
                int temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }

            // Inicjalizacja miejsc na stosy i umieszczanie na nich kart
            stackPlaces = new List<int>[7];
            for (int i = 0; i < 7; i++)
            {
                stackPlaces[i] = new List<int>();
                for (int j = 0; j < i + 1; j++)
                {
                    stackPlaces[i].Add(cards[0]);
                    cards = cards.Skip(1).ToArray();
                }
            }

            // Inicjalizacja stosu do dobierania i umieszczanie na nim kart
            drawStackCards = new List<int>();
            for (int i = 0; i < 24; i++)
            {
                drawStackCards.Add(cards[0]);
                cards = cards.Skip(1).ToArray();
            }

            drawStack = new DrawStack(drawStackCards);

            // Inicjalizacja stosow koncowych
            topStacks = new TopStack[4];
            for (int i = 0; i < 4; i++)
            {
                topStacks[i] = new TopStack(i);
            }

            // Inicjalizacja stosow zwyklych
            stacks = new PlayStack[7];
            for (int i = 0; i < 7; i++)
            {
                stacks[i] = new PlayStack(stackPlaces[i]);
            }
        }

        // Funkcja sprawdzajaca czy dany indeks jest odpowiedni (czy odpowiada jakiemus stosowi)
        static bool IsValidStackIndex(int index)
        {
            return index >= 1 && index <= 7;
        }

        // Funkcja odpowiadajca za przesuwanie kart
        static string MoveCards(dynamic stackOne, dynamic stackTwo, int amount)
        {
            string info = "Niedozwolony ruch.";

            // Przechodzimy dalej tylko jesli ilosc do przesuniecia jest mniejsza lub rowna ilosci kart na stosie z ktorego bierzemy
            // lub w przypadku brania ze stosu zwyklego jesli ilosc ktora bierzemy jest mniejsza lub rowna ilosci odslonietych
            if ((stackOne.GetCardAmount() >= amount && !(stackOne is PlayStack)) || ((stackOne is PlayStack) && stackOne.GetExposed() >= amount))
            {
                // Pobieramy ze stosu pierwszego karty do przesuniecia
                int[] cards = (int[])stackOne.GetTopCards(amount).Clone();
                int cardOne = cards[amount - 1];

                int cardTwo = 0;
                bool isTwoEmpty = false;

                // Jesli drugi stos nie jest pusty to bierzemy z niego wierzchnia karte a jesli jest to zapisujemy to w zmiennej isTwoEmpty
                if (stackTwo.GetCardAmount() > 0)
                    cardTwo = ((int[])stackTwo.GetTopCards(1).Clone())[0];
                else
                {
                    isTwoEmpty = true;
                    // Jesli przesuwamy do stosu koncowego i jest on pusty to ustawiamy karte 2 na indeks tego stosu zeby mozna bylo sprawdzic
                    // czy karty maja taki sam kolor
                    if (stackTwo is TopStack)
                        cardTwo = stackTwo.GetID();
                }

                // Sprawdzamy funkcja CanMoveCard czy mozna przesunac karte biorac pod uwage nasze karty, czy stos drugi jest pusty oraz
                // czy przesuwamy na stos koncowy i jesli mozna to je przesuwamy
                if (CanMoveCard(cardOne, cardTwo, isTwoEmpty, stackTwo is TopStack))
                {
                    stackOne.DeleteTop(amount);
                    stackTwo.AddCards(cards);
                    
                    info = "Przesunięto karty.";
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

            // Jesli odkladamy karte na stos koncowy
            if (isDeposit)
            {
                // Jesli ten stos jest pusty to zwracamy prawde jesli wartosc pierwszej karty to 0 (as)
                if (isTwoEmpty && suitOne == suitTwo)
                {
                    return valueOne == 0;
                }

                // Zwracamy prawde jesli karty maja taki sam kolor i jedna ma wyzsza o 1 wartosc od drugiej
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

            // Pobieranie linii stosow koncowych do wydrukowania
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
                            stackLines[j].Add(drawStackLines[7 + i]); break;
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

            // Printowanie linii gornych stosow w konsoli
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Console.Write(stackLines[j][i] + " ");
                }
                Console.WriteLine();
            }

            // Printowanie numerow stosow w konsoli
            for (int i = 0; i < 7; i++)
            {
                Console.Write($"     {i + 1}      ");
            }
            Console.WriteLine();

            // Printowanie linii zwyklych stosow w konsoli
            for (int i = 0; i < 29; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Console.Write(stackLines[j][i + 7] + " ");
                }
                Console.WriteLine();
            }

            // Printowanie informacji dla uzytkownika
            Console.WriteLine(info);

            Console.Write("> ");
        }

        // Potrzebne do maksymalizowania okna konsoli
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_MAXIMIZE = 3;

        // Potrzebne do ustawiania czcionki konsoli
        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public int cbSize;
            public int nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        const int STD_OUTPUT_HANDLE = -11;

        static void SetConsoleFontSize(short fontSize)
        {
            IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);
            CONSOLE_FONT_INFO_EX info = new CONSOLE_FONT_INFO_EX();
            info.cbSize = Marshal.SizeOf(info);
            info.FaceName = "Consolas";
            info.dwFontSize = new COORD() { X = 0, Y = fontSize };
            SetCurrentConsoleFontEx(hnd, false, ref info);
        }
    }
}

