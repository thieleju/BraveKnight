using UnityEngine;
using System.Collections;

public class Mob : MonoBehaviour
{
  public Transform attackPointRight;
  public Transform attackPointLeft;

  public float attackRange = 0.7f;
  public float attackDamage = 40f;

  public LayerMask enemyLayers;
  public string playerHurtboxTag = "Player";

  public float speed = 0.5f;
  public float health_max = 100.0f;
  public float health = 0.0f;
  public float damage = 10.0f;
  public GameObject target;

  public Animator animator;
  public Rigidbody2D body2d;

  private int facingDirection = 1;
  private int currentAttack = 0;
  private float timeSinceAttack = 0.0f;
  private float delayToIdle = 0.0f;
  private float delayToAttack = 0.0f;

  private bool isAttacking = false;

  /**
   * AnimState values:
   * 0 - Idle
   * 1 - Walking
   * Trigger:
   * AttackLight
   * AttackHeavy
   * Hurt
   * Death
   */

  // Start is called before the first frame update
  void Start()
  {
    health = health_max;
  }

  // Update is called once per frame
  void Update()
  {
    // reduce delay to attack
    if (delayToAttack > 0)
    {
      delayToAttack -= Time.deltaTime;
    }
    else if (isAttacking)
    {
      Attack();
    }

    // Calculate the vector pointing from our position to the target
    Vector2 direction = target.transform.position - transform.position;

    // Swap direction of sprite depending on walk direction
    if (direction.x > 0)
    {
      GetComponent<SpriteRenderer>().flipX = false;
      facingDirection = 1;
    }
    else if (direction.y < 0)
    {
      GetComponent<SpriteRenderer>().flipX = true;
      facingDirection = -1;
    }

    // Attack
    if (Input.GetKeyDown(KeyCode.Space))
    {
      // stop movement velocity
      body2d.velocity = new Vector2(0, 0);

      // start attack animation
      if (currentAttack == 0)
      {
        currentAttack = 1;
        animator.SetTrigger("AttackLight");
      }
      else
      {
        currentAttack = 0;
        animator.SetTrigger("AttackHeavy");
      }

      isAttacking = true;
      delayToAttack = 0.70f;
      delayToIdle = 2.01f;
    }
    // move the character
    // else if (Mathf.Abs(direction.x) > Mathf.Epsilon || Mathf.Abs(direction.y) > Mathf.Epsilon)
    // {
    //   delayToIdle = 0.01f;
    //   animator.SetInteger("AnimState", 1);
    //   body2d.velocity = new Vector2(speed * direction.x, speed * direction.y);
    //   return;
    // }
    else
    {
      // Prevents flickering transitions to idle
      delayToIdle -= Time.deltaTime;
      if (delayToIdle < 0)
        animator.SetInteger("AnimState", 0);
    }
  }

  void Attack()
  {
    if (facingDirection == 1)
      AttackCollision(attackPointRight);
    else
      AttackCollision(attackPointLeft);

    // Reset timer
    timeSinceAttack = 0.0f;
    isAttacking = false;
  }

  void AttackCollision(Transform attackPoint)
  {
    // Detect collision with colliders in enemyLayers in range of attack
    Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

    // check which hitObject has the gameobject with the tag "Mob"
    foreach (Collider2D hitObject in hitObjects)
    {
      if (hitObject.gameObject.tag == playerHurtboxTag)
      {
        // Damage enemies
        hitObject.gameObject.transform.parent.gameObject.GetComponent<HeroKnight>().TakeDamage(attackDamage);
      }
    }
  }

  public void TakeDamage(float damage)
  {
    health -= damage;

    // take damage
    animator.SetTrigger("Hurt");
    Debug.Log("Mob took " + damage + " damage");

    // check if mob is dead
    if (health <= 0)
    {
      animator.SetBool("isDead", true);
      this.enabled = false;
      // Disable colliders for mob and hurtbox
      GetComponent<Collider2D>().enabled = false;
      transform.Find("Hurtbox").GetComponent<Collider2D>().enabled = false;

      Debug.Log("Mob died");
      return;
    }
  }

  void OnDrawGizmosSelected()
  {
    if (attackPointRight != null && facingDirection == 1)
      Gizmos.DrawWireSphere(attackPointRight.position, attackRange);
    if (attackPointLeft != null && facingDirection == -1)
      Gizmos.DrawWireSphere(attackPointLeft.position, attackRange);
  }
}
