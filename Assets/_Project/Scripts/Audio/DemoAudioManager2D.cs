using UnityEngine;

public class DemoAudioManager2D : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip doorClip;
    [SerializeField] private AudioClip shopClip;
    [SerializeField] private AudioClip buffClip;
    [SerializeField] private AudioClip bossPhaseClip;
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip laserClip;
    [SerializeField] private AudioClip keycardClip;
    [SerializeField] private float volume = 0.6f;

    private void Awake()
    {
        EnsureAudioSource();
    }

    public void PlayShoot()
    {
        PlayClip(shootClip);
    }

    public void PlayHit()
    {
        PlayClip(hitClip);
    }

    public void PlayPickup()
    {
        PlayClip(pickupClip);
    }

    public void PlayDoor()
    {
        PlayClip(doorClip);
    }

    public void PlayShop()
    {
        PlayClip(shopClip);
    }

    public void PlayBuff()
    {
        PlayClip(buffClip);
    }

    public void PlayBossPhase()
    {
        PlayClip(bossPhaseClip);
    }

    public void PlayVictory()
    {
        PlayClip(victoryClip);
    }

    public void PlayGameOver()
    {
        PlayClip(gameOverClip);
    }

    public void PlayLaser()
    {
        PlayClip(laserClip);
    }

    public void PlayKeycard()
    {
        PlayClip(keycardClip != null ? keycardClip : pickupClip);
    }

    public void PlayClip(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        EnsureAudioSource();

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
    }

    private void EnsureAudioSource()
    {
        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        sfxSource.playOnAwake = false;
    }

    private void OnValidate()
    {
        volume = Mathf.Clamp01(volume);
    }
}
