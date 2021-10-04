using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private Animator playerAnim;
    private AudioSource playerAudio;

    // Audio and Video FX components are not inherent part of the Player (unlike Rigidbody & Animator) so we need to declare them as public
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;
    public AudioSource cameraAudio;
    public AudioClip jumpSound;
    public AudioClip crashSound;

    public float jumpForce = 15;
    public float gravityModifier = 2;
    public bool isOnGround = true;
    public bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityModifier;

        // Video & Audio Initialization
        playerAnim = GetComponent<Animator>();
        cameraAudio = FindObjectOfType<Camera>().GetComponent<AudioSource>();
        playerAudio = GetComponent<AudioSource>();

        // Bug fix to allow audio
        if (playerAudio == null)
            playerAudio = gameObject.AddComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && !gameOver)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerAnim.SetTrigger("Jump_trig");
            playerAudio.PlayOneShot(jumpSound);
            isOnGround = false;
            dirtParticle.Stop();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !gameOver)
        {
            isOnGround = true;
            dirtParticle.Play();
        }
        else if (isOnGround && gameOver)
        {
            Debug.Log("Game over");
            dirtParticle.Stop();
        }
        else if (collision.gameObject.CompareTag("Obstacle") && !gameOver)
        // Transition to game over
        {
            Debug.Log("Game over");
            gameOver = true;
            playerAnim.SetBool("Death_b", true);
            playerAnim.SetInteger("DeathType_int", 1);
            playerAudio.PlayOneShot(crashSound);
            dirtParticle.Stop();
            explosionParticle.Play();
        }
        else
        // Game over!
        {
            dirtParticle.Stop();
            cameraAudio.Stop();
            explosionParticle.Stop();

            // Bug Fix to make sure body lies on the floor
            transform.position = new Vector3(transform.position.x - 1, 0, 0);
        }
    }
}
