using System;
using System.Collections.Generic;

namespace Chess
{
    class OpeningBook
    {
        // openings.txt contains the first 4 moves from around 2000 games from garry kasparov, obtained from pgnmentor.com
        public static string[] MoveList = System.IO.File.ReadAllLines("Openings.txt");
        public static List<string> LinesAvailable = new List<string>();
        public static string MovesPlayed = "";

        public static List<Move> AvailableMoves = new List<Move>();

        // 'Lines' refer to the moves that can be made, can be confusing as lines in the file are called the same thing
        // GetMove searches for the moves played string within the openings.txt file
        // If it finds a match (or more)
        // It will chose a random match to play
        // Then convert the next moves descriptive notation into a move to play
        public static Move GetMove(List<Move> availableMoves)
        {
            AvailableMoves = availableMoves;

            Move moveToPlay = new Move { Piece = 0, MoveFrom = -1, MoveTo = -1 };

            foreach(string line in MoveList)
            {
                // If the line starts with the moves played, we can consider it as a possible line
                if (line.StartsWith(MovesPlayed))
                {
                    LinesAvailable.Add(line);
                }
            }

            Random rand = new Random();
            try
            {
                // Get random line
                int randomLineNumber = rand.Next(0, LinesAvailable.Count - 1);
                string lineToPlay = LinesAvailable[randomLineNumber];

                Console.WriteLine($"Line found to play {lineToPlay}");

                // Get the next move from the line by removing the moves played and splitting it into an array [0] gets first
                string moveAsDescNote = lineToPlay.Replace(MovesPlayed, "").Split(' ')[0];
                moveToPlay = ChangeDesriptiveNotationToMove(moveAsDescNote);
            }
            catch // If no lines are found, return move with negative numbers, this is then checked to see if a move needs to be generated
            {
                return moveToPlay;
            }

            return moveToPlay;
        }

        // Gets a move class from descriptive notation given (descNote)
        // Gets piece
        // Then moveTo
        // Move from cant be determined so a check against legal moves is used
        public static Move ChangeDesriptiveNotationToMove(string descNote)
        {
            Move move = new Move { Piece = 0, MoveFrom = -1, MoveTo = -1 };

            char[] descNoteAsCharArray = descNote.ToCharArray(); // Split into char array so it is easier to manipulate

            // Get Piece
            int piece = 0;

            // If no piece specified eg "e4", piece is a pawn
            if (descNoteAsCharArray[0] == Convert.ToInt32(descNoteAsCharArray[descNoteAsCharArray.Length - 2]))
            {
                piece = GameControl.computerSide | Piece.Pawn;
            }
            else
            {
                char pieceChar = Char.ToLower(descNoteAsCharArray[0]);

                switch (pieceChar) // return piece depending on letter
                {
                    case 'n':
                        piece = GameControl.computerSide | Piece.Knight;
                        break;
                    case 'b':
                        piece = GameControl.computerSide | Piece.Bishop;
                        break;
                    case 'r':
                        piece = GameControl.computerSide | Piece.Rook;
                        break;
                    case 'q':
                        piece = GameControl.computerSide | Piece.Queen;
                        break;
                    case 'k':
                        piece = GameControl.computerSide | Piece.King;
                        break;
                }
            }

            // Get MoveTo
            // Last char will be row number -1 to get (0-7)
            int row = int.Parse(descNoteAsCharArray[descNoteAsCharArray.Length - 1].ToString()) - 1;
            // Second last will be col letter, convert to ascii, minus 97 (lower a) to get col number (0-7)
            int col = Convert.ToInt32(descNoteAsCharArray[descNoteAsCharArray.Length - 2]) - 97;

            int location = (56 - (8 * row)) + col; // Minus from 56 as rows are inverted

            // Set Move
            // Check if move is in available moves as MoveFrom is impossible to get from the descriptive notaion
            foreach (Move testMove in AvailableMoves)
            {
                if (testMove.Piece == piece && testMove.MoveTo == location)
                {
                    move = testMove;
                }
            }

            Console.WriteLine($"piece {Piece.PieceToFullName(move.Piece)} moves from {move.MoveFrom} to {move.MoveTo}");

            return move;
        }

        // Get desc notation from move and add to moves played
        public static void AddToMoveString(Move move)
        {
            string MoveAsDescNotation = MoveStack.MoveToDescriptiveNotation(move);

            MovesPlayed += MoveAsDescNotation + " ";
        }
    }
}
