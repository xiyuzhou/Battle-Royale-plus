using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
public class SyncChild : MonoBehaviourPun, IPunObservable
{
    public Transform gunModel;
    public Transform mainRotation;
    public Transform SwayRotation;

    private void Start()
    {
        PhotonNetwork.SerializationRate = 60;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gunModel.transform.position);
            stream.SendNext(mainRotation.transform.rotation.y* SwayRotation.transform.rotation.y);
        }
        else if (stream.IsReading)
        {
            gunModel.transform.position = (Vector3)stream.ReceiveNext();
            float receivedRotationY = (float)stream.ReceiveNext();
            Quaternion newRotation = gunModel.transform.rotation;
            newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, receivedRotationY, newRotation.eulerAngles.z);
            gunModel.transform.rotation = newRotation;
        }
    }
}
