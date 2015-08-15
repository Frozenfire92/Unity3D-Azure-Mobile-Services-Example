// Original License in Leaderboard.cs, very long. See https://github.com/bitrave/azure-mobile-services-for-unity3d/issues/32
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Bitrave.Azure;

public class AzureLeaderboard : MonoBehaviour
{
    // Static reference of this class
    public static AzureLeaderboard instance;

    #region PublicInspectorVariables
    [Header("Azure Credentials")]
    public string AzureEndPoint = "https://yourapp.azure-mobile.net/"; // Your Connection URL
    public string ApplicationKey = "NKFDzIEoPQpsQfMasdTYdQTvdMNaxHF23"; // Your API Key

    [Header("UI Text References")]
    public Text endpointText;
    public Text appKeyText;
    public Text createStatusText;
    public Text updateStatusText;

    [Header("UI Input References")]
    public InputField createUsernameInput;
    public InputField createScoreInput;
    public InputField updateUsernameInput;
    public InputField updateScoreInput;

    [Header("UI Button References")]
    public Button createButton;
    public Button deleteButton;
    public Button updateButton;
    public Button allButton;
    public Button button500;
    public Button queryButton;

    [Header("UI Panel References")]
    public GameObject updatePanel;
    #endregion

    #region PrivateVariables
    // Azure instance
    private AzureMobileServices azure;
    
    // Item to insert/create
    private Leaderboard newScore = new Leaderboard()
    {
        Score = 0,
        Username = "Anon"
    };

    // Item to update
    private Leaderboard selectedScore = new Leaderboard()
    {
        Score = 0,
        Username = "Anon"
    };

    // UI Controller that adds/clears our list items
    private ListItemCreator listItemCreator;

    // Triggers to perform action after Azure response has completed
    private bool createComplete = false,
                 readComplete = false,
                 updateComplete = false,
                 deleteComplete = false;

    // List to store items from Azure read operations
    private List<Leaderboard> listToShow;
    #endregion

