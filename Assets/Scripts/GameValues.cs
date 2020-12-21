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
		gameVersion = (GameVersion)1;
		trackData = true;
		includeMap = false;

		gameDuration = 10.0f;
		timeBetweenSoundCueAndSpawn = 1.0f;

		moveSpeed = 500.0f;
		jumpForce = 400.0f;

		minSpawnInterval = 1.25f;
		maxSpawnInterval = 5.0f;
		fallingSpeed = 2.0f;

		lerpScoreGain = true;
		minScoreGain = 1.0f;
		maxScoreGain = 5.0f;
	}
}
