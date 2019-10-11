using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{

    PlayableDirector playableDirector;
    public bool isSkip = false;

    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }
    
    void Update()
    {
        RepeatTimeline();
    }

    public void SkipTimeline()
    {        
        playableDirector.time = 20f;
        isSkip = true;    
    }

    void RepeatTimeline()
    {
        // 120초 기준으로 반복
        if (playableDirector.time >= 120f)
        {
            playableDirector.time = 20f;
        }
    }
}
