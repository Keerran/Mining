using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Inventory : ScriptableObject
{
    private static Inventory _instance;
    public static Inventory instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.FindObjectsOfTypeAll<Inventory>().FirstOrDefault();

            #if UNITY_EDITOR
            if(!_instance)
                _instance = CreateInstance<Inventory>();
            #endif

            return _instance;
        }
    }

    private Dictionary<InventoryItem, ItemStack> _stacks = new Dictionary<InventoryItem, ItemStack>();
    [field: SerializeField]
    public List<ItemStack> items { get; private set; } = new List<ItemStack>();

    public void LoadItems(SaveData save)
    {
        var allItems = AssetDatabase.FindAssets("t:InventoryItem").Select(AssetDatabase.GUIDToAssetPath)
                                    .Select(AssetDatabase.LoadAssetAtPath<InventoryItem>);
        var itemDict = allItems.ToDictionary(item => (item.id, item));
        items = new List<ItemStack>();
        foreach(var (id, amount) in save.itemIds)
        {
            AddItem(itemDict[id], amount);
        }
    }

    public void AddItem(InventoryItem item, int amount = 1)
    {
        if (_stacks.TryGetValue(item, out ItemStack value))
            value.Add(amount);
        else
        {
            var stack = new ItemStack(item, amount);
            _stacks.Add(item, stack);
            items.Add(stack);
        }
    }

    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Window/Inventory")]
    public static void ShowInventory()
    {
        UnityEditor.Selection.activeObject = instance;
    }
    #endif
}
