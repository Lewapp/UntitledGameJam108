

public interface IDirectable 
{
    public bool difficultyApplied { get; set; }
    public void SetDifficulty(DifficultyInfo difficultyInfo);
}
