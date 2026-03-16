using UnityEngine;

public class Knife : MonoBehaviour
{
    public float maxDistance = 10f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(startPos, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Blocker")
        {
            Destroy(gameObject);
        }
    }
}