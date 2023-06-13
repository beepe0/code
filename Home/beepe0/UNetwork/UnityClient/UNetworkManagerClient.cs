using System.Threading.Tasks;
using UnityEngine;

namespace beepe0.UNetwork.UnityClient
{
    public sealed class UNetworkManagerClient : MonoBehaviour
    {
        public static UNetworkManagerClient Instance;
        
        [Header("Connection settings")]
        public ushort serverPort = 12345;
        public string serverInternetProtocol = "127.0.0.1";
        
        [Header("Client settings")]
        public ushort receiveBufferSize = 512;
        public ushort sendBufferSize = 512;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                UNetworkLogs.ErrorSingletonOnce();
                Destroy(this);
            }
            else Instance = this;
        }
        private async void Start()
        {
            await UNetworkCore.StartAsync();
            await UNetworkCore.ConnectAsync();
        }
        private void FixedUpdate()
        {
            UNetworkUpdate.Update();
        }
        private void OnApplicationQuit()
        {
            UNetworkCore.Close();
        }
        public static async Task WaitForInitialization() => await Task.Run(() => { while (Instance == null) { } });
    }
}