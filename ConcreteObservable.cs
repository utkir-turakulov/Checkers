using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Сheckers
{
    class ConcreteObservable : IObservable
    {
        private List<IObserver> observers;

        public ConcreteObservable()
        {
            observers = new List<IObserver>();
        }

        public void AddObserver(IObserver o)
        {
            observers.Add(o);
        }

        public void NotifyObservers(string message)
        {
           foreach(IObserver observer in observers)
            {
                observer.Update(message);
            }
        }

        public void RemoveObserver(IObserver o)
        {
            observers.Remove(o);
        }
    }
}
