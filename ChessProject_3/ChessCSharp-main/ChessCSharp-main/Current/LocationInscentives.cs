using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
	class LocationInscentives
	{
		public static int GetLocationInscentiveFor(int piece, int location)
		{
			// Get colour and type to check which board to use
			int type = Piece.Type(piece);
			int colour = Piece.Colour(piece);

			int[] LocationInscentiveGrid = new int[64]; // Location grid that will be used

			if (type == Piece.King)
			{
				// Decide whether to give end game or middle game grid
				LocationInscentiveGrid = kingMiddle;
				if (GameControl.GetEndGameWeight() >= 4) // Over 5 is considered end game
				{
					LocationInscentiveGrid = kingEnd;
				}
			}
			else
			{
				// Assign location grid depending on piece type
				switch (type)
				{
					case Piece.Pawn:
						LocationInscentiveGrid = pawns;
						break;
					case Piece.Knight:
						LocationInscentiveGrid = knights;
						break;
					case Piece.Bishop:
						LocationInscentiveGrid = bishops;
						break;
					case Piece.Rook:
						LocationInscentiveGrid = rooks;
						break;
					case Piece.Queen:
						LocationInscentiveGrid = queens;
						break;
				}
			}

			// Reverse array for blacks moves
			if (colour == 16)
			{
				LocationInscentiveGrid = LocationInscentiveGrid.Reverse().ToArray();
			}

			return LocationInscentiveGrid[location]*-1;

		}

		// Location inscentive boards

		public static readonly int[] pawns = {
			99, 99, 99, 99, 99, 99, 99, 99,
			50, 50, 50, 50, 50, 50, 50, 50,
			10, 10, 20, 30, 30, 20, 10, 10,
			5,  5, 10, 27, 27, 10,  5,  5,
			0,  0,  0, 30, 30,  0,  0,  0,
			3, -5,-10, 10, 10,-10, -5,  3,
			3, 10, 10,-20,-20, 10, 10,  3,
			0,  0,  0,  0,  0,  0,  0,  0
		};

		public static readonly int[] knights = {
			-50,-40,-30,-30,-30,-30,-40,-50,
			-40,-20,  0,  0,  0,  0,-20,-40,
			-30,  0, 20, 15, 15, 20,  0,-40,
			-30,  5, 15, 25, 25, 15,  5,-30,
			-30,  0, 15, 25, 25, 15,  0,-30,
			-10,  5, 30, 15, 15, 30,  5,-10,
			-40,-20,  0,  5,  5,  0,-20,-40,
			-50,-40,-30,-30,-30,-30,-40,-50,
		};

		public static readonly int[] bishops = {
			-20,-10,-10,-10,-10,-10,-10,-20,
			-10,  0,  0,  0,  0,  0,  0,-10,
			-10,  0,  5, 10, 10,  5,  0,-10,
			-10,  5,  5, 10, 10,  5,  5,-10,
			-10,  0, 10, 10, 10, 10,  0,-10,
			-10, 10, 10, 10, 10, 10, 10,-10,
			 10,  5,  0,  0,  0,  0,  5, 10,
			-20,-10,-10,-10,-10,-10,-10,-20,
		};

		public static readonly int[] rooks = {
			 0,  0,  0,  0,  0,  0,  0,  0,
			 10, 10, 10, 10, 10, 10, 10,  10,
			-5,  0,  0,  0,  0,  0,  0, -5,
			-5,  0,  0,  0,  0,  0,  0, -5,
			-5,  0,  0,  5,  5,  0,  0, -5,
			-5,  0,  0,  0,  0,  0,  0, -5,
			-5,  0,  0,  0,  0,  0,  0, -5,
			 0,  0,  3,  5,  5,  3,  0,  0
		};

		public static readonly int[] queens = {
			-20,-10,-10, -5, -5,-10,-10,-20,
			-10,  0,  0,  0,  0,  0,  0,-10,
			-10,  0,  5,  5,  5,  5,  0,-10,
			-5,  0,  5,  5,  5,  5,  0, -5,
			  0,  0,  5,  5,  5,  5,  0, -5,
			-10,  5,  5,  5,  5,  5,  0,-10,
			-10,  0,  5,  0,  0,  0,  0,-10,
			-20,-10,-10, -5, -5,-10,-10,-20
		};

		public static readonly int[] kingMiddle = {
			-30,-40,-40,-50,-50,-40,-40,-30,
			-30,-40,-40,-50,-50,-40,-40,-30,
			-30,-40,-40,-50,-50,-40,-40,-30,
			-30,-40,-40,-50,-50,-40,-40,-30,
			-20,-30,-30,-40,-40,-30,-30,-20,
			-10,-20,-20,-20,-20,-20,-20,-10,
			 13, 13,  0,  0,  0,  0, 13, 13,
			 15, 17,  0,  0,  0,  0, 17, 15
		};

		public static readonly int[] kingEnd = {
			-50,-40,-30,-20,-20,-30,-40,-50,
			-40,-20,-10,  0,  0,-10,-20,-30,
			-40,-20, 20, 25, 25, 20,-20,-40,
			-40,-20, 25, 35, 35, 25,-20,-40,
			-40,-20, 25, 35, 35, 25,-20,-40,
			-40,-20, 20, 25, 25, 20,-20,-40,
			-40,-30,  0,  0,  0,  0,-30,-40,
			-50,-40,-30,-30,-30,-30,-40,-50
		};
	}
}
