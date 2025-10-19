using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] Image image;        // Kartın Image bileşeni (Button üstündeki Image)
    Sprite frontSprite;
    Sprite backSprite;
    int pairId;
    bool revealed = false;
    bool matched  = false;

    public void Init(Sprite front, Sprite back, int id)
    {
        frontSprite = front;
        backSprite  = back;
        pairId = id;
        ShowBack();
        matched = false;
        revealed = false;
    }

    void Awake()
    {
        // Button tıklamasını bu karta bağla
        var btn = GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(OnClick);
        if (image == null) image = GetComponent<Image>();
    }

    void OnClick()
    {
        if (matched || revealed || GameManager.Instance.InputLocked) return;
        Reveal();
        GameManager.Instance.OnCardRevealed(this);
    }

    public void Reveal()
    {
        revealed = true;
        image.sprite = frontSprite;
    }

    public void Hide()
    {
        revealed = false;
        image.sprite = backSprite;
    }

    public void SetMatched()
    {
        matched = true;
        // İstersen butonu kapat: GetComponent<Button>().interactable = false;
    }

    void ShowBack()
    {
        image.sprite = backSprite;
    }

    public int  Id        => pairId;
    public bool Revealed  => revealed;
    public bool Matched   => matched;
}
