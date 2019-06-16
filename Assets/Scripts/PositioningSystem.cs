using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositioningSystem : MonoBehaviour
{
    public List<Checkpoint> cars;

    void Update()
    {
        if (Checkpoint.updatePosition)
        {
            cars = cars.OrderByDescending(car => car.round).ThenByDescending(car => car.currentNode).ThenBy(car => car.targetNodeDistance).ToList();
            
            int i=1;
            foreach (Checkpoint car in cars)
            {
                car.position = i;
                i++;
            }
            //si round =4, alors on sort la voiture du tri et on la met dans un tableau qui est le tableau final
            Checkpoint.updatePosition = false;
        }
    }
}