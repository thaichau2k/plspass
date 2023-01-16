using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private GameObject player;
  [SerializeField] private PhotonView view;
  [SerializeField] private TextMesh playerName;
  [SerializeField] private Transform groundCheck;
  [SerializeField] private LayerMask groundLayer;
  [SerializeField] private Animator ani;
  [SerializeField] private float speed = 7f;
  [SerializeField] private float jumpPower = 12f;

  private enum MovementState
  {
    Idle, Running, Fall, Jump
  };
  private Rigidbody2D rb;
  private float horizontal;
  private bool isFacingRight = true;
  public Vector3 playerSpawnPos { get; set; }

  private void Start()
  {
    SetName();
    rb = GetComponent<Rigidbody2D>();
  }

  private void FixedUpdate()
  {
    if (view.IsMine)
    {
      horizontal = Input.GetAxisRaw("Horizontal");

      //Move left and right
      rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

      //Jump
      if (Input.GetButtonDown("Jump") && IsGrounded())
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);

      //Hold jump button to jump higher
      if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

      Flip();
      UpdateAnimation();
    }
  }

  private void SetName()
  {
    playerName.text = view.Owner.NickName;
  }

  private void UpdateAnimation()
  {
    MovementState state = MovementState.Idle;

    if (IsGrounded())
    {
      if (horizontal != 0) state = MovementState.Running;
      else state = MovementState.Idle;
    }
    else
    {
      if (rb.velocity.y > .1f) state = MovementState.Jump;
      else if (rb.velocity.y < -.1f) state = MovementState.Fall;
    }

    ani.SetInteger("MovementState", (int)state);
  }

  private bool IsGrounded()
  {
    return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
  }

  private void Flip()
  {
    if ((horizontal < 0 && isFacingRight) || (horizontal > 0 && !isFacingRight))
    {
      Vector3 currentScale = player.gameObject.transform.localScale;
      currentScale.x *= -1;
      player.gameObject.transform.localScale = currentScale;

      isFacingRight = !isFacingRight;
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (view.IsMine)
    {
      //death
      if (collision.gameObject.CompareTag("Trap"))
      {
        view.RPC("HandleDeath", RpcTarget.All);
      }
      //win
      else if (collision.gameObject.CompareTag("Goal"))
      {
        view.RPC("HandleWin", RpcTarget.All);
      }
    }
  }

  [PunRPC]
  private void HandleDeath()
  {
    playerName.gameObject.SetActive(false);
    ani.SetTrigger("Death");
    Invoke("DestroyPlayer", 0.8f);
  }

  private void DestroyPlayer()
  {
    GameController.instance.UpdatePlayerState(playerName.text, "score");
    gameObject.SetActive(false);
  }

  [PunRPC]
  private void HandleWin()
  {
    GameController.instance.UpdatePlayerState(playerName.text, "score");
    //win alert to caculate point
  }

  public void SpawnPlayer()
  {
    ani.SetInteger("MovementState", 0);
    playerName.gameObject.SetActive(true);
    gameObject.transform.position = playerSpawnPos;
  }
}
