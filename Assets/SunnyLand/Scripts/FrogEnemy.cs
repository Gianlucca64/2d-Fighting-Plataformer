using UnityEngine;

public class FrogEnemy : WalkerEnemy
{
    void FixedUpdate()
    {
        Patrol();
    }
}