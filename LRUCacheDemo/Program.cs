using System;
using System.Threading;
using CustomDataStructures;

namespace LRUCacheDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Демо: LRU Cache\n");

            // === Пример 1: Базовое использование ===
            Console.WriteLine("Пример 1: Базовое использование");
            var cache = new LRUCache<string, int>(capacity: 3);

            cache.Put("A", 1);
            cache.Put("B", 2);
            cache.Put("C", 3);

            Console.WriteLine(cache);

            // Обращаемся к A — он становится "свежим"
            Console.WriteLine("\nЧитаем A...");
            var value = cache.Get("A");
            Console.WriteLine($"A = {value}");
            Console.WriteLine(cache);

            // Добавляем D — B должен удалиться (самый старый)
            Console.WriteLine("\nДобавляем D (B должен удалиться)...");
            cache.Put("D", 4);
            Console.WriteLine(cache);

            // === Пример 2: Кэш запросов ===
            Console.WriteLine("\n\nПример 2: Кэш API-запросов");
            var apiCache = new LRUCache<string, string>(capacity: 5);

            // Симуляция запросов
            var requests = new[] { "users", "posts", "users", "comments", "posts", "users", "settings" };

            foreach (var endpoint in requests)
            {
                if (apiCache.TryGet(endpoint, out var cached))
                {
                    Console.WriteLine($"Кэш: {endpoint} -> {cached}");
                }
                else
                {
                    // Симуляция запроса к API
                    var response = $"Data from {endpoint}";
                    apiCache.Put(endpoint, response);
                    Console.WriteLine($"API: {endpoint} -> {response}");
                }
            }

            Console.WriteLine("\n" + apiCache);

            // === Пример 3: Кэш изображений ===
            Console.WriteLine("\n\nПример 3: Кэш изображений");
            var imageCache = new LRUCache<int, byte[]>(capacity: 4);

            // Загружаем "изображения"
            for (int i = 1; i <= 6; i++)
            {
                imageCache.Put(i, new byte[1024]); // 1KB "изображение"
                Console.WriteLine($"Загружено изображение #{i}");
            }

            Console.WriteLine("\n" + imageCache);
            Console.WriteLine("Изображения 1 и 2 удалены (самые старые)");

            // === Пример 4: Статистика ===
            Console.WriteLine("\n\nПример 4: Статистика кэша");
            var statsCache = new LRUCache<int, string>(capacity: 3);

            // Симуляция работы
            var random = new Random();
            for (int i = 0; i < 20; i++)
            {
                int key = random.Next(1, 5); // Ключи 1-4
                statsCache.TryGet(key, out _);
            }

            var stats = statsCache.GetStats();
            Console.WriteLine($"Попадания: {stats.Hits}");
            Console.WriteLine($"Промахи: {stats.Misses}");
            Console.WriteLine($"Hit Rate: {stats.HitRate:F1}%");

            // === Пример 5: Многопоточность ===
            Console.WriteLine("\n\nПример 5: Многопоточный доступ");
            var threadCache = new LRUCache<int, int>(capacity: 100);

            var tasks = new Thread[5];
            for (int t = 0; t < 5; t++)
            {
                int threadId = t;
                tasks[t] = new Thread(() =>
                {
                    for (int i = 0; i < 20; i++)
                    {
                        threadCache.Put(threadId * 100 + i, i);
                        threadCache.TryGet(threadId * 100 + i, out _);
                    }
                });
                tasks[t].Start();
            }

            foreach (var task in tasks)
                task.Join();

            Console.WriteLine($"Элементов в кэше: {threadCache.Count}");
            Console.WriteLine($"Статистика: {threadCache.GetStats()}");

            Console.WriteLine("\nLRU Cache готов к использованию!");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}