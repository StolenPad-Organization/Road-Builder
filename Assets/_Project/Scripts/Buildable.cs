using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class Buildable : MonoBehaviour
{
    [SerializeField] private Renderer buildableRenderer;
    [SerializeField] private Collider buildableCollider;
    [SerializeField] private Vector3 initialPos;
    [SerializeField] private Vector3 initialWorldPos;
    [SerializeField] private Vector3 initialRot;
    [SerializeField] private Vector3 initialscale;
    private ParticleSystem smoke;

    void Start()
    {
        initialPos = transform.localPosition;
        initialWorldPos = transform.position;
        initialRot = transform.localEulerAngles;
        initialscale = transform.localScale;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        BuildPiece();
    }

    private void BuildPiece()
    {
        buildableCollider.enabled = false;
        buildableRenderer.enabled = true;
        transform.position = PlayerController.instance.asphaltMachine.partsSpawnPoint.position + PlayerController.instance.asphaltMachine.partsSpawnPoint.right * Random.Range(-1.5f, 1.5f);
        transform.localScale = Vector3.zero;
        transform.DOLocalJump(initialPos, 0.25f, 1, 0.25f).OnComplete(()=> 
        {
            //smoke = SmokePooler.instance.GetSmoke();
            //smoke.transform.position = transform.position + Vector3.up * 0.2f;
        });
        transform.DOLocalRotate(initialRot, 0.25f);
        transform.DOScale(initialscale, 0.25f);
        Material material = buildableRenderer.material;
        Color initialEmissionColor = material.GetColor("_EmissionColor");
        Color targetEmissionColor = Color.black;
        smoke = SmokePooler.instance.GetSmoke();
        smoke.transform.position = initialWorldPos + Vector3.up * 0.2f;
        DOTween.To(() => initialEmissionColor, x => initialEmissionColor = x, targetEmissionColor, 2.0f)
            .OnUpdate(()=> material.SetColor("_EmissionColor", initialEmissionColor)).OnComplete(() => 
            {
                if (smoke != null)
                    SmokePooler.instance.ReturnSmoke(smoke);
            });
    }

    public void SetBuildableEditor()
    {
        buildableCollider = GetComponent<Collider>();
        buildableRenderer = GetComponent<Renderer>();
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }
}
