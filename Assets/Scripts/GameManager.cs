using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//추가필요
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//스테이지, 점수를 관리
public class GameManager : MonoBehaviour
{
    //점수, 스테이지
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public PlayerMove player;
    public GameObject[] stages;

    //체력 관리
    public int health;
    
    //UI를 담을 공간 마련
    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RetryBtn;


    //점수는 왜 update문으로 표시를 할까? 단일문인가?
    void Update() {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage(){
        //stage move
        if(stageIndex<stages.Length-1){
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE" + (stageIndex+1);
        }
        else{//game end
            Time.timeScale = 0;
            UIStage.text = "CLEAR";
        }
        

        //calculate point
        totalPoint += stagePoint;
        stagePoint = 0;
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag=="Player"){
            HealthDown();

            other.transform.position = new Vector3(-0.45f,1.97f,0);
        }
    }

    public void HealthDown(){
        if(health>1){
            health--;
            UIhealth[health].enabled = false;
        }
        else{
            UIhealth[0].enabled = false;
            player.Ondie();
            RetryBtn.SetActive(true);
        }
    }

    void PlayerReposition(){
        player.transform.position = new Vector3(-0.45f,1.97f,0);
    }

    //재시작시 0번씬 시작
    public void Retry(){
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
