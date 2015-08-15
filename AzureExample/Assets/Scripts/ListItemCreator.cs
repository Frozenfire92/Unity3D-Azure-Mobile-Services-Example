using UnityEngine;
using System;
using System.Collections.Generic;

public class ListItemCreator : MonoBehaviour
{
    List<ListItem> listItems;
    GameObject listItemPrefab;

    void Awake()
    {
        // Delete the existing template list item(s) we have for designing
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform) { children.Add(child.gameObject); }
        children.ForEach(child => Destroy(child));

        // Setup our list of items and load the prefab
        listItems = new List<ListItem>();
        listItemPrefab = Resources.Load<GameObject>("Prefabs/ListItem");
    }

    public void AddHighScore(Guid? id, string username, int score)
    {
        ListItem li = Instantiate<GameObject>(listItemPrefab).GetComponent<ListItem>();
        li.transform.SetParent(transform);
        li.SetupListItem(id, username, score);
        listItems.Add(li);
    }

    public void ClearScores()
    {
        listItems.Clear();
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform) { children.Add(child.gameObject); }
        children.ForEach(child => Destroy(child));
    }

    public void RemoveItem(Guid? removalId)
    {
        foreach (ListItem item in listItems)
        {
            if (item.id.text == removalId.ToString())
            {
                Destroy(item.gameObject);
                listItems.Remove(item);
                return;
            }
        }
    }
}
