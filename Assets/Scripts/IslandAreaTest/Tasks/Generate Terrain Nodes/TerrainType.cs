﻿using UnityEngine;

[System.Serializable]
public class TerrainType
{
	public string Name = "New Terrain Type";
	public Color Color;
	[Range(0f, 1f)]
	public float StartingHeight = 0; // This terrain type's color will be used from its starting height to the starting height of other type
	public Noise2DParams NoiseParams;

	public TerrainType(string name, Color color, float startingHeight, Noise2DParams noiseParams)
	{
		Name = name;
		Color = color;
		StartingHeight = startingHeight;
		NoiseParams = noiseParams;
	}
}