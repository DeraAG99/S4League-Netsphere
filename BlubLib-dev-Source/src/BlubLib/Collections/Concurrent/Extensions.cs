// ReSharper disable once CheckNamespace
namespace System.Collections.Concurrent
{
    public static class ConcurrentDictionaryExtensions
    {
        public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this, TKey key)
        {
            TValue item;
            return @this.TryRemove(key, out item);
        }

        // Fix ambiguous reference problem
        // ConcurrentDictionary implements IDictionary and IReadOnlyDictionary
        public static TValue GetValueOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this, TKey key)
        {
            TValue value;
            return @this.TryGetValue(key, out value) ? value : default(TValue);
        }
    }
}
