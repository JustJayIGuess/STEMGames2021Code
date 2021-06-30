[System.Serializable]
public class GameData
{
	public readonly float[] highscores;
	public readonly string uname;

	public GameData(float[] scores, string _uname)
	{
		highscores = scores;
		uname = _uname;
	}
}
