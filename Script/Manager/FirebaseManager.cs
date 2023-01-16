using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FirebaseManager : MonoBehaviour
{
  public static FirebaseManager instance;

  [Header("Firebase")]
  public FirebaseUser _user;
  public DatabaseReference dbReference;

  private void Awake()
  {
    DontDestroyOnLoad(gameObject);

    if (instance == null) instance = this;
    else if (instance != this)
    {
      Destroy(instance.gameObject);
      instance = this;
    }
  }

  private void Start()
  {
    StartCoroutine(CheckAndFixDependencies());
  }

  #region InitFirebase

  private IEnumerator CheckAndFixDependencies()
  {
    var task = FirebaseApp.CheckAndFixDependenciesAsync();

    yield return new WaitUntil(predicate: () => task.IsCompleted);

    var dependencyResult = task.Result;

    if (dependencyResult == DependencyStatus.Available)
    {
      Debug.Log("Available!!!");
      InitFirebase();
    }
    else Debug.LogError("Could not resolve all Firebase dependencie: " + dependencyResult);
  }

  private void InitFirebase()
  {
    dbReference = FirebaseDatabase.DefaultInstance.RootReference;
  }

  #endregion

  #region Realtime-Database

  private IEnumerator IUpdateUsernameAuth(string _username)
  {
    UserProfile profile = new UserProfile { DisplayName = _username };

    var ProfileTask = _user.UpdateUserProfileAsync(profile);

    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

    if (ProfileTask.Exception != null)
    {
      _user.DeleteAsync();
      //Error Handle
      FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
      AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
    }
    else
    {
      //username updated!
    }
  }

  private IEnumerator IUpdateUsernameDatabase(string _username)
  {
    var DbTask = dbReference.Child("users").Child(_user.UserId).Child("username").SetValueAsync(_username);

    yield return new WaitUntil(predicate: () => DbTask.IsCompleted);

    if (DbTask.Exception != null)
    {
      Debug.Log(message: $"Failed to update username with {DbTask.Exception}");
    }
    else
    {
      //username updated!
    }
  }

  private IEnumerator IUpdateWinCount(int _winCount)
  {
    var DbTask = dbReference.Child("users").Child(_user.UserId).Child("number_of_wins").SetValueAsync(_winCount);

    yield return new WaitUntil(predicate: () => DbTask.IsCompleted);

    if (DbTask.Exception != null)
    {
      Debug.Log(message: $"Failed to update score with {DbTask.Exception}");
    }
    else
    {
      //number of wins updated! 
    }
  }

  private IEnumerator IUpdateTotalGamePlayed(int _total)
  {
    var DbTask = dbReference.Child("users").Child(_user.UserId).Child("games_played").SetValueAsync(_total);

    yield return new WaitUntil(predicate: () => DbTask.IsCompleted);

    if (DbTask.Exception != null)
    {
      Debug.Log(message: $"Failed to update total games with {DbTask.Exception}");
    }
    else
    {
      //number of wins updated! 
    }
  }

  public void UpdateUsername(string name, bool isRegister)
  {
    if (!isRegister)
      StartCoroutine(IUpdateUsernameAuth(name));

    StartCoroutine(IUpdateUsernameDatabase(name));
  }

  public void UpdateUserWinCnt(int winCnt)
  {
    StartCoroutine(IUpdateWinCount(winCnt));
  }

  public void UpdateUserTotalGames(int total)
  {
    StartCoroutine(IUpdateTotalGamePlayed(total));
  }

  #endregion
}
