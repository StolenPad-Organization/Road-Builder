using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapingEffectPooler : MonoBehaviour
{
    public static ScrapingEffectPooler instance;

    [SerializeField] private ParticleSystem scrapingEffectPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private Material originalMat;
    ParticleSystemRenderer particleSystemRenderer;

    private Queue<ParticleSystem> effectsPool = new Queue<ParticleSystem>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem effect = Instantiate(scrapingEffectPrefab);
            effect.gameObject.SetActive(false);
            effectsPool.Enqueue(effect);
        }
    }

    public ParticleSystem GetEffect()
    {
        if (effectsPool.Count == 0)
        {
            ParticleSystem effect = Instantiate(scrapingEffectPrefab);
            effect.gameObject.SetActive(false);
            effectsPool.Enqueue(effect);
        }

        ParticleSystem pooledeffects = effectsPool.Dequeue();
        pooledeffects.gameObject.SetActive(true);
        return pooledeffects;
    }

    public void ReturnEffect(ParticleSystem effect)
    {
        StartCoroutine(EffectReturnCounter(effect));
    }

    private IEnumerator EffectReturnCounter(ParticleSystem effect)
    {
        yield return new WaitForSeconds(1.5f);

        effect.transform.position = transform.position;
        effect.transform.rotation = transform.rotation;
        particleSystemRenderer = effect.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.material = originalMat;
        effect.gameObject.SetActive(false);
        effectsPool.Enqueue(effect);
    }
}
