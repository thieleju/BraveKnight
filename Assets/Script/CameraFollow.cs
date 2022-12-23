using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
  public float lerpSpeed = 10.0f;
  public float offset_x = 0.0f;
  public float offset_y = 1.8f;

  private Transform target;
  private Vector3 offset;
  private Vector3 targetPos;

  public void SetTarget(Transform newTarget)
  {
    target = newTarget;
  }

  private void Update()
  {
    if (target == null) return;

    targetPos = target.position + new Vector3(offset_x, offset_y, 0);
    transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
  }

}
