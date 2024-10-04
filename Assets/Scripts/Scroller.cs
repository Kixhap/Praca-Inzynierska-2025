using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Scroller : MonoBehaviour
{
    [SerializeField]
    private int ScrollSpeed;

    public void startMap(float? lastX1)
    {
        ScrollSpeed /= 10;
        StartCoroutine(ScrollCoroutine());
        StartCoroutine(FinishMap(lastX1));
    }

    private IEnumerator ScrollCoroutine()
    {
        while (true)
        {
            transform.position -= new Vector3(ScrollSpeed * Time.deltaTime, 0f, 0f);
            yield return null;
        }
    }
    private IEnumerator FinishMap(float? lastX1)
    {
        if (transform.position.x == lastX1) 
        {
            SceneManager.LoadScene("ScoreScreen");
        }
        yield return null;
    }
}