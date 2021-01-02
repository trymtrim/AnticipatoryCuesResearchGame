using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameVersion
{
	WithoutSound = 0,
	WithSoundNormal = 1,
	WithSoundEarly = 2
}

public class GameController : MonoBehaviour
{
	[Header("Prefabs")]
	[SerializeField] private GameObject _fallingCatPrefab;
	[SerializeField] private GameObject _mapPrefab;

	[Header("Scene")]
	[SerializeField] private PlayerController _player;
	[SerializeField] private Text _scoreText;
	[SerializeField] private Image _progressBar;
	[SerializeField] private GameObject _gameFinishedPanel;
	[SerializeField] private GameObject _loadingIndicator;
	[SerializeField] private GameObject _quitButton;
	[SerializeField] private GameObject _freePlayButton;
	[SerializeField] private Text _endScreenScoreText;

	public static GameController instance;

	private List<float> _catSpawnIntervals = new List<float>();
	private List<float> _catSpawnXPositions = new List<float>();

	private bool _gameFinished = false;

	private float _score = 0.0f;
	private float _timeElapsed = 0.0f;
	private List<float> _reactionTimes = new List<float>();
	int _spawnedCats = 0;

	private void Awake()
	{
		GameValues.Load();

		for (int i = 0; i < 100; i++)
			_catSpawnIntervals.Add(Util.RandomRangeWithStaticSeed(GameValues.minSpawnInterval, GameValues.maxSpawnInterval));

		for (int i = 0; i < 100; i++)
		{
			float boundsDistance = GameValues.boundsDistance - 0.5f;
			_catSpawnXPositions.Add(Util.RandomRangeWithStaticSeed(-boundsDistance, boundsDistance));
		}

		Time.timeScale = 0.0f;

		if (PlayerPrefs.GetFloat("Volume") == 0.0f)
			PlayerPrefs.SetFloat("Volume", 1.0f);
	}

	private void Start()
	{
		instance = this;

		if (GameValues.includeMap)
			Instantiate(_mapPrefab);

		GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Volume");
	}

	private void Update()
	{
		if (_gameFinished)
			return;

		_timeElapsed += Time.deltaTime;
		_progressBar.fillAmount = Util.Map(_timeElapsed, 0.0f, GameValues.gameDuration, 0.0f, 1.0f);

		if (_timeElapsed >= GameValues.gameDuration)
			OnGameFinished();
	}

	private async void SpawnCat()
	{
		if (_timeElapsed > GameValues.gameDuration - 2.0f)
			return;

		float pitch = Util.RandomRangeWithStaticSeed(0.9f, 1.15f);
		AudioSource audioSource = GetComponent<AudioSource>();
		audioSource.pitch = pitch;

		if (GameValues.gameVersion != GameVersion.WithoutSound)
			audioSource.Play();

		if (GameValues.gameVersion == GameVersion.WithSoundEarly)
			await Task.Delay((int)(GameValues.timeBetweenSoundCueAndSpawn * 1000.0f));

		if (_gameFinished)
			return;

		Instantiate(_fallingCatPrefab, new Vector2(_catSpawnXPositions[0], 0.0f), Quaternion.identity);
		print(_timeElapsed);

		_catSpawnIntervals.RemoveAt(0);
		_catSpawnXPositions.RemoveAt(0);

		if (_catSpawnIntervals.Count > 0 && _catSpawnXPositions.Count > 0)
			Invoke("SpawnCat", _catSpawnIntervals[0] - GameValues.timeBetweenSoundCueAndSpawn);

		_spawnedCats++;
	}

	public void AddScore(float scoreGain)
	{
		_score += scoreGain;
		_scoreText.text = GameValues.lerpScoreGain ? _score.ToString("F", CultureInfo.CreateSpecificCulture("en-CA")) : ((int)_score).ToString();
	}

	public void AddReactionTime(float reactionTime)
	{
		_reactionTimes.Add(reactionTime);
	}

