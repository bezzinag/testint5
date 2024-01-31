using DataLayer; 
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PresentationLayer.Presentation;

namespace PresentationLayer
{
    public enum GameState
    {
        Initial,
        PlayersAdded,
        Player1ConfiguringShips,
        Player2ConfiguringShips,
        ShipsConfigured,
    }
    public class Presentation
    {
        public int currentGameId;
        private List<string> players = new List<string>();

        private GameState gameState = GameState.Initial;
        private Logic logic = new Logic();
        
        public Presentation() 
        {

        }

        public void Start()
        {
            Console.Clear();
            
            ShowMenu();
        }
        public void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to Battleship!");
                Console.WriteLine("Please select an option:");

                switch (gameState)
                {
                    case GameState.Initial:
                        Console.WriteLine("1. Add Player Details");
                        Console.WriteLine("4. Quit");
                        break;
                    case GameState.PlayersAdded:
                        Console.WriteLine("2. Configure Ships");
                        Console.WriteLine("4. Quit");
                        break;
                    case GameState.ShipsConfigured:
                        Console.WriteLine("3. Launch Attack");
                        Console.WriteLine("4. Quit");
                        break;
                }
                string userInput = Console.ReadLine();

                /*if (userInput == "1" && gameState == GameState.PlayersAdded) 
                {

                }*/
                switch (userInput)
                {
                    case "1":
                        if (gameState == GameState.Initial)
                        {
                            addPlayer();
                            addtitle();
                            gameState = GameState.PlayersAdded;
                        }
                        else
                        {
                            Console.WriteLine("Invalid option.");
                        }
                        break;

                    case "2":
                        if (gameState == GameState.PlayersAdded)
                        {
                            PlayerShipchoice();
                            gameState = GameState.ShipsConfigured; 
                        }
                       
                        else
                        {
                            Console.WriteLine("Invalid option.");
                        }
                        break;

                    case "3":
                        if (gameState == GameState.ShipsConfigured)
                        {
                           LaunchAttack();
                        }
                        else
                        {
                            Console.WriteLine("Invalid dadada.");
                        }
                        break;

                    case "4":
                        Quit();
                        return;

                    default:
                        Console.WriteLine("Invalid option, try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
        private void Quit()
        {
            Console.WriteLine("Thank you for playing Battleship!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private void addPlayer()
        {
            players.Clear();
            for (int i = 1; i <= 2; i++) 
            {
                Console.Clear();
                Console.WriteLine($"Add Player {i} details:");
                Console.WriteLine("Please enter a username:");
                string username = Console.ReadLine();

                if (logic.checkifplayerexists(username))
                {
                    while (true)
                    {
                        Console.WriteLine($"Welcome back {username}, Please enter your password or press Esc to exit:");
                        var keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Escape)
                        {
                            Quit();
                            return;
                        }

                       

                        string password = keyInfo.KeyChar + Console.ReadLine();
                       
                        if (logic.confirmpassword(username, password))
                        {
                            Console.WriteLine("Login successful");
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Password incorrect. Please try again.");
                        }
                    }
                players.Add(username);
                }
                else
                {

                    Console.WriteLine("Username not found.");
                    Console.WriteLine("Please enter a password to sign up.");
                    string password = Console.ReadLine();
                    logic.addplayer(username, password);
                    Console.WriteLine("Player has been successfully added to the database!");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private void addtitle()
        {
            // Check and handle ongoing games
            if(!HandleOngoingGames())
            {
                // Proceed with adding a new game
                Console.WriteLine("Please enter the title of the game:");
                string title = Console.ReadLine();
                // Store the returned game ID in currentGameId
                currentGameId = logic.addgame(title, false, players[0], players[1]);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
        private bool HandleOngoingGames()
        {
            var ongoingGames = logic.GetOngoingGames();
            if (ongoingGames.Any())
            {
                bool userselectedgame = false;
                do
                {
                    Console.Clear();
                    foreach (var game in ongoingGames)
                    {
                        Console.WriteLine($"Game ID: {game.ID} {game.Player} VS {game.Player1}");
                    }
                    Console.WriteLine("Select Game:");
                    string userInput = Console.ReadLine();
                    foreach (var game in ongoingGames)
                    {
                        if (userInput == game.ID.ToString())
                        {
                            Console.WriteLine("Game Selected!");
                            userselectedgame = true;
                            currentGameId = game.ID;
                            break;
                        }
                    }
                } while (!userselectedgame);
            }
            else
            {
                return false;
            }
            return true;
        }


        public void PlayerShipchoice()
        {
            foreach (var plr in players)
            {
                GameScreen gameScreen = new GameScreen(new List<GameScreen.Cell>());
                while (true)
                {
                    gameScreen.printgrid();
                    var unconfiguredShips = logic.GetUnconfiguredShips(currentGameId, plr);
                    if (!unconfiguredShips.Any())
                    {
                        Console.WriteLine("All ships have been configured!");
                        Console.WriteLine($"Congratulations, {plr} all your ships have been placed.");
                        Console.ReadKey();
                        break;
                        // Move onto player 2 or onto menu 3
                    }

                    Console.Clear();
                    Console.WriteLine($"{plr}, this is your board:");
                    gameScreen.printgrid();

                    foreach (var ship in unconfiguredShips)
                    {
                        Console.WriteLine($"ID: {ship.ID}, Name: {ship.Title}, Size: {ship.Size}");
                    }

                    int shipId = 0;
                    
                    bool shipValid = false;
                    do
                    {
                        Console.WriteLine("Please select a ship by ID:");
                        string userInput = Console.ReadLine();
                        foreach (var ship in unconfiguredShips)
                        {
                            if (userInput == ship.ID.ToString())
                            {
                                Console.WriteLine("Ship Selected!");
                                shipId = ship.ID;
                                shipValid = true;
                                break;
                            }
                        }
                        if(shipValid == false)
                        {
                            Console.WriteLine("Invalid ship ID. Please enter a valid ID:");
                        }
                    } while (!shipValid);

                    bool validPlacement = false;
                    while (!validPlacement)
                    {
                        char orientation;
                        do
                        {
                            Console.WriteLine("Enter orientation (V for vertical, H for horizontal):");
                            orientation = Console.ReadLine().ToUpper()[0];
                        } while (orientation != 'V' && orientation != 'H');

                        char startRowChar;
                        int startRow;
                        do
                        {
                            Console.WriteLine("Enter starting row (A, B, C, etc.):");
                            startRowChar = Console.ReadLine().ToUpper()[0];
                            startRow = startRowChar - 'A' + 1; // +1 is removed
                        } while (startRow < 1 || startRow > GameScreen.height); // Adjusted condition

                        int startColumn;
                        do
                        {
                            Console.WriteLine("Enter starting column (1, 2, 3, etc.):");
                        } while (!int.TryParse(Console.ReadLine(), out startColumn) || startColumn < 1 || startColumn > GameScreen.width); //why startcolumn < 1?//

                        string coordinate = ConvertToCoordinateString(startRow, startColumn);
                        if (CanPlaceShip(gameScreen, shipId, orientation, startRow, startColumn, plr))
                        {
                            validPlacement = true;
                            Ship selectedShip = logic.GetShipById(shipId);
                            
                            for (int i=0; i<selectedShip.Size; i++)
                            {
                                int adjustedRow = startRow - 1; // Adjust for zero-based indexing
                                int adjustedColumn = startColumn - 1; // Adjust for zero-based indexing

                                if (orientation == 'H')
                                {
                                    // For horizontal placement, increment the column
                                    gameScreen.PlaceShipInGrid(adjustedRow, adjustedColumn + i, selectedShip.Size, orientation);
                                }
                                else if (orientation == 'V')
                                {
                                    // For vertical placement, increment the row
                                    gameScreen.PlaceShipInGrid(adjustedRow + i, adjustedColumn, selectedShip.Size, orientation);
                                }
                            }
                            gameScreen.PlaceShipInGrid(startRow, startColumn, selectedShip.Size, orientation);
                            logic.MarkShipAsConfigured(shipId, currentGameId, plr, coordinate);

                            // Re-fetch the list of unconfigured ships
                            unconfiguredShips = logic.GetUnconfiguredShips(currentGameId, plr);

                            Console.Clear();
                            Console.WriteLine("Ship placed successfully. Here's the updated board:");
                            gameScreen.printgrid();
                        }
                        else
                        {
                            Console.WriteLine("You cannot overlay ships, select different coordinates.");
                        }
                    }
                }
            }
            Console.WriteLine("All ships have been placed!");
            Console.ReadKey();
        }

        public void LaunchAttack()
        {
            while (gameState == GameState.ShipsConfigured)
            {
                if (players.Count < 2)
                {
                    Console.WriteLine("Error: Not enough players to start the attack.");
                    return; // Exit the method early
                }

                for (int i=0; i<2; i++)
                {
                    Console.Clear();//fix the below line
                    Console.WriteLine($"{players[i]} it's your turn to attack.");

                    GameScreen gameScreen = new GameScreen(new List<GameScreen.Cell>());
                    gameScreen.printgrid();
                    // Get Row for Attack
                    int startRow;
                    do
                    {
                        Console.WriteLine("Enter the row letter for your attack (A to G):");
                        char rowChar = Console.ReadLine().ToUpper()[0];
                        startRow = rowChar - 'A' + 1;
                        if (startRow < 1 || startRow > GameScreen.height)
                        {
                            Console.WriteLine("Invalid row. Please try again.");
                        }
                    } while (startRow < 1 || startRow > GameScreen.height);

                    // Get Column for Attack
                    int startColumn;
                    do
                    {
                        Console.WriteLine("Enter the column number for your attack (1 to 8):");
                        if (!int.TryParse(Console.ReadLine(), out startColumn) || startColumn < 1 || startColumn > GameScreen.width)
                        {
                            Console.WriteLine("Invalid column. Please try again.");
                        }
                    } while (startColumn < 1 || startColumn > GameScreen.width);


                    // Process the attack
                    if (i == 0)
                    {
                        ProcessAttack(startRow, startColumn, players[1], gameScreen); //the opponent is the opposite of the current iteration
                    }
                    else
                    {
                        ProcessAttack(startRow, startColumn, players[0], gameScreen);
                    }
                        
                }

            }
        }
        
        private void ProcessAttack(int startRow, int startColumn, string targetPlayer, GameScreen gameScreen)
        {
            // Convert row and column to coordinate string
            string coordinates = ConvertToCoordinateString(startRow, startColumn);

            // Check if the attack hits any of the opponent's ships
            bool attackResult = logic.ProcessPlayerAttack(currentGameId, targetPlayer, coordinates);

            // Update the game screen based on the attack result
            gameScreen.MarkAttack(startRow, startColumn, attackResult);
            if (attackResult)
            {
                Console.WriteLine("Hit!");
            }
            else
            {
                Console.WriteLine("Miss!");
            }
        }
        
        private string ConvertToCoordinateString(int startRow, int startColumn)
        {
            char rowChar = (char)('A' + startRow - 1); // Adjusted for zero-based indexing
            return $"{rowChar}{startColumn}";
        }

        private bool CanPlaceShip(GameScreen gameScreen, int shipId, char orientation, int startRow, int startColumn, string playerUsername)
        {
            Ship selectedShip = logic.GetShipById(shipId);
            if (selectedShip == null || selectedShip.IsConfigured())
            {
                return false;
            }

            return gameScreen.IsValidPlacement(selectedShip, orientation, startRow, startColumn, playerUsername);
        }
        


        public class GameScreen
        {
            public const int width = 8;  // Width of the game grid
            public const int height = 7; // Height of the game grid
            private Dictionary<string, List<Cell>> playerGrids = new Dictionary<string, List<Cell>>();
            private List<Cell> cells;
            public GameScreen(List<Cell> cells)
            {
                this.cells = cells;
            }
            public void ClearGrid()
            {
                cells.Clear();
            }
            public void InitializePlayerGrid(string playerUsername)
            {
                if (!playerGrids.ContainsKey(playerUsername))
                {
                    playerGrids[playerUsername] = new List<Cell>();
                }
            }

            // Gets the grid for a specific player
            public List<Cell> GetPlayerGrid(string playerUsername)
            {
                if (playerGrids.ContainsKey(playerUsername))
                {
                    return playerGrids[playerUsername];
                }
                return null; // or consider throwing an exception if the username is not found
            }
            public void printgrid()
            {
                char startLetter = 'A';

                // Print the top header numbers
                Console.Write("  ");
                for (int num = 1; num <= width; num++)
                {
                    Console.Write(num + " ");
                }
                Console.WriteLine();

                // Print the grid
                for (int row = 0; row < height; row++)
                {
                    // Print the row letter
                    Console.Write((char)(startLetter + row) + " ");

                    for (int col = 0; col < width; col++)
                    {
                        var cell = cells.FirstOrDefault(c => c.Row == row && c.Column == col);
                        if (cell != null)
                        {
                            cell.PrintCell();
                        }
                        else
                        {
                            Console.Write(". ");
                        }
                    }

                    Console.WriteLine();

                }
                Console.ReadKey();
            }
            public abstract class Cell
            {
                public int Row { get; set; }
                public int Column { get; set; }
                public abstract void PrintCell();
            }

            public void MarkAttack(int row, int column, bool isHit)
            {
                cells.Add(new AttackCell { Row = row, Column = column, IsHit = isHit});
            }
        
        public class ShipCell : Cell
            {
                public bool IsHit { get; set; } // Indicates if the ship cell is hit
                public override void PrintCell()
                {
                    Console.Write(IsHit ? "X " : "S ");//if ishit is true print x else print s
                }
            }
            public class AttackCell : Cell
            {
                public bool IsHit { get; set; } // Determines if the attack was successful

                public override void PrintCell()
                {
                    Console.Write(IsHit ? "X " : "O ");
                }
            }
            private bool IsCellEmpty(int row, int column, List<Cell> playerGrid)
            {
                // Check if the cell at the given row and column is empty in the specified player's grid
                return !playerGrid.Any(c => c.Row == row && c.Column == column);
            }

            public void PlaceShipInGrid(int row, int column, int shipSize, char orientation)
            {
                for (int i = 0; i < shipSize; i++)
                {
                    int adjustedRow = row - 1; // Adjust for zero-based indexing
                    int adjustedColumn = column - 1; // Adjust for zero-based indexing

                    if (orientation == 'H')
                    {
                        // For horizontal placement, increment the column
                        cells.Add(new ShipCell { Row = adjustedRow, Column = adjustedColumn + i });
                    }
                    else if (orientation == 'V')
                    {
                        // For vertical placement, increment the row
                        cells.Add(new ShipCell { Row = adjustedRow + i, Column = adjustedColumn });
                    }
                }
            }
            public bool IsValidPlacement(Ship ship, char orientation, int startRow, int startColumn, string playerUsername)
            {
                List<Cell> playerGrid = GetPlayerGrid(playerUsername);
                if (playerGrid == null)
                {
                    Console.WriteLine("Error: Player grid not found.");
                    return false;
                }

                int shipSize = ship.Size;
                int adjustedStartRow = startRow - 1;
                int adjustedStartColumn = startColumn - 1;

                if (orientation == 'H' && (adjustedStartColumn + shipSize) > width) return false;
                if (orientation == 'V' && (adjustedStartRow + shipSize) > height) return false;

                for (int i = 0; i < shipSize; i++)
                {
                    int checkRow = orientation == 'V' ? adjustedStartRow + i : adjustedStartRow;
                    int checkColumn = orientation == 'H' ? adjustedStartColumn + i : adjustedStartColumn;
                    if (!IsCellEmpty(checkRow, checkColumn, playerGrid))
                    {
                        return false; // Collision with another ship
                    }
                }

                return true; // No collisions
            }



        }
    }
}




           
    
