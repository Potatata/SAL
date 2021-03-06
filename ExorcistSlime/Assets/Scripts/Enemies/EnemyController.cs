﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : CharacterController
{
    //Constants
    public const float INVINCIBILITY_TIME = 2f;
    public float SPEED_INCREASE_TAUNT;

    public float shootingTimer;
    public GameObject bulletPrefab;
    protected List<BulletType> bullets;
    public bool taunted;
    protected Transform firePoint;
    protected PlayerController player;
    public bool isInvincible;
    private SceneController sceneController; 

    public virtual void Awake()
    {
        health = new Health();
        bullets = new List<BulletType>() { };
        firePoint = transform.Find("FirePoint").transform;
        player = FindObjectOfType<PlayerController>();
        sceneController = FindObjectOfType<SceneController>();
        taunted = isInvincible = false;
    }

    public virtual void Start()
    {
        StartCoroutine(Shoot());
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Move();
    }

    /// <summary>
    /// The movement of the enemy.
    /// </summary>
    public override void Move()
    {
        if (taunted)
            InTaunt(this, player);
        else
            MovePattern(this, player);
    }

    /// <summary>
    /// The type of enemy pattern
    /// </summary>
    /// <param name="enemy">This enemy.</param>
    /// <param name="player">Where the player is at.</param>
    protected abstract void MovePattern(EnemyController enemy, PlayerController player);


    /// <summary>
    /// When the enemy is taunted. Follows the player.
    /// </summary>
    public void InTaunt(EnemyController enemy, PlayerController player)
    {
        //Moves in the desired direction
        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, player.transform.position, enemy.movementSpeed * Time.deltaTime * SPEED_INCREASE_TAUNT);
        //Enemy goes towards the player
        enemy.transform.up = new Vector2(player.transform.position.x - enemy.transform.position.x, player.transform.position.y - enemy.transform.position.y);
    }

    /// <summary>
    /// Shoots at the enemy
    /// </summary>
    /// <returns>IEnumerator for the Coroutine</returns>
    protected IEnumerator Shoot()
    {
        while (true)
        {
            //For every bullet that the enemy will throw
            for (int index = 0; index < bullets.Count; ++index)
            {
                //Creates the bullet
                var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<BulletController>();
                bullet.gameObject.layer = 8;
                bullet.Initialize(bullets[index]);
            }
            yield return new WaitForSeconds(shootingTimer);
        }
    }

    /// <summary>
    /// Detect if the enemy is hurt by salt
    /// </summary>
    /// <param name="objectHit">Which object was hit</param>
    void OnTriggerEnter2D(Collider2D objectHit)
    {
        //If it hit a bullet
        if (objectHit.gameObject.GetComponent<SaltController>() && !isInvincible)
        {
            TakeDamage();
            StartCoroutine(Invincibility());
        }
    }

    /// <summary>
    /// Takes damage from hit
    /// </summary>
    protected virtual void TakeDamage()
    {
        //Take damage and check if he died
        --health.currentHealth;
        if (health.currentHealth <= 0)
            Died();
    }

    /// <summary>
    /// Destroy this game object
    /// </summary>
    protected void Died()
    {
        if(sceneController != null)
        {
            sceneController.EnemyDies();
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets the enemy invicible for a certain amount of time
    /// </summary>
    /// <returns>When it is not invincible anymore</returns>
    protected IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(INVINCIBILITY_TIME);
        isInvincible = false;
    }
}
