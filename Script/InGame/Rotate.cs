using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
  [SerializeField] private float speed = 0.1f;

  private void Update()
  {
    transform.Rotate(0, 0, 360 * speed * Time.deltaTime);
  }
}