    #region Initialization
    // Create the static instance of this class
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(gameObject); }
    }

    // Setup the UI, get the list item creator, create azure instance
    void Start()
    {
        updatePanel.SetActive(false);

        endpointText.text = AzureEndPoint;
        appKeyText.text = ApplicationKey;

        createStatusText.text = "";
        updateStatusText.text = "";

        listItemCreator = FindObjectOfType<ListItemCreator>();

        azure = new AzureMobileServices(AzureEndPoint, ApplicationKey);
    }
    #endregion

    #region Update
    // Surely there is a better way, but this will avoid errors of not being able
    // to access Unity objects & UI outside of the main thread (from the azure callbacks)
    void Update()
    {
        if (createComplete)
        {
            createStatusText.text = newScore.Id.ToString();
            listItemCreator.AddHighScore(newScore.Id, newScore.Username, newScore.Score);
            createButton.interactable = true;
            createStatusText.text = "";
            createUsernameInput.text = "";
            createScoreInput.text = "";
            createComplete = false;
        }
        if (readComplete)
        {
            foreach (Leaderboard item in listToShow)
            {
                listItemCreator.AddHighScore(item.Id, item.Username, item.Score);
            }
            SetQueryButtons(true);
            readComplete = false;
        }
        if (updateComplete)
        {
            listItemCreator.AddHighScore(selectedScore.Id, selectedScore.Username, selectedScore.Score);
            ResetUpdate();
            updateComplete = false;
        }
        if (deleteComplete)
        {
            ResetUpdate();
            deleteComplete = false;
        }
    }
    #endregion

    #region Helpers
    public void SetSelectedItem(Guid id, string username, int score)
    {
        selectedScore.Id = id;
        selectedScore.Username = username;
        selectedScore.Score = score;
        updateScoreInput.text = score.ToString();
        updateUsernameInput.text = username;
        updatePanel.SetActive(true);
    }

    public void SetQueryButtons(bool value)
    {
        allButton.interactable = value;
        button500.interactable = value;
        queryButton.interactable = value;
    }

    public void SetUpdateButtons(bool value)
    {
        updateButton.interactable = value;
        deleteButton.interactable = value;
    }

    public void ResetUpdate()
    {
        updateUsernameInput.text = "";
        updateScoreInput.text = "";
        updateStatusText.text = "Done!";
        SetUpdateButtons(true);
        updatePanel.SetActive(false); // This should likely get a coroutine for delay
        updateStatusText.text = "";
    }
    #endregion

    #region AzureCreation
    public void CreateItem()
    {
        createStatusText.text = "Creating...";
        createButton.interactable = false;
        if (createUsernameInput.text == "" || createScoreInput.text == "")
        {
            createStatusText.text = "Invalid input";
            Debug.Log("Invalid input");
            createButton.interactable = true;
        }
        else
        {
            newScore.Username = createUsernameInput.text;
            newScore.Score = Int32.Parse(createScoreInput.text); //This should be TryParse with a proper catch, but the new input forces integers
            if (newScore.Score > 0)
            {
                azure.Insert<Leaderboard>(newScore, CreateHandler);
            }
            else
            {
                createStatusText.text = "Score not > 0";
                Debug.Log("Score must be > 0 to insert");
                createButton.interactable = true;
            }
        }
    }
    #endregion

    #region AzureRetrieval
    public void GetAllItems()
    {
        SetQueryButtons(false);
        listItemCreator.ClearScores();
        azure.Where<Leaderboard>(p => p.Username != null, ReadHandler);
    }

    public void QueryItems()
    {
        SetQueryButtons(false);
        listItemCreator.ClearScores();
        azure.Where<Leaderboard>(p => p.Username == createUsernameInput.text, ReadHandler);
    }

    public void Query500()
    {
        SetQueryButtons(false);
        listItemCreator.ClearScores();
        azure.Where<Leaderboard>(p => p.Score >= 500, ReadHandler);
    }
    #endregion
    
    #region AzureUpdate
    public void UpdateSelectedItem()
    {
        if (updateUsernameInput.text == "" || updateScoreInput.text == "")
        {
            updateStatusText.text = "Invalid input";
            Debug.Log("Invalid input");
        }
        else
        {
            SetUpdateButtons(false);
            updateStatusText.text = "Updating...";
            selectedScore.Username = updateUsernameInput.text;
            selectedScore.Score = Int32.Parse(updateScoreInput.text); //This should be TryParse with a proper catch, but the new input forces integers
            if (selectedScore.Score > 0)
            {
                listItemCreator.RemoveItem(selectedScore.Id);
                azure.Update<Leaderboard>(selectedScore, UpdateHandler);
            }
            else
            {
                Debug.Log("Score must be > 0 to update");
                updateStatusText.text = "Done!";
                SetUpdateButtons(true);
            }
        }
    }

    public void DeletedSelectedItem()
    {
        SetUpdateButtons(false);
        updateStatusText.text = "Deleting...";
        listItemCreator.RemoveItem(selectedScore.Id);
        azure.Delete<Leaderboard>(selectedScore, DeleteHandler);
        deleteComplete = true; // This is a temporary fix until the Delete handler is implemented. See https://github.com/bitrave/azure-mobile-services-for-unity3d/issues/31
    }
    #endregion

    #region AzureResponseHandlers
    public void CreateHandler(AzureResponse<Leaderboard> response)
    {
        Debug.Log("CreateHandler response: " + response.ResponseData.ToString());
        createComplete = true;
    }

    public void ReadHandler(AzureResponse<List<Leaderboard>> response)
    {
        listToShow = response.ResponseData;
        Debug.Log("ReadHandler response: ==================");
        foreach (Leaderboard item in listToShow)
        {
            Debug.Log(Convert.ToString(item.Score) + ", " + item.Username + ", " + item.Id);
        }
        Debug.Log("==================");
        readComplete = true;
    }

    public void UpdateHandler(AzureResponse<Leaderboard> response)
    {
        Debug.Log("UpdateHandler response: " + response.ResponseData.ToString());
        updateComplete = true;
    }

    public void DeleteHandler(AzureResponse<object> response)
    {
        Debug.Log("DeleteHandler response: " + response.ResponseData.ToString());
        deleteComplete = true;
    }
    #endregion
}
