using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
public enum MoveDirection
{
    Right,
    Left
}
public class UniversalObstacle : MonoBehaviour
{
    public PlayerController2D controller;
    public enum ObstacleType
    {
        Laser,
        Gas,
        SpinningCutter,
        Spike
    }

    [Header("Select Obstacle Type")]
    public ObstacleType obstacleType;

    [Header("Common Settings")]
    public int damage = 1;

    [Header("Laser Settings")]
    public float laserOnTime = 2f;
    public float laserOffTime = 2f;

    [Header("Gas Settings")]
    public float gasDamageInterval = 1f;

    [Header("Spinning Cutter Settings")]
    public float rotationSpeed = 300f;

    [Header("Spinning Cutter Movement")]
    public MoveDirection moveDirection = MoveDirection.Right;
    public bool enableMovement = true;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    private Vector3 startPosition;

    private Collider2D col;
    private SpriteRenderer sr;

    public SpriteRenderer[] ray;
    private bool laserActive = true;
    private float gasTimer;

    void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (obstacleType == ObstacleType.SpinningCutter)
        {
            startPosition = transform.position;
        }

        switch (obstacleType)
        {
            case ObstacleType.Laser:
                StartCoroutine(LaserCycle());
                break;

            case ObstacleType.SpinningCutter:
                break;
        }
    }

    void Update()
    {
        if (obstacleType == ObstacleType.SpinningCutter)
        {
            // Rotate
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            // Move Left ↔ Right
            if (enableMovement)
            {
                float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance);

                if (moveDirection == MoveDirection.Right)
                    transform.position = startPosition + new Vector3(offset, 0, 0);
                else
                    transform.position = startPosition + new Vector3(-offset, 0, 0);
            }
        }
    }

    // ---------------- LASER LOGIC ----------------
    IEnumerator LaserCycle()
    {
        while (true)
        {
            laserActive = true;
            if (col) col.enabled = true;
            if (sr) sr.enabled = true;
            for (int i = 0; i < ray.Length; i++)
            {
                ray[i].enabled = true;
            }
                

            yield return new WaitForSeconds(laserOnTime);

            laserActive = false;
            if (col) col.enabled = false;
            if (sr) sr.enabled = false;
            for (int i = 0; i < ray.Length; i++)
            {
                ray[i].enabled = false;
            }
               

            yield return new WaitForSeconds(laserOffTime);
        }
    }

    // ---------------- TRIGGER DAMAGE ----------------
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player == null) return;

        switch (obstacleType)
        {
            case ObstacleType.Laser:
                if (laserActive && !controller.Islasermode)
                    player.TakeDamage(damage);
                break;

            case ObstacleType.Gas:
                gasTimer = gasDamageInterval;
                player.TakeDamage(damage);
                break;

            case ObstacleType.Spike:
                player.Die();
                break;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (obstacleType != ObstacleType.Gas) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player == null) return;

        gasTimer -= Time.deltaTime;

        if (gasTimer <= 0f)
        {
            player.TakeDamage(damage);
            gasTimer = gasDamageInterval;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (obstacleType != ObstacleType.SpinningCutter) return;

        PlayerHealth player = collision.collider.GetComponent<PlayerHealth>();
        if (player == null) return;

        player.TakeDamage(damage);
    }
}