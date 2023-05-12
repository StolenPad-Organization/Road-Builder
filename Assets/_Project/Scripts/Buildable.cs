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
    [SerializeField] private Vector3 initialRot;
    [SerializeField] private Vector3 initialscale;
    void Start()
    {
        initialPos = transform.localPosition;
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
        transform.DOLocalJump(initialPos, 4, 1, 0.5f);
        transform.DOLocalRotate(initialRot, 0.5f);
        transform.DOScale(initialscale, 0.5f);
    }

    public void SetBuildableEditor()
    {
        buildableCollider = GetComponent<Collider>();
        buildableRenderer = GetComponent<Renderer>();
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }
}
