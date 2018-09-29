using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerSkill
{
    doubleJump
}

public class Player : MonoBehaviour
{ 
    public float speed;
    public float jumpforce;
    public int health;
    public bool isAlive = true;
    public bool invunerable;
    public float invencibleTime;
    public bool doubleJumpSkill = false;
    private bool jump = false;
    private bool doubleJump;
    private bool facingRight = true;
    private bool deathByLava = false;
    private SpriteRenderer sprite;

    public float radius;
    public LayerMask layerGround;
    [SerializeField] bool onGround = false;

    private Rigidbody2D rb2d;
    public Transform groundCheck;

    public int contador = 0;
    public bool gravidadeAtivada = false;
    public bool canPress = true;

    //Variaveis do Tiro
    public GameObject bullet;
    public Transform bulletSpawn;
    public float fireRate;
    private float nextFire;

    private Animator anim;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        JumpPlayer();
        //Fire();
        MovePlayer();
    }

    void JumpPlayer()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, radius, layerGround);
     
        if (isAlive)
        {
            if (onGround)
                doubleJump = false;

            if (Input.GetButtonDown("Jump") && (onGround || (!doubleJump && doubleJumpSkill)))
            {
                jump = true;
                if (!doubleJump && !onGround)
                    doubleJump = true;
            }

            if (jump)
            {
                rb2d.velocity = Vector2.zero;
                rb2d.AddForce(new Vector2(0f, jumpforce));
                jump = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, radius);
    }

    void Fire()
    {
        if (isAlive)
        {
            if (Input.GetButton("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                GameObject cloneBullet = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);

                if (facingRight == false)
                {
                    cloneBullet.transform.eulerAngles = new Vector3(0, 0, 180);
                }
            }
        }
    }

    void MovePlayer()
    {
        if (isAlive)
        {
            float move = Input.GetAxis("Horizontal");

            anim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));

            rb2d.velocity = new Vector2(move * speed * Time.deltaTime, rb2d.velocity.y);

            if (move > 0 && facingRight == false)
            {
                FlipPlayer();
            }
            else if (move < 0 && facingRight == true)
            {
                FlipPlayer();
            }

            if (Input.GetKeyDown(KeyCode.G) && canPress)
            {
                StartCoroutine(CanPress());
            }
            //Debug.DrawRay(transform.position, directionLight * 0.8f, Color.red);
        }
    }

    void FlipPlayer()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void DamagePlayer()
    {
        invunerable = true;
        health--;
        if (health > 0)
        {
            StartCoroutine(Damage());
        }

        else if (health <= 0)
        {
            health = 0;
            anim.SetTrigger("Death");
            isAlive = false;

            if (deathByLava)
            {
                rb2d.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            }
            else
            {
                rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            }
            Invoke("ReloadLevel", 3f);
            //AudioManager.audioManager.PlaySoundFX(0);
        }
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator Damage()
    {
        sprite.enabled = false;
        for (float i = 0; i < invencibleTime; i += 0.1f)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        invunerable = false;
        sprite.enabled = true;
    }

    IEnumerator CanPress()
    {
        rb2d.gravityScale *= -1;
        contador += 1;
        gravidadeAtivada = true;
        jumpforce *= -1;
        canPress = false;

        if (gravidadeAtivada)
        {
            if (contador == 1 && gravidadeAtivada)
            {
                groundCheck.localPosition = new Vector3(0f, 0.438f, 0f);
                sprite.flipY = true;
                contador++;
            }
            else if (contador >= 2 && gravidadeAtivada)
            {
                contador = 0;
                sprite.flipY = false;
                groundCheck.localPosition = -new Vector3(0f, 0.438f, 0f);
                gravidadeAtivada = false;
            }
        }
        yield return new WaitForSeconds(3f);
        canPress = true;
    }

    IEnumerator CanPress2()
    {
        rb2d.gravityScale *= -1;
        canPress = false;
        sprite.flipY = true;
        groundCheck.localPosition = new Vector3(0f, 0.438f, 0f);
        jumpforce *= -1;
        yield return new WaitForSeconds(3f);
        rb2d.gravityScale *= -1;
        jumpforce *= -1;
        sprite.flipY = false;
        groundCheck.localPosition = -new Vector3(0f, 0.438f, 0f);
        yield return new WaitForSeconds(1f);
        canPress = true;
    }
}