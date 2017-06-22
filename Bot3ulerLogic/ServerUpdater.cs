using System;
using System.Collections.Generic;
using System.Text;

namespace Bot3ulerLogic
{
    public class ServerUpdater<T>
    {
        List<IServerObserver<T>> observers;
        T currentData;
        public ServerUpdater()
        {
            observers = new List<IServerObserver<T>>();
        }
        public void AddObserver(IServerObserver<T> observer)
        {
            observers.Add(observer);
        }
        public void UpdateObservers(T data)
        {
            currentData = data;
            foreach (IServerObserver<T> toUpdate in observers)
                toUpdate.BotUpdate(data);
        }
        public T GetCurrentData()
        {
            return currentData;
        }
    }
    public interface IServerObserver<T>
    {
        void BotUpdate(T update);
    }
}
