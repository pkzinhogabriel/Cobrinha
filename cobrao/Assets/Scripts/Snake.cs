using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public static Snake instance {  get; private set; }
    public GameObject prefabSegmento;
    public float intervaloMovimento = 0.1f;
    private Vector2 direcao = Vector2.right;
    private List<Transform> segmentos;
    private float temporizadorMovimento;

    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
       segmentos = new List<Transform>();
        AdicionarSegmento();
        temporizadorMovimento = intervaloMovimento;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
