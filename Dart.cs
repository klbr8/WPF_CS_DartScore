namespace WPFDartScoringApp
{
    /// <summary>
    /// A simple class describing one dart in a dartboard
    /// </summary>
    public class Dart
    {
        /// <summary>
        /// The point value of the score eligible dart, unmodified by double or triple
        /// </summary>
        public int Points { get; set; } = 0;

        /// <summary>
        /// The cricket score value from the ScoreValue enumeration
        /// </summary>
        public ScoreValue Value { get; set; } = ScoreValue.NotCricket;

        /// <summary>
        /// Describes if the dart landed in a score modifier from the enumeration
        /// </summary>
        public Modifier Quality { get; set; } = Modifier.Single;

        /// <summary>
        /// Enumeration of dart score quality (multiplier)
        /// </summary>
        public enum Modifier
        {
            Single = 1,
            Double = 2,
            Triple = 3
        }

        /// <summary>
        /// Enumeration of valid cricket scores. 
        /// </summary>
        /// <remarks>NotCricket describes any dart that doesn't land in a scoring location</remarks>
        public enum ScoreValue
        {
            Twenties = 0,
            Nines = 1,
            Eights = 2,
            Sevens = 3,
            Sixes = 4,
            Fives = 5,
            Cores = 6,
            NotCricket = 7
        }
    }
}