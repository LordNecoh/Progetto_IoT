using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    private float speed = 5f;
    private float terrainLength = 173f;

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        if (transform.position.z <= -terrainLength)
        {
            transform.position += new Vector3(0, 0, terrainLength * 2);
        }
    }
}