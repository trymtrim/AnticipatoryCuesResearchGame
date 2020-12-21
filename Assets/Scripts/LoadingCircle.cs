using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LoadingCircle : MonoBehaviour
{
	private RectTransform rectComponent;
	private float _rotationSpeed = 300.0f;

	private void Start()
	{
		rectComponent = GetComponent<RectTransform>();
	}

	private void Update()
	{
		rectComponent.Rotate(0.0f, 0.0f, -_rotationSpeed * Time.unscaledDeltaTime);
	}
}
