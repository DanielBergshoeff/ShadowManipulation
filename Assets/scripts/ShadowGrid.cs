using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowGrid : MonoBehaviour
{
    public static ShadowGrid Instance;

    public LayerMask WallMask;//This is the mask that the program will look for when trying to find obstructions to the path.
    public Vector3 vGridWorldSize;//A vector2 to store the width, height and depth of the graph in world units.
    public float fNodeRadius;//This stores how big each square on the graph will be
    public float fDistanceBetweenNodes;//The distance that the squares will spawn from eachother.

    public Node[,,] NodeArray;//The array of nodes that the A Star algorithm uses.

    public float fNodeDiameter;//Twice the amount of the radius (Set in the start function)
    int iGridSizeX, iGridSizeY, iGridSizeZ;//Size of the Grid in Array units.

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        fNodeDiameter = fNodeRadius * 2;//Double the radius to get diameter
        iGridSizeX = Mathf.RoundToInt(vGridWorldSize.x / fNodeDiameter);//Divide the grids world co-ordinates by the diameter to get the size of the graph in array units.
        iGridSizeY = Mathf.RoundToInt(vGridWorldSize.y / fNodeDiameter);//Divide the grids world co-ordinates by the diameter to get the size of the graph in array units.
        iGridSizeZ = Mathf.RoundToInt(vGridWorldSize.z / fNodeDiameter);//Divide the grids world co-ordinates by the diameter to get the size of the graph in array units.
        CreateGrid();//Draw the grid
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateGrid() {
        NodeArray = new Node[iGridSizeX, iGridSizeY, iGridSizeZ];//Declare the array of nodes.
        Vector3 bottomLeft = transform.position - Vector3.right * vGridWorldSize.x / 2 - Vector3.forward * vGridWorldSize.z / 2 - Vector3.up * vGridWorldSize.y / 2;//Get the real world position of the bottom left of the grid.
        for (int x = 0; x < iGridSizeX; x++)//Loop through the array of nodes.
        {
            for (int y = 0; y < iGridSizeY; y++)//Loop through the array of nodes
            {
                for (int z = 0; z < iGridSizeZ; z++) {
                    Vector3 worldPoint = bottomLeft + Vector3.right * (x * fNodeDiameter + fNodeRadius) + Vector3.forward * (z * fNodeDiameter + fNodeRadius) + Vector3.up * (y * fNodeDiameter + fNodeRadius);//Get the world co ordinates of the bottom left of the graph
                    bool Wall = true;//Make the node a wall


                    //If the node is not being obstructed
                    //Quick collision check against the current node and anything in the world at its position. If it is colliding with an object with a WallMask,
                    //The if statement will return false.
                    if (Physics.CheckSphere(worldPoint, fNodeRadius, WallMask)) {
                        Wall = false;//Object is not a wall
                    }

                    NodeArray[x, y, z] = new Node(Wall, worldPoint, x, y, z);//Create a new node in the array.
                }
            }
        }
    }

    //Gets the closest node to the given world position.
    public Node NodeFromWorldPoint(Vector3 a_vWorldPos) {
        float ixPos = ((a_vWorldPos.x - this.transform.position.x + vGridWorldSize.x / 2) / vGridWorldSize.x);
        float iyPos = ((a_vWorldPos.y - this.transform.position.y + vGridWorldSize.y / 2) / vGridWorldSize.y);
        float izPos = ((a_vWorldPos.z - this.transform.position.z + vGridWorldSize.z / 2) / vGridWorldSize.z);

        ixPos = Mathf.Clamp01(ixPos);
        iyPos = Mathf.Clamp01(iyPos);
        izPos = Mathf.Clamp01(izPos);

        int ix = Mathf.RoundToInt((iGridSizeX - 1) * ixPos);
        int iy = Mathf.RoundToInt((iGridSizeY - 1) * iyPos);
        int iz = Mathf.RoundToInt((iGridSizeZ - 1) * izPos);

        return NodeArray[ix, iy, iz];
    }

    //Function that draws the wireframe
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(vGridWorldSize.x, vGridWorldSize.y, vGridWorldSize.z));//Draw a wire cube with the given dimensions from the Unity inspector

        if(NodeArray != null) {
            foreach(Node n in NodeArray) {
                if (n.Shadow) {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.vPosition, fNodeDiameter * Vector3.one);
                }
            }
        }
    }
}
