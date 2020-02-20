using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper
{
    private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
    private string lastElapsedTime;

    public bool Start()
    {
        stopWatch.Start();
        return true;
    }

    public string Stop()
    {
        stopWatch.Stop();
        lastElapsedTime = stopWatch.Elapsed.TotalMilliseconds.ToString() + " ms";
        stopWatch.Reset();
        return lastElapsedTime;
    }

    public string GetLastElapsedTime()
    {
        return lastElapsedTime;
    }

}
