using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "ColorDict", menuName = "SO/Color")]
public class ColorDictionary : ScriptableObject
{
    public SerializedDictionary<GameColor, Color> _colors;
}

public enum GameColor
{
    None,
    DefaultBlock,
    PlayerABlock,
    PlayerBBlock,
    Max
}