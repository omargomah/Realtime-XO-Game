using System;
using System.Collections.Generic;
using System.Text;

namespace XOGame.Shared
{
    public class Room
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Player> Players { get; set; } = new();
        public Game Game { get; set; } = new();
        /// <summary>
        /// Adds a new player to the game if capacity allows and the player is not already present.
        /// Assigns the first player as <c>XPlayer</c> and the second player as <c>OPlayer</c> (maximum 2 players).
        /// </summary>
        /// <param name="player">The <see cref="Player"/> object representing the player attempting to join.</param>
        /// <returns>
        /// <c>true</c> if the player was successfully added and assigned a role; 
        /// otherwise, <c>false</c> if the game is already full (2 players) or the player is already connected.
        /// </returns>
        public bool AddPlayer(Player player)
        {
            if (Players.Count < 2 && !Players.Any(x => x.ConnectionId == player.ConnectionId))
            {
                if (Players.Count == 0)
                    Game.XPlayer = player.ConnectionId;
                else
                    Game.OPlayer = player.ConnectionId;
                Players.Add(player);
                return true;
            }
            return false;
        }
    }
}
