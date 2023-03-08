using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public Transform _fieldGenerator;

    [SerializeField] Material _combinedMat;
    public void OnCombine()
    {
        CombineMesh();
    }

    void CombineMesh()
    {
        MeshFilter parentMeshFilter = CheckParentComponent<MeshFilter>(_fieldGenerator.gameObject);
        MeshRenderer parentMeshRenderer = CheckParentComponent<MeshRenderer>(_fieldGenerator.gameObject);

        MeshFilter[] meshFilters = _fieldGenerator.GetComponentsInChildren<MeshFilter>();
        List<MeshFilter> meshFilterList = new List<MeshFilter>();
        for (int i = 1; i < meshFilters.Length; i++)
        {
            meshFilterList.Add(meshFilters[i]);
        }

        CombineInstance[] combine = new CombineInstance[meshFilterList.Count];

        for (int i = 0; i < meshFilterList.Count; i++)
        {
            combine[i].mesh = meshFilterList[i].sharedMesh;
            combine[i].transform = meshFilterList[i].transform.localToWorldMatrix;
            meshFilterList[i].gameObject.SetActive(false);
        }

        parentMeshFilter.mesh = new Mesh();
        parentMeshFilter.mesh.CombineMeshes(combine);

        parentMeshRenderer.material = _combinedMat;

        MeshCollider meshCol = CheckParentComponent<MeshCollider>(_fieldGenerator.gameObject);
        meshCol.sharedMesh = parentMeshFilter.mesh;

        _fieldGenerator.gameObject.SetActive(true);
    }

    T CheckParentComponent<T>(GameObject go) where T : Component
    {
        var targetComp = go.GetComponent<T>();
        if (targetComp == null)
        {
            targetComp = go.AddComponent<T>();
        }
        return targetComp;
    }
}