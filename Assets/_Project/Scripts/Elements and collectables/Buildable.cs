using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class Buildable : MonoBehaviour
{
    [SerializeField] private int index;
    public Renderer buildableRenderer;
    [SerializeField] private Collider buildableCollider;
    [SerializeField] private Vector3 initialPos;
    [SerializeField] private Vector3 initialWorldPos;
    [SerializeField] private Vector3 initialRot;
    [SerializeField] private Vector3 initialscale;
    private ParticleSystem smoke;
    [SerializeField] private GameObject copy;

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
        if(PlayerController.instance.asphaltMachine != null)
        {
            if(PlayerController.instance.asphaltMachine.UseAsphalt())
                BuildPiece();
        }
    }

    private void BuildPiece()
    {
        GameManager.instance.currentZone.SaveBuildable(index, true);
        buildableCollider.enabled = false;
        buildableRenderer.enabled = true;
        transform.position = PlayerController.instance.asphaltMachine.partsSpawnPoint.position + PlayerController.instance.asphaltMachine.partsSpawnPoint.right * Random.Range(-1.5f, 1.5f);
        transform.localScale = Vector3.zero;
        transform.DOLocalJump(initialPos, 0.25f, 1, 0.265f).OnComplete(()=>
        {
            GameManager.instance.currentZone.OnRoadBuild();
            GameManager.instance.currentZone.buildableManager.OnBuild(transform.position);
            //smoke = SmokePooler.instance.GetSmoke();
            //smoke.transform.position = transform.position + Vector3.up * 0.2f;

            if (copy != null)
                copy.SetActive(true);
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

        if (PlayerController.instance.canDoStrictedHaptic)
        {
            EventManager.invokeHaptic.Invoke(vibrationTypes.LightImpact);
            PlayerController.instance.canDoStrictedHaptic = false;
        }
    }

#if UNITY_EDITOR
    public void SetBuildableEditor(int _index)
    {
        index = _index;
        buildableCollider = GetComponent<Collider>();
        buildableRenderer = GetComponent<Renderer>();
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }
    public void SetMaterialEditor(Material mat)
    {
        buildableRenderer.material = mat;

        EditorUtility.SetDirty(buildableRenderer);
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }

    public GameObject SetBuildableCopy(Material mat, float ycenter, float copySize)
    {
        if (copy != null)
            DestroyImmediate(copy);
        copy = Instantiate(buildableRenderer.gameObject, buildableRenderer.transform);
        copy.transform.localScale = new Vector3(copySize, 0.01f, copySize);
        copy.transform.localPosition = Vector3.up * ycenter;
        copy.GetComponent<Renderer>().enabled = true;
        copy.GetComponent<Renderer>().material = mat;
        copy.SetActive(false);

        DestroyImmediate(copy.GetComponent<BoxCollider>());
        DestroyImmediate(copy.GetComponent<Buildable>());

        EditorUtility.SetDirty(copy);
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);

        return copy;
    }

    public void RemoveCopy()
    {
        if (copy != null)
            DestroyImmediate(copy);
        copy = null;

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }
#endif

    public void LoadBuildable(bool check)
    {
        buildableCollider.enabled = false;
        buildableRenderer.enabled = true;
        if(check)
            GameManager.instance.currentZone.OnRoadBuild();
        Material material = buildableRenderer.material;
        material.SetColor("_EmissionColor", Color.black);
        if (copy != null)
            copy.SetActive(true);
    }
}
