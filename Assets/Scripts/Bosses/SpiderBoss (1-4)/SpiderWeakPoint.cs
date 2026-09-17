using UnityEngine;

public class SpiderWeakPoint : MonoBehaviour
{
    public SpiderBoss boss;

    public void HitByPogo()
    {
        if (boss == null)
            return;

        boss.Stun();
    }
}