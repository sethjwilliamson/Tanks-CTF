using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 

public class FlagManager : MonoBehaviour
{    
    
    //public static AudioSource m_Announcer;  
    public AudioClip m_RedScore;   
    public AudioClip m_BlueScore;   
    public AudioClip m_RedFlagTaken; 
    public AudioClip m_BlueFlagTaken;   
    public AudioClip m_RedFlagReturned;   
    public AudioClip m_BlueFlagReturned;   
    [HideInInspector] public Transform m_SpawnPointRed; 
    [HideInInspector] public Transform m_SpawnPointBlue;  
    [HideInInspector] private static Text m_ScoreRed; 
    [HideInInspector] private static Text m_ScoreBlue;  
    [HideInInspector] private static GameObject m_FlagDownRed; 
    [HideInInspector] private static GameObject m_FlagDownBlue;  
    [HideInInspector] private static GameObject m_FlagAtBaseRed; 
    [HideInInspector] private static GameObject m_FlagAtBaseBlue;  
    [HideInInspector] private static GameObject m_FlagTakenRed; 
    [HideInInspector] private static GameObject m_FlagTakenBlue; 
    [HideInInspector] private GameManager gm;  

    void Start()
    {
        m_SpawnPointBlue =  GameObject.Find("SpawnPointBlue").transform;
        m_SpawnPointRed =   GameObject.Find("SpawnPointRed").transform;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        Transform mc = GameObject.Find("MessageCanvas").transform;

        foreach (Transform child in mc) {
            switch (child.name) {
                case "ScoreRed":
                    m_ScoreRed = child.gameObject.GetComponent<Text>();
                    break;
                case "ScoreBlue":
                    m_ScoreBlue = child.gameObject.GetComponent<Text>();
                    break;
                case "FlagDownRed":
                    m_FlagDownRed = child.gameObject;
                    break;
                case "FlagDownBlue":
                    m_FlagDownBlue = child.gameObject;
                    break;
                case "FlagAtBaseRed":
                    m_FlagAtBaseRed = child.gameObject;
                    break;
                case "FlagAtBaseBlue":
                    m_FlagAtBaseBlue = child.gameObject;
                    break;
                case "FlagTakenBlue":
                    m_FlagTakenBlue = child.gameObject;
                    break;
                case "FlagTakenRed":
                    m_FlagTakenRed = child.gameObject;
                    break;
            }
        }

        m_FlagDownRed.SetActive(false);
        m_FlagDownBlue.SetActive(false);
        m_FlagAtBaseRed.SetActive(true);
        m_FlagAtBaseBlue.SetActive(true);
        m_FlagTakenRed.SetActive(false);
        m_FlagTakenBlue.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 9) {
        // Collision is with Player
            if (other.gameObject.tag == gameObject.tag) {
            // Same Team
                if (other.gameObject.transform.Find("WholeFlag").gameObject.activeSelf) {
                // Carrying a flag
                    if (gameObject.transform.position == m_SpawnPointBlue.position || gameObject.transform.position == m_SpawnPointRed.position) {
                    // Team's flag is at base
                        if (gameObject.tag == "Red") {
                        // This flag (this game object) is red
                            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
                                if (go.name == "Blue Flag") {
                                    go.gameObject.transform.position = m_SpawnPointBlue.position;
                                    go.gameObject.SetActive(true);
                                    GameManager.redCaptures++;

                                    m_ScoreRed.GetComponent<Text>().text = GameManager.redCaptures.ToString();
                                    
                                    Debug.Log("Red: " + GameManager.redCaptures);

                                    m_FlagAtBaseBlue.SetActive(true);
                                    m_FlagDownBlue.SetActive(false);
                                    m_FlagTakenBlue.SetActive(false);
                                    
                                    /// Red Score
                                    gm.SendMessage("PlayAudio", m_RedScore);

                                    /// Takes lead ?
                                }
                            }
                        } else {
                        // This flag (this game object) is blue
                            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
                                if (go.name == "Red Flag") {
                                    go.gameObject.transform.position = m_SpawnPointRed.position;
                                    go.gameObject.SetActive(true);
                                    GameManager.blueCaptures++;

                                    //Debug.Log(m_ScoreRed.GetComponent<Text>().text);

                                    m_ScoreBlue.GetComponent<Text>().text = GameManager.blueCaptures.ToString();

                                    Debug.Log("Blue: " + GameManager.blueCaptures);

                                    m_FlagAtBaseRed.SetActive(true);
                                    m_FlagDownRed.SetActive(false);
                                    m_FlagTakenRed.SetActive(false);

                                    /// Blue Score
                                    gm.SendMessage("PlayAudio", m_BlueScore);

                                    /// Takes lead ?
                                }
                            }
                        }

                        other.gameObject.transform.Find("WholeFlag").gameObject.SetActive(false);

                    } else {
                    // Flag is not at base
                        if (gameObject.tag == "Red") {
                            gameObject.transform.position = m_SpawnPointRed.position;
                            
                            m_FlagAtBaseRed.SetActive(true);
                            m_FlagDownRed.SetActive(false);
                            m_FlagTakenRed.SetActive(false);

                            /// Red Flag Returned
                            gm.SendMessage("PlayAudio", m_RedFlagReturned);

                        } else {
                            gameObject.transform.position = m_SpawnPointBlue.position;

                            m_FlagAtBaseBlue.SetActive(true);
                            m_FlagDownBlue.SetActive(false);
                            m_FlagTakenBlue.SetActive(false);

                            /// Blue Flag Returned
                            gm.SendMessage("PlayAudio", m_BlueFlagReturned);
                        }
                    }
                } else {
                // Not carrying a flag
                    if (gameObject.tag == "Red") {                        
                        if (gameObject.transform.position != m_SpawnPointRed.position) {
                            gm.SendMessage("PlayAudio", m_RedFlagReturned);
                        }

                        gameObject.transform.position = m_SpawnPointRed.position;
                        
                        m_FlagAtBaseRed.SetActive(true);
                        m_FlagDownRed.SetActive(false);
                        m_FlagTakenRed.SetActive(false);

                        /// Red Flag Returned
                    } else {
                        if (gameObject.transform.position != m_SpawnPointBlue.position) {
                            gm.SendMessage("PlayAudio", m_BlueFlagReturned);
                        }

                        gameObject.transform.position = m_SpawnPointBlue.position;
                        
                        m_FlagAtBaseBlue.SetActive(true);
                        m_FlagDownBlue.SetActive(false);
                        m_FlagTakenBlue.SetActive(false);

                        /// Blue Flag Returned
                    }
                }
            } else {
            // Other Team
                gameObject.SetActive(false);
                other.gameObject.transform.Find("WholeFlag").gameObject.SetActive(true);

                if (gameObject.tag == "Red") {
                    gameObject.transform.position = m_SpawnPointRed.position;
                    
                    m_FlagAtBaseRed.SetActive(false);
                    m_FlagDownRed.SetActive(false);
                    m_FlagTakenRed.SetActive(true);

                    /// Red Flag Taken
                    gm.SendMessage("PlayAudio", m_RedFlagTaken);
                } else {
                    gameObject.transform.position = m_SpawnPointBlue.position;
                    
                    m_FlagAtBaseBlue.SetActive(false);
                    m_FlagDownBlue.SetActive(false);
                    m_FlagTakenBlue.SetActive(true);

                    /// Blue Flag Taken
                    gm.SendMessage("PlayAudio", m_BlueFlagTaken);
                }
            }
        }
    }

}
