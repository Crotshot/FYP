using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Builder : MonoBehaviour
{
    /* Tier 1
     * Pick a random point and generate a wall end piece
     * Wall pieces position to create another random wall piece out of 3 segements (Mid-50%, End-25%, Gap-25%)
     * Create the next piece at tag "End"
     * If child with "End" tag does not exist stop maing wall
     * 
     * Tier 2
     * Check that "End" Piece is within the 20x20 area of Tile, if not rotate until it is 
     * 
     * Tier 3
     * Wall end point and start point clean up -> For every child the does not have tag "End" or "Stop" make them child to wall builder else dleete them
     * 
     * Tier 4
     * Optimise for any number of variant wall segments
     * Forking walls
     */

    //Note - would no need to shift down all wall pieces if end and start points were at y 0

    [SerializeField] bool recreateWall;
    [SerializeField] GameObject wall_End, wall_Mid, wall_Gap;
    [SerializeField] float end_Chance, mid_Chance, gap_Chance;
    [SerializeField] float offsetRange, maxAngle, minAngle;

    float totalOdds;
    bool building = false;
    GameObject starterWall;
    private void Start()
    {
        totalOdds = end_Chance + mid_Chance + gap_Chance;
        StartCoroutine(BuildRandomWall());
    }

    private void Update()
    {
        if (recreateWall && !building)
        {
            Destroy(starterWall);
            recreateWall = false;
            totalOdds = end_Chance + mid_Chance + gap_Chance;
            StartCoroutine(BuildRandomWall());
        }
    }

    IEnumerator BuildRandomWall()
    {
        building = true;
        Vector3 randomOffset = new Vector3(Random.Range(offsetRange, -offsetRange), 0, Random.Range(offsetRange, -offsetRange));
        starterWall = Instantiate(wall_End, transform);
        starterWall.transform.position += randomOffset;
        starterWall.transform.RotateAround(starterWall.transform.position, Vector3.up, Random.Range(180, -180));
        GameObject currentWall = null;
        int num = WallOdds();

        foreach (Transform child in starterWall.transform)
        {
            if (child.tag != "Stop")
                continue;
            if (num == 0)
            {
                currentWall = Instantiate(wall_End, child);
                currentWall.transform.RotateAround(child.transform.position, Vector3.up, Random.Range(315, 45));
                building = false;
                yield break; //Break out if we make another end cap
            }
            else if (num == 1)
            {
                currentWall = Instantiate(wall_Mid, child);
                currentWall.transform.RotateAround(child.transform.position, Vector3.up, Random.Range(maxAngle, minAngle));
            }
            else
            {
                currentWall = Instantiate(wall_Gap, child);
                currentWall.transform.RotateAround(child.transform.position, Vector3.up, Random.Range(maxAngle, minAngle));
            }
            break;
        }
        while (true)
        {
            foreach (Transform child in currentWall.transform)
            {
                if (child.tag == "Stop")
                {
                    building = false;
                    yield break;
                }
                else if (child.tag == "End")
                {
                    if (StaticHelpers.Vector3Distance(transform.TransformPoint(transform.position),transform.TransformPoint(child.position)) >= 50f)
                    {
                        Transform currentParent = currentWall.transform.parent;
                        Destroy(currentWall);
                        currentWall = currentWall = Instantiate(wall_End, currentParent);
                        currentWall.transform.RotateAround(currentWall.transform.position, Vector3.up, Random.Range(maxAngle, minAngle) + 180);
                        building = false;
                        yield break; //Break out if we make another end cap
                    }
                    num = WallOdds();
                    if (num == 0)
                    {
                        currentWall = Instantiate(wall_End, child);
                        currentWall.transform.RotateAround(currentWall.transform.position, Vector3.up, Random.Range(maxAngle, minAngle) + 180);
                        building = false;
                        yield break; //Break out if we make another end cap
                    }
                    else if (num == 1)
                    {
                        currentWall = Instantiate(wall_Mid, child);
                        currentWall.transform.RotateAround(currentWall.transform.position, Vector3.up, Random.Range(maxAngle, minAngle));
                        break;
                    }
                    else
                    {
                        currentWall = Instantiate(wall_Gap, child);
                        currentWall.transform.RotateAround(currentWall.transform.position, Vector3.up, Random.Range(maxAngle, minAngle));
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    private int WallOdds()
    {
        float pickedNum = Random.Range(0, totalOdds);
        if(pickedNum <= end_Chance)
        {
            return 0;
        }
        else
        {
            if(pickedNum <= mid_Chance + end_Chance)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

    }
}