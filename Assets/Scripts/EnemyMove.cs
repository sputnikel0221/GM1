using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
        
    //Awake에서 게임이 시작하자마자 Think를 호출하는데, Think는 본인을 호출하는 재귀함수
    //이렇게 하는 이유는 FixedUpdate에서 그냥 Think를 호출하는 것보다 자원을 절약하기 위함
    void Awake()
    {
        rigid=GetComponent<Rigidbody2D>();
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
            nextMove = -nextMove;
        }
    }

    //Think라는 함수는, -1,0,1이라는 3개의 방향을 랜덤으로 받을 것임,
    //랜덤은 아래 함수를 쓰고, range는 최소,최대+1로 int값으로 해주어야 함
    //Invoke는 해당 초 뒤에 해당 함수를 실행하는 것인데, 재귀호출이므로 해당초마다 Think는 반복함
    void Think(){
        nextMove = Random.Range(-1,2);
        Debug.Log(nextMove);
        if(nextMove==0) Think();
        Invoke("Think", 5);
    }
}
