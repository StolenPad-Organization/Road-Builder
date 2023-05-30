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
            GameManager.instance.currentStage.currentZone.SavePeelable(index, true, false);
            Collectable collectable = CollectablesPooler.Instance.GetCollectable(collectableType, transform.position);
            collectable.peelable = this;
            collectable.Spawn();
            gameObject.SetActive(false);
            GameManager.instance.currentStage.currentZone.OnBlockRemove();
        }
    }

    public void SetPeelableEditor(int _index)
    {
        index = _index;
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }

    public void LoadCollectable()
    {
        Collectable collectable = CollectablesPooler.Instance.GetCollectable(collectableType, transform.position);
        collectable.peelable = this;
    }

    public void OnCollect()
    {
        GameManager.instance.currentStage.currentZone.SavePeelable(index, true, true);
    }
}
