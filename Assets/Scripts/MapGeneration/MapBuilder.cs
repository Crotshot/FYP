using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using Helpers = Crotty.Helpers.StaticHelpers;
using Mirror;

/*
 * Linear Congruential Generator https://en.wikipedia.org/wiki/Linear_congruential_generator
 */

public class MapBuilder : NetworkBehaviour {
    //Random generator
    [SerializeField] bool oldMap;
    float currentSeed, multiplier = 257321, increment = 802997, modulus = 689101, state = 123456, constant = 84327;
    [SerializeField] int seed = 874326894;
    //Prefabs
    [SerializeField] GameObjectVariants[] tiles, buildings, largeProps, smallProps;//, centralPoints, basePoints, sidePoints; ->Maybe later add different aesthetic points
    [SerializeField] Path[] paths;
    const int MAP_WIDTH = 8, MAP_LENGTH = 8;
    private bool buildingsPlaced, sideTwo, cleaned;

    [SerializeField] float largePropDist = 11f, largePropClearing = 40f, smallPropDist = 3f, outerBuildingClearing = 7f, innerBuildingClearing = 12f, halfPathWidth = 8f;
    //Nav mesh baking
    List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();
    private Transform m_Tracked; //Center of build
    private Vector3 m_Size = new Vector3(400.0f, 1.0f, 400.0f);// The size of the build bounds
    NavMeshData m_NavMesh;
    NavMeshDataInstance m_Instance;

    private List<Vector3> outerBuildingPositions = new List<Vector3>(),
        innerBuildingPositions = new List<Vector3>(),
        COPYouterBuildingPositions = new List<Vector3>(),
        COPYinnerBuildingPositions = new List<Vector3>();
    private List<int> innerBuildingNums = new List<int>(), outerBuildingNums = new List<int>();

    public bool regen;
    private void Update() {
        if (isServer && regen) {
            regen = false;
            //Delete all the buildings
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("structure")) {
                NetworkServer.Destroy(obj);
            }
            RpcRegen(); //Gets called on server aswell as the host is a client
        }
    }

    private void RegenerateMap() {
        Destroy(transform.GetChild(1).gameObject);
        Destroy(transform.GetChild(2).gameObject);
        buildingsPlaced = false;
        sideTwo = false;
        cleaned = false;
        GenerateMap(seed);
    }

    [ClientRpc]
    private void RpcRegen() {
        RegenerateMap();
    }

    public void GenerateMap(int seed) {
        this.seed = seed;
        if (seed < 0)
            seed *= -1;
        ChangeSeed(seed);
        m_NavMesh = new NavMeshData();
        m_Instance = NavMesh.AddNavMeshData(m_NavMesh);


        if (oldMap) {
            StartCoroutine("MakeMapDelayed");
        }
        else {
            GenerateNew();
        }
    }
    /* How the new map generation works
     * 1. Pick 1 random point per tile and place a flora Vector, adding it to the floraPropList & pointsToCheckList;
     * 2. while the pointsToCheckList.Count > 0, do 30 attempts per position to place more points.
     * 3. To pick a point it uses the seeded Linear Congruential Generator to pick a point on the x,z that is within +- max flora radius,
     * the points distance must be within the max radius but outside the min radius. If the point is a valid place away from the point that is generated from
     * it then checks all other flora points to see if it is valid in their radius.
     * 4. Map Boundaries are in place to prevent an infinite loop of point generation, with this method points are guaranteed to run out eventually.
     * 5. Perlin noise is used here to adjust the min and max radius, this creates a very natural looking spread of points.
     * 6. After a point passes the checks it is added to both the pointsToCheckList and the Flora List
     * 7 => We repeat this proeces for the smallProps but this time the after the pointsToCheckList is exhausted it removes all flora within its clearingDistance
     * 8 => And the same again for large props except this time again we remove flora and also smallProps.
     * 9 => Finally buildings spots are picked and for inner and outer and all Large, small and flora props are removed within the clearing distance. 
     */
    [SerializeField] int attempts = 30;
    int attemptCounter = 0;
    [SerializeField] bool useNoise;
    [SerializeField]
    float floraClearMin, floraClearMax, smallClearMin, smallClearMax, largeClearMin, largeClearMax,
        floraNoiseModifier, smallNoiseModifier, largeNoiseModifier, smallClearingDistance, 
        largeClearingDistance, outerBuildingClearingDistance, innerBuildingClearingDistance; 
    List<Vector3> innerBuilingList, outerBuildingList, floraPropList, smallPropList, largePropList, pointsToCheckList;
    
    private void GenerateNew() {
        for(int x = 0; x < 8; x++) {
            for(int z = 0; z < 8; z++) {
                Vector2 point = new Vector2(x * 50, z * -50);
                point = RandomPoint(point, point + new Vector2(50, -50));
                floraPropList.Add(point);
                pointsToCheckList.Add(point);
            }
        }
        while (pointsToCheckList.Count > 0) {
            attemptCounter = 0;
            while (attemptCounter < attempts) { //Random points within max dist
                attemptCounter++;
                Vector2 point = RandomPointCentered(pointsToCheckList[pointsToCheckList.Count -1], floraClearMax);
                if(point.x < 0 || point.x > 400 || point.y > 0 || point.y < 200) {
                    continue;
                }
                float dist = Helpers.Vector2Distance(pointsToCheckList[pointsToCheckList.Count - 1], point); //Check if valid to parent point if not skip to next
                if (dist > floraClearMin && dist < floraClearMax) {
                    bool success = true;
                    foreach (Vector2 floraPoint in floraPropList) {
                        dist = Helpers.Vector2Distance(floraPoint, point);
                        if (dist < floraClearMin) {
                            success = false;
                            break;
                        }
                    }
                    if (success) {
                        floraPropList.Add(point);
                        pointsToCheckList.Add(point);
                    }
                }
                else
                    continue;
            }
            pointsToCheckList.RemoveAt(pointsToCheckList.Count - 1);
        }

        foreach(Vector2 floraPos in floraPropList) {
            Instantiate(smallProps[5].getVariant(0), new Vector3(floraPos.x, 0, floraPos.y), Quaternion.identity);
        }
    }
