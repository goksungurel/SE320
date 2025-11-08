using UnityEngine;
using UnityEngine.EventSystems;

public class CardFlipSound : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioSource source;
    [Range(0.9f, 1.1f)] public float minPitch = 0.96f;
    [Range(0.9f, 1.1f)] public float maxPitch = 1.04f;
    [Range(0f, 0.2f)]  public float cooldown = 0.05f;

    private float _lastPlay = -999f;

    void Reset() {
        source = GetComponent<AudioSource>();
        if (!source) source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D
        source.dopplerLevel = 0f;
    }

    public void OnPointerClick(PointerEventData e) {
        OnCardFlipped();
    }

    public void OnCardFlipped() {
        if (Time.unscaledTime - _lastPlay < cooldown) return;
        _lastPlay = Time.unscaledTime;
        if (clips == null || clips.Length == 0 || source == null) return;

        int i = Random.Range(0, clips.Length);
        source.pitch = Random.Range(minPitch, maxPitch);
        source.PlayOneShot(clips[i]);
    }

    // Animator event’inde kullanmak istersen
    public void PlayFlipSfx() => OnCardFlipped();
}
