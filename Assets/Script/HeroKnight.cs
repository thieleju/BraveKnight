using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour
{
  public float speed = 4.0f;

  public Transform attackPointRight;
  public Transform attackPointLeft;

  public float attackRange = 0.7f;
  public float attackDamage = 40f;
  public float health_max = 100.0f;
  public float health = 0.0f;
  public float damage = 10.0f;

  public LayerMask enemyLayers;
  public string enemyHurtboxTag = "Mob";

  public Animator animator;
  public Rigidbody2D body2d;

  private int facingDirection = 1;
  private int currentAttack = 0;
  private float timeSinceAttack = 0.0f;
  private float delayToIdle = 0.0f;

  private float inputX_damped = 0.0f;
  private float inputY_damped = 0.0f;

  private bool isDead = false;


  // Use this for initialization
  void Start()
  {
    health = health_max;
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
    if (Input.GetKeyDown("e"))
    {
      animator.SetTrigger("Death");
    }
    //Hurt
    else if (Input.GetKeyDown("q"))
    {
      animator.SetTrigger("Hurt");
    }
    //Attack
    else if (Input.GetMouseButtonDown(0) && timeSinceAttack > 0.25f)
    {
      Attack();
      // stop movement velocity
      body2d.velocity = new Vector2(0, 0);
      delayToIdle = 1.01f;
    }
    //Run
    else if ((Mathf.Abs(inputX) > Mathf.Epsilon || Mathf.Abs(inputY) > Mathf.Epsilon))
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
    Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

    // check which hitObject has the gameobject with the tag "Mob"
    foreach (Collider2D hitObject in hitObjects)
    {
      if (hitObject.gameObject.tag == enemyHurtboxTag)
      {
        // Damage enemies
        hitObject.gameObject.transform.parent.gameObject.GetComponent<Mob>().TakeDamage(attackDamage);
      }
    }
  }

  public void TakeDamage(float damage)
  {
    health -= damage;

    // take damage
    animator.SetTrigger("Hurt");
    Debug.Log("Player took " + damage + " damage");

    // check if mob is dead
    if (health <= 0)
    {
      animator.SetBool("isDead", true);
      isDead = true;

      animator.SetTrigger("Death");

      // reset velocity
      body2d.velocity = new Vector2(0, 0);

      Debug.Log("Player died");
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
// groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
// wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
// wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
// wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
// wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
// Increase timer that checks roll duration
// if (rolling)
//     rollCurrentTime += Time.deltaTime;

// Disable rolling if timer extends duration
// if (rollCurrentTime > rollDuration)
//     rolling = false;

//Check if character just landed on the ground
// if (!grounded && groundSensor.State())
// {
// grounded = true;
// animator.SetBool("Grounded", grounded);
// }

//Check if character just started falling
// if (grounded && !groundSensor.State())
// {
//     grounded = false;
//     animator.SetBool("Grounded", grounded);
// }

//Set AirSpeed in animator
// animator.SetFloat("AirSpeedY", body2d.velocity.y);

// -- Handle Animations --
//Wall Slide
// isWallSliding =
//     (wallSensorR1.State() && wallSensorR2.State())
//     || (wallSensorL1.State() && wallSensorL2.State());
// animator.SetBool("WallSlide", isWallSliding);
// // Block
// else if (Input.GetMouseButtonDown(1) && !rolling)
// {
//     animator.SetTrigger("Block");
//     animator.SetBool("IdleBlock", true);
// }
// else if (Input.GetMouseButtonUp(1))
//     animator.SetBool("IdleBlock", false);
// // Roll
// else if (Input.GetKeyDown("left shift") && !rolling && !isWallSliding)
// {
//     rolling = true;
//     animator.SetTrigger("Roll");
//     body2d.velocity = new Vector2(facingDirection * rollForce, body2d.velocity.y);
// }
//Jump
// else if (Input.GetKeyDown("space") && grounded && !rolling)
// {
//     animator.SetTrigger("Jump");
//     grounded = false;
//     animator.SetBool("Grounded", grounded);
//     body2d.velocity = new Vector2(body2d.velocity.x, jumpForce);
//     groundSensor.Disable(0.2f);
// }
// Animation Events
// Called in slide animation.
// void AE_SlideDust()
// {
//     Vector3 spawnPosition;

//     if (facingDirection == 1)
//         spawnPosition = wallSensorR2.transform.position;
//     else
//         spawnPosition = wallSensorL2.transform.position;

//     if (slideDust != null)
//     {
//         // Set correct arrow spawn position
//         GameObject dust =
//             Instantiate(slideDust, spawnPosition, gameObject.transform.localRotation)
//             as GameObject;
//         // Turn arrow in correct direction
//         dust.transform.localScale = new Vector3(facingDirection, 1, 1);
//     }
// }