using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gravitons.UI.Modal;

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
        return;
      }
      // show message 
      GameObject ui_manager = GameObject.Find("Demo");
      ui_manager.GetComponent<DemoManager>().ShowInfo("Locked", "You need to kill all mobs to progress!");
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
