using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TermsHandler : MonoBehaviour
{
	[SerializeField] private Toggle _termsAgreedToggle;
	[SerializeField] private Slider _audioSlider;
	[SerializeField] private GameObject _volumePanel;

	private AudioSource _audio;

	private void Awake()
	{
		GameValues.Load();
	}

	private void Start()
	{
		_audio = GetComponent<AudioSource>();
	}

	public void UpdateTermsAgreed()
	{
		bool agreed = _termsAgreedToggle.isOn;
		PlayerPrefs.SetString("TermsAgreed", agreed ? "Yes" : "No");

		if (GameValues.gameVersion != 0)
			_volumePanel.SetActive(true);
		else
			OpenIntroScene();
	}

	public void ChangeVolume()
	{
		if (_audioSlider.value <= 1.0f)
			_audioSlider.value = 1.0f;

		float volume = _audioSlider.value / 10.0f;
		_audio.volume = volume;

		_audio.Play();
	}

	public void OpenIntroScene()
	{
		float volume = _audioSlider.value / 10.0f;
		PlayerPrefs.SetFloat("Volume", volume);

		SceneManager.LoadScene("IntroScene");
	}
}
