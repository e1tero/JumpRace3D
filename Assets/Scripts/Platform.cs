using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Platform : MonoBehaviour
{
    [Header("Moving Settings")]
    [SerializeField] float distance;
    [SerializeField] float speedMove;
    
    [Header("Effects Settings")]
    [SerializeField] private GameObject destructionEffect;
    
    public int destruction;
    private Vector3 startPosition;
    private Vector3 velocity;

    void Start()
    {
        startPosition = transform.position;
        velocity = Vector3.right * speedMove;
    }

    void Update()
    {
        if (transform.position.x > startPosition.x + distance)
            velocity.x = -speedMove;
        else if (transform.position.x < startPosition.x - distance)
            velocity.x = speedMove;
 
        transform.position += velocity * Time.deltaTime;

        if (destruction == 3)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
            this.gameObject.SetActive(false);
        }
    }
}
