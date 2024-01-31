using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Data
    {
        

        public Data() { }



        public IQueryable<Player> checkifplayerexists(string username)
        {
         BattleshipDataContext db = new BattleshipDataContext(); 
       
                var player = db.Players.Where(p => p.Username == username);
                return player;
        }

        public IQueryable<Player> confirmpassword(string username, string password)
        {
            BattleshipDataContext db = new BattleshipDataContext();
            var player = db.Players.Where(p => p.Username == username && p.Password == password);
                return player;
        }
        public void addplayertodb(string username, string password)
        {
            BattleshipDataContext db = new BattleshipDataContext();

            Player player = new Player();
                player.Username = username;
                player.Password = password;
                db.Players.InsertOnSubmit(player);
                db.SubmitChanges();
        }
        public int addgametodb(string title, bool complete, string creator, string opponent)
        {
            BattleshipDataContext db = new BattleshipDataContext();

            Game game = new Game();
                game.Title = title;
                game.Complete = complete;
                game.CreatorFK = creator;
                game.OpponentFK = opponent;
                db.Games.InsertOnSubmit(game);
                db.SubmitChanges();

                // Return the ID of the newly created game
                return game.ID;
        }

        public IQueryable<Ship> getshipsfromdb()
        {
            BattleshipDataContext db = new BattleshipDataContext();

            var ships = db.Ships;
                return ships;
        }

        //need Iqueryable to get a player gameshipconfiguration from database by username and gameid//ans//
        public IQueryable<GameShipConfiguration> getplayergameshipconfiguration(string username, int gameid)
        {
            BattleshipDataContext db = new BattleshipDataContext();


            var playergameshipconfiguration = db.GameShipConfigurations.Where(p => p.PlayerFK == username && p.GameFK == gameid);
                return playergameshipconfiguration;

        }
       
       
        public Ship GetShipById(int shipId)
        {
            BattleshipDataContext db = new BattleshipDataContext();

            return db.Ships.FirstOrDefault(ship => ship.ID == shipId);

        }
        public List<Ship> GetUnconfiguredShips(int gameId, string playerUsername)
        {
            BattleshipDataContext db = new BattleshipDataContext();

            var unconfiguredShips = db.Ships
                .Where(ship => !db.GameShipConfigurations
                    .Any(conf => conf.ShipFK == ship.ID && conf.GameFK == gameId && conf.PlayerFK == playerUsername))
                .ToList();

                return unconfiguredShips;

        }

        //creates a coordinate for the ship
        public void MarkShipAsConfigured(int shipId, int gameId, string playerUsername, string coordinate)
        {
            BattleshipDataContext db = new BattleshipDataContext();

            GameShipConfiguration newConfig = new GameShipConfiguration
                {
                    ShipFK = shipId,
                    GameFK = gameId,
                    PlayerFK = playerUsername,
                    Coordinate = coordinate
                };

                db.GameShipConfigurations.InsertOnSubmit(newConfig);
                db.SubmitChanges();
        }
        public List<Game> GetOngoingGames()
        {
            BattleshipDataContext db = new BattleshipDataContext();

            return db.Games.Where(g => !g.Complete).ToList();

        }
        public void MarkGameAsComplete(int gameId)
        {
            BattleshipDataContext db = new BattleshipDataContext();

            var game = db.Games.FirstOrDefault(g => g.ID == gameId);
                if (game != null)
                {
                    game.Complete = true;
                    db.SubmitChanges();
                }

        }
        public void ClearGameShipConfigurations(int gameId)
        {
            BattleshipDataContext db = new BattleshipDataContext();
            var configurations = db.GameShipConfigurations.Where(config => config.GameFK == gameId);
                db.GameShipConfigurations.DeleteAllOnSubmit(configurations);
                db.SubmitChanges();
        }
        public Ship GetShipAtCoordinates(string playerUsername, string coordinates, int gameId)
        {
            BattleshipDataContext db = new BattleshipDataContext();

            // Assuming GameShipConfiguration links a Ship with its location for a specific game
            return db.GameShipConfigurations
                              .Where(c => c.PlayerFK == playerUsername && c.Coordinate == coordinates && c.GameFK == gameId)
                              .Select(c => c.Ship)
                              .FirstOrDefault();
        }

        public void RecordAttack(int gameId, string attackerUsername, string coordinates, bool isHit)
        {
            BattleshipDataContext db = new BattleshipDataContext();

                var attack = new Attack
                {
                    GameFK = gameId,
                    UserNameFK = attackerUsername,
                    Coordinate = coordinates,
                    Hit = isHit
                };
                db.Attacks.InsertOnSubmit(attack);
                db.SubmitChanges();

        }



    }
}