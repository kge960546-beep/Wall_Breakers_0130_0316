using UnityEngine;

public class TestPlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    Rigidbody rb;
    public GameObject player;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {

    }


    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);

        player.transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
