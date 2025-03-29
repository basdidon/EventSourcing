using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Api.Tests.Integration
{
    public class UserLoginDictionary : IDictionary<string, UserLogin>
    {
        private readonly Dictionary<string, UserLogin> items = [];

        public UserLogin this[string key]
        {
            get => items.TryGetValue(key, out var value)
                ? value
                : throw new KeyNotFoundException($"Key '{key}' not found.");
            set
            {
                if (!items.ContainsKey(key))
                    throw new KeyNotFoundException($"Key '{key}' not found.");
                items[key] = value;
            }
        }

        public ICollection<string> Keys => items.Keys;
        public ICollection<UserLogin> Values => items.Values;
        public int Count => items.Count;
        public bool IsReadOnly => false;

        public void Add(string key, UserLogin value)
        {
            if (value.Username != key)
                throw new InvalidOperationException("Key must match the username.");
            items.Add(key, value);
        }

        public void Add(KeyValuePair<string, UserLogin> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear() => items.Clear();

        public bool Contains(KeyValuePair<string, UserLogin> item)
        {
            return items.TryGetValue(item.Key, out var value) && EqualityComparer<UserLogin>.Default.Equals(value, item.Value);
        }

        public bool ContainsKey(string key) => items.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, UserLogin>[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);

            if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < items.Count) throw new ArgumentException("The array is too small.");

            foreach (var kvp in items)
            {
                array[arrayIndex++] = kvp;
            }
        }

        public IEnumerator<KeyValuePair<string, UserLogin>> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Remove(string key) => items.Remove(key);

        public bool Remove(KeyValuePair<string, UserLogin> item)
        {
            return Contains(item) && items.Remove(item.Key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out UserLogin value) => items.TryGetValue(key, out value);
    }
}
