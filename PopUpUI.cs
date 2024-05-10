using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI questionTitle;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Transform buttonsPanel;
    [SerializeField] private Button answerButtonPrefab;
    private AudioButtonsUI audioButtons;

    private QuestionSO currentQuestion;

    private void Start()
    {
        audioButtons = GameObject.FindObjectOfType<AudioButtonsUI>();
    }

    private void OnEnable()
    {
        SetupQuestionUI();
      //  StartTimer();
    }

    private void OnDisable()
    {
        GameManager.Instance.StopAnswering();
    }



    private void SetupQuestionUI()
    {
        currentQuestion = GameManager.Instance.GetQuestion();
        if(currentQuestion == null)
        {
            Debug.Log("currentQuestion is null");
            return;
        }
        questionText.text = currentQuestion.GetQuestionText();
        questionTitle.text = currentQuestion.name;
        foreach (Transform child in buttonsPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (string answer in currentQuestion.GetAnswers())
        {
            Button buttonObj = Instantiate(answerButtonPrefab, buttonsPanel);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = answer;
            buttonObj.onClick.AddListener(() => OnButtonPressed(answer));
        }
    }

    private void OnButtonPressed(string answer)
    {
        audioButtons.PlaySound();
        if (answer == currentQuestion.GetRightAnswer())
        {
            GameManager.Instance.ProceedRightAnswer();
        }
        else
        {
            GameManager.Instance.ProccessedWrongAnswer();
        }
        
        gameObject.SetActive(false);
    }

    private void OnMenuOpen()
    {
        PopUpMenu();
    }
    
    public void PopUpMenu()
    {
    //    gameObj.SetActive(!gameObj.activeInHierarchy);
    }
    public void PopUpWindow(GameObject windowGO)
    {
        windowGO.SetActive(!windowGO.activeInHierarchy);
    }
}
