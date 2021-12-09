using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] int seed = 874326894;
    /* tiles 0-Green 1-Yellow 2-White 3-Blue 4-Purple
     * walls
     * buildings
     * props
     * points
     */
    [SerializeField] GameObjectVariants[] tiles, walls, buildings, props, points;
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
        m_NavMesh = new NavMeshData();
        m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
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

        GameObject SideTwo = Instantiate(SideOne, transform);
        SideTwo.transform.RotateAround(transform.position, Vector3.up, 180f);

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

}
/*
 * Map tiles are 50x50y units and are displaced by 25x25y units so that the grid is centred at 0,0 
 * A map side is 8 wide and 4 long so the normal game map is a 8x8 of tiles, we generate 1/2 and 
 * then duplicate it and rotate 180* around y axis so each player gets a fair map
 * 
 * NOTE: Cool little idea where the players load in as a high up static camera, and then we build 
 * the map nice and slowly, adding little paricle effects and some moving rats
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
}