using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBHandler : MonoBehaviour
{
    [SerializeField] private MeshRenderer OtherVersion;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshRenderer meshRenderer;
    public RBTile RBTile;
    public void SetVersion(MeshRenderer version)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        OtherVersion = version;
        OtherVersion.sharedMaterial = meshRenderer.sharedMaterial;
#if UNITY_EDITOR

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(OtherVersion.gameObject);
#endif
    }

    public void SetRB(Rigidbody _rb)
    {
        rb = _rb;
#if UNITY_EDITOR

        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public void CheckSwitch(bool state)
    {
        Switch(state);
    }   
    private void Switch(bool state)//00 01 10 11
    {
        //if (rb.velocity.magnitude > 0.2f || rb.angularVelocity.magnitude > 0.2f)
        //{
        //    state = true;
        //}
        if (gameObject.activeSelf && !OtherVersion.gameObject.activeSelf && state) return;
        if (!gameObject.activeSelf && OtherVersion.gameObject.activeSelf && !state) return;
        
        if (state)
        {
            gameObject.SetActive(true);
            OtherVersion.gameObject.SetActive(false);
        }
        else
        {
            OtherVersion.transform.SetPositionAndRotation(transform.position, transform.rotation);
            gameObject.SetActive(false);
            OtherVersion.gameObject.SetActive(true);
            //OtherVersion.material.color = meshRenderer.material.color;
        }
    }

    public void RemoveFromTile()
    {
        RBTile.rbPositions.Remove(this);
    }

    public void SwitchTile(RBTile newTile)
    {
        RemoveFromTile();
        newTile.rbPositions.Add(this);
        RBTile = newTile;
    }
}
