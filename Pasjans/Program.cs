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

namespace Pasjans
{
    internal class Program
    {
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

            // Stworzenie stosow kart
            Stack[] stacks = new Stack[7];
            for (int i = 0;i < 7; i++)
            {
                stacks[i] = new Stack(stackPlaces[i], 1);
            }

            stacks[0].exposed = 4;

            // Pierwsze wyrysowanie kart
            Draw(lines, stacks, "Wykonaj ruch.");

            string info = "";

            stacks[0].PrintStack();

            while (true)
            {
                Console.Clear();
                Draw(lines, stacks, info);

                string input = Console.ReadLine();

                string[] inputElements = input.Split(' ');

                if (inputElements[0] == "m" || inputElements[0] == "move")
                {
                    if (inputElements.Length == 4 &&
                        int.TryParse(inputElements[1], out int stackOne) && 
                        int.TryParse(inputElements[2], out int amount) && 
                        int.TryParse(inputElements[3], out int stackTwo))
                    {
                        info = MoveCard(stacks, stackOne - 1, stackTwo - 1);
                    }
                    else
                        info = "Nieprawidlowe wejscie.";
                }
                else if (inputElements[0] == "rem")
                {
                    stacks[0].DeleteTop(1);
                    info = "Usunięto kartę.";
                }
                else if (inputElements[0] == "exit")
                    break;
                else
                    info = "Niepoprawne wejście.";
            }
        }

        static string MoveCard(Stack[] stacks, int stackOne, int stackTwo)
        {
            int[] cards = stacks[stackOne].GetTopCards(2);
            int cardTwo = stacks[stackTwo].GetTopCards(1)[0];
            int valueOne = cards[cards.Length - 1] / 4;
            int valueTwo = cardTwo / 4;
            int suitOne = cards[cards.Length - 1] % 4;
            int suitTwo = cardTwo % 4;

            //if (valueOne == valueTwo - 1 && ((suitOne >= 2 && suitTwo < 2) || (suitTwo >= 2 && suitOne < 2)))
            //{

            Debug.WriteLine($"Przesuwam z talii {stackOne} do {stackTwo}");
            Debug.WriteLine("Przed ruch:");
            stacks[stackOne].PrintStack();
            stacks[stackTwo].PrintStack();

            stacks[stackOne].DeleteTop(2);
            stacks[stackTwo].AddCards((int[])cards.Clone());
            //stacks[stackOne].exposed -= 3;
            stacks[stackTwo].exposed += 2;

            Debug.WriteLine("Po ruchu:");
            stacks[stackOne].PrintStack();
            stacks[stackTwo].PrintStack();

            return "Przesunięto kartę.";
            //}

            //return "Nieprawidłowy ruch.";

        }

        // Rysowanie w konsoli stosow
        static void Draw(string[] lines, Stack[] stacks, string info)
        {
            // Tworzenie tablicy list do przechowywania linii do wydrukowania otrzymanych od stosow
            List<string>[] stackLines = new List<string>[7];
            for (int i = 0; i < 7; i++)
            {
                stackLines[i] = new List<string>();
            }

            // Pobieranie od stosow linii do wyprintowania
            for (int i = 0; i < 7; i++)
            {
                stackLines[i] = stacks[i].GenerateDrawLines(lines);
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

            Console.WriteLine(info);

            foreach (Stack stack in stacks)
            {
                Console.Write(stack.places.Count + "," + stack.exposed + ";");
            }
            Console.WriteLine();

            stacks[0].PrintStack();

            Console.Write("> ");
        }
    }
}
