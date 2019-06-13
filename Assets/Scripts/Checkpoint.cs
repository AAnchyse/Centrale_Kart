using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform path;
    public List<Transform> nodes;
    public int currentNode = 0;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for(int i = 0; i< pathTransforms.Length; i++)
        {
            if(pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }

    //New Waypoint
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Waypoint"))
        {
            if(currentNode == nodes.Count -1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode ++;
            }
        }
    }
}
