using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float bounceForce = 10f;
    private bool isBouncing = false;
    public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // ��ʼ��Ϊ Idle ״̬
        if (animator != null)
        {
            animator.SetBool("isIdle", true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerBounce(collision.gameObject);
        }
    }
    
    private void HandlePlayerBounce(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
    
        if (rb)
        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }

        if (animator != null)
        {
            animator.SetBool("isIdle", false); // ���� Press ״̬
            isBouncing = true;
            StartCoroutine(ResetToIdle());
        }
    }

    private IEnumerator ResetToIdle()
    {
        yield return new WaitForSeconds(0.2f); // ��������ʱ��
        if (animator != null)
        {
            animator.SetBool("isIdle", true);
        }

        isBouncing = false;
    }

}
