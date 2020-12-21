//using System;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
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
	[SerializeField] private Text _scoreText;
	[SerializeField] private Image _progressBar;
	[SerializeField] private GameObject _gameFinishedPanel;
	[SerializeField] private GameObject _loadingIndicator;
	[SerializeField] private GameObject _quitButton;

	public static GameController instance;

	private List<float> _catSpawnIntervals = new List<float>();
	private List<float> _catSpawnXPositions = new List<float>();

	private bool _gameFinished = false;

	private float _score = 0.0f;
	private float _timeElapsed = 0.0f;

	private void Awake()
	{
		GameValues.Load();

		for (int i = 0; i < 100; i++)
			_catSpawnIntervals.Add(Util.RandomRangeWithStaticSeed(GameValues.minSpawnInterval, GameValues.maxSpawnInterval));

		for (int i = 0; i < 100; i++)
		{
			float boundsDistance = GameValues.boundsDistance + 0.5f;
			_catSpawnXPositions.Add(Util.RandomRangeWithStaticSeed(-boundsDistance, boundsDistance));
		}
	}

	private void Start()
	{
		instance = this;

		if (GameValues.includeMap)
			Instantiate(_mapPrefab);

		Invoke("SpawnCat", _catSpawnIntervals[0] - 1.0f);
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
		print(_catSpawnIntervals[0]);

		if ((int)GameValues.gameVersion == 2)
			GetComponent<AudioSource>().Play();

		await Task.Delay((int)(GameValues.timeBetweenSoundCueAndSpawn * 1000.0f));

		if (_gameFinished)
			return;

		if ((int)GameValues.gameVersion == 1)
			GetComponent<AudioSource>().Play();

		Instantiate(_fallingCatPrefab, new Vector2(_catSpawnXPositions[0], 0.0f), Quaternion.identity);

		_catSpawnIntervals.RemoveAt(0);
		_catSpawnXPositions.RemoveAt(0);

		if (_catSpawnIntervals.Count > 0 && _catSpawnXPositions.Count > 0)
			Invoke("SpawnCat", _catSpawnIntervals[0] - 1.0f);
	}

	public void AddScore(float scoreGain)
	{
		_score += scoreGain;
		_scoreText.text = GameValues.lerpScoreGain ? _score.ToString("F", CultureInfo.CreateSpecificCulture("en-CA")) : ((int)_score).ToString();
	}

	private void OnGameFinished()
	{
		_gameFinished = true;
		Time.timeScale = 0.0f;
		CancelInvoke();
		_gameFinishedPanel.SetActive(true);

		if (GameValues.trackData && PlayerPrefs.GetInt("AlreadyPlayed") == 0)
			PushDataToDatabase(true); //TODO: Pass in actual validity
		else
			OnDataPushed();
	}

	private async void PushDataToDatabase(bool valid)
	{
		string gameVersion = ((int)GameValues.gameVersion).ToString();
		string score = _score.ToString();
		string averageReactionTime = "2.5"; //TODO: Add actual reaction time
		string distanceTraveled = "12.5"; //TODO: Add actual distance traveled
		string validity = valid ? "Yes" : "No";
		DateTime dateTime = DateTime.Now;

		/*string connectionString = "Server=_;Database=_;User Id=_;Password=_;";

		using (SqlConnection conn = new SqlConnection(connectionString))
		{
			conn.Open();

			string text = $"INSERT INTO _ VALUES ('{dateTime}', '{score}', '{averageReactionTime}');";

			using (SqlCommand cmd = new SqlCommand(text, conn))
			{
				//Execute the command and log the # rows affected.
				var rows = await cmd.ExecuteNonQueryAsync();
				print($"{rows} rows were updated");
			}

			conn.Close();
		}*/

		//Send discord message
		DiscordWebhook.Webhook webhook = new DiscordWebhook.Webhook("https://discord.com/api/webhooks/790416883413286963/M_DjeIv912oVMcKEeCTzujWXbFpV0gR4qWjd9OQoueAxelcbKzB6PPhco6toiG1Xyfxx");

		string webhookString = $"A participant just finished playing\n```Game version: {gameVersion}   |   Score: {score}   |   Average reaction time: {averageReactionTime}   |   Distance traveled: {distanceTraveled}   |   Valid: {validity}```";
		await webhook.Send(webhookString);

		PlayerPrefs.SetInt("AlreadyPlayed", 1);

		//TODO: Do this after data has been pushed (await database web request)
		OnDataPushed();
	}

	private void OnDataPushed()
	{
		_loadingIndicator.SetActive(false);
		_quitButton.SetActive(true);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
