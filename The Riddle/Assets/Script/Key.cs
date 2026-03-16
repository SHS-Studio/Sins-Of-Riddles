using UnityEngine;

public class Key : MonoBehaviour
{
  public   PlayerHealth hp;
    public static int keyCount = 0;     // Total keys collected
    public int keysRequired = 3;        // Number needed to disable shield
    public GameObject shield;           // Assign shield in Inspector

    public void Update()
    {
        if (hp.isDead)
        {
            keyCount = 0;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            keyCount++;

            Debug.Log("Keys Collected: " + keyCount);

            if (keyCount >= keysRequired)
            {
                if (shield != null)
                {
                    shield.SetActive(false);
                }
            }

            Destroy(gameObject); // remove key
        }
    }
}