using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a preview object for a texture pixels array.
/// </summary>
public class ShowTexture : SingleTask
{
	// Inputs
	private Material material;
	private float sideLength;
	private int resolution;
	private Transform parent;

	// Inputs from previous steps
	private Func<Color[]> getTexturePixels;
	private Color[] texturePixels;

	// Output
	private TexturePreview previewObject;

	// Internal
	private const float initialElevation = 0.1f;

	public ShowTexture(Material material, float sideLength, int resolution, Transform parent , Func<Color[]> getTexturePixels)
	{
		Name = "Show Texture";

		this.material = material;
		this.sideLength = sideLength;
		this.resolution = resolution;
		this.parent = parent;
		this.getTexturePixels = getTexturePixels;
	}

	public TexturePreview GetResult()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		return previewObject;
	}

	/// <summary>
	/// Wraps the result in a list so it can be processed by tasks requiring a list input.
	/// </summary>
	public List<TexturePreview> GetResultInList()
	{
		if (!Finished) Debug.LogWarning($"\"GetResult()\" called on {Name} task before finished.");
		List<TexturePreview> previewObjectListWrapper = new List<TexturePreview>();
		previewObjectListWrapper.Add(previewObject);
		return previewObjectListWrapper;
	}

	protected override void ExecuteStep()
	{
		previewObject = new TexturePreview(material, parent);
		previewObject.SetName(Name);

		Texture2D texture = new Texture2D(resolution, resolution);
		texture.SetPixels(texturePixels);
		texture.Apply();

		float elevation = initialElevation;

		previewObject.SetTexture(texture);
		previewObject.SetDimensions(new Vector3(sideLength, 0f, sideLength));
		previewObject.SetLocalPosition(new Vector3(0f, elevation, 0f));
	}

	protected override void GetInputFromPreviousStep()
	{
		texturePixels = getTexturePixels();
	}

	protected override void SetSteps()
	{
		TotalSteps = 1;
		RemainingSteps = TotalSteps;
	}
}
