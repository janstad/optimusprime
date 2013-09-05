using SKYPE4COMLib;

namespace OptimusPrime.Interfaces
{
    public interface IListener
    {
        string Call(string pCommand, ChatMessage pMsg);
    }
}
