namespace WingTechBot.Extensions;

public static class IListExtensions
{
    public static T GetRandom<T>(this IList<T> list) => list[Random.Shared.Next(0, list.Count)];
}