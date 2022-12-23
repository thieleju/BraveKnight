using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour
{
  /* Public variables */
  public Transform attackPointRight;
  public Transform attackPointLeft;

  public Animator animator;
  public Rigidbody2D body2d;
  public HealthBar healthBar;

  public LayerMask enemyLayers;
  public string enemyHurtboxTag = "MobHurtbox";

  public float speed = 4.0f;
  public float attack_range = 0.7f;
  public float attack_damage = 40f;
  public float health_max = 100.0f;
  public float health = 0.0f;

  /* Private variables */
  private int facingDirection = 1;
  private int currentAttack = 0;
  private float respawn_delay = 3.0f;
  private float timeSinceAttack = 0.0f;
  private float delayToIdle = 0.0f;
  private float inputX_damped = 0.0f;
  private float inputY_damped = 0.0f;
  private bool isDead = false;

  // Use this for initialization
  void Start()
  {
    health = health_max;
    healthBar.SetMaxHealth(health_max);
  }

  // Update is called once per frame
  void Update()
  {
    // Increase timer that controls attack combo
    timeSinceAttack += Time.deltaTime;

    // -- Handle input and movement --
    float inputX = Input.GetAxis("Horizontal");
    float inputY = Input.GetAxis("Vertical");

    // Swap direction of sprite depending on walk direction
    if (inputX > 0 && !isDead)
    {
      GetComponent<SpriteRenderer>().flipX = false;
      facingDirection = 1;
    }
    else if (inputX < 0 && !isDead)
    {
      GetComponent<SpriteRenderer>().flipX = true;
      facingDirection = -1;
    }

    // Move character if not attacking and not dead
    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") == false &&
        animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") == false &&
        animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") == false &&
        !isDead)
    {
      body2d.velocity = new Vector2(inputX * speed, inputY * speed);
    }

    //Death
    // if (Input.GetKeyDown("e"))
    // {
    //   animator.SetTrigger("Death");
    // }
    // //Hurt
    // else if (Input.GetKeyDown("q"))
    // {
    //   animator.SetTrigger("Hurt");
    // }
    //Attack
    if (Input.GetMouseButtonDown(0) && timeSinceAttack > 0.25f && !isDead)
    {
      Attack();
      // stop movement velocity
      body2d.velocity = new Vector2(0, 0);
      delayToIdle = 1.01f;
    }
    //Run
    else if ((Mathf.Abs(inputX) > Mathf.Epsilon || Mathf.Abs(inputY) > Mathf.Epsilon) && !isDead)
    {
      // Reset timer
      delayToIdle = 0.01f;
      animator.SetInteger("AnimState", 1);
    }
    //Idle
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
    currentAttack++;

    // Loop back to one after third attack
    if (currentAttack > 3)
      currentAttack = 1;

    // Reset Attack combo if time since last attack is too large
    if (timeSinceAttack > 1.0f)
      currentAttack = 1;

    // Call one of three attack animations "Attack1", "Attack2", "Attack3"
    animator.SetTrigger("Attack" + currentAttack);

    // Reset timer
    timeSinceAttack = 0.0f;

    if (facingDirection == 1)
      AttackCollision(attackPointRight);
    else
      AttackCollision(attackPointLeft);
  }

  void AttackCollision(Transform attackPoint)
  {
    // Detect collision with colliders in enemyLayers in range of attack
    Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attack_range, enemyLayers);

    // check which hitObject has the gameobject with the tag "Mob"
    foreach (Collider2D hitObject in hitObjects)
    {
      if (hitObject.gameObject.tag == enemyHurtboxTag)
      {
        // Damage enemies
        hitObject.gameObject.transform.parent.gameObject.GetComponent<Mob>().TakeDamage(attack_damage);
      }
    }
  }

  public void TakeDamage(float damage)
  {
    // take damage
    health -= damage;
    healthBar.SetHealth(health);

    animator.SetTrigger("Hurt");

    // return if player is still alive
    if (health > 0) return;

    animator.SetBool("isDead", true);
    isDead = true;

    animator.SetTrigger("Death");

    // reset velocity
    body2d.velocity = new Vector2(0, 0);

    // hide healthbar
    healthBar.gameObject.SetActive(false);

    // respawn player after x seconds
    Invoke("Respawn", respawn_delay);
  }

  void Respawn()
  {
    // reset health
    health = health_max;
    healthBar.SetHealth(health);

    // trigger respawn
    animator.SetTrigger("Respawn");

    // show healthbar
    healthBar.gameObject.SetActive(true);

    // reset death state
    animator.SetBool("isDead", false);
    isDead = false;

    // remove all mobs
    GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
    foreach (GameObject mob in mobs)
    {
      Destroy(mob);
    }

    // move to spawn point
    GameObject game_handler = GameObject.Find("Main Camera");
    game_handler.GetComponent<GameHandler>().LoadRoom(0);
  }

  public bool IsAlive()
  {
    return !isDead;
  }

  void OnDrawGizmosSelected()
  {
    if (attackPointRight != null && facingDirection == 1)
      Gizmos.DrawWireSphere(attackPointRight.position, attack_range);
    if (attackPointLeft != null && facingDirection == -1)
      Gizmos.DrawWireSphere(attackPointLeft.position, attack_range);
  }
}