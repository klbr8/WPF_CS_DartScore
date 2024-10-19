namespace WPFDartScoringApp
{
    /// <summary>
    /// Holds a list of up to 3 darts. The Round class does not limit itself to holding only 3 darts. The calling code should be checking.
    /// </summary>
    public class Round
    {
        /// <summary>
        /// The list of darts.
        /// </summary>
        public List<Dart> Darts { get; set; }
        /// <summary>
        /// Name of the player that threw this round.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// How many darts are in the list of Darts.
        /// </summary>
        public int Count { get { return Darts.Count; } set { } }

        /// <summary>
        /// Clears/Empties the list of darts.
        /// </summary>
        public void ResetRound()
        {
            Darts.Clear();
        }

        /// <summary>
        /// Add a dart to the list.
        /// </summary>
        /// <param name="dart"></param>
        public void AddDart(Dart dart)
        {
            Darts.Add(dart);
        }

        /// <summary>
        /// Replace at dart at index(which) with a dart.
        /// </summary>
        /// <param name="which"></param>
        /// <param name="dart"></param>
        public void EditDart(int which, Dart dart)
        {
            Darts[which] = dart;
        }


        /// <summary>
        /// Remove the dart at index(which).
        /// </summary>
        /// <param name="which"></param>
        public void RemoveDart(int which)
        {
            Darts.RemoveAt(which);
        }


        /// <summary>
        /// Insert a dart at index(where) displace other darts down the list. Will simply add the dart if there isn't one or more darts at 'where'.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="dart"></param>
        public void InsertDart(int where, Dart dart)
        {
            if (where > Darts.Count)
            {
                Darts.Add(dart);
            }
            else
            {
                Darts.Insert(where, dart);
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Round()
        {
            Darts = new List<Dart>();
            PlayerName = "";
        }
    }
}
