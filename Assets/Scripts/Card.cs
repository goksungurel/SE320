using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Card : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] AudioClip wrongSound;

    Sprite frontSprite;
    Sprite backSprite;
    int pairId;

    bool revealed = false;
    bool matched  = false;

    Animator animator;
    AudioSource audioSource;

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
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnClick);

        if (image == null)
            image = GetComponentInChildren<Image>();

        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 🔴 2D SES
        audioSource.volume = 1f;
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
        if (animator != null)
            animator.SetTrigger("Pop");
    }

    public void FlashWrong()
    {
        if (wrongSound != null)
            audioSource.PlayOneShot(wrongSound);

        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        Color originalColor = image.color;
        image.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        image.color = originalColor;
    }

    void ShowBack()
    {
        image.sprite = backSprite;
    }

    public int  Id       => pairId;
    public bool Revealed => revealed;
    public bool Matched  => matched;
}
