using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1 : MonoBehaviour
{
    private Alien create_start_monster;
    public Transform create_monster_position;
    public Transform create_monster_position2;
    // Start is called before the first frame update
    private GameObject robot;
    private GameObject[] robotarray = new GameObject[20];
    private GameObject[] robot2array = new GameObject[20];
    void Start()
    {
        create_start_monster = GameObject.Find("Alien").GetComponent<Alien>();
        robot = GameObject.Find("Robot");
        for (int i = 0; i < 20; i++)
        {
            robotarray[i] = Instantiate(robot,this.transform);
            robotarray[i].transform.position = create_monster_position.position;
            robotarray[i].SetActive(false);

            robot2array[i] = Instantiate(robot, this.transform);
            robot2array[i].transform.position = create_monster_position2.position;
            robot2array[i].SetActive(false);

        }
    }
    void Create_Monster()
    {
            
    }
    public void CreateRobotFirst()
    {
        StartCoroutine(CreateRobot());
        
    }

    IEnumerator CreateRobot()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(2.0f);

            robotarray[i].SetActive(true);

            robot2array[i].SetActive(true);
        }
        
    }
}
