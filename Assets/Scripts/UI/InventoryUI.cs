using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsPanel;
    public GameObject item;

    void OnEnable()
    {
        foreach(var (idx, stack) in Inventory.instance.items.Enumerate())
        {
            var go = Instantiate(item, Vector3.zero, Quaternion.identity, itemsPanel);

            var rect = go.GetComponent<RectTransform>();
            var y = Math.DivRem(idx, 4, out var x);
            rect.anchoredPosition = new Vector3(x * 120, -y * 120, 0);

            var img = go.GetComponent<Image>();
            img.sprite = stack.item.image;

            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            text.text = stack.size.ToString();
        }
    }

    void OnDisable()
    {
        foreach(Transform child in itemsPanel)
        {
            Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
