using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PersonalPronouns.Scripts
{
    public class Client : MonoBehaviourPunCallbacks
    {
        public VRRig Local;
        public Text PronounLabel;

        public void Start()
        {
            if (Local == null && transform.TryGetComponent(out VRRig local))
                Local = local;

            Utils.TryCreateLabel(Local, out PronounLabel);
            PronounLabel.text = ""; // Default since usually the player doesn't have the mod
            if (Local.IsMyPlayer()) SetPronouns(Utils.GeneratePronounString());
        }

        public void Update()
        {
            // Mainly for GorillaShirts and obviously ingame sweaters & slingshots that move the nametag
            if (PronounLabel != null) PronounLabel.transform.localPosition = new Vector3(PronounLabel.transform.localPosition.x, PronounLabel.transform.localPosition.y, Local.playerText.transform.localPosition.z);
        }

        public void SetPronouns(string Pronouns)
        {
            if (Local.IsMyPlayer() && PhotonNetwork.InRoom)
                SetPronounsNetworked(Pronouns);
            else if (Local.IsMyPlayer() && !PhotonNetwork.InRoom)
                SetPronounsLocal(Pronouns);
        }

        public async override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            await Task.Delay(500);
            if (PhotonNetwork.InRoom) // Check if they're still in a room, anything could happen 
                SetPronounsNetworked(Utils.GeneratePronounString());
        }

        public void SetPronounsLocal(string pronouns)
        {
            if (PronounLabel != null)
                PronounLabel.text = pronouns.ToUpper();
        }

        public void SetPronounsNetworked(string pronouns)
        {
            Hashtable CustomProp = new Hashtable
            {
                { Utils.PronounKey(), pronouns }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(CustomProp);
            SetPronounsLocal(pronouns);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            try
            {
                if (targetPlayer.IsLocal) return;

                PhotonView targetView = GorillaGameManager.instance.FindVRRigForPlayer(targetPlayer);
                bool flag = targetView != null && targetView.TryGetComponent(out Client client);
                if (flag)
                {
                    flag = changedProps.TryGetValue(Utils.PronounKey(), out var PronounObject);
                    if (flag && PronounObject is string Pronouns) SetPronounsLocal(Pronouns);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Concat("Error attempting to get networked pronouns: ", ex.GetType().Name, "\n", ex.StackTrace));
            }
        }
    }
}
