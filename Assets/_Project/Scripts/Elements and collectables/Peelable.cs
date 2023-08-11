using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Peelable : MonoBehaviour
{
    [SerializeField] private int index;
    public float power;
    [SerializeField] private float speedAmount;
    [SerializeField] private CollectableType collectableType;
    public MeshFilter peelableMeshFilter;
    public MeshRenderer peelableRenderer;
    public bool diffirentCollectable;
    private Vector3 collectablePosition;
    private Vector3 collectableRotation;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController.instance.OnPeelableDetection(speedAmount, power);
    }

    private void OnTriggerStay(Collider other)
    {
        power -= PlayerController.instance.scrapeTool.power;
        PlayerController.instance.scrapeTool.ShakeTool();
        if(power <= 0)
        {
            collectablePosition = transform.position + (PlayerController.instance.GetClosestCheckDirection(transform.position));
            collectablePosition += Vector3.up * Random.Range(0.0f, 0.5f);
            collectableRotation = new Vector3(Random.Range(-25f, 25f), Random.Range(-90f, 90f), Random.Range(-25f, 25f));
            GameManager.instance.currentZone.SavePeelable(index, true, false, collectablePosition, collectableRotation);
            Collectable collectable = CollectablesPooler.Instance.GetCollectable(collectableType, transform.position);
            collectable.peelable = this;
            collectable.Spawn(collectablePosition, collectableRotation);
            gameObject.SetActive(false);
            GameManager.instance.currentZone.OnBlockRemove();
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
