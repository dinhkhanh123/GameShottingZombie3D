using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] GameObject lootButton;
    [SerializeField] Transform lootButtonsHolder;
   public void InstantiateLootButton(int itemId,int tempId,string itemName,string itemType,int itemNumber,Texture itemTexture,bool isAccumulative,TextMeshProUGUI lootInfo)
    {
        var buttonInstance = Instantiate(lootButton, lootButtonsHolder);
        buttonInstance.transform.SetParent(lootButtonsHolder);

        buttonInstance.GetComponent<LootButton>().SetButtonInfo(itemId, tempId, itemName, itemType, itemNumber, itemTexture, isAccumulative, lootInfo);
    }
}
