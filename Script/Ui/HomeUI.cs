using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class HomeUI : MonoBehaviour
{
  public static HomeUI instance;
  private DatabaseReference db;
  [SerializeField] private Button signOutBtn;

  [Header("User Profile")]
  [SerializeField] private Button profileBtn;
  [SerializeField] private GameObject profileLayer;
  [SerializeField] private TMP_InputField username;
  [SerializeField] private TMP_Text winrate;
  [SerializeField] private Button updateNameBtn;

  [Header("Lobby")]
  [SerializeField] private Button lobbyBtn;
  [SerializeField] private GameObject lobbyLayer;
  [SerializeField] private Button createRoomBtn; //just link to game scene for database right now
  [SerializeField] private TMP_InputField roomName;
  [SerializeField] private GameObject roomItemPrefab;
  [SerializeField] private Transform roomItemHolder;
  private Dictionary<string, GameObject> roomListEntries;
  private List<UserModel> userList;

  private void Awake()
  {
    if (instance == null) instance = this;
    else if (instance != this)
    {
      Destroy(instance.gameObject);
      instance = this;
    }

    roomListEntries = new Dictionary<string, GameObject>();
  }

  void Start()
  {
    db = FirebaseManager.instance.dbReference;
    GetUserData();
    LobbyScreen();
    RegisterEvent();
  }

  private void ClearUi()
  {
    lobbyLayer.SetActive(false);
    profileLayer.SetActive(false);
  }

  private void LobbyScreen()
  {
    ClearUi();
    lobbyLayer.SetActive(true);
  }

  private void ProfileScreen()
  {
    ClearUi();
    GetUserData();
    profileLayer.SetActive(true);
  }

  private void RegisterEvent()
  {
    signOutBtn.onClick.AddListener(() =>
    {
      UserManager.instance.BackToLoginScene();
      NetworkManager.instance.Disconnect();
      GameManager.instance.ChangeScene("LoginScene");
    });

    lobbyBtn.onClick.AddListener(() => { LobbyScreen(); });
    profileBtn.onClick.AddListener(() => { ProfileScreen(); });

    username.onValueChanged.AddListener(name =>
    {
      updateNameBtn.interactable = Validate(name);
    });

    updateNameBtn.onClick.AddListener(() =>
    {
      FirebaseManager.instance.UpdateUsername(username.text, false);
      updateNameBtn.interactable = false;
      UserManager.instance.SetUsername(username.text);
    });

    createRoomBtn.onClick.AddListener(() =>
    {
      if (roomName.text != "" && roomName.text != null)
        NetworkManager.instance.CreateRoom(roomName.text);
    });
  }

  #region User Profile

  public void GetUserData()
  {
    username.text = UserManager.instance.GetUsername();
    winrate.text = "Win rate: " + (double)UserManager.instance.GetWin() +
                   "/" + (double)UserManager.instance.GetTotalGames();
  }

  private bool Validate(string name)
  {
    if (name == "") return false;
    if (name == FirebaseManager.instance._user.DisplayName) return false;

    return true;
  }

  #endregion

  #region  Lobby

  public void UpdateRoomListView()
  {
    foreach (RoomInfo info in NetworkManager.instance.cachedRoomList.Values)
    {
      GameObject entry = Instantiate(roomItemPrefab);
      entry.transform.SetParent(roomItemHolder.transform);
      entry.transform.localScale = Vector3.one;
      entry.GetComponent<RoomInfoItem>().SetRoomName(info.Name);

      roomListEntries.Add(info.Name, entry);
    }
  }

  public void ClearRoomListView()
  {
    foreach (GameObject entry in roomListEntries.Values)
    {
      Destroy(entry.gameObject);
    }

    roomListEntries.Clear();
  }

  #endregion
}


