using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

namespace PersonalPronouns.Scripts
{
    public class Network : MonoBehaviourPunCallbacks
    {
        public static Network Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            SetPronounsNetworked(Utils.GeneratePronounString());
        }

        public void SetPronounsNetworked(string pronouns)
        {
            Hashtable CustomProp = new Hashtable
            {
                { Utils.PronounKey(), pronouns }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(CustomProp);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            try
            {
                if (targetPlayer.IsLocal) return;

                PhotonView targetView = GorillaGameManager.instance.FindVRRigForPlayer(targetPlayer);
                if (targetView != null && targetView.TryGetComponent(out Client client))
                {
                    if (changedProps.TryGetValue(Utils.PronounKey(), out var PronounObject) && PronounObject is string Pronouns) client.SetPronounsLocal(Pronouns);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Concat("Error attempting to get networked pronouns: ", ex.GetType().Name, "\n", ex.StackTrace));
            }
        }
    }
}
