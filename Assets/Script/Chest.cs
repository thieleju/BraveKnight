using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
  public SpriteRenderer spriteRenderer;
  public Sprite openChest;

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      spriteRenderer.sprite = openChest;

      // heal player
      GameObject player = GameObject.FindWithTag("Player");
      player.GetComponent<HeroKnight>().HealHealth(50);

      // TODO show message 
    }
  }


}
