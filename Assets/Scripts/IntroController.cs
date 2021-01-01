using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
	[SerializeField] private Image _progressBar;
	[SerializeField] private Button _readyButton;
	[SerializeField] private InputField _participantIDInputField;

	private const float _introDuration = 20.0f;

	private float _timeElapsed = 0.0f;

	private void Awake()
	{
		GameValues.Load();
	}

	private void Start()
    {
		Time.timeScale = 0.0f;
	}

	private void Update()
	{
		_timeElapsed += Time.deltaTime;
		_progressBar.fillAmount = Util.Map(_timeElapsed, 0.0f, _introDuration, 0.0f, 1.0f);
	}

	public void OnParticipantIDEditied()
	{
		_readyButton.interactable =_participantIDInputField.text != string.Empty;
	}

	public void EnterParticipantID()
	{
		PlayerPrefs.SetString("ParticipantID", _participantIDInputField.text);
	}

	public void StartPlaying()
	{
		Time.timeScale = 1.0f;
		Invoke("OpenGameScene", _introDuration);
	}

	private void OpenGameScene()
	{
		SceneManager.LoadScene("GameScene");
	}
}
