using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGenerator : MonoBehaviour
{
	[SerializeField]
	ColorToGameObject[] mappings;
	public Texture2D tex;
	public Vector2 doorSizeInTiles;

	// Start is called before the first frame update
	private void Start()
	{
		GenerateDoorTiles();
	}
	private void GenerateDoorTiles()
	{
		//loop through every pixel of the texture
		for (int x = 0; x < tex.width; x++)
		{
			for (int y = 0; y < tex.height; y++)
			{
				GenerateTile(x, y);
			}
		}
	}
	void GenerateTile(int x, int y)
	{
		Color pixelColor = tex.GetPixel(x, y);
		//skip clear spaces in texture
		if (pixelColor.a == 0)
		{
			return;
		}
		//find the color to math the pixel
		foreach (ColorToGameObject mapping in mappings)
		{
			if (mapping.color.Equals(pixelColor))
			{
				Vector3 spawnPos = PositionFromTileGrid(x, y);
				Instantiate(mapping.prefab[Random.Range(0, mapping.prefab.Length)], spawnPos, Quaternion.identity).transform.parent = this.transform;
			}
		}
	}
	Vector2 PositionFromTileGrid(int x, int y)
	{
		Vector2 ret;
		int tileSize = 1;
		//find difference between the corner of the texture and the center of this object
		Vector2 offset = new Vector3((-doorSizeInTiles.x + 1) * tileSize, (doorSizeInTiles.y) / 4 * tileSize);
		//find scaled up position at the offset
		ret = new Vector2(tileSize * (float)x, -tileSize * (float)y) + offset + (Vector2)transform.position;
		return ret;
	}
}
