using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number : MonoBehaviour {

    private Vector3 posStart = Vector3.zero;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
    public void SetInfo(int score, Vector3 pos)
    {
        posStart = pos;
        StartCoroutine("Show");
    }

    private IEnumerator Show()
    {
        for (int i = 0; i <= 10; i++)
        {
            transform.position = new Vector3(0, Mathf.Lerp(0, 3, i / 10.0f), 0) + posStart;
            transform.localScale = new Vector3(Mathf.Lerp(1, -1, i / 10.0f), 1, 0);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i <= 10; i++)
        {
            transform.position = new Vector3(0, Mathf.Lerp(3, 1.5f, i / 10.0f), 0) + posStart;
            transform.localScale = new Vector3(Mathf.Lerp(-1, 1, i / 10.0f), 1, 0);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(2);
        Destroy(transform.gameObject);
    }
}
