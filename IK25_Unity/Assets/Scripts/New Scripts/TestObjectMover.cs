using UnityEngine;

public class TestObjectMover : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            moveY = moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            moveY = -moveSpeed * Time.deltaTime;
        }

        transform.Translate(moveX, moveY, moveZ);
    }
}