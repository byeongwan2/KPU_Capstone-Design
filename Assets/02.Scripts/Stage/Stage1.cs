using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private GameObject alien_gun2;
    void Start()
    {
        create_start_monster = GameObject.Find("Alien").GetComponent<Alien>();
        robot = GameObject.Find("Robot");
        robot.SetActive(false);
        alien_gun2 = GameObject.Find("Alien_Gun2");
        alien_gun2.SetActive(false);
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
    void Create_Monster()
    {
            
    }
    public void CreateRobotFirst(eCHAPTER _chapter)
    {
        if (_chapter == eCHAPTER.ONE)
            StartCoroutine(CreateRobot());
        else if (_chapter == eCHAPTER.TWO)
        {
            StartCoroutine(CreateSecondRobot());
            alien_gun2.SetActive(true);
        }
    }

    IEnumerator CreateRobot()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(2.0f);

            robotarray[i].SetActive(true);

            robot2array[i].SetActive(true);
        }
        
    }
    IEnumerator CreateSecondRobot()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(2.0f);

            robot3array[i].SetActive(true);

            robot4array[i].SetActive(true);
        }

    }
}
