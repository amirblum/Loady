using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[Header("References")] 
	[SerializeField] private Transform legs;

	[SerializeField] private AudioEvent jumpAudioEvent;
	
	[Header("Parameters")] 
	[SerializeField] private float speed;
	[SerializeField] private float jumpForce;
	[SerializeField] private float miniJumpForce;
	[SerializeField] private LayerMask ground;
	[SerializeField] private float jumpPressGravity;

	private float moveHorizontalInput;
	private bool isJumpInput;

	private Rigidbody2D rigidbody2D;

	public Vector2 velocity => rigidbody2D.velocity;

	public bool IsGrounded { get; set; }

	private bool isFirstJumpPart;

	private void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		GroundCheck();
		MoveHorizontalUpdate();
		JumpUpdate();
	}

	public RaycastHit2D HitBelow(LayerMask layerMask)
	{
		return Physics2D.BoxCast(legs.position + Vector3.down * 0.01f/2f, new Vector2(transform.lossyScale.x * 0.9f, 0.01f), 0, Vector2.down, 0.015f, layerMask);
		// return Physics2D.Raycast(legs.position, Vector2.down, 0.09f, layerMask);
	}

	public void MiniJump()
	{
		var tempVelocity = rigidbody2D.velocity;
		tempVelocity.y = 0;
		rigidbody2D.velocity = tempVelocity;
		Jump(miniJumpForce);
	}

	private void MoveHorizontalUpdate()
	{
		float targetSpeed = moveHorizontalInput * speed;
		var currentSpeed = rigidbody2D.velocity.x;
		var difference = targetSpeed - currentSpeed;
		rigidbody2D.AddForce(Vector2.right * difference);
	}

	private void JumpUpdate()
	{
		if (IsGrounded)
		{
			if (isJumpInput)
			{
				// Debug.Log("Jumped");
				Jump(jumpForce);
				IsGrounded = false;
				isFirstJumpPart = true;
			}
		}
		else
		{
			if (isFirstJumpPart)
			{
				if (isJumpInput && velocity.y > 0)
				{
					rigidbody2D.gravityScale = jumpPressGravity;
				}
				else
				{
					isFirstJumpPart = false;
					rigidbody2D.gravityScale = 1;
				}
			}
		}
	}

	private void Jump(float force)
	{
		rigidbody2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		jumpAudioEvent.Play();
	}

	private void GroundCheck()
	{
		RaycastHit2D hit = HitBelow(ground);
		IsGrounded = hit.collider;
	}

	#region input methods

	public void OnMoveHorizontal(InputAction.CallbackContext context)
	{
		moveHorizontalInput = context.ReadValue<float>();
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		isJumpInput = context.started || context.performed;
	}	

	#endregion
}