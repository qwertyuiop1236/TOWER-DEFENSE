using UnityEngine;

public class PathManager : MonoBehaviour
{
    // Статическая ссылка для доступа из других скриптов
    public static PathManager Instance;
    
    // Массив точек пути (заполняем в инспекторе)
    public Transform[] waypoints;
    
    void Awake()
    {
        // Сохраняем ссылку на себя
        Instance = this;
    }
    
    // Рисуем линию между точками в редакторе
    void OnDrawGizmos()
    {
        // Если точек нет или меньше 2 - не рисуем
        if (waypoints == null || waypoints.Length < 2) return;
        
        Gizmos.color = Color.yellow;
        
        // Рисуем линии между всеми точками
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            // Проверяем что точки не пустые
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}