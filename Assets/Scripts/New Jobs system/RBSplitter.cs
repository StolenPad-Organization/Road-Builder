using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using static UnityEditor.Progress;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider))]
public class RBSplitter : MonoBehaviour
{
    public Transform RBHolder;
    [SerializeField] private List<RBHandler> rbhandlerObjects = new List<RBHandler>();

    [Range(1, 10)]
    public int Row;
    [Range(1, 10)]
    public int Column;

    [SerializeField] private List<RBTile> RbTileList = new List<RBTile>();
    public Grid<RBTile> Grid;
    public void SplitBoxCollider()
    {
        GetObjectsFromRbHolder();
        Clear();
        Grid = new Grid<RBTile>(Row, Column);

#if UNITY_EDITOR
        Undo.RecordObject(this, "Split Box Collider");
#endif

        BoxCollider originalCollider = GetComponent<BoxCollider>();
        Vector3 originalSize = originalCollider.size;
        Vector3 originalCenter = originalCollider.center;
        Quaternion originalRotation = originalCollider.transform.rotation;

        int originalLayer = gameObject.layer;
        string originalTag = gameObject.tag;

        // Calculate the size for the smaller boxes
        Vector3 newSize = new Vector3(
            originalSize.x / Row,
            originalSize.y,
            originalSize.z / Column
        );

        // Calculate the start position for the first box in the grid
        Vector3 startPosition = originalCenter - (originalSize / 2) + (newSize / 2);
        byte id = 0;
        // Create new colliders based on grid
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                // Calculate the position for this box
                Vector3 newPosition = startPosition + new Vector3(
                    i * newSize.x,
                    0,
                    j * newSize.z
                );

                // Create new GameObject for the new collider
                GameObject newColliderGO = new GameObject("BoxCollider_" + i + "_" + j);
                newColliderGO.transform.parent = this.transform;
                newColliderGO.transform.localPosition = newPosition;
                newColliderGO.transform.localRotation = Quaternion.identity;
                newColliderGO.transform.localScale = Vector3.one;

                // Set layer and tag from original GameObject
                newColliderGO.layer = originalLayer;
                newColliderGO.tag = originalTag;

                // Create the new box collider with calculated size and position
                BoxCollider newCollider = newColliderGO.AddComponent<BoxCollider>();
                newCollider.size = newSize;
                newCollider.center = Vector3.zero; // Since the GameObject's position is already set

                newCollider.isTrigger = originalCollider.isTrigger;

                var rbtile = newColliderGO.AddComponent<RBTile>();
                rbtile.Index = new int2(i, j);
                rbtile.ID = id;
                
                id++;
                RbTileList.Add(rbtile);
                Grid.Add(i, j, rbtile);
#if UNITY_EDITOR
                Undo.RegisterCreatedObjectUndo(newColliderGO, "Created new Box Collider");
#endif

              
            }
        }

        // Restore original rotation (if any)
        transform.rotation = originalRotation;
        originalCollider.enabled = false;

        SetRbHandlers();
    }

    public void Clear()
    {
        Grid = new Grid<RBTile>(0, 0);
#if UNITY_EDITOR
        Undo.RecordObject(this, "Clear Box Colliders");
#endif

        foreach (var item in RbTileList)
        {
#if UNITY_EDITOR
            if (item != null)
                Undo.DestroyObjectImmediate(item.gameObject);
#else
            DestroyImmediate(item.gameObject);
#endif
        }
        RbTileList.Clear();
        //rbhandlerObjects.Clear();
        BoxCollider originalCollider = GetComponent<BoxCollider>();
        originalCollider.enabled = true;
    }

    private void GetObjectsFromRbHolder()
    {
        rbhandlerObjects.Clear();
        foreach (Transform item in RBHolder)
        {
            var rb = item.GetComponent<RBHandler>();
            rbhandlerObjects.Add(rb);
            //foreach (Transform item2 in item)
            //{
                
            //}
        }
    }
    private void SetRbHandlers()
    {
        foreach (var item in rbhandlerObjects)
        {
            float closestDistance = Mathf.Infinity;
            int closestIndex = 0;
            for (int i = 0; i < RbTileList.Count; i++)
            {
                float distance = math.distance(item.transform.position, RbTileList[i].transform.position);
                if (distance < closestDistance)
                {
                    closestIndex = i;
                    closestDistance = distance;
                }
            }
            RbTileList[closestIndex].AddRbHandler(item);
            item.RBTile = RbTileList[closestIndex];

            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(item);
#endif
        }

        foreach (var item in RbTileList)
        {
            var t = Grid.GetAdjustantTiles(item.Index);
            Debug.Log(t);
            t.Add(item);
            item.adjustantTiles = t;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(item);
#endif
        }
    }
}