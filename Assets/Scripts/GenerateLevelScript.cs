using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class GenerateLevelScript : MonoBehaviour {
    public GameObject ground;
    public GameObject originalWall;
    public GameObject winTrigger;
    public GameObject winShow1;
    public GameObject winShow2;
    public GameObject winShow3;
    public GameObject winShow4;

    private int blockSize = 10;
    private int wallHeight = 3;
	// Use this for initialization
	void Start () {
        makeWalls(makeMap());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void makeWalls(Dictionary<Vector4, bool> wallList)
    {
        Vector3 planeOrigin = new Vector3(-1 * (ground.transform.localScale.x / 2), 0, -1 * (ground.transform.localScale.y / 2));
        foreach (Vector4 oneWall in wallList.Keys)
        {

            if (wallList[oneWall] == false)
            {
                continue;
            }

            GameObject cube = Instantiate(originalWall);
            
            //todo 3 and 1.5 should be parameters somewheer
            if (new Vector2(oneWall.z, oneWall.w) - new Vector2(oneWall.x, oneWall.y) == new Vector2(0, 1))
            {
              
                cube.transform.localScale = new Vector3(1, wallHeight, (float)blockSize);
                cube.transform.position = planeOrigin + new Vector3(oneWall.x * blockSize + (1f * .5f), wallHeight*0.5f, oneWall.y * blockSize + (blockSize * .5f));
            }
            else
            {
               
                cube.transform.position = planeOrigin + new Vector3(oneWall.x * blockSize + (blockSize * .5f), wallHeight*0.5f, oneWall.y * blockSize + (1f * .5f));
                cube.transform.localScale = new Vector3((float)blockSize, wallHeight, 1);
            }
            
            cube.SetActive(true);
        }
        int rows = (int)(ground.transform.localScale.x) / blockSize;
        int cols = (int)(ground.transform.localScale.y) / blockSize;
        winTrigger.transform.position = planeOrigin + new Vector3((rows - 1) * blockSize + (blockSize*.5f) + .5f, winTrigger.transform.position.y, (cols - 1) * blockSize + (blockSize*.5f) + .5f);
        winTrigger.transform.localScale = new Vector3(blockSize - .5f, winTrigger.transform.localScale.y, blockSize - .5f);

        winShow1.transform.position = planeOrigin + new Vector3((rows - 1) * blockSize + (blockSize * .5f) + .25f, winShow1.transform.position.y, (cols - 1) * blockSize + .25f);
        winShow1.transform.localScale = new Vector3(blockSize - .5f, winShow1.transform.localScale.y, 0.25f);
        winShow2.transform.position = planeOrigin + new Vector3((rows - 1) * blockSize + .25f, winShow2.transform.position.y, (cols - 1) * blockSize + (blockSize * .5f) + .25f);
        winShow2.transform.localScale = new Vector3(0.25f, winShow2.transform.localScale.y, blockSize - .5f);

        winShow3.transform.position = planeOrigin + new Vector3((rows - 1) * blockSize + (blockSize * .5f), winShow3.transform.position.y, (cols) * blockSize);
        winShow3.transform.localScale = new Vector3(blockSize - .5f, winShow3.transform.localScale.y, 0.25f);
        winShow4.transform.position = planeOrigin + new Vector3((rows) * blockSize, winShow4.transform.position.y, (cols - 1) * blockSize + (blockSize * .5f));
        winShow4.transform.localScale = new Vector3(0.25f, winShow4.transform.localScale.y, blockSize - .5f);
    }
    /* implementation of Recursive backtracker depth-first search maze generation */
    Dictionary<Vector4, bool> makeMap()
    {
        /* keys = x1,y1, x2,y2 list of walls, value = if wall is in place or open */
        Dictionary<Vector4, bool> wallList = new Dictionary<Vector4, bool>();
        /* keys = x1,y1 lower left corner of cell, value = if cell has yet to be processed */
        Dictionary<Vector2, bool> cellList = new Dictionary<Vector2, bool>();
        int rows = (int)(ground.transform.localScale.x) / blockSize;
        int cols = (int)(ground.transform.localScale.y) / blockSize;
        /* generate the walls and cells */
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                /* Unity doesn't have a tuple class so we'll use Vector4 */
                if (!wallList.ContainsKey(new Vector4(i, j, i + 1, j)))
                {
                    wallList.Add(new Vector4(i, j, i + 1, j), true);
                }
                if (!wallList.ContainsKey(new Vector4(i, j, i, j + 1)))
                {
                    wallList.Add(new Vector4(i, j, i, j + 1), true);
                }
                if (!wallList.ContainsKey(new Vector4(i + 1, j, i + 1, j + 1)))
                {
                    wallList.Add(new Vector4(i + 1, j, i + 1, j + 1), true);
                }
                if (!wallList.ContainsKey(new Vector4(i, j + 1, i + 1, j + 1)))
                {
                    wallList.Add(new Vector4(i, j + 1, i + 1, j + 1), true);
                }
                cellList.Add(new Vector2(i, j), true);
            }
        }
        /* remove the walls for the start and end */
        wallList[new Vector4(0, 0, 0, 1)] = false;
        wallList[new Vector4(0, 0, 1, 0)] = false;
        wallList[new Vector4(rows, cols - 1, rows, cols)] = false;
        wallList[new Vector4(rows - 1, cols, rows, cols)] = false;
        Stack<Vector2> backtrackStack = new Stack<Vector2>();

        Vector2 currentCell = new Vector2(0, 0);
        /* while there are unvisited nodes */
        while (cellList.Count > 0)
        {

            List<Vector2> validNeighborsRelative = new List<Vector2>();
            cellList[currentCell] = false;

            /* make a list of unvisited neighbors then select one and remove the wall between them 
            these coordinates are relative to the current */
            Vector2[] neighborCoordsRelative = new Vector2[] { new Vector2(-1, 0),
                                                new Vector2(1, 0),
                                                new Vector2(0, -1),
                                                new Vector2(0, 1)};
            foreach (Vector2 neighborCoordRelative in neighborCoordsRelative)
            {
                Vector2 neighborCoordAbsolute = currentCell + neighborCoordRelative;
                if (cellList.ContainsKey(neighborCoordAbsolute) &&
                        cellList[neighborCoordAbsolute])
                {
                    validNeighborsRelative.Add(neighborCoordRelative);
                }
            }
            print("valid neighbors " + validNeighborsRelative.Count);
            if (validNeighborsRelative.Count > 0)
            {
                backtrackStack.Push(currentCell);

                /* pick a random, valid, and unvisited neighbor. Remove the wall between us
                and set the currentCell as this new neighbor */
                System.Random rnd = new System.Random();
                int neighborIndex = rnd.Next(0, validNeighborsRelative.Count);
                Vector2 nextCellRelative = validNeighborsRelative[neighborIndex];

                Vector4 wallCoord = new Vector4(0,0,0,0);
                if (nextCellRelative == new Vector2(-1, 0))
                {
                    wallCoord = new Vector4(0, 0, 0, 1);
                }
                if (nextCellRelative == new Vector2(1, 0))
                {
                    wallCoord = new Vector4(1, 0, 1, 1);
                }
                if (nextCellRelative == new Vector2(0, -1))
                {
                    wallCoord = new Vector4(0, 0, 1, 0);
                }
                if (nextCellRelative == new Vector2(0, 1))
                {
                    wallCoord = new Vector4(0, 1, 1, 1);
                }
                Assert.AreNotEqual(wallCoord, new Vector4(0, 0, 0, 0));
                Vector4 wallCoordAbsolute = new Vector4(currentCell.x + wallCoord.x,
                    currentCell.y + wallCoord.y,
                    currentCell.x + wallCoord.z,
                    currentCell.y + wallCoord.w);

                wallList.Remove(wallCoordAbsolute);
                Vector2 nextCellAbsolute = currentCell + nextCellRelative;
 
                Assert.IsTrue(cellList.ContainsKey(nextCellAbsolute));
                if (!cellList.ContainsKey(nextCellAbsolute)){
                    return wallList;
                }
               
                currentCell = nextCellAbsolute;
            }
            else if (backtrackStack.Count > 0)
            {
                currentCell = backtrackStack.Pop();
            }
            else
            {
                return wallList;
            }
        }
        return wallList;
    }
}
