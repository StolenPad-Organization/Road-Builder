using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Duplicator : MonoBehaviour
{
    public Transform V1Parent;
    public Transform V2Parent;

    [ContextMenu("Set Versions")]
    private void SetDuplicate()
    {
        List<GameObject> V1list = new List<GameObject>();
        foreach (Transform child in V1Parent)
        {
            V1list.Add(child.gameObject);
        }
        List<GameObject> V2list = new List<GameObject>();

        foreach (Transform child in V2Parent)
        {
            V2list.Add(child.gameObject);
        }

        for (int i = 0; i < V1list.Count; i++)
        {
            if (V1list[i].GetComponent<RBHandler>())
                DestroyImmediate(V1list[i].GetComponent<RBHandler>());
          var v1 =  V1list[i].AddComponent<RBHandler>();
           V1list[i].GetComponent<Peelable>().SetRBHandler(v1);

            if (V2list[i].GetComponent<MeshRenderer>())
                v1.SetVersion(V2list[i].GetComponent<MeshRenderer>());
        }

        GetRigidbody();
    }

    [ContextMenu("Set RB")]
    private void GetRigidbody()
    {
        foreach (Transform child in V1Parent)
        {
            var rb = child.GetComponent<Rigidbody>();
            var rbHandler = child.GetComponent<RBHandler>();
            rbHandler.SetRB(rb);
        }
    }
}
