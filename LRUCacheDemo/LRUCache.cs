using System.Text;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CustomDataStructures
{
    /// <summary>
    /// LRU Cache — кэш с ограниченным размером и вытеснением старых элементов.
    /// </summary>
    public class LRUCache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, LinkedListNode<CacheEntry>> _map;
        private readonly LinkedList<CacheEntry> _list;
        private readonly object _lock = new();

        // Статистика
        private int _hits;
        private int _misses;

        private class CacheEntry
        {
            public TKey Key { get; }
            public TValue Value { get; set; }

            public CacheEntry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        public LRUCache(int capacity)
        {
            if (capacity <= 0) throw new ArgumentException("Capacity must be positive");
            _capacity = capacity;
            _map = new Dictionary<TKey, LinkedListNode<CacheEntry>>(capacity);
            _list = new LinkedList<CacheEntry>();
        }

        /// <summary>
        /// Получить значение по ключу.
        /// Возвращает false, если ключ не найден.
        /// </summary>
        public bool TryGet(TKey key, out TValue value)
        {
            lock (_lock)
            {
                if (_map.TryGetValue(key, out var node))
                {
                    // Помечаем как "использованное" — перемещаем в начало
                    _list.Remove(node);
                    _list.AddFirst(node);

                    _hits++;
                    value = node.Value.Value;
                    return true;
                }

                _misses++;
                value = default!;
                return false;
            }
        }

        /// <summary>
        /// Получить значение по ключу (бросает исключение если не найден).
        /// </summary>
        public TValue Get(TKey key)
        {
            if (TryGet(key, out var value))
                return value;
            throw new KeyNotFoundException($"Key '{key}' not found in cache");
        }

        /// <summary>
        /// Добавить или обновить значение.
        /// </summary>
        public void Put(TKey key, TValue value)
        {
            lock (_lock)
            {
                if (_map.TryGetValue(key, out var existingNode))
                {
                    // Обновляем существующий
                    existingNode.Value.Value = value;
                    _list.Remove(existingNode);
                    _list.AddFirst(existingNode);
                }
                else
                {
                    // Создаём новый
                    if (_map.Count >= _capacity)
                    {
                        // Удаляем самый старый (в конце списка)
                        var oldest = _list.Last!;
                        _list.RemoveLast();
                        _map.Remove(oldest.Value.Key);
                    }

                    var newNode = new LinkedListNode<CacheEntry>(new CacheEntry(key, value));
                    _list.AddFirst(newNode);
                    _map.Add(key, newNode);
                }
            }
        }

        /// <summary>
        /// Удалить элемент по ключу.
        /// </summary>
        public bool Remove(TKey key)
        {
            lock (_lock)
            {
                if (_map.TryGetValue(key, out var node))
                {
                    _list.Remove(node);
                    _map.Remove(key);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Очистить весь кэш.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _map.Clear();
                _list.Clear();
                _hits = 0;
                _misses = 0;
            }
        }

        /// <summary>
        /// Текущее количество элементов.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock) return _map.Count;
            }
        }

        /// <summary>
        /// Максимальная вместимость.
        /// </summary>
        public int Capacity => _capacity;

        /// <summary>
        /// Процент попаданий в кэш (0-100).
        /// </summary>
        public double HitRate
        {
            get
            {
                lock (_lock)
                {
                    int total = _hits + _misses;
                    return total == 0 ? 0 : (double)_hits / total * 100;
                }
            }
        }

        /// <summary>
        /// Статистика кэша.
        /// </summary>
        public (int Hits, int Misses, double HitRate) GetStats()
        {
            lock (_lock)
            {
                int total = _hits + _misses;
                return (_hits, _misses, total == 0 ? 0 : (double)_hits / total * 100);
            }
        }

        public override string ToString()
        {
            lock (_lock)
            {
                var stats = GetStats();
                var sb = new StringBuilder();
                sb.AppendLine($"LRUCache<{typeof(TKey).Name}, {typeof(TValue).Name}>");
                sb.AppendLine($"Вместимость: {_map.Count}/{_capacity}");
                sb.AppendLine($"Попадания: {stats.Hits} | Промахи: {stats.Misses}");
                sb.AppendLine($"Hit Rate: {stats.HitRate:F1}%");
                sb.AppendLine("Элементы (от свежих к старым):");

                int i = 0;
                foreach (var entry in _list)
                {
                    string position = i == 0 ? " " : i == _list.Count - 1 ? " " : " ";
                    sb.AppendLine($"   {position} [{entry.Key}] = {entry.Value}");
                    i++;
                }

                return sb.ToString();
            }
        }
    }
}