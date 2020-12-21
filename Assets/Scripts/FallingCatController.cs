using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingCatController : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		GameObject collisionObject = collision.gameObject;

		if (collisionObject.tag == "Player")
		{
			collisionObject.GetComponent<AudioSource>().Play();
			Disappear();
		}
		else if (collisionObject.tag == "Floor")
		{
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
