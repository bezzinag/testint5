using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class Logic
    {
        
        private Data data = new Data();
        public Logic() { }

        public bool checkifplayerexists(string username)
        {
            var player = data.checkifplayerexists(username);
            if (player.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //method to check if password is correct from data layer
        public bool confirmpassword(string username, string password)
        {
            var player = data.confirmpassword(username, password);
            if (player.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
      
        
        public void addplayer(string username, string password)
        {
            data.addplayertodb(username, password);
        }
        public int addgame(string title, bool complete, string creator, string opponent)
        {
            // This will now return the game ID
            return data.addgametodb(title, complete, creator, opponent);
        }

        public List<Ship> getshiplistfromdb()
        {
            var ships = data.getshipsfromdb();
            List<Ship> shipList = new List<Ship>();
            foreach (var ship in ships)
            {
                shipList.Add(ship);
            }
            return shipList;
        }
        public Ship GetShipById(int shipId)
        {
            return data.GetShipById(shipId);
        }
        public List<Ship> GetUnconfiguredShips(int gameId, string playerUsername)
        {
            return data.GetUnconfiguredShips(gameId, playerUsername);
        }
        public void MarkShipAsConfigured(int shipId, int gameId, string playerUsername, string coordinate)
        {
            data.MarkShipAsConfigured(shipId, gameId, playerUsername, coordinate);
        }
        public List<Game> GetOngoingGames()
        {
            return data.GetOngoingGames();
        }
        public void MarkGameAsComplete(int gameId)
        {
            data.MarkGameAsComplete(gameId);
        }
        public void ClearGameShipConfigurations(int gameId)
        {
            data.ClearGameShipConfigurations(gameId);
        }

        //method to get player gameshipconfiguration from database by username and gameid from data layer
        public List<GameShipConfiguration> getplayergameshipconfiguration(string username, int gameid)
        {
            var playergameshipconfiguration = data.getplayergameshipconfiguration(username, gameid);
            List<GameShipConfiguration> playergameshipconfigurationList = new List<GameShipConfiguration>();//why do we need this?//ans//to convert the iqueryable to list
            foreach (var gameShipConfiguration in playergameshipconfiguration)
            {
                playergameshipconfigurationList.Add(gameShipConfiguration);
            }
            return playergameshipconfigurationList;
        }

        public bool ProcessPlayerAttack(int gameId, string playerUsername, string coordinate)
        {
            foreach (var gameShipConfiguration in getplayergameshipconfiguration(playerUsername, gameId))
            {
                if (gameShipConfiguration.Coordinate == coordinate)
                {
                    data.RecordAttack(gameId, playerUsername, coordinate, true);
                    return true;
                }
            }
            data.RecordAttack(gameId, playerUsername, coordinate, false);
            return false;
        }

    }
}

