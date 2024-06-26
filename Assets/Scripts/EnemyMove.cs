using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public int nextMove;
    public float minThinkTimeInSecInclusive = 2f;
    public float maxThinkTimeInSecExclusive = 5f;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    CapsuleCollider2D capsuleCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        ReserveThink();
    }

    void FixedUpdate()
    {
        // move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // Landing Platform
        Vector2 frontVec = new Vector2 (rigid.position.x + nextMove * 0.4f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, Color.green);
        RaycastHit2D raycastHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (raycastHit.collider == null)
            Turn();
    }
    
    void Think()
    {
        // create next action -1, 0, 1
        nextMove = Random.Range(-1, 2);

        UpdateSprite();
        ReserveThink();
    }

    void Turn()
    {
        CancelInvoke();

        nextMove = -nextMove;

        UpdateSprite();
        ReserveThink();

    }

    void UpdateSprite()
    {
        // sprite animation
        animator.SetInteger("WalkSpeed", nextMove);

        // flip sprite
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;
    }

    void ReserveThink()
    {
        // 오... 이 동네는 주기적 실행을 이렇게 하는건가...
        float nextThinkTime = Random.Range(minThinkTimeInSecInclusive, maxThinkTimeInSecExclusive);
        Invoke("Think", nextThinkTime);
    }

    public void OnDamaged()
    {
        // 맞으면 그냥 죽음
        OnDie();
    }

    void OnDie()
    {
        // Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Sprite Flip Y
        spriteRenderer.flipY = true;

        // Collider Disable
        capsuleCollider.enabled = false;

        // Die Effect Jump
        rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);

        // Destroy
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
