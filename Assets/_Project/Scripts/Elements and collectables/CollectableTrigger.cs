using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableTrigger : MonoBehaviour
{
    [SerializeField] private Collectable collectable;
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer != LayerMask.NameToLayer("ScrapTool")) return;
        //    GameManager.instance.player.OnCollect(collectable);
    }
}
