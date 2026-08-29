using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Room Camera Movement
    [SerializeField] private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    //Follow Player Camera Movement
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    private float lookAhead;

    //Vertical Follow
    [SerializeField] private float yFollowSpeed;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    private float currentY;


    private void Update()
    {

        //Room Camera Movement
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z),ref velocity, speed);

        //Follow Player Camera Movement
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);

        currentY = Mathf.Lerp(currentY, player.position.y, Time.deltaTime * yFollowSpeed);
        currentY = Mathf.Clamp(currentY, minY, maxY);

        transform.position = new Vector3(player.position.x + lookAhead, currentY, transform.position.z);


    }


    public void MoveToNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }

}