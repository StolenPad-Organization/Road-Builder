using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject[] rectPrefabs; // Assign your rectangle Prefab here
    public int columns = 10; // Number of columns
    public int rows = 10; // Number of rows
    public float offsetX = 1.0f; // X Offset
    public float offsetY = 1.0f; // Y Offset
    public Transform GridParent;

    public List<GameObject> Children;
    void Start()
    {
        GenerateGrid();
    }

    [ContextMenu("Set Grid")]
    void GenerateGrid()
    {
        foreach (var child in Children)
        {
            if (child != null)
                DestroyImmediate(child);
        }
        Children.Clear();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                var rectPrefab = rectPrefabs[Random.Range(0, rectPrefabs.Length)];
                // Calculate position for each rectangle
                Vector3 spawnPosition = new Vector3(x * (rectPrefab.transform.localScale.x + offsetX),
                    0f,
                                                    y * (rectPrefab.transform.localScale.y + offsetY));

                // Instantiate the prefab at the calculated position
                var go = Instantiate(rectPrefab, GridParent);
                go.transform.SetLocalPositionAndRotation(spawnPosition, Quaternion.identity);
                //   go.transform.SetParent(GridParent);
                Children.Add(go);
            }
        }
    }
}