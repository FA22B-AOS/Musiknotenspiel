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
        private MelodyManager melodyManager;

        private Melody currentMelody;
        private int currentNoteIndex;

        public MainWindow()
        {
            InitializeComponent();

            gameManager = new GameManager();
            midiPlayer = new MidiPlayer();
            midiPlayer.ChangeInstrument(1);
            melodyManager = new MelodyManager("melodies.csv");
            melodyManager.LoadFromFile();
            scoreDisplay = new ScoreDisplay(ScoreLabel);
            gameManager.Subscribe(scoreDisplay);

            InitializeMelody();
        }

        private void InitializeMelody()
        {
            Random random = new Random();
            currentMelody = melodyManager.Melodies[random.Next(0, melodyManager.MelodyCount)];
            Title.Text = currentMelody.Name;

            NotesDisplay.Children.Clear();

            currentNoteIndex = 0;

            foreach (var note in currentMelody.Notes)
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

                { "C2", 72 },
                { "C#2", 73 },
                { "D2", 74 },
                { "D#2", 75 },
                { "E2", 76 },
                { "F2", 77 },
                { "F#2", 78 },
                { "G2", 79 },
                { "G#2", 80 },
                { "A2", 81 },
                { "A#2", 82 },
                { "B2", 83 }
            };

            if (noteMapping.TryGetValue(pressedNote, out int midiNote))
            {
                await midiPlayer.PlayNoteAsync(midiNote);

                if (pressedNote == currentMelody.Notes[currentNoteIndex])
                {
                    HighlightCurrentNote();

                    currentNoteIndex++;
                    gameManager.UpdateScore(10);

                    if (currentNoteIndex >= currentMelody.Notes.Count)
                    {
                        MessageBox.Show("Bravo! Du hast die Melodie fertig gespielt!");
                        for (int i = 0; i < currentMelody.Notes.Count; i++)
                            ShiftNotes(-40);
                        InitializeMelody(); 
                    }
                }
                else
                {
                    gameManager.UpdateScore(-10);
                }
            }
            else
            {
                MessageBox.Show($"Unbekannte Note: {pressedNote}");
            }
        }

        private void ShiftNotes(double offsetX)
        {
            if (NotesDisplay.RenderTransform is TranslateTransform transform)
            {
                transform.X -= offsetX; 
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
                        ShiftNotes(40);
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