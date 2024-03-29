using UnityEngine;

namespace WrongWarp
{
    public class EmotePlayer : MonoBehaviour
    {
        private bool playAlert = false;
        private bool playBruh = false;

        [SerializeField] private Sprite alert;
        [SerializeField] private Sprite bruh;

        public void Init()
        {
            InputManager.PlayerControls.PlayerEmote.Alert.started += ctx => playAlert = true;
            InputManager.PlayerControls.PlayerEmote.Bruh.started += ctx => playBruh = true;
            TickManager.PollInput += PollInput;
        }

        private void PollInput(InputPacket packet)
        {
            if(playAlert)
            {
                packet.data.AddDataSlice(ADDataType.Byte, (byte)EmoteID.Alert, (int)FrameActionID.PlayEmote);
            }
            else if(playBruh)
            {
                packet.data.AddDataSlice(ADDataType.Byte, (byte)EmoteID.Bruh, (int)FrameActionID.PlayEmote);
            }
            playAlert = false;
            playBruh = false;
        }

        public void ClientPlayEmote(EmoteID id)
        {
            Billboard billboard = BillboardManager.CreateBillboard(BillboardManager.TestSprite, transform.position + Vector3.up * 3, 0.25f, 1.5f);
            billboard.stayVertical = false;
            switch (id)
            {
                case EmoteID.Alert:
                    billboard.SpriteRenderer.sprite = alert;
                    AudioManager.PlaySound(transform.position + Vector3.up * 3, SFXLibrary.GetAudioClip(SFXLibrary.SFXLibrarySFX._5000SoundsBlocklandAlert), 10);
                    break;
                case EmoteID.Bruh:
                    billboard.SpriteRenderer.sprite = bruh;
                    AudioManager.PlaySound(transform.position + Vector3.up * 3, SFXLibrary.GetAudioClip(SFXLibrary.SFXLibrarySFX.BruhSoundEffect2), 10);
                    break;
            }
        }

        public enum EmoteID : byte
        {
            Alert = 0,
            Bruh = 1
        }
    }
}
