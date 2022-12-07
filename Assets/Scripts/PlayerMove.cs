using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //game Manager는 public으로 생성.. 그리고 inspector에 사용자가 직접 매니져를 끌어다 넣어서 사용..
    public GameManager gameManager;
    public float maxSpeed;
    public float decSpeed;
    public float jumpPower;

    //Inspector의 객체를 가져와서 쓰기 위한 변수
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    
    Animator animator;

    //사운드를 담을 공간
    public AudioClip aJump;
    public AudioClip aAttack;
    public AudioClip aDamaged;
    public AudioClip aItem;
    public AudioClip aDie;
    public AudioClip aFinish;
    AudioSource audioSource;
    //오디오소스 객체가 해당 오디오클립을 받아 실행하는 원리인 듯


    //start보다 먼저 호출되며, 게임시작전 변수나 상태의 초기화에 주로 사용
    //rigid라는 변수에 해당 스크립터가 적용된 컴포넌트를 가져온다.
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        //왼,오에 키가 눌려있으면(getbuttonDown인데, getbutton으로 바꿈), flip을 하는데, Horizontal축에 대한 입력값이 -1, 즉 왼쪽이면,
        //우항은 true가 되고, flipX가 true기에, X가 flip된다.
        //근데 왜 오른쪽으로 갈 때, flip이 알아서 원래대로 우측을 보게 복귀되는지?
        //버튼이 눌리고 있는가에 대한 if문을 계속 들어오므로, ==-1인지에 대한 논리식도 계속 검사하게되고
        //false로 뜨기에 우측을 보는 깔끔한 코드
         if(Input.GetButton("Horizontal")){
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
         }

        
        //애니메이션 전환 (걷기<->대기)
        //normalize인데 어떻게 0 이나올수가 있을까? 단위벡터로 1또는 -1일텐데
        //그냥 속도가 0이면 멈춤이고, 그 외는 다 움직임인데, 0.3보다 속도가 작으면 멈춤으로 바꿔본다.
        //절대값으로 씌운다.
        if(Mathf.Abs(rigid.velocity.x) <0.3){
            animator.SetBool("isWalking", false);
        }else animator.SetBool("isWalking", true);
        

        //점프 - vertical이 아니라 jump라는 예약어가 따로 있다..
        if(Input.GetButtonDown("Jump") && !animator.GetBool("isJumping")){
            rigid.AddForce(Vector2.up * jumpPower , ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
            PlaySound("JUMP");
        }
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


        //플레이어의 발이 어딘가에 붙어있는지 찾는법, oncollider의 심화버전, RAY를 쓴다
        //Landing Platform, 디버그문은 에디터창에서만 보인다. 현재 위치 아래를 쏘는 것/
        //Ray를 그리는데, 현재위치,아래로,초록색, 이건 그냥 디버그용 허울이고
        //실제 Raycast는 물리를 이용한다, raycasthit2d는 아래 명령어로 현재위치,아래로,1칸을 탐지, "특정레이어"만 감지
        //그래서 rayHit이 충돌한 물체가 존재하면 해당 이름을 표시하는 디버그 (layer를 지정하지 않으면 본인이 탐지됨)
        //0이 아니라 0.5인 이유는 플레이어가 1의 collider인데 현재위치에서 1만큼 나가기에 0.5여야 땅에 완전히 붙은것
        //y의 속도값이 0이하 즉 내려오고 있는 경우에만 파악 : 땅에서 점프 하는 찰나 레이캐스트가 인식하는 경우가 존재했다.
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0,1,0));

        if(rigid.velocity.y<0){
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down,1, LayerMask.GetMask("Platform"));
            if(rayHit.collider!=null){
                if(rayHit.distance < 0.5f){
                    animator.SetBool("isJumping", false);
            }
            }
        }
    }

    //현재 스크립트가 적용된 물체가 충돌을 한다면, 해당 객체를 other로 받고 진행
    //transform.position.y>other.transform.position.y 이게 중요한듯 싶다
    //둘 다 inspector의 transform.position항목의 y값을 사용.
    //본인은 굳이 앞에 객체이름을 붙일 필요가 없으나, 부딪힌것은 표시해야됨

    //위에서 내려오는 중에 enemy객체를 부딪히는 경우 공격으로 인식해야됨
    void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag=="Enemy"){
            if(rigid.velocity.y <0 && transform.position.y>other.transform.position.y){
                OnAttack(other.gameObject);
            }
            else{
            OnDamaged();
        }
        } 
        
    }

    //사용자 정의함수, 적을 공격
    //https://mingyu0403.tistory.com/22
    //https://docs.unity3d.com/kr/530/ScriptReference/Collision2D.html
    //왜 gameObject가 아닌 Transform으로 enemy를 받을까 > 둘 다 잘 작동함

    //EnemyMove 스크립트의 객체를 만들고, 부딪힌 enemy객체와 동기화
    //해당 객체의 on damaged함수를 실행
    void OnAttack(GameObject enemy){
        rigid.AddForce(Vector2.up*3,ForceMode2D.Impulse);
        gameManager.stagePoint+=100;
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        PlaySound("ATTACK");
    }
    // void OnAttack(Transform enemy){
    //     rigid.AddForce(Vector2.up*3,ForceMode2D.Impulse);
    //     EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
    //     enemyMove.OnDamaged();
    // }

    //사용자 정의함수, 피해를 입으면 실행
    //gameObject는 스크립트가 적용된 객체 : 즉 맞은 객체의 레이어를 바꿈
    void OnDamaged(){
        gameObject.layer = 11;
        gameManager.HealthDown();
        //무적효과를 위한 반투명화
        spriteRenderer.color = new Color(1,1,1,0.4f);

        //순간적인 위로의 반작용
        rigid.AddForce(new Vector2(0,2)*7, ForceMode2D.Impulse);

        //animation - trigger
        animator.SetTrigger("getDamaged");
        PlaySound("DAMAGED");
        Invoke("offDamaged", 3);
    }

    //무적을 해제
    void offDamaged(){
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1,1,1,1);
    }

    //동전과 finish깃발에 적용하기 위한 함수
    //이건 트리거, isTrigger부분을 체크해야 쓸 수 있다.
    //참고 : https://keykat7.blogspot.com/2020/12/unity3d-ontrigger-oncollision.html
    //is trigger은 내부로 통과할 수 있는듯
    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag=="Item"){
            gameManager.stagePoint+=100;
            other.gameObject.SetActive(false);
            PlaySound("ITEM");
        }
        else if (other.gameObject.tag=="Finish"){
            gameManager.NextStage();
            PlaySound("FINISH");
        }

    }

    public void Ondie(){
        Time.timeScale = 0;
        Debug.Log("죽었습니다!");
        //플레이어의 죽음
    }

    void PlaySound(string action){
        switch(action){
            case "JUMP":
                audioSource.clip = aJump;
                break;
            case "ATTACK":
                audioSource.clip = aAttack;
                break;
            case "DAMAGED":
                audioSource.clip = aDamaged;
                break;
            case "ITEM":
                audioSource.clip = aItem;
                break;
            case "DIE":
                audioSource.clip = aDie;
                break;
            case "FINISH":
                audioSource.clip = aFinish;
                break;   
        }
        audioSource.Play();
    }
}
