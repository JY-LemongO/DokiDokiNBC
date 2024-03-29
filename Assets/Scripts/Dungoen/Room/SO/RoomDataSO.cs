using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Create Room Data",menuName = "Default room data")]
public class RoomDataSO : ScriptableObject
{
    public int width;
    public int height;

    public GameObject leftDoorObj;
    public GameObject rightDoorObj;
    public GameObject topDoorObj;
    public GameObject bottomDoorObj;

    public GameObject tile_leftDoor;
    public GameObject tile_rightDoor;
    public GameObject tile_topDoor;
    public GameObject tile_bottomDoor;


    
}
