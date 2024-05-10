
using System;
using UnityEngine;

public class GoalPanelUI : MonoBehaviour
{
    [SerializeField] private CharacterGoalSO[] goals;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private Transform goalContainer;
    [SerializeField] private Transform goalReward;

    private void OnEnable()
    {
        ClearContainer(goalContainer);
        SpawnGoals();
    }

    private void ClearContainer(Transform container)
    {
        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }
    }

    private void SpawnGoals()
    {
        foreach (var goal in goals)
        {
            GameObject goalGO = Instantiate(goalPrefab, goalContainer);
            SetupGoalUI(goalGO, goal);
        }
    }

    private void SetupGoalUI(GameObject goalGO, CharacterGoalSO goalSO)
    {
        GoalUI goalUI = goalGO.GetComponent<GoalUI>();
        goalUI.SetGoalDescription(goalSO.GetDescription());
        goalUI.SetInteractabilityClaimButton(goalSO.IsCompleted() && !IsClaimed(goalSO.GetName()));
        goalUI.SetGoalName(goalSO.GetName());
        goalUI.UpdateProgressBar(goalSO.GetProgress());
        goalUI.SetRewardQuantity(goalSO.GetRewardQuantity());
        goalUI.SetRewardPopUp(goalReward);
    }

    private bool IsClaimed(string goalName)
    {
        foreach (string item in Blackboard.Instance.GetCompletedGoals())
        {
            if(item == goalName)
            {
                return true;
            }
        }
        return false;
    }
}
