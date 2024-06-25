using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Looting : MonoBehaviour
{
    List<int> lootList;
    List<GameObject> loot;
    int itemId, itemTempId, tempId;
    string itemType, itemName;
    Texture itemTexture;
    bool isItemAccumulate;
    int itemNumber;

    [SerializeField] GameObject lootButton;
    InventoryHandler inventoryHandler;
    bool isThereItems;
    BoxCollider boxCollider;
    TextMeshProUGUI lootInfo;

    private void Start()
    {
        inventoryHandler = GameObject.FindWithTag("inventoryHandler").GetComponent<InventoryHandler>();
        lootList = new List<int>();
        loot = new List<GameObject>();
        tempId = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("loot"))
        {
            LootItem lootItem = other.GetComponent<LootItem>();

            itemId = lootItem._ID;
            itemTempId = lootItem._tempId;
            itemType = lootItem._type;
            itemName = lootItem._name;
            isItemAccumulate = lootItem._isAccumulative; 
            itemNumber = lootItem._number;  
            itemTexture = lootItem._texture;
            lootInfo = lootItem.itemInfo;


            lootItem.SetTempId(itemId);
            itemTempId = lootItem.tempId;

            lootList.Add(itemId);
            loot.Add(other.gameObject);
            inventoryHandler.InstantiateLootButton(itemId, itemTempId, itemName, itemType, itemNumber, itemTexture, isItemAccumulate, lootInfo);
            tempId++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("loot"))
        {
            LootItem lootItem = other.GetComponent<LootItem>();

            itemTempId = lootItem._ID;
            lootList.Remove(itemId);

            for(int i = 0; i < loot.Count; i++)
            {
                int lootId = loot[i].GetComponent<LootItem>()._ID;   
                if(itemId == lootId)
                {
                    loot.Remove(loot[i]);
                }
            }
            GameObject[] buttons = GameObject.FindGameObjectsWithTag("lootButton");
            for(int i = 0; i< buttons.Length; i++)
            {
                int butId = buttons[i].GetComponent<LootButton>().id;

                if(butId == itemId)
                {
                    GameObject.Destroy(buttons[i]);
                    return;
                }
            }
        }
    }

}
