using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

    public int health {get { return currentHealth; }}
    public int currentHealth;

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    public GameObject projectilePrefab;

    public ParticleSystem hurtEffect;
    public ParticleSystem healEffect;
    public ParticleSystem reloadEffect;
    public ParticleSystem speedEffect;
    

    private AudioSource audioSource;
    public AudioClip throwClip;
    public AudioClip hurtClip;
    public AudioClip reloadClip;
    public AudioClip powerupClip;
    public AudioClip talkClip;

    public Text scoreText;
    public int score = 0;

    public Text resultText;

    private bool gameOver = false;

    public static int level;

    public int cogAmmo = 4;
    public Text cogText;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        scoreText.text = "Fixed Robots: " + score.ToString();
        resultText.text = " ";
        cogText.text = "Cogs: " + cogAmmo.ToString();

        timeInvincible = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
        
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical"); 

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }

        if (cogAmmo > 0)
        {
            if(Input.GetKeyDown(KeyCode.C))
            {
            Launch();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    if(score == 4)
                    {
                        SceneManager.LoadScene("Level-2");
                        level = 2;
                    }
                    
                    audioSource.PlayOneShot(talkClip);
                    character.DisplayDialog();
                }
            }
        }

        if (score == 4)
        {
            resultText.text = "Talk to Jambi, your frog friend, to visit Stage 2!";
        }

        if (score == 4 & level == 2)
        {
            resultText.text = "Good fixing! You Win! Game by David :) Press R to restart";
            speed = 0;
            gameOver = true;
        }

        if (currentHealth == 0)
        {
            resultText.text = "Too bad! You Lose! Press R to restart";
            speed = 0;
            timeInvincible = 500.0f;
            gameOver = true;
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            
            if (isInvincible)
                return;
            isInvincible = true;
            invincibleTimer = timeInvincible;
            hurtEffect.Play();
            audioSource.PlayOneShot(hurtClip);
        }
        if (amount > 0)
        {
            healEffect.Play();
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void ChangeScore (int amount)
    {
        score += 1;
        scoreText.text = "Fixed Robots: " + score.ToString();
    }


    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        audioSource.PlayOneShot(throwClip);
        cogAmmo -= 1;
        cogText.text = "Cogs: " + cogAmmo.ToString();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ammo")
        {
            cogAmmo = cogAmmo + 4;
            cogText.text = "Cogs: " + cogAmmo.ToString();
            audioSource.PlayOneShot(reloadClip);
            Destroy(collision.collider.gameObject);
            reloadEffect.Play();
        }

        if(collision.collider.tag == "Powerup")
        {
            speed = speed + 2;
            Destroy(collision.collider.gameObject);
            audioSource.PlayOneShot(powerupClip);
            speedEffect.Play();
            
        }
    }
}
