using System;
using System.Collections.Generic;

public class AbilityOptionEffectSelector
{
    private readonly Random random = new Random();
    private Dictionary<AbilityOptionEffectType, double> effectProbabilities;

    public AbilityOptionEffectSelector(Dictionary<AbilityOptionEffectType, double> effectProbabilities)
    {
        this.effectProbabilities = effectProbabilities;
    }

    public AbilityOptionEffectType SelectEffect()
    {
        double randomValue = (double)random.NextDouble();
        double sum = 0.0;

        foreach (var effectProbability in effectProbabilities)
        {
            sum += effectProbability.Value;

            if (randomValue <= sum)
            {
                return effectProbability.Key;
            }
        }

        return AbilityOptionEffectType.None;
    }
}