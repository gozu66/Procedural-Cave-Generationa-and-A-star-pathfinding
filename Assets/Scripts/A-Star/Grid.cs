﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;

    public LayerMask unWalkableMask;
    public Vector2 worldSize;

    Node[,] grid;

    public float nodeRadius;
    float nodeDiameter;

    int gridSizeX, gridSizeY;

    Unit[] agents;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;

        gridSizeX = Mathf.RoundToInt(worldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(worldSize.y / nodeDiameter);

        agents = FindObjectsOfType<Unit>();
    }
    
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.A))
            CreateGrid();*/
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSizeX / 2 - transform.position - Vector3.forward * gridSizeY / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPosition = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPosition, nodeRadius, unWalkableMask));
                grid[x, y] = new Node(walkable, worldPosition, x, y);
            }
        }

        for(int i = 0; i < agents.Length; i++)
        {
            agents[i].enabled = true;
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neightbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neightbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neightbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //float percentX = (worldPosition.x + worldSize.x / 2) / worldSize.x;     //Unoptimized
        //float percentY = (worldPosition.z + worldSize.y / 2) / worldSize.y;     //Unoptimized
        
        float percentX = worldPosition.x / worldSize.x + 0.5f;      //Optimized
        float percentY = worldPosition.z / worldSize.y + 0.5f;      //Optimized
        
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(worldSize.x, 1, worldSize.y));

        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}