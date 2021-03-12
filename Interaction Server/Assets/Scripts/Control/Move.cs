using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;
public class Move : MonoBehaviour
{
    // private float acceleration;
    public bool dominator;

    private float Speed;
    private float rotateSpeed;
    private KeyCode[] inputKeys;
    private KeyCode[] rotateInputKeys;
    private Vector3[] directionForkeys;

    private Vector3 position;
    private Quaternion rotation;
    private bool change = false, send = false;

    private NetWorkAsServer networkScript;
    //VR key ---xiaolong 20210310


    private Vector3[] directionForVRkeys;

    // 手柄的按键
    private string[] cotrollerKeys;
    private string[] cotrollerRotateKeys;
    //平移
    public SteamVR_Action_Boolean up
       ;
    public SteamVR_Action_Boolean down;

    public SteamVR_Action_Boolean back;

    public SteamVR_Action_Boolean forward;

    public SteamVR_Action_Boolean left;
    public SteamVR_Action_Boolean right;

    public SteamVR_Action_Boolean x_axis;
    public SteamVR_Action_Boolean in_x_axis;
    public SteamVR_Action_Boolean y_axis;
    public SteamVR_Action_Boolean in_y_axis;
    public SteamVR_Action_Boolean z_axis;
    public SteamVR_Action_Boolean in_z_axis;

    //
    public SteamVR_Action_Boolean isClientView;
    public GameObject clientCamera;


    public int times;
    private string VRKey;//

    bool clientView;

    // Start is called before the first frame update
    void Start()
    {
        dominator = true;
        Speed = 0.3f;
        rotateSpeed = 0.3f;
        clientView = false;

        inputKeys = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.D, KeyCode.S, KeyCode.Q, KeyCode.E };
        rotateInputKeys = new KeyCode[] { KeyCode.I, KeyCode.J, KeyCode.L, KeyCode.K, KeyCode.U, KeyCode.O };
        directionForkeys = new Vector3[] { Vector3.forward, Vector3.left, Vector3.right, Vector3.back, Vector3.up, Vector3.down };


        //xiaolong -20210310

        cotrollerKeys = new string[] { "forward", "left", "right", "back", "up", "down" };
        cotrollerRotateKeys = new string[] { "x", "-x", "y", "-y", "z", "-z" };
        networkScript = GameObject.Find("Relief").GetComponent<NetWorkAsServer>();
    }

    void FixedUpdate()
    {
        if (change)
        {
            transform.position = position;
            transform.rotation = rotation;
            change = false;
        }

        if (!dominator) return;
        if (!GameObject.Find("Controller (right)").GetComponent<MyRay>().detected) return;
        if (!GameObject.Find("Controller (right)").GetComponent<MyRay>().oppositeDetected) return;
        //clientView = isClientView.GetStateDown(SteamVR_Input_Sources.RightHand);


        if (isClientView.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            clientCamera.SetActive(clientView);
            clientView = !clientView;//是否显示clientview
        }
        // 平移
        for (int i = 0; i < inputKeys.Length; i++)
        {
            // var key = inputKeys[i];
            if (Input.GetKey(inputKeys[i]))
            {
                transform.Translate(directionForkeys[i] * Time.fixedDeltaTime * Speed, Space.World);
                send = true;
            }
        }

        // 旋转
        for (int i = 0; i < inputKeys.Length; i++)
        {
            if (Input.GetKey(rotateInputKeys[i]))
            {
                transform.Rotate(directionForkeys[i] * rotateSpeed);
                send = true;
            }
        }

        //
        //vr中的平移

        for (int i = 0; i < cotrollerKeys.Length; i++)
        {
            var key = cotrollerKeys[i];
            VRKey = getVRkey();//获取手柄的按健
            if (key == VRKey)
            {
                transform.Translate(directionForkeys[i] * Time.fixedDeltaTime * Speed, Space.World);
                send = true;
            }
        }

        // VR中的旋转
        for (int i = 0; i < cotrollerRotateKeys.Length; i++)
        {
            var key = cotrollerRotateKeys[i];
            VRKey = getVRkey();//获取手柄的按健
            if (key == VRKey)
            {
                // Quaternion q = Quaternion.LookRotation(Vector3.up, Vector3.forward);
                // Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.fixedDeltaTime);
                transform.Rotate(directionForkeys[i] * rotateSpeed);
                send = true;
                
            }
        }
        if (send)
        {
            SendPoint();
            send = false;
        }
        
       
    }

    /*
     * 给物体增加一个力的效果让其平滑移动，但是很难控制其停止
    void FixedUpdate()
    {
        bool enterKey = false;
        for (int i = 0; i < inputKeys.Length; i++)
        {
            var key = inputKeys[i];
            if (Input.GetKey(key))
            {
                enterKey = true;
                Vector3 movement = directionForkeys[i] * acceleration * Time.deltaTime;
                rigidBody.AddForce(movement);
            }
        }
    }
    */

    public void SetPosition(Vector3 _position)
    {
        position = _position;
        change = true;
    }

    public void SetRotation(Quaternion _rotation)
    {
        rotation = _rotation;
        change = true;
    }

    public void SetPosition(Vector3 _position, bool _send)
    {
        SetPosition(_position);
        send = _send;
    }

    public void SetRotation(Quaternion _rotation, bool _send)
    {
        SetRotation(_rotation);
        send = _send;
    }

    void SendPoint()
    {
        networkScript.SendMessageToClient("Target " + Vec3toStr(transform.position) + "," + QuatoStr(transform.rotation) + ",");
    }

    string Vec3toStr(Vector3 _vec)
    {
        string precision = "0.000";
        return _vec.x.ToString(precision) + "," + _vec.y.ToString(precision) + "," + _vec.z.ToString(precision);
    }

    string QuatoStr(Quaternion _q)
    {
        string precision = "0.000";
        return _q.x.ToString(precision) + "," + _q.y.ToString(precision) + "," + _q.z.ToString(precision) + "," + _q.w.ToString(precision);
    }

    //0310xiaolong
    public string getVRkey()
    {
        string key;
        key = " ";
        //if (up.GetStateDown(SteamVR_Input_Sources.Any)) key = "up";
        //if (down.GetStateDown(SteamVR_Input_Sources.Any)) key = "down";
        //if (back.GetStateDown(SteamVR_Input_Sources.Any)) key = "back";
        //if (forward.GetStateDown(SteamVR_Input_Sources.Any)) key = "forward";
        //if (left.GetStateDown(SteamVR_Input_Sources.Any)) key = "left";
        //if (right.GetStateDown(SteamVR_Input_Sources.Any)) key = "right";

        //平移
        if (up.GetState(SteamVR_Input_Sources.RightHand)) key = "up";
        if (down.GetState(SteamVR_Input_Sources.RightHand)) key = "down";
        if (back.GetState(SteamVR_Input_Sources.RightHand)) key = "back";
        if (forward.GetState(SteamVR_Input_Sources.RightHand)) key = "forward";
        if (left.GetState(SteamVR_Input_Sources.RightHand)) key = "left";
        if (right.GetState(SteamVR_Input_Sources.RightHand)) key = "right";

        if (up.GetStateDown(SteamVR_Input_Sources.RightHand)) print("up");
        if (down.GetStateDown(SteamVR_Input_Sources.RightHand)) print("down");
        if (back.GetStateDown(SteamVR_Input_Sources.RightHand)) print("back");
        if (forward.GetStateDown(SteamVR_Input_Sources.RightHand)) print("foward");
        if (left.GetStateDown(SteamVR_Input_Sources.RightHand)) print("left");
        if (right.GetStateDown(SteamVR_Input_Sources.RightHand)) print("right");

        //旋转
        if (x_axis.GetState(SteamVR_Input_Sources.LeftHand)) key = "x";
        if (in_x_axis.GetState(SteamVR_Input_Sources.LeftHand)) key = "-x";
        if (y_axis.GetState(SteamVR_Input_Sources.LeftHand)) key = "y";
        if (in_y_axis.GetState(SteamVR_Input_Sources.LeftHand)) key = "-y";
        if (z_axis.GetState(SteamVR_Input_Sources.LeftHand)) key = "z";
        if (in_z_axis.GetState(SteamVR_Input_Sources.LeftHand)) key = "-z";
        return key;
    }
}