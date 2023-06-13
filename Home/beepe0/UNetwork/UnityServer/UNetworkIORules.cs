namespace beepe0.UNetwork.UnityServer
{
    public static class UNetworkIORules
    {
        public interface IGeneralRules
        {
            public void OnDisconnect(ushort clientId);
            public void OnClose();
        }
        public interface IInputRules
        {
            public void OnWelcome(ushort clientId, UNetworkReadablePacket inputPacket);
        }
        
        public interface IOutputRules
        {
            public void OnWelcome(ushort clientId);
        }
    }
}