using System;
using System.Resources;
using System.Drawing;

namespace Chess
{
    public class Piece
    {
        // Idea from sebastian lauge
        // Piece numbers represented as binary mean colour is first two bits and piece number is last 3 bits
        // eg 10110 is a black queen 10 (black) 110 (queen) 01001 is a white pawn
        public const int None = 0;
        public const int King = 1;
        public const int Pawn = 2;
        public const int Knight = 3;
        public const int Bishop = 4;
        public const int Rook = 5;
        public const int Queen = 6;

        public const int White = 8;
        public const int Black = 16;

        const int typeMask = 0b00111; // Mask first two bits to get piece type
        const int colourMask = 0b11000; // Mask last three bits to get piece type

        public static bool IsColour(int piece, int colour)
        {
            return (piece & colourMask) == colour; // & binary operation on piece number and colour mask
        }

        public static bool IsType(int piece, int type)
        {
            return (piece & typeMask) == type; // & binary operation on piece number and type mask
        }

        public static int Colour(int piece)
        {
            return piece & colourMask;
        }

        public static int Type(int piece)
        {
            return piece & typeMask;
        }

        public static int Value(int piece) // Returns the value of a piece
        {
            switch (Type(piece))
            {
                case Pawn:
                    return 130;
                case Knight:
                    return 300;
                case Bishop:
                    return 350;
                case Rook:
                    return 500;
                case Queen:
                    return 900;
            }

            return 1000; // king

        }

        public static string PieceToImageName(int piece) // For getting image from recources to show on image board
        {
            string Colour = "W";
            string Type = "Pawn";

            if (Piece.Colour(piece) == 16)
            {
                Colour = "B"; // Swap colour string is colour is black
            }

            switch (Piece.Type(piece)) // Set type
            {
                case 1:
                    Type = "King";
                    break;
                case 2:
                    Type = "Pawn";
                    break;
                case 3:
                    Type = "Knight";
                    break;
                case 4:
                    Type = "Bishop";
                    break;
                case 5:
                    Type = "Rook";
                    break;
                case 6:
                    Type = "Queen";
                    break;
            }

            string pieceImageName = Colour + Type; // Combine for image name

            return pieceImageName;
        }

        public static string PieceToFullName(int piece) // Get piece name for making trouble shooting output look nice
        {
            if(piece == 0)
            {
                return "null"; // No piece name
            }

            string pieceImageName = PieceToImageName(piece); // Get image name
            if(pieceImageName[0] == 'B')
            {
                return ($"Black {pieceImageName.Remove(0, 1)}"); // Move the B and put piece name with "Black"
            }
            else
            {
                return ($"White {pieceImageName.Remove(0, 1)}"); // Move the W and put piece name with "White"
            }
        }

        public static Bitmap PieceToImage(int piece) // Gets the image from recources
        {
            if(piece == 0)
            {
                return null;
            }

            string pieceImageName = PieceToImageName(piece); // Get image name

            ResourceManager RecourceManager = Properties.Resources.ResourceManager; // Set Recource manager variable
            Bitmap pieceImage = (Bitmap)RecourceManager.GetObject(pieceImageName); // Get the variable as a bitmap

            return pieceImage;
        }

        public static string ColourNameFromPieceBin(int piece) // Get piece colour (given binary for piece) for trouble shooting
        {
            string pieceImageName = PieceToImageName(piece);
            if (pieceImageName[0] == 'B')
            {
                return ("Black");
            }
            else
            {
                return ("White");
            }
        }

        public static string ColourNameFromColourBin(int colour)// Get piece colour (given int colour) for trouble shooting
        {
            return colour == 8 ? "White" : "Black";
        }

    }
}
