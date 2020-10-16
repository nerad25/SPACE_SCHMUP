﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; //singleton

    [Header("Set in Inspector")]
    //These fields control movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;

    //This variable holds a reference to the last triggered gameObject
    private GameObject lastTriggeredGo = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    private void Awake()
    {
        if (S == null)
        {
            S = this;   //set the singleton
        }
        else 
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }

        //fireDelegate += TempFire;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(yAxis*pitchMult,xAxis*rollMult, 0);

        //Allow ship to fire

  //      if (Input.GetKeyDown(KeyCode.Space))
   //     {
    //        TempFire();            
      //  }
        
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }

    /*void TempFire()
    {
        GameObject projGo = Instantiate<GameObject>(projectilePrefab);
        projGo.transform.position = transform.position;
        Rigidbody rigidB = projGo.GetComponent<Rigidbody>();
        //       rigidB.velocity = Vector3.up * projectileSpeed;

        Projectile proj = projGo.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }*/

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);

        //Make sure its not the same triggering go as last time
        if (go == lastTriggeredGo)
        {
            return;
        }
        lastTriggeredGo = go;

        if (go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }

    }

    public float shieldLevel
    {
        get {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            //If the shield is going to be set to less than zero
            if (value < 0)
            {
                Destroy(this.gameObject);
                //Tell main.s to restart the game after a delay
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

}
