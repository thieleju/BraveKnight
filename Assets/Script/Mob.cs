using UnityEngine;
using System.Collections;

public class Mob : MonoBehaviour
{
  /**
   * AnimState values:
   * 0 - Idle
   * 1 - Walking
   * Trigger:
   * - AttackLight
   * - AttackHeavy
   * - Hurt
   * - Death
   */

  /* Public variables */
  public Transform attack_point_right;
  public Transform attack_point_left;

  public Animator animator;
  public Rigidbody2D body2d;
  public HealthBar healthBar;

  public LayerMask playerLayers;
  public string playerHurtboxTag = "PlayerHurtbox";

  public float health_max = 100.0f;
  public float health = 0.0f;
  public float speed = 0.5f;
  public float attack_range = 0.7f;
  public float attack_damage = 40f;

  /* Private variables */
  private float delay_to_idle = 0.0f;
  private float delay_attack_hit = 0.0f;
  private float delay_attack_cooldown = 0.0f;

  private const float IDLE_DELAY = 0.01f;
  private const float ATTACK_HIT_DELAY = 0.70f;
  private const float ATTACK_COOLDOWN_DELAY = 0.7f;

  private int currentAttack = 0;
  private bool isAttacking = false;
  private bool isDead = false;
  private bool mob_facing_left = true;
  private Vector2 direction_to_target = new Vector2(0, 0);

  // Start is called before the first frame update
  void Start()
  {
    health = health_max;
    healthBar.SetMaxHealth(health_max);
  }

  // Update is called once per frame
  void Update()
  {
    // flip sprite and set variable direction_to_target
    SetFacingDirection();

    // reduce delay timers
    if (delay_attack_hit > 0)
      delay_attack_hit -= Time.deltaTime;

    if (delay_attack_cooldown > 0)
      delay_attack_cooldown -= Time.deltaTime;

    // execute hit of attack when delay is over
    if (delay_attack_hit < 0 && isAttacking)
      AttackHit();

    if (MobShouldStartAttack())
      MobAttack();
    else if (MobShouldStartMoving())
      MobMovement();
    else
    { // IDLE
      // Prevents flickering transitions to idle
      delay_to_idle -= Time.deltaTime;
      if (delay_to_idle < 0)
        animator.SetInteger("AnimState", 0);
    }
  }

  bool MobShouldStartAttack()
  {
    if (PlayerisDead()) return false;

    if (isAttacking) return false;

    if (delay_attack_cooldown > 0) return false;

    Transform attackPoint;
    if (mob_facing_left)
      attackPoint = attack_point_left;
    else
      attackPoint = attack_point_right;

    // Detect collision with colliders in playerLayers in range of attack
    Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attack_range, playerLayers);

    // check which hitObject has the gameobject with the tag "Player"
    foreach (Collider2D hitObject in hitObjects)
    {
      if (hitObject.gameObject.tag == playerHurtboxTag)
      {
        return true;
      }
    }
    return false;
  }

  void MobAttack()
  {
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
    // stop movement velocity
    body2d.velocity = new Vector2(0, 0);

    // start attacking
    isAttacking = true;
    delay_attack_hit = ATTACK_HIT_DELAY;
    delay_to_idle = IDLE_DELAY;
  }


  bool MobShouldStartMoving()
  {
    if (PlayerisDead()) return false;
    if (isAttacking) return false;
    return Mathf.Abs(direction_to_target.x) > Mathf.Epsilon ||
           Mathf.Abs(direction_to_target.y) > Mathf.Epsilon;
  }

  void MobMovement()
  {
    animator.SetInteger("AnimState", 1);
    body2d.velocity = new Vector2(speed * direction_to_target.x, speed * direction_to_target.y);
    delay_to_idle = IDLE_DELAY;
  }

  void SetFacingDirection()
  {
    // Calculate the vector pointing from our position to the target
    GameObject target = GameObject.FindWithTag("Player");
    direction_to_target = target.transform.position - transform.position;
    // Swap direction of sprite depending on walk direction
    if (direction_to_target.x > 0)
      mob_facing_left = false;
    else if (direction_to_target.y < 0)
      mob_facing_left = true;
    // set sprite flip on x axis
    GetComponent<SpriteRenderer>().flipX = mob_facing_left;
  }

  void AttackHit()
  {
    Transform attackPoint;
    if (mob_facing_left)
      attackPoint = attack_point_left;
    else
      attackPoint = attack_point_right;

    // Detect collision with colliders in playerLayers in range of attack
    Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attack_range, playerLayers);

    // check which hitObject has the gameobject with the tag "Mob"
    foreach (Collider2D hitObject in hitObjects)
    {
      if (hitObject.gameObject.tag == playerHurtboxTag)
      {
        // Damage Player
        hitObject.gameObject.transform.parent.gameObject.GetComponent<HeroKnight>().TakeDamage(attack_damage);
      }
    }
    isAttacking = false;
    delay_attack_cooldown = ATTACK_COOLDOWN_DELAY;
  }

  public void TakeDamage(float damage)
  {
    health -= damage;
    healthBar.SetHealth(health);

    if (health <= 0)
    {
      MobDeath();
      return;
    }

    if (!isAttacking)
      animator.SetTrigger("Hurt");
  }

  void MobDeath()
  {
    animator.SetTrigger("Hurt");
    animator.SetBool("isDead", true);
    this.enabled = false;
    isDead = true;

    // Disable colliders for mob and hurtbox
    GetComponent<Collider2D>().enabled = false;
    transform.Find("Hurtbox").GetComponent<Collider2D>().enabled = false;
    // disable healthbar
    healthBar.gameObject.SetActive(false);

    Debug.Log("Mob died");
  }

  bool PlayerisDead()
  {
    return !GameObject.FindWithTag("Player").GetComponent<HeroKnight>().IsAlive();
  }

  void OnDrawGizmosSelected()
  {
    if (mob_facing_left)
      Gizmos.DrawWireSphere(attack_point_left.position, attack_range);
    if (!mob_facing_left)
      Gizmos.DrawWireSphere(attack_point_right.position, attack_range);
  }

  public bool IsAlive()
  {
    return !isDead;
  }
}
