using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveClouds : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed = 0.5f;
    private float resetX;

    void Start()
    {
        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;

        resetX = Camera.main.transform.position.x + camWidth / 2f + 2f; // 右边多加一点边距
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        if (transform.position.x > resetX)
        {
            // 例如从左边 -resetX 重来
            transform.position = new Vector3(-resetX, transform.position.y, transform.position.z);
        }
    }
}
