using Mirror;

namespace WrongWarp
{
    /// <summary>
    /// Can this object be interacted with?
    /// <para>Requires string ReturnText();</para>
    /// <para>Requires void CmdInteract(); with optional NetworkConnectionToClient parameter</para>
    /// </summary>
    public interface IInteractable
    {
        string ReturnText();
        void CmdInteract(NetworkConnectionToClient sender = null);
    }
}