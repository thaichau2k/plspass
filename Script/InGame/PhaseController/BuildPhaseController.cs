using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class BuildPhaseController : MonoBehaviour
{
  public static BuildPhaseController instance;

  private NetworkManager _networkInstance => NetworkManager.instance;

  [SerializeField] private GameObject playerCursorPref;
  [SerializeField] private GameObject Inventory;
  [SerializeField] private Transform CursorHolder;
  [SerializeField] private Transform ObjectHolder;

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
    DOTween.Init();
    Inventory.transform.position = hideInvenPos.position;
    GameObject playerCur = PhotonNetwork.Instantiate(playerCursorPref.name, Vector3.zero, Quaternion.identity);

    playerCur.transform.SetParent(CursorHolder);
  }

  public void Init()
  {
    HandleBuildPhaseObject(true);
    ResetInventory();
  }

  public void HandleBuildPhaseObject(bool isShow)
  {
    Inventory.SetActive(isShow);
    foreach (Transform child in CursorHolder)
    {
      child.gameObject.SetActive(isShow);
    }
  }

  public void SetChosenObjectParent(GameObject obj)
  {
    obj.transform.SetParent(ObjectHolder);
  }

  #region Inventory

  [SerializeField] private Transform hideInvenPos;
  [SerializeField] private Transform showInvenPos;

  public void MoveInventory(bool isHide)
  {
    if (isHide)
      Inventory.transform.DOMove(hideInvenPos.position, 2f);
    else Inventory.transform.DOMove(showInvenPos.position, 2f);
  }

  private void ResetInventory()
  {
    foreach (Transform obj in Inventory.transform)
    {
      obj.gameObject.SetActive(true);
    }

    MoveInventory(false);
  }

  #endregion
}
