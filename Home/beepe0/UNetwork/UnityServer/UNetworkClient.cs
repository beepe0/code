using System;
using System.Net;
using System.Net.Sockets;

namespace beepe0.UNetwork.UnityServer
{
    [Serializable]
    public sealed class UNetworkClient
    {
        public readonly ushort ClientId;
        
        public ProtocolTcp Tcp;
        public ProtocolUdp Udp;

        public UNetworkClient(ushort clientId)
        {
            ClientId = clientId;
            Tcp = new ProtocolTcp(clientId);
            Udp = new ProtocolUdp(clientId);
        }
        
        [Serializable]
        public sealed class ProtocolTcp
        {
            public TcpClient TcpSocket;
            public NetworkStream NetworkStream;

            public bool isTcpConnect;

            private readonly ushort _clientId;
            
            public byte[] receiveData;

            public ProtocolTcp(ushort clientId)
            {
                _clientId = clientId;
            }
            public void Connect(TcpClient tcpSocket)
            {
                if (!isTcpConnect)
                {
                    TcpSocket = tcpSocket;
                    TcpSocket.ReceiveBufferSize = UNetworkManagerServer.Instance.receiveBufferSize;
                    TcpSocket.SendBufferSize = UNetworkManagerServer.Instance.sendBufferSize;
            
                    NetworkStream = TcpSocket.GetStream();
                    receiveData = new byte[UNetworkManagerServer.Instance.receiveBufferSize];
                    isTcpConnect = true;
                    
                    UNetworkCore.OutputRules.OnWelcome(_clientId);
                    
                    NetworkStream.BeginRead(receiveData, 0, receiveData.Length, CallBackReceive, null);
                }
            }
            private void CallBackReceive(IAsyncResult asyncResult)
            {
                if (isTcpConnect)
                {
                    try
                    {
                        int sizeData = NetworkStream.EndRead(asyncResult);

                        if (sizeData < 4)
                        {
                            UNetworkLogs.ErrorReceivingTcp();
                            UNetworkCore.Clients[_clientId].Close();
                            return;
                        }

                        HandleData(sizeData, receiveData);
                        receiveData = new byte[UNetworkManagerServer.Instance.receiveBufferSize];
                        NetworkStream.BeginRead(receiveData, 0, receiveData.Length, CallBackReceive, null);
                    }
                    catch (Exception e)
                    {
                        UNetworkLogs.ErrorReceivingTcp(e);
                        UNetworkCore.Clients[_clientId].Close();
                    }
                }
            }
            private void HandleData(int sizeData, byte[] data)
            {
                UNetworkIOPacket packet = new UNetworkIOPacket(data);
                while (sizeData - 1 > packet.ReadPointer)
                {
                    UNetworkReadablePacket handlerPacket = new UNetworkReadablePacket
                    {
                        Length =  packet.ReadUShort(),
                        Index = packet.ReadUShort(),
                        PacketNumber = packet.ReadUShort(),
                        BufferBytes = packet.ReadBytes((ushort)(sizeData - packet.ReadPointer))
                    };

                    UNetworkUpdate.AddToQueue(() =>
                    {
                        UNetworkCore.RulesHandler.ExecuteRule(handlerPacket);
                    }); 
                }
            }
            public void SendData(byte[] data)
            {
                try
                {
                    if (TcpSocket != null)
                    {
                        NetworkStream.BeginWrite(data, 0, data.Length, null, null);
                    }
                }
                catch (Exception e)
                {
                    UNetworkLogs.ErrorSendingTcp(e);
                    UNetworkCore.Clients[_clientId].Close();
                }
            }
        }
        [Serializable]
        public sealed class ProtocolUdp
        {
            public IPEndPoint EndPoint;

            public bool isUdpConnect;
            private readonly ushort _clientId;

            public ProtocolUdp(ushort clientId)
            {
                _clientId = clientId;
            }
            public void Connect(IPEndPoint endPoint)
            {
                if (!isUdpConnect)
                {
                    EndPoint = endPoint;
                    isUdpConnect = true;
                }
            }
            public void SendData(byte[] data)
            {
                try
                {
                    if (UNetworkCore.UdpListener != null && EndPoint != null)
                    {
                        UNetworkCore.UdpListener.BeginSend(data, data.Length, EndPoint, null, null);
                    }
                }
                catch (Exception e)
                {
                    UNetworkLogs.ErrorSendingUdp(e);
                    UNetworkCore.Clients[_clientId].Close();
                }
            }
            public void HandleData(UNetworkReadablePacket handlerPacket)
            {
                UNetworkUpdate.AddToQueue(() =>
                {
                    UNetworkCore.RulesHandler.ExecuteRule(handlerPacket);
                });
            }
        }
        
        public void Close()
        {
            UNetworkCore.GeneralRules.OnDisconnect(ClientId);
            if (Tcp is { isTcpConnect: true })
            {
                Tcp.isTcpConnect = false;
                
                Tcp.TcpSocket.Close();
                Tcp.NetworkStream.Close();
                
                Tcp.receiveData = null;
                Tcp.TcpSocket = null;
                Tcp.NetworkStream = null;
            }

            if (Udp is { isUdpConnect: true })
            {
                Udp.isUdpConnect = false;
                
                Udp.EndPoint = null;
            }
        }
    }
}