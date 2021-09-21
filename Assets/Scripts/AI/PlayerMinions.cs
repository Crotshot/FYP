using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMinions : MonoBehaviour
{
    //A class controlled by the player for commanding minions
    [SerializeField] int maxFollowers, startingFollowers;
    int currentMaxFollowers;

    //Following minions are minions that are exculsively following the conqueror, attacking and defending are moved to the recallable list
    List<GameObject> followingMinions = new List<GameObject>(), recallableMinions = new List<GameObject>();

    float minionSpacing = -0.5f;
    //TEST
    [SerializeField] int minionsTest, minionsPerRow;
    [SerializeField] GameObject instObj;
    //
    private void Start()
    {
        currentMaxFollowers = startingFollowers;
        SetMinionPositions();

        if(minionsPerRow % 2 == 1)
        {
            minionsPerRow++;
        }
    }

    private void Update()
    {

    }

    private void SetMinionPositions()
    {
        Vector3 setPos = Vector3.zero;
        int row = 0, column = 0;
        for(int i = 0; i < minionsTest; i++)
        {
            if(i % minionsPerRow == 0)
            {
                row++;
                column = 0;
            }
            if (i % 2 == 0)
            {
                setPos = new Vector3(column * minionSpacing + 0.5f * minionSpacing, 0, row * minionSpacing + minionSpacing);
                column++;
            }
            else
                setPos = new Vector3(setPos.x * -1, 0, setPos.z);
            
            GameObject banana = Instantiate(instObj, transform);
            banana.transform.position += setPos;
        }
    }

    public void OrderAttack()
    {   //Send a minion forward to attack
        //Set the minions nav target to a position along a ray ,
        //if the  minion goes and does not fight or enter a structure it will remain for a few seconds and then return

    }

    public void OrderDefend()
    {   //Tell a minion to stay and defend this spot, ordering defend to minions near this point will add that minion to that defence group
        //Ordering minions to attack while looking at the defence group will cause them to join it

    }

    public void Recall(bool all)
    {   //Call a minion back from an attack or defence point, bool all to call back all follower minions on the map

    }
}
