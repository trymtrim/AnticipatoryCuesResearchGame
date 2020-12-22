using System.Collections.Generic;
using System.Globalization;
using System.IO;

public static class GameValues
{
	//General
	public static GameVersion gameVersion;
	public static bool trackData;
	public static bool includeMap;

	//Game
	public static float gameDuration;
	public static float timeBetweenSoundCueAndSpawn;

	//Player
	public static float moveSpeed;
	public static float jumpForce;

	//Falling cat
	public static float minSpawnInterval;
	public static float maxSpawnInterval;
	public static float fallingSpeed;

	//Score
	public static bool lerpScoreGain;
	public static float minScoreGain;
	public static float maxScoreGain;

	//Values not loaded from excel sheet
	public static float boundsDistance = 8.6f;

	public static void Load()
	{
		string filePath = Path.GetFullPath("Config.csv");
		List<string> gameValues = GetValuesFromCSV("Config.csv");

		gameVersion = (GameVersion)int.Parse(gameValues[0], CultureInfo.InvariantCulture.NumberFormat);
		trackData = gameValues[1] == "TRUE" ? true : false;
		includeMap = gameValues[2] == "TRUE" ? true : false;

		gameDuration = float.Parse(gameValues[3], CultureInfo.InvariantCulture.NumberFormat);

		if (gameVersion == GameVersion.WithSoundEarly)
			timeBetweenSoundCueAndSpawn = float.Parse(gameValues[4], CultureInfo.InvariantCulture.NumberFormat);
		else
			timeBetweenSoundCueAndSpawn = 0.0f;

		moveSpeed = float.Parse(gameValues[5], CultureInfo.InvariantCulture.NumberFormat);
		jumpForce = float.Parse(gameValues[6], CultureInfo.InvariantCulture.NumberFormat);

		minSpawnInterval = float.Parse(gameValues[7], CultureInfo.InvariantCulture.NumberFormat);
		maxSpawnInterval = float.Parse(gameValues[8], CultureInfo.InvariantCulture.NumberFormat);
		fallingSpeed = float.Parse(gameValues[9], CultureInfo.InvariantCulture.NumberFormat);

		lerpScoreGain = gameValues[10] == "TRUE" ? true : false;
		minScoreGain = float.Parse(gameValues[11], CultureInfo.InvariantCulture.NumberFormat);
		maxScoreGain = float.Parse(gameValues[12], CultureInfo.InvariantCulture.NumberFormat);
	}

	private static List<string> GetValuesFromCSV(string filePath)
	{
		List<string> gameValuesList = new List<string>();

		using (var reader = new StreamReader(filePath))
		{
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();
				var values = line.Split(';');
				var value = values[3];

				if (value != string.Empty)
					gameValuesList.Add(value);
			}
		}

		return gameValuesList;
	}
}
