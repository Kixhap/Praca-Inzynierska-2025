using System.Collections;
using UnityEngine;

public class HitHandler : MonoBehaviour
{
    private int value;
    private int combo;
    private float healthchange;
    private ScoreUpdater scoreUpdater;
    private void Awake()
    {
        scoreUpdater = FindObjectOfType<ScoreUpdater>();
    }
    public void CheckForNotes(KeyCode przycisk)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right);
        Debug.Log(hit.distance);
        if (hit.collider != null)
        {
            if (hit.distance < 1.5f)
            {

                if (hit.collider.gameObject.CompareTag("Slider"))
                {
                    Vector3 leftEdge = hit.collider.gameObject.GetComponent<Collider2D>().bounds.min;
                    Vector3 rightEdge = hit.collider.gameObject.GetComponent<Collider2D>().bounds.max;
                    CheckValueSlider(hit.collider.gameObject, leftEdge, rightEdge, przycisk);
                    Debug.Log("Test bounds redge" + rightEdge);
                    Debug.Log("Test bounds ledge" + leftEdge);
                }
                else if (hit.collider.gameObject.CompareTag("Key"))
                {
                    //Vector2 position = -transform.position + hit.collider.gameObject.transform.position;
                    CheckValueNote(hit.collider.gameObject, /*position*/hit.distance);
                }
            }
        }
    }
    void CheckValueSlider(GameObject slider, Vector2 Pos,Vector2 RightE, KeyCode button)
    {
            if (Pos.x > 0.75)
            {
                value = 0;
                combo = 0;
                healthchange = -0.1f;
            }
            else if (Pos.x > 0.5)
            {
                value = 100;
                combo = 1;
                healthchange = 0.01f;
            }
            else if (Pos.x > 0.15)
            {
                value = 200;
                combo = 1;
                healthchange = 0.05f;
            }
            else if (Pos.x <= 0.15)
            {
                value = 300;
                combo = 1;
                healthchange = 0.1f;
            }
        StartCoroutine(SliderScore(value, slider, RightE, button));
    }
      
    void CheckValueNote(GameObject Note, float Pos)
    {
            Destroy(Note);
            if (Pos > 0.75)
            {
                value = 0;
                combo = 0;
                healthchange = -0.1f;
            }
            else if (Pos > 0.5)
            {
                value = 100;
                combo = 1;
                healthchange = 0.01f;
            }
            else if (Pos > 0.15)
            {
                value = 200;
                combo = 1;
                healthchange = 0.05f;
                
            }
            else if (Pos <= 0.15)
            {
                value = 300;
                combo = 1;
                healthchange = 0.1f;
            }
        
        scoreUpdater.ScoreLiczenie(combo, value, scoreUpdater.score, healthchange);
    }
    private IEnumerator SliderScore(int value, GameObject slider,Vector2 RightE, KeyCode przycisk)
    {
        int x=0;
        while (Input.GetKey(przycisk)&& RightE.x>-4 )
        {
            x++;
            if (x == 10)
            {
                combo += 1;
                x = 0;
            }
            if (slider != null)
            {
                RightE.x = slider.GetComponent<Collider2D>().bounds.max.x;
                Debug.Log(RightE.x);
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(.01f);
        }
        scoreUpdater.ScoreLiczenie(combo, value, scoreUpdater.score, healthchange);
        Destroy(slider);
        yield break;
    }
}
