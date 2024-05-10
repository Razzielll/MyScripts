using GameDevTV.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterManager : MonoBehaviour, ISaveable
{
    [SerializeField] private SpriteRenderer prisonerSprite;
    [SerializeField] private List<Prisoner> allPrisoners;
    [SerializeField] private bool onlyNewPrisoners = true;
    [SerializeField] private List<Prisoner> lockedPrisoners = new List<Prisoner>();

    private Dictionary<int, List<Prisoner>> lockedPrisonersByTier = new Dictionary<int, List<Prisoner>>();
    private Dictionary<int, List<Prisoner>> allPrisonersByTier = new Dictionary<int, List<Prisoner>>();
    private List<Prisoner> availablePrisoners = new List<Prisoner>();
    private Prisoner currentPrisoner;
    private List<string> characterPacksOpened = new List<string>();
    private Shop shop;
    public event Action OnCharacterOpened;

    // Start is called before the first frame update
    void Start()
    {
       // SetRandomCharacter();
        shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<Shop>();
        UpdateLockedPrisoners();
        UpdateAllPrisonersByTier();
    }

    public void SetCurrentPrisoner(Prisoner prisoner)
    {
        currentPrisoner = prisoner;
        prisonerSprite.sprite = prisoner.GetSprite();
    }

    private void UpdateLockedPrisonersByTier()
    {
        int maxTierLvl = GetComponent<QuestionManager>().GetMaxTierLvl();
        lockedPrisonersByTier.Clear();
        for (int i = 1; i <= maxTierLvl; i++)
        {
            lockedPrisonersByTier[i] = new List<Prisoner>();
            foreach (var prisoner in lockedPrisoners)
            {
                if(prisoner.GetTierLevel() == i)
                {
                    lockedPrisonersByTier[i].Add(prisoner);
                }
            }
        }
    }
    private void UpdateAllPrisonersByTier()
    {
        int maxTierLvl = GetComponent<QuestionManager>().GetMaxTierLvl();
        allPrisonersByTier.Clear();
        for (int i = 1; i <= maxTierLvl; i++)
        {
            allPrisonersByTier[i] = new List<Prisoner>();
            foreach (var prisoner in allPrisoners)
            {
                if(prisoner.GetTierLevel() == i)
                {
                    allPrisonersByTier[i].Add(prisoner);
                }
            }
        }
    }

    public void SetOnlyNewPrisoners(bool togleValue)
    {
        onlyNewPrisoners=togleValue;
    }

    public List<string> GetAllOpenedPacks()
    {
        return characterPacksOpened;
    }

    public void SetRandomCharacter()
    {
        int tierLvl = GameManager.Instance.GetCurrentTier();
        if (onlyNewPrisoners && lockedPrisonersByTier[tierLvl].Count > 0)
        {
            int randNumber = Random.Range(0, lockedPrisonersByTier[tierLvl].Count);
            SetCurrentPrisoner(lockedPrisonersByTier[tierLvl][randNumber]);
        }
        else
        {
            int randNumber = Random.Range(0, allPrisonersByTier[tierLvl].Count);
            SetCurrentPrisoner(allPrisonersByTier[tierLvl][randNumber]);
        }
        
    }

    public Prisoner GetCurrentPrisoner()
    {
        return currentPrisoner;
    }

    public void OpenCharacter(Prisoner prisoner)
    {
        if (availablePrisoners.Contains(prisoner))
        {
            return;
        }
        availablePrisoners.Add(prisoner);
        lockedPrisoners.Remove(prisoner);
        UpdateLockedPrisonersByTier();
        OnCharacterOpened?.Invoke();
    }

    public List<Prisoner> GetAvailableCharacters()
    {
        return availablePrisoners;
    }
    public List<Prisoner> GetAllCharacters()
    {
        return allPrisoners;
    }

    public int GetLockedPrisonersCountByTier(int tierLvl)
    {
        int lockedPrisonerCount = 0;
        foreach (var prisoner in lockedPrisoners)
        {
            if(prisoner.GetTierLevel() == tierLvl)
            {
                lockedPrisonerCount++;
            }
        }
        return lockedPrisonerCount;
    }

    public void AddCharacterPack(PrisonerPackSO pack)
    {
        if (characterPacksOpened.Contains(pack.GetPackName()))
        {
            return;
        }
        allPrisoners.AddRange(pack.GetPrisonersFromPack());
        UpdateLockedPrisoners();
        characterPacksOpened.Add(pack.GetPackName());
        GameManager.Instance.Save();
    }

    [System.Serializable]
    public struct SaveData 
    {
        public List<string> prisonersIDs;
        public List<string> characterPacksOpened;
    }

    public object CaptureState()
    {
        SaveData saveData = new SaveData();        
        saveData.prisonersIDs = availablePrisoners.Select(p => p.GetCharacterID()).ToList(); 
        saveData.characterPacksOpened = characterPacksOpened;
        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = new SaveData();
        saveData = (SaveData)state;
        List<string> prisonersIDs = new List<string>();
        prisonersIDs = saveData.prisonersIDs;
        characterPacksOpened.Clear();
        characterPacksOpened = saveData.characterPacksOpened;
        foreach (string prisonerID in prisonersIDs)
        {
            availablePrisoners.Add(Prisoner.GetFromID(prisonerID));
        }
        shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<Shop>();
        foreach (string characterPack in characterPacksOpened)
        {
            foreach (PrisonerPackSO pack in shop.GetAllCharacterPacks())
            {
                if(characterPack == pack.GetPackName())
                {
                    allPrisoners.AddRange(pack.GetPrisonersFromPack());
                }
            }

        }
        UpdateLockedPrisoners();
    }

    private void UpdateLockedPrisoners()
    {
        lockedPrisoners = allPrisoners.Except(availablePrisoners).ToList();
        UpdateLockedPrisonersByTier();
    }
}
