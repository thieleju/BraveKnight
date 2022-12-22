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
      GameObject game_handler = GameObject.Find("Main Camera");
      game_handler.GetComponent<GameHandler>().LoadRoom(room_number);
    }
  }
}