	private void OnGameFinished()
	{
		_gameFinished = true;
		Time.timeScale = 0.0f;
		CancelInvoke();
		_gameFinishedPanel.SetActive(true);
		_endScreenScoreText.text = $"Score: {_score.ToString("F", CultureInfo.CreateSpecificCulture("en-CA"))} / {GameValues.maxScoreGain * _spawnedCats}"; 

		if (SceneManager.GetActiveScene().name == "FreePlayScene")
			return;

		bool valid = _player.distanceTraveled >= GameValues.gameDuration * ((GameValues.minSpawnInterval + GameValues.maxSpawnInterval) / 4.0f) &&
			_score >= GameValues.minScoreGain * GameValues.gameDuration / 10.0f;

		float averageReactionTime = 0.0f;
		foreach (float reactionTime in _reactionTimes)
			averageReactionTime += reactionTime;

		if (_reactionTimes.Count > 1)
			averageReactionTime /= _reactionTimes.Count;

		if (GameValues.trackData && PlayerPrefs.GetInt("AlreadyPlayed") == 0)
			PushDataToDatabase(averageReactionTime, valid);
		else
			OnDataPushed();
	}

	private async void PushDataToDatabase(float reactionTime, bool valid)
	{
		string participantID = PlayerPrefs.GetString("ParticipantID");
		string gameVersion = ((int)GameValues.gameVersion).ToString();
		string score = _score.ToString();
		string averageReactionTime = reactionTime.ToString();
		string distanceTraveled = _player.distanceTraveled.ToString();
		string termsAgreed = PlayerPrefs.GetString("TermsAgreed");

		string validity = valid ? "Yes" : "No";

		if (valid)
		{
			print (participantID);
			string arguments = "termsagreed=" + termsAgreed + "&participantid=" + participantID + "&gameversion=" + gameVersion + "&score=" + score + "&averagereactiontime=" + averageReactionTime + "&distancetraveled=" + distanceTraveled;
			string url = "https://sharpraccoon.azurewebsites.net/api/SMTProjectTemp?" + arguments;

			await SendWebRequest(url);

			PlayerPrefs.SetInt("AlreadyPlayed", 1);
		}

		//Send discord message
		DiscordWebhook.Webhook webhook = new DiscordWebhook.Webhook("https://discord.com/api/webhooks/790416883413286963/M_DjeIv912oVMcKEeCTzujWXbFpV0gR4qWjd9OQoueAxelcbKzB6PPhco6toiG1Xyfxx");

		string webhookString = $"Participant {participantID} just finished playing:\n```Game version: {gameVersion}  |  Score: {score}  |  Average reaction time: {averageReactionTime}  |  Distance traveled: {distanceTraveled}  |  Valid: {validity}  |  Terms agreed: {termsAgreed}```";
		await webhook.Send(webhookString);

		OnDataPushed();
	}

	private async Task<HttpResponseMessage> SendWebRequest(string url)
	{
		return await new HttpClient().GetAsync(url);
	}

	private void OnDataPushed()
	{
		_loadingIndicator.SetActive(false);
		_quitButton.SetActive(true);
		_freePlayButton.SetActive(true);
	}

	public void OpenQuestionnaire()
	{
		string url = GameValues.gameVersion == 0 ? "https://docs.google.com/forms/d/e/1FAIpQLSdzaAcVL-lNBsRksKBZtMTvsjbwkoTCTONG6m0nbljpPUdMyw/viewform" : "https://docs.google.com/forms/d/e/1FAIpQLSfCfPqyeNqwy7MyEJvv7Vbod1sts-sAO1QkBYfrDqrM9is3Dg/viewform";
		Application.OpenURL(url);

		_quitButton.GetComponent<Button>().interactable = true;
		_freePlayButton.GetComponent<Button>().interactable = true;
	}

	public void OpenFreePlayScene()
	{
		SceneManager.LoadScene("FreePlayScene");
	}

	public void StartGame()
	{
		Time.timeScale = 1.0f;
		Invoke("SpawnCat", _catSpawnIntervals[0] - GameValues.timeBetweenSoundCueAndSpawn);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
