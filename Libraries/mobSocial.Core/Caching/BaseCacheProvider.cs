namespace mobSocial.Core.Caching
{
    public abstract class BaseCacheProvider<TCache> : ICacheProvider
    {
        protected TCache _cache;

        protected BaseCacheProvider()
        {
            _cache = InitializeCacheProvider();
        }

        protected abstract TCache InitializeCacheProvider();

        public abstract void Dispose();

        public abstract T Get<T>(string cacheKey);

        public abstract bool IsSet(string cacheKey);

        public abstract void Set<T>(string cacheKey, T cacheData, int expiration = 60);

        public abstract void Remove(string cacheKey);

        public abstract void Clear();
    }
}