using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musiknotenspiel.Klassen
{
    internal class GameManager : IObservable<int>
    {
        private List<IObserver<int>> observers = new List<IObserver<int>>();
        private int score;

        public IDisposable Subscribe(IObserver<int> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber(observers, observer);
        }

        public void UpdateScore(int points)
        {
            score += points;
            if (score < 0)
                score = 0;
            Notify(score);
        }

        private void Notify(int score)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(score);
            }
        }
    }

    internal class Unsubscriber : IDisposable
    {
        private List<IObserver<int>> observers;
        private IObserver<int> observer;

        public Unsubscriber(List<IObserver<int>> observers, IObserver<int> observer)
        {
            this.observers = observers;
            this.observer = observer;
        }

        public void Dispose()
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }
    }
}