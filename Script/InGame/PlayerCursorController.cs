using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerCursorController : MonoBehaviourPunCallbacks
{
  [SerializeField] private PhotonView view;
  [SerializeField] private TextMesh playerName;
  [SerializeField] private GameObject stopAlert;
  [SerializeField] private List<GameObject> objs;
  [SerializeField] private List<GameObject> objPrefs;

  private Camera mainCam;
  private Vector3 mousePosition;
  private int objIndex;
  private bool isOnProhibitedZone = false;

  // Start is called before the first frame update
  void Start()
  {
    objIndex = -1;
    mainCam = Camera.main;
    playerName.text = view.Owner.NickName;
    Clear();
  }

  // Update is called once per frame
  void Update()
  {
    if (view.IsMine)
    {
      mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
      mousePosition.z = -9f;

      gameObject.transform.position = mousePosition;

      if (GameController.instance.GetGameState() == "build" && objIndex >= 0)
      {
        if (isOnProhibitedZone) stopAlert.SetActive(true);
        else
        {
          stopAlert.SetActive(false);

          if (Input.GetMouseButtonDown(0))
            view.RPC("Build", RpcTarget.All, objIndex, mousePosition, view.Owner);
        }
      }
    }
  }

  private void Clear()
  {
    stopAlert.SetActive(false);
    foreach (GameObject obj in objs) obj.SetActive(false);
  }

  public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
  {
    object prop;
    if (changedProps.TryGetValue("choose", out prop))
    {
      if (view.Owner == targetPlayer)
      {
        objIndex = (int)prop;
        objs[objIndex].SetActive(true);
      }
    }
  }

  [PunRPC]
  private void Build(int index, Vector3 pos, Player player)
  {
    GameObject obj = Instantiate(objPrefs[index], pos, Quaternion.identity);
    BuildPhaseController.instance.SetChosenObjectParent(obj);

    if (view.Owner == player)
    {
      objIndex = -1;
      Clear();
      gameObject.SetActive(false);
    }

    GameController.instance.UpdatePlayerState(player.NickName, "play");
  }

  private void OnTriggerStay2D(Collider2D other)
  {
    if (other.CompareTag("ProhibitedZone"))
    {
      isOnProhibitedZone = true;
    }
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    if (other.CompareTag("ProhibitedZone")) isOnProhibitedZone = false;
  }
}
