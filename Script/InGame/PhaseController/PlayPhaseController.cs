using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;
public class PlayPhaseController : MonoBehaviour
{
  public static PlayPhaseController instance;

  [SerializeField] private GameObject playerPref;
  [SerializeField] private Transform playerHolder;
  [SerializeField] private Transform spawnPos;

  private Vector3 randomSpawnPos;

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
    float randomXPos = Random.Range(spawnPos.position.x - 3, spawnPos.position.x + 3);
    randomSpawnPos = new Vector3(randomXPos, spawnPos.position.y, spawnPos.position.z);

    GameObject newPlayer = PhotonNetwork.Instantiate(playerPref.name, randomSpawnPos, Quaternion.identity);
    newPlayer.transform.SetParent(playerHolder);
    newPlayer.GetComponent<PlayerController>().playerSpawnPos = randomSpawnPos;
    newPlayer.SetActive(false);
  }

  public void Init()
  {
    //fx toon appear

    foreach (GameObject player in playerHolder)
    {
      player.SetActive(true);
      player.GetComponent<PlayerController>().SpawnPlayer();
    }
  }
}
