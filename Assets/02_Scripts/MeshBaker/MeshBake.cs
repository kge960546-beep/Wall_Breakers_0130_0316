using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//MeshFilter, MesgRenderer 속성을 스크립트를 넣으면 적용되게 하기위한 코드
[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshBake : MonoBehaviour
{
    //======
    //공부중
    //======
    //mesh란 무엇인가?
    // ㄴ물체의 형태를 정의하는 데이터 집합으로 쉽게 말해 3D모델의 뼈대와 피부
    void Start()
    {
        //현재 게임오브젝트의 자식들까지 전부 훑어서 MeshFilter 컴포넌트를 배열로 가져옴
        //MeshFilter: 메시의 "데이터(정점, 삼각형 등)"를 담는 바구니

        //MEshFilter 컴포넌트는 원래 가지고있는건지 직접 넣어야하는건지?
        // ㄴ RequireComponent를 사용하여 스크립트를 넣으면 넣어지게 구성
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter> ();

        //머티리얼은 1개만 적용할 수 있어서 첫번째 자식의 머티리얼을 부모의 머티리얼로 복사하기
        foreach (var filter in meshFilters) 
        {
            if(filter.gameObject != gameObject)
            {
                GetComponent<MeshRenderer>().sharedMaterial = filter.GetComponent<MeshRenderer>().sharedMaterial;
                break;
            }
        }

        //CombineInstance[]: Mesh들을 합칠때 요구하는 입력 구조로 다른 인스턴스와 병합하여 결합된 메시를 만들 단일 메시를 설명하는 구조체
        //mesh: 실제 데이터
        //transform (mesh가 어디에 어떤 크기와 회전으로 있었는지)
        CombineInstance[] instances = new CombineInstance[meshFilters.Length];

        Matrix4x4 myTransform = transform.worldToLocalMatrix;
        

        //각 MeshFilter를 CombineInstance로 변환하고 자식 비활성화(원본이라 생각함)
        for(int i = 0; i < meshFilters.Length; i++)
        {
            if (meshFilters[i] == GetComponent<MeshFilter>()) continue;

            var meshFilter = meshFilters[i];
            instances[i] = new CombineInstance
            {
                //해당 오브젝트가 참조 중인 Mesg를 가져오는 함수
                mesh = meshFilter.sharedMesh,

                //각 메시가 월드 공간에서 어디에 있었는지를 4x4행렬로 저장하기 때문에
                //(왜죠?)
                //ㄴ 각자 흩어져있는 아이들을 하나의 새로운 좌표로 통일시키기 위해서 이동, 회전, 크기 3가지 변화를 하나의 수식으로 계산하기 위해
                //   수학적으로 약속된 형태가 4x4 행렬이다.
                //모든 메시를 같은 좌표계로 합쳐야해서 행렬함수가 필요하다
                //메시 자체를 이동시키는게 아닌 버텍스에 행렬을 곱해서 합쳐진 메시 안에서 같은 위치에 있도록 굽는 방식
                transform = myTransform * meshFilter.transform.localToWorldMatrix,
            }; 
            
            meshFilter.gameObject.SetActive (false);
        }

        //합쳐질 새 메시 생성
        Mesh combinedMesh = new Mesh ();

        //Mesh는 약 65,000개의 정점까지만 가질 수 있어서 한계를 늘리는 설정
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        //매시 합치기
        combinedMesh.CombineMeshes (instances);

        //부모 MeshFilter에 결과물 넣기
        gameObject.GetComponent<MeshFilter>().sharedMesh = combinedMesh;

        /*
         추가하고 다시 넣은 이유
        콜라이더를 미리 붙여두지 않을때가 많아서 좀더 확실하고 안전하게 콜라이더 추가를 하기위해 !=가 아닌
        ==를 사용해서 추가한다음 새로고침 하는 방식으로 구현함
         */
        //콜라이더 없으면 추가하기
        MeshCollider meshCollider = GetComponent<MeshCollider> ();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        //콜라이더를 비우고 다시 넣기
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = combinedMesh;

        GridManager grid = FindAnyObjectByType<GridManager> ();
        if (grid != null) 
        {
            grid.GridData();
        }
        gameObject.SetActive (true);
    }
}
