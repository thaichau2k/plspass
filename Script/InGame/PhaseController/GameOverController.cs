using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
  public static GameOverController instance;

  private NetworkManager networkManager;

  private void Awake()
  {
    if (instance == null) instance = this;
    else if (instance != this)
    {
      Destroy(instance.gameObject);
      instance = this;
    }
  }
}
