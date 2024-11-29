using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [SerializeField]
    private int startingGold; // Default starting gold
    private int currentGold;

    public int CurrentGold => currentGold; // Property to get the current gold

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentGold = startingGold;
        UpdateGoldUI(); // Update the UI when the game starts
    }

    public void EarnGold(float amount)
    {
        currentGold += Mathf.RoundToInt(amount);
        UpdateGoldUI();
        //Debug.Log($"Gold earned! Current gold: {currentGold}");
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount) {
            currentGold -= amount;
            UpdateGoldUI();
            Debug.Log($"Spent {amount} gold. Remaining gold: {currentGold}");
            return true; // Successful purchase
        } else {
            Debug.LogWarning("Not enough gold!");
            return false; // Failed purchase
        }
    }

    private void UpdateGoldUI()
    {
        GoldUI goldUI = FindObjectOfType<GoldUI>(); // Find the GoldUI script in the scene
        if (goldUI != null) {
            goldUI.UpdateGoldText(currentGold);
        }
    }
}
