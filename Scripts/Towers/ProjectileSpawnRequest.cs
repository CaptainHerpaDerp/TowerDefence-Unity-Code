using UnityEngine;

namespace Towers
{
    public enum ProjectileType
    {
        Arrow,
        Fireball,
        Cluster
    }

    public struct ProjectileSpawnRequest
    {
        public ProjectileType projectileType;
        public Vector3 spawnPosition;
        public Quaternion spawnRotation;

        public float projectileDamage;
        public Transform attackTarget;
        public Vector3 preferredPosition;

        public ProjectileSpawnRequest(ProjectileType projectileType, Vector3 spawnPosition, Quaternion spawnRotation, float projectileDamage, Transform attackTarget, Vector3 preferredPosition)
        {
            this.projectileType = projectileType;
            this.spawnPosition = spawnPosition;
            this.spawnRotation = spawnRotation;
            this.projectileDamage = projectileDamage;
            this.attackTarget = attackTarget;
            this.preferredPosition = preferredPosition;
        }
    }
}
