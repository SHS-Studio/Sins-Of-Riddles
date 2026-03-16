using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController2D : MonoBehaviour
{
    [Header("Ability UI Bars")]
    public Slider gravityBar;
    public Slider invisibilityBar;
    public Slider phaseBar;

    PlayerHealth m_pHP;
    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Knife Teleport")]
    public GameObject knifePrefab;
    public Transform knifeSpawnPoint;
    public float knifeSpeed = 15f;
    public float Destroytime;
    private GameObject currentKnife;

    [Header("Ability Duration")]
    public float gravityDuration = 5f;
    public float transparentDuration = 5f;
    public float phaseDuration = 5f;

    [Header("Fall Damage")]
    public float maxFallDistance = 8f;

    public Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;

    private float normalGravity;
    private Color normalColor;

    private bool abilityActive = false;
    public bool Islasermode;
    public bool Isgravitymode;
    private Coroutine currentAbilityRoutine;

    private bool isGrounded = true;
    private float fallStartHeight;
    private bool wasGroundedLastFrame = true;
    private bool hasInitializedGround = false;


    Coroutine abilityBarRoutine;
    void Start()
    {
        m_pHP = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        normalGravity = rb.gravityScale;
        normalColor = sr.color;
    }

    void Update()
    {
        Move();
        HandleTeleport();
        HandleAbilities();
        CheckGroundStatus();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetToNormal();
        }
    }

    // ---------------- MOVEMENT ----------------
    void Move()
    {
        float move = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            move = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            move = 1f;
        }

        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), transform.localScale.y, 1);
        }
    }



    // ---------------- TELEPORT ----------------
    void HandleTeleport()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentKnife == null)
                ThrowKnife();
            else
                TeleportToKnife();
        }
    }

    void ThrowKnife()
    {
        currentKnife = Instantiate(knifePrefab, knifeSpawnPoint.position, Quaternion.Euler(0, 0, 90));

        Rigidbody2D knifeRb = currentKnife.GetComponent<Rigidbody2D>();

        float direction = transform.localScale.x; // player facing direction

        // Adjust throw direction depending on gravity
        Vector2 throwDirection;

        if (rb.gravityScale > 0)
        {
            // Normal gravity
            throwDirection = new Vector2(direction, 0);
        }
        else
        {
            // Reversed gravity
            throwDirection = new Vector2(direction, 0);
        }

        knifeRb.AddForce(throwDirection * knifeSpeed, ForceMode2D.Impulse);

        Destroy(currentKnife, Destroytime);
    }

    void TeleportToKnife()
    {
        transform.position = currentKnife.transform.position;
        Destroy(currentKnife);
    }

    // ---------------- ABILITIES ----------------
    void SwitchAbility(IEnumerator newAbility)
    {
        // If any ability running → reset first
        if (abilityActive)
        {
            ResetToNormal();
        }

        currentAbilityRoutine = StartCoroutine(newAbility);
    }
    void HandleAbilities()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            SwitchAbility(GravityAbility());

        if (Input.GetKeyDown(KeyCode.RightArrow))
            SwitchAbility(TransparentAbility());

        if (Input.GetKeyDown(KeyCode.DownArrow))
            SwitchAbility(PhaseAbility());
    }

    IEnumerator GravityAbility()
    {
        abilityActive = true;
        Isgravitymode = true;
        rb.gravityScale *= -1;
        sr.flipY = true;

        if (abilityBarRoutine != null)
            StopCoroutine(abilityBarRoutine);

        abilityBarRoutine = StartCoroutine(AbilityTimer(gravityBar, gravityDuration));

        yield return new WaitForSeconds(gravityDuration);


        ResetToNormal();
    }

    IEnumerator TransparentAbility()
    {
        abilityActive = true;
        Islasermode = true;
        sr.color = new Color(normalColor.r, normalColor.g, normalColor.b, 0.3f);
        if (abilityBarRoutine != null)
            StopCoroutine(abilityBarRoutine);

        abilityBarRoutine = StartCoroutine(AbilityTimer(invisibilityBar, transparentDuration));

        yield return new WaitForSeconds(transparentDuration);


        ResetToNormal();
    }

    IEnumerator PhaseAbility()
    {
        abilityActive = true;

        col.isTrigger = true;

        sr.color = Color.white;
        if (abilityBarRoutine != null)
            StopCoroutine(abilityBarRoutine);

        abilityBarRoutine = StartCoroutine(AbilityTimer(phaseBar, phaseDuration));

        yield return new WaitForSeconds(phaseDuration);


        ResetToNormal();
    }

    IEnumerator AbilityTimer(Slider bar, float duration)
    {
        float timer = duration;

        bar.gameObject.SetActive(true);
        bar.value = 1f;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            bar.value = timer / duration;
            yield return null;
        }

        bar.value = 0;
        bar.gameObject.SetActive(false);
    }

    // ---------------- FALL DAMAGE SYSTEM ----------------
    void CheckGroundStatus()
    {
        isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

        // FIRST FRAME INITIALIZATION
        if (!hasInitializedGround)
        {
            wasGroundedLastFrame = isGrounded;
            fallStartHeight = transform.position.y;
            hasInitializedGround = true;
            return;
        }

        // Player just left ground
        if (!isGrounded && wasGroundedLastFrame)
        {
            fallStartHeight = transform.position.y;
        }

        // Player just landed
        if (isGrounded && !wasGroundedLastFrame)
        {
            float fallDistance = Mathf.Abs(fallStartHeight - transform.position.y);

            if (!Isgravitymode && fallDistance > maxFallDistance)
            {
                m_pHP.Die();
                Debug.Log("Player Died from Fall!");
            }
        }

        wasGroundedLastFrame = isGrounded;
    }


    // ---------------- RESET ----------------
    void ResetToNormal()
    {
        if (currentAbilityRoutine != null)
            StopCoroutine(currentAbilityRoutine);

        if (abilityBarRoutine != null)
            StopCoroutine(abilityBarRoutine);

        rb.gravityScale = normalGravity;
        sr.flipY = false;
        sr.color = normalColor;
        col.isTrigger = false;

        abilityActive = false;
        Islasermode = false;
        Isgravitymode = false;

        gravityBar.value = 0;
        invisibilityBar.value = 0;
        phaseBar.value = 0;

        gravityBar.gameObject.SetActive(false);
        invisibilityBar.gameObject.SetActive(false);
        phaseBar.gameObject.SetActive(false);
    }


    // ---------------- Out Of Camera  ----------------

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (col.gameObject.tag == "BORDER")
        {
            m_pHP.Die();
        }
    }

    public void OnCollisionEnter2D(Collision2D coll)
    {
        if (col.gameObject.tag == "BORDER")
        {
            m_pHP.Die();
        }
    }
}