﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMover : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed;
    public int damage=1;
    public float lifeTime = 3.0f;
    float birthTime;
    public bool isDestroyed = true;

    Rigidbody rb;

    void Start()
    {
        birthTime = Time.time;
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.up * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(lifeTime > 0 && Time.time - birthTime > lifeTime)
        {
            Disapear();
        }
    }

    void Disapear()
    {
        Destroy(gameObject);
    }
}
