using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float turnDelay = 0.1f;

    private Animator animator;

    private bool isMoving;
    private bool useFirstStep = true;
    private bool waitingAfterTurn;
    private float turnTime;

    private Vector3 facingDirection = Vector3.down;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        PlayIdleAnimation();
    }

    private void Update()
    {
        if (isMoving)
            return;

        Vector3 requestedDirection = GetDirection();

        // Button was released after turning.
        // Next press may move immediately.
        if (requestedDirection == Vector3.zero)
        {
            waitingAfterTurn = false;
            return;
        }

        // Different direction: turn, but don't move yet.
        if (requestedDirection != facingDirection)
        {
            facingDirection = requestedDirection;
            PlayIdleAnimation();

            waitingAfterTurn = true;
            turnTime = Time.time;

            return;
        }

        // We just turned into this direction.
        // Only start walking if the button is held long enough.
        if (waitingAfterTurn)
        {
            if (Time.time - turnTime < turnDelay)
                return;

            waitingAfterTurn = false;
        }

        Vector3 targetPosition =
            transform.position + requestedDirection * gridSize;

        if (IsWalkable(targetPosition))
        {
            StartCoroutine(Move(requestedDirection));
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

        string stepAnimation = GetStepAnimationName(direction);
        animator.Play(stepAnimation, 0, 0f);

        Vector3 startPosition = transform.position;
        Vector3 targetPosition =
            startPosition + direction * gridSize;

        while ((targetPosition - transform.position).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;

        useFirstStep = !useFirstStep;

        PlayIdleAnimation();

        isMoving = false;
    }

    private string GetStepAnimationName(Vector3 direction)
    {
        string directionName = GetDirectionName(direction);
        string stepName = useFirstStep ? "step1" : "step2";

        return $"{stepName}_{directionName}";
    }

    private void PlayIdleAnimation()
    {
        string directionName = GetDirectionName(facingDirection);

        animator.Play($"idle_{directionName}", 0, 0f);
    }

    private string GetDirectionName(Vector3 direction)
    {
        if (direction == Vector3.down)
            return "down";

        if (direction == Vector3.left)
            return "left";

        if (direction == Vector3.up)
            return "up";

        if (direction == Vector3.right)
            return "right";

        return "down";
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