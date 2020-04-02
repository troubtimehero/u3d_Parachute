using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour {

    public GameObject lightBg;
    public GameObject lightFt;
    public GameObject box;
    public Animator cap;
    public GameObject lightFace;
    public GameObject score;
    public Text scoreText;

    private bool isOpen = false;

    private bool canClick = false;

    public bool CanClick
    {
        get { return canClick; }
        set { canClick = value; }
    }

    public void OnOpen()
    {
        if (isOpen || !canClick)
            return;
        isOpen = true;

        lightBg.SetActive(true);
        lightFt.SetActive(true);
        cap.Play("boxCap");

        Invoke("ShowScore", 1);

        scoreText.text = AwardMgr._inistance.GetScore().ToString();
        AwardMgr._inistance.OnBoxOpen();
    }

    private void ShowScore()
    {
        score.SetActive(true);
        if(isOpen)
        {
            lightFace.SetActive(true);
            Invoke("HideOther", 3);
        }
    }

    private void HideOther()
    {
        lightBg.SetActive(false);
        lightFt.SetActive(false);
        box.SetActive(false);
        cap.gameObject.SetActive(false);
        lightFace.SetActive(false);
    }

    public void AutoShow()
    {
        if(!isOpen)
        {
            cap.Play("boxCap");
            scoreText.text = AwardMgr._inistance.GetScore().ToString();
            scoreText.color = new Color(0.5f, 0.5f, 0.5f);
            Invoke("ShowScore", 0.3f);
        }
    }
}
