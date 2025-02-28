using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessObject : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public Vector2 size;

    private Vector3 originPosition;
    
    public void Awake()
    {
        originPosition = transform.position;
    }

    public void Update()
    {
        transform.position += Time.deltaTime * speed * direction;
        var difVec = originPosition - transform.position;
        if (difVec.x >= size.x || difVec.y >= size.y)
        {
            transform.position = originPosition;
        }
    }
}
