﻿//gpsnmeajp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(uOSC.uOscServer))]
public class ExternalReceiverForVMC : MonoBehaviour {
    public ExternalSender externalSender;

    public SortedDictionary<string, SteamVR_Utils.RigidTransform> virtualController = new SortedDictionary<string, SteamVR_Utils.RigidTransform>();
    public SortedDictionary<string, SteamVR_Utils.RigidTransform> virtualTracker = new SortedDictionary<string, SteamVR_Utils.RigidTransform>();

    Vector3 pos;
    Quaternion rot;

    void Start () {
        var server = GetComponent<uOSC.uOscServer>();
        server.onDataReceived.AddListener(OnDataReceived);
    }

    void OnDataReceived(uOSC.Message message)
    {
        //有効なとき以外処理しない
        if (this.isActiveAndEnabled)
        {
            //仮想コントローラー V2.3
            if (message.address == "/VMC/Ext/Con/Pos"
                && (message.values[0] is string)
                && (message.values[1] is float)
                && (message.values[2] is float)
                && (message.values[3] is float)
                && (message.values[4] is float)
                && (message.values[5] is float)
                && (message.values[6] is float)
                && (message.values[7] is float)
            )
            {
                string serial = (string)message.values[0];
                var rigidTransform = SetTransform(ref pos, ref rot, ref message);

                if (virtualController.ContainsKey(serial))
                {
                    virtualController[serial] = rigidTransform;
                }
                else
                {
                    virtualController.Add(serial, rigidTransform);
                }
            }
            //仮想トラッカー V2.3
            else if ((message.address == "/VMC/Ext/Hmd/Pos"
                || message.address == "/VMC/Ext/Tra/Pos")
                && (message.values[0] is string)
                && (message.values[1] is float)
                && (message.values[2] is float)
                && (message.values[3] is float)
                && (message.values[4] is float)
                && (message.values[5] is float)
                && (message.values[6] is float)
                && (message.values[7] is float)
            )
            {
                string serial = (string)message.values[0];
                var rigidTransform = SetTransform(ref pos, ref rot, ref message);

                if (virtualTracker.ContainsKey(serial))
                {
                    virtualTracker[serial] = rigidTransform;
                }
                else
                {
                    virtualTracker.Add(serial, rigidTransform);
                }
            }
            //フレーム設定 V2.3
            else if (message.address == "/VMC/Ext/Set/Period"
                && (message.values[0] is int)
                && (message.values[1] is int)
                && (message.values[2] is int)
                && (message.values[3] is int)
                && (message.values[4] is int)
                && (message.values[5] is int)
            )
            {
                externalSender.periodStatus = (int)message.values[0];
                externalSender.periodRoot = (int)message.values[1];
                externalSender.periodBone = (int)message.values[2];
                externalSender.periodBlendShape = (int)message.values[3];
                externalSender.periodCamera = (int)message.values[4];
                externalSender.periodDevices = (int)message.values[5];
            }
        }
    }
    SteamVR_Utils.RigidTransform SetTransform(ref Vector3 pos, ref Quaternion rot,ref uOSC.Message message) {
        pos.x = (float)message.values[1];
        pos.y = (float)message.values[2];
        pos.z = (float)message.values[3];
        rot.x = (float)message.values[4];
        rot.y = (float)message.values[5];
        rot.z = (float)message.values[6];
        rot.w = (float)message.values[7];
        return new SteamVR_Utils.RigidTransform(pos, rot);
    }

    void Update()
    {

    }
}
