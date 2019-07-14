using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EVENT_TYPE { NPC_CHAT_START }           //이벤트 종류
//현재는 안쓰고 있는 스크립트
public class EventManager : MonoBehaviour {

	public static EventManager Instance{ get { return instance; }  }
    private static EventManager instance = null;
    public delegate void OnEvent(EVENT_TYPE _eventType, Component _sender, object _param = null);

    private Dictionary<EVENT_TYPE, List<OnEvent>> m_Listeners = new Dictionary<EVENT_TYPE, List<OnEvent>>();//보관소 //여러이벤트보관

    void Awake()            
    {
        if (instance == null)               //매니저가 없다면 생성
        {
            instance = this;                //다른스크립트에서 매니저로접근한다는뜻은 = 인스턴스변수로 접근한다는뜻
        } 
        else DestroyImmediate(this);        //매니저가이미있다면 파괴
    }

    public void AddListener(EVENT_TYPE _eventType, OnEvent _listener)
    {
        List<OnEvent> listenList = null;

        if (m_Listeners.TryGetValue(_eventType, out listenList))
        {
            listenList.Add(_listener);//이벤트가 생겼을때 들을새기들 등록
            return;
        }

        listenList = new List<OnEvent>();       //처음 만드는거라면 리스트먼저생성하고 보관소에 이런 이벤트리스트도 등록
        m_Listeners.Add(_eventType, listenList);
        listenList.Add(_listener);
    }

    public void PostNotification(EVENT_TYPE _eventType, Component _sender,object _param = null)//들을새기들한테 이벤트보냄
    {
        List<OnEvent> listenList = null;

        if (!m_Listeners.TryGetValue(_eventType, out listenList)) return;// 보관에서 들어온 이벤트타입을 들을새기가 없다면리턴

        for(int i = 0; i < listenList.Count; i++)       //있다면 들을새기들한테 들려주고싶은내용전달
        {
            if (!listenList[i].Equals(null))
                listenList[i](_eventType, _sender, _param);
        }
    }

    public void RemoveEvent(EVENT_TYPE _eventType)          //동적으로 보관소에서 이벤트를제거
    {
        m_Listeners.Remove(_eventType);
    }

    public void RemoveRedundancies()        //씬이교체할때만 일어난다 //당장쓸모없음  //쓰잘데기없는것들 제거하고 새로운보관소생성기능
    {
        Dictionary<EVENT_TYPE, List<OnEvent>> TmpListeners = new Dictionary<EVENT_TYPE, List<OnEvent>>();

        foreach(KeyValuePair<EVENT_TYPE,List<OnEvent>> item in m_Listeners)
        {
            for(int i = item.Value.Count-1; i >= 0; i--)
            {
                if (item.Value[i].Equals(null))
                    item.Value.RemoveAt(i);
            }

            if (item.Value.Count > 0)
                TmpListeners.Add(item.Key, item.Value);
            m_Listeners = TmpListeners;
        }
    }

    void OnLevelWasLoaded()     //씬이 바뀔때 호출되는 함수 //유니티기능
    {
        RemoveRedundancies();
    }
}
