using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class Paintable : MonoBehaviour
{
    [SerializeField] private int index;
    public Renderer paintableRenderer;
    [SerializeField] private Collider paintableCollider;
    [SerializeField] private Vector3 initialPos;
    [SerializeField] private Vector3 initialRot;
    [SerializeField] private Vector3 initialscale;
    bool fullyPainted;
    void Start()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localEulerAngles;
        initialscale = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.player.paintMachine != null && !fullyPainted)
        {
            if (other.CompareTag("FootTrigger")) return;
            if (GameManager.instance.player.paintMachine.UsePaint())
                PaintPiece();
        }
        if (fullyPainted && other.CompareTag("FootTrigger"))
        {
            GameManager.instance.player.ActivateFootPrints(true);
        }
    }

    private void PaintPiece()
    {
        fullyPainted = true;
        GameManager.instance.player.movementController.SetSpeedMultiplayer(50);
        GameManager.instance.currentZone.SavePaintable(index, true);
        //paintableCollider.enabled = false;
        paintableRenderer.enabled = true;
        transform.position = GameManager.instance.player.paintMachine.partsSpawnPoint.position + GameManager.instance.player.paintMachine.partsSpawnPoint.right * Random.Range(-0.5f, 0.5f);
        transform.localScale = Vector3.zero;
        transform.DOLocalMove(initialPos, 0.1f).OnComplete(() =>
        {
            GameManager.instance.currentZone.OnRoadPaint();

            //Material mat = paintableRenderer.material;
            //float t = 1.0f;
            //DOTween.To(() => t, x => t = x, 0.0f, GameManager.instance.player.paintMachine.paintDuration)
            //   .OnUpdate(() => mat.SetFloat("_Animation", t)).SetDelay(GameManager.instance.player.paintMachine.paintDelay);
            GameManager.instance.currentZone.paintableManager.OnPaint(transform.position);
        });
        transform.DOLocalRotate(initialRot, 0.1f);
        transform.DOScale(initialscale, 0.1f);

        if (GameManager.instance.player.canDoStrictedHaptic)
        {
            EventManager.invokeHaptic.Invoke(vibrationTypes.LightImpact);
            GameManager.instance.player.canDoStrictedHaptic = false;
        }
    }

#if UNITY_EDITOR
    public void SetPaintableEditor(int _index)
    {
        index = _index;
        paintableCollider = GetComponent<Collider>();
        paintableRenderer = GetComponent<Renderer>();
        paintableRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        paintableRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        paintableRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        paintableRenderer.allowOcclusionWhenDynamic = false;
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(paintableRenderer);
        EditorUtility.SetDirty(gameObject);
    }

    public void SetMaterialEditor(Material mat)
    {
        paintableRenderer.material = mat;
        EditorUtility.SetDirty(paintableRenderer);
        EditorUtility.SetDirty(gameObject);
    }
#endif

    public void LoadPaintable(bool check)
    {
        //paintableCollider.enabled = false;
        paintableRenderer.enabled = true;
        if(check)
            GameManager.instance.currentZone.OnRoadPaint();
        Material mat = paintableRenderer.material;
        mat.SetFloat("_Animation", 0.0f);
        fullyPainted = true;
    }
}
