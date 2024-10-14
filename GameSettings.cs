using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFDartScoringApp
{
    /// <summary>
    /// Tracks the various settings for a game of darts
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// Number of players on each team, typically 1, 2 or 3
        /// </summary>
        public int PlayersPerTeam { get; set; }

        /// <summary>
        /// Describes how the leg will be decided, a single win or 2 wins out of 3 possible matches
        /// </summary>
        public SetFormat Format { get; set; }

        /// <summary>
        /// Describes the game that will be played for each leg of the match
        /// </summary>
        public List<GameType> GameList { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GameSettings()
        {
            PlayersPerTeam = 2;
            Format = SetFormat.SingleGame;
            GameList = new List<GameType>();
            GameList.Add(GameType.Three01);
        }

        /// <summary>
        /// Constructor for a single game match up
        /// </summary>
        /// <param name="numberOfPlayers"></param>
        /// <param name="gameType"></param>
        public GameSettings(int numberOfPlayers, GameType gameType)
        {
            PlayersPerTeam = numberOfPlayers;
            Format = SetFormat.SingleGame;
            GameList = new List<GameType>();
            GameList.Add(gameType);
        }

        /// <summary>
        /// Constructor for a Best of Three match up
        /// </summary>
        /// <param name="numberOfPlayers"></param>
        /// <param name="gameType1"></param>
        /// <param name="gameType2"></param>
        /// <param name="gameType3"></param>
        public GameSettings(int numberOfPlayers, GameType gameType1, GameType gameType2, GameType gameType3)
        {
            PlayersPerTeam = numberOfPlayers;
            Format = SetFormat.BestOfThree;
            GameList = new List<GameType>();
            GameList.Add(gameType1);
            GameList.Add(gameType2);
            GameList.Add(gameType3);
        }

        /// <summary>
        /// Add a game to a match up. Shouldn't be needed.
        /// </summary>
        /// <param name="gameType"></param>
        public void AddGameType(GameType gameType)
        {
            if (Format == SetFormat.SingleGame)
            {
                if (GameList.Count > 0)
                {
                    MessageBox.Show("To add more games change format to Best of Three.");
                }
                else
                {
                    GameList.Add(gameType);
                }
            }
            else
            {
                if (GameList.Count < 3)
                {
                    GameList.Add(gameType);
                }
                else
                {
                    MessageBox.Show("Games List is full.");
                }
            }
        }

        /// <summary>
        /// Available game types 301, 501, 601, 801, Cricket and Choice (winner of 'shoot for core' chooses 01 or cricket
        /// </summary>
        public enum GameType
        {
            Three01,
            Five01,
            Six01,
            Eight01,
            Cricket,
            Choice
        }

        /// <summary>
        /// Describes whether a match is decide by a single win or 2 wins out of 3
        /// </summary>
        public enum SetFormat
        {
            SingleGame,
            BestOfThree
        }
    }
}
