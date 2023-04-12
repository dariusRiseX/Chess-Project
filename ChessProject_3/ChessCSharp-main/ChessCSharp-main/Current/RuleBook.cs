using System;
using System.Collections.Generic;
using System.Drawing;

namespace Chess
{
    class RuleBook
    {
        // -8 up, 8 down, -1 left, 1 right, -7 up diag right, 7 down diag left, -9 up diag left, 9 down diag right
        // Split into up, bottom, left, right directions to correspond with distance from edges
        // Pawn directions stored in generate pawn moves function as they are small arrays
        public static int[] Directions = { -8, 8, -1, 1, -7, 7, -9, 9 };
        public static int[] KnightDirections = { -17, -15, 17, 15, -10, 6, 10, -6 };
        public static int[][] DistanceFromEdges = new int[64][];

        public static int FriendlyKingSquare = 0;
        public static bool KingInCheck = false;
        public static bool DoubleCheck = false;
        public static bool GeneratingAttacks = false;

        public static int SideToGenerateFor;

        public static List<int> TempSquaresToMoveToThatStopCheck = new List<int>();
        public static List<int> SquaresToMoveToThatStopCheck = new List<int>();

        public static List<int> PinnedLocations = new List<int>();
        public static List<int> EnemyAttacks = new List<int>();
        public static List<Move> LegalMoves = new List<Move>();

        public static void ClearLists()
        {
            TempSquaresToMoveToThatStopCheck.Clear();
            SquaresToMoveToThatStopCheck.Clear();
            PinnedLocations.Clear();
            EnemyAttacks.Clear();
            LegalMoves.Clear();
            GameWindow.currentLegalMoves.Clear();

            PieceLocator.PawnLocations.Clear();
            PieceLocator.KnightLocations.Clear();
            PieceLocator.BishopLocations.Clear();
            PieceLocator.RookLocations.Clear();
            PieceLocator.QueenLocations.Clear();
            PieceLocator.WKingLocation.Clear();
            PieceLocator.BKingLocation.Clear();

        }