//OLD MAP GEN
/*
    IEnumerator MakeMapDelayed() {
        yield return new WaitForSeconds(0.01f);
        Generate();
    }
    private void OnDestroy() {
        m_Instance.Remove();// Unload navmesh
    }
    public void Generate() {
        GameObject SideOne = new GameObject();
        SideOne.transform.parent = transform;
        SideOne.transform.position = Vector3.zero;
        SideOne.name = "Side_1";
        #region MapTileInstantiating
        int index = 0;
        //for MAP_W for MAP_LENGTH/2 make tiles, count index and check against array for different tiles
        for (int h = (MAP_LENGTH / 2) - 1; h > -1; h--) { // Tile pos x = 25 + (w * 50)   && z = 25 + (h * 50)
            for (int w = -MAP_WIDTH / 2; w < MAP_WIDTH / 2; w++) {
                GameObject tile, prefab;
                if (index < 2 || index > 5 && index < 9 || index > 14 && index < 17 || index == 23 || index == 31) { //WHITE TILES
                    //TODO add random selection of tile variants
                    prefab = tiles[2].getVariant(0);
                }
                else if (index > 1 && index < 6 || index > 9 && index < 14 || index == 17 || index > 26 && index < 29 || index == 30) { //GREEN TILES
                    prefab = tiles[0].getVariant(0);
                }
                else if (index == 24 || index == 26) { //PURPLE TILES
                    prefab = tiles[4].getVariant(0);
                }
                else if (index > 19 && index < 22) { //BLUE TILES
                    prefab = tiles[3].getVariant(0);
                }
                else { //YELLOW TILESw
                    prefab = tiles[1].getVariant(0);
                }
                tile = Instantiate(prefab, new Vector3(25 + (w * 50), 0, 25 + (h * 50)), Quaternion.identity, SideOne.transform);
                index++;
            }
        }
        #endregion

        //Removing poorly placed Structures & Props
        foreach (Transform mapSide in transform) {
            foreach (Transform mapTile in mapSide) {
                List<Transform> structurePositions = new List<Transform>();
                List<Transform> largePropPositions = new List<Transform>();
                List<Transform> smallPropPositions = new List<Transform>();

                foreach (Transform point in mapTile) {
                    #region DeletingBadPositions
                    //Removing Objects at edge of map
                    if (Mathf.Abs(point.position.x) >= 185f || Mathf.Abs(point.position.z) >= 185f || point.position.z < 0) {
                        Destroy(point.gameObject);
                        continue;
                    }
                    //Removing Objects within bases
                    if (Helpers.Vector2DistanceXZ(point.position, new Vector3(0, 0, 200)) <= 90f) {
                        Destroy(point.gameObject);
                        continue;
                    }
                    //Removing Objects from Centre of map
                    if (Helpers.Vector2DistanceXZ(point.position, Vector3.zero) <= 27f) {
                        Destroy(point.gameObject);
                        continue;
                    }
                    //Removing Objects inside Gate Walls
                    if (Helpers.Vector2DistanceXZ(point.position, new Vector3(-200, 0, 25)) <= 40f || Helpers.Vector2DistanceXZ(point.position, new Vector3(200, 0, -25)) <= 40f) {
                        Destroy(point.gameObject);
                        continue;
                    }
                    //Removing Objects inside Angle Walls
                    if (Helpers.Vector2DistanceXZ(point.position, new Vector3(190, 0, 5)) <= 22f || Helpers.Vector2DistanceXZ(point.position, new Vector3(100, 0, 182.5f)) <= 22f || Helpers.Vector2DistanceXZ(point.position, new Vector3(-100, 0, 182.5f)) <= 22f) {
                        Destroy(point.gameObject);
                        continue;
                    }
                    //Remove Objects off Side Points
                    if (Helpers.Vector2DistanceXZ(point.position, new Vector3(-125, 0, 75)) <= 12f || Helpers.Vector2DistanceXZ(point.position, new Vector3(125, 0, 25)) <= 12f) {
                        Destroy(point.gameObject);
                        continue;
                    }
                    bool cont = false;
                    foreach (Path path in paths) {
                        if (Helpers.Vector2PerpendicularXZ(path.getPointA(), path.getPointB(), point.position, halfPathWidth) <= halfPathWidth) {
                      //if (Helpers.Vector2PerpendicularXZ(path.getPointA(), path.getPointB(), point.position, 8) <= 8) {
                            Destroy(point.gameObject);
                            cont = true;
                            break;
                        }
                    }
                    if (cont)
                        continue;
                    #endregion
                    if (point.tag.Equals("structurePos")) {
                        structurePositions.Add(point);
                    }
                    else if (point.tag.Equals("largeProp")) {
                        largePropPositions.Add(point);
                    }
                    else if (point.tag.Equals("smallProp")) {
                        smallPropPositions.Add(point);
                    }
                }

                //Pick Random random building and place on random position
                //Transform pickedPos;
                //int pickedBuilding;
                #region CapturableStructurePlacement
                if (structurePositions.Count > 0) {
                   // if (isServer) { //Positions are needed on the client
                    if (mapTile.name.Contains("Inner")) {
                        innerBuildingNums.Add(RandomPick(0, buildings[0].VariantCount() - 1));
                        innerBuildingPositions.Add(structurePositions[RandomPick(0, structurePositions.Count - 1)].position);
                        COPYinnerBuildingPositions.Add(innerBuildingPositions[innerBuildingPositions.Count -1]);
                    }
                    else {
                        outerBuildingNums.Add(RandomPick(0, buildings[1].VariantCount() - 1));
                        outerBuildingPositions.Add(structurePositions[RandomPick(0, structurePositions.Count - 1)].position);
                        COPYouterBuildingPositions.Add(outerBuildingPositions[outerBuildingPositions.Count - 1]);
                    }
                    //}

                    foreach (Transform pos in structurePositions) {
                        Destroy(pos.gameObject);
                    }
                    structurePositions.Clear();
                }
                #endregion
                #region LargePropPlacement
                Transform pickedPos;
                while (largePropPositions.Count > 0) {
                    //Pick Large Prop type
                    float totalWeight = 0; //Weigh odds of picking a large prop variant
                    foreach (GameObjectVariants variants in largeProps) {
                        totalWeight += variants.weight;
                    }
                    if (totalWeight == 0) { Debug.Log("No Large Prop variants"); break; } //No large props, break out of loop

                    float type = RandomFloat(0, totalWeight), count = 0;
                    index = 0;
                    foreach (GameObjectVariants variants in largeProps) {
                        count += variants.weight;
                        if (count >= type) break; else index++;
                    }
                    totalWeight = 0;//Weigh odds of picking a prop from the selected variant
                    //Pick variant
                    GameObjectVariants variant = largeProps[index];
                    foreach (float num in variant.getWeights()) {
                        totalWeight += num;
                    }
                    if (totalWeight == 0) { Debug.Log("No Large Props in variant"); break; } //No props in this variant

                    count = 0;
                    type = RandomFloat(0, totalWeight);
                    index = 0;
                    foreach (float num in variant.getWeights()) {
                        count += num;
                        if (count >= type) break; else index++;
                    }
                    //Pick a place and spawn prop
                    int _num = RandomPick(0, largePropPositions.Count - 1);
                    pickedPos = largePropPositions[_num];
                    if (_num < 0 || _num > smallPropPositions.Count) {
                        Debug.Log("Skipped one prop as _num was not valid");
                    }
                    else {
                        GameObject obj = Instantiate(variant.getVariant(index), pickedPos);
                        obj.transform.eulerAngles = new Vector3(0, RandomFloat(0f, 360f), 0);
                    }
                    //Debug.Log("Instantiated large prop: " + variant.getVariant(index).name);
                    largePropPositions.Remove(pickedPos); //Remove from list
                    //Delete all propPositions in general area that have no children  ~~ 12m
                    foreach (Transform point in mapTile) {
                        if (point.tag.Equals("largeProp") && point != pickedPos && Helpers.Vector2DistanceXZ(pickedPos.position, point.position) <= largePropDist){// && point.childCount < 1) {// && point.childCount < 1) {
                            Destroy(point.gameObject);
                            continue;
                        }
                        else if (!point.tag.Equals("largeProp") && Helpers.Vector2DistanceXZ(pickedPos.position, point.position) <= largePropClearing && point.childCount < 1) {
                            Destroy(point.gameObject);
                            continue;
                        }
                    }
                }
                #endregion
                #region SmallPropPlacement
                while (smallPropPositions.Count > 0) {
                    //Pick Small Prop type
                    float totalWeight = 0; //Weigh odds of picking a small prop variant
                    foreach (GameObjectVariants variants in smallProps) {
                        totalWeight += variants.weight;
                    }
                    if (totalWeight == 0) { Debug.Log("No Small Prop variants"); break; } //No small props, break out of loop

                    float type = RandomFloat(0, totalWeight), count = 0;
                    index = 0;
                    foreach (GameObjectVariants variants in smallProps) {
                        count += variants.weight;
                        if (count >= type) break; else index++;
                    }
                    totalWeight = 0;//Weigh odds of picking a prop from the selected variant
                    //Pick variant
                    GameObjectVariants variant = smallProps[index];
                    foreach (float num in variant.getWeights()) {
                        totalWeight += num;
                    }
                    if (totalWeight == 0) { Debug.Log("No Small Props in variant"); break; } //No props in this variant

                    count = 0;
                    type = RandomFloat(0, totalWeight);
                    index = 0;
                    foreach (float num in variant.getWeights()) {
                        count += num;
                        if (count >= type) break; else index++;
                    }
                    //Pick a place and spawn prop
                    int _num = RandomPick(0, smallPropPositions.Count - 1);
                    pickedPos = smallPropPositions[_num];
                    if (_num < 0 || _num > smallPropPositions.Count) {
                        Debug.Log("Skipped one prop as _num was not valid");
                    }
                    else {
                        GameObject obj = Instantiate(variant.getVariant(index), pickedPos);
                        obj.transform.eulerAngles = new Vector3(0, RandomFloat(0f, 360f), 0);
                    }
                    //Debug.Log("Instantiated small prop: " + variant.getVariant(index).name);
                    smallPropPositions.Remove(pickedPos); //Remove from list
                    //Delete all propPositions in close proximity that have no children  ~~ 3m
                    foreach (Transform point in mapTile) {
                        if (Helpers.Vector2DistanceXZ(pickedPos.position, point.position) <= smallPropDist && point.childCount < 1) {
                            Destroy(point.gameObject);
                            continue;
                        }
                    }
                }
                #endregion
            }
        }

        if (isServer)
            StartCoroutine("CreateBuildings");

        StartCoroutine("SideTwoDelay");
    }
    IEnumerator CreateBuildings() {        
        yield return new WaitForSeconds(0.01f);
        while(innerBuildingNums.Count > 0) {
            GameObject building_1 = Instantiate(buildings[0].getVariant(innerBuildingNums[innerBuildingNums.Count - 1]),null),
                building_2 = Instantiate(buildings[0].getVariant(innerBuildingNums[innerBuildingNums.Count - 1]), null);
            Vector3 pos = innerBuildingPositions[innerBuildingNums.Count - 1];
            NetworkServer.Spawn(building_1);
            building_1.transform.position = pos;
            building_1.transform.RotateAround(building_1.transform.position, building_1.transform.up, 180);

            NetworkServer.Spawn(building_2);
            building_2.transform.position = new Vector3(-pos.x, pos.y, -pos.z);


            innerBuildingNums.RemoveAt(innerBuildingNums.Count - 1);
            innerBuildingPositions.RemoveAt(innerBuildingPositions.Count - 1);
        }

        while (outerBuildingNums.Count > 0) {
            GameObject building_1 = Instantiate(buildings[1].getVariant(outerBuildingNums[outerBuildingNums.Count - 1]), null),
                building_2 = Instantiate(buildings[1].getVariant(outerBuildingNums[outerBuildingNums.Count - 1]), null);
            Vector3 pos = outerBuildingPositions[outerBuildingNums.Count - 1];
            NetworkServer.Spawn(building_1);
            building_1.transform.position = pos;

            Vector3 rounded = new Vector3(Helpers.Round((int)pos.x, 50, 25),0, Helpers.Round((int)pos.z, 50, 25));
            float angle = -90;
            if (Mathf.Abs(rounded.z) == Mathf.Abs(rounded.x)) {
                if (rounded.x < 0)
                    angle -= 135;
                else
                    angle -= 45;
            }
            else if (Mathf.Abs(rounded.z) >= Mathf.Abs(rounded.x)) {
                angle -= 90;
            }
            else if (rounded.x < 0) {
                angle -= 180;
            }

            building_1.transform.eulerAngles = new Vector3(0, angle, 0);

            NetworkServer.Spawn(building_2);
            building_2.transform.position = new Vector3(-pos.x, pos.y, -pos.z);
            building_2.transform.eulerAngles = new Vector3(0, building_1.transform.eulerAngles.y + 180, 0);

            outerBuildingNums.RemoveAt(outerBuildingNums.Count - 1);
            outerBuildingPositions.RemoveAt(outerBuildingPositions.Count - 1);
        }
        buildingsPlaced = true;
    }
    [ClientRpc]
    private void BuildingsPlaced() {
        buildingsPlaced = true;
    }
    IEnumerator SideTwoDelay() {
        while (!buildingsPlaced)
            yield return new WaitForSeconds(0.01f);
        StartCoroutine("Clean");

        while (!cleaned)
            yield return new WaitForSeconds(0.01f);
        SideTwo();
    }
    private void SideTwo() {
        GameObject SideTwo = Instantiate(transform.GetChild(1).gameObject, transform);
        SideTwo.transform.eulerAngles = new Vector3(0, 180f, 0);
        SideTwo.name = "Side_2";

        StartCoroutine("BakeDelay");
    }
    IEnumerator Clean() {
        foreach (Transform mapSide in transform) {
            foreach (Transform tile in mapSide) {
                foreach (Transform prop in tile) {
                    foreach(Vector3 pos in COPYouterBuildingPositions) {
                        if(Helpers.Vector3Distance(pos, prop.position) <= outerBuildingClearing) {
                            Destroy(prop.gameObject);
                            Debug.Log("DeletedProp => Too close to outer building");
                        }
                    }
                    foreach (Vector3 pos in COPYinnerBuildingPositions) {
                        if (Helpers.Vector3Distance(pos, prop.position) <= innerBuildingClearing) {
                            Destroy(prop.gameObject);
                            Debug.Log("DeletedProp => Too close to inner building");
                        }
                    }
                }
            }
        }
        COPYouterBuildingPositions.Clear();
        COPYinnerBuildingPositions.Clear();
        yield return new WaitForSeconds(1f);
        cleaned = true;
    }
    IEnumerator BakeDelay() {
        yield return new WaitForSeconds(1f);
        BakeMap();
    }
    private void BakeMap() {
        m_Sources.Clear();
        foreach (Transform wall in transform.GetChild(0)) {
            Mesh sMesh = null;
            if (wall.TryGetComponent<MeshFilter>(out MeshFilter mesh)) {
                if (mesh != null)
                    sMesh = mesh.sharedMesh;
                else
                    continue;
            }
            MeshFilter meshF = wall.GetComponent<MeshFilter>();
            if (meshF == null || sMesh == null) continue;
            var s = new NavMeshBuildSource {
                shape = NavMeshBuildSourceShape.Mesh,
                sourceObject = sMesh,
                transform = meshF.transform.localToWorldMatrix,
                area = 0
            };
            m_Sources.Add(s);
        }
        #region  Tiles
        foreach (Transform tile in transform.GetChild(1)) {
            Mesh sMesh = null;
            if (tile.TryGetComponent<MeshFilter>(out MeshFilter mesh)) {
                if (mesh != null)
                    sMesh = mesh.sharedMesh;
                else
                    continue;
            }
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
        foreach (Transform tile in transform.GetChild(2)) {
            Mesh sMesh = null;
            if (tile.TryGetComponent<MeshFilter>(out MeshFilter mesh)) {
                if (mesh != null)
                    sMesh = mesh.sharedMesh;
                else
                    continue;
            }
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
        #endregion
        #region Props
        foreach (Transform mapSide in transform) {
            foreach (Transform tile in mapSide) {
                foreach (Transform prop in tile) {
                    if (prop.GetChild(0).tag.Equals("navExempt"))
                        continue;
                    Mesh sMesh = null;
                    if (prop.GetChild(0).TryGetComponent<MeshFilter>(out MeshFilter mesh)) {
                        if (mesh != null)
                            sMesh = mesh.sharedMesh;
                        else
                            continue;
                    }
                    MeshFilter meshF = prop.GetChild(0).GetComponent<MeshFilter>();
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
        }

        #endregion
        UpdateNavMesh();
        Debug.Log("Making nav mesh");
        StopAllCoroutines();
    }
*/

    void UpdateNavMesh() {
        var defaultBuildSettings = NavMesh.GetSettingsByID(0);
        defaultBuildSettings.agentRadius = 0.5f;
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

    private float RandomFloat(float min, float max) {
        return min + (max - min) * Shuffle() / (constant - 1);
    }
    /// <summary>
    /// Gets Random point in a square corner1 corner2 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private Vector2 RandomPoint(Vector2 corner1, Vector2 corner2) {
        return new Vector2(RandomFloat(corner1.x, corner2.x), RandomFloat(corner1.y, corner2.y));
    }

    private Vector2 RandomPointCentered(Vector2 corner, float radius) {
        Vector2 corner1 = new Vector2(corner.x + radius, corner.y + radius),
            corner2 = new Vector2(corner.x - radius, corner.y - radius);
        return new Vector2(RandomFloat(corner1.x, corner2.x), RandomFloat(corner1.y, corner2.y));
    }
}
/*
 * NOTE: Cool little idea where the players load in as a high up static camera, and then we build 
 * the map nice and slowly, adding little paricle effects and some moving rats with little construction hats 
 */
[System.Serializable]
class GameObjectVariants {
    [SerializeField] private GameObject[] variants;
    [SerializeField] private float[] weights; //Weight of each variant
    [SerializeField] public float weight; //Weight of being selected 
    public GameObject[] getVariants() {
        return variants;
    }

    public GameObject getVariant(int index) {
        return variants[index];
    }

    public float[] getWeights() {
        return weights;
    }

    public float getWeight(int index) {
        return weights[index];
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


//foreach (Transform tile in transform) {
//    Mesh sMesh = null;
//    if (tile.TryGetComponent<MeshFilter>(out MeshFilter mesh)) {
//        if (mesh != null)
//            sMesh = mesh.sharedMesh;
//        else
//            continue;
//    }
//    MeshFilter meshF = tile.GetComponent<MeshFilter>();
//    if (meshF == null || sMesh == null) continue;
//    var s = new NavMeshBuildSource {
//        shape = NavMeshBuildSourceShape.Mesh,
//        sourceObject = sMesh,
//        transform = meshF.transform.localToWorldMatrix,
//        area = 0
//    };
//    m_Sources.Add(s);
//}