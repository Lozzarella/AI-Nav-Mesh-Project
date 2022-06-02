using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]

public class CameraMovement : MonoBehaviour
{
    #region Variables
    [Header("Character")]
    [Tooltip("use this to apply movement in worldspace")]
    public Vector3 moveDir; // we will use this to apply movement in worldspace
    public CharacterController charC;//this is our reference variable to the character controller
    [Header("Speeds")]//headers create a header for the variable directly below
    public float moveSpeed = 5f;
    public float jumpSpeed = 8f, gravity = 20f;
    #endregion
    

    // Start is called before the first frame update
    void Start()
    {
   

        //assign a component to our reference
        charC = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        //if our character is grounded
        if (charC.isGrounded)
        {
            //set moveDir to the inputs direction
            moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //moveDir's forward is changed from global z (forward) to the Game Objects local Z (forward)
            moveDir = transform.TransformDirection(moveDir);//allows us to move where player is facing
            //moveDir is multiplied by speed so we move at a decent pace
            moveDir *= moveSpeed;
        
        }

        //regardless of if we are grounded or not the players moveDir.y is always affected by gravity multiplied my time.deltaTime to normalize it
        moveDir.y -= gravity * Time.deltaTime;
        //we then tell the character Controller that it is moving in a direction multiplied Time.deltaTime
        charC.Move(moveDir * Time.deltaTime);
    }
}
