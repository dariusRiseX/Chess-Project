using System.Collections.Generic;


namespace Chess
{
    public class FenStringUtility
    {
        // Idea from Sebastian Lague

        // Dictionary for making pieces easier to identify later
        public static Dictionary<char, int> pieceTypeFromSymbol = new Dictionary<char, int>()
        {
            ['k'] = Piece.King,
            ['p'] = Piece.Pawn,
            ['n'] = Piece.Knight,
            ['b'] = Piece.Bishop,
            ['r'] = Piece.Rook,
            ['q'] = Piece.Queen
        };


        // FEN STRING INFO SO I DONT FORGET
        // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR - board layout
        // w - white to move (active colour)
        // KQkq - castling availability
        // En passant
        // Halfmove clock - number of half move since the last capture of pawn push
        // Fullmove number - number of full moves in a game (black and white move) starts after blacks move

        public static string InputedPostion = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public const string StartingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"; // FEN string for starting position of classic chess
        // public const string StartingPosition = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w - - 0 1"; // FEN for testing position

        // Simple function that splits fen and gets side to move first
        // Get second part of FEN, check colour and update colour int accordingly
        public static int GetSideToMoveFirst()
        {
            int colour = StartingPosition.Split(' ')[1] == "b" ? 16 : 8;

            return colour;
        }

        // Loads board from a given fen string
        // Get board info (excludes w KQkq - 0 1) as its split to first half
        public static void LoadBoardFromFenString(string fen)
        {
            string fenSplit = fen.Split(' ')[0];
            int row = 0;
            int col = 0;

            foreach (char chara in fenSplit)
            {
                if (chara == '/') // If new line add to row clear col
                {
                    col = 0; // If not just add to row
                    row+=1; // Move to next row
                }
                else
                {
                    if (char.IsDigit(chara) == true) // Check for digit
                    {
                        col += (int)char.GetNumericValue(chara); // Add digit no to col
                    }
                    else
                    {
                        int pieceColour = (char.IsUpper(chara)) ? Piece.White : Piece.Black; // Check if character is upper or lower for black or white
                        int pieceType = pieceTypeFromSymbol[char.ToLower(chara)]; // Get piece type from character and dictonary

                        // Console.WriteLine($"row {row}, col {col}, pos {(row*8)+col}"); ""
                        int location = (row * 8) + col; // Get location 0 - 63 from row and col
                        GameControl.AddPiece(pieceColour | pieceType, location); // Add piece to location
                        col+=1; // Move to next col
                    }
                }
            }
        }

        // Returns current pos for trouble shooting
        public static string GetFenStringFromCurrentBoard(BoardSquare[] Board)
        {
            string fen = "";
            for (int row = 7; row >= 0; row--) // Loop through rows
            {
                int numEmptycols = 0; 
                for (int col = 0; col < 8; col++) // Loop through cols
                {
                    int i = row * 8 + col; // Get current square number
                    int piece = Board[i].PieceOnSquare; // Get piece on sqaure number
                    if (piece != 0) // If its not empty
                    {
                        if (numEmptycols != 0)
                        {
                            fen += numEmptycols; // Adds the empty col number
                            numEmptycols = 0; // Resets for next loop
                        }
                        // Check what piece is
                        bool isBlack = Piece.IsColour(piece, Piece.Black);
                        int pieceType = Piece.Type(piece);
                        char pieceChar = ' ';

                        switch (pieceType) // Set piecechar to coresponding piece
                        {
                            case Piece.Rook:
                                pieceChar = 'R';
                                break;
                            case Piece.Knight:
                                pieceChar = 'N';
                                break;
                            case Piece.Bishop:
                                pieceChar = 'B';
                                break;
                            case Piece.Queen:
                                pieceChar = 'Q';
                                break;
                            case Piece.King:
                                pieceChar = 'K';
                                break;
                            case Piece.Pawn:
                                pieceChar = 'P';
                                break;
                        }

                        fen += (isBlack) ? pieceChar.ToString().ToLower() : pieceChar.ToString(); // Change to black or white (lower or keep upper)
                    }
                    else
                    {
                        numEmptycols+=1;
                    }

                }
                if (numEmptycols != 0) // If there are empty cols, add how many there are
                {
                    fen += numEmptycols;
                }
                if (row != 0) // At the end of row, add dash
                {
                    fen += '/';
                }
            }

            return fen;
        }
        
    }
}



