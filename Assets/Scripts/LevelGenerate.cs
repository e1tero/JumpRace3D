using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelGenerate : MonoBehaviour
{
    private Vector3 pole;
    
    [Header("Spiral setup")] 
    [SerializeField] private float _a = 60;
    [SerializeField] private float phi;
    
    [SerializeField] private int numOfPlatforms;
    
    public GameObject platform;
    public List<GameObject> platforms;
    private Vector3 point;
    private LineRenderer _lineRenderer;
    
    private TextMeshPro text;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.numPositions = numOfPlatforms;
    }

    void Start()
    {
        for (int i = 0; i < numOfPlatforms; i++)
        {
            point.x = (int) ((_a / (Math.PI * 2)) * phi * Math.Cos(phi));
            point.z = (int) ((_a / (Math.PI * 2)) * phi * Math.Sin(phi));
            point.y -= 30;
            phi += 0.5f;
            _lineRenderer.SetPosition(i,point);

            if (i > 5)
            {
                platforms.Add(Instantiate(platform, point, Quaternion.identity));
                platforms[i-6].transform.Find("Text").GetComponent<TextMeshPro>().text = (i-5).ToString();
            }
        }
        
    }
}
