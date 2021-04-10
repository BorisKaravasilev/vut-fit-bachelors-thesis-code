﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GenerateTerrainNodesParams", fileName = "New Generate Terrain Nodes Params")]
public class TerrainNodesParams : ScriptableObject
{
	public List<TerrainType> TerrainTypes;
	public int MinNodes = 1;
	public int MaxNodes = 3;
	[Range(0f, 1f)]
	public float MaxDistanceMultiplier = 1;

	public TerrainNodesParams(List<TerrainType> terrainTypes, int minNodes, int maxNodes, float maxDistanceMultiplier)
	{
		TerrainTypes = terrainTypes;
		MinNodes = minNodes;
		MaxNodes = maxNodes;
		MaxDistanceMultiplier = maxDistanceMultiplier;
	}
}