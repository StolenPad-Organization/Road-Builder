using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokePooler : MonoBehaviour
{
    public static SmokePooler instance;

    [SerializeField] private ParticleSystem smokePrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<ParticleSystem> smokePool = new Queue<ParticleSystem>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem smoke = Instantiate(smokePrefab);
            smoke.gameObject.SetActive(false);
            smokePool.Enqueue(smoke);
        }
    }

    public ParticleSystem GetSmoke()
    {
        if (smokePool.Count == 0)
        {
            ParticleSystem smoke = Instantiate(smokePrefab);
            smoke.gameObject.SetActive(false);
            smokePool.Enqueue(smoke);
        }

        ParticleSystem pooledsmoke = smokePool.Dequeue();
        pooledsmoke.gameObject.SetActive(true);
        return pooledsmoke;
    }

    public void ReturnSmoke(ParticleSystem smoke)
    {
        smoke.transform.position = transform.position;
        smoke.transform.rotation = transform.rotation;

        smoke.gameObject.SetActive(false);
        smokePool.Enqueue(smoke);
    }
}
