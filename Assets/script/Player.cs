using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    private Rigidbody2D rg;
    private bool isClick = false;   //防止开场下落时的反弹引发 OnCollisionExit2D
    private bool isStopping = false;
    private bool isMouseDown = false;

    public float power;
    public float rotMove;
    public float rotOpen;
    public float rotWheel;
    private float lastRotation = 0;
    public float dragAir;
    public float dragGround;
    public float gravityOpen;

    enum State{
        STATE_APPEAR,
        STATE_MOVE,
        STATE_DROP,
        STATE_OPEN,
        STATE_CRASH,
        STATE_LAND,
        STATE_WIN,
        STATE_LOSE
    };
    private State curState;

    public GameObject goMove;
    public Transform tfMoveWheel;
    public GameObject goDrop;
    public Transform tfDropSan;
    public GameObject goFace;
    public GameObject goWin;
    public GameObject goLose;

    public GameObject[] number;

    public AudioClip clipStart;
    public AudioClip clipSpeedUp1;
    public AudioClip clipSpeedUp2;
    public AudioClip clipSpeedDown;
    public AudioClip clipDrop;
    public AudioClip clipDropCry1;
    public AudioClip clipDropCry2;
    public AudioClip clipDropCry3;
    public AudioClip clipOpen;
    public AudioClip clipWin;
    public AudioClip clipLose;

    private AudioSource snd;

	// Use this for initialization
	void Start () {
        rg = GetComponent<Rigidbody2D>();
        snd = GetComponent<AudioSource>();
        score = -1;

        SetState(State.STATE_APPEAR);
	}
	
	// Update is called once per frame
	void Update () {
//         if (State.STATE_APPEAR == curState || State.STATE_WIN == curState || State.STATE_LOSE == curState)
//             return;

        if (Input.GetMouseButtonDown(0))
            isMouseDown = true;
        if (Input.GetMouseButtonUp(0))
            isMouseDown = false;

        switch (curState)
        {
            case State.STATE_MOVE:
                if(isMouseDown)
                {
                    float force = (Input.mousePosition.x - 680) / 680.0f;
                    force = Mathf.Clamp(force, -1, 1);
                    print(force);
                    if (force != 0)
                    {
                        if (lastRotation < 0 && force == -1)
                            PlayAudio(clipSpeedDown);

                        isClick = true;
                        rg.AddForce(new Vector2(1, 0) * force * power);
                        lastRotation = Mathf.Lerp(lastRotation, -force * rotMove, Time.deltaTime * 10);
                        transform.localRotation = Quaternion.Euler(0, 0, lastRotation);
                    }
                }
                else
                {
                    lastRotation = Mathf.Lerp(lastRotation, 0, Time.deltaTime * 5);
                    transform.localRotation = Quaternion.Euler(0, 0, lastRotation);
                }
                break;

            case State.STATE_DROP:
                if (Input.GetMouseButtonDown(0))
                {
                    SetState(State.STATE_OPEN);
                }
                lastRotation = Mathf.Lerp(lastRotation, 0, Time.deltaTime * 3);
                transform.localRotation = Quaternion.Euler(0, 0, lastRotation);
                break;

            case State.STATE_OPEN:
                lastRotation = Mathf.Lerp(lastRotation, rg.velocity.x * rotOpen, Time.deltaTime * 3);
                transform.localRotation = Quaternion.Euler(0, 0, lastRotation);
                break;

            case State.STATE_CRASH:
            case State.STATE_LAND:
                lastRotation = Mathf.Lerp(lastRotation, 0, Time.deltaTime * 3);
                transform.localRotation = Quaternion.Euler(0, 0, lastRotation);
                if (isStopping && rg.velocity.magnitude == 0)
                    AlgScore();
                break;
        }

        //轮子转动
        if (State.STATE_MOVE == curState || State.STATE_CRASH == curState || State.STATE_LAND == curState)
        {
            tfMoveWheel.rotation = Quaternion.Euler(0, 0, rotWheel * transform.position.x);
        }
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.transform.name == "pingTai")
        {
            SetState(State.STATE_MOVE);
            rg.velocity = Vector2.zero;
        }
        else if (other.transform.name == "ground")
        {
            rg.velocity = new Vector2(rg.velocity.x, 0);
            rg.drag = dragGround;

            switch (curState)
            {
                case State.STATE_DROP:
                    SetState(State.STATE_CRASH);
                    break;

                case State.STATE_OPEN:
                    SetState(State.STATE_LAND);
                    break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (isClick && other.transform.name == "pingTai")
        {
            SetState(State.STATE_DROP);
            rg.drag = dragAir;
        }
    }

    private void SetState(State state)
    {
        curState = state;
        switch(state)
        {
            case State.STATE_APPEAR:
                goMove.SetActive(true);
                goDrop.SetActive(false);
                tfDropSan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                goFace.SetActive(false);
                goWin.SetActive(false);
                goLose.SetActive(false);
                PlayAudio(clipStart);
                break;

            case State.STATE_MOVE:
                goFace.SetActive(true);
                break;

            case State.STATE_DROP:
                PlayAudio(clipDrop);
                break;

            case State.STATE_OPEN:
                goMove.SetActive(false);
                goDrop.SetActive(true);
                rg.velocity = new Vector2(rg.velocity.x, 0);
                rg.gravityScale = gravityOpen;
                PlayAudio(clipOpen);
                break;

            case State.STATE_CRASH:
                Invoke("OnStopping", 1.5f);
                PlayAudio(clipDropCry3);
                break;

            case State.STATE_LAND:
                Invoke("OnStopping", 1.5f);
                StartCoroutine("IDroppingSan");
                PlayAudio(clipDropCry2);
                break;

            case State.STATE_WIN:
                goMove.SetActive(false);
                goDrop.SetActive(false);
                goFace.SetActive(false);
                goWin.SetActive(true);
                PlayAudio(clipWin);
                break;

            case State.STATE_LOSE:
                goMove.SetActive(false);
                goDrop.SetActive(false);
                goFace.SetActive(false);
                goLose.SetActive(true);
                PlayAudio(clipLose);
                break;
        }
    }

    IEnumerator IDroppingSan()
    {
        for (int i = 0; i < 10; i++)
        {
            tfDropSan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, i / 9.0f));
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnStopping()
    {
        isStopping = true;
    }
    
    private void AlgScore()
    {
        if (score >= 0)
        {
            SetState(State.STATE_WIN);
            if(!GameManager._inistance.isLevel)
                ShowScore(score);
        }
        else
        {
            SetState(State.STATE_LOSE);
        }

        if (GameManager._inistance.isLevel && score < 0)
            Invoke("Retry", 2);
        else
            Invoke("Finish", 1.5f);
    }

    private void Finish()
    {
        Destroy(transform.gameObject);
        GameManager._inistance.NextPlayer();
    }

    private void Retry()
    {
        SceneManager.LoadScene(0);
    }

    private int score;

    public int Score
    {
        set { score = value; }
    }
    
    public void ShowScore(int score)
    {
        int numIdx = -1;
        switch (score)
        {
            case 5: numIdx = 0; break;
            case 10: numIdx = 1; break;
            case 15: numIdx = 2; break;
            case 20: numIdx = 3; break;
            case 0: numIdx = 4; break;
        }
        if(numIdx >= 0)
        {
            GameObject go = GameObject.Instantiate(number[numIdx]);
            go.GetComponent<Number>().SetInfo(score, transform.position);
        }
    }

    private void PlayAudio(AudioClip clip)
    {
        if(snd.isPlaying)
            snd.Stop();
        snd.clip = clip;
        snd.Play();
    }
}
