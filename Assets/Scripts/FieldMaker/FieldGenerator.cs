using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class FieldGenerator : MonoBehaviour
{
    [Header("シード値")]
    [SerializeField, Tooltip("シード値X")]
    private float _seedX;

    [SerializeField, Tooltip("シード値Z")] private float _seedZ;

    [Header("マップサイズ")]
    [SerializeField, Tooltip("幅")]
    private float _width = 50f;

    [SerializeField, Tooltip("深さ")] private float _depth = 50f;
    [SerializeField, Tooltip("高さの最大値")] private float _maxHeight = 10f;
    [SerializeField, Tooltip("起伏の激しさ")] private float _undulation = 15f;
    [SerializeField, Tooltip("マップの大きさ")] private float _mapSize = 1f;


    [SerializeField, Tooltip("Field用Box")]
    private GameObject _fieldBox;

    [SerializeField, Tooltip("Field用Tree")]
    private GameObject _fieldTree;

    [SerializeField, Tooltip("Field用Stone")]
    private GameObject _fieldStone;

    [SerializeField] private Material[] _materialArray;

    private GameObject _gameObjectContainer;
    private FieldBox[,] _fieldArray;

    private void Awake()
    {
        _maxHeight = Random.Range(5, 20);

        _gameObjectContainer = GameObject.Find("GameObjectContainer");
        _fieldArray = new FieldBox[(int)_width, (int)_depth];

        transform.localScale = new Vector3(_mapSize, _mapSize, _mapSize);

        _seedX = Random.value * 100f;
        _seedZ = Random.value * 100f;

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _depth; z++)
            {
                GameObject cube = Instantiate(_fieldBox);
                cube.transform.localPosition = new Vector3(x, 0, z);
                cube.transform.SetParent(transform);
                _fieldArray[x, z] = cube.GetComponent<FieldBox>();

                //高さ設定
                SetYPosition(cube);
            }
        }

        //PutObject();
    }

    private void SetYPosition(GameObject cube)
    {
        float y = 0;

        float xSample = (cube.transform.localPosition.x + _seedX) / _undulation;
        float zSample = (cube.transform.localPosition.z + _seedZ) / _undulation;

        var perlin = new MathfPerlin();

        float noise = perlin.Noise(xSample, zSample);

        y = _maxHeight * noise;

        y = Mathf.Round(y);

        cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);

        ChangeCubeColor(cube, y);
    }

    void ChangeCubeColor(GameObject cube, float y) //高さによって色を段階的に変更
    {
        if (y > _maxHeight * 0.2f) //草
        {
            cube.GetComponent<MeshRenderer>().material = _materialArray[0];
        }
        else if (y > _maxHeight * 0f) // 土
        {
            cube.GetComponent<MeshRenderer>().material = _materialArray[1];
        }
        else if (y > _maxHeight * -0.2f) // 石
        {
            cube.GetComponent<MeshRenderer>().material = _materialArray[2];
        }
        else
        {
            cube.GetComponent<MeshRenderer>().material = _materialArray[3];
        }
    }

    public void PutObject()
    {
        for (int x = 0; x < _width; x += 10)
        {
            for (int z = 0; z < _depth; z += 10)
            {
                FieldBox go = _fieldArray[x, z];

                if (PerimeterCheck(x, z))
                {
                    var y = go.transform.position.y;

                    if (y > _maxHeight * 0.3f && y < _maxHeight * 0.5f)
                    {
                        Instantiate(_fieldTree, go._objectCreationPosition.position, Quaternion.identity).transform.SetParent(_gameObjectContainer.transform);
                        go._isPut = true;
                    }
                    else if (y > _maxHeight * 0f && y < _maxHeight * 0.2f)
                    {
                        Instantiate(_fieldStone, go._objectCreationPosition.position, Quaternion.identity).transform.SetParent(_gameObjectContainer.transform);
                        go._isPut = true;
                    }
                }
            }
        }
    }

    bool PerimeterCheck(int x, int z)
    {
        if (x + 1 < _width && x - 1 >= 0 && z + 1 < _depth && z - 1 >= 0)
        {
            if (!_fieldArray[x + 1, z]._isPut && !_fieldArray[x - 1, z]._isPut &&
                !_fieldArray[x, z + 1]._isPut && !_fieldArray[x, z - 1]._isPut &&
                !_fieldArray[x + 1, z + 1]._isPut && !_fieldArray[x - 1, z - 1]._isPut &&
                !_fieldArray[x + 1, z - 1]._isPut && !_fieldArray[x - 1, z + 1]._isPut)
            {
                _fieldArray[x + 1, z]._isPut = true;
                _fieldArray[x - 1, z]._isPut = true;
                _fieldArray[x, z + 1]._isPut = true;
                _fieldArray[x, z - 1]._isPut = true;
                _fieldArray[x + 1, z + 1]._isPut = true;
                _fieldArray[x - 1, z - 1]._isPut = true;
                _fieldArray[x + 1, z - 1]._isPut = true;
                _fieldArray[x - 1, z + 1]._isPut = true;

                return true;
            }

            return false;
        }

        return false;
    }
}