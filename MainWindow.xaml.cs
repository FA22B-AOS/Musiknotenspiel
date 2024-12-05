using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Musiknotenspiel.Klassen;


namespace Musiknotenspiel
{
    
    public partial class MainWindow : Window
    {
        private GameManager gameManager;
        private ScoreDisplay scoreDisplay;
        private MidiPlayer midiPlayer;

        private List<string> currentMelody;
        private int currentNoteIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            gameManager = new GameManager();
            midiPlayer = new MidiPlayer();
            midiPlayer.ChangeInstrument(1);
            scoreDisplay = new ScoreDisplay(ScoreLabel);
            gameManager.Subscribe(scoreDisplay);

            InitializeMelody();
        }

        private void InitializeMelody()
        {
            currentMelody = new List<string> { "C", "E", "G", "C'", "C" };

            NotesDisplay.Children.Clear();

            foreach (var note in currentMelody)
            {
                var noteContainer = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                var noteEllipse = new Ellipse
                {
                    Width = 40,
                    Height = 40,
                    Fill = Brushes.Red,
                };

                var noteText = new TextBlock
                {
                    Text = note,
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, -40, 0, 0),
                };

                noteContainer.Children.Add(noteEllipse);
                noteContainer.Children.Add(noteText);

                NotesDisplay.Children.Add(noteContainer);
            }
        }


        private async void OnPianoKeyClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            string pressedNote = button.Tag.ToString();

            var noteMapping = new Dictionary<string, int>
            {
                { "C", 60 },
                { "C#", 61 },
                { "D", 62 },
                { "D#", 63 },
                { "E", 64 },
                { "F", 65 },
                { "F#", 66 },
                { "G", 67 },
                { "G#", 68 },
                { "A", 69 },
                { "A#", 70 },
                { "B", 71 },
                { "C'", 72 }
            };

            if (noteMapping.TryGetValue(pressedNote, out int midiNote))
            {
                await midiPlayer.PlayNoteAsync(midiNote);

                if (pressedNote == currentMelody[currentNoteIndex])
                {
                    HighlightCurrentNote();

                    currentNoteIndex++;
                    gameManager.UpdateScore(10);

                    if (currentNoteIndex >= currentMelody.Count)
                    {
                        MessageBox.Show("Bravo! Du hast die Melodie fertig gespielt!");
                        InitializeMelody(); 
                        currentNoteIndex = 0;
                    }
                }
                else
                {
                   // MessageBox.Show("Falscher Ton! Versuch es nochmal.");
                }
            }
            else
            {
                MessageBox.Show($"Unbekannte Note: {pressedNote}");
            }
        }

        private void HighlightCurrentNote()
        {
            if (currentNoteIndex < NotesDisplay.Children.Count)
            {
                var noteContainer = NotesDisplay.Children[currentNoteIndex] as StackPanel;

                if (noteContainer != null)
                {
                    var ellipse = noteContainer.Children.OfType<Ellipse>().FirstOrDefault();

                    if (ellipse != null)
                    {
                        ellipse.Fill = Brushes.LightGreen;
                    }
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            midiPlayer.Dispose();
        }
    }
}