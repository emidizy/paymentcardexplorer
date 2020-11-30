using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.Shared.Interfaces
{
    public interface IHttpCache
    {
        void SaveToCache(string key, object data, int secondsInterval);
        T LoadFromCache<T>(string key) where T : new();
        void RemoveFromCache(string key);
    }
}
