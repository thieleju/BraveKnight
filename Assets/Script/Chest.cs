using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chest : MonoBehaviour
{
  public SpriteRenderer spriteRenderer;
  public Sprite openChest;
  public Sprite closedChest;

  private bool used = false;

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player") && !used)
    {
      spriteRenderer.sprite = openChest;

      // heal player
      GameObject player = GameObject.FindWithTag("Player");
      player.GetComponent<HeroKnight>().HealHealth(50);

      // play sound
      FindObjectOfType<AudioManager>().Play("PlayerHeal");

      // show message 
      GameObject ui_manager = GameObject.Find("Modal");
      ui_manager.GetComponent<MessageManager>().ShowInfo("Chest", "You found a small potion and healed 50 health!");

      used = true;
    }
  }

  public void ResetChest()
  {
    spriteRenderer.sprite = closedChest;
    used = false;
  }


}
