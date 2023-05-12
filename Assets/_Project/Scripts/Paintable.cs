using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class Paintable : MonoBehaviour
{
    [SerializeField] private Renderer paintableRenderer;
    [SerializeField] private Collider paintableCollider;
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
        PaintPiece();
    }

    private void PaintPiece()
    {
        paintableCollider.enabled = false;
        paintableRenderer.enabled = true;
        transform.position = PlayerController.instance.paintMachine.partsSpawnPoint.position + PlayerController.instance.paintMachine.partsSpawnPoint.right * Random.Range(-0.5f, 0.5f);
        transform.localScale = Vector3.zero;
        transform.DOLocalJump(initialPos, 1.5f, 1, 0.5f);
        transform.DOLocalRotate(initialRot, 0.5f);
        transform.DOScale(initialscale, 0.5f);
    }

    public void SetPaintableEditor()
    {
        paintableCollider = GetComponent<Collider>();
        paintableRenderer = GetComponent<Renderer>();
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }
}
