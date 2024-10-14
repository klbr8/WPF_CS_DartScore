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
using System.Windows.Shapes;

namespace WPFDartScoringApp
{
    /// <summary>
    /// Interaction logic for PracticeSetupDialog.xaml
    /// </summary>
    public partial class PracticeSetupDialog : Window
    {
        private bool isValid = false;

        public GameSettings.GameType GameType { get; set; }
        public int PlayersPerTeam { get; set; }
        public List<string> PlayerNames { get; set; } = [];


        public PracticeSetupDialog()
        {
            InitializeComponent();
        }

        private void CbPSPlayersPerTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CBPSPlayersPerTeam.SelectedIndex)
            {
                case 0:
                    PlayersPerTeam = 1;
                    tbT1P2.IsEnabled = false;
                    tbT2P2.IsEnabled = false;
                    tbT1P3.IsEnabled = false;
                    tbT2P3.IsEnabled = false;
                    break;
                case 1:
                    PlayersPerTeam = 2;
                    tbT1P2.IsEnabled = true;
                    tbT2P2.IsEnabled = true;
                    tbT1P3.IsEnabled = false;
                    tbT2P3.IsEnabled = false;
                    break;
                case 2:
                    PlayersPerTeam = 3;
                    tbT1P2.IsEnabled = true;
                    tbT2P2.IsEnabled = true;
                    tbT1P3.IsEnabled = true;
                    tbT2P3.IsEnabled = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Close form after validation 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            switch (CBPSPlayersPerTeam.SelectedIndex)
            {
                case 0:
                    isValid = (!tbT1P1.Text.Equals("") && !tbT2P1.Text.Equals(""));
                    break;
                case 1:
                    isValid = (!tbT1P2.Text.Equals("") && !tbT2P2.Text.Equals(""));
                    break;
                case 2:
                    isValid = (!tbT1P3.Text.Equals("") && !tbT2P3.Text.Equals(""));
                    break;
                default:
                    break;
            }
            if (isValid)
            {
                PlayerNames.Clear();
                PlayerNames.Add(tbT1P1.Text);
                PlayerNames.Add(tbT2P1.Text);
                PlayerNames.Add(tbT1P2.Text);
                PlayerNames.Add(tbT2P2.Text);
                PlayerNames.Add(tbT1P3.Text);
                PlayerNames.Add(tbT2P3.Text);
                GameType = CBPSGameType.SelectedIndex switch
                {
                    0 => GameSettings.GameType.Three01,
                    1 => GameSettings.GameType.Five01,
                    2 => GameSettings.GameType.Six01,
                    3 => GameSettings.GameType.Eight01,
                    4 => GameSettings.GameType.Cricket,
                    _ => GameSettings.GameType.Three01,
                };
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("You must fill in a name for each player in the match");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CBPSPlayersPerTeam.SelectedIndex = 0;
            CBPSGameType.SelectedIndex = 0;
        }

        private void CBPSGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CBPSGameType.SelectedIndex == 4)
            {
                MustDoubleOn.IsEnabled = false;
                MustDoubleOn.Foreground = Brushes.LightGray;
            }
            else
            {
                MustDoubleOn.IsEnabled = true;
                MustDoubleOn.Foreground = Brushes.Black;
            }
        }
    }
}
