using System;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
	[Header("References")] 
	public GameObject GameObjectToCreateOnDestroy;
	[SerializeField] private AudioEvent killedAudioEvent;
	
	[Header("Parameters")] 
	[SerializeField] private float speed;
	[SerializeField] private LayerMask ground;
    
	private Rigidbody2D rigidbody2D;

	private bool isRight => rigidbody2D.velocity.x > 0;

	public bool Alive { get; private set; }
	
	private void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
		Alive = true;
	}

	private void Start()
	{
		rigidbody2D.velocity = Vector2.right * speed;
	}

	public void KillEnemy()
	{
		Alive = false;
		killedAudioEvent.Play();
		if (GameObjectToCreateOnDestroy)
		{
			Instantiate(GameObjectToCreateOnDestroy, transform.position, Quaternion.identity);
		}
		Destroy(gameObject, 0.5f);
	}

	private void FixedUpdate()
	{
		if (Alive)
		{
			MoveHorizontalUpdate();
		}
	}

	private void MoveHorizontalUpdate()
	{
		bool toChangeDirection;
		if (isRight)
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.6f, ground);
			toChangeDirection = hit.collider;
		}
		else
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 0.6f, ground);
			toChangeDirection = hit.collider;
		}

		if (toChangeDirection)
		{
			rigidbody2D.velocity = -rigidbody2D.velocity;
		}
	}
	
}