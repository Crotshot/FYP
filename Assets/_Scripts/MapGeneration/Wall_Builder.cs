using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers = Crotty.Helpers.StaticHelpers;

public class Wall_Builder : MonoBehaviour
{
 //OLD Wall building class, will probably be removed before long

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
                    if (Helpers.Vector3Distance(transform.TransformPoint(transform.position),transform.TransformPoint(child.position)) >= 50f)
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