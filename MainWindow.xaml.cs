using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] playernames = new string[6];

        public MainWindow()
        {
            InitializeComponent();
        }
        
        

        private void MenuItem_MatchSetup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Practice_Click(object sender, RoutedEventArgs e)
        {
            PracticeSetupDialog dialog = new();
            int index = 0;
            
            if (dialog.ShowDialog() == true)
            {
                foreach (string name in dialog.PlayerNames)
                {
                    playernames[index++] = name;
                } 
                sb.GameType = dialog.GameType;
                sb.CurrentTeam = 0;
                sb.ResetScore();
                db.ResetBoard();
                sb.DoubleOn = dialog.MustDoubleOn.IsChecked == true;
                db.CanScore = true;
                sb.txtCurrShooter.Text = playernames[sb.CurrentTeam];
            }
        }

        private void MenuItem_Reports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnDartsChanged(object sender, EventArgs e)
        {
            sb.Update(db.CurrentRound);
            if (sb.GameWon)
            {
                db.CanScore = false;
            }
        }

        private void OnAcceptClick(object sender, EventArgs e)
        {
            sb.Update(db.CurrentRound);
            sb.CurrentTeam = sb.CurrentTeam == 0 ? 1 : 0;
            sb.txtCurrShooter.Text = playernames[sb.CurrentTeam];
            db.CurrentRound.PlayerName = playernames[sb.CurrentTeam];

            db.ResetBoard();
            if (sb.GameWon)
            {
                sb.ToggleAcceptButton();
                MessageBox.Show("Winner!!!");
                db.ResetBoard();
            }
            
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // code below was for testing without using the practice setup dialog

            //sb.GameType = GameSettings.GameType.Cricket;
            //db.CanScore = true;
            //sb.CurrentTeam = 0;
            //sb.ResetScore();
            //sb.DoubleOn = false;
            //playernames[0] = "Team One Player";
            //playernames[1] = "Team Two Player";
            //sb.CurrentShooter = playernames[sb.CurrentTeam];
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // TODO - set this up to only fire if a match is incomplete
            MessageBoxResult result = MessageBox.Show("Are you sure?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel, MessageBoxOptions.None);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}