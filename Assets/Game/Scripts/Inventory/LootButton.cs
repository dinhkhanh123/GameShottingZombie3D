using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _buttonName;
    [SerializeField] TextMeshProUGUI _buttonNumber;
    [SerializeField] TextMeshProUGUI _buttonInfo;

    [SerializeField] RawImage _buttonImage;

    string type;
    string namee;
    bool isAccumulative;
    public int id, tempId;
    public int number;
    Texture image;
    InventoryHandler inventoryHandler;

    private void Start()
    {
        inventoryHandler = GameObject.FindWithTag("inventoryHandler").GetComponent<InventoryHandler>();
    }

    public void SetButtonInfo(int _id,int _tempId, string _name, string _type,int _number, Texture _image, bool _isAccumulative, TextMeshProUGUI lootInfo)
    {
        _buttonName.text = _name;  
        _buttonNumber.text = "" + _number;
        _buttonInfo.text = lootInfo.text; 
        _buttonImage.texture = _image;

        id = _id;
        tempId = _tempId;
        isAccumulative = _isAccumulative;
        namee = _name;
        type = _type;
        image = _image;
        number = _number;
    }

}
