using UnityEngine.UI;

namespace WrongWarp
{
    public interface IToggleableUI
    {
        public void OpenUI();
        public void CloseUI();
        public UIType uiType { get; }
        public bool isOpen { get; }
        public GraphicRaycaster graphicRaycaster { get; }
    }
}