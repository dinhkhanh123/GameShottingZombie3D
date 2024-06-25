using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LootItem : MonoBehaviour
{
    [SerializeField] public int _ID;
    [SerializeField] public int _tempId;
    [SerializeField] public string _name;
    [SerializeField] public string _type;
    [SerializeField] public bool _isAccumulative;
    [SerializeField] public int _number;
    [SerializeField] public Texture _texture;
    [SerializeField] public TextMeshProUGUI itemInfo;

    public int tempId = 0;
    public Vector3 holsterRot;
    public Vector3 handRot;
    public Vector3 groundRot;

    public void SetTempId(int _num)
    {
        _tempId = _num;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && UIManager.instance != null)
        {
            UIManager.instance.healthCount++;
            UIManager.instance.UpdateHealthCounter();
            Destroy(gameObject); 
        }
    }
}
