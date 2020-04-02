using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject player;
    public Transform posEnter;
    public bool isLevel;
    public Transform[] scores;

    public static GameManager _inistance; //单例


    private int iLevel = 0;

    void Awake()
    {
        Screen.SetResolution(1360, 768, true);
        _inistance = this;
    }

	// Use this for initialization
	void Start () {
        NextPlayer();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NextPlayer(){
        if(isLevel)
        {
            if (iLevel >= 5)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                if (iLevel > 0)
                    scores[iLevel - 1].gameObject.SetActive(false);

                Vector3 pos = scores[iLevel].position;
                pos.x += Random.Range(-2, 2);
                scores[iLevel].position = pos;
                scores[iLevel].gameObject.SetActive(true);

                GameObject.Instantiate(player, posEnter.position, Quaternion.identity, transform.parent);
                iLevel++;
            }
        }
        else
        {
            GameObject.Instantiate(player, posEnter.position, Quaternion.identity, transform.parent);
        }
    }
}
