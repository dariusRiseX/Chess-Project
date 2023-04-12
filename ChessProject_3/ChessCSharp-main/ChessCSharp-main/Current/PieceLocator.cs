using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class PieceLocator
    {
        public const int LocationMask = 0b01111111;
        public const int ColourMask = 0b10000000;

        // stored as ints, first bit is the colour (128 or 0, 1 or 0) and last  7 bits store location (0, 63)

        public static List<int> PawnLocations = new List<int>();
        public static List<int> KnightLocations = new List<int>();
        public static List<int> BishopLocations = new List<int>();
        public static List<int> RookLocations = new List<int>();
        public static List<int> QueenLocations = new List<int>();
        public static List<int> BKingLocation = new List<int> { 4 };
        public static List<int> WKingLocation = new List<int> { 60 };


        // get piece colour, and type
        // search corresponding piece list for pieces of that colour
        // filter out colour using mask to return location
        // return list of locations
        public static List<int> GetLocationsOf(int piece)
        {
            List<int> locationList = new List<int>();

            // Change global colour int to the local colour int: 128 white, 0 black instead of 8 white and 16 black
            int colour = Piece.Colour(piece) == 8 ? 128 : 0;
            int type = Piece.Type(piece);

            if (type == Piece.King)
            {
                return colour == 128 ? WKingLocation : BKingLocation;
            }

            // Switch case used to return location list
            switch (type)
            {
                case Piece.Pawn:
                    locationList = PawnLocations;
                    break;
                case Piece.Knight:
                    locationList = KnightLocations;
                    break;
                case Piece.Bishop:
                    locationList = BishopLocations;
                    break;
                case Piece.Rook:
                    locationList = RookLocations;
                    break;
                case Piece.Queen:
                    locationList = QueenLocations;
                    break;
            }
            
            return searchList(locationList, colour);

        }

        // Searches a given list for a colour
        // Done by comparing colour mask to remove location
        // If found, use colour mask to just append location to list
        // return locations
        public static List<int> searchList(List<int> locationList, int colour)
        {
            List<int> locations = new List<int>();

            foreach(int location in locationList)
            {
                // Console.WriteLine($"datanumber ={location} | colour = {colour} | colourmasked data = {location & ColourMask} | locationmasked data = {location & LocationMask} ");
                if ((location & ColourMask) == colour) // Check if piece location is of correct colour
                {
                    locations.Add(location & LocationMask); // Add just the location to the location list, colour not needed
                }
            }

            /* Debug to show all locations returned
            foreach(int location in locations)
            {
                Console.Write($"{location}, ");
            }
            Console.WriteLine(" ");
            */

            return locations;
        }

        // Gets number of given piece, with set colour using search list function
        // Multiplies the result by the piece value
        // Checks the side its checking for
        // Adds or subtracts black and white values accordingly
        public static int GetPosEval()
        {
            int PositionEval = 0;
            int WhitePieceTotal = 0;
            int BlackPieceTotal = 0;

            WhitePieceTotal += (searchList(PawnLocations, 128).Count() * 130); // Normal value given in 100 but changed as computer was too willing to give free pawns
            WhitePieceTotal += (searchList(KnightLocations, 128).Count() * 300);
            WhitePieceTotal += (searchList(BishopLocations, 128).Count() * 350); // Normal value given is 300 but most modern day computers value the bishop more than the knight
            WhitePieceTotal += (searchList(RookLocations, 128).Count() * 500);
            WhitePieceTotal += (searchList(QueenLocations, 128).Count() * 900);
                               
            BlackPieceTotal += (searchList(PawnLocations, 0).Count() * 130);
            BlackPieceTotal += (searchList(KnightLocations, 0).Count() * 300);
            BlackPieceTotal += (searchList(BishopLocations, 0).Count() * 350);
            BlackPieceTotal += (searchList(RookLocations, 0).Count() * 500);
            BlackPieceTotal += (searchList(QueenLocations,0).Count() * 900);

            if(GameControl.CheckSideToMove() == 8) // If white
            {
                PositionEval += WhitePieceTotal -= BlackPieceTotal; // Add white piece value, subtract black
            }
            else
            {
                PositionEval += BlackPieceTotal -= WhitePieceTotal; // Vice Versa
            }

            return PositionEval;
        }

        // Gets the location and piece colour and combines them to get data value
        // Then simply appends to corresponding list
        // For kings, as there should only be one, list is cleared and location is added
        public static void AddToList(int piece, int location)
        {
            int colour = Piece.Colour(piece) == 8 ? 128 : 0; // Change global colour int to the local colour int: 128 white, 0 black instead of 8 white and 16 black
            int type = Piece.Type(piece); // Get type of piece as int
            int data = location + colour; // Location and colour ints create the data to be stored

            // Console.WriteLine($"Adding piece to piece list: {Piece.PieceToFullName(piece)} | Location: {location} | Data number: {data}\n");

            switch (type)
            {
                case Piece.Pawn:
                    PawnLocations.Add(data);
                    break;
                case Piece.Knight:
                    KnightLocations.Add(data);
                    break;
                case Piece.Bishop:
                    BishopLocations.Add(data);
                    break;
                case Piece.Rook:
                    RookLocations.Add(data);
                    break;
                case Piece.Queen:
                    QueenLocations.Add(data);
                    break;
                case Piece.King:
                    // Clear whole list as there should only be one king location, this saves having to serach list when removing
                    if (colour == 128) // Check colour
                    {
                        WKingLocation.Clear();
                        WKingLocation.Add(location);
                    }
                    else
                    {
                        BKingLocation.Clear();
                        BKingLocation.Add(location);
                    }
                    break;
            }
        }

        // Gets the location and piece colour and combines them to get data value
        // Then simply removes from corresponding list
        // King isnt needed as if it captured somthing has gone wrong
        public static void RemoveFromList(int piece, int location)
        {
            int colour = Piece.Colour(piece) == 8 ? 128 : 0; // Change global colour int to the local colour int: 128 white, 0 black instead of 8 white and 16 black
            int type = Piece.Type(piece); // Get type of piece as int
            int data = location + colour; // Location and colour ints create the data to be stored

            // Console.WriteLine($"Removing piece from piece list: {Piece.PieceToFullName(piece)} | Location: {location} | Data number: {data}\n");

            switch (type) // Switch case to remove from correct list
            {
                case Piece.Pawn:
                    PawnLocations.Remove(data);
                    break;
                case Piece.Knight:
                    KnightLocations.Remove(data);
                    break;
                case Piece.Bishop:
                    BishopLocations.Remove(data);
                    break;
                case Piece.Rook:
                    RookLocations.Remove(data);
                    break;
                case Piece.Queen:
                    QueenLocations.Remove(data);
                    break;
            }
        }
    }
}
