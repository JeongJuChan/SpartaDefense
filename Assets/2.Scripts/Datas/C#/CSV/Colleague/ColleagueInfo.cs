using System;

[Serializable]
public struct ColleagueInfo
{
    public Rank rank;
    public ColleagueType colleagueType;

    public ColleagueInfo(Rank rank, ColleagueType colleagueType)
    {
        this.rank = rank;
        this.colleagueType = colleagueType;
    }
}