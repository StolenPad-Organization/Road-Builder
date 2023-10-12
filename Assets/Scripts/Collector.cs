using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    Player,
    AI
}
public class Collector : MonoBehaviour
{
    [SerializeField] PlayerType playerType;
    public PlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("parts"))
        {
            Peelable p = other.GetComponent<Peelable>();
            if (p.zoneIndex != GameManager.instance.levelProgressData.ZoneIndex) return;

            p.CollectPeeled();

            if (p != null && !p.peeled)
            {
                switch (playerType)
                {
                    case PlayerType.Player:
                        player.OnPeelableDetection(p.speedAmount, p.initialPower, p.dustColor);

                        break;
                    case PlayerType.AI:
                        // 
                        break;
                }
            }

            else if (!p.collected)
            {
                switch (playerType)
                {
                    case PlayerType.Player:
                        player.OnCollect(p);
                        break;
                    case PlayerType.AI:
                        break;
                }
            }
            if(playerType == PlayerType.Player)
            {
                if (player.canDoStrictedHaptic)
                {
                    EventManager.invokeHaptic.Invoke(vibrationTypes.MediumImpact);
                    player.canDoStrictedHaptic = false;
                }
            }
       
        }
        else
        {
            PeelableCopy pc = other.GetComponent<PeelableCopy>();
            if(pc != null)
            {
                var p = pc.peelable;
                if (p.zoneIndex != GameManager.instance.levelProgressData.ZoneIndex) return;
                switch (playerType)
                {
                    case PlayerType.Player:
                        player.OnCollect(p);
                        break;
                    case PlayerType.AI:
                        break;
                }
            }

        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("parts"))
        {
            Peelable p = other.GetComponent<Peelable>();
            if (p.zoneIndex != GameManager.instance.levelProgressData.ZoneIndex) return;

            switch (playerType)
            {
                case PlayerType.Player:

                    p.PeeledStay(player.scrapeTool.power);
                    player.SetScrapingMovementSpeed(p.speedAmount, p.initialPower);

                    break;
                case PlayerType.AI:
                    break;
            }
           
        }
    }
}
