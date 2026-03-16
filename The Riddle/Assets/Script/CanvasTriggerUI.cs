using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasTriggerUI : MonoBehaviour
{
    [Header("Canvas UI")]
    public GameObject canvasUI;
    public Text SolvingTime;
    public GameObject gameoverUI;

    [Header("Timer")]
    public float timer = 10f;
    private bool timerRunning = false;

    [Header("Sprite Spawning")]
    public Transform[] spawnPoints;   // 3 spawn points
    public GameObject[] sprites;          // 3 sprites

    private bool playerInside = false;
    private bool spritesSpawned = false;

    void Start()
    {
        if (canvasUI != null)
            canvasUI.SetActive(false);
    }

    void Update()
    {
        // Close canvas when SPACE pressed
        if (playerInside && Input.GetKeyDown(KeyCode.Space))
        {
            if (canvasUI != null)
                canvasUI.SetActive(false);
                SolvingTime.gameObject.SetActive(true);
                timerRunning = true;
        }

        // Timer logic
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            SolvingTime.text = timer.ToString("00");

            if (timer <= 0)
            {
                timerRunning = false;
                SolvingTime.gameObject.SetActive(false);
                gameoverUI.SetActive(true);
                Debug.Log("Timer Finished");

            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (canvasUI != null)
            {
                canvasUI.SetActive(true);
                this.GetComponent<BoxCollider2D>().enabled = false;
            }

            if (!spritesSpawned)
            {
                SpawnSprites();
                spritesSpawned = true;
            }
        }
    }

    void SpawnSprites()
    {
        int count = Mathf.Min(spawnPoints.Length, sprites.Length);

        for (int i = 0; i < count; i++)
        {
            Instantiate(sprites[i], spawnPoints[i].position, Quaternion.identity);
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}