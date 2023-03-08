using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGenerateTerrain : MonoBehaviour
{
    public float[,] _terrainHeightData;
    public int _rowsAndColumns = 0;
    public float _refInement = 0f;
    public float _multipiler = 0f;
    private float _perlinNoise = 0f;
    private Terrain _terrain;

    private void Start()
    {
        _terrainHeightData = new float[_rowsAndColumns, _rowsAndColumns];
        
        _terrain = GetComponent<Terrain>();

        for (int r = 0; r < _rowsAndColumns; r++)
        {
            for (int c = 0; c < _rowsAndColumns; c++)
            {
                _perlinNoise = Mathf.PerlinNoise(r * _refInement, c * _refInement);
                _terrainHeightData[r, c] = _perlinNoise * _multipiler;
            }
        }

        _terrain.terrainData.SetHeights(0, 0, _terrainHeightData);
        
    }
}