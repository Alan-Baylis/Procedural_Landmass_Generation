﻿using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
	const int mapChunkSize = 241;

	[Range (0, 6)]
	public int levelOfDetail;

	public DrawMode drawMode;
	public float noiseScale;

	public int octaves;
	[Range (0, 1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;
	public float meshHeightMultipier;
	public AnimationCurve meshHeightCurve;

	public bool autoUpdate;

	public TerrainType[] regions;

	public enum DrawMode
	{
		NoiseMap,
		ColorMap,
		Mesh
	}

	public void GenerateMap ()
	{
		float[,] noiseMap = Noise.GenerateNoiseMap (mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

		Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

		for (int y = 0; y < mapChunkSize; y++) {
			for (int x = 0; x < mapChunkSize; x++) {
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight <= regions [i].height) {
						colorMap [y * mapChunkSize + x] = regions [i].color;
						break;
					}
				}
			}
		}

		MapDisplay mapDisplay = FindObjectOfType<MapDisplay> ();
		if (drawMode == DrawMode.NoiseMap) {
			mapDisplay.drawTexture (TextureGenerator.textureFromHeightMap (noiseMap));
		} else if (drawMode == DrawMode.ColorMap) {
			mapDisplay.drawTexture (TextureGenerator.textureFromColorMap (colorMap, mapChunkSize, mapChunkSize));
		} else if (drawMode == DrawMode.Mesh) {
			mapDisplay.DrawMesh (MeshGenerator.GeneratTerrainMesh (noiseMap, meshHeightMultipier, meshHeightCurve, levelOfDetail), TextureGenerator.textureFromColorMap (colorMap, mapChunkSize, mapChunkSize));
		}
	}

	void OnValidate ()
	{
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}
	}
}

[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color color;
}