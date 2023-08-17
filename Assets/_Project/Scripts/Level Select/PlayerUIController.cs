using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private LevelSelectController LevelSelectController;
    [SerializeField] private float speed;
    void Start()
    {
        
    }

    public IEnumerator FollowPath(Transform[] levelPath)
    {
        foreach (var item in levelPath)
        {
            yield return transform.DOMove(item.position, speed).SetSpeedBased(true).WaitForCompletion();
        }
        LevelSelectController.ShowPlayButton();
    }
}
