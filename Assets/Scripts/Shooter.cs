using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifeTime = 5f;
    [SerializeField] private float baseFiringRate = 0.2f;
    [SerializeField] private float firingRateVariance = 0;
    [SerializeField] private float minimumFiringRate = 0.1f;
    [SerializeField] private bool useAI;

    [SerializeField] private enum Cannons { One = 1, Two = 2, Three = 3 }
    [SerializeField] private Cannons cannonAmount;
    private Vector3 cannonOffsetL;
    private Vector3 cannonOffsetR;

    [HideInInspector]
    public bool isFiring;

    private Coroutine firingCor;
    private Vector2 moveDirection;
    private void Start()
    {
        if (useAI)
        {
            isFiring = true;
            moveDirection = transform.up * -1;
        }
        else
        {
            moveDirection = transform.up;
        }
    }

    private void Update()
    {
        Fire();
    }

    void Fire()
    {
        if (isFiring && firingCor == null)
        {
            firingCor = StartCoroutine(FireContinuously());
        }
        else if(!isFiring && firingCor != null)
        {
            StopCoroutine(firingCor);
            firingCor = null;
        }
        
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            List <GameObject> projectilesToSpawn = new();
            switch ((int) cannonAmount)
            {
                case 0:
                    yield break;
                case 1:
                    projectilesToSpawn.Add(Instantiate(projectilePrefab, transform.position, Quaternion.identity));
                    break;
                case 2:
                    cannonOffsetL = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.75f);
                    cannonOffsetR = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.75f);
                    projectilesToSpawn.Add(Instantiate(projectilePrefab, cannonOffsetL, Quaternion.identity));
                    projectilesToSpawn.Add(Instantiate(projectilePrefab, cannonOffsetR, Quaternion.identity));
                    break;
                case 3:
                    cannonOffsetL = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.75f);
                    cannonOffsetR = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.75f);
                    projectilesToSpawn.Add(Instantiate(projectilePrefab, cannonOffsetL, Quaternion.identity));
                    projectilesToSpawn.Add(Instantiate(projectilePrefab, transform.position, Quaternion.identity));
                    projectilesToSpawn.Add(Instantiate(projectilePrefab, cannonOffsetR, Quaternion.identity));
                    break;
            }

            for (int i = 0; i<projectilesToSpawn.Count; i++)
            {
                Rigidbody2D rb = projectilesToSpawn[i].GetComponent<Rigidbody2D>();
                
                if (rb != null)
                {
                    rb.velocity = moveDirection * projectileSpeed;
                }

                Destroy(projectilesToSpawn[i], projectileLifeTime);
            }

            float timeToNextProjectile = Random.Range(baseFiringRate - firingRateVariance, baseFiringRate + firingRateVariance);
            timeToNextProjectile = Mathf.Clamp(timeToNextProjectile, minimumFiringRate, float.MaxValue);
            yield return new WaitForSeconds(timeToNextProjectile);
        }
    }
}
