using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator animator;
    SpriteRenderer spriteRenderer;
    BoxCollider2D box;

    //Awake에서 게임이 시작하자마자 Think를 호출하는데, Think는 본인을 호출하는 재귀함수
    //이렇게 하는 이유는 FixedUpdate에서 그냥 Think를 호출하는 것보다 자원을 절약하기 위함
    void Awake()
    {
        rigid=GetComponent<Rigidbody2D>();
        animator=GetComponent<Animator>();
        spriteRenderer=GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        Think();
    }


    //velocity는 속도, 거리가 아니기에 멈추지 않는다. 
    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
        
        //다음 움직일 곳의 아래를 파악하고, platform레이어가 없으면, nextMove라는 속도를 반대방향으로 바꿈 
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0,1,0));
        RaycastHit2D rayhit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if(rayhit.collider == null){
            Turn();
        }
    }

    //맞았을때 흐려지고, 뒤집어지고, 콜라이더가 사라지고, deactivate를 실행하는 사용자함수
    public void OnDamaged(){
        spriteRenderer.color = new Color(1,1,1,0.4f);
        spriteRenderer.flipY = true;
        box.enabled = false;
        rigid.AddForce(Vector2.up*5, ForceMode2D.Impulse);
        Invoke("Deactivate",5);
    }

    //비활성화
    void Deactivate(){
        gameObject.SetActive(false);
    }



    //Think라는 함수는, -1,0,1이라는 3개의 방향을 랜덤으로 받을 것임,
    //랜덤은 아래 함수를 쓰고, range는 최소,최대+1로 int값으로 해주어야 함
    //Invoke는 해당 초 뒤에 해당 함수를 실행하는 것인데, 재귀호출이므로 해당초마다 Think는 반복함
    //RunningSpeed라는 애니메이터변수 사용, 0이면 멈춤, 0이아닌값이면 달리기 모션
    void Think(){

        //nextMove의 랜덤화
        nextMove = Random.Range(-1,2);
        // Debug.Log(nextMove);
        if(nextMove==0){
            animator.SetInteger("RunningSpeed",0);
            Invoke("Think", 3);
        } else {
            animator.SetInteger("RunningSpeed", nextMove);
            spriteRenderer.flipX = nextMove==1;
        }
        //재귀
        Invoke("Think", 5);
    }

    //nextMove가 FixedUpdate에서 되고, 방향전환은 Think에만 있기 때문에, 
    //명령으로 -1, 1을 받은 경우에 알아서 좌우로 바뀌지만,
    //만약 모서리에가서, FixedUpdate에서 레이로 감지한 결과, -1이 부호가 1로 바뀌어 자동으로 전환되는 경우에는
    //좌우가 변환하지 않기에, 따로 Turn이라는 함수를 넣어 처리를 해줌
    void Turn(){
        nextMove = -nextMove;
        spriteRenderer.flipX = nextMove==1;
        animator.SetInteger("RunningSpeed", nextMove);
    }
}
