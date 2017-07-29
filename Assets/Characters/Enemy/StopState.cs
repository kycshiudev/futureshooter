﻿using UnityEngine;

public class StopState : BaseEnemyState, EnemyState
{
    private int timer;
    private Transform player;
    private bool gotHit = false;

    public EnemyState HandleTransition(Enemy enemy)
    {
        var baseTransition = base.HandleTransition(enemy);
        if (baseTransition != null)
            return baseTransition;

        if (timer > 0)
            return null;

        if (Random.value > 0.5f)
            return ScriptableObject.CreateInstance<AdvanceState>();
        else
            return ScriptableObject.CreateInstance<FlankState>();
    }

    public void HandleUpdate(Enemy enemy)
    {
        // no movement
        enemy.transform.LookAt(player);
        --timer;
    }

    public void OnEnter(Enemy enemy)
    {
        base.OnEnter(enemy);

        // set timer based on enemy settings
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timer = Random.Range(enemy.stopTimeMin, enemy.stopTimeMax+1);
    }

    public void OnExit(Enemy enemy)
    {
        base.OnExit(enemy);
    }
}
