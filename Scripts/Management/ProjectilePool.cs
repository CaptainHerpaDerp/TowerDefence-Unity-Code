using Core;
using Core.Character;
using System.Collections;
using System.Collections.Generic;
using Towers;
using UnityEngine;

namespace Management
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectilePool : MonoBehaviour
    {
        // Singleton instance
        public static ProjectilePool Instance;

        // Where all projectiles will be stored
        private Transform projectileParent;

        // The tag of the parent object that will hold all projectiles
        [SerializeField] private string projectileTag;

        // The prefabs of the projectiles
        [SerializeField] private GameObject arrowProjectilePrefab, fireballProjectilePrefab, catapultProjectilePrefab;

        // Singleton references
        EventBus eventBus;

        [SerializeField] private Queue<Projectile> arrowPool = new();
        [SerializeField] private Queue<FireballProjectile> fireballPool = new();
        [SerializeField] private Queue<ClusterProjectile> catapultPool = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("Multiple ProjectilePool instances found!");
                Destroy(this);
            }
        }

        private void Start()
        {
            projectileParent = GameObject.FindGameObjectWithTag(projectileTag).transform;

            if (projectileParent == null)
            {
                Debug.LogError($"Projectile parent not found with tag {projectileTag}!");
            }

            eventBus = EventBus.Instance;

            eventBus.Subscribe("ProjectileSpawnRequest", CreateProjectile);

            eventBus.Subscribe("ProjectileReturn", (data) =>
            {
                ReturnProjectile((Projectile)data);
            });
        }

        private void CreateProjectile(object spawnRequest)
        {
            ProjectileSpawnRequest projectileSpawnRequest = (ProjectileSpawnRequest)spawnRequest;

            ProjectileType projectileType = projectileSpawnRequest.projectileType;
            Vector3 spawnPosition = projectileSpawnRequest.spawnPosition;
            Quaternion spawnRotation = projectileSpawnRequest.spawnRotation;
            float projectileDamage = projectileSpawnRequest.projectileDamage;
            Transform attackTarget = projectileSpawnRequest.attackTarget;
            Vector3 preferredPosition = projectileSpawnRequest.preferredPosition;

            Projectile projectile = GetProjectile(projectileType);

            projectile.transform.SetParent(projectileParent);
            projectile.transform.position = spawnPosition;
            projectile.SetDamage(projectileDamage);

            // The catapult tower has a different target position for the cluster projectile, it uses a preferred position instead
            if (projectileType == ProjectileType.Cluster)
            {
                Debug.Log("Setting cluster target");

                // Cast the projectile to a cluster projectile to set the target
                ClusterProjectile clusterProjectile = (ClusterProjectile)projectile;      
                clusterProjectile.SetTarget(spawnPosition, null, preferredPosition);
            }
            else
            {
                projectile.SetTarget(spawnPosition, attackTarget, attackTarget.position);
            }
        }

        private void ReturnProjectile(object projectileObj)
        {
            if (projectileObj is Projectile projectile)
            {
                projectile.gameObject.SetActive(false);

                switch (projectile)
                {
                    case FireballProjectile fireballProjectile:
                        Debug.Log("Returning fireball");
                        fireballPool.Enqueue(fireballProjectile);
                        break;

                    case ClusterProjectile clusterProjectile:
                        Debug.Log("Returning cluster");
                        catapultPool.Enqueue(clusterProjectile);
                        break;

                    default:
                        Debug.Log("Returning arrow");
                        arrowPool.Enqueue(projectile);
                        break;
                }
            }
        }

        public Projectile GetProjectile(ProjectileType type)
        {
            switch (type)
            {
                case ProjectileType.Arrow:

                    if (arrowPool.Count > 0)
                    {
                        Debug.Log("Reusing arrow");
                        Projectile projectile = arrowPool.Dequeue();
                        projectile.gameObject.SetActive(true);
                        return projectile;
                    }
                    else
                    {
                        Debug.Log("Creating arrow");    
                        return Instantiate(arrowProjectilePrefab).GetComponent<Projectile>();
                    }

                case ProjectileType.Fireball:

                    if (fireballPool.Count > 0)
                    {
                        Debug.Log("Reusing fireball");
                        Projectile projectile = fireballPool.Dequeue();
                        projectile.gameObject.SetActive(true);
                        return projectile;
                    }
                    else
                    {
                        Debug.Log("Creating fireball");
                        return Instantiate(fireballProjectilePrefab).GetComponent<Projectile>();
                    }

                case ProjectileType.Cluster:

                    if (catapultPool.Count > 0)
                    {
                        Projectile projectile = catapultPool.Dequeue();
                        projectile.gameObject.SetActive(true);
                        return projectile;
                    }
                    else
                    {
                        return Instantiate(catapultProjectilePrefab).GetComponent<Projectile>();
                    }

                default:
                    Debug.LogWarning("Projectile type not found!");
                    break;

            }

            return null;
        }
    }
}

