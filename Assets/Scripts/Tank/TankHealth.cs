using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    public GameObject m_BrokenTank;
    public AudioClip m_RedFlagDropped;
    public AudioClip m_BlueFlagDropped;
    
    
    [HideInInspector] private GameManager gm;  
    //[HideInInspector] private FlagManager fm;  
    [HideInInspector] public Transform m_SpawnPointRed; 
    [HideInInspector] public Transform m_SpawnPointBlue;  
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            
    private TankMovement m_Movement;
    private TankShooting m_Shooting;

    void Start()
    {
        m_SpawnPointBlue = GameObject.Find("SpawnPointBlue").transform;
        m_SpawnPointRed = GameObject.Find("SpawnPointRed").transform;

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();

        if (gameObject.tag == "Red") {
            gameObject.transform.position = m_SpawnPointRed.position;
        } else {
            gameObject.transform.position = m_SpawnPointBlue.position;
        }

        gameObject.GetComponent<Collider>().enabled = true;
    }

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        m_CurrentHealth -= amount;

        SetHealthUI();

        if (m_CurrentHealth <= 0f && !m_Dead) {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.

        m_Slider.value = m_CurrentHealth;

        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth); /// Change this to include yellow in middle
        // https://docs.unity3d.com/ScriptReference/Gradient.html
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.

        m_Dead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();
        

        StartCoroutine(Respawn(5));

    }

    
    IEnumerator Respawn(float spawnDelay)
     {
        m_Movement = gameObject.GetComponent<TankMovement>();
        m_Shooting = gameObject.GetComponent<TankShooting>();

        m_Movement.disable();
        m_Shooting.enabled = false;

        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        gameObject.transform.Find("Canvas").gameObject.SetActive(false);

        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        Color color = renderers[0].material.color;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].gameObject.SetActive(false);
        }

        GameObject brokenTank = Instantiate(m_BrokenTank, transform.position, transform.rotation);

        MeshRenderer[] brokenRenderers = brokenTank.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < brokenRenderers.Length; i++)
        {
            brokenRenderers[i].material.color = color;
        }


        if (gameObject.transform.Find("WholeFlag").gameObject.activeSelf) {
            if (gameObject.tag == "Red") {
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
                    if (go.name == "Blue Flag") {
                        gm.BroadcastMessage("FlagDrop", "Blue");
                        gm.SendMessage("PlayAudio", m_BlueFlagDropped);
                        
                        go.gameObject.transform.position = transform.position;
                        go.gameObject.SetActive(true);
                    }
                }
            } else {
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
                    if (go.name == "Red Flag") {
                        gm.SendMessage("FlagDrop", "Red");
                        gm.SendMessage("PlayAudio", m_RedFlagDropped);

                        go.gameObject.transform.position = transform.position;
                        go.gameObject.SetActive(true);
                    }
                }
            }
        }
        
        gameObject.GetComponent<Collider>().enabled = false;

        gameObject.transform.Find("WholeFlag").gameObject.SetActive(false);


        yield return new WaitForSeconds(spawnDelay);

        gameObject.transform.Find("Canvas").gameObject.SetActive(true);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].gameObject.SetActive(true);
        }
        
        Destroy(brokenTank);

        m_Movement.enable();
        m_Shooting.enabled = true;

        
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        OnEnable();
     }

}