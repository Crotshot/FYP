using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using Helpers = Crotty.Helpers.StaticHelpers;

/*
 * Linear Congruential Generator https://en.wikipedia.org/wiki/Linear_congruential_generator
 */

public class MapBuilder : MonoBehaviour
{
    float currentSeed, multiplier = 257321, increment = 802997, modulus = 689101, state = 123456, constant = 84327;
    [SerializeField] int seed = 874326894;
    /* tiles 0-Green 1-Yellow 2-White 3-Blue 4-Purple
     * walls
     * buildings
     * props
     * points
     */
    [SerializeField] GameObjectVariants[] tiles, walls, buildings, props, points, centralPoints, basePoints, sidePoints;
    [SerializeField] Path[] paths;
    const int OFFSET_X = 25, OFFSET_Y = 25, MAP_WIDTH = 8, MAP_LENGTH = 8;
    private bool generated;

    List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();
    public Transform m_Tracked; //Center of build
    public Vector3 m_Size = new Vector3(400.0f, 10.0f, 400.0f);// The size of the build bounds
    NavMeshData m_NavMesh;
    NavMeshDataInstance m_Instance;

#if UNITY_EDITOR
    public bool testGeneration, destroyMap;
    private void Update() {
        if (testGeneration) {
            ChangeSeed(seed);
            testGeneration = false;
            Generate();
        }
        else if (destroyMap) {
            destroyMap = false;
            DestroyMap();
        }
    }
#endif

    private void Start() {
        ChangeSeed(seed);
        m_NavMesh = new NavMeshData();
        m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
        StartCoroutine("MakeMapDelayed");
    }

    IEnumerator MakeMapDelayed() {
        yield return new WaitForSeconds(0.01f);
        Generate();
    }

    private void OnDestroy() {
        m_Instance.Remove();// Unload navmesh
    }

    //Loop for width 8
    public void Generate() {
        if (generated) {
            Debug.LogWarning("Map is already created");
            return;
        }
        //Make child object
        GameObject SideOne = new GameObject();
        SideOne.transform.parent = transform;
        SideOne.transform.position = Vector3.zero;
        SideOne.name = "Side_1";

        int index = 0; 
        //for MAP_W for MAP_LENGTH/2 make tiles, count index and check against array for different tiles
        for (int h = (MAP_LENGTH / 2) - 1; h > -1; h--) { // Tile pos x = 25 + (w * 50)   && z = 25 + (h * 50)
            for (int w = -MAP_WIDTH / 2; w < MAP_WIDTH / 2; w++) {
                GameObject tile, prefab;
                if(index < 2 || index > 5 && index < 9 || index > 14 && index < 17 || index == 23 || index == 31) { //WHITE TILES
                    //TODO add random selection of tile variants
                    prefab = tiles[2].getVariant(0);
                }
                else if(index > 1 && index < 6 || index > 9 && index < 14 || index == 17 || index > 26 && index < 29 || index == 30) { //GREEN TILES
                    prefab = tiles[0].getVariant(0);
                }
                else if (index == 24 || index == 26) { //PURPLE TILES
                    prefab = tiles[4].getVariant(0);
                }
                else if (index > 19 && index < 22) { //BLUE TILES
                    prefab = tiles[3].getVariant(0);
                }
                else{ //YELLOW TILESw
                    prefab = tiles[1].getVariant(0);
                }
                tile = Instantiate(prefab, new Vector3(25 + (w * 50), 0, 25 + (h * 50)), Quaternion.identity, SideOne.transform);
                index++;
            }
        }


        //Removing poorly placed Structures & Props
        foreach (Transform mapSide in transform) {
            foreach (Transform tile in mapSide) {
                List<Transform> structurePositions = new List<Transform>();
                foreach (Transform point in tile) {
                    //Removing Objects at edge of map
                    if(Mathf.Abs(point.position.x) >= 185f || Mathf.Abs(point.position.z) >= 185f || point.position.z < 0) {
                        Destroy(point.gameObject);
                        continue;
                    }
                    //Removing Objects within bases
                    if (Helpers.Vector2DistanceXZ(point.position, new Vector3(0,0,200)) <= 90f) {
                        Destroy(point.gameObject);
                    }
                    //Removing Objects from Centre of map
                    if (Helpers.Vector2DistanceXZ(point.position, Vector3.zero) <= 27f) {
                        Destroy(point.gameObject);
                    }
                    //Removing Objects inside Gate Walls
                    if (Helpers.Vector2DistanceXZ(point.position, new Vector3(-200,0,25)) <= 40f || Helpers.Vector2DistanceXZ(point.position, new Vector3(200, 0, -25)) <= 40f) {
                        Destroy(point.gameObject);
                    }
                    //Removing Objects inside Angle Walls
                    if (Helpers.Vector2DistanceXZ(point.position, new Vector3(190, 0, 5)) <= 22f || Helpers.Vector2DistanceXZ(point.position, new Vector3(100, 0, 182.5f)) <= 22f || Helpers.Vector2DistanceXZ(point.position, new Vector3(-100, 0, 182.5f)) <= 22f) {
                        Destroy(point.gameObject);
                    }
                    //Remove Objects off Side Points
                    if (Helpers.Vector2DistanceXZ(point.position, new Vector3(-125, 0, 75)) <= 12f || Helpers.Vector2DistanceXZ(point.position, new Vector3(125, 0, 25)) <= 12f)  {
                        Destroy(point.gameObject);
                    }
                    
                    foreach(Path path in paths) {
                        if(Helpers.Vector2PerpendicularXZ(path.getPointA(), path.getPointB(), point.position, 8) <= 8) {
                            Destroy(point.gameObject);
                        }
                    }

                    if (point.tag.Equals("structurePos")) {
                        structurePositions.Add(point);
                    }
                }

                //Pick Random random building and place on random position
                if(structurePositions.Count > 0) {
                    int bIndex = 0;
                    if (tile.name.Contains("Inner"))
                        bIndex = 0;
                    else
                        bIndex = 1;
                    GameObject pickedBuilding = buildings[bIndex].getVariant(RandomPick(0, buildings[bIndex].VariantCount()-1));
                    Transform pickedPos = structurePositions[RandomPick(0, structurePositions.Count-1)];
                    Instantiate(pickedBuilding, pickedPos);

                    float angle = -90;
                    if (Mathf.Abs(tile.position.z) == Mathf.Abs(tile.position.x)) {
                        if (tile.position.x < 0)
                            angle -= 135;
                        else
                            angle -= 45;
                    }
                    else if (Mathf.Abs(tile.position.z) >= Mathf.Abs(tile.position.x)) {
                        angle -= 90;
                    }
                    else if (tile.position.x < 0) {
                        angle -= 180;
                    }

                    if (tile.name.Contains("Outer"))
                        pickedPos.transform.eulerAngles = new Vector3(0, angle, 0);

                    foreach (Transform pos in structurePositions) {
                        if (pos != pickedPos)
                            Destroy(pos.gameObject);
                    }
                }
            }
        }

        //MAP SIDE 2
        //GameObject SideTwo = Instantiate(SideOne, transform);
        //SideTwo.transform.RotateAround(transform.position, Vector3.up, 180f);
        //SideTwo.name = "Side_2";

        m_Sources.Clear();
        foreach (Transform mapSide in transform) {
            foreach (Transform tile in mapSide) {
                Mesh sMesh = tile.GetComponent<MeshFilter>().sharedMesh;
                MeshFilter meshF = tile.GetComponent<MeshFilter>();
                if (meshF == null || sMesh == null) continue;
                var s = new NavMeshBuildSource {
                    shape = NavMeshBuildSourceShape.Mesh,
                    sourceObject = sMesh,
                    transform = meshF.transform.localToWorldMatrix,
                    area = 0
                };
                m_Sources.Add(s);
            }
        }
        UpdateNavMesh();
        generated = true;
    }

    public void DestroyMap() {
        generated = false;
        Destroy(transform.GetChild(0).gameObject);
        Destroy(transform.GetChild(1).gameObject);
    }

    void UpdateNavMesh() {
        var defaultBuildSettings = NavMesh.GetSettingsByID(0);
        var center = m_Tracked ? m_Tracked.position : transform.position;
        NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_Sources, new Bounds(Quantize(center, 0.1f * m_Size), m_Size));
    }

    static Vector3 Quantize(Vector3 v, Vector3 quant) {
        float x = quant.x * Mathf.Floor(v.x / quant.x);
        float y = quant.y * Mathf.Floor(v.y / quant.y);
        float z = quant.z * Mathf.Floor(v.z / quant.z);
        return new Vector3(x, y, z);
    }


    private void ChangeSeed(float seed) {
        currentSeed = seed;
        state = (currentSeed * multiplier + increment) % modulus;
        Shuffle();
    }

    private float Shuffle() {
        state = (multiplier * state + increment) % modulus;
        return state % constant;
    }

    private int RandomPick(int min, int max) {
        return (int)(min + Mathf.Floor((max - min + 1) * Shuffle() / (constant - 1)));
    }
}
/*
 * NOTE: Cool little idea where the players load in as a high up static camera, and then we build 
 * the map nice and slowly, adding little paricle effects and some moving rats with little construction hats 
 */
[System.Serializable]
class GameObjectVariants {
    [SerializeField] private GameObject[] variants;

    public GameObject[] getVariants() {
        return variants;
    }

    public GameObject getVariant(int index) {
        return variants[index];
    }

    public int VariantCount() {
        return variants.Length;
    }
}

[System.Serializable]
class Path {
    [SerializeField] private Transform pointA, pointB;

    public Vector3 getPointA() {
        return pointB.position;
    }

    public Vector3 getPointB() {
        return pointA.position;
    }
}