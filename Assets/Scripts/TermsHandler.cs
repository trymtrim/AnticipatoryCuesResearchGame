using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TermsHandler : MonoBehaviour
{
	[SerializeField] private Toggle _termsAgreedToggle;
	[SerializeField] private Slider _audioSlider;
	[SerializeField] private GameObject _volumePanel;
	[SerializeField] private Button _termsContinueButton;
	[SerializeField] private GameObject _alreadyPlayedPanel;

	private AudioSource _audio;

	private float _timeSpentPressingDownResetKeyCombination = 0.0f;

	private void Awake()
	{
		GameValues.Load();
	}

	private void Start()
	{
		_audio = GetComponent<AudioSource>();

		if (PlayerPrefs.GetInt("AlreadyPlayed") == 1)
			_alreadyPlayedPanel.SetActive(true);
	}

	private void Update()
	{
		if (_alreadyPlayedPanel.activeInHierarchy)
		{
			if (Input.GetKey(KeyCode.R) &&
				Input.GetKey(KeyCode.Alpha1) &&
				Input.GetKey(KeyCode.Alpha9) &&
				Input.GetKey(KeyCode.Alpha0))
			{
				_timeSpentPressingDownResetKeyCombination += Time.deltaTime;

				if (_timeSpentPressingDownResetKeyCombination > 3.0f)
				{
					PlayerPrefs.DeleteAll();
					SceneManager.LoadScene("TermsScene");
				}
			}
			else
			{
				_timeSpentPressingDownResetKeyCombination = 0.0f;
			}
		}
	}

	public void UpdateTermsToggled()
	{
		_termsContinueButton.interactable = _termsAgreedToggle.isOn;
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

	public void OpenFreePlayScene()
	{
		SceneManager.LoadScene("FreePlayScene");
	}
}
