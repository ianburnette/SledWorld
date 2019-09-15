namespace PlayerController
{
    public class PlayerSled: InputListener
    {
        protected override void SubscribeToInputListeners() {
            PlayerInput.OnSledPress += AttemptToGetOnSled;
        }
        protected override void UnsubscribeFromInputListeners() {
            PlayerInput.OnSledPress += AttemptToGetOnSled;
        }
        
        public bool OnSled { get; private set; }

        void AttemptToGetOnSled() => OnSled = !OnSled;
    }
}