using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
  [SerializeField] private Animator ani;
  [SerializeField] private float trampolineJumpPower;

  // Start is called before the first frame update
  void Start()
  {
    ani = GetComponent<Animator>();
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject.CompareTag("Player"))
    {
      ani.Play("Jump");
      other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.up * trampolineJumpPower;
    }
  }
}
