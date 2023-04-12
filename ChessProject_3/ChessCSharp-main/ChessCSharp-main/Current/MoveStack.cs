using System;
using System.Collections.Generic;

namespace Chess
{
    class MoveStack
    {
        public static List<Move> moveStack = new List<Move>();
        public static List<Move> PoppedMoves = new List<Move>();

        // Changes given move to descriptive notation
        // For example - Piece = Knight, MoveFrom = 4, MoveTo = 21
        // Would be Nf6
        public static string MoveToDescriptiveNotation(Move move)
        {
            // Castling must be hard coded
            if (Piece.Type(move.Piece) == Piece.King && Math.Abs((move.MoveFrom % 8) - (move.MoveTo % 8)) > 1) // If move is a castle
            {
                if (move.MoveTo == 2) // BQS
                {
                    return "O-O-O";
                }
                else if (move.MoveTo == 6) // BKS
                {
                    return "O-O";
                }
                else if (move.MoveTo == 58) // WQS
                {
                    return "O-O-O";
                }
                else if (move.MoveTo == 62) // WKS
                {
                    return "O-O";
                }
            }

            // Set up strings
            string DescriptiveNotation = "";
            string pieceDesc = "";
            string capture = "";
            string check = "";

            if (Piece.Type(move.Piece) == Piece.Knight) // As king and knight both produce K, get 'N' for knight
            {
                pieceDesc = "N";
            }
            else if(Piece.Type(move.Piece) == Piece.Pawn)
            {
                pieceDesc = "";
            }
            else
            {
                // Convert int to piece name (Black Bishop), split and get second word (Bishop), get first letter (B)
                // No need to convert to lower/upper for white or black as descriptive notation is unlike FEN
                pieceDesc = Piece.PieceToFullName(move.Piece).Split(' ')[1][0].ToString();
            }

            // Convert board square into a-h 1-8 notation
            int row = move.MoveTo / 8;
            int col = move.MoveTo % 8;

            // Convert to ASCII then add 97 to get letter
            string letter = (Convert.ToChar(col + 97)).ToString();
            // Minus from 8 as row is inverted
            string number = (8 - row).ToString();

            if(GameControl.Board[move.MoveTo].PieceOnSquare != 0) // Move is a capture
            {
                int moveFromCol = move.MoveFrom % 8;
                string moveFromLetter="";

                if (pieceDesc != "q" && pieceDesc != "k") // Usually only one king or queen so the square moved from is not neccassary
                {
                    // Convert int col to letter, make ascii thena dd 65 for letter, the convert to string to add to piece desc
                    moveFromLetter = char.ToLower(Convert.ToChar(moveFromCol + 65)).ToString();
                }

                capture = "x"; // Set capture from nothing to the x
                pieceDesc = pieceDesc+moveFromLetter; // Update the piece description to include the capture
            }

            if(GameControl.KingInCheck == true)
            {
                check = "+";
            }

            DescriptiveNotation += pieceDesc + capture + letter + number + check; // Add all together to get final move description

            return DescriptiveNotation;
        }

        public static void Pop() // Used for undoing moves
        {
            Move move = moveStack[moveStack.Count];
            PoppedMoves.Add(move);
            moveStack.RemoveAt(moveStack.Count);
        }

        public static void Push(Move move) // Add move to move stack
        {
            moveStack.Add(move);
            GameWindow.UpdateMoveStackDisplay(MoveToDescriptiveNotation(move));
        }
    }
}
