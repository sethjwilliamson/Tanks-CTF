﻿using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor; 
    //Temporary
    public Color m_OtherColor;
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;                     


    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        if (m_PlayerNumber % 2 == 0) {
            m_Instance.tag = "Red";
        } else {
            m_Instance.tag = "Blue";
        }

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].name == "Pole") {
                continue;
            } else if (renderers[i].name == "Flag") {
                renderers[i].material.color = m_OtherColor;
            } else {
                renderers[i].material.color = m_PlayerColor;
            }
        }

        m_Instance.transform.Find("WholeFlag").gameObject.SetActive(false);

        //GameObject[] children = m_Instance.GetComponentsInChildren<GameObject>();
//
        //for (int i = 0; i < children.Length; i++) {
        //    if (children[i].name == "WholeFlag") {
        //        children[i].SetActive(false);
        //    }
        //}

    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
