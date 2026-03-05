using System.IO;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

//MeshFilter, MesgRenderer 속성을 스크립트를 넣으면 적용되게 하기위한 코드
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshBakeAsset : MonoBehaviour
{
    public string meshSavePath = "Assets/BakedMeshes/";
    public string meshName = "section";

    //mesh란 무엇인가?
    // ㄴ물체의 형태를 정의하는 데이터 집합으로 쉽게 말해 3D모델의 뼈대와 피부
   
    public void BakeLevel()
    {
        CombineMesh();

        StartCoroutine(WaitGridBake());

    }

    IEnumerator WaitGridBake()
    {
        yield return new WaitForSeconds(0.1f);

        GridManager grid = FindAnyObjectByType<GridManager>();
        if (grid != null)
        {
            grid.GridData();
            Debug.Log($"{gameObject.name} 베이크후 그리드 갱신 완료");
        }
    }

   
    public void CombineMesh()
    {        
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
       
        foreach (var filter in meshFilters)
        {
            if (filter.gameObject != gameObject)
            {
                GetComponent<MeshRenderer>().sharedMaterial = filter.GetComponent<MeshRenderer>().sharedMaterial;
                break;
            }
        }
       
        CombineInstance[] instances = new CombineInstance[meshFilters.Length];

        Matrix4x4 myTransform = transform.worldToLocalMatrix;

        int actualCount = 0;
        
        for (int i = 0; i < meshFilters.Length; i++)
        {
            if (meshFilters[i].gameObject == gameObject || meshFilters[i].sharedMesh == null) continue;

            var meshFilter = meshFilters[i];

            if (actualCount == 0)
            {
                GetComponent<MeshRenderer>().sharedMaterial = meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial;
            }
                        
            instances[actualCount] = new CombineInstance
            {                
                mesh = meshFilter.sharedMesh,                
                transform = myTransform * meshFilter.transform.localToWorldMatrix,
            };

            meshFilter.gameObject.SetActive(false);

            actualCount++;
        }

        if (actualCount == 0) return;

        CombineInstance[] finalInstances = new CombineInstance[actualCount];
        System.Array.Copy(instances, finalInstances, actualCount);

        Mesh combinedMesh = new Mesh();
        
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        
        combinedMesh.CombineMeshes(finalInstances);
        
        gameObject.GetComponent<MeshFilter>().sharedMesh = combinedMesh;
       
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = combinedMesh;

        SaveMeshAsset(combinedMesh);

        gameObject.SetActive(true);
    }
    public void SaveMeshAsset(Mesh mesh)
    {
#if UNITY_EDITOR
        if (!Directory.Exists(meshSavePath)) Directory.CreateDirectory(meshSavePath);

        string path = AssetDatabase.GenerateUniqueAssetPath(meshSavePath + meshName + ".asset");
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
#endif
    }
}
