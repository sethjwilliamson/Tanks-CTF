using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 

public class FlagManager : MonoBehaviour
{    
    [HideInInspector] public Transform m_SpawnPointRed; 
    [HideInInspector] public Transform m_SpawnPointBlue;  

    // Start is called before the first frame update
    void Start()
    {
        m_SpawnPointBlue = GameObject.Find("SpawnPointBlue").transform;
        m_SpawnPointRed = GameObject.Find("SpawnPointRed").transform;
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 9) {
            if (other.gameObject.tag == gameObject.tag) {
                Debug.Log("Touches own Flag");
                // Check if player is carrying a flag
                if (other.gameObject.transform.Find("WholeFlag").gameObject.activeSelf) {
                    Debug.Log("Carrying Flag");
                    if (gameObject.transform.position == m_SpawnPointBlue.position || gameObject.transform.position == m_SpawnPointRed.position) {
                        Debug.Log("+1");
                        
                        if (gameObject.tag == "Red") {
                            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
                                if (go.name == "Blue Flag") {
                                    Debug.Log(go.name);
                                    go.gameObject.transform.position = m_SpawnPointBlue.position;
                                    go.gameObject.SetActive(true);
                                    GameManager.blueCaptures++;
                                }
                            }
                        } else {
                            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
                                if (go.name == "Red Flag") {
                                    Debug.Log(go.name);
                                    go.gameObject.transform.position = m_SpawnPointRed.position;
                                    go.gameObject.SetActive(true);
                                    GameManager.redCaptures++;
                                }
                            }
                        }

                        other.gameObject.transform.Find("WholeFlag").gameObject.SetActive(false);

                    } else {
                        if (gameObject.tag == "Red") {
                            gameObject.transform.position = m_SpawnPointRed.position;
                        } else {
                            gameObject.transform.position = m_SpawnPointBlue.position;
                        }
                    }
                } else {
                    Debug.Log("Not Carrying Flag");
                    if (gameObject.transform.position != m_SpawnPointBlue.position || gameObject.transform.position != m_SpawnPointRed.position) {
                        if (gameObject.tag == "Red") {
                            gameObject.transform.position = m_SpawnPointRed.position;
                        } else {
                            gameObject.transform.position = m_SpawnPointBlue.position;
                        }
                    }
                }
            } else {
                Debug.Log("Touches other Flag");
                gameObject.SetActive(false);
                other.gameObject.transform.Find("WholeFlag").gameObject.SetActive(true);
            }
        }
    }

    
}
