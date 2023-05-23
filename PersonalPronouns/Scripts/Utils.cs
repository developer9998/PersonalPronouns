using PersonalPronouns.Models;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace PersonalPronouns.Scripts
{
    public static class Utils
    {
        public static List<Pronouns> Pronouns;

        public static List<string> Subject = new List<string>();
        public static int SubjectPronoun;

        public static List<string> Object = new List<string>();
        public static int ObjectPronoun;

        public static List<string> Possessive = new List<string>();
        public static int PossessivePronoun;

        public static Pronouns CurrentPronouns;

        public enum PronounType
        {
            Subject = 0,
            Object = 1,
            Possessive = 2
        }

        public static void RegisterPronouns()
        {
            Pronouns = new List<Pronouns>();
            Pronouns.Add(new Pronouns("He", "Him", "His"));
            Pronouns.Add(new Pronouns("She", "Her", "Hers"));
            Pronouns.Add(new Pronouns("They", "Them", "Theirs"));
            Pronouns.Add(new Pronouns("It", "It", "Its"));

            Pronouns.ForEach(pronoun =>
            {
                Subject.Add(pronoun.Subject);
                Object.Add(pronoun.Object);
                Possessive.Add(pronoun.Possessive);
            });
        }

        public static void RecoverPronouns()
        {
            var PronounPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Pronouns.json");

            if (File.Exists(PronounPath))
            {
                var pronouns = JsonUtility.FromJson<Pronouns>(File.ReadAllText(PronounPath));
                CurrentPronouns = new Pronouns(pronouns.Subject, pronouns.Object, pronouns.Possessive);
            }
            else
            {
                CurrentPronouns = new Pronouns(GetPronoun(PronounType.Subject, 2), GetPronoun(PronounType.Object, 2), GetPronoun(PronounType.Possessive, 2));
                var PronounContents = JsonUtility.ToJson(CurrentPronouns);
                File.WriteAllText(PronounPath, PronounContents);
            }

            SubjectPronoun = Subject.IndexOf(CurrentPronouns.Subject);
            ObjectPronoun = Object.IndexOf(CurrentPronouns.Object);
            PossessivePronoun = Possessive.IndexOf(CurrentPronouns.Possessive);
        }

        public static void SavePronouns()
        {
            var PronounPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Pronouns.json");
            var PronounContents = JsonUtility.ToJson(CurrentPronouns);
            File.WriteAllText(PronounPath, PronounContents);
        }

        public static string GetPronoun(PronounType pronounType, int index)
        {
            var list = pronounType == PronounType.Subject ? Subject : (pronounType == PronounType.Object ? Object : Possessive);
            return list[index];
        }

        public static string GetFullPronoun(Pronouns pronouns)
        {
            if (pronouns.Possessive == pronouns.Object)
                return string.Concat(pronouns.Subject, "/", pronouns.Object);

            return string.Concat(pronouns.Subject, "/", pronouns.Object, "/", pronouns.Possessive);
        }

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
                GameObject PronounObject = UnityEngine.Object.Instantiate(NametagLabel.gameObject, NametagLabel.transform.parent);
                if (PronounObject.TryGetComponent(out PronounLabel))
                {
                    Transform PronounTransform = PronounObject.transform;
                    PronounTransform.localPosition -= new Vector3(0, 7, 0);
                    PronounTransform.localScale = Vector3.one * 0.4f;
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
            RegisterPronouns();
            RecoverPronouns();

            Debug.Log(string.Concat("Our pronouns are currently ", GetFullPronoun(CurrentPronouns)));
        }

        public static void UpdatePronouns(int subjectIndex, int objectIndex, int possessiveIndex)
        {
            SubjectPronoun = subjectIndex;
            ObjectPronoun = objectIndex;
            PossessivePronoun = possessiveIndex;
            CurrentPronouns = new Pronouns(Subject[SubjectPronoun], Object[ObjectPronoun], Possessive[PossessivePronoun]);

            SavePronouns();
            if (GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig.TryGetComponent(out Client client))
                client.SetPronouns(GetFullPronoun(CurrentPronouns));
        }

        public static PhotonView GetPhotonViewFromPlayer(Player viewOwner)
        {
            if (GorillaParent.instance != null && GorillaParent.instance.vrrigs is List<VRRig> userRigs && userRigs.Count > 0)
            {
                foreach (var rig in userRigs)
                    if (rig.photonView != null && rig.photonView.Owner == viewOwner) return rig.photonView;
            }

            return null;
        }
    }
}
