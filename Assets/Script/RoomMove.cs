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
      GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");

      if (mobs.Length > 0)
        Debug.Log(mobs[0].name);

      // check if all mobs are dead
      bool all_dead = true;
      foreach (GameObject mob in mobs)
        if (mob.GetComponent<Mob>().enabled)
          all_dead = false;

      if (!all_dead) return;

      GameObject game_handler = GameObject.Find("Main Camera");
      game_handler.GetComponent<GameHandler>().LoadRoom(room_number);
    }
  }
}
