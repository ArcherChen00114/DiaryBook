using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameTime
{
    public int Day;
    public int Hour;
    public int Minute;
    public float Second;

    public GameTime(int day, int hour,int minute, int second)
    {
        Day = day;
        Hour = hour;
        Minute = minute;
        Second = second;
    }
    public static GameTime operator +(GameTime a, GameTime b)
    {
        int totalSeconds = (int)a.Second + (int)b.Second;
        int totalMinutes = a.Minute + b.Minute + (totalSeconds / 60);
        int totalHours = a.Hour + b.Hour + (totalMinutes / 60);
        int totalDays = a.Day + b.Day + (totalHours / 24);

        return new GameTime(totalDays, totalHours % 24, totalMinutes % 60, totalSeconds % 60);
    }

    // 定义一个方法来执行相同的功能
    public static GameTime Add(GameTime a, GameTime b)
    {
        return a + b; // 调用重载的 + 运算符
    }

    public void Update() {
    }

    public void UpdateTime(int deltaSeconds)
    {
        Second += deltaSeconds;

        while (Second >= 60)
        {
            Second -= 60;
            Hour += 1;
        }

        while (Hour >= 24)
        {
            Hour -= 24;
            Day += 1;
        }
    }
}
