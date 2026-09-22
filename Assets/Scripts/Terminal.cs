using UnityEngine;
using TMPro;
using System.Data.Common;
using System.Drawing.Text;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class Terminal : MonoBehaviour
{
    [SerializeField]
    private int termid = 0;
    private Backend backend;
    public int Id => termid;
    [SerializeField]
    private TMP_Text uiText;
    private TermEnvironment term;
    private Quaternion playerCamAngle;
    private Vector3 playerCamPos;
    [SerializeField]
    private Camera termcam;
    [SerializeField]
    private Quaternion termCamAngle;
    [SerializeField]
    private Vector3 termCamPos;
    public List<String> keyspressed=new List<String>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject hierarchyObject= GameObject.FindWithTag("TerminalBackend");
        backend = hierarchyObject.GetComponent<Backend>();
        if (!Backend.termEnv.termExists(termid)){
            term = new TermEnvironment();
            term.id = termid;
        }

        //fetch your termid and grab your config + harddisk info/powerstate/memory state if you have any
    }
    // void OnEnable()
    // {
        
    // }
    // void OnDisable()
    // {
        
    // }

    public void Focus(Transform cam)
    {
        playerCamAngle = cam.rotation;
        playerCamPos = cam.position;
        Debug.Log($"Moving camera to pos: {termCamPos} and camera to rot: {termCamAngle}");
        // cam.rotation = Quaternion.Lerp(cam.rotation, termCamAngle, 20f*Time.deltaTime);
        // cam.position = Vector3.Lerp(cam.position, termCamPos, 8f*Time.deltaTime);
        cam.rotation = termCamAngle;
        cam.position = termCamPos;
    }
    // Update is called once per frame
    public void Unfocus(Transform cam)
    {
        cam.rotation = playerCamAngle;
        cam.position = playerCamPos;
    }
    // Update is called once per frame
    void Update()
    {
        if (keyspressed.Count > 0)
        {
            String pressed="";
            foreach (String p in keyspressed)
            {
                pressed+=p;
            }
            Debug.Log(pressed);
        }
    }
    void FixedUpdate()
    {
        
    }

    public void processInput(InputControl keyName)
    {
        keyspressed.Add(keyName.name);
        Debug.Log(keyName.name);
        Debug.Log(keyspressed.Count);
    }
}