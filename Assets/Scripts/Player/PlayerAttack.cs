using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private AudioClip fireballSound;
    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        bool mouseAttack = Input.GetMouseButtonDown(0);
#else
        bool mouseAttack = false;
#endif

        bool touchAttack = TouchInputManager.instance != null && TouchInputManager.instance.attackPressed;

        if ((mouseAttack || touchAttack) && cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            Attack();

            if (touchAttack)
                TouchInputManager.instance.ConsumeAttack();
        }
        cooldownTimer += Time.deltaTime;
    }
    private void Attack()
    {
        SoundManager.instance.PlaySound(fireballSound);
        cooldownTimer = 0;
        anim.SetTrigger("attack");

        fireballs[GetFireball()].transform.position = firePoint.position;
        fireballs[GetFireball()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));

    }
    private int GetFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}