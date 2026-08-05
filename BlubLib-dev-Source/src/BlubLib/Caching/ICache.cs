using System;

namespace BlubLib.Caching
{
    /// <summary>
    /// A simple cache interface
    /// </summary>
    public interface ICache : IDisposable
    {
        /// <summary>
        /// Sets an cache item without expiration
        /// </summary>
        /// <param name="key">A unique key to identify the item</param>
        /// <param name="value">The item to cache</param>
        void Set(string key, object value);

        /// <summary>
        /// Sets an cache item
        /// </summary>
        /// <param name="key">A unique key to identify the item</param>
        /// <param name="value">The item to cache</param>
        /// <param name="ttl">The time to live. Set to 0 if the item shouldn't expire</param>
        void Set(string key, object value, TimeSpan ttl);

        /// <summary>
        /// Gets an item from the cache
        /// </summary>
        /// <returns>The item or null if the expired or doesn't exist</returns>
        object Get(string key);

        /// <summary>
        /// Gets an item from the cache
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <returns>The item or default(T) if the item expired or doesn't exist</returns>
        T Get<T>(string key);

        /// <summary>
        /// Removes the given key from the cache
        /// </summary>
        /// <returns>True if the key was found</returns>
        bool Remove(string key);

        /// <summary>
        /// Clears the cache
        /// </summary>
        void Clear();
    }
}
