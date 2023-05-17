using UnityEngine;

public class PaintArea : MonoBehaviour
{
    public GameObject paintObjectPrefab;
    public Transform paintParent;
    public Collider paintAreaCollider;

    public float paintRadius = 1f;

    [SerializeField] private bool isPainting;

    private void Update()
    {
        if (isPainting)
        {
            Vector3 playerPosition = transform.position;

            if (paintAreaCollider.bounds.Contains(playerPosition))
            {
                GameObject paintObject = Instantiate(paintObjectPrefab, playerPosition, transform.rotation, paintParent);
            }
        }
    }

    public void StartPainting()
    {
        isPainting = true;
    }

    public void StopPainting()
    {
        isPainting = false;
    }
}
