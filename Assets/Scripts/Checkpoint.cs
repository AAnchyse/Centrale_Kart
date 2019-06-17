using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Checkpoint : MonoBehaviour
{
    public Transform path;
    public List<Transform> nodes;
    public int currentNode = -1;
    public int targetNode = 0; 
    public bool finish = true;
    public Respawn respawn;
    public int position = 0;

    public TextMeshProUGUI roundText;
    public TextMeshProUGUI positionText;

    [HideInInspector]
    public static bool updatePosition= false;
    public static bool computeTargetDistance = false;

    public int round = 1;
    
    public int counterDelay;
    private int counter = 0;
    public float targetNodeDistance = 0;

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
        roundText.text = round.ToString()+" / "+ PositioningSystem.roundNumber.ToString();
    }
    void Update() 
    {
        if(counter >counterDelay)
        {
            computeTargetDistance = true;
            counter = 0;
        }
        if(computeTargetDistance)
        {
            Vector3 relativeVector = transform.InverseTransformPoint(nodes[targetNode].position);
            targetNodeDistance = relativeVector.sqrMagnitude;
            positionText.text = position.ToString()+" / "+ PositioningSystem.carsNumber.ToString();
        }
        counter++;
    }
    //New Waypoint
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Waypoint"))
        {
            int colliderNode = collider.GetComponent<NodeID>().nodeID;

            //we check if we haven't missed a checkpoint, otherwise we respawn at currentNode
            if(colliderNode == targetNode)
            {
                if(colliderNode ==0 && currentNode == nodes.Count - 1)
                {
                    round++;

                    if(round > PositioningSystem.roundNumber)
                        roundText.text = "Finish ! ";
                    else
                    roundText.text = round.ToString()+" / "+ PositioningSystem.roundNumber.ToString();
                }
                
                currentNode = colliderNode;
                updatePosition = true;
                
                //we create a new target
                if(currentNode == nodes.Count - 1)
                {
                    targetNode = 0;
                }
                else
                {
                    targetNode = currentNode + 1;
                }
            }
            else
            {
                respawn.respawn = true;
            }
        }
    }
}
