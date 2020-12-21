using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
	public float distanceTraveled { get; private set; } = 0.0f;

	private Rigidbody2D _rigidbody;

	private bool _isGrounded = false;
	private Vector2 _lastFramePosition;

	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
		_lastFramePosition = transform.position;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			Jump();
	}

	private void FixedUpdate()
	{
		UpdateGroundedState();
		Move(Input.GetAxis("Horizontal"));

		float distanceTraveledSinceLastFrame = Vector2.Distance(_lastFramePosition, transform.position);
		distanceTraveled += distanceTraveledSinceLastFrame;
		_lastFramePosition = transform.position;
	}

	private void UpdateGroundedState()
	{
		_isGrounded = false;

		Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position - new Vector2(0.0f, 0.15f), 0.2f);

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject.tag == "Floor")
				_isGrounded = true;
		}
	}

	private void Move(float inputValue)
	{
		float y = _rigidbody.velocity.y;
		_rigidbody.velocity = new Vector2(inputValue * GameValues.moveSpeed * Time.deltaTime, y);

		if (_rigidbody.velocity.x > 0.0f)
		{
			if (transform.position.x >= GameValues.boundsDistance)
				_rigidbody.velocity = new Vector2(0.0f, y);
		}
		else if (_rigidbody.velocity.x < 0.0f)
		{
			if (transform.position.x <= -GameValues.boundsDistance)
				_rigidbody.velocity = new Vector2(0.0f, y);
		}
	}

	private void Jump()
	{
		if (!_isGrounded)
			return;

		_rigidbody.AddForce(new Vector2(0.0f, GameValues.jumpForce));
	}
}
