# LRUCache-CSharp
LRU Cache implementation in C# with thread-safety and statistics
LRUCache<TKey, TValue>
Кэш с ограниченным размером и автоматической очисткой наименее используемых элементов.
LRU (Least Recently Used) — стратегия кэширования, где при заполнении удаляются элементы, к которым давно не обращались.
Возможности:
- Get/put за O(1)
- Автоматическое вытеснение старых элементов
- Статистика попаданий/промахов
- Потокобезопасность

Пример использования (C#):
var cache = new LRUCache<string, int>(capacity: 100);
cache.Put("user:123", 42);
if (cache.TryGet("user:123", out var value))
{
    Console.WriteLine($"Found: {value}");
}
