//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SelectedTrapController : MonoBehaviour
{
  [SerializeField] private int objType;
  [SerializeField] private Button btn;

  private Player local;

  private void Start()
  {
    local = NetworkManager.instance.LocalPlayer;
    btn.onClick.AddListener(ChooseObject);
  }

  private void ChooseObject()
  {
    //To show that player pick the object and ready for next phase
    Hashtable playerProps = new Hashtable { { "choose", objType } };
    local.SetCustomProperties(playerProps);

    gameObject.SetActive(false);
    BuildPhaseController.instance.MoveInventory(true);
  }
}
