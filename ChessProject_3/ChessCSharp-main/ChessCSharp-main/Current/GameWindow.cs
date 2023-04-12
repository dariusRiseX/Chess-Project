using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Chess
{
    public partial class GameWindow : Form
    {
        public static string Player1Name = "Player 1";
        public static string Player2Name = "Player 2";
        public static int TimeControl = 120;

        public static bool ShowBoardSquareNumbers = false;

        public static int WhiteTimeLeft;
        public static int BlackTimeLeft;

        public static EventHandler WhiteEv = new EventHandler(WhiteTimerTick);
        public static EventHandler BlackEv = new EventHandler(BlackTimerTick);

        public static ListView MoveStack;
        public static string lastMove;
        public static List<Move> currentLegalMoves = new List<Move>();

        public GameWindow(bool singlePlayer)
        {
            InitializeComponent();

            // Set up move stack
            MoveStack = MoveStackDisplay;

            // Make sure variables are set to default values
            GameControl.SetOriginalVariables(singlePlayer);

            // Set start time for both players
            if (TimeControl > 0)
            {
                // Make times the time control
                WhiteTimeLeft = TimeControl;
                BlackTimeLeft = TimeControl;

                if (GameControl.NoGames == 1)
                {
                    // Add tick handlers to timers
                    GameControl.WhiteTimer.Tick += WhiteEv;
                    GameControl.BlackTimer.Tick += BlackEv;
                }

                // Format time displays
                BlackTimeDisplay.Text = TimeSpan.FromSeconds(BlackTimeLeft).ToString(@"mm\:ss");
                WhiteTimeDisplay.Text = TimeSpan.FromSeconds(WhiteTimeLeft).ToString(@"mm\:ss");
            }

            else
            {
                GameControl.WhiteTimer.Tick -= WhiteEv;
                GameControl.BlackTimer.Tick -= BlackEv;

                BlackTimeDisplay.Text = "";
                WhiteTimeDisplay.Text = "";
            }
        }

        public void GameWindow_Load(object sender, EventArgs e)
        {
            Size = new Size(1000, 1000); // Set size
            BackColor = Color.LightGray; // And bg colour

            DrawBoard();

            // Find distance from edges before game begins to calculate legal moves
            RuleBook.FindDistanceFromEdges();
        }
        public static void UpdateMoveStackDisplay(string MoveDesc)
        {
            if(GameControl.sideToMove == 8) // Check side to move
            {
                // If white, just add move to display
                ListViewItem item = new ListViewItem(MoveDesc);
                MoveStack.Items.Add(item);
            }
            else
            {
                // Annoying to do but couldnt find another way
                // If black add, remove white move from first col
                // Create new listview item with last move and current move so it spans both columns
                // Then add the item
                MoveStack.Items[MoveStack.Items.Count - 1].Remove();
                ListViewItem item = new ListViewItem(new[] {lastMove, MoveDesc});
                MoveStack.Items.Add(item);
            }
            // Save white move
            lastMove = MoveDesc;
        }
        public static void SetLegalMoves()
        {
            currentLegalMoves = RuleBook.GenerateLegalMoves(); // Get friendly moves
            // Backup check for mate as a fall through if it isnt detected in game control
            if (currentLegalMoves.Count == 0 && RuleBook.KingInCheck == true) // No legal moves and king is in check, checkmate
            {
                GameControl.EndGame(GameControl.OpposingSide()); // End game for opposite side it is to move
            }
            if (currentLegalMoves.Count == 0 && RuleBook.KingInCheck == false) // No legal moves and king isnt in check, stalemate
            {
                GameControl.EndGame(-1);
            }
        }
        public static void WhiteTimerTick(object sender, EventArgs e)
        {
            if (WhiteTimeLeft > 0) // If white time has not run out
            {
                WhiteTimeLeft -= 1; // Decrease time left (as it is called every sec this varibale will decrease by one every second meaning the millisecond the timer is on doesnt need to be used and converted)
                WhiteTimeDisplay.Text = TimeSpan.FromSeconds(WhiteTimeLeft).ToString(@"mm\:ss"); // update time display
            }
            // If no time left, end game and pass black as the winner
            else
            {
                GameControl.EndGame(8);
            }
        }
        public static void BlackTimerTick(object sender, EventArgs e)
        {
            if (BlackTimeLeft > 0) // If black time has not run out
            {
                BlackTimeLeft -= 1;  // Decrease time left (as it is called every sec this varibale will decrease by one every second meaning the millisecond the timer is on doesnt need to be used and converted)
                BlackTimeDisplay.Text = TimeSpan.FromSeconds(BlackTimeLeft).ToString(@"mm\:ss"); // update time display
            }
            // If no time left, end game and pass white as the winner
            else
            {
                GameControl.EndGame(16);
            }
        }

        // Create grid array and tile sizes
        readonly GameControl GameControl = new GameControl(); // For adding objects to 'controls'
        const int tileSize = 80; // 640 by 640 px
        int TileNo = 0;

        public static Label WinnerDisplay = new Label();

        // Colours for board squares
        public static Color Brown = ColorTranslator.FromHtml("#b0722c");
        public static Color White = ColorTranslator.FromHtml("#ede4d8");
        // Old colour variable for hovering over squares
        Color oldColor;

        public void DrawBoard()
        {
            // Set up winner display programatically
            WinnerDisplay = new Label();
            WinnerDisplay.Location = new Point(250, 400);
            WinnerDisplay.AutoSize = true;
            WinnerDisplay.ForeColor = Color.Red;
            WinnerDisplay.Font = new Font("Arial", 24);
            WinnerDisplay.Text = "Winner: ";

            Controls.Add(WinnerDisplay);
            // Hide it until the end of the game
            WinnerDisplay.Hide();

            // Set name displays
            P1Name.Text = Player1Name;
            P2Name.Text = Player2Name;

            // Loop to 64 creating board square on each loop
            for (var row = 0; row < 8; row++)
            {
                for (var col = 0; col < 8; col++)
                {
                    // Create picture box for pieces
                    var newSquare = new BoardSquare
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point((tileSize * col) + 20, (tileSize * row) + 100),
                        AllowDrop = true,
                        SizeMode = PictureBoxSizeMode.CenterImage,
                        // BorderStyle = BorderStyle.FixedSingle, // Uncomment for grid squares to be more obvious
                        SquareNumber = TileNo,
                    };

                    // Make the a - h at bottom of board
                    if (row % 8 == 7)
                    {
                        var boardNotationLabel = new Label {
                            Text = Convert.ToChar(col + 97).ToString(), // Turn number to ascii than +97 to get the corresponding letter
                            AutoSize = true, 
                            Location = new Point((tileSize * col) + 21, (tileSize * row) + 180)
                        };
                        Controls.Add(boardNotationLabel);
                    }

                    // Make the 1 - 8 up side of board
                    if (col % 8 == 0)
                    {
                        var boardNotationLabel = new Label
                        {
                            Text = (8-row).ToString(), // Minus number from 8 as row numbers are inverted
                            AutoSize = true,
                            Location = new Point((tileSize * col)+5, (tileSize * row)+100)
                        };
                        Controls.Add(boardNotationLabel);
                    }

                    // Board Square numbers for testing - displays 0 to 63 for each board square to link up with console outputs
                    if(ShowBoardSquareNumbers == true)
                    {
                        Label lbl = new Label();
                        lbl.Parent = newSquare;
                        lbl.BackColor = Color.Transparent;
                        lbl.Text = newSquare.SquareNumber.ToString();
                    }

                    // add square to grid and assign place in board array
                    Controls.Add(newSquare);
                    GameControl.Board[TileNo] = newSquare;

                    // Colour grid accordingly
                    if (col % 2 == 0)
                    {
                        newSquare.BackColor = row % 2 != 0 ? Brown : White;
                    }
                    else
                    {
                        newSquare.BackColor = row % 2 != 0 ? White : Brown;
                    }

                    // Assign event handlers so pieces move smoothly
                    newSquare.MouseEnter += new EventHandler(MouseEnter);
                    newSquare.MouseLeave += new EventHandler(MouseLeave);
                    newSquare.MouseDown += new MouseEventHandler(MouseDown);
                    newSquare.MouseUp += new MouseEventHandler(MouseUp);

                    TileNo += 1;
                }
            }

            // Load board position
            try
            {
                FenStringUtility.LoadBoardFromFenString(FenStringUtility.InputedPostion);
            }
            catch
            {
                FenStringUtility.LoadBoardFromFenString(FenStringUtility.StartingPosition);
            }
            //FenStringUtility.GetFenStringFromCurrentBoard(Board.PieceBoard);

            Console.WriteLine("Finished initialising board"); // Debug

            RuleBook.FindDistanceFromEdges(); // Get distances before any move generation, call here as we only want to do it once
            currentLegalMoves = RuleBook.GenerateLegalMoves(); // Get friendly moves
        }

        public static void resetColours()// Reset colours so the availble move and move from / to highlights go away
        {
            for (int i = 0; i < 64; i++) // Loop board
            {
                // If row number is even, even numbers brown, if row number is odd, even numbers white
                if ((i / 8) % 2 == 0){GameControl.Board[i].BackColor = i % 2 == 0 ? White : Brown;}
                if ((i / 8) % 2 != 0){GameControl.Board[i].BackColor = i % 2 == 0 ? Brown : White;}
            }
            if(RuleBook.KingInCheck == true)
            {
                GameControl.Board[RuleBook.FriendlyKingSquare].BackColor = Color.Red; // set king square bg colour to red if there is a check
            }
        }

        // Save cursor so pieces move smoothly
        Cursor Pointer = Cursor.Current;
        // Make copied piece and old location variables
        public int copiedPiece = 0;
        public int oldLocation = 0;

        new void MouseEnter(object sender, EventArgs e) // Called when mouse entered a grid square
        {
            //Console.WriteLine("Mouse Entered Square"); // debug

            BoardSquare currentPictureBox = (BoardSquare)sender; // Set object
            int location = currentPictureBox.SquareNumber; // Get location

            if (copiedPiece != 0) // If there is a piece that has been grabbed
            {
                Move[] moves = currentLegalMoves.ToArray();  // Set as an array otherwise "changed during execution" error will occur

                bool moveMade = false;
                Move AttemptingMove = new Move { Piece = copiedPiece, MoveFrom = oldLocation, MoveTo = location }; // Save move that is being attempted to check if it is legal

                foreach (Move move in moves) // Loop through moves
                {
                    if (move.MoveFrom == AttemptingMove.MoveFrom && move.MoveTo == AttemptingMove.MoveTo) // Check if the move the player is trying is within the legal move list
                    {
                        GameControl.Move(move); // If so, make the move
                        moveMade = true;
                    }
                }
                if (moveMade == false) // If not, don't allow
                {
                    // Play illegal move sound
                    System.IO.Stream stream = Properties.Resources.IllegalMove;
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(stream);
                    player.Play();

                    // Add the piece to the old location
                    GameControl.AddPiece(copiedPiece, oldLocation);
                }

                // Reset the copied piece for the next move
                copiedPiece = 0;
            }

            // Save old colour for higlighting squares
            oldColor = currentPictureBox.BackColor;
            currentPictureBox.BackColor = Color.LightYellow;
        }
        new void MouseLeave(object sender, EventArgs e) // Called when mouse entered a leaves a grid square
        {
            BoardSquare currentPictureBox = (BoardSquare)sender; // Set object
            // Console.WriteLine("Mouse Left Square \n");

            // Set back colour back to origonal
            currentPictureBox.BackColor = oldColor;
        }
        new void MouseDown(object sender, EventArgs e) // called if mouse if clicked within grid square
        {
            BoardSquare currentPictureBox = (BoardSquare)sender; // Set oject
            // Console.WriteLine("Mouse Down");
            int location = currentPictureBox.SquareNumber; // save location
            oldLocation = location;
            resetColours(); // Reset colours so available moves go

            // So back colour doesnt stay yellow
            currentPictureBox.BackColor = oldColor;

            // Check if there is an image to copy
            if (currentPictureBox.Image != null)
            { 
                // Set cursor to piece image
                Bitmap bmp = (Bitmap)currentPictureBox.Image;
                this.Cursor = new Cursor(bmp.GetHicon());

                copiedPiece = GameControl.Board[location].PieceOnSquare; // Save the piece to print later into another box

                GameControl.RemovePiece(copiedPiece, location); // remove the piece from the board array
                PieceLocator.RemoveFromList(location, copiedPiece); // remove the piece from the location lists
            }

            // Display all legal moves a piece has
            foreach(Move move in currentLegalMoves)
            {
                if(move.MoveFrom == location)
                {
                    GameControl.Board[move.MoveTo].BackColor = Color.Crimson;
                }
            }
        }
        new void MouseUp(object sender, EventArgs e) // Mouse click lifted in box
        {
            BoardSquare currentPictureBox = (BoardSquare)sender; // Set object
            // Console.WriteLine("Mouse Up");
            int location = currentPictureBox.SquareNumber;

            // If mouse still in same picutre box when mouse lifted, player no longer wants to move that piece
            if (currentPictureBox.ClientRectangle.Contains(currentPictureBox.PointToClient(Control.MousePosition)) & copiedPiece != 0) // Check if mouse is still in same picture box
            {
                // Add piece to same location
                GameControl.AddPiece(copiedPiece, location);
                copiedPiece = 0; // Reset copied piece
                //resetColours();
            }

            // Change to normal mouse pointer again
            this.Cursor = Pointer;
        }

        private void button1_Click(object sender, EventArgs e) // resign button
        {
            GameControl.EndGame(GameControl.OpposingSide()); // End game for opposing side
        }
    }
}
