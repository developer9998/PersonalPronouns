using Photon.Pun;
using ScoreboardAttributes;
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

            if (Local.IsMyPlayer()) SetPronouns(Utils.GetFullPronoun(Utils.CurrentPronouns));
            else if (photonView != null && !photonView.IsMine) Network.Instance.OnPlayerPropertiesUpdate(photonView.Owner, photonView.Owner.CustomProperties);
        }

        public void Update()
        {
            // Mainly for GorillaShirts and obviously ingame sweaters & slingshots that move the nametag
            if (PronounLabel != null)
            {
                PronounLabel.color = Local.playerText.color;
                PronounLabel.transform.localPosition = new Vector3(PronounLabel.transform.localPosition.x, PronounLabel.transform.localPosition.y, Local.playerText.transform.localPosition.z);
            }
        }

        public void SetPronouns(string Pronouns)
        {
            if (Local.IsMyPlayer() && PhotonNetwork.InRoom)
            {
                Network.Instance.SetPronounsNetworked(Pronouns);
                SetPronounsLocal(Pronouns);
            }
            else if (Local.IsMyPlayer() && !PhotonNetwork.InRoom)
                SetPronounsLocal(Pronouns);
        }

        public void SetPronounsLocal(string pronouns)
        {
            if (PronounLabel != null)
                PronounLabel.text = pronouns.ToUpper();
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            if (Local.IsMyPlayer()) PlayerTexts.RegisterAttribute(PronounLabel.text, PhotonNetwork.LocalPlayer);
        }
    }
}
