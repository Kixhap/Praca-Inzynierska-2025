using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteDelete : MonoBehaviour
{
    [SerializeField] string Notetype;
    private ScoreUpdater scoreUpdater;
    private GameObject xdd;
    private void Awake()
    {
        scoreUpdater = FindObjectOfType<ScoreUpdater>();
      //  StartCoroutine(Deletion());
        Debug.Log("xdd");
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right);
        Debug.DrawRay(transform.position, Vector2.right * hit.distance, Color.blue);
        if (hit.collider != null)
        {
            if (hit.distance <= 0.3f)
            {
                if (hit.collider.gameObject.CompareTag("Key"))
                {
                    Destroy(hit.collider.gameObject);
                    scoreUpdater.ScoreLiczenie(0, 0, scoreUpdater.score, -0.1f);
                }
                else if (hit.collider.gameObject.CompareTag("Slider"))
                {
                    xdd = hit.collider.gameObject;
                    Invoke("destroySlider", 15f);
                }
                else if (hit.collider.gameObject.CompareTag("MapEnd"))
                {
                    Destroy(hit.collider.gameObject);
                    SceneManager.LoadScene(2);
                }
            }
        }
    }
    private void destroySlider()
    {
        if (xdd!=null)
        {
            Destroy(xdd);
            scoreUpdater.ScoreLiczenie(0, 0, scoreUpdater.score, -0.1f);
        }
        else
        {
            return;
        }

    }
}