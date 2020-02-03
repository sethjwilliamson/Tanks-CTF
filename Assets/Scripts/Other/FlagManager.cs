using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 

public class FlagManager : MonoBehaviour
{    
    
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

    // Start is called before the first frame update
    void Start()
    {
        m_SpawnPointBlue =  GameObject.Find("SpawnPointBlue").transform;
        m_SpawnPointRed =   GameObject.Find("SpawnPointRed").transform;

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
            if (other.gameObject.tag == gameObject.tag) {
                if (other.gameObject.transform.Find("WholeFlag").gameObject.activeSelf) {
                    if (gameObject.transform.position == m_SpawnPointBlue.position || gameObject.transform.position == m_SpawnPointRed.position) {
                        if (gameObject.tag == "Red") {
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
                                    
                                }
                            }
                        } else {
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
                                }
                            }
                        }

                        other.gameObject.transform.Find("WholeFlag").gameObject.SetActive(false);

                    } else {
                        if (gameObject.tag == "Red") {
                            gameObject.transform.position = m_SpawnPointRed.position;
                            
                            m_FlagAtBaseRed.SetActive(true);
                            m_FlagDownRed.SetActive(false);
                            m_FlagTakenRed.SetActive(false);

                        } else {
                            gameObject.transform.position = m_SpawnPointBlue.position;

                            m_FlagAtBaseBlue.SetActive(true);
                            m_FlagDownBlue.SetActive(false);
                            m_FlagTakenBlue.SetActive(false);
                        }
                    }
                } else {
                    if (gameObject.transform.position != m_SpawnPointBlue.position || gameObject.transform.position != m_SpawnPointRed.position) {
                        if (gameObject.tag == "Red") {
                            gameObject.transform.position = m_SpawnPointRed.position;
                            
                            m_FlagAtBaseRed.SetActive(true);
                            m_FlagDownRed.SetActive(false);
                            m_FlagTakenRed.SetActive(false);
                        } else {
                            gameObject.transform.position = m_SpawnPointBlue.position;
                            
                            m_FlagAtBaseBlue.SetActive(true);
                            m_FlagDownBlue.SetActive(false);
                            m_FlagTakenBlue.SetActive(false);
                        }
                    }
                }
            } else {
                gameObject.SetActive(false);
                other.gameObject.transform.Find("WholeFlag").gameObject.SetActive(true);

                if (gameObject.tag == "Red") {
                    gameObject.transform.position = m_SpawnPointRed.position;
                    
                    m_FlagAtBaseRed.SetActive(false);
                    m_FlagDownRed.SetActive(false);
                    m_FlagTakenRed.SetActive(true);
                } else {
                    gameObject.transform.position = m_SpawnPointBlue.position;
                    
                    m_FlagAtBaseBlue.SetActive(false);
                    m_FlagDownBlue.SetActive(false);
                    m_FlagTakenBlue.SetActive(true);
                }
            }
        }
    }

    public static void FlagDown(string tag) {
        if (tag == "Red") {
            m_FlagAtBaseBlue.SetActive(false);
            m_FlagDownBlue.SetActive(true);
            m_FlagTakenBlue.SetActive(false);
        } else {
            m_FlagAtBaseRed.SetActive(false);
            m_FlagDownRed.SetActive(true);
            m_FlagTakenRed.SetActive(false);
        }
    }
}
