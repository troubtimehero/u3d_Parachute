using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AwardMgr : MonoBehaviour {

    static public AwardMgr _inistance;
    public Transform grid;
    public GameObject box;

    public AudioClip clipShowBox;
    public AudioClip clipOpen;
    public AudioClip clipWin;

    private Box[] boxes;
    private int clickCount = 0;
    private List<int> lstScores;

    void Awake()
    {
        _inistance = this;
        boxes = new Box[8];

        lstScores = new List<int>();
        lstScores.Add(10);
        lstScores.Add(20);
        lstScores.Add(50);
        lstScores.Add(100);
        lstScores.Add(200);
        lstScores.Add(500);
        lstScores.Add(1000);
        lstScores.Add(3000);
    }

    void Start()
    {
        StartCoroutine("IShowBox");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator IShowBox()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 8; i++)
        {
            GameObject go = GameObject.Instantiate(box);
            boxes[i] = go.GetComponent<Box>();
            go.transform.SetParent(grid);
            AudioSource.PlayClipAtPoint(clipShowBox, Vector3.zero);
            yield return new WaitForSeconds(0.15f);
        }

        for (int i = 0; i < 8; i++)
        {
            boxes[i].CanClick = true;
        }
    }

    public int GetScore()
    {
        print("list Count : " + lstScores.Count);
        int idx = Random.Range(0, lstScores.Count-1);
        int score = lstScores[idx];
        lstScores.RemoveAt(idx);
        return score;
    }

    public void OnBoxOpen()
    {
        clickCount++;
        AudioSource.PlayClipAtPoint(clipOpen, Vector3.zero);

        if (clickCount >= 3)
        {
            for (int i = 0; i < 8; i++)
            {
                boxes[i].CanClick = false;
            }
            Invoke("AutoShow", 3);
        }
    }

    private void AutoShow()
    {
        for (int i = 0; i < 8; i++)
        {
            boxes[i].AutoShow();
        }
        AudioSource.PlayClipAtPoint(clipWin, Vector3.zero);

        Invoke("Back2Game", 5);
    }

    private void Back2Game()
    {
        SceneManager.LoadScene(0);
    }
}
