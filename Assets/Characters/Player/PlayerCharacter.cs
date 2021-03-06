﻿using UnityEngine;
using System;

using Player;

public class PlayerDamagedEventArgs : EventArgs
{
    public int Health { get; set; }
}

public class PlayerCharacter : MonoBehaviour
{
    private const float FIRETIMERMAX = 60f;
    private const float GROUNDCHECKDIST = 1.1f;
    private const int GROUNDGRACETIME = 5;

    public float moveSpeed;
    public float turnSpeed;
    public float aimingTurnSpeed;
    public float fireRate;

    public float jumpSpeed;
    public float gravity;
    public int dashTime;
    public float dashDistance;

    public int maxHealth = 4;
    public int health;
    public int hitTime;
    public float bounceback;

    public AudioClip projectileSFX;
    public AudioClip hurtSFX;
    public GameObject sword;
    
    // for tuning aim when aiming at nothing
    public float radius = Screen.width/2;

    public Projectile projectileFab;
    public Texture2D crosshairImage;
    public Vector2 reticlePos;
    
    public bool grounded = false;
    public bool attacking = false;
    public bool aiming = false;
    public bool dead = false;

    private int groundgracetimer = 0;
    private PlayerActions actions;
    private IPlayerState state;

    // events
    public event EventHandler<PlayerDamagedEventArgs> PlayerDamaged;

    private void Start()
    {
        reticlePos = new Vector2(Screen.width/2, Screen.height/2);
        state = new MeleeState();
        health = maxHealth;
        actions = PlayerActions.GetInstance();
    }

    private void Update ()
    {
        if (dead)
        {
            return;
        }

        // Check Ground
        RaycastHit hitInfo;
        if (groundgracetimer <= 0 && Physics.Raycast(transform.position, Vector3.down, out hitInfo, GROUNDCHECKDIST, ~LayerMask.NameToLayer("Terrain")))
        {
            grounded = true;
        }
        else
        {
            groundgracetimer--;
            grounded = false;
        }
        

        // State implementation
        var newState = state.HandleTransition(this);
        if (newState != null)
        {
            state.OnExit(this);
            state = newState;
            actions = state.OnEnter(this);
        }

        state.HandleUpdate(this);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            OnPlayerDamaged();
        }
    }

    private void OnGUI()
    {
        if (aiming)
        {
            float xMin = reticlePos.x - (crosshairImage.width / 2);
            float yMin = (Screen.height - reticlePos.y) - (crosshairImage.height / 2);
            GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
        }
    }

    public void OnPlayerDamaged()
    {
        state.HandleHit(this);
        if (PlayerDamaged != null)
            PlayerDamaged(this, new PlayerDamagedEventArgs() { Health = health });
    }

    public PlayerActions Actions
    {
        get { return actions; }
    }

    public void LeaveGround()
    {
        this.grounded = false;
        this.groundgracetimer = GROUNDGRACETIME;
    }
}
