using DataLayer; 
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PresentationLayer.Presentation;
using static PresentationLayer.Presentation.GameScreen;

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

        private GameScreen player1GameScreen = new GameScreen(new List<GameScreen.Cell>());
        private GameScreen player2GameScreen = new GameScreen(new List<GameScreen.Cell>());



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
            // Instantiate game screens before initializing player grids
            if (player1GameScreen == null)
            {
                player1GameScreen = new GameScreen(new List<GameScreen.Cell>());
            }
            if (player2GameScreen == null)
            {
                player2GameScreen = new GameScreen(new List<GameScreen.Cell>());
            }
            players.Clear();
            for (int i = 1; i <= 2; i++) 
            {
                Console.Clear();
                Console.WriteLine($"Add Player {i} details:");
                Console.WriteLine("Please enter a username:");
                string username = Console.ReadLine();
                player1GameScreen.InitializePlayerGrid(username);

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
                player1GameScreen = new GameScreen(new List<GameScreen.Cell>());
                player2GameScreen = new GameScreen(new List<GameScreen.Cell>());
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
            // Assuming player1GameScreen and player2GameScreen are initialized and available at class level
            for (int i = 0; i < players.Count; i++)
            {
                var currentPlayer = players[i];
                // Select the correct GameScreen instance for the current player
                var currentGameScreen = (i == 0) ? player1GameScreen : player2GameScreen;

                while (true)
                {
                    currentGameScreen.printgrid();
                    var unconfiguredShips = logic.GetUnconfiguredShips(currentGameId, currentPlayer);
                    if (!unconfiguredShips.Any())
                    {
                        Console.WriteLine("All ships have been configured!");
                        Console.WriteLine($"Congratulations, {currentPlayer} all your ships have been placed.");
                        Console.ReadKey();
                        break;
                        // Move onto player 2 or onto menu 3
                    }

                    Console.Clear();
                    Console.WriteLine($"{currentPlayer}, this is your board:");
                    currentGameScreen.printgrid();

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
                        if (!shipValid)
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
                            startRow = startRowChar - 'A' + 1;
                        } while (startRow < 1 || startRow > GameScreen.height);

                        int startColumn;
                        do
                        {
                            Console.WriteLine("Enter starting column (1, 2, 3, etc.):");
                        } while (!int.TryParse(Console.ReadLine(), out startColumn) || startColumn < 1 || startColumn > GameScreen.width);

                        string coordinate = ConvertToCoordinateString(startRow, startColumn);

                        // Retrieve the player's grid for the current player
                        List<Cell> currentPlayerGrid = currentGameScreen.GetPlayerGrid(currentPlayer);

                        if (CanPlaceShip(currentGameScreen, shipId, orientation, startRow, startColumn, currentPlayer))
                        {
                            validPlacement = true;
                            Ship selectedShip = logic.GetShipById(shipId);

                            // Correctly pass the player's grid to the PlaceShipInGrid method
                            currentGameScreen.PlaceShipInGrid(startRow, startColumn, selectedShip.Size, orientation, currentPlayerGrid);

                            logic.MarkShipAsConfigured(shipId, currentGameId, currentPlayer, coordinate);

                            // Re-fetch the list of unconfigured ships
                            unconfiguredShips = logic.GetUnconfiguredShips(currentGameId, currentPlayer);

                            Console.Clear();
                            Console.WriteLine("Ship placed successfully. Here's the updated board:");
                            currentGameScreen.printgrid();
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
            if (selectedShip == null) return false;

            int shipSize = selectedShip.Size;
            // Check boundaries
            if (orientation == 'H' && startColumn + shipSize > GameScreen.width) return false;
            if (orientation == 'V' && startRow + shipSize > GameScreen.height) return false;

            // Retrieve the correct player grid for overlap checks
            List<Cell> currentPlayerGrid = gameScreen.GetPlayerGrid(playerUsername);

            // Check for overlap using the correct player grid
            for (int i = 0; i < shipSize; i++)
            {
                int checkRow = (orientation == 'V') ? startRow + i : startRow;
                int checkColumn = (orientation == 'H') ? startColumn + i : startColumn;

                // Pass the currentPlayerGrid to IsCellEmpty
                if (!gameScreen.IsCellEmpty(checkRow, checkColumn, currentPlayerGrid)) return false;
            }

            return true; // No boundary issues or overlaps found
        }



        public class GameScreen
        {
            public const int width = 8;  // Width of the game grid
            public const int height = 7; // Height of the game grid
            public Dictionary<string, List<Cell>> playerGrids = new Dictionary<string, List<Cell>>();
            public List<Cell> cells;
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
                    playerGrids.Add(playerUsername, new List<Cell>());
                }
            }

            // Gets the grid for a specific player
            public List<Cell> GetPlayerGrid(string playerUsername)
            {
                if (!playerGrids.ContainsKey(playerUsername))
                {
                    // Initialize an empty list for the player if not found
                    playerGrids[playerUsername] = new List<Cell>();
                }
                return playerGrids[playerUsername];
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
                public bool IsOccupied { get; set; } // Indicates if the cell is occupied (part of a ship)
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
            public bool IsCellEmpty(int row, int column, List<Cell> playerGrid)
            {
                // Find the cell at the specified row and column in the player's grid.
                var cell = playerGrid.FirstOrDefault(c => c.Row == row && c.Column == column);

                // The cell is considered empty if it doesn't exist in the grid (null) or if it's not occupied.
                return cell == null || !cell.IsOccupied;
            }

            public void PlaceShipInGrid(int row, int column, int shipSize, char orientation, List<Cell> playerGrid)
            {
                for (int i = 0; i < shipSize; i++)
                {
                    int adjustedRow = orientation == 'V' ? row + i : row;
                    int adjustedColumn = orientation == 'H' ? column + i : column;

                    // Assuming ShipCell is a subclass of Cell that represents a cell occupied by a ship.
                    ShipCell shipCell = new ShipCell
                    {
                        Row = adjustedRow,
                        Column = adjustedColumn,
                        IsOccupied = true // Mark the cell as occupied
                    };

                    playerGrid.Add(shipCell); // Add the cell to the player's grid
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



           
    
