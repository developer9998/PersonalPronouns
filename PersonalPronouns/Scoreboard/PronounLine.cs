using PersonalPronouns.Scripts;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PersonalPronouns.Scoreboard
{
    public class PronounLine : MonoBehaviour
    {
        public GorillaPlayerScoreboardLine baseLine;

        public Client pronounClient;
        public Text pronounTag;

        public async void Start()
        {
            try
            {
                if (baseLine != null && baseLine.playerName != null)
                {
                    var pronounTagObject = Instantiate(baseLine.playerName.gameObject, baseLine.playerName.transform.parent);
                    pronounTagObject.SetActive(true);
                    if (pronounTagObject.TryGetComponent(out pronounTag))
                    {
                        pronounTag.text = "";
                        pronounTag.transform.localPosition = new Vector3(-89.5f, -5.185f, 0f);
                        pronounTag.transform.localScale = Vector3.one * 0.45f;

                        var textList = baseLine.texts.ToList();
                        textList.Add(pronounTag);
                        baseLine.texts = textList.ToArray();
                    }

                    await Task.Delay(500);
                    if (baseLine.linePlayer != null)
                    {
                        var lineViewPlayer = Utils.GetPhotonViewFromPlayer(baseLine.linePlayer);
                        if (lineViewPlayer.IsMine && GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig.TryGetComponent(out Client localPlayerClient))
                        {
                            pronounClient = localPlayerClient;
                            pronounTag.text = pronounClient.PronounLabel.text;
                        }
                        else if (lineViewPlayer != null && lineViewPlayer.TryGetComponent(out Client networkedPlayerClient))
                        {
                            pronounClient = networkedPlayerClient;
                            pronounTag.text = pronounClient.PronounLabel.text;
                        }
                    }
                    InvokeRepeating(nameof(UpdateLine), 5f, 2.5f);
                }
            }
            catch { } // Too bad if it runs into an issue
        }

        public void UpdateLine()
        {
            if (pronounClient != null && pronounClient.PronounLabel != null)
            {
                // Match the label on the body with our label
                pronounTag.text = pronounClient.PronounLabel.text;
            }
        }
    }
}
