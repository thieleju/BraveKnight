using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : MonoBehaviour
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
  public Collider2D attack_hitbox_right;
  public Collider2D attack_hitbox_left;

  public Animator animator;
  public Rigidbody2D body2d;
  public HealthBar healthBar;

  public LayerMask playerLayers;
  public string playerHurtboxTag = "PlayerHurtbox";

  public float health_max = 600.0f;
  public float health = 0.0f;
  public float speed = 0.4f;
  public float attack_damage = 40f;

  /* Private variables */
  private List<Collider2D> attack_collider_right = new List<Collider2D>();
  private List<Collider2D> attack_collider_left = new List<Collider2D>();

  private float delay_to_idle = 0.0f;
  private float delay_attack_hit = 0.0f;
  private float delay_attack_cooldown = 0.0f;

  private const float IDLE_DELAY = 0.01f;
  private const float ATTACK_HIT_DELAY = 0.35f;
  private const float ATTACK_COOLDOWN_DELAY = 1.5f;

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

    // Detect collision with polygon colliders in playerLayers in range of attack
    // check which hitObject has the gameobject with the tag "Player"
    foreach (Collider2D hitObject in GetAttackCollider())
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
    animator.SetTrigger("Attack");

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
      mob_facing_left = true;
    else if (direction_to_target.y < 0)
      mob_facing_left = false;

    // set sprite flip on x axis
    GetComponent<SpriteRenderer>().flipX = mob_facing_left;
  }

  void AttackHit()
  {
    // Detect collision with colliders in playerLayers in range of attack
    foreach (Collider2D hitObject in GetAttackCollider())
    {
      if (hitObject.gameObject.tag == playerHurtboxTag)
      {
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

    // destroy this gameobject after 1 second
    Destroy(gameObject, 1.0f);
  }

  bool PlayerisDead()
  {
    return !GameObject.FindWithTag("Player").GetComponent<HeroKnight>().IsAlive();
  }

  public bool IsAlive()
  {
    return !isDead;
  }

  List<Collider2D> GetAttackCollider()
  {
    if (!mob_facing_left)
      return attack_collider_right;
    else
      return attack_collider_left;
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (!mob_facing_left)
      attack_collider_right.Add(other);
    else
      attack_collider_left.Add(other);
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    if (!mob_facing_left)
      attack_collider_right.Remove(other);
    else
      attack_collider_left.Remove(other);
  }
}

