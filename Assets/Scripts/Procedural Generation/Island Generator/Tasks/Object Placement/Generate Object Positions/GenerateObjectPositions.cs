using ObjectPlacement.JitteredGrid;
using System;
using System.Collections.Generic;
using TaskManagement;
using UnityEngine;

namespace ProceduralGeneration.IslandGenerator
{
	/// <summary>
	/// Generates positions on an island area.
	/// </summary>
	public class GenerateObjectPositions : DividableTask
	{
		// Inputs
		private PlacedObjectParams placedObjectParams;
		private int resolution;
		private float radius;
		private float maxTerrainHeight;

		// Inputs from previous tasks
		private Func<Color[]> getHeightmap;
		private Color[] heightmap;

		private Func<TerrainBlend[]> getTerrainTypesAtPixels;
		private TerrainBlend[] terrainTypesAtPixels;

		// Internal
		private BoundingBox3D boundingBox;
		private JitteredGrid jitteredGrid;

		// Outputs
		private List<GridPoint> positions;

		public GenerateObjectPositions(GenerateObjectPositionsParams parameters)
		{
			placedObjectParams = parameters.PlacedObjectParams;
			resolution = parameters.Resolution;
			radius = parameters.Radius;
			maxTerrainHeight = parameters.MaxTerrainHeight;
			getHeightmap = parameters.GetHeightmap;
			getTerrainTypesAtPixels = parameters.GetTerrainTypesAtPixels;

			Vector3 areaBottomLeft = new Vector3(-radius, 0f, -radius);
			Vector3 areaTopRight = new Vector3(radius, 0f, radius);
			boundingBox = new BoundingBox3D(areaBottomLeft, areaTopRight);

			jitteredGrid = new JitteredGrid(placedObjectParams.GridParams, placedObjectParams.OffsetParams);
		}

		public List<GridPoint> GetResult()
		{
			if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
			return positions;
		}

		protected override void ExecuteStep()
		{
			positions = jitteredGrid.GetPointsInBoundingBox(boundingBox);
			positions.RemoveAll(IsPositionBad);
			SetPositionsHeights();
		}

		protected override void GetInputFromPreviousStep()
		{
			heightmap = getHeightmap();
			terrainTypesAtPixels = getTerrainTypesAtPixels();
		}

		protected override void SetSteps()
		{
			TotalSteps = 1;
			RemainingSteps = TotalSteps;
		}

		private void SetPositionsHeights()
		{
			foreach (GridPoint position in positions)
			{
				Vector2Int pixelCoords = TextureFunctions.LocalPositionToPixel(position.Position, resolution, radius);
				int pixelIndex = TextureFunctions.CoordsToArrayIndex(resolution, resolution, pixelCoords);

				Vector3 liftedPosition = position.Position;
				liftedPosition.y = heightmap[pixelIndex].r * maxTerrainHeight;
				position.Position = liftedPosition;
			}
		}

		private bool IsPositionBad(GridPoint position)
		{
			Vector2Int pixelCoords = TextureFunctions.LocalPositionToPixel(position.Position, resolution, radius);
			int pixelIndex = TextureFunctions.CoordsToArrayIndex(resolution, resolution, pixelCoords);

			return !HeightOk(pixelIndex) || !TerrainTypeOk(pixelIndex);
		}

		private bool HeightOk(int pixelIndex)
		{
			float pixelHeight = heightmap[pixelIndex].r;
			bool tooLow = pixelHeight < placedObjectParams.MinHeight;
			bool tooHigh = pixelHeight > placedObjectParams.MaxHeight;
			return !tooLow && !tooHigh;
		}

		private bool TerrainTypeOk(int pixelIndex)
		{
			List<TerrainTypeFraction> terrainTypes = terrainTypesAtPixels[pixelIndex].TerrainFractions;

			foreach (TerrainTypeFraction typeFraction in terrainTypes)
			{
				bool typeOk = typeFraction.Type.Name == placedObjectParams.MinimumTerrainFractionName;
				bool minAmountOk = typeFraction.Amount >= placedObjectParams.MinimumTerrainFractionAmount;

				if (typeOk && minAmountOk) return true;
			}

			return false;
		}
	}
}