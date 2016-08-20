﻿using UnityEngine;
using System.Collections;

public class Player {
	public int maxStamina = 100;
	public int currentStamina = 100;
	public int staminaRecoveryRate = 1;
}

public class PombaController : MonoBehaviour
{
    //public enum State
    //{
    //    Idle,
    //    Walking,
    //    Jumping,
    //    Falling,
    //    Death,
    //    Dying
    //}

    public int hp = 3;
    public int estamina = 100;

    //public State state;

    public float gravity = 0.3f;
    public float jumpForce = 1;
    public float walkSpeed = 1;

    public bool isGround = false;
    public bool isDamaged = false;
    public float invulnerabilityTime = 1;

    private float damagedTime = 0;
	private float blinkTime = 0;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private InputConfig _inputConfig;

    private SpriteRenderer _spriteRenderer;

    private float horizontalForce = 0;
    private float verticalForce = 0;

    public AudioClip _jumpClip;

	private Player player;

    void Start()
    {
         //state = State.Idle;
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();

        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        _inputConfig = new InputConfig();
		player = new Player();
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag != "Enemy")
        {
            isGround = true;
        }

        if (isDamaged == false && other.gameObject.tag == "Enemy")
        {
            _animator.SetTrigger("IsDamaged"); 
            
            hp--;
            damagedTime = invulnerabilityTime;
            isDamaged = true;
            
            _rigidbody.AddForce(new Vector2(-(gameObject.transform.localScale.x * jumpForce/2), jumpForce/4));
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag != "Enemy")
        {
            isGround = true;
        }
    }
    

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag != "Player")
        {
            isGround = false;
        }
    }


    void Update()
    {
         Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), isDamaged);
      
		if (_inputConfig.jump() && player.currentStamina >= 20)
        {
            _rigidbody.AddForce(new Vector2(0, jumpForce));
            //  Debug.Log("Jump");

            _animator.SetTrigger("Jump");

            GameConfig.soundManager.PlaySound(_jumpClip, gameObject.transform.position);
			player.currentStamina -= 20;
			Debug.Log("Jumped, stamina: " + player.currentStamina);
        }

        if (_inputConfig.action())
        {
            Debug.Log("B...");
        }

        if (_inputConfig.walkLeft())
        {
            horizontalForce += walkSpeed;
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (_inputConfig.walkRight())
        {
            horizontalForce += walkSpeed;
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (damagedTime > 0)
        {
            damagedTime = damagedTime - Time.deltaTime;

            if (damagedTime <= 0)
            {
                isDamaged = false;
            }

            blink();
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }

        _animator.SetBool("Walk", isGround && horizontalForce != 0);

        _animator.SetBool("IsGround", isGround);


		if (isGround && (player.currentStamina < player.maxStamina)) {
			player.currentStamina = Mathf.Clamp(player.currentStamina,
			                                    player.currentStamina + player.staminaRecoveryRate,
			                                    player.maxStamina);

			Debug.Log("Recovering stamina... " + player.currentStamina);
		}

        horizontalForce = horizontalForce * Time.deltaTime;
 
        transform.Translate(new Vector3(horizontalForce, 0, transform.position.z));
    }

    void blink()
    {
        if (blinkTime >= 0.1f)
        {
            blinkTime = 0;
            //Debug.Log("B");
            GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            //Debug.Log("A");
            GetComponent<SpriteRenderer>().enabled = false;
        }

        blinkTime = blinkTime + Time.deltaTime;
    }

}