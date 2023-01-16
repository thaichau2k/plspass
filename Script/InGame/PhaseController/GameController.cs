using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public static class Configs
{
  public static float buildTime = 60f;
  public static float playTime = 450f;
  public static int winScore = 100;
  public static int[] point = { 20, 15, 10 };
}

public class GameController : MonoBehaviourPunCallbacks
{
  public static GameController instance;

  [SerializeField] private List<GameObject> phases;

  private NetworkManager networkManager;
  private BuildPhaseController buildManager;
  private PlayPhaseController playManager;
  private ScorePhaseController scoreManager;

  private string gameState;
  private Dictionary<string, string> playerStates;
  private float countdownTimer;
  private bool isCountdown = false;

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
    //Get all singleton
    networkManager = NetworkManager.instance;
    buildManager = BuildPhaseController.instance;
    playManager = PlayPhaseController.instance;
    scoreManager = ScorePhaseController.instance;

    //Init state info
    gameState = "build";
    playerStates = new Dictionary<string, string>();
    foreach (Player p in networkManager.PlayerList)
      playerStates.Add(p.NickName, "build");

    UpdateState(gameState);
  }

  private void CheckReadyUpdateState(string state)
  {
    if (!networkManager.IsMasterClient) return;

    foreach (string value in playerStates.Values)
    {
      if (value != state) return;
    }

    isUpdatingState = true;
    isCountdown = false;
    ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable { { "state", state } };
    NetworkManager.instance.CurrentRoom.SetCustomProperties(roomProps);
  }

  public void UpdatePlayerState(string playerName, string state)
  {
    if (playerStates.ContainsKey(playerName))
      playerStates[playerName] = state;

    CheckReadyUpdateState(state);
  }

  bool isUpdatingState = false;

  private void Update()
  {
    if (!networkManager.IsMasterClient) return;

    if (isCountdown)
    {
      if (countdownTimer > 0) countdownTimer -= Time.deltaTime;
      else if (!isUpdatingState)
      {
        isUpdatingState = true;
        isCountdown = false;
        ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable { { "state", gameState + 1 } };
        NetworkManager.instance.CurrentRoom.SetCustomProperties(roomProps);
      }
    }
  }

  public void UpdateState(string state)
  {
    gameState = state;
    switch (state)
    {
      case "build":
        buildManager.Init();
        countdownTimer = Configs.buildTime;
        isCountdown = true;
        break;
      case "play":
        buildManager.HandleBuildPhaseObject(false);
        playManager.Init();
        countdownTimer = Configs.playTime;
        isCountdown = true;
        break;
      case "score":
        break;
      case "over": break;
    }

    isUpdatingState = false;
  }

  public string GetGameState()
  {
    return gameState;
  }
}
