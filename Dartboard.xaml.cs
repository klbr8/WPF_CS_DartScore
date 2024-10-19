using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFDartScoringApp
{
    /// <summary>
    /// Interaction logic for Dartboard.xaml
    /// </summary>
    public partial class Dartboard : UserControl
    {
        /// <summary>
        /// Notifies the calling program that a dart was added to the round.
        /// </summary>
        public event EventHandler? DartsChanged;

        private bool editing = false; // Is set to true when an already placed dart is clicked again to remove it. Reset to false when the dart is placed again. Corrects location mistakes.
        private int editCount; // which dart in the CurrentRound is being edited/moved.
        private Ellipse[] marker = new Ellipse[3]; //visual for a scored dart location.

        /// <summary>
        /// Holds a list of up to 3 darts and is passed to the scoreboard.
        /// </summary>
        /// <remarks>The list is not limited to 3 internally. The calling code should track this. Will accomodate games that allow more than 3 darts in a round.</remarks>
        public Round CurrentRound { get; set; }

        /// <summary>
        /// When true the dartboard will accept a click and add a dart to itself and the current round.
        /// </summary>
        /// <remarks>In most games after the third dart has been thrown each round this is set to false, then back to true after the score is accepted and the board is cleared.</remarks>
        public bool CanScore { get; set; }

        /// <summary>
        /// Constructor - sets the properties of the 3 markers and adds them to the dartboard with visibility set to hidden. Initializes the CurrentRound Property.
        /// </summary>
        public Dartboard()
        {
            InitializeComponent();

            for (int i = 0; i < 3; i++)
            {
                marker[i] = new Ellipse
                {
                    Visibility = Visibility.Hidden,
                    Height = 10,
                    Width = 10,
                    Fill = Brushes.Blue,
                    Style = (Style)this.FindResource("EllipseStyle1"),
                    Tag = i + 1,
                    ToolTip = "Dart " + i + 1.ToString()
                };
                Board.Children.Add(marker[i]);
            }
            CurrentRound = new Round();
        }


        /// <summary>
        /// Hides the markers and clears the previous round of darts.
        /// </summary>
        public void ResetBoard()
        {
            foreach (Ellipse m in marker)
            {
                m.Visibility = Visibility.Hidden;
            }
            CurrentRound.ResetRound();
        }

        /// <summary>
        /// Shows a marker where the mouse is clicked and adds the dart to the CurrentRound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scoreclick(object sender, MouseButtonEventArgs e)
        {
            if (CanScore)
            {
                if (CurrentRound.Darts.Count < 3 || editing)
                {
                    Path path = (Path)e.OriginalSource; // reference the part of the dartboard that was clicked
                    Dart dart = new()
                    {
                        Quality = Modify(path.Name[..1]), // from that reference set Quality property to single, double or triple
                        Points = int.Parse(path.Name[1..]) // and how many points for a single
                    };
                    dart.Value = GetDartValue(dart.Points); // then set Value for cricket games (same but different)
                    if (editing) // if a marker was removed and is being placed again, edit the correct dart in the round i.e. first, second or third
                    {
                        CurrentRound.EditDart(editCount - 1, dart);
                        ShowMarker(editCount, e);
                        editing = false;
                    }
                    else // this is a new dart to be added to the end of the rounds list of darts
                    {
                        CurrentRound.AddDart(dart);
                        ShowMarker(CurrentRound.Darts.Count, e);
                    }
                    DartsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Decodes the integer parsed from the senders name that describes the point value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Dart.Scorevalue</returns>
        private static Dart.ScoreValue GetDartValue(int value)
        {
            var sv = value switch
            {
                15 => Dart.ScoreValue.Fives,
                16 => Dart.ScoreValue.Sixes,
                17 => Dart.ScoreValue.Sevens,
                18 => Dart.ScoreValue.Eights,
                19 => Dart.ScoreValue.Nines,
                20 => Dart.ScoreValue.Twenties,
                25 => Dart.ScoreValue.Cores,
                _ => Dart.ScoreValue.NotCricket,
            };
            return sv;
        }

        /// <summary>
        /// Removes an already place dart, allowing for fixing placement errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dotclick(object sender, MouseButtonEventArgs e)
        {
            if (!editing) //avoids removing more than one marker at a time.
            {
                FrameworkElement frameworkElement = (FrameworkElement)e.Source;
                frameworkElement.Visibility = Visibility.Hidden;
                editing = true;
                editCount = (int)frameworkElement.Tag;
                CurrentRound.EditDart(editCount - 1, new Dart());
                if (!CanScore)
                {
                    CanScore = true;
                }
                DartsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Moves the marker to location of the mouse click and set it to Visible.
        /// </summary>
        /// <param name="which"></param>
        /// <param name="e"></param>
        private void ShowMarker(int which, MouseEventArgs e)
        {
            Point p = e.GetPosition(Board);
            Canvas.SetLeft(marker[which - 1], p.X - 5);
            Canvas.SetTop(marker[which - 1], p.Y - 5);
            marker[which - 1].Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Decodes the char parsed from the senders name that describes single, double and triple targets.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Dart.Modifier Modify(string value)
        {
            var modifier = value switch
            {
                "S" => Dart.Modifier.Single,
                "D" => Dart.Modifier.Double,
                "T" => Dart.Modifier.Triple,
                _ => Dart.Modifier.Single,
            };
            return modifier;
        }
    }
}
