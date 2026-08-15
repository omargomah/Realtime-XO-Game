using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace XOGame.Shared
{
    public class Game
    {
        public string XPlayer { get; set; }
        public string OPlayer { get; set; }
        public bool Start { get; set; }
        public bool Stop { get; set; }
        public bool Draw { get; set; }
        public bool GameOver { get; set; }
        public string CurrentPlayerRoundId { get; set; }
        public string WinnerPlayerId { get; set; }
        public string CurrentSymbolRound => CurrentPlayerRoundId == XPlayer ? "X" : "O";
        public List<List<string>> Board { get; set; } = new(3);
        public void StartNewGame()
        {
            Board.Clear();
            for (int i = 0; i < Board.Capacity; i++)
            {
                Board.Add(new(3));
                for (int j = 0; j < Board[i].Capacity; j++)
                    Board[i].Add(string.Empty);
            }
            Start = true; Stop = false; Draw = false; GameOver = false; CurrentPlayerRoundId = XPlayer;
        }
    }
}
