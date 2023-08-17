using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Peelable : MonoBehaviour
{
    [SerializeField] private int index;
    public float power;
    private float initialPower;
    [SerializeField] private float speedAmount;
    [SerializeField] private CollectableType collectableType;
    public MeshFilter peelableMeshFilter;
    public MeshRenderer peelableRenderer;
    public bool diffirentCollectable;
    private Vector3 collectablePosition;
    private Vector3 collectableRotation;
    public int zoneIndex;
    public CollectableShape collectableShape;
    public Color dustColor;

    void Start()
    {
        initialPower = power;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (zoneIndex != GameManager.instance.levelProgressData.ZoneIndex) return;
        PlayerController.instance.OnPeelableDetection(speedAmount, power, dustColor);
        EventManager.invokeHaptic.Invoke(vibrationTypes.MediumImpact);
    }

    private void OnTriggerStay(Collider other)
    {
        if (zoneIndex != GameManager.instance.levelProgressData.ZoneIndex) return;
        power -= PlayerController.instance.scrapeTool.power;
        if(peelableRenderer.material.HasFloat("_Power"))
            peelableRenderer.material.SetFloat("_Power", 1 - (power / initialPower));
        PlayerController.instance.scrapeTool.ShakeTool();
        if(power <= 0)
        {
            collectablePosition = transform.position + (PlayerController.instance.GetClosestCheckDirection(transform.position));
            if (GameManager.instance.currentZone.groundYRef > -1)
                collectablePosition.y = GameManager.instance.currentZone.groundYRef;
            collectablePosition += Vector3.up * Random.Range(0.0f, 0.5f);
            collectableRotation = new Vector3(Random.Range(-25f, 25f), Random.Range(-90f, 90f), Random.Range(-25f, 25f));
            GameManager.instance.currentZone.SavePeelable(index, true, false, collectablePosition, collectableRotation);
            Collectable collectable = CollectablesPooler.Instance.GetCollectable(collectableType, transform.position);
            collectable.peelable = this;
            collectable.Spawn(collectablePosition, collectableRotation);
            gameObject.SetActive(false);
            GameManager.instance.currentZone.OnBlockRemove();
            EventManager.invokeHaptic.Invoke(vibrationTypes.MediumImpact);
        }
    }
#if UNITY_EDITOR
    public void SetPeelableEditor(int _index)
    {
        index = _index;
        peelableMeshFilter = GetComponent<MeshFilter>();
        peelableRenderer = GetComponent<MeshRenderer>();
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }

    public void SetMaterialEditor(Material mat)
    {
        peelableRenderer.material = mat;
        EditorUtility.SetDirty(peelableRenderer);
        EditorUtility.SetDirty(gameObject);
    }

    public void SetPowerSpeed(float _power, float _speed)
    {
        power = _power;
        speedAmount = _speed;
    }
#endif

    public void LoadCollectable(PeelableData peelableData)
    {
        collectablePosition = peelableData.CollectablePosition;
        collectableRotation = peelableData.CollectableRotation;
        Collectable collectable = CollectablesPooler.Instance.GetCollectable(collectableType, transform.position);
        collectable.peelable = this;
        collectable.transform.position = collectablePosition;
        collectable.transform.localEulerAngles = collectableRotation;
        collectable.loadCollectableShape();
    }

    public void OnCollect()
    {
        GameManager.instance.currentZone.SavePeelable(index, true, true, collectablePosition, collectableRotation);
    }
}
