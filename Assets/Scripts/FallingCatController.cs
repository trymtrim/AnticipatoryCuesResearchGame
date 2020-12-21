using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class FallingCatController : MonoBehaviour
{
	private float _spawnHeight = 5.25f;

	private bool _isGrounded = false;

	private void Start()
	{
		transform.position = new Vector2(transform.position.x, _spawnHeight);
	}

	private void Update()
	{
		if (!_isGrounded)
			transform.position -= (Vector3)new Vector2(0.0f, GameValues.fallingSpeed * Time.deltaTime);
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		GameObject collisionObject = collider.gameObject;

		if (collisionObject.tag == "Player")
		{
			float score = Util.Map(transform.position.y, -1.535f, _spawnHeight - 1.6f, GameValues.minScoreGain, GameValues.maxScoreGain);
			score = Mathf.Clamp(score, GameValues.minScoreGain, GameValues.maxScoreGain);

			if (!GameValues.lerpScoreGain)
				score = Mathf.RoundToInt(score);

			GameController.instance.AddScore(score);

			collisionObject.GetComponent<AudioSource>().Play();
			Disappear();
		}
		else if (collisionObject.tag == "Floor")
		{
			_isGrounded = true;
			Blink();
			Invoke("Disappear", 2.0f);
		}
	}

	private void Blink()
	{

	}

	private void Disappear()
	{
		Destroy(gameObject);
	}
}
