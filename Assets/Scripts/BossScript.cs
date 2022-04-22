using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class BossScript : MonoBehaviour
{
    public int attackDamage = 1;
    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;
    public Slider HealthBar;
    private float Health = 10;
    public Animator BossAnimator;

    public AudioClip winClip;
    public GameObject winTextObject;
    public AudioSource backgroundMusic;
   

    public Transform player;

    public bool isFlipped = false;

    private bool playerWin;

    void Start()
    {
        playerWin = false;
    }

    void Update()
    {
        //Win Restart
        if (Input.GetKey(KeyCode.R) && playerWin == true)
        {
            SceneManager.LoadScene("Main");
        }
    }

    public void Attack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            colInfo.GetComponent<RubyController>().ChangeHealth(-1);


        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (!BossAnimator.GetBool("Dead"))
        {



            RubyController controller = other.GetComponent<RubyController>();


            if (controller != null)
            {
                controller.ChangeHealth(-1);
            }

        }
    }
    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Gizmos.DrawWireSphere(pos, attackRange);
    }

    public void LookAtPlayer()


    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }


    }

    public void takeDamage()
    {
        Health -= (1);
        HealthBar.value = Health;
        

        if (Health <= 0)
        {

            BossAnimator.SetBool("Dead", true);
            HealthBar.gameObject.SetActive(false);
            winTextObject.SetActive(true);
            backgroundMusic.Stop();
            backgroundMusic.PlayOneShot(winClip);
            playerWin = true;

        }

    }
}