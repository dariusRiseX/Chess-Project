using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
    public class GameControl
    {
        public static BoardSquare[] Board;

        public static bool gameEnded = false;
        public static int sideToMove = 8;
        public static int computerSide = 16;
        public static int Moves = 0; // keep track of how many moves have been played
        public static bool KingInCheck = false;
        public static bool SinglePlayer = false; // checked to see if playing against computer of friend
        public static int NoGames = 0;

        public static Timer WhiteTimer = new Timer() { Interval = 1000 }; // Set interval to 1000 so its in secs
        public static Timer BlackTimer = new Timer() { Interval = 1000 };

        public GameControl() 
        {
            Board = new BoardSquare[64];
        }

        // Resets all of the variables to default values for resetting game
        public static void SetOriginalVariables(bool single)
        {
            if (single == true)
            {
                SinglePlayer = true;
            }
            else
            {
                SinglePlayer = false;
            }

            BlackTimer.Stop();
            WhiteTimer.Stop();

            sideToMove = FenStringUtility.GetSideToMoveFirst();
            Moves = 0;
            KingInCheck = false;
            gameEnded = false;

            RuleBook.BlackKingSide = true;
            RuleBook.WhiteKingSide = true;
            RuleBook.BlackQueenSide = true;
            RuleBook.WhiteQueenSide = true;

            RuleBook.ClearLists();

            NoGames += 1;
        }

        // Ends the game by removing all legal moves and displaying the winner.
        // Can be used to check if game has ended or not
        // Or to end the game with a given winning side
        // A check for a negative value checks for if the game has ended or not
        // And a check for a 0 checks for stalemate
        public static bool EndGame(int winningSide)
        {
            if (gameEnded == false && winningSide != -1)
            {
                GameWindow.currentLegalMoves.Clear();
                if (winningSide != 0) 
                {
                    Console.WriteLine($"{Piece.ColourNameFromPieceBin(winningSide | Piece.King)} Has Won The Game");
                    GameWindow.WinnerDisplay.Text = $"{Piece.ColourNameFromColourBin(winningSide)} Wins!";
                }
                else
                {
                    Console.WriteLine("Stalemate");
                    GameWindow.WinnerDisplay.Text = "Stalemate!";
                }
                // stop timers
                BlackTimer.Stop();
                WhiteTimer.Stop();

                GameWindow.WinnerDisplay.Visible = true;

                // play gameover sound
                System.IO.Stream stream = Properties.Resources.GameOver;
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(stream);
                player.Play();

                return true; // fallthrough
            }
            else if (gameEnded == false && winningSide == 0) { return false; } // return that game HASNT ended
            return true; // return that game HAS ended

        }

        // Returns side to move
        public static int CheckSideToMove()
        {
            return sideToMove;
        } 
        // Returns opposing side to move
        public static int OpposingSide()
        {
            return sideToMove == 8 ? 16 : 8;
        }
        // Swap the side to move
        public static void ChangeSideToMove()
        {
            sideToMove = sideToMove == 8 ? 16 : 8;
        }

        // Calculate how far through the game it is and updates variable
        // Checks how many moves have been played
        // Check for how many piece ares left
        // checks how many pawns are left (divide by four as it will skew it too much if not)
        public static int GetEndGameWeight()
        {
            int weighting = 0;

            // Check how many moves have been played
            if(Moves > 30)
            {
                weighting += 3;
            }
            else if(Moves > 20)
            {
                weighting += 2;
            }
            else if (Moves > 10)
            {
                weighting += 1;
            }

            // Check how many pieces are left
            if(PieceLocator.BishopLocations.Count == 0)
            {
                weighting += 1;
            }
            if (PieceLocator.KnightLocations.Count == 0)
            {
                weighting += 1;
            }
            if (PieceLocator.RookLocations.Count == 0)
            {
                weighting += 2;
            }

            weighting += (16 - PieceLocator.PawnLocations.Count) / 4;

            return weighting;
        }

        // Makes a given move
        // First check if MoveTo is -1 to see if computer has returned its out of moves
        // Then check if its checkmate Or stalemate
        // Update move stack and add move to moves string
        // Move number is incremented
        // Timers started / stopped
        // New legal moves are generated
        // Board background colours are reset
        // Computer move checked
        public static void Move(Move move)
        {
            if (move.MoveTo == -1)
            {
                if (RuleBook.KingInCheck == true)
                {
                    EndGame(8);
                    return;
                }
                EndGame(-1);
                return;
            }

            MoveStack.Push(move); // Display move in side move stack

            if (Moves < 9) // For first 8 moves (4 each), computer uses opening book
            {
                OpeningBook.AddToMoveString(move);
            }

            // Sound Stuff
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();

            // Check for capture
            if (Board[move.MoveTo].PieceOnSquare != 0)
            {
                System.IO.Stream stream = Properties.Resources.Capture;
                player = new System.Media.SoundPlayer(stream);
            }
            // Normal Move
            else
            {
                System.IO.Stream stream = Properties.Resources.Move;
                player = new System.Media.SoundPlayer(stream);
            }

            // Keep track of castle to see if the sound needs to swap to castle sound instead of move sound
            bool Castled = false;

            if (Piece.Type(move.Piece) == Piece.King && Math.Abs((move.MoveFrom % 8) - (move.MoveTo % 8)) > 1) // Player is trying to castle
            {
                Castled = true;
                Castle(move); // Castle instead of move
            }
            else {
                if (Board[move.MoveTo].PieceOnSquare != 0) // Check for capture
                {
                    RemovePiece(Board[move.MoveTo].PieceOnSquare, move.MoveTo); // Remove piece first only if there is a capture
                }
                if (Piece.Type(move.Piece) == Piece.Pawn)
                {
                    move.Piece = RuleBook.CheckPromotion(move); // Check for a promotion, if so, make the piece moving into a queen
                }
                AddPiece(move.Piece, move.MoveTo); // Add piece to board
            }

            Moves += 1; // Increment moves
            StartStopTimers();
            ChangeSideToMove();

            RuleBook.CheckIfCastlingRightsHaveChanged(move);
            RuleBook.SquaresToMoveToThatStopCheck.Clear();
            GameWindow.SetLegalMoves(); // Generate legal moves for next player

            // Big debug statment
            Console.WriteLine($"Side to move: {Piece.ColourNameFromColourBin(sideToMove)}, Total moves: {Moves}, {Piece.PieceToFullName(sideToMove | Piece.King)} is in check: {RuleBook.KingInCheck}, Current Eval:{PieceLocator.GetPosEval()} \n");

            // Set sound as check instead of move if there is check
            // This is not set with the other sounds as it needs to be checked after the move has been made
            if (RuleBook.KingInCheck == true)
            {
                System.IO.Stream stream = Properties.Resources.Check;
                player = new System.Media.SoundPlayer(stream);
            }

            // Only play sound if move wasnt a castle as the castle sound is played in the castle function
            if (Castled == false)
            {
                player.Play(); // Play required sound
            }

            GameWindow.resetColours(); // Reset the colours so the available moves dont show
            Board[move.MoveFrom].BackColor = ColorTranslator.FromHtml("#dbdb42"); // Show where player moved from
            Board[move.MoveTo].BackColor = ColorTranslator.FromHtml("#e8dd10"); // And to

            // Refresh the required squares so it shows when computer is calulating its move
            Board[move.MoveFrom].Refresh();
            Board[move.MoveTo].Refresh();

            // Check if computer needs to generate a move
            if (SinglePlayer == true && sideToMove == computerSide)
            {
                Move ComputerMove = Computer.GenerateMove(); // get move
                try { RemovePiece(ComputerMove.Piece, ComputerMove.MoveFrom); } // If game needs to be ended, the move will return -1 as move from so it will crash before the end game is checked
                catch { Move(ComputerMove); } // To prevent this, catch the error and pass the move anyway so game can be ended
                Move(ComputerMove); // If the computer isnt out of moves, play the move given
            }
        }

        public static Stack<int> CapturedPieceTrackerStack = new Stack<int>(); // Stack for checking which pieces to unmake move with with generating computer move
        public static void makeTestMove(Move move)
        {
            // Console.WriteLine($"Making test move {Piece.PieceToFullName(move.Piece)} moves from {move.MoveFrom} to {move.MoveTo}"); ""
            ChangeSideToMove(); // Change side to move as when computer is generating moves it will need to generate responses

            int piece = move.Piece;

            // Check if move is castle
            if (Piece.Type(move.Piece) == Piece.King && Math.Abs((move.MoveFrom % 8) - (move.MoveTo % 8)) > 1)
            {
                Board[move.MoveTo].PieceOnSquare = move.Piece; // add king to moveTo
                PieceLocator.AddToList(move.Piece, move.MoveTo);

                Board[move.MoveFrom].PieceOnSquare = 0; // remove king from moveFrom
                PieceLocator.RemoveFromList(move.Piece, move.MoveFrom);

                // Check which side is being castled at and castle accordingly
                if (move.MoveTo == 2) // BQS
                {
                    makeTestCastle(move.MoveTo + 1, 0, 16);
                }
                else if (move.MoveTo == 6) // BKS
                {
                    makeTestCastle(move.MoveTo - 1, 7, 16);
                }
                else if (move.MoveTo == 58) // WQS
                {
                    makeTestCastle(move.MoveTo + 1, 56, 8);
                }
                else if (move.MoveTo == 62) // WKS
                {
                    makeTestCastle(move.MoveTo - 1, 63, 8);
                }
            }
            // Move isnt a castle move
            else
            {
                Board[move.MoveFrom].PieceOnSquare = 0; // Remove piece from old location
                PieceLocator.RemoveFromList(move.Piece, move.MoveFrom);

                CapturedPieceTrackerStack.Push(Board[move.MoveTo].PieceOnSquare); // Save captured piece
                PieceLocator.RemoveFromList(Board[move.MoveTo].PieceOnSquare, move.MoveTo);

                if (Piece.Type(piece) == Piece.Pawn) // check for promotion
                {
                    piece = RuleBook.CheckPromotion(move);
                }

                Board[move.MoveTo].PieceOnSquare = piece; // Add piece to new location
                PieceLocator.AddToList(piece, move.MoveTo);
            }

            // Below is commented code that will show the computer trying out different moves if uncomented

            //Board[move.MoveTo].Image = Piece.PieceToImage(move.Piece);
            //Board[move.MoveFrom].Image = null;
            //Board[move.MoveTo].Refresh();
            //Board[move.MoveFrom].Refresh();
            //System.Threading.Thread.Sleep(100);
        }
        public static void unmakeTestMove(Move move)
        {
            // Console.WriteLine($"UnMaking test move {Piece.PieceToFullName(move.Piece)} moves from {move.MoveFrom} to {move.MoveTo}"); ""
            ChangeSideToMove(); // Change side back

            int piece = move.Piece;

            // Check if move is castle
            if (Piece.Type(piece) == Piece.King && Math.Abs((move.MoveFrom % 8) - (move.MoveTo % 8)) > 1)
            {
                Board[move.MoveTo].PieceOnSquare = 0; // Remove king to moveTo
                PieceLocator.RemoveFromList(move.Piece, move.MoveTo);

                Board[move.MoveFrom].PieceOnSquare = move.Piece; // Add king to moveFrom
                PieceLocator.AddToList(move.Piece, move.MoveFrom);

                // Check which side has castled and undo accordingly
                if (move.MoveTo == 2) // BQS
                {
                    unmakeTestCastle(move.MoveTo + 1, 0, 16);
                }
                else if (move.MoveTo == 6) // BKS
                {
                    unmakeTestCastle(move.MoveTo - 1, 7, 16);
                }
                else if (move.MoveTo == 58) // WQS
                {
                    unmakeTestCastle(move.MoveTo + 1, 56, 8);
                }
                else if (move.MoveTo == 62) // WKS
                {
                    unmakeTestCastle(move.MoveTo - 1, 63, 8);
                }

                // Below is commented code that will show the computer trying out different moves if uncommented

                //Board[move.MoveFrom].Image = Piece.PieceToImage(move.Piece);
                //Board[move.MoveTo].Image = null;
            }

            // Normal move was played
            else
            {
                Board[move.MoveFrom].PieceOnSquare = piece; // Add piece to origonal location
                PieceLocator.AddToList(piece, move.MoveFrom);

                if(Piece.Type(piece) == Piece.Pawn) // Check for promotion
                {
                    if(piece != RuleBook.CheckPromotion(move))
                    {
                        piece = Piece.Colour(piece) | Piece.Queen; // Ciece is set to queen to make sure to remove queen from promotion square instead of trying to remove pawn (this lead to ghost queens)
                    }
                }

                PieceLocator.RemoveFromList(piece, move.MoveTo); // Remove piece

                // Console.WriteLine($"Captured piece at top of stack: {Piece.PieceToFullName(CapturedPieceTrackerStack.Peek())}");

                // Put captured piece back
                int capturedPiece = CapturedPieceTrackerStack.Pop(); // Get the piece at the top of the stack as this will be the piece captured most recently at the most recent depth

                Board[move.MoveTo].PieceOnSquare = capturedPiece; // Put piece back
                PieceLocator.AddToList(capturedPiece, move.MoveTo);

                // Below is commented code that will show the computer trying out different moves if uncommented

                //Board[move.MoveFrom].Image = Piece.PieceToImage(move.Piece);
                //Board[move.MoveTo].Image = Piece.PieceToImage(capturedPiece);
            }

            // Below is commented code that will show the computer trying out different moves if uncommented

            //Board[move.MoveTo].Refresh();
            //Board[move.MoveFrom].Refresh();
            //System.Threading.Thread.Sleep(100);

        }
        public static void makeTestCastle(int kingOffset, int RookCorner, int colour)
        {
            // Most of castling is done in the make / unmake test move functions, however, this saved alot of repetative code

            int rook = colour | Piece.Rook; // Get rook piece corresponding to colour

            Board[RookCorner].PieceOnSquare = 0; // Remove rook from corner
            PieceLocator.RemoveFromList(rook, RookCorner);

            Board[kingOffset].PieceOnSquare = rook; // Add rook next to king
            PieceLocator.AddToList(rook, kingOffset);

            // Below is commented code that will show the computer trying out different moves if uncommented

            //Board[RookCorner].Image = null;
            //Board[kingOffset].Image = Piece.PieceToImage(rook);
            //Board[RookCorner].Refresh();
            //Board[kingOffset].Refresh();
        }
        public static void unmakeTestCastle(int kingOffset, int RookCorner, int colour)
        {
            // Most of castling is done in the make / unmake test move functions, however, this saved alot of repetative code

            int rook = colour | Piece.Rook; // Get rook piece corresponding to colour

            Board[RookCorner].PieceOnSquare = rook; // Add rook back to corner
            PieceLocator.AddToList(rook, RookCorner);

            Board[kingOffset].PieceOnSquare = 0; // Remove rook next to king
            PieceLocator.RemoveFromList(rook, kingOffset);

            // Below is commented code that will show the computer trying out different moves if uncommented

            //Board[RookCorner].Image = Piece.PieceToImage(rook);
            //Board[kingOffset].Image = null;
            //Board[RookCorner].Refresh();
            //Board[kingOffset].Refresh();
        }

        // Plays castle sound
        // Adds king to new position
        // Calls add/remove piece with corresponding arguments depending on side castled to
        public static void Castle(Move move)
        {
            Console.WriteLine($"castling {move.MoveTo}");

            // play castle sound
            System.IO.Stream stream = Properties.Resources.Castle;
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(stream);
            player.Play();

            AddPiece(move.Piece, move.MoveTo); // Add king to position

            // Update Castling rights
            // Remove rook in corner
            // Add rook next to king
            if(move.MoveTo == 2){
                RuleBook.BlackQueenSide = false; 
                RemovePiece(Piece.Black | Piece.Rook, 0);
                AddPiece(Piece.Black | Piece.Rook, move.MoveTo + 1);
            }
            else if (move.MoveTo == 6){
                RuleBook.BlackKingSide = false;
                RemovePiece(Piece.Black | Piece.Rook, 7);
                AddPiece(Piece.Black | Piece.Rook, move.MoveTo - 1);
            }
            else if (move.MoveTo == 58){
                RuleBook.WhiteQueenSide = false; 
                RemovePiece(Piece.White | Piece.Rook, 56); 
                AddPiece(Piece.White | Piece.Rook, move.MoveTo + 1);
            }
            else if (move.MoveTo == 62)
            {
                RuleBook.WhiteKingSide = false; 
                RemovePiece(Piece.White | Piece.Rook, 63);
                AddPiece(Piece.White | Piece.Rook, move.MoveTo - 1);
            }

            // Refresh locations so move is obvious when computer is generating move
            Board[0].Refresh();
            Board[7].Refresh();
            Board[56].Refresh();
            Board[63].Refresh();
            Board[move.MoveTo - 1].Refresh();
            Board[move.MoveTo + 1].Refresh();
        }

        // Stops and starts timers depending on who needs to move
        public static void StartStopTimers()
        {
            if (sideToMove == 8) // Check side
            {
                BlackTimer.Stop(); // Update timers accordingly
                WhiteTimer.Start();
            }
            else if (sideToMove == 16)
            {
                WhiteTimer.Stop();
                BlackTimer.Start();
            }
        }

        // Adds to piece board
        // Updates image accordingly
        // Adds piece to locator lists
        // Debug
        public static void AddPiece(int piece, int location)
        {
            Board[location].PieceOnSquare = piece;
            Board[location].Image = Piece.PieceToImage(piece);
            PieceLocator.AddToList(piece, location);

            Console.WriteLine($"{Piece.PieceToFullName(piece)} added at square {location}");
            Console.WriteLine(FenStringUtility.GetFenStringFromCurrentBoard(Board) + "\n");
        }

        // Sets piece to nothing on given location
        // Sets image to nothing aswell
        // Removes piece from locator lists
        public static void RemovePiece(int piece, int location)
        {
            Board[location].PieceOnSquare = 0;
            Board[location].Image = null;
            PieceLocator.RemoveFromList(piece, location);

            Console.Write($"{Piece.PieceToFullName(piece)} removed at square {location}\n");
            //FenStringUtility.GetFenStringFromCurrentBoard(Board.PieceBoard);
        }

    }
}
