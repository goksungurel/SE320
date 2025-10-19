using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Board")]
    public Transform gridParent;     // CardsContainer (Grid Layout Group’lu)
    public GameObject cardPrefab;    // Tek Card.prefab
    public Sprite backSprite;        // Ortak arka yüz
    public List<Sprite> fronts;      // Eiffel, Croissant, Macaron, ...

    [Range(2, 20)] public int pairs = 6;   // kaç çift istiyorsun?
    public float hideDelay = 0.6f;

    public bool InputLocked { get; private set; }

    Card first, second;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => BuildBoard();

    void BuildBoard()
    {
        // 1) Deste: listedeki ilk 'pairs' sprite'tan ikişer adet
        var deck = new List<(Sprite spr, int id)>();
        int usable = Mathf.Min(pairs, fronts.Count);
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
            var go   = Instantiate(cardPrefab, gridParent);
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
}
