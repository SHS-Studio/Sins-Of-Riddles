using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerHealth : MonoBehaviour
{
    PlayerController2D controller;
    public int maxHealth = 1;
    private int currentHealth;

    public bool isDead = false;

    void Start()
    {
        controller = GetComponent<PlayerController2D>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        controller.rb.linearVelocity = Vector2.zero;
        enabled = false; // disables controller
        Debug.Log("Player Died");
        gameObject.SetActive(false);
    }
}