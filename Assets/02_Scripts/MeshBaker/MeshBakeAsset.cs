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
        //1.하위 오브젝트의 모든 MeshFilter 컴포넌트 수집 (병합 대상 찾기)
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        //2.대표 머테리얼(재질) 설정
        foreach (var filter in meshFilters)
        {
            if (filter.gameObject != gameObject)
            {
                GetComponent<MeshRenderer>().sharedMaterial = filter.GetComponent<MeshRenderer>().sharedMaterial;
                break;
            }
        }
        // 3.CombineInstance 배열 생성: 메쉬들을 하나로 합칠 때 필요한 '설계도' 리스트생성
        CombineInstance[] instances = new CombineInstance[meshFilters.Length];

        // 4. 좌표 변환용 행렬 계산: 자식들의 '월드 좌표'를 부모의 '로컬 좌표'로 변환하기 위함
        Matrix4x4 myTransform = transform.worldToLocalMatrix;

        //실제로 합칠 수 있는 메쉬만 세기 위한 카운트
        int actualCount = 0;

        for (int i = 0; i < meshFilters.Length; i++)
        {
            //매쉬 데이터가 없거나 빈 오브젝트는 건너뛰기
            if (meshFilters[i].gameObject == gameObject || meshFilters[i].sharedMesh == null) continue;

            var meshFilter = meshFilters[i];

            //머테리얼 설정이 안됬을경우 2차방어
            if (actualCount == 0)
            {
                GetComponent<MeshRenderer>().sharedMaterial = meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial;
            }

            //5. 개별 메쉬 정보를 설계도(CombineInstance)에 변환 행렬 기록
            instances[actualCount] = new CombineInstance
            {
                mesh = meshFilter.sharedMesh,
                transform = myTransform * meshFilter.transform.localToWorldMatrix,
            };
            
            //자식 오브젝트 비활성화
            meshFilter.gameObject.SetActive(false);

            actualCount++;
        }

        if (actualCount == 0) return;

        //6. 배열 압축 (빈 구멍 없는 최종 설계도 생성)
        //넉넉하게 잡았던 배열에서 실제 데이터가 들어간 개수만큼만 딱 잘라서 복사
        CombineInstance[] finalInstances = new CombineInstance[actualCount];
        System.Array.Copy(instances, finalInstances, actualCount);

        //새로운 메쉬 생성 및 최적화 설정
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
