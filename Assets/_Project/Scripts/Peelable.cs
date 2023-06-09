using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Peelable : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private float power;
    [SerializeField] private float speedAmount;
    [SerializeField] private CollectableType collectableType;
    public MeshFilter peelableMeshFilter;
    public MeshRenderer peelableRenderer;

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
        if(power <= 0)
        {
            GameManager.instance.currentZone.SavePeelable(index, true, false);
            Collectable collectable = CollectablesPooler.Instance.GetCollectable(collectableType, transform.position);
            collectable.peelable = this;
            collectable.Spawn();
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

    public void LoadCollectable()
    {
        Collectable collectable = CollectablesPooler.Instance.GetCollectable(collectableType, transform.position);
        collectable.peelable = this;
        collectable.loadCollectableShape();
    }

    public void OnCollect()
    {
        GameManager.instance.currentZone.SavePeelable(index, true, true);
    }
}
