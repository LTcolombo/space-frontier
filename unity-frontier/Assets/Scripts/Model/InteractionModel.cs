using Utils.Injection;
using Utils.Signal;

namespace DefaultNamespace.Model
{
    public enum InteractionState
    {
        Walking,
        Dialog,
        Building
    }
    
    [Singleton]
    public class InteractionModel
    {
        private InteractionState _state = InteractionState.Walking;
        public Signal Updated = new Signal();
        
        public InteractionState Get()
        {
            return _state;
        }
        
        public void Set(InteractionState value)
        {
            _state = value;
            Updated.Dispatch();
        }
    }
}