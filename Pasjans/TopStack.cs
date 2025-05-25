using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasjans
{
    internal class TopStack : Stack
    {
        int stackID;

        public TopStack(int stackID) : base(new List<int>()) 
        {
            this.stackID = stackID;
        }

        // Funkcja odpowiedzialna za zwracanie linii do wypisania w konsoli
        public override List<string> GenerateDrawLines(string[] lines)
        {
            List<string> drawLines = new List<string>();

            if (cards.Count == 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    drawLines.Add(lines[(54 + stackID) * 7 + i]);
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    drawLines.Add(lines[cards[0] * 7 + i]);
                }
            }

            return drawLines;
        }

        public override int[] GetTopCards(int amount)
        {
            if (this.cards.Count == 0)
                return new[] { -1 };

            int[] cards = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                cards[i] = this.cards[i];
            }
            return cards;
        }
    }
}
