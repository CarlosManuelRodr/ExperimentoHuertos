using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Collections;

//This is a very thin wrapper file
public class ManyMouseWrapper : MonoBehaviour
{
    public enum ManyMouseUpdateStyle
    {
        Update,
        FixedUpdate
    }
    private ManyMouseUpdateStyle _updateStyle = ManyMouseUpdateStyle.Update;
    private static ManyMouseWrapper _instance;
    public static ManyMouseWrapper Instance
    {
        get
        {
           if(_instance == null)
           {
               _instance = FindObjectOfType(typeof (ManyMouseWrapper)) as ManyMouseWrapper;
               if (_instance == null)
               {
                   GameObject go = new GameObject("_ManyMouseWrapper");
                   _instance = go.AddComponent<ManyMouseWrapper>();
               }
           }
            return _instance;
        }
    }

    [DllImport("ManyMouse")]
    private static extern int ManyMouse_Init();

    [DllImport("ManyMouse")]
    private static extern IntPtr ManyMouse_DriverName(); //The string is in UTF-8 format. 

    [DllImport("ManyMouse")]
    private static extern void ManyMouse_Quit();

    [DllImport("ManyMouse", CharSet = CharSet.Ansi)]
    private static extern IntPtr ManyMouse_DeviceName(uint index);

    [DllImport("ManyMouse")]
    private static extern int ManyMouse_PollEvent(ref ManyMouseEvent mouseEvent);


    //Internal

    private int _numMice = 0;

    
    private List<ManyMouse> _manyMice;

	// Use this for initialization
	void Awake ()
	{
        ManyMouse_Quit();
	    int initCode = ManyMouse_Init();

        if (initCode < 0)
        {
            Debug.Log("ManyMouse Init Code:" + initCode + " so there must be some error. Trying to close and open again");
          
            ManyMouse_Quit();
            initCode = ManyMouse_Init();
            if (initCode < 0)
            {
                Debug.Log("ManyMouse Init Code:" + initCode + " so there must be some error. Retrying");
                
                return;
            }
        }

	    Debug.Log("ManyMouse Init Code:" + initCode);
        IntPtr mouseDriverNamePtr = ManyMouse_DriverName();
        string mouseDriverName = StringFromNativeUtf8(mouseDriverNamePtr);
        Debug.Log("ManyMouse Driver Name: " + mouseDriverName);

	    _numMice = initCode;

        
	    _manyMice = _manyMice ?? new  List<ManyMouse>();
        for (int i = 0; i < _numMice; i++)
        {
            ManyMouse mouse = new ManyMouse(i);
            //todo: check if we already have a mouse in that id.
            //if it's not the correct id anymore, we have to check by the mouse's name. this might read very generically!
            _manyMice.Add(mouse);
            
        }
	}

    public static string StringFromNativeUtf8(IntPtr nativeUtf8)
    {
        int len = 0;
        while (Marshal.ReadByte(nativeUtf8, len) != 0) ++len;
        if (len == 0) return string.Empty;
        byte[] buffer = new byte[len - 1];
        Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer);
    }

	
	void Update () {

        if (_updateStyle == ManyMouseUpdateStyle.Update)
        {
            Poll();
        }
	   
	}
    void FixedUpdate()
    {
        if (_updateStyle == ManyMouseUpdateStyle.FixedUpdate)
        {
            Poll();
        }
       
    }
    private void Poll()
    {
        if (_numMice == 0)
        {
            // Refresh check for mice occassionally?
        }
        else
        {
            //TODO: should sometimes check if we lost a mouse and then send out a "lost mouse" signal in that mouse object

            // Signal to clear out old deltas
            for (int i = 0; i < _numMice; i++)
            {
                _manyMice[i].PollingReset();
            }

            // Poll until empty
            ManyMouseEvent mouseEvent = new ManyMouseEvent();
            int eventsLeft = ManyMouse_PollEvent(ref mouseEvent);
            while (eventsLeft > 0)
            {
                ProcessEvent(mouseEvent);
                eventsLeft = ManyMouse_PollEvent(ref mouseEvent);
            }
        }
    }

   

    // Note you'll be recieving this very rapidly!
    private void ProcessEvent(ManyMouseEvent mouseEvent)
    {
        _manyMice[(int)mouseEvent.device].ProcessEvent(mouseEvent);
    }

    public static int MouseCount { get { return Instance._manyMice == null ? 0 : Instance._manyMice.Count; }}

    public static string MouseDeviceName(int id)
    {
        if(id > MouseCount)
        {
            return "Mouse ID Not found: " + id + ". There are only " + MouseCount + " devices found";
        }
        IntPtr mouseNamePtr = ManyMouse_DeviceName((uint)id);
        return Marshal.PtrToStringAnsi(mouseNamePtr);
    }

    //TODO: GetMouseBy by device name? but these are not unique?
    public static ManyMouse GetMouseByID(int id)
    {
        return Instance._manyMice[id];
    }

    void OnDestroy()
    {
        Debug.Log("ShuttingDown");
        ManyMouse_Quit();
    }
}
