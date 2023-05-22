using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PersonalPronouns.Scripts
{
    public static class Utils
    {
        public enum PronounFirst
        {
            He,
            She,
            They,
            It
        }

        public enum PronounSecond
        {
            Him,
            Her,
            Them,
            Its
        }

        public static Dictionary<string, int> PronounDict = new Dictionary<string, int>()
        {
            { "He", 0 },
            { "Him", 0 },
            { "She", 1 },
            { "Her", 1 },
            { "They", 2 },
            { "Them", 2 },
            { "It", 3 },
            { "Its", 3 },
        };

        public static PronounFirst First;
        public static PronounSecond Second;


        public static bool TryCreateLabel(VRRig vrRig, out Text PronounLabel)
        {
            PronounLabel = CreatePronounLabel(vrRig);
            return PronounLabel != null;
        }

        public static Text CreatePronounLabel(VRRig vRRig)
        {
            Text PronounLabel = null;
            if (vRRig != null)
            {
                Text NametagLabel = vRRig.playerText;
                GameObject PronounObject = Object.Instantiate(NametagLabel.gameObject, NametagLabel.transform.parent);
                if (PronounObject.TryGetComponent(out PronounLabel))
                {
                    Transform PronounTransform = PronounObject.transform;
                    PronounTransform.localPosition -= new Vector3(0, 7, 0);
                    PronounTransform.localScale = Vector3.one * 0.6f;
                }
            }

            return PronounLabel;
        }

        public static bool IsMyPlayer(VRRig vRRig)
            => vRRig.isOfflineVRRig || (vRRig.photonView != null && vRRig.photonView.IsMine);

        public static string PronounKey()
            => "PronNet";

        public static void InitPronouns()
        {
            string Inital = PlayerPrefs.GetString(PronounKey(), "They/Them");
            var Items = Inital.Split('/');
            First = (PronounFirst)PronounDict[Items[0]];
            Second = (PronounSecond)PronounDict[Items[1]];
            Debug.Log(string.Concat("This cool monke goes by ", First, " / ", Second, " pronouns :3"));

            PlayerPrefs.SetString(PronounKey(), GeneratePronounString());
            PlayerPrefs.Save();
        }

        public static string GeneratePronounString()
            => string.Concat(First, "/", Second);

        public static string GeneratePronounString(PronounFirst first, PronounSecond second)
            => string.Concat(first, "/", second);

        public static void UpdatePronouns(PronounFirst first, PronounSecond second)
        {
            var Pref = string.Concat(first, "/", second);
            PlayerPrefs.SetString(PronounKey(), Pref);
            PlayerPrefs.Save();

            if (GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig.TryGetComponent(out Client client))
                client.SetPronouns(Pref);
        }
    }
}
