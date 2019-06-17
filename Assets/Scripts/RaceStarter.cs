using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceStarter : MonoBehaviour
{
    public List<Checkpoint> cars;
    public float decounter = 3;
    public TextMeshProUGUI decounterText;

    private float counter;
    private bool start = false;

    void Start()
    {
        foreach (Checkpoint car in cars)
        {
            car.finish = true;
            counter = 0;
        }
    }

    void Update()
    {
        if(!start)
        {
            if(counter >100)
            {
                if(decounter>=0)
                {
                    decounter-= Time.deltaTime;
                    decounterText.text = Mathf.Ceil(decounter).ToString();
                }
                else
                {
                    start = true;
                    decounterText.text = "";
                    foreach (Checkpoint car in cars)
                        car.finish = false;
                }
            }
            else
            {
                counter++;
            }
        }
    }
}
