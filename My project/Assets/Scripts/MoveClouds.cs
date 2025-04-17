using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveClouds : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed = 0.5f;
    public List<Transform> cloudParts; // 拖进去几张不同风格的云图（或同一张重复用）
    private float partWidth;

    void Start()
    {
        if (cloudParts.Count == 0) return;

        // 预设假定每张图宽度一样
        SpriteRenderer sr = cloudParts[0].GetComponent<SpriteRenderer>();
        partWidth = sr.bounds.size.x;

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform cloud in cloudParts)
        {
            cloud.Translate(Vector2.right * speed * Time.deltaTime);

            float camRight = Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect;
            if (cloud.position.x - partWidth / 2f > camRight)
            {
                // 找到最左边的云图，接在它后面
                Transform leftMost = cloudParts.OrderBy(c => c.position.x).First();
                cloud.position = new Vector3(leftMost.position.x - partWidth, cloud.position.y, cloud.position.z);
            }
        }
    }
}
