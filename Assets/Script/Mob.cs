using UnityEngine;
using System.Collections;

public class Mob : MonoBehaviour
{
  public float m_speed = 0.5f;
  public float m_health = 100.0f;
  public float m_damage = 10.0f;
  public GameObject m_target;

  private Animator m_animator;
  private Rigidbody2D m_body2d;
  private int m_facingDirection = 1;
  private int m_currentAttack = 0;
  private float m_timeSinceAttack = 0.0f;
  private float m_delayToIdle = 0.0f;

  /**
   * AnimState values:
   * 0 - Idle
   * 1 - Walking
   * 2 - Light Attack
   * 3 - Heavy Attack
   * 4 - Hurt
   * 5 - Death
   */

  // Start is called before the first frame update
  void Start()
  {
    m_animator = GetComponent<Animator>();
    m_body2d = GetComponent<Rigidbody2D>();
  }

  // Update is called once per frame
  void Update()
  {
    // Check if mob is dead
    if (m_health <= 0)
    {
      m_animator.SetInteger("AnimState", 5);
      return;
    }

    // Calculate the vector pointing from our position to the target
    Vector2 direction = m_target.transform.position - transform.position;

    // Swap direction of sprite depending on walk direction
    if (direction.x > 0)
    {
      GetComponent<SpriteRenderer>().flipX = false;
      m_facingDirection = 1;
    }
    else if (direction.y < 0)
    {
      GetComponent<SpriteRenderer>().flipX = true;
      m_facingDirection = -1;
    }

    // move the character
    if (Mathf.Abs(direction.x) > Mathf.Epsilon || Mathf.Abs(direction.y) > Mathf.Epsilon)
    {
      m_delayToIdle = 0.01f;
      m_animator.SetInteger("AnimState", 1);
      m_body2d.velocity = new Vector2(m_speed * direction.x, m_speed * direction.y);
      return;
    }

    // Prevents flickering transitions to idle
    m_delayToIdle -= Time.deltaTime;
    if (m_delayToIdle < 0)
      m_animator.SetInteger("AnimState", 0);
  }
}
