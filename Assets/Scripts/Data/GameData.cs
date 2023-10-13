using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/GameData", order = 1)]
public class GameData : ScriptableObject
{
    [SerializeField] private int levelIndex;
    [SerializeField] private int fakeLevelIndex;
    public int LevelIndex
    {
        get { return levelIndex; }
        set
        {
            if (value < 1) levelIndex = 1;
            else if (value > 5) levelIndex = 1;
            else levelIndex = value;
        }
    }
    public int FakeLevelIndex
    {
        get { return fakeLevelIndex; }
        set
        {
            if (value < 1) fakeLevelIndex = 1;
            else fakeLevelIndex = value;
        }
    }
    [Button]
    void ResetData()
    {
        levelIndex = 1;
        fakeLevelIndex = 1;
    }
}
