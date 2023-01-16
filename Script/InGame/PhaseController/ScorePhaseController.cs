using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class ScorePhaseController : MonoBehaviour
{
  public static ScorePhaseController instance;
  private NetworkManager networkManager;

  private Dictionary<string, int> playerScores;

  private void Awake()
  {
    if (instance == null) instance = this;
    else if (instance != this)
    {
      Destroy(instance.gameObject);
      instance = this;
    }
  }

  private void Start()
  {
    networkManager = NetworkManager.instance;

    playerScores = new Dictionary<string, int>();
    foreach (Player p in networkManager.PlayerList)
      playerScores.Add(p.NickName, 0);
  }


}
