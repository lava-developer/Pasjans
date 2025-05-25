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
        // Konstruktor z klasy bazowej
        public DrawStack(List<int> cards) : base(cards) { }

        // Funkcja odpowiedzialna za przekladanie karty z gory stosu na dol
        public void Draw()
        {
            int card = cards[0];
            cards.RemoveAt(0);
            cards.Insert(cards.Count, card);
        }
    }
}
