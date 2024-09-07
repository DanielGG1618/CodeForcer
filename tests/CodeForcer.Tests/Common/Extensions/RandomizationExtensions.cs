namespace CodeForcer.Tests.Common.Extensions;

public static class RandomizationExtensions
{
    public static T PickRandom<T>(this List<T> list) =>
        list[Random.Shared.Next(list.Count)];

    public static T PickRandom<T>(this T[] array) =>
        array[Random.Shared.Next(array.Length)];

    public static T PickRandom<T>(this IEnumerable<T> enumerable) =>
        enumerable.ToArray().PickRandom();

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> enumerable, int count) =>
        enumerable.Shuffle().Take(count);

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable) =>
        enumerable.OrderBy(_ => Random.Shared.Next());
}
