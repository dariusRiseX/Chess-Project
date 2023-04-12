using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Move
    {
        public int Piece { get; set; }
        public int MoveFrom { get; set; }
        public int MoveTo { get; set; }
        public int MoveScore { get; set; } // Not actual eval of move, just for quick sort to help alpha beta pruning
    }
}
