using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private Item item;

    private void Start()
    {
        item = GetComponent<Item>();
    }
    public Item GetItem()
    {
        return item;
    }

}
