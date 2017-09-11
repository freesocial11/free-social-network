using System;

namespace mobSocial.Core.Infrastructure.Utils
{
    public class Singleton<T> where T: class
    {
        private static T instance;
        private static readonly object padlock = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        return instance ?? (instance = Activator.CreateInstance<T>());
                    }
                }
                return instance;

            }
        } 
    }
}