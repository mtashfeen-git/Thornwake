using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Spikehead : Enemy_Damage
{
    [Header("SpikeHead Attributes")]
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;
    private Vector3[] directions = new Vector3[4];
    private Vector3 destination;
    private float checkTimer;
    private bool attacking;
    private Health playerHealth;

    [Header("SFX")]
    [SerializeField] private AudioClip impactSound;

    private void OnEnable()
    {
        Stop();
    }
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerHealth = player.GetComponent<Health>();
    }
    private void Update()
    {
        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            Stop();
            return;
        }

        //Move spikehead to destination only if attacking
        if (attacking)
            transform.Translate(destination * Time.deltaTime * speed);
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
                CheckForPlayer();
        }
    }
    private void CheckForPlayer()
    {
        CalculateDirections();

        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i], Color.red);

            // Check player, wall, AND ground together — whichever the ray hits FIRST wins.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i].normalized, range, playerLayer | wallLayer | groundLayer);

            if (hit.collider != null && !attacking)
            {
                // Only attack if what we hit is actually the player, not a wall or floor blocking the way
                if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                {
                    attacking = true;
                    destination = directions[i];
                    checkTimer = 0;
                }
            }
        }
    }
    private void CalculateDirections()
    {
        directions[0] = transform.right * range; //Right direction
        directions[1] = -transform.right * range; //Left direction
        directions[2] = transform.up * range; //Up direction
        directions[3] = -transform.up * range; //Down direction
    }
    private void Stop()
    {
        destination = transform.position; //Set destination as current position so it doesn't move
        attacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SoundManager.instance.PlaySound(impactSound); //Play impact sound
        base.OnTriggerEnter2D(collision);
        Stop(); //Stop spikehead once he hits something
    }
}