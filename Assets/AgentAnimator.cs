using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAnimator : MonoBehaviour
{
    [SerializeField] private bool isFlippingSprite;
    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 animationDirection;

    private Dictionary<Vector2, int> cardinalIntMappings = new Dictionary<Vector2, int>{
        { Vector2.zero, 0 },
        { Vector2.up, 1 },
        { Vector2.right, 2 },
        { Vector2.down, 3 },
        { Vector2.left, 4 }
    };

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animationDirection = Vector2.zero;
    }

    void Update() {
        Vector2 velocity = (Vector2) agent.velocity;

        if (isFlippingSprite) {
            spriteRenderer.flipX = velocity.x < 0;
            return;
        }

        float threshold = 1.1f;
        Vector2 newAnimationDirection;
        if (velocity == Vector2.zero) {
            newAnimationDirection = Vector2.zero;
        } else if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y) * threshold) {
            newAnimationDirection = velocity.x > 0 ? Vector2.right : Vector2.left;
        } else if (Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x) * threshold) {
            newAnimationDirection = velocity.y > 0 ? Vector2.up : Vector2.down;
        } else {
            newAnimationDirection = animationDirection;
        }

        // Update animation direction if needed
        if (newAnimationDirection != animationDirection) {
            animationDirection = newAnimationDirection;
            animator.SetInteger("Direction", cardinalIntMappings[animationDirection]);
        }
    }
}
