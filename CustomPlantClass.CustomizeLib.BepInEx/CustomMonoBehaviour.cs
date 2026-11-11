
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Random = System.Random;

namespace CustomPlantClass.CustomizeLib;

public class CheckCardState : MonoBehaviour
{
    public GameObject? card = null;
    public PlantType cardType = PlantType.Nothing;
    public CardUI? movingCardUI = null;
    public bool isNormalCard = false;

    public void Start()
    {
        CustomCore.checkBehaviours.Add(this);
    }

    public void CheckState()
    {
        if (card is null || movingCardUI is null)
            return;

        List<PlantType> cardsOnSeedBank = new List<PlantType>();
        Dictionary<PlantType, List<bool>> cardsOnSeedBankExtra = new Dictionary<PlantType, List<bool>>();
        GameObject? seedGroup = null;
        if (Board.Instance != null && !Board.Instance.boardTag.isIZ)
            seedGroup = InGameUI.Instance.SeedBank.transform.GetChild(0).gameObject;
        else if (Board.Instance != null && Board.Instance.boardTag.isIZ)
            seedGroup = InGameUI_IZ.Instance.transform.FindChild("SeedBank/SeedGroup").gameObject;
        if (seedGroup == null)
            return;
        for (int i = 0; i < seedGroup.transform.childCount; i++)
        {
            GameObject seed = seedGroup.transform.GetChild(i).gameObject;
            if (seed.transform.childCount > 0)
            {
                cardsOnSeedBank.Add(seed.transform.GetChild(0).GetComponent<CardUI>().thePlantType);
                if (!cardsOnSeedBankExtra.ContainsKey(seed.transform.GetChild(0).GetComponent<CardUI>().thePlantType))
                    cardsOnSeedBankExtra.Add(seed.transform.GetChild(0).GetComponent<CardUI>().thePlantType, new List<bool>() { seed.transform.GetChild(0).GetComponent<CardUI>().isExtra });
                else
                    cardsOnSeedBankExtra[seed.transform.GetChild(0).GetComponent<CardUI>().thePlantType].Add(seed.transform.GetChild(0).GetComponent<CardUI>().isExtra);
            }
        }

        if (card != movingCardUI && card.transform.childCount >= 2 && movingCardUI.thePlantType == cardType && !isNormalCard)
        {
            if (cardsOnSeedBank.Contains(cardType))
                card.transform.GetChild(1).gameObject.SetActive(false);
            else
                card.transform.GetChild(1).gameObject.SetActive(true);
        }
        if (card != movingCardUI && card.transform.childCount >= 3 && movingCardUI.thePlantType == cardType && isNormalCard)
        {

            if (cardsOnSeedBankExtra.ContainsKey(cardType) && cardsOnSeedBankExtra[cardType].Contains(true))
                card.transform.GetChild(1).gameObject.SetActive(false);
            else
                card.transform.GetChild(1).gameObject.SetActive(true);
            if (cardsOnSeedBankExtra.ContainsKey(cardType) && cardsOnSeedBankExtra[cardType].Contains(false))
                card.transform.GetChild(2).gameObject.SetActive(false);
            else
                card.transform.GetChild(2).gameObject.SetActive(true);
        }
    }
}