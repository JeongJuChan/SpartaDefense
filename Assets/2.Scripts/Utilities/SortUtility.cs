using System;

public class SortUtility
{
    public static Comparison<MonsterCountData> GetSortDescendingCondition()
    {
        return (x, y) => -x.count.CompareTo(y.count);
    }

    public static Comparison<MonsterCountData> GetSortAscendingCondition()
    {
        return (x, y) => x.count.CompareTo(y.count);
    }
}