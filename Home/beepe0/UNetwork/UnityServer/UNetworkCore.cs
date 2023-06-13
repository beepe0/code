using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace beepe0.UNetwork.UnityServer
{
    public static class UNetworkCore
    {
        public static TcpListener TcpListener;
        public static UdpClient UdpListener;
        
        public static Dictionary<ushort, UNetworkClient> Clients = new Dictionary<ushort, UNetworkClient>();
        
        public static UNetworkIORules.IGeneralRules GeneralRules;
        public static UNetworkIORules.IInputRules InputRules;
        public static UNetworkIORules.IOutputRules OutputRules;

        public static bool IsRunServer;

        public static class RulesHandler
        {
            private static Dictionary<ushort, PacketHandler> _rulesHandler = new Dictionary<ushort, PacketHandler>();
            public static Dictionary<ushort, PacketHandler> Rules => _rulesHandler;

            public static void UpdateGeneralRules(UNetworkIORules.IGeneralRules generalRules) => GeneralRules = generalRules;
            public static void UpdateInputRules(UNetworkIORules.IInputRules inputRules) => InputRules = inputRules;
            public static void UpdateOutputRules(UNetworkIORules.IOutputRules outputRules) => OutputRules = outputRules;
            public static void AddNewRule(ushort packetNumber,  PacketHandler packetHandler) => _rulesHandler.Add(packetNumber, packetHandler);
            public static void ExecuteRule(UNetworkReadablePacket packet) => _rulesHandler[packet.PacketNumber](packet.Index, packet);
            public static void Clear() => _rulesHandler.Clear();

            public delegate void PacketHandler(ushort clientId, UNetworkReadablePacket packet);
        }
        public static class DataHandler
        {
            public static class Tcp
            {
                public static void SendData(ushort clientId, UNetworkIOPacket outputPacket)
                {
                    if (!(Clients.TryGetValue(clientId, out var client) && client != null && client.Tcp.isTcpConnect)) return;
                        
                    outputPacket.Insert(clientId);
                    outputPacket.Insert(outputPacket.GetLength());
                    client.Tcp.SendData(outputPacket.ToArray());
                }

                public static void SendDataToAll(ushort clientId, UNetworkIOPacket outputPacket)
                {
                    if (!(Clients.TryGetValue(clientId, out var client) && client != null && client.Tcp.isTcpConnect)) return;
                    
                    outputPacket.Insert(clientId);
                    outputPacket.Insert(outputPacket.GetLength());
                    
                    foreach (UNetworkClient c in Clients.Values)
                    {
                        c.Tcp.SendData(outputPacket.ToArray());
                    }
                }

                public static void SendDataAllExceptClient(ushort clientId, UNetworkIOPacket outputPacket)
                {
                    if (!(Clients.TryGetValue(clientId, out var client) && client != null && client.Tcp.isTcpConnect)) return;
                    
                    outputPacket.Insert(clientId);
                    outputPacket.Insert(outputPacket.GetLength());
                    
                    foreach (UNetworkClient c in Clients.Values)
                    {
                        if (clientId != c.ClientId) c.Tcp.SendData(outputPacket.ToArray());
                    }
                }
            }

            public static class Udp
            {
                public static void SendData(ushort clientId, UNetworkIOPacket outputPacket)
                {
                    if (!(Clients.TryGetValue(clientId, out var client) && client != null && client.Udp.isUdpConnect)) return;
                    
                    outputPacket.Insert(clientId);
                    outputPacket.Insert(outputPacket.GetLength());
                    
                    client.Udp.SendData(outputPacket.ToArray());
                }

                public static void SendDataToAll(ushort clientId, UNetworkIOPacket outputPacket)
                {
                    if (!(Clients.TryGetValue(clientId, out var client) && client != null && client.Udp.isUdpConnect)) return;

                    outputPacket.Insert(clientId);
                    outputPacket.Insert(outputPacket.GetLength());
                    
                    foreach (UNetworkClient c in Clients.Values)
                    {
                        c.Udp.SendData(outputPacket.ToArray());
                    }
                }
        
                public static void SendDataToAllExceptClient(ushort clientId, UNetworkIOPacket outputPacket)
                {
                    if (!(Clients.TryGetValue(clientId, out var client) && client != null && client.Udp.isUdpConnect)) return;

                    outputPacket.Insert(clientId);
                    outputPacket.Insert(outputPacket.GetLength());
                    
                    foreach (UNetworkClient c in Clients.Values)
                    {
                        if(clientId != c.ClientId) c.Udp.SendData(outputPacket.ToArray());
                    }
                }
            }
        }

        private static void CallBackAcceptTcpClient(IAsyncResult asyncResult)
        {
            TcpClient cl = TcpListener.EndAcceptTcpClient(asyncResult);
            TcpListener.BeginAcceptTcpClient(CallBackAcceptTcpClient, null);

            foreach (UNetworkClient t in Clients.Values)
            {
                if(t.Tcp.TcpSocket == null) { t.Tcp.Connect(cl); return; }
            }
            
        }
        private static void CallBackUdpReceive(IAsyncResult asyncResult)
        {
            if (IsRunServer)
            {
                try
                {
                    IPEndPoint epClient = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = UdpListener.EndReceive(asyncResult, ref epClient);
                    
                    if(data.Length < 4)
                    {
                        UNetworkLogs.ErrorReceivingUdp();
                        return;
                    }

                    UNetworkIOPacket inputPacket = new UNetworkIOPacket(data);
                    
                    UNetworkReadablePacket readablePacket = new UNetworkReadablePacket
                    {
                        Length = inputPacket.ReadUShort(),
                        Index = inputPacket.ReadUShort(),
                        PacketNumber = inputPacket.ReadUShort(),
                        BufferBytes = inputPacket.ReadBytes((ushort)(inputPacket.GetLength() - inputPacket.ReadPointer)),
                    };
                    
                    if(!Clients.ContainsKey(readablePacket.Index)) return;
             
                    var uNetworkClient = Clients[readablePacket.Index];

                    if (uNetworkClient.Udp.EndPoint == null)
                    {
                        uNetworkClient.Udp.Connect(epClient); 
                        uNetworkClient.Udp.HandleData(readablePacket);
                    }
                    else if (uNetworkClient.Udp.EndPoint.Equals(epClient))
                    {
                        uNetworkClient.Udp.HandleData(readablePacket);
                    }

                    UdpListener.BeginReceive(CallBackUdpReceive, null);
                }
                catch (Exception e)
                {
                    UNetworkLogs.ErrorReceivingUdp(e);
                    Close();
                }
            }
        }
        public static async Task Start()
        {
            if (!IsRunServer)
            {
                await UNetworkManagerServer.WaitForInitialization();
            
                for (ushort clientId = 0; clientId < UNetworkManagerServer.Instance.slots; clientId++)
                {
                    Clients.Add(clientId, new UNetworkClient(clientId));
                }
                
                TcpListener = new TcpListener(new IPEndPoint(IPAddress.Parse(UNetworkManagerServer.Instance.serverInternetProtocol), UNetworkManagerServer.Instance.serverPort));
                TcpListener.Start();
                TcpListener.BeginAcceptTcpClient(CallBackAcceptTcpClient, null);
                IsRunServer = true;
                UdpListener = new UdpClient(UNetworkManagerServer.Instance.serverPort);
                UdpListener.BeginReceive(CallBackUdpReceive, null);
            }
        }
        public static void Close()
        {
            if (IsRunServer)
            {
                GeneralRules.OnClose();
                IsRunServer = false;
                
                UdpListener.Close();
                TcpListener.Stop();
                
                if(RulesHandler.Rules != null) RulesHandler.Clear();
                
                foreach (UNetworkClient networkClient in Clients.Values)
                {
                    networkClient.Close();
                }
                
                Clients.Clear();
            }
        }
    }
}