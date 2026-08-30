using UnityEngine;

public class Player_Respawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkPointSound;
    private Transform currentCheckPoint;
    private Health playerHealth;
    private UI_Manager uiManager;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiManager = FindFirstObjectByType<UI_Manager>();
    }

    public void checkRespawn()
    {
        if (currentCheckPoint == null) 
        {
            //GAME OVER SCREEN
            uiManager.GameOver();

            return;
        }
         
        transform.position = currentCheckPoint.position;
        playerHealth.Respawn();

        Camera.main.GetComponent<CameraController>().MoveToNewRoom(currentCheckPoint.parent);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CheckPoint"))
        {
            currentCheckPoint = collision.transform;
            SoundManager.instance.PlaySound(checkPointSound);
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("appear");
        }
    }


}
 