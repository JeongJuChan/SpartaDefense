using System;
using System.Collections.Generic;

public class AbilityOptionCalculator
{
    private readonly Random random = new Random();

    private Dictionary<AbilityOptionEffectType, Dictionary<Rank, (float min, float max, ArithmeticStatType type)>> effectIncreaseRanges;

    public AbilityOptionCalculator(Dictionary<AbilityOptionEffectType, Dictionary<Rank, (float min, float max, ArithmeticStatType type)>> effectIncreaseRanges)
    {
        this.effectIncreaseRanges = effectIncreaseRanges;
    }

    public float DetermineEffectIncrease(AbilityOptionEffectType effectType, Rank rank)
    {
        if (effectIncreaseRanges.TryGetValue(effectType, out var rankRanges) && rankRanges.TryGetValue(rank, out var range))
        {
            return (float)(random.NextDouble() * (range.max - range.min) + range.min);
        }
        return 0.0f;
    }

    public ArithmeticStatType DetermineStatType(AbilityOptionEffectType effectType, Rank rank)
    {
        if (effectIncreaseRanges.TryGetValue(effectType, out var rankRanges) && rankRanges.TryGetValue(rank, out var range))
        {
            return range.type;
        }
        return ArithmeticStatType.Base;
    }

}