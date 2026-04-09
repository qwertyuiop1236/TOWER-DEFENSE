[System.Serializable]
public class ProgressData
{
    public int lastUnlockedLevel;      // индекс первого заблокированного уровня (например, 3 означает, что уровни 0,1,2 открыты, 3 - нет)
    public bool[] levelCompleted;      // true, если уровень пройден
}
