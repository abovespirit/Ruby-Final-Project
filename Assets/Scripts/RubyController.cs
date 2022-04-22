using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;
    public GameObject projectilePrefab;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    public GameObject jambiTextObject;
    public GameObject robotTextObject;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public ParticleSystem hitParticle;
    public ParticleSystem healthParticle;
    public int health { get { return currentHealth; } }
    int currentHealth;
    public float timeInvincible = 2.0f;
    bool isInvincible;
    private bool levelTwo;
    private bool levelThree;
    float invincibleTimer;
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    AudioSource audioSource;
    private bool playerLose;
    private bool playerWin;


    public TextMeshProUGUI scoreText;
    private int scoreValue;
    public AudioClip winClip;
    public AudioClip loseClip;
    public AudioClip walkClip;
    public AudioSource backgroundMusic;
    public int cogs { get { return currentCogs; } }
    int currentCogs;
    public TextMeshProUGUI cogsText;
    public AudioClip questClip;
    public AudioSource footstepSource;



    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;


        playerWin = false;
        playerLose = false;
        scoreValue = 0;
        currentCogs = 4;
        scoreText.text = "Robots Fixed: " + scoreValue.ToString();
        cogsText.text = "x" + currentCogs.ToString();
        levelTwo = false;
        levelThree = false;
        audioSource = GetComponent<AudioSource>();
        hitParticle.Stop();
        healthParticle.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
            footstepSource.enabled = true;

        }
        else
        {
            footstepSource.enabled = false;
        }
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        // Quit game / Restart Game
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
            // Lose Restart
            if (Input.GetKey(KeyCode.R) && playerLose == true)
            {
                Application.LoadLevel(Application.loadedLevel);
            }
            //Win Restart
            if (Input.GetKey(KeyCode.R) && playerWin == true)
            {
                Application.LoadLevel(Application.loadedLevel);
            }

        }
        //Jambi 
        if (Input.GetKeyDown(KeyCode.X))
        {
            jambiTextObject.SetActive(false);
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    backgroundMusic.PlayOneShot(questClip);
                }

                if (scoreValue == 4)
                    SceneManager.LoadScene("Level 2");
                levelTwo = true;
            }
        }
        //Robot NPC
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC2"));
            if (hit.collider != null)
            {
                RobotNPC character = hit.collider.GetComponent<RobotNPC>();
                if (character != null)
                {
                    character.DisplayDialog();
                    backgroundMusic.PlayOneShot(questClip);
                }

                if (scoreValue == 4)
                    SceneManager.LoadScene("Level 3");
                levelThree = true;
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
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            animator.SetTrigger("Hit");
            PlaySound(hitSound);

            Instantiate(hitParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        if (amount > 0)
        {
            Instantiate(healthParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }


        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        // Lose 
        if (health == 0)
        {
            speed = 0;
            playerLose = true;
            loseTextObject.SetActive(true);
            backgroundMusic.Stop();
            backgroundMusic.PlayOneShot(loseClip);
        }
    }

    public void ChangeCogs(int amount)
    {
        currentCogs = (currentCogs + amount);
        cogsText.text = "x" + currentCogs.ToString();
    }

    void Launch()
    {
        if (currentCogs == 0)
        {
            animator.SetTrigger("Launch");
            return;
        }

        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
        currentCogs = (currentCogs - 1);
        cogsText.text = "x" + currentCogs.ToString();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void ChangeScore()
    {
        scoreValue += 1;
        scoreText.text = "Robots Fixed:" + scoreValue.ToString();

        //Win

        if (scoreValue == 4 && playerWin != true)
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main"))
            {
                jambiTextObject.SetActive(true);
            }

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level 2"))
            {
                robotTextObject.SetActive(true);
            }



            if (scoreValue == 8)
            {
                playerWin = true;
            }


        }
    }
}
//    public void robotDead()
//    {
//            winTextObject.SetActive(true);
//            backgroundMusic.Stop();
//            backgroundMusic.PlayOneShot(winClip);
//            playerWin = true;
        
//    }
//}
