
using UnityEngine;
using UnityEngine.UI;

public class Lock : MonoBehaviour, IClickable
{
    [SerializeField] private float forceY = 1f;
    [SerializeField] private float forceX = 1f;
    [SerializeField] private float torque = 20f;
    [SerializeField] private float timeToDestroy = 2f;
    [SerializeField] private Sprite unLock;
    [SerializeField] private Chains chains;
    private GameObject popUpQuestionUI;
    

  //  QuestionSO currentQuestion;

    private void Awake()
    {
        popUpQuestionUI = FindObjectOfType<PopUpUI>(true).gameObject;
    }

   
    public void ProcessClickAction()
    {
        if (popUpQuestionUI.activeInHierarchy)
        {
            return;
        }
        //  GameManager.Instance.SetCurrentQuestion(currentQuestion);
        GameManager.Instance.StartAnswering();
        GameManager.Instance.SetCurrentLock(this.gameObject);
        popUpQuestionUI.SetActive(true);
        GameManager.Instance.ResetQuestionTimer();
    }
    public void OnCorrectAnswer()
    {
        Destroy(transform.parent.gameObject, timeToDestroy);
        GetComponent<BoxCollider2D>().enabled = false;
        AnimateRotation();

        GetComponent<SpriteRenderer>().sprite = unLock;
        chains.ReleaseChains();

    }

    private void AnimateRotation()
    {
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        rb2D.isKinematic = false;
        Vector2 force = new Vector2(forceX, forceY);
        float randSide = Random.Range(-1, 1);
        rb2D.AddTorque(torque * Mathf.Sign(randSide));
        rb2D.AddForce(force, ForceMode2D.Impulse);
    }

}