        // Idea to calculate the distances from the edge of board from Sebastian Lauge
        // Loops through each square on board - keeping track of row and col
        // Gets distance from each edge
        // This could have just been stored as a really big hard coded array to save calulation
        // But as it is done at the start of the game and it is only done once
        // It is better to do it this way as it make more aesthetic code
        public static void FindDistanceFromEdges() // Calculate all the distances to the edge of the board for each location for later use
        {
            // Loop through every square on board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    // Calculate number of squares from top and bottom
                    int numFromTop = row;
                    int numFromBottom = 7 - row; // Minus from 7 not 8

                    // Calculate number of squares from left and right
                    int numFromLeft = col;
                    int numFromRight = 7 - col; // Minus from 7 not 8

                    int squareNo = (row * 8) + col;

                    DistanceFromEdges[squareNo] = new int[8] {
                        numFromTop,
                        numFromBottom,
                        numFromLeft,
                        numFromRight,
                        Math.Min(numFromTop, numFromRight), // diagonally up to the right
                        Math.Min(numFromBottom, numFromLeft), // diagonally down to the left
                        Math.Min(numFromTop, numFromLeft), // diagonally up to the left
                        Math.Min(numFromBottom, numFromRight) // diagonally down to the right
                    };
                }
            }
        }

        // Given a move (already know it will be a pawn move as its checked before calling)
        // Checks for colour
        // Check if corresponding MoveTo value is opposing back rank
        // If so return a queen as the piece for use
        // If not just return back the same pawn
        public static int CheckPromotion(Move move)
        {
            if (Piece.Colour(move.Piece) == 8 && (move.MoveTo / 8) == 0) // If the piece is white and the location is blacks back rank
            { 
                return Piece.White | Piece.Queen; // Turn pawn to queen
            }
            if (Piece.Colour(move.Piece) == 16 && (move.MoveTo / 8) == 7)
            {
                return Piece.Black | Piece.Queen;
            }
            else
            {
                return move.Piece; // If its not, carry on as usual.
            }
        }

        // Checks if the move generated for a piece should be added
        // Checks if the piece is pinned, if so return false no matter as pinned piece moves are added prior in GetPins Function
        // Check if King is in check, if not, the move will be allowed no matter what
        // If King is in check, check themoveTo value against the locations that stop check list
        public static bool CheckIfMoveIsAllowed(Move move)
        {
            if (PinnedLocations.Contains(move.MoveFrom)) // If the piece is pinned, dont allow move as moves for pinned pieces will have already been calculated
            {
                return false;
            }
            if (KingInCheck == false) // If there is no check and piece isnt pinned, allow move
            {
                return true;
            }
            else if (SquaresToMoveToThatStopCheck.Contains(move.MoveTo)) // If there is a check, and the moveTo square isnt one that stops check, dont allow
            {
                return true;
            }
            else // Anything else, allow the move
            {
                return false;
            }
        }

        // Swaps side to move for
        // Set generating attacks to true so captures arent included
        // Swap side to move to get enemy attacksd (dont call GameControl.ChangeSideToMove as it is not checked in generate move function)
        // Call all generate move functions
        // Reset side and generating attacks variable
        public static void GenerateEnemyAttacks()
        {
            // Console.WriteLine("Getting squares enemy attacks"); // Debug
            GeneratingAttacks = true;
            SideToGenerateFor = SideToGenerateFor == 8 ? 16 : 8;

            // Generate all the moves as usual but only save locations attacked
            GetPins();
            GetKingMoves();
            GetPawnMoves();
            GetKnightMoves();
            GetBishopMoves();
            GetRookMoves();
            GetQueenMoves();

            GeneratingAttacks = false;
            SideToGenerateFor = SideToGenerateFor == 8 ? 16 : 8;
        }

        // Gets side to generate for as fail safe
        // Clears previous moves, attacking lists and pinned locations
        // Sets king in check variable
        // Calls each generate move function
        // Returns a list of all the legal moves in a given position
        public static List<Move> GenerateLegalMoves()
        {
            // Console.WriteLine($"Generating Legal Moves For {(sideToGenerateFor == 8 ? "White" : "Black")}"); // Debug

            // Check side to move as fail safe as generating computer move can leave it flipped
            int sideToGenerateFor = GameControl.CheckSideToMove();
            SideToGenerateFor = sideToGenerateFor;
            FriendlyKingSquare = PieceLocator.GetLocationsOf(sideToGenerateFor | Piece.King)[0]; // Getlocation of friendly king

            // Clear old moves, pins and squares attacked
            EnemyAttacks.Clear();
            GenerateEnemyAttacks();
            LegalMoves.Clear();
            PinnedLocations.Clear();

            // Check if own king is in check in given position, if so the moves will be generated with this is mind
            KingInCheck = false;
            if (EnemyAttacks.Contains(FriendlyKingSquare))
            {
                KingInCheck = true;
                GameControl.Board[FriendlyKingSquare].BackColor = Color.Red;
            }

            // Generate new moves for all pieces for position

            GetPins();
            GetKingMoves();

             // NOT working but not essential - if it was working, it would only save a minimal amount of time
             // with this uncommented, if the king is in check, only king moves will be returned, not blocks or captures
             
            //if(DoubleCheck == true) // If there is a double check, only king can move so return early
            //{
            //    // Console.WriteLine("Double check, returning king moves");
            //    DoubleCheck = false;
            //    return LegalMoves;
            //}

            GetPawnMoves();
            GetKnightMoves();
            GetBishopMoves();
            GetRookMoves();
            GetQueenMoves();

            // Debug statment to print every legal move in given position - will spam console if playing against computer
            /* Console.WriteLine("Legal Moves for {sideToGenerateFor}: ");
            foreach(Move move in LegalMoves)
            {
                Console.WriteLine($"{Piece.PieceToFullName(move.Piece)} moves from {move.MoveFrom} to {move.MoveTo}");
            } */

            return LegalMoves;
        }

        // loops through all axis from king, up down, left right, and diagonals
        // if enemy piece is found (it would be check but nvm) - search next direction
        // if friendly piece is found, save location, keep searching same axis
        // if another friendly piece is found, search next direction
        // if enemy piece is found
        // check type with axis searching to see if its a pin
        // if it is a pin, save the location of the pinned piece
        // add the available moves for the pinned piece
        public static void GetPins()
        {
            int KingSquare = PieceLocator.GetLocationsOf(SideToGenerateFor | Piece.King)[0]; // King list should only have one location and so we can grab first element '[0]'

            for (int directionIndex = 0; directionIndex < 8; directionIndex++) // Loop through each possible direction
            {
                int locationToCheckPinFor = -1; // Set to minus one so we can check for if we're checking for a pin
                int pieceToCheckPinFor = 0; // Piece set to null to begin with

                for (int i = 0; i < DistanceFromEdges[KingSquare][directionIndex]; i++)
                {
                    int targetSquare = KingSquare + Directions[directionIndex] * (i + 1); // Targetsquare to move to is the current location + the direction we want to move times how many square away it is
                    int pieceOnTargetSquare = GameControl.Board[targetSquare].PieceOnSquare; // To check for captures
                    // Console.WriteLine($"direction: {Directions[directionIndex]}, target square: {targetSquare}, pieceOnTargetSquare: {pieceOnTargetSquare}, i = {i}"); // Debug

                    if (pieceOnTargetSquare == 0) // No piece on square
                    {
                        // Do nothing
                    }

                    if (pieceOnTargetSquare != 0 && Piece.IsColour(pieceOnTargetSquare, SideToGenerateFor)) // Friendly piece is on target square
                    {
                        if(locationToCheckPinFor == -1)
                        {
                            // Console.WriteLine("First friendly piece was found, saving location"); // Debug
                            locationToCheckPinFor = targetSquare; // Save location of piece
                            pieceToCheckPinFor = GameControl.Board[locationToCheckPinFor].PieceOnSquare; // Save piece we're checking for a pin on
                        }
                        else
                        {
                            // Console.WriteLine("Second friendly piece was found, searching next axis"); // Debug
                            break; // Second friendly piece was found, no pin so search next axis
                        }
                    }

                    if (pieceOnTargetSquare != 0 && !Piece.IsColour(pieceOnTargetSquare, SideToGenerateFor)) // Enemy piece is on target square
                    {
                        if(locationToCheckPinFor == -1) // If not checking a pin
                        {
                            break; // Search next axis
                        }
                        else // Else, must be checking for a pin
                        {
                            // Console.WriteLine("Checking For A Pin"); // Debug
                            if((Piece.Type(pieceOnTargetSquare) == Piece.Rook || Piece.Type(pieceOnTargetSquare) == Piece.Queen) && directionIndex < 4) // if piece is rook, queen or pawn and checking an orthogonal axis, add pin
                            {
                                PinnedLocations.Add(locationToCheckPinFor);
                                // Console.WriteLine("Pin found from rook or queen, adding moves to pinned piece");
                                // Check for rook and queen and add their moves if there is a pin
                                if(Piece.Type(pieceToCheckPinFor) == Piece.Rook || Piece.Type(pieceToCheckPinFor) == Piece.Queen)
                                {
                                    // For each square from king location in directions[direction index] until targetSquare add moves to move list excluding locationToCheckPinFor
                                    for (int square = 0; square < i+1; square++)
                                    {
                                        // Console.WriteLine($"Move To Location: { KingSquare + (Directions[directionIndex] * (square + 1))}"); // Debug
                                        if(EnemyAttacks.Contains(KingSquare) == false) // If king is in check, pinned piece can't move, have to check it this way and not "kingInCheck" variable as it gets changed when AI generates moves
                                        {
                                            LegalMoves.Add(new Move { Piece = pieceToCheckPinFor, MoveFrom = locationToCheckPinFor, MoveTo = KingSquare + (Directions[directionIndex] * (square + 1)) }); // Add move
                                        }
                                        EnemyAttacks.Add(KingSquare + (Directions[directionIndex] * (square + 1))); // Add that we attack the square no matter what
                                    }
                                }

                                // Add pawn moves if its pinned
                                if (Piece.Type(pieceToCheckPinFor) == Piece.Pawn && directionIndex < 2)
                                {
                                    if (Piece.Colour(pieceToCheckPinFor) == Piece.White)
                                    {
                                        // Add normal moves (not captures as that would be a diagonal axis)
                                        if (GameControl.Board[locationToCheckPinFor - 8].PieceOnSquare == 0 && EnemyAttacks.Contains(KingSquare) == false) // If location is empty and king not in check
                                        {
                                            LegalMoves.Add(new Move { Piece = pieceToCheckPinFor, MoveFrom = locationToCheckPinFor, MoveTo = locationToCheckPinFor - 8 }); // Add move
                                        }
                                        if ((locationToCheckPinFor / 8 == 6) && (GameControl.Board[locationToCheckPinFor - 16].PieceOnSquare == 0) && EnemyAttacks.Contains(KingSquare) == false) // If location is empty and king not in check
                                        {
                                            LegalMoves.Add(new Move { Piece = pieceToCheckPinFor, MoveFrom = locationToCheckPinFor, MoveTo = locationToCheckPinFor - 16 }); // Add move
                                        }

                                    }
                                    // Repeated code but for black :/ - just differnet signs for directions up/down the board
                                    else if (Piece.Colour(pieceToCheckPinFor) == Piece.Black)
                                    {
                                        // Add normal moves (not captures as that would be a diagonal axis)
                                        if (GameControl.Board[locationToCheckPinFor + 8].PieceOnSquare == 0 && EnemyAttacks.Contains(KingSquare) == false)
                                        {
                                            LegalMoves.Add(new Move { Piece = pieceToCheckPinFor, MoveFrom = locationToCheckPinFor, MoveTo = locationToCheckPinFor + 8 });
                                        }
                                        if ((locationToCheckPinFor / 8 == 1) && (GameControl.Board[locationToCheckPinFor + 16].PieceOnSquare == 0) && EnemyAttacks.Contains(KingSquare) == false)
                                        {
                                            LegalMoves.Add(new Move { Piece = pieceToCheckPinFor, MoveFrom = locationToCheckPinFor, MoveTo = locationToCheckPinFor + 16 });
                                        }
                                    }
                                }
                            }

                            else if((Piece.Type(pieceOnTargetSquare) == Piece.Bishop || Piece.Type(pieceOnTargetSquare) == Piece.Queen) && directionIndex > 3) // If piece is bishop or queen and diagonal axis, add pin
                            {
                                PinnedLocations.Add(locationToCheckPinFor);
                                // Console.WriteLine("Pin found from bishop or queen, adding moves to pinned piece"); // Debug
                                // Check for bishop and queen
                                if (Piece.Type(pieceToCheckPinFor) == Piece.Bishop || Piece.Type(pieceToCheckPinFor) == Piece.Queen)
                                {
                                    // For each square from king location in directions[direction index] until targetSquare add moves to move list excluding locationToCheckPinFor
                                    for (int square = 0; square < i+1; square++)
                                    {
                                        // Console.WriteLine($"Move To Location: { KingSquare + (Directions[directionIndex] * (square+1))}"); // Debug
                                        LegalMoves.Add(new Move { Piece = pieceToCheckPinFor, MoveFrom = locationToCheckPinFor, MoveTo = KingSquare + (Directions[directionIndex] * (square + 1)) });
                                        EnemyAttacks.Add(KingSquare + (Directions[directionIndex] * (square + 1)));
                                    }
                                }

                                // Add pawn captures
                                if (Piece.Type(pieceToCheckPinFor) == Piece.Pawn)
                                {
                                    if (Piece.Colour(pieceToCheckPinFor) == Piece.White)
                                    {
                                        if (locationToCheckPinFor - 9 == targetSquare && EnemyAttacks.Contains(KingSquare) == false)
                                        {
                                            LegalMoves.Add(new Move { Piece = pieceToCheckPinFor, MoveFrom = locationToCheckPinFor, MoveTo = locationToCheckPinFor - 9 });
                                        }
                                        if (locationToCheckPinFor - 7 == targetSquare && EnemyAttacks.Contains(KingSquare) == false)
                                        {
                                            LegalMoves.Add(new Move { Piece = pieceToCheckPinFor, MoveFrom = locationToCheckPinFor, MoveTo = locationToCheckPinFor - 7 });
                                        }
                                    }
                                    // Same code repeated for black again, just swapped signs
                                    else if (Piece.Colour(pieceToCheckPinFor) == Piece.Black)
                                    {
                                        if (locationToCheckPinFor + 9 == targetSquare && EnemyAttacks.Contains(KingSquare) == false)
                                        {
                                            LegalMoves.Add(new Move { Piece = pieceToCheckPinFor, MoveFrom = locationToCheckPinFor, MoveTo = locationToCheckPinFor + 9 });
                                        }
                                        if (locationToCheckPinFor + 7 == targetSquare && EnemyAttacks.Contains(KingSquare) == false)
                                        {
                                            LegalMoves.Add(new Move { Piece = pieceToCheckPinFor, MoveFrom = locationToCheckPinFor, MoveTo = locationToCheckPinFor + 7 });
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        } // Gets all pinned pieces and their moves for position

        public static void GetKingMoves()
        {
            int KingSquare = PieceLocator.GetLocationsOf(SideToGenerateFor | Piece.King)[0]; // king list should only have one location and so we can grab first element '[0]'

            // Normal Moves
            foreach (int direction in Directions)
            {
                int targetSquare = KingSquare + direction; // No need to loop through direction indexes and directions as king can only move one square at a time

                // Check if move is off board
                int row = KingSquare % 8;
                int col = KingSquare / 8;

                int targetRow = targetSquare % 8;
                int targetCol = targetSquare / 8;

                if (Math.Abs(targetRow - row) > 1 || Math.Abs(targetCol - col) > 1) // Cant move 'round' the board
                {
                    continue;
                }
                // End of check

                if (targetSquare < 0 || targetSquare > 63) { continue; } // If square is off the board, catch the indexoutofrange

                int pieceOnTargetSquare = GameControl.Board[targetSquare].PieceOnSquare; // Checking for captures
                Move attemptingMove = new Move { Piece = SideToGenerateFor | Piece.King, MoveFrom = KingSquare, MoveTo = targetSquare };

                if (GeneratingAttacks == true)
                {
                    EnemyAttacks.Add(targetSquare); // Add attacks square no matter when generating attacks
                }

                if (GeneratingAttacks == false)
                {
                    // Console.WriteLine($"King Move | Move From: {KingSquare}, Move To: {targetSquare}, Piece On Target Square: {Piece.PieceToFullName(pieceOnTargetSquare)}"); // Debug

                    if (pieceOnTargetSquare == 0) // No piece on target square - add move if enemy doesnt attack
                    {
                        if (EnemyAttacks.Contains(targetSquare) == false)
                        {
                            LegalMoves.Add(attemptingMove);
                        }
                    }

                    else if (Piece.Colour(pieceOnTargetSquare) != SideToGenerateFor) // Enemy piece is on target square - add move if it isnt being attacked by enemy
                    {
                        if (EnemyAttacks.Contains(targetSquare) == false)
                        {
                            LegalMoves.Add(attemptingMove);
                        }
                    }

                    // No need to generate if friendly piece is on square
                }
            }

            // Castling -- Really ugly way of doing it but i cant think of another way :/
            if (KingInCheck == false && GeneratingAttacks == false) // cant castle out of check, castling isnt an attacking move
            {
                if (SideToGenerateFor == 8)
                {
                    if (KingSquare == 60) // King must be on square 60 to castle if white
                    {
                        // Line below checks if path to rook is clear, none of the squares are being attacked and the right to castle is true
                        // Same for each one, just different caslte locations so different locations are passed in
                        if (WhiteQueenSide == true && GameControl.Board[57].PieceOnSquare == 0 && GameControl.Board[58].PieceOnSquare == 0 && GameControl.Board[59].PieceOnSquare == 0 && EnemyAttacks.Contains(57) == false && EnemyAttacks.Contains(58) == false && EnemyAttacks.Contains(59) == false && GameControl.Board[56].PieceOnSquare == (Piece.White | Piece.Rook))
                        {
                            LegalMoves.Add(new Move { Piece = SideToGenerateFor | Piece.King, MoveFrom = KingSquare, MoveTo = 58 });
                        }
                        if (WhiteKingSide == true && GameControl.Board[61].PieceOnSquare == 0 && GameControl.Board[62].PieceOnSquare == 0 && EnemyAttacks.Contains(61) == false && EnemyAttacks.Contains(62) == false && GameControl.Board[63].PieceOnSquare == (Piece.White | Piece.Rook))
                        {
                            LegalMoves.Add(new Move { Piece = SideToGenerateFor | Piece.King, MoveFrom = KingSquare, MoveTo = 62 });
                        }
                    }
                }
                if (SideToGenerateFor == 16)
                { 
                    if (KingSquare == 4) // King must be on square 4 to castle if black
                    {
                        // Line below checks if path to rook is clear, none of the squares are being attacked and the right to castle is true
                        // Same for each one, just different caslte locations so different locations are passed in
                        // (Once again repeated code >:( )
                        if (BlackQueenSide == true && GameControl.Board[1].PieceOnSquare == 0 && GameControl.Board[2].PieceOnSquare == 0 && GameControl.Board[3].PieceOnSquare == 0 && EnemyAttacks.Contains(1) == false && EnemyAttacks.Contains(2) == false && EnemyAttacks.Contains(3) == false && GameControl.Board[0].PieceOnSquare == (Piece.Black | Piece.Rook))
                        {
                            LegalMoves.Add(new Move { Piece = SideToGenerateFor | Piece.King, MoveFrom = KingSquare, MoveTo = 2 });
                        }
                        if (BlackKingSide == true && GameControl.Board[5].PieceOnSquare == 0 && GameControl.Board[6].PieceOnSquare == 0 && EnemyAttacks.Contains(5) == false && EnemyAttacks.Contains(6) == false && GameControl.Board[7].PieceOnSquare == (Piece.Black | Piece.Rook))
                        {
                            LegalMoves.Add(new Move { Piece = SideToGenerateFor | Piece.King, MoveFrom = KingSquare, MoveTo = 6 });
                        }
                    }
                }
            }
        } // Appends king moves to move list

        public static void GetKnightMoves()
        {
            List<int> KnightSqaures = PieceLocator.GetLocationsOf(SideToGenerateFor | Piece.Knight);

            foreach (int location in KnightSqaures)
            {
                // Console.WriteLine($"Knight Square = {location}");

                // Get row and col as if we just add knight direction indexs, it will be able to jump ''around'' the board. These will be checked against the distance of the move later
                int row = location % 8;
                int col = location / 8;

                foreach (int direction in KnightDirections)
                {
                    int targetSquare = location + direction;
                    int targetRow = targetSquare % 8;
                    int targetCol = targetSquare / 8;

                    // If target row or col is more than 2 away (in any direction) from origonal row or col, continue as move would jump 'around' the board
                    if (Math.Abs(row - targetRow) > 2)
                    {
                        continue;
                    }
                    if (Math.Abs(col - targetCol) > 2)
                    {
                        continue;
                    }

                    //Console.WriteLine($"location: {location}, direction: {direction}, target square: {targetSquare}"); // Debug
                    if (targetSquare > 63 || targetSquare < 0) // Quick check to see if target square is inside bounds of board array
                    {  
                        //Console.WriteLine("target square not on board, next number");
                        continue;
                    }
                    int pieceOnTargetSquare = GameControl.Board[targetSquare].PieceOnSquare; // Check for capture

                    if (pieceOnTargetSquare == 0) // No piece on target square
                    {
                        // Console.WriteLine($"Target Square Empty, Adding move: {targetSquare}"); // Debug
                        if (EnemyAttacks.Contains(targetSquare) == false) { EnemyAttacks.Add(targetSquare); } // Add to attacks
                        Move AttemptingMove = new Move { Piece = SideToGenerateFor | Piece.Knight, MoveFrom = location, MoveTo = targetSquare };
                        if (GeneratingAttacks == false && CheckIfMoveIsAllowed(AttemptingMove)) { LegalMoves.Add(AttemptingMove); } // Check if move is legal and add if true
                    }
                    else if (Piece.IsColour(pieceOnTargetSquare, SideToGenerateFor) == false) // Enemy piece on target square
                    {
                        // Console.WriteLine($"Enemy piece found, Adding move: {targetSquare}"); // add move to move list
                        if(Piece.Type(pieceOnTargetSquare) == Piece.King)
                        { 
                            SquaresToMoveToThatStopCheck.Add(location); // If the knight gives a check, add the location to the places that stop check as capturing it would stop check
                            if (KingInCheck == true)
                            {
                                DoubleCheck = true;  // If king is already in check Its A double check
                            }
                            else 
                            { 
                                KingInCheck = true;
                            } 
                        }
                        if (EnemyAttacks.Contains(targetSquare) == false) { EnemyAttacks.Add(targetSquare); } // Add to squares attacked
                        Move AttemptingMove = new Move { Piece = SideToGenerateFor | Piece.Knight, MoveFrom = location, MoveTo = targetSquare };
                        if (GeneratingAttacks == false && CheckIfMoveIsAllowed(AttemptingMove)) { LegalMoves.Add(AttemptingMove); } // Check if move legal, and add to legal moves if so
                    }
                    else if (Piece.IsColour(pieceOnTargetSquare, SideToGenerateFor)) // Friendly piece is on target square - break before adding move so you cant capture own piece
                    {
                        if (EnemyAttacks.Contains(targetSquare) == false) { EnemyAttacks.Add(targetSquare); } // Add Enemy attacks anyway
                        // Console.WriteLine("Friendly piece found");
                    }
                }
            }
        } // Appends knight moves to move list

        public static void GetPawnMoves()
        {
            List<int> PawnSqaures = PieceLocator.GetLocationsOf(SideToGenerateFor | Piece.Pawn);

            foreach (int location in PawnSqaures)
            {
                // Console.WriteLine($"Pawn Square = {location}"); // Debug

                int[] WPawnTakeDirections = { -9, -7 }; // Take directions from int array
                int[] BPawnTakeDirections = { 9, 7 }; // Reversed for black as they are going opposite direction

                int piece = GameControl.sideToMove | Piece.Pawn; // Set piece to pass into promotion

                if (SideToGenerateFor == 8)
                {
                    // Check for moves
                    int targetSquare = location - 8;
                    if (GameControl.Board[targetSquare].PieceOnSquare == 0)
                    {
                        Move AttemptingMove = new Move { Piece = piece, MoveFrom = location, MoveTo = targetSquare };
                        if (GeneratingAttacks == false && CheckIfMoveIsAllowed(AttemptingMove)) { LegalMoves.Add(AttemptingMove); }
                        targetSquare = location - 16; 
                        // Dont have to check for promotion here as it is first move
                        if (location / 8 == 6 && GameControl.Board[targetSquare].PieceOnSquare == 0)
                        {
                            Move AttemptingMove2 = new Move { Piece = piece, MoveFrom = location, MoveTo = targetSquare };
                            if (GeneratingAttacks == false && CheckIfMoveIsAllowed(AttemptingMove2)) { LegalMoves.Add(AttemptingMove2); }
                        }
                    }
                    // Check for captures
                    foreach (int direction in WPawnTakeDirections)
                    {
                        // Check if trying to capture 'around' the board
                        if ((location % 8 == 0 && direction == -9) || (location % 8 == 7 && direction == -7)) 
                        {
                            continue;
                        }

                        int pieceOnTargetSquare = GameControl.Board[location + direction].PieceOnSquare;

                        targetSquare = location + direction; // Get target location
                        if (EnemyAttacks.Contains(targetSquare) == false) { EnemyAttacks.Add(targetSquare); }
                        if (Piece.IsColour(pieceOnTargetSquare, SideToGenerateFor) == false && pieceOnTargetSquare != 0) // If piece isnt freidnly and there is a piece on the square
                        {
                            if (Piece.Type(pieceOnTargetSquare) == Piece.King) { SquaresToMoveToThatStopCheck.Add(location); if (KingInCheck == true) { DoubleCheck = true; } else { KingInCheck = true; } }
                            Move AttemptingMove = new Move { Piece = piece, MoveFrom = location, MoveTo = targetSquare };
                            if (GeneratingAttacks == false && CheckIfMoveIsAllowed(AttemptingMove)) { LegalMoves.Add(AttemptingMove); }
                        }
                    }
                }
                // Repeated code with directions changed
                if (SideToGenerateFor == 16)
                {
                    // Check for moves
                    int targetSquare = location + 8;
                    if (GameControl.Board[targetSquare].PieceOnSquare == 0)
                    {
                        Move AttemptingMove = new Move { Piece = piece, MoveFrom = location, MoveTo = targetSquare };
                        if (GeneratingAttacks == false && CheckIfMoveIsAllowed(AttemptingMove)) { LegalMoves.Add(AttemptingMove); }

                        targetSquare = location + 16; // Check for double pawn push
                        // Don't have to check for promotion here as it is first move
                        if (location / 8 == 1 && GameControl.Board[targetSquare].PieceOnSquare == 0)
                        {
                            Move AttemptingMove2 = new Move { Piece = piece, MoveFrom = location, MoveTo = targetSquare };
                            if (GeneratingAttacks == false && CheckIfMoveIsAllowed(AttemptingMove2)) { LegalMoves.Add(AttemptingMove2); }
                        }
                    }
                    // Check for captures
                    foreach (int direction in BPawnTakeDirections)
                    {
                        // Check if trying to capture 'around' the board
                        if ((location % 8 == 0 && direction == 7) || (location % 8 == 7 && direction == 9))
                        {
                            continue;
                        }

                        int pieceOnTargetSquare = GameControl.Board[location + direction].PieceOnSquare;

                        targetSquare = location + direction;
                        if (EnemyAttacks.Contains(targetSquare) == false) { EnemyAttacks.Add(targetSquare); }
                        if (Piece.IsColour(pieceOnTargetSquare, SideToGenerateFor) == false && pieceOnTargetSquare != 0)
                        {
                            if (Piece.Type(pieceOnTargetSquare) == Piece.King) { SquaresToMoveToThatStopCheck.Add(location); if (KingInCheck == true) { DoubleCheck = true; } else { KingInCheck = true; } }
                            Move AttemptingMove = new Move { Piece = piece, MoveFrom = location, MoveTo = targetSquare };
                            if (GeneratingAttacks == false && CheckIfMoveIsAllowed(AttemptingMove)) { LegalMoves.Add(AttemptingMove); }
                        }
                    }
                }
            }
        } // Appends pawn moves to move list

        public static void GetBishopMoves()
        {
            List<int> BishopSquares = PieceLocator.GetLocationsOf(SideToGenerateFor | Piece.Bishop);
            GetSlidingPieceMoves(BishopSquares, SideToGenerateFor | Piece.Bishop);
        } // Calls "get sliding piece moves" for all bishops

        public static void GetRookMoves()
        {
            List<int> RookSquares = PieceLocator.GetLocationsOf(SideToGenerateFor | Piece.Rook);
            GetSlidingPieceMoves(RookSquares, SideToGenerateFor | Piece.Rook);
        } // Calls "get sliding piece moves" for all rooks

        public static void GetQueenMoves()
        {
            List<int> QueenSquares = PieceLocator.GetLocationsOf(SideToGenerateFor | Piece.Queen);
            GetSlidingPieceMoves(QueenSquares, SideToGenerateFor | Piece.Queen);
        } // Calls "get sliding piece moves" for queen/s

        public static void GetSlidingPieceMoves(List<int> locationList, int piece)
        {
            foreach(int location in locationList)
            {
                //Console.WriteLine($"{Piece.PieceToFullName(piece).Split(' ')[1]} Square = {location}");

                int start = (Piece.IsType(piece, Piece.Bishop)) ? 4 : 0; // Only loop through distance index for either diags or axis depending on piece
                int end = (Piece.IsType(piece, Piece.Rook)) ? 4 : 8;

                for (int directionIndex = start; directionIndex < end; directionIndex++)
                {
                    //Console.Write($"curretn distance from edge checking: {DistanceFromEdges[location][directionIndex]}, ");
                    TempSquaresToMoveToThatStopCheck.Clear();
                    for (int i = 0; i < DistanceFromEdges[location][directionIndex]; i++)
                    {
                        int targetSquare = location + Directions[directionIndex] * (i + 1); // Targetsquare to move to is the current location + the direction we want to move times how many square away it is
                        int pieceOnTargetSquare = GameControl.Board[targetSquare].PieceOnSquare;
                        //Console.WriteLine($"location: {location} direction: {Directions[directionIndex]}, target square: {targetSquare}, pieceOnTargetSquare: {pieceOnTargetSquare}");

                        if (pieceOnTargetSquare != 0 && Piece.IsColour(pieceOnTargetSquare, SideToGenerateFor)) // Friendly piece is on target square - break before adding move so you cant capture own piece
                        {
                            // Console.WriteLine("Friendly piece found");
                            if (EnemyAttacks.Contains(targetSquare) == false) { EnemyAttacks.Add(targetSquare); }
                            break;
                        }

                        if (EnemyAttacks.Contains(targetSquare) == false) { EnemyAttacks.Add(targetSquare); }
                        TempSquaresToMoveToThatStopCheck.Add(targetSquare);

                        Move AttemptingMove = new Move { Piece = piece, MoveFrom = location, MoveTo = targetSquare };
                        if (GeneratingAttacks == false && CheckIfMoveIsAllowed(AttemptingMove)) {LegalMoves.Add(AttemptingMove);}

                        if (pieceOnTargetSquare != 0 && !Piece.IsColour(pieceOnTargetSquare, SideToGenerateFor)) // Enemy piece is on target square - break after adding move so it allows us to take
                        {
                            // Console.WriteLine("Enemy piece found");
                            if(Piece.Type(pieceOnTargetSquare) == Piece.King)
                            {
                                // Console.WriteLine("King found, making King in check true and adding moves attacked by piece to locations that stop check.");
                                if (KingInCheck == true) { DoubleCheck = true; } else { KingInCheck = true; }
                                SquaresToMoveToThatStopCheck.AddRange(TempSquaresToMoveToThatStopCheck);
                                SquaresToMoveToThatStopCheck.Add(location);
                                EnemyAttacks.Add(location + Directions[directionIndex] * (i + 2)); // Add the square behind the king to the attacks
                            }
                            break;
                        }
                    }
                }
            }
        }

        // Variables for checking castling availablility
        public static bool BlackQueenSide = true;
        public static bool BlackKingSide = true; 
        public static bool WhiteQueenSide = true; 
        public static bool WhiteKingSide = true;

        public static bool BlackKingHasMoved = false;
        public static bool WhiteKingHasMoved = false;

        // Checks if rook has moved or been captured
        // Checks if king has moved
        // Updates corresponding variables accordingly
        public static void CheckIfCastlingRightsHaveChanged(Move move)
        {
            if (move.MoveFrom == 0 || move.MoveTo == 0) // Check if rook has moved or been captured
            {
                BlackQueenSide = false;
            }
            if (move.MoveFrom == 7 || move.MoveTo == 7)
            {
                BlackKingSide = false;
            }
            if (move.MoveFrom == 56 || move.MoveTo == 56)
            {
                WhiteQueenSide = false;
            }
            if (move.MoveFrom == 63 || move.MoveTo == 63)
            {
                WhiteKingSide = false;
            }

            if (move.Piece == (Piece.White | Piece.King)) // If king moves, cant castle anymore
            {
                WhiteKingHasMoved = true;
                WhiteQueenSide = false;
                WhiteKingSide = false;
            }
            if (move.Piece == (Piece.Black | Piece.King))
            {
                BlackKingHasMoved = true;
                BlackQueenSide = false;
                BlackKingSide = false;
            }
        }

    }
}