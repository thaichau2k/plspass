using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHeadController : MonoBehaviour
{
  [SerializeField] private GameObject spikeHolders;

  private float switchSpikeTime = 4f;
  private float cntdwn;

  private void Start()
  {
    cntdwn = switchSpikeTime;
    spikeHolders.SetActive(false);
  }

  private void Update()
  {
    if (cntdwn > .1f)
      cntdwn -= Time.deltaTime;
    else
    {
      spikeHolders.SetActive(!spikeHolders.activeSelf);
      cntdwn = switchSpikeTime;
    }
  }
}
