using UnityEngine;
using System.Collections.Generic;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Board")]
    public Transform gridParent;     // CardsContainer (Grid Layout Group’lu)
    public GameObject cardPrefab;    // Tek Card.prefab
    public Sprite backSprite;        // Ortak arka yüz
    public List<Sprite> fronts;      // Eiffel, Croissant, Macaron, ...


    [Header("Win UI")]
    public GameObject winPanel;       // Canvas altındaki WinPanel (başta inactive olsun)
    public TextMeshProUGUI winText;   // Panel içindeki yazı (opsiyonel)

    [Range(2, 20)] public int pairs = 6;   // kaç çift istiyorsun?
    public float hideDelay = 0.6f;

    public bool InputLocked { get; private set; }

    Card first, second;
    int foundPairs = 0;
    int totalPairs = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Kazanma paneli oyunun başında kapalı kalsın
        if (winPanel) winPanel.SetActive(false);

        BuildBoard();
    }

    void BuildBoard()
    {
        foundPairs = 0;

        // 1) Deste: listedeki ilk 'pairs' sprite'tan ikişer adet
        var deck = new List<(Sprite spr, int id)>();
        int usable = Mathf.Min(pairs, fronts.Count);
        totalPairs = usable;
        for (int i = 0; i < usable; i++)
        {
            deck.Add((fronts[i], i));
            deck.Add((fronts[i], i));
        }

        // 2) Karıştır
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int k = Random.Range(0, i + 1);
            (deck[i], deck[k]) = (deck[k], deck[i]);
        }

        // 3) Instantiate + Init
        foreach (var (spr, id) in deck)
        {
            var go = Instantiate(cardPrefab, gridParent);
            var card = go.GetComponent<Card>();
            card.Init(spr, backSprite, id);
        }
    }

    public void OnCardRevealed(Card c)
    {
        if (first == null) { first = c; return; }
        if (second == null && c != first)
        {
            second = c;
            StartCoroutine(CheckMatch());
        }
    }

    System.Collections.IEnumerator CheckMatch()
    {
        InputLocked = true;

        if (first.Id == second.Id)
        {
            first.SetMatched();
            second.SetMatched();

            foundPairs++;

            if (foundPairs >= totalPairs)
                Win();
        }
        else
        {
            yield return new WaitForSeconds(hideDelay);
            first.Hide();
            second.Hide();
        }

        first = null;
        second = null;
        InputLocked = false;
    }
    public AudioSource winSound;

    void Win()
    {
        if (winText)
            winText.text = "Tebrikler, Paris’e gidiyorsunuz!";

        if (winPanel)
        {
            // Paneli öne getir ve görünür yap
            winPanel.SetActive(true);
            winPanel.transform.SetAsLastSibling(); // z-order en öne
            
            var img = winPanel.GetComponent<UnityEngine.UI.Image>();
            if (img) { var c = img.color; c.a = 0.75f; img.color = c; } // yarı opak arka plan (opsiyonel)
        }
        if (winSound) winSound.Play();


    }
    
}
