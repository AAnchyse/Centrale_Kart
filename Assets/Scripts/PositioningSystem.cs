using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositioningSystem : MonoBehaviour
{
    public List<Checkpoint> cars;
    public List<Checkpoint> finalRank;

    public static int roundNumber=1;
    public static int carsNumber;

    [HideInInspector]
    public List<int> carsIndexToRemove;

    void Start() 
    {
        carsNumber = cars.Count;
        carsIndexToRemove = new List<int>();
    }

    void Update()
    {
        if (Checkpoint.updatePosition || Checkpoint.computeTargetDistance)
        {
            cars = cars.OrderByDescending(car => car.round).ThenByDescending(car => car.currentNode).ThenBy(car => car.targetNodeDistance).ToList();
            
            int i = carsNumber - cars.Count +1;
            int j =0;
            foreach (Checkpoint car in cars)
            {
                car.position = i;

                //check if we have finished
                if(car.round == roundNumber+1)
                {
                    //on enlève le contrôle de la voiture, on l'enlève du classement temps réel et on le met dans le classement final
                    car.finish = true;
                    carsIndexToRemove.Add(j);
                    j++;
                    finalRank.Add(car); 
                }
                i++;
            }

            foreach (int index in carsIndexToRemove)
            {
                cars.RemoveAt(index);
            }
            carsIndexToRemove.Clear();

            Checkpoint.updatePosition = false;
            Checkpoint.computeTargetDistance = false;
            
        }
    }
}