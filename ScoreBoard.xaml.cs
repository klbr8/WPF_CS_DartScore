using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFDartScoringApp
{
    /// <summary>
    /// Interaction logic for ScoreBoard.xaml
    /// </summary>
    public partial class ScoreBoard : UserControl
    {
        /// <summary>
        /// This event fires when the Accept Score button is clicked.
        /// </summary>
        public event EventHandler? AcceptClick;

        private int[] totalScores = new int[2]; // tracks each teams total score
        private bool tempScoreShowing = false; // tracks how the current score update/change should be hilited
        private int[] oldScore = new int[2]; // tracks where in the list of scores we should be drawing the scores for each team
        private bool[] isDoubledOn = new bool[2]; //starts as false and changes once, permanantly, to true when a double is scored for the first time, per team
        private bool isBustDart = false; // is set to true when the 01 type game score falls below 2 without a valid double out
        private int[,] marks = new int[2, 7]; // tracks the cricket marks from 0-3 for each number, for all completed rounds
        private int[,] tempmarks = new int[2, 7]; // tracks the current of cricket marks until the round is accepted


        /// <summary>
        /// Set to true if a player must double on to begin counting points in an 01 type game.
        /// </summary>
        public bool DoubleOn { get; set; }
        
        /// <summary>
        /// Which teams player is shooting the current round.
        /// </summary>
        public int CurrentTeam { get; set; }
        
        /// <summary>
        /// Is set to true when the Scoreboard should consider the current round complete and finalize the rounds scoring.
        /// </summary>
        public bool Accepted { get; set; }
        
        /// <summary>
        /// Tells the scoreboard what games rules are used to score.
        /// </summary>
        public GameSettings.GameType GameType { get; set; }
        
        /// <summary>
        /// Is set to true when the game has been won.
        /// </summary>
        public bool GameWon { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public ScoreBoard()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clears the Scoreboard of prior scores and prepares it for the new game.
        /// </summary>
        public void ResetScore()
        {
            switch (GameType) // Sets the score to match the game type and resets the tracking of cricket marks.
            {
                case GameSettings.GameType.Three01:
                    totalScores[0] = 301;
                    totalScores[1] = 301;
                    GameName.Text = "301";
                    break;
                case GameSettings.GameType.Five01:
                    totalScores[0] = 501;
                    totalScores[1] = 501;
                    GameName.Text = "501";
                    break;
                case GameSettings.GameType.Six01:
                    totalScores[0] = 601;
                    totalScores[1] = 601;
                    GameName.Text = "601";
                    break;
                case GameSettings.GameType.Eight01:
                    totalScores[0] = 801;
                    totalScores[1] = 801;
                    GameName.Text = "801";
                    break;
                case GameSettings.GameType.Cricket:
                    totalScores[0] = 0;
                    totalScores[1] = 0;
                    GameName.Text = "Cricket";
                    for (int x = 0; x < 2; x++)
                    {
                        for (int y = 0; y < 7; y++)
                        {
                            marks[x, y] = 0;
                            tempmarks[x, y] = 0;
                        }
                    }
                    break;
                default:
                    break;
            }
            // Next 6 lines clear the scoreboard of previous game score and marks
            lbRoundScores1.Items.Clear(); 
            lbRoundScores2.Items.Clear();
            lbTotalScore1.Items.Clear();
            lbTotalScore2.Items.Clear();
            ClearTempMarks(spPlayer1Final);
            ClearTempMarks(spPlayer2Final);

            for (int i = 0; i < 2; i++)  // Do this block once for each team 
            {
                isDoubledOn[i] = false;  // initial setting
                oldScore[i] = 0;    // initial setting
                TextBox rtb = new(); // rtb and ttb will be added to the listboxes that hold round score and total score for each team.
                TextBox ttb = new(); // here they will be set to a blank and the initial score and the correct style.
                rtb.Text = "";
                ttb.Text = totalScores[i].ToString();
                rtb.Style = (Style)FindResource("OldScoreStyle");
                ttb.Style = (Style)FindResource("HiliteScoreStyle");
                
                switch (i) // put rtb and ttb on the correct teams side.
                {
                    case 0:
                        lbRoundScores1.Items.Add(rtb);
                        lbTotalScore1.Items.Add(ttb);
                        break;
                    case 1:
                        lbRoundScores2.Items.Add(rtb);
                        lbTotalScore2.Items.Add(ttb);
                        break;
                    default:
                        break;
                }
            }
            btnAcceptScore.IsEnabled = true;
        }

        /// <summary>
        /// Exposes and switches the IsEnabled Property of the Accept button to the opposite state.
        /// </summary>
        public void ToggleAcceptButton()
        {
            btnAcceptScore.IsEnabled = !btnAcceptScore.IsEnabled;
        }

        /// <summary>
        /// Updates the score base on the round of darts passed in. 
        /// </summary>
        /// <param name="round"></param>
        public void Update(Round round)
        {
            int roundscore = 0;
            if (GameType != GameSettings.GameType.Cricket)
            // score 01 type game
            {
                bool isRoundDoubledOn = isDoubledOn[CurrentTeam];
                if (GameWon)
                {
                    GameWon = false;
                }
                foreach (var dart in round.Darts)
                {
                    roundscore += ((int)dart.Quality) * dart.Points;
                    if (totalScores[CurrentTeam] - roundscore == 0)
                    {
                        if (dart.Quality == Dart.Modifier.Double)
                        {
                            GameWon = true;
                        }
                    }
                    if (totalScores[CurrentTeam] - roundscore < 1)
                    {
                        if (!(dart.Quality == Dart.Modifier.Double))
                        {
                            isBustDart = true;
                        }
                    }
                    if (totalScores[CurrentTeam] == 1)
                    {
                        isBustDart = true;
                    }
                    if (DoubleOn && !isRoundDoubledOn)
                    {
                        if (dart.Quality == Dart.Modifier.Double)
                        {
                            isRoundDoubledOn = true;
                        }
                        else
                        {
                            roundscore = 0;
                        }
                    }
                }
                DrawScore(Accepted, roundscore);
                if (Accepted)
                {
                    if (isRoundDoubledOn)
                    {
                        isDoubledOn[CurrentTeam] = true;
                    }
                    Accepted = false;
                }
            }
            else
            // score cricket game
            {
                int OppTeam = CurrentTeam == 0 ? 1 : 0;
                int[] rm = [0, 0, 0, 0, 0, 0, 0];
                foreach (var dart in round.Darts)
                {
                    if (!(dart.Value == Dart.ScoreValue.NotCricket))
                    {
                        rm[(int)dart.Value] += (int)dart.Quality;
                    }
                }
                for (int i = 0; i < 7; i++)
                {
                    if (rm[i] > 0)
                    {
                        if ((rm[i] + marks[CurrentTeam, i]) > 3 && marks[OppTeam, i] < 3)
                        {
                            int extramarks = rm[i] + marks[CurrentTeam, i] - 3;
                            roundscore += extramarks * CricketValue(i);
                        }

                        if (marks[CurrentTeam, i] < 3)
                        {
                            tempmarks[CurrentTeam, i] = (rm[i] + marks[CurrentTeam, i]) < 4 ? rm[i] : (3 - marks[CurrentTeam, i]);
                            if (Accepted)
                            {
                                marks[CurrentTeam, i] += tempmarks[CurrentTeam, i];
                            }
                        }
                    }
                }
                CheckCricketWin();
                DrawScore(Accepted, roundscore);
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 7; y++)
                    {
                        tempmarks[x, y] = 0;
                    }
                }
                Accepted = false;
            }
        }

        /// <summary>
        /// Updates the score temporarily after each dart and permanantly after accept button click.
        /// </summary>
        /// <remarks>isFinal will be true after accept button is clicked.</remarks>
        /// <param name="isFinal"></param>
        /// <param name="rscore"></param>
        private void DrawScore(bool isFinal, int rscore)
        {
            ListBox RStempList = new();
            ListBox TStempList = new();
            TextBox tempBox;
            TextBox rtb = new();
            TextBox ttb = new();
            int tempScore;

            switch (CurrentTeam)
            {
                case 0:
                    RStempList = lbRoundScores1;
                    TStempList = lbTotalScore1;
                    break;
                case 1:
                    RStempList = lbRoundScores2;
                    TStempList = lbTotalScore2;
                    break;
                default:
                    break;
            }

            tempBox = (TextBox)TStempList.Items[oldScore[CurrentTeam]];
            if (tempScoreShowing)
            {
                rtb = (TextBox)RStempList.Items[oldScore[CurrentTeam] + 1];
                ttb = (TextBox)TStempList.Items[oldScore[CurrentTeam] + 1];
            }
            rtb.Text = rscore.ToString();
            if (GameType == GameSettings.GameType.Cricket)
            {
                tempScore = totalScores[CurrentTeam] + rscore;
                UpdateMarks(Accepted);
            }
            else
            {
                tempScore = totalScores[CurrentTeam] - rscore;
            }
            if (isBustDart)
            {
                ttb.Text = "Bust";
            }
            else
            {
                ttb.Text = tempScore.ToString();
            }
            rtb.Style = (Style)FindResource("OldScoreStyle");

            if (isFinal)
            {
                if (isBustDart)
                {
                    rtb.Text = "Bust";
                    ttb.Text = tempBox.Text;
                    isBustDart = false;
                }
                else
                {
                    totalScores[CurrentTeam] = tempScore;
                }
                ttb.Style = (Style)FindResource("HiliteScoreStyle");
                tempBox.Style = (Style)FindResource("OldScoreStyle");
                oldScore[CurrentTeam] += 1;
                tempScoreShowing = false;
            }
            else
            {
                if (!tempScoreShowing)
                {
                    RStempList.Items.Add(rtb);
                    TStempList.Items.Add(ttb);
                }
                ttb.Style = (Style)FindResource("TempScoreStyle");
                tempScoreShowing = true;
            }
        }


        /// <summary>
        /// Returns the point value of a cricket target.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int CricketValue(int value)
        {
            var retVal = value switch
            {
                0 => 20,
                1 => 19,
                2 => 18,
                3 => 17,
                4 => 16,
                5 => 15,
                6 => 25,
                _ => 0,
            };
            return retVal;
        }

        /// <summary>
        /// Updates the marks /, X, O that show how many times a cricket target has been hit.
        /// </summary>
        /// <param name="final"></param>
        private void UpdateMarks(bool final)
        {
            StackPanel stackPanel = new();
            if (final)
            {
                if (CurrentTeam == 0)
                {
                    stackPanel = spPlayer1Final;
                    ClearTempMarks(spPlayer1Temp);
                }
                else
                {
                    stackPanel = spPlayer2Final;
                    ClearTempMarks(spPlayer2Temp);
                }
            }
            else
            {
                if (CurrentTeam == 0)
                {
                    stackPanel = spPlayer1Temp;
                }
                else
                {
                    stackPanel = spPlayer2Temp;
                }
            }
            if (final)
            {
                for (int i = 0; i < 7; i++)
                {
                    // int marksCount = marks[CurrentTeam, i] > 3 ? (marks[CurrentTeam, i] + tempmarks[CurrentTeam, i]) : 0;
                    Border border = stackPanel.Children[i] as Border;
                    string markColor = final ? "White" : "Red";
                    string rsrc;
                    rsrc = marks[CurrentTeam, i] switch
                    {
                        1 => "F",
                        2 => "X",
                        3 => "O",
                        _ => "0",
                    };
                    if (rsrc != "0")
                    {
                        border.Background = (Brush)FindResource(markColor + rsrc);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    // int marksCount = marks[CurrentTeam, i] < 3 ? (marks[CurrentTeam, i] + tempmarks[CurrentTeam, i] - 3) : 0;
                    Border border = stackPanel.Children[i] as Border;
                    string markColor = final ? "White" : "Red";
                    string rsrc;
                    switch (marks[CurrentTeam, i])
                    {
                        case 0:
                            rsrc = tempmarks[CurrentTeam, i] switch
                            {
                                0 => "0",
                                1 => "F",
                                2 => "X",
                                3 => "O",
                                _ => "0",
                            };
                            break;
                        case 1:
                            rsrc = tempmarks[CurrentTeam, i] switch
                            {
                                0 => "0",
                                1 => "B",
                                2 => "O",
                                _ => "0",
                            };
                            break;
                        case 2:
                            rsrc = tempmarks[CurrentTeam, i] switch
                            {
                                0 => "0",
                                1 => "O",
                                _ => "0",
                            };
                            break;
                        default:
                            rsrc = "0";
                            break;
                    }
                    if (rsrc != "0")
                    {
                        border.Background = (Brush)FindResource(markColor + rsrc);
                    }
                }
            }
        }
        
        /// <summary>
        /// Clears the stackpanel of marks.
        /// </summary>
        /// <param name="stackPanel"></param>
        private static void ClearTempMarks(StackPanel stackPanel)
        {
            foreach (Border b in stackPanel.Children)
            {
                b.Background = Brushes.Transparent;
            }
        }

        /// <summary>
        /// Sets the Accepted property to true and fires the AcceptClick Event.
        /// </summary>
        /// <remarks>The main program should handle this event by checking for GameWon, then calling Update, resetting the dartboard, and changing to the next team/player.</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAcceptScore_Click(object sender, RoutedEventArgs e)
        {
            Accepted = true;
            AcceptClick?.Invoke(this, e);
        }

        /// <summary>
        /// Checks if current team has won a cricket game. Sets GameWon accordingly.
        /// </summary>
        private void CheckCricketWin()
        {
            bool canWin = false;
            int oppTeam = CurrentTeam == 0 ? 1 : 0;

            if (totalScores[CurrentTeam] >= totalScores[oppTeam])
            {
                canWin = true;
                for (int i = 0; i < 7; i++)
                {
                    if (marks[CurrentTeam, i] != 3)
                    {
                        canWin = false;
                        break;
                    }
                }
            }
            GameWon = canWin;
        }
    } 
}
