using System.Windows;
using System.Windows.Controls;

namespace Musiknotenspiel.Klassen
{
    internal class ScoreDisplay : IObserver<int>
    {
        private TextBlock scoreLabel;

        public ScoreDisplay(TextBlock label)
        {
            scoreLabel = label;
        }

        public void OnNext(int score)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                scoreLabel.Text = $"Punkte: {score}";
            });
        }

        public void OnError(Exception error)
        {

        }

        public void OnCompleted()
        {

        }
    }
}
