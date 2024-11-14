using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonBackground", menuName = "SO/DungeonBackground")]
public class DungeonBackgroundSO : ScriptableObject
{
    [field: SerializeField] public Sprite[] dungeonBackgrounds { get; private set; } 
}
