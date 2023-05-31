using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class Paintable : MonoBehaviour
{
    [SerializeField] private int index;
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
        if (PlayerController.instance.paintMachine != null)
        {
            if (PlayerController.instance.paintMachine.UsePaint())
                PaintPiece();
        }
    }

    private void PaintPiece()
    {
        GameManager.instance.currentStage.currentZone.SavePaintable(index, true);
        paintableCollider.enabled = false;
        paintableRenderer.enabled = true;
        transform.position = PlayerController.instance.paintMachine.partsSpawnPoint.position + PlayerController.instance.paintMachine.partsSpawnPoint.right * Random.Range(-0.5f, 0.5f);
        transform.localScale = Vector3.zero;
        transform.DOLocalMove(initialPos, 0.1f).OnComplete(() =>
        {
            GameManager.instance.currentStage.currentZone.OnRoadPaint();

            Material mat = paintableRenderer.material;
            float t = 1.0f;
            DOTween.To(() => t, x => t = x, 0.0f, PlayerController.instance.paintMachine.paintDuration)
               .OnUpdate(() => mat.SetFloat("_Animation", t)).SetDelay(PlayerController.instance.paintMachine.paintDelay);
        });
        transform.DOLocalRotate(initialRot, 0.1f);
        transform.DOScale(initialscale, 0.1f);
    }

    public void SetPaintableEditor(int _index)
    {
        index = _index;
        paintableCollider = GetComponent<Collider>();
        paintableRenderer = GetComponent<Renderer>();
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }

    public void LoadPaintable(bool check)
    {
        paintableCollider.enabled = false;
        paintableRenderer.enabled = true;
        if(check)
            GameManager.instance.currentStage.currentZone.OnRoadPaint();
        Material mat = paintableRenderer.material;
        mat.SetFloat("_Animation", 0.0f);
    }
}
