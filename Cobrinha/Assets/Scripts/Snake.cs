using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public GameManager gameManager;
    private Vector2 direction;
    private float changeCellTime = 0;
    public List<Transform> body = new List<Transform>();
    public float speed = 10.0f;
    public float cellSize = 0.3f;
    public Vector2 cellIndex = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        direction = Vector2.up;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ChangeDirection()
    {
        Vector2 newdirection = Vector2.zero;
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.y == -1) newdirection = Vector2.down;
        else if (input.y == 1) newdirection = Vector2.up;
        else if (input.x == -1) newdirection = Vector2.left;
        else if (input.x == 1) newdirection = Vector2.right;
        if (newdirection + newdirection != Vector2.zero && newdirection != Vector2.zero)
        {
            direction = newdirection;
        }
    }
    void Move()
    {
        if (Time.time > changeCellTime)
        {
            for (int i = body.Count - 1; i > 0; i--)
            {
                body[i].position = body[i - 1].position;
            }
            if (body.Count > 0) body[0].position = (Vector2)transform.position;

            transform.position += (Vector3)direction * cellSize;

            changeCellTime = Time.time + 1 / speed;
            cellIndex = transform.position / cellSize;

            CheckWallWrapAround();
        }
    }
    void CheckWallWrapAround()
    {

    }
}
