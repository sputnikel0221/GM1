using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float decSpeed;

    //Inspector의 객체를 가져와서 쓰기 위한 변수
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    
    Animator animator;

    //start보다 먼저 호출되며, 게임시작전 변수나 상태의 초기화에 주로 사용
    //rigid라는 변수에 해당 스크립터가 적용된 컴포넌트를 가져온다.
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    //단발적인 키 입력은 Update를 쓰는게 좋다. 
    //일반적으로 fixed update는 60초에 50프레임이라, 한 번 눌렀을때 우연히 키가 씹힐 수 있기에
    //GetButtonUp = "버튼에서 손을 뗏을때", 속도는(x벡터, y벡터?) > x벡터 = x의 +혹은-라는 방향 + 속도값
    //normalized는 단위벡터로 바꾸는 것, 즉 현재 velocity에서 방향만 가져오고, 속도값은 1로 단위벡터화 시킴
    //즉 키보드에서 손을 떼면, 아래의 명령어가 1번 실행되어, 현재 속도가 1에서 decSpeed로 감속된만큼 적용됨.
    void Update() {
        if(Input.GetButtonUp("Horizontal")){
            rigid.velocity = new Vector2(decSpeed*rigid.velocity.normalized.x, rigid.velocity.y);
        }

        //방향전환, GetAxisRaw : https://docs.unity3d.com/ScriptReference/Input.GetAxisRaw.html
        //왼,오에 키가 눌려있으면, flip을 하는데, Horizontal축에 대한 입력값이 -1, 즉 왼쪽이면,
        //우항은 true가 되고, flipX가 true기에, X가 flip된다.
        //근데 왜 오른쪽으로 갈 때, flip이 알아서 원래대로 우측을 보게 복귀되는지?
        //버튼이 눌리고 있는가에 대한 if문을 계속 들어오므로, ==-1인지에 대한 논리식도 계속 검사하게되고
        //false로 뜨기에 우측을 보는 깔끔한 코드
         if(Input.GetButtonDown("Horizontal")){
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
         }

        
        //애니메이션 전환 (걷기<->대기)
        //normalize인데 어떻게 0 이나올수가 있을까? 단위벡터로 1또는 -1일텐데
        //그냥 속도가 0이면 멈춤이고, 그 외는 다 움직임인데, 0.3보다 속도가 작으면 멈춤으로 바꿔본다.
        //절대값으로 씌운다.
        if(Mathf.Abs(rigid.velocity.x) <0.3){
            animator.SetBool("isWalking", false);
        }else animator.SetBool("isWalking", true);
        
    }


    //키보드값을 입력받는다, 가로로만 가능
    //addforce기에 계속 누른다면 계속 힘을 증가하게 됨
    private void FixedUpdate() {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h , ForceMode2D.Impulse);

        //속도에 제한을 건다, 일정 속도 이상이면, 해당 속도벡터값을 고정값으로 변화하는 식으로..
        //해당 객체.velocity로 속도를 받아오는데, 일단 캐릭터가 좌우로만 이동하므로 .x를 붙여 x값만을 받아온다.
        //velocity는 x, y벡터로 이루어졌다, 이런걸 Vector2라고 하고, x값이 x속도, y값이 y속도인데
        //new로 x값은 maxSpeed라는 고정값으로 변경하고, y값은 현재 객체의 y속도값으로 정하기에, x의 속도만 변화된다.
        if(rigid.velocity.x > maxSpeed){
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < -maxSpeed){
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
        }
    }
}
