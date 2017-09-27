using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        public async Task UpdateObservers(T data)
        {
            currentData = data;
            foreach (IServerObserver<T> toUpdate in observers)
                await toUpdate.BotUpdate(data);
        }
        public T GetCurrentData()
        {
            return currentData;
        }
    }
    public interface IServerObserver<T>
    {
        Task BotUpdate(T update);
    }
}
