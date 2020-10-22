using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainPlayer : MonoBehaviour
{
    [Header("Joystick control")]
    public FloatingJoystick joystick;
    [SerializeField] 
    private float _sensitive = 1f;
    [SerializeField] 
    private float _speedModifier = 1;
    
    [Header("LineRenderer settings")]
    [SerializeField]
    private float _rayLength;
    
    [Header("Player settings")]
    [SerializeField] 
    private float _jumpForce;
    [SerializeField] 
    private float _longJumpTime;

    [Header("UI")] 
    public GameObject perfectText;
    public GameObject longJumpText;
    public GameObject startGameUI;
    public Slider slider;
    public GameObject sliderObject;
    public GameObject winText;

    [Header("Effects")] 
    public GameObject repulsionEffect;
    public GameObject firework;

    private Animator anim;
    private LineRenderer _lineRenderer;
    private Color startColor;
    private Color secondColor;
    private Rigidbody rb;
    private GameObject lineRendererObject;
    private float _timer;
    private float _deltaX;
    public GameObject camera;
    private ParticleSystem pSystem;
    public int distanceTraveled;
    public LevelGenerate levelGenerate;

    void Awake()
    {
        pSystem = gameObject.transform.Find("FireWork").GetComponent<ParticleSystem>();
        lineRendererObject = transform.Find("LineRenderer").gameObject;
        _lineRenderer = lineRendererObject.GetComponent<LineRenderer>();
    }

    void Start()
    {
        Time.timeScale = 0;
        _timer = 0;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        startColor = _lineRenderer.startColor;
        secondColor = Color.green;
        secondColor.a = 0.5f;
    }
    
    void Update()
    {
        pSystem.Simulate(Time.unscaledDeltaTime,true,false);
        
        slider.value = distanceTraveled;
        _timer += Time.deltaTime;
        
        transform.Translate(Vector3.forward * joystick.Direction.y);
        _deltaX += (joystick.Direction.x + 1) * (joystick.Direction.x + 1) * _sensitive - _sensitive;
        transform.rotation = Quaternion.Euler(0, _deltaX * _speedModifier, 0);
    }

    void LateUpdate()
    {
        _lineRenderer.SetPosition(0,transform.position);
        var secondLinePosition = new Vector3(transform.position.x, transform.position.y - _rayLength, transform.position.z);
        _lineRenderer.SetPosition(1,secondLinePosition);

        Ray ray = new Ray(transform.position, Vector3.down * _rayLength);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Trampoline")
                _lineRenderer.SetColors(secondColor, secondColor);
        }
        else _lineRenderer.SetColors(startColor, startColor);

        if (distanceTraveled >= levelGenerate.platforms.Count() && Time.timeScale > 0)
        {
            Time.timeScale = 0;
            firework.SetActive(true);
            camera.gameObject.GetComponent<Animator>().SetTrigger("finish");
            winText.SetActive(true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Trampoline")
        {
            rb.velocity = Vector3.up * _jumpForce;
            anim.SetTrigger("flip");
            Instantiate(repulsionEffect, transform.position, Quaternion.identity);
            repulsionEffect.SetActive(true);

            if (_timer >= _longJumpTime)
            {
                _timer = 0;
                longJumpText.SetActive(true);
                StartCoroutine(ShutdownAnimation(longJumpText));
            }
            
            if (collision.gameObject.name == "Bull's-eye")
            {
                if (_timer < _longJumpTime)
                {
                    _timer = 0;
                    perfectText.SetActive(true);
                    StartCoroutine(ShutdownAnimation(perfectText));
                }
            }
            
            collision.gameObject.transform.parent.GetComponent<Platform>().destruction++;
            int.TryParse(collision.transform.parent.Find("Text").GetComponent<TextMeshPro>().text, out var helpfulOut);
            distanceTraveled = helpfulOut;
        }
    }
    
    public void StartGame()
    {
        startGameUI.SetActive(false);
        camera.gameObject.GetComponent<Animator>().SetTrigger("lookAround");
        StartCoroutine(TimeNormalization());
    }

    IEnumerator TimeNormalization()
    {
        yield return new WaitForSecondsRealtime(5.50f);
        sliderObject.SetActive(true);
        Time.timeScale = 1;
    }
    
    IEnumerator ShutdownAnimation(GameObject UI)
    {
        yield return new WaitForSeconds(1f);
        UI.SetActive(false);
    }
    
}
