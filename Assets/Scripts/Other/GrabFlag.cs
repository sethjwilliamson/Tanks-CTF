using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 

public class GrabFlag : MonoBehaviour
{    
    public Transform m_SpawnPointRed; 
    public Transform m_SpawnPointBlue;  
    // Start is called before the first frame update
    //void Start()
    //{
    //}
//
    //// Update is called once per frame
    //void Update()
    //{
    //    
    //}

	//When a tank touches the flag
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 9) {
            if (other.gameObject.tag == gameObject.tag) {
                Debug.Log("Touches own Flag");
                // Check if player is carrying a flag
                if (other.gameObject.transform.Find("WholeFlag").gameObject.activeSelf) {
                    Debug.Log("Carrying Flag");
                    if (gameObject.transform.position == m_SpawnPointBlue.position || gameObject.transform.position == m_SpawnPointRed.position) {
                        Debug.Log("+1");
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
