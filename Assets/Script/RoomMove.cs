using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomMove : MonoBehaviour
{
  public int room_number = 0;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      if (CanPlayerProgress())
      {
        GameObject game_handler = GameObject.Find("Main Camera");
        game_handler.GetComponent<GameHandler>().LoadRoom(room_number);

        // play sound
        FindObjectOfType<AudioManager>().Play("RoomMove");

        return;
      }
      // show message 
      GameObject ui_manager = GameObject.Find("Modal");
      ui_manager.GetComponent<MessageManager>().ShowInfo("Locked", "You need to kill all mobs to progress!");
    }
  }

  private bool CanPlayerProgress()
  {
    GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");

    // check if all mobs are dead
    bool all_dead = true;
    foreach (GameObject mob in mobs)
      if (mob.GetComponent<Mob>().enabled)
        all_dead = false;

    return all_dead;
  }
}
