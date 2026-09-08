using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask collisionLayer;

    private bool isMoving;

    private void Update()
    {
        if (isMoving)
            return;

        Vector3 direction = GetDirection();

        if (direction != Vector3.zero)
        {
            Vector3 targetPosition =
                transform.position + direction * gridSize;

            if (IsWalkable(targetPosition))
            {
                StartCoroutine(Move(direction));
            }
        }
    }

    private Vector3 GetDirection()
    {
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        bool up = Input.GetKey(KeyCode.W);
        bool down = Input.GetKey(KeyCode.S);

        if (left && right)
            return Vector3.zero;

        if (up && down)
            return Vector3.zero;

        if (right)
            return Vector3.right;

        if (left)
            return Vector3.left;

        if (up)
            return Vector3.up;

        if (down)
            return Vector3.down;

        return Vector3.zero;
    }

    private IEnumerator Move(Vector3 direction)
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition =
            startPosition + direction * gridSize;

        while ((targetPosition - transform.position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(
                // Where from
                transform.position,
                // Where to
                targetPosition,
                // How much
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;

        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPosition)
    {
        Collider2D collider = Physics2D.OverlapCircle(
            targetPosition,
            0.2f,
            collisionLayer
        );

        return collider == null;
    }
}