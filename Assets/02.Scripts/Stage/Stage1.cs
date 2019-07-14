using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//첫번쨰 스테이지 스크립트
public class Stage1 : MonoBehaviour
{
    private Alien create_start_monster;
    public Transform create_monster_position;
    public Transform create_monster_position2;
    public Transform create_monster_position3;
    public Transform create_monster_position4;
    // Start is called before the first frame update
    private GameObject robot;
    private GameObject[] robotarray = new GameObject[10];
    private GameObject[] robot2array = new GameObject[10];
    private GameObject[] robot3array = new GameObject[10];
    private GameObject[] robot4array = new GameObject[10];

    private Alien_Gun alien_gun2;

    //1스테이지에서 로봇은 4군데서 10마리씩 나옴 미리 전부 캐싱
    void Start()
    {
        create_start_monster = GameObject.Find("Alien").GetComponent<Alien>();
        robot = GameObject.Find("Robot");
        robot.SetActive(false);
        alien_gun2 = GameObject.Find("Alien_Gun2").GetComponent<Alien_Gun>();
        alien_gun2.gameObject.SetActive(false);
        for (int i = 0; i < 10; i++)
        {
            robotarray[i] = Instantiate(robot,this.transform);
            robotarray[i].transform.position = create_monster_position.position;
            robotarray[i].SetActive(false);

            robot2array[i] = Instantiate(robot, this.transform);
            robot2array[i].transform.position = create_monster_position2.position;
            robot2array[i].SetActive(false);

            PrefabSystem.instance.allMonster.Add(robotarray[i]);
            PrefabSystem.instance.allMonster.Add(robot2array[i]);
        }

        for(int i = 0; i < 10; i++)
        {
            robot3array[i] = Instantiate(robot, this.transform);
            robot3array[i].transform.position = create_monster_position3.position;
            robot3array[i].SetActive(false);

            robot4array[i] = Instantiate(robot, this.transform);
            robot4array[i].transform.position = create_monster_position4.position;
            robot4array[i].SetActive(false);

            PrefabSystem.instance.allMonster.Add(robot3array[i]);
            PrefabSystem.instance.allMonster.Add(robot4array[i]);
        }
    }
    
    //챕터는 플레이어가 진행하는 단계인데 해당 챕터가되면 몬스터가 나온다.
    public void CreateRobotFirst(eCHAPTER _chapter)
    {
        if (_chapter == eCHAPTER.ONE)
            StartCoroutine(CreateRobot());
        else if (_chapter == eCHAPTER.TWO)
        {
            StartCoroutine(CreateSecondRobot());
            alien_gun2.gameObject.SetActive(true);
        }
    }
    //실제 몬스터(로봇)이 나오는 함수   한 챕터당 2군데서 10마리씩나옴 총20마리
    IEnumerator CreateRobot()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(2.0f);

            robotarray[i].SetActive(true);

            robot2array[i].SetActive(true);
        }
        
    }
    //마찬가지
    IEnumerator CreateSecondRobot()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(2.0f);

            robot3array[i].SetActive(true);

            robot4array[i].SetActive(true);
        }

    }

    //총을 쏘는 두번째 에일리언이 죽었는지 판단 죽으면 다음스테이지 이동 가능
    public bool Check_MonsterLife()
    {
        if (alien_gun2.vitality <= 0)
            return true;
        else return false;
    }
}
