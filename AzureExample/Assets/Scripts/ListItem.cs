using System;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    public Text id, username, score;

    public void SetupListItem(Guid? id, string username, int score)
    {
        this.id.text = id.ToString();
        this.username.text = username;
        this.score.text = score.ToString();
    }

    public void EditListItem()
    {
        AzureLeaderboard.instance.SetSelectedItem(new Guid(id.text),
                                                  username.text,
                                                  Int32.Parse(score.text)); //This should be TryParse with a proper catch, but the new input forces integers
    }
}
