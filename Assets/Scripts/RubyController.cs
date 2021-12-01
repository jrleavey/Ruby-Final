using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;
    public GameObject projectilePrefab;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public ParticleSystem DamageEffect;
    public ParticleSystem HealEffect;
    [SerializeField]
    private bool _isGameOver = false;
    [SerializeField]
    private Text _restartLevelText;
    [SerializeField]
    private Text wintext;
    [SerializeField]
    private Text loseText;
    [SerializeField]
    private Text ammotext;

    public int health { get { return currentHealth; } }
    int currentHealth;
    public float timeInvincible = 2.0f;
    [SerializeField]
    bool isInvincible;
    float invincibleTimer;
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioSource audiosource1;
    [SerializeField]
    AudioSource audiosource2;
    [SerializeField]
    AudioSource audiosource3;
    [SerializeField]
    AudioSource audiosource4;
    [SerializeField]
    private bool _isspeedActive;

    public int FixedRobots;
    public Text FixedText;
    private int ammo = 4;
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        _restartLevelText.gameObject.SetActive(false);
        _isspeedActive = false;
    }
    void Update()
    {
            Movement();
            if (Input.GetKeyDown(KeyCode.C) && ammo > 0)
            {
                Launch();
                ammo--;
                UpdateAmmo();
            }
            if (Input.GetKeyDown(KeyCode.C) && ammo == 0)
        {
            audiosource3.Play();
        }

            if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(0);
        }
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.X) && _isGameOver == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    SceneManager.LoadScene(1);
                }
            }
        }
            if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (_isspeedActive == true)
        {
            speed = 7;
        }
        if (currentHealth == 0)
        {
            speed = 0;
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {

        if (amount < 0)
        {
            if (isInvincible)
                return;
            animator.SetTrigger("Hit");
            PlaySound(hitSound);
            DamageEffect.Play();
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
        else
        {
            HealEffect.Play();
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        if (currentHealth <= 1)
        {
            audiosource4.Play();
        }
        if (currentHealth >= 2)
        {
            audiosource4.Stop();
        }
        if (health <= 0)
        {
            _isGameOver = true;
            GameOverSequence();
        }

    }
    public void ChangeSpeed ()
    {
        speed = 2;
        StartCoroutine(SpeedNormal());
        IEnumerator SpeedNormal()
        {
            yield return new WaitForSeconds(1.0f);
            speed = 4;
        }
    }

    void Launch()
    {
        if (ammo > 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");

            PlaySound(throwSound);
        }
        else
        {
            audiosource3.Play();
        }
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void GameOverSequence()
    {
        AudioSource source1 = GameObject.FindGameObjectWithTag("Bmusic").GetComponent<AudioSource>();
        source1.Stop();
        if (currentHealth == 0)
        {
            loseText.gameObject.SetActive(true);
            audiosource2.Play();
            speed = 0;
        }
        else
        {
            wintext.gameObject.SetActive(true);
            audiosource1.Play();
        }
    }

    void Movement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
    }
   public void UpdateFixed()
    {
        FixedRobots = FixedRobots + 1;
        FixedText.text = "Robots Fixed: " + FixedRobots.ToString();

        if(FixedRobots >= 4)
        {
            _isGameOver = true;
            GameOverSequence();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    public void UpdateAmmo()
    {
        ammotext.text = "Cogs: " + ammo.ToString();
    }
    public void AddAmmo()
    {
        ammo = ammo + 4;
        UpdateAmmo();
    }
    public void SpeedActive()
    {
        _isspeedActive = true;
        speed = 7;
        StartCoroutine(SpeedPowerDownRoutine());

        IEnumerator SpeedPowerDownRoutine()
        {
            yield return new WaitForSeconds(5.0f);
            _isspeedActive = false;
            speed = 4;
        }
    }
}


