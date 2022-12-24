using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//when something get into the alta, make the runes glow
public class PropsAltar : MonoBehaviour
{
  public List<SpriteRenderer> runes;
  public float lerpSpeed;
  public bool isEnd;

  private Color curColor;
  private Color targetColor;

  private void OnTriggerEnter2D(Collider2D other)
  {
    targetColor = new Color(1, 1, 1, 1);

    if (!isEnd) return;

    // TODO show the end screen

    // wait 2 seconds
    StartCoroutine(waiter());
  }

  IEnumerator waiter()
  {
    yield return new WaitForSeconds(3);

    // load first room
    // move to spawn point
    GameObject game_handler = GameObject.Find("Main Camera");
    game_handler.GetComponent<GameHandler>().LoadRoom(0);
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    targetColor = new Color(1, 1, 1, 0);
  }

  private void Update()
  {
    curColor = Color.Lerp(curColor, targetColor, lerpSpeed * Time.deltaTime);

    foreach (var r in runes)
    {
      r.color = curColor;
    }
  }
}

