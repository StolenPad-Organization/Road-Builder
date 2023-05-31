using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType
{
    Snow,
    Rock
}

public class CollectablesPooler : MonoBehaviour
{
    public static CollectablesPooler Instance;
    public List<Collectable> collectablesPrefabs;
    [SerializeField] private int poolSize = 5;
    [SerializeField] private bool expandable = true;
    public Transform collectableParent;

    private Dictionary<CollectableType, Queue<Collectable>> poolDictionary = new Dictionary<CollectableType, Queue<Collectable>>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (Collectable collectable in collectablesPrefabs)
        {
            Queue<Collectable> objectPool = new Queue<Collectable>();

            for (int i = 0; i < poolSize; i++)
            {
                Collectable obj = Instantiate(collectable, collectableParent);
                obj.gameObject.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(collectable.collectableType, objectPool);
        }
    }

    public Collectable GetCollectable(CollectableType collectableType, Vector3 position)
    {
        if (!poolDictionary.ContainsKey(collectableType))
        {
            return null;
        }

        Queue<Collectable> objectPool = poolDictionary[collectableType];

        if (objectPool.Count == 0)
        {
            if (expandable)
            {
                Collectable obj = null;
                foreach (var item in collectablesPrefabs)
                {
                    if (item.collectableType == collectableType)
                    {
                        obj = Instantiate(item, collectableParent);
                        break;
                    }
                }
                if (obj != null)
                {
                    obj.gameObject.SetActive(false);
                    objectPool.Enqueue(obj);
                }
            }
            else
            {
                return null;
            }
        }

        Collectable pooledObject = objectPool.Dequeue();
        pooledObject.transform.position = position;
        pooledObject.gameObject.SetActive(true);

        if(pooledObject.transform.parent != collectableParent)
        {
            pooledObject.transform.SetParent(collectableParent);
        }

        return pooledObject;
    }

    public void ReturnCollectable(Collectable collectable)
    {
        collectable.gameObject.SetActive(false);
        poolDictionary[collectable.collectableType].Enqueue(collectable);
    }
}
