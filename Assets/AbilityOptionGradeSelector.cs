using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
public class AbilityOptionGradeSelector
{
    private readonly Random random = new Random();
    private Dictionary<Rank, double> gradeProbabilities;

    public AbilityOptionGradeSelector(Dictionary<Rank, double> gradeProbabilities)
    {
        this.gradeProbabilities = gradeProbabilities;
    }

    public Rank SelectGrade()
    {
        double randomValue = (double)random.NextDouble();
        double sum = 0.0;

        foreach (var gradeProbability in gradeProbabilities)
        {
            sum += gradeProbability.Value;

            if (randomValue <= sum)
            {
                return gradeProbability.Key;
            }
        }

        return Rank.None;
    }
}