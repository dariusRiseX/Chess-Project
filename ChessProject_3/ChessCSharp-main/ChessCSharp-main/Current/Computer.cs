using System;
using System.Linq;
using System.Collections.Generic;

namespace Chess
{
    class Computer
    {
        public Computer()
        {
        }
        public static int Depth = 3; // Change this to make computer more powerful, sacrificing speed
        // anything higher than 6 is too high to be usable (at least for my PC)
        public static bool MovePutKingInCheck = false;

        // Evaluates given board position and returns evaulation as an Interger
        // Takes into account the pieces left and their values
        // The kings distances apart - This relies on the endgame weight
        // The distance the king is from the edgde of the board - again relies on the endgame weight
        public static int EvaluatePos()
        {
            int Evaluation = 0; // Keeps track of current evaluation
            int EndGameWeight = GameControl.GetEndGameWeight();

            // Moves where king is put in check are often good
            if (MovePutKingInCheck == true)
            {
                Evaluation += 150 * GameControl.GetEndGameWeight() > 4 ? 3 : 1; // If endgame weight is greater than 4, double, else leave as is
            }

            // Get locations that are usable for kings
            int OpposingKingSquare = PieceLocator.GetLocationsOf(GameControl.OpposingSide() | Piece.King)[0];
            int FriendlyKingSquare = PieceLocator.GetLocationsOf(GameControl.computerSide | Piece.King)[0];

            int OpposingKingRow = OpposingKingSquare % 8;
            int OpposingKingCol = OpposingKingSquare / 8;

            int FriendlyKingRow = FriendlyKingSquare % 8;
            int FriendlyKingCol = FriendlyKingSquare / 8;

            // Move king towards opponents king in endgamees
            int DistanceBetweenKingsCols = Math.Abs(FriendlyKingCol - OpposingKingCol);
            int DistanceBetweenKingsRows = Math.Abs(FriendlyKingRow - OpposingKingRow);
            int DistanceBetweenKings = DistanceBetweenKingsCols + DistanceBetweenKingsRows;

            Evaluation += 14 - (DistanceBetweenKings * EndGameWeight);

            // Keep opponents king out of centre and friendly king in centre
            int OpposingKingsDistanceFromCentreRow = Math.Max(3 - OpposingKingRow, OpposingKingRow - 4);
            int OpposingKingsDistanceFromCentreCol = Math.Max(3 - OpposingKingCol, OpposingKingCol - 4);
            int OpponsingKingsDistanceFromCentre = OpposingKingsDistanceFromCentreRow + OpposingKingsDistanceFromCentreCol;

            Evaluation += (OpponsingKingsDistanceFromCentre * EndGameWeight);

            // Get the value of each piece on the board and add to eval
            Evaluation += PieceLocator.GetPosEval();

            return Evaluation;
        }

        // Generates all moves available in position
        // Loops through and gets the eval to certain depth of each move
        // Compares move to bets move and updates if its better
        // Keeps track of how quick a move was generated
        public static Move GenerateMove()
        {
            // Start timer for checking how long a move takes to generate
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Generating Computer Move");

            // Set move from and to to negative values to check later
            Move moveToPlay = new Move { Piece = 0, MoveFrom = -1, MoveTo = -1 };
            List<Move> AvailableMoves = RuleBook.GenerateLegalMoves();

            // If the computer has no moves, return the negative values, this will mean the gameControl will check for stale/check mate
            if (AvailableMoves.Count == 0) 
            {
                return moveToPlay;
            }

            // Opening book section
            // 4 moves a side (GameControl.Moves < 8)
            // If move found, return move, if not, return move to play with negative values
            // So computer knows to generate its own
            if (GameControl.Moves < 8)
            {
                Console.WriteLine("Move within first four, getting opening move");
                moveToPlay = OpeningBook.GetMove(AvailableMoves);

                if (moveToPlay.MoveFrom != -1)
                {
                    Console.WriteLine("Move found, not calculating own move\n");
                    return moveToPlay;
                }
            }

            // Anything will be better than starting best eval value, even being checkmated - this means the computer will always return a move
            int BestEval = -999999999;

            // Sorts moves from best to worst to help with alpha beta pruning
            AvailableMoves = sortMoves(AvailableMoves);

            // For every move in the sorted list
            // Make the test move
            // Multiply by minus one as move that is good for opponent is bad for computer
            // If the current eval is better than the best eval, make the move to play the current move
            // Undo test move to evaluate next one
            foreach (Move move in AvailableMoves.ToArray())
            {
                // Console.WriteLine($"Move To Make: {Piece.PieceToFullName(move.Piece)} moves from {move.MoveFrom} to {move.MoveTo}, move score: {move.MoveScore}"); 

                GameControl.makeTestMove(move);
                int moveEval = (GetMoveEvaluationToDepthOf(Depth, -100000000, 100000000)*-1) + LocationInscentives.GetLocationInscentiveFor(move.Piece, move.MoveTo)*-1;

                Console.WriteLine($"Best eval for move {Piece.PieceToFullName(move.Piece)} moves from {move.MoveFrom} to {move.MoveTo}: {moveEval} "); 
                if (moveEval > BestEval)
                {
                    BestEval = moveEval;
                    moveToPlay = move;
                }

                GameControl.unmakeTestMove(move);
            }

            // Timer stuff
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            GameWindow.BlackTimeLeft -= Convert.ToInt32(elapsedMs) / 1000;

            Console.WriteLine($"\nTime For Computer to generate move: {elapsedMs}ms");
            Console.WriteLine($"Minusing {Convert.ToInt32(elapsedMs) / 1000} seconds from computer clock. New time: {GameWindow.BlackTimeLeft}");
            Console.WriteLine($"Returning move {Piece.PieceToFullName(moveToPlay.Piece)} moves from {moveToPlay.MoveFrom} to {moveToPlay.MoveTo}, eval: {BestEval}");

            return moveToPlay;
        }


        // Generates opponents responses in given position
        // Makes that move
        // Gets best response for computer, this repeats until min depth is reached
        // Sorts moves from worst to best to help AB pruning
        // Returns the position evaluation at end of serach
        public static int GetMoveEvaluationToDepthOf(int depth, int alpha, int beta)
        {
            if(depth == 0) // If end of move tree reached, return the current evalutaion
            {
                return EvaluatePos();
            }

            List<Move> responses = RuleBook.GenerateLegalMoves(); 

            // This is saved as when the program checks if the move is good,
            // it will need to check if the king is in check and the Rule book check checker can be changed during move generation
            MovePutKingInCheck = false;
            if(RuleBook.KingInCheck == true)
            {
                MovePutKingInCheck = true;
            }

            if(responses.Count == 0)
            {
                if(MovePutKingInCheck == true)
                {
                    return -10000 * depth; // multiplying by depth means quicker checkmates are favoured
                }
                return 0;
            }

            responses = sortMoves(responses); // Get moves from best to worst

            foreach(Move response in responses.ToArray()) // Change to array otherwise "List changed during execution" error will occur
            {
                GameControl.makeTestMove(response); // Make test response
                int MoveEval = (GetMoveEvaluationToDepthOf(depth - 1, -beta, -alpha) * -1); // Recursiveley call function
                GameControl.unmakeTestMove(response); // Unmake response at the end of recursion
                if(MoveEval >= beta) // If move eval is better than last best move
                {
                    return beta; // Update beta
                }
                alpha = Math.Max(alpha, MoveEval); // If not, get the best out of prev worst and current eval
            }

            return alpha; // Fall through
        }


        // Assign each move a value of whether we think it will be good or not
        // QuickSort these values to minimise data movement, therefore increasing speed of move generation
        // Reverse list as quick sort gives worst to best
        public static List<Move> sortMoves(List<Move> moveList)
        {
            // assign move score guesses
            foreach (Move move in moveList)
            {
                int moveScore = 0;
                int piece = move.Piece;
                int capturedPiece = GameControl.Board[move.MoveTo].PieceOnSquare;

                // Console.WriteLine($"Assigning Move Score To {Piece.PieceToFullName(move.Piece)} moves from {move.MoveFrom} to {move.MoveTo}"); 

                if (capturedPiece != 0)
                {
                    moveScore = Piece.Value(capturedPiece) - Piece.Value(piece); // Capturing piece of lower value is good other way is bad
                }

                if(MovePutKingInCheck == true)
                {
                    moveScore += 300; // Checks are often good as they can lead to forks
                }

                if (RuleBook.SquaresToMoveToThatStopCheck.Contains(move.MoveTo))
                {
                    moveScore += 100; // Blocking check is often good (as opposed to moving the king)
                }

                if (RuleBook.EnemyAttacks.Contains(move.MoveTo))
                {
                    moveScore -= 250; // Moving to position where enemy attacks is often bad
                }

                if (Piece.Type(piece) == Piece.Pawn && Piece.Type(RuleBook.CheckPromotion(move)) == Piece.Queen)
                {
                    moveScore += 500; // Trying to promote is good
                }

                move.MoveScore = moveScore;
            }

            moveList = QuickSort(moveList.ToArray(), 0, moveList.Count - 1).ToList();
            moveList.Reverse();

            return moveList;
        }

        // Quick sort the unsorted list
        public static Move[] QuickSort(Move[] unsortedList, int leftStartPointer, int rightStartPointer)
        {
            int leftPointer = leftStartPointer; // Auto set to 0
            int rightPointer = rightStartPointer; // Auto set to size of list (-1 as list starts at 0 not 1)
            int pivotNumber = unsortedList[leftPointer].MoveScore; // Get value at start of list

            while (leftPointer <= rightPointer) 
            {
                while (unsortedList[leftPointer].MoveScore < pivotNumber) // As pivot number is actually the score, we compare to this, not index of pivot number
                {
                    leftPointer++; // Move left pointer "right"
                }
                while (unsortedList[rightPointer].MoveScore > pivotNumber) // As pivot number is actually the score, we compare to this, not index of pivot number
                {
                    rightPointer--; // Move right pointer "left"
                }

                if (leftPointer <= rightPointer)
                {
                    // Swap the actual moves
                    // Not the move score
                    Move moveHolder = unsortedList[leftPointer]; // Make Move holder to swap
                    unsortedList[leftPointer] = unsortedList[rightPointer]; // Swap moves
                    unsortedList[rightPointer] = moveHolder;
                    leftPointer++; // Move left pointer "right"
                    rightPointer--; // And right pointer "left"
                }
            }

            if (leftStartPointer < rightPointer) // Check if left start has crossed positon of the right pointer
            {
                QuickSort(unsortedList, leftStartPointer, rightPointer); // if so search left of pivot
            }
            if (leftPointer < rightStartPointer) // Check if right start has crossed positon of the left pointer
            {
                QuickSort(unsortedList, leftPointer, rightStartPointer); // if so search right of pivot
            }

            Move[] sortedList = unsortedList; // Just do this so it says its returing sorted list to avoid confusion

            return sortedList;

        }
    }
}

// ====== SIMPLE POSITION EVAL ======
// make every test move
// check eval
// save eval
// choose move with highest eval

// ====== MINMAX ALGO ======
// make a recursive tree of positions
// explore tree to given depth
// evaluate position
// choose move that gives oposition worse set of moves

// ====== MOVE GUESSES ======

// ====== PIECE PREFERED LOCATION MAPS TO EVAL ======
// create heat maps of where pieces should try to go
// make computer prefer those moves
// rooks on 7th rank, bishops in corners ect

// ====== ALPHA BETA PRUNING ======
// optimization of minmax
// disregard branches when they lead to worse position that prev one
// same result as minmax
// just faster

// ====== GM OPENING BOOK =====
// give computer a dictionary of first 4moves from gm games
// this allows it to get a good opening position
// is this cheating?

// ====== OTHER STUFF =====
// push king to edge at endgame
// prefer to give checks
// prefer rook over double bishops despite point difference (for easy checkmate)
// move order pruning (same position can be reached by different move order)
