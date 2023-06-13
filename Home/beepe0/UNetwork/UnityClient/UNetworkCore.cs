using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace beepe0.UNetwork.UnityClient
{
    public static class UNetworkCore
    {
        public static ushort ClientId;
        
        private static ProtocolTcp _tcp;
        private static ProtocolUdp _udp;

        public static UNetworkIORules.IGeneralRules GeneralRules;
        public static UNetworkIORules.IInputRules InputRules;
        public static UNetworkIORules.IOutputRules OutputRules;
        
        public static ProtocolTcp Tcp => _tcp;
        public static ProtocolUdp Udp => _udp;

        public sealed class ProtocolTcp
        {
            public TcpClient TcpClient;
            public NetworkStream NetworkStream;

            public bool IsTcpConnect;
        
            public byte[] ReceiveData;
            
            public void Connect()
            {
                if (!IsTcpConnect)
                {
                    TcpClient = new TcpClient();
                    TcpClient.ReceiveBufferSize = UNetworkManagerClient.Instance.receiveBufferSize;
                    TcpClient.SendBufferSize = UNetworkManagerClient.Instance.sendBufferSize;
            
                    ReceiveData = new byte[UNetworkManagerClient.Instance.receiveBufferSize];
              
                    TcpClient.BeginConnect(UNetworkManagerClient.Instance.serverInternetProtocol, UNetworkManagerClient.Instance.serverPort, CallBackConnect, null);
                }
            }
            private void CallBackConnect(IAsyncResult asyncResult)
            {
                TcpClient.EndConnect(asyncResult);

                if (!TcpClient.Connected)
                {
                    Close();
                    return;
                }

                NetworkStream = TcpClient.GetStream();
                NetworkStream.BeginRead(ReceiveData, 0, UNetworkManagerClient.Instance.receiveBufferSize, CallBackReceive, null);
                
                IsTcpConnect = true;
            }
            private void CallBackReceive(IAsyncResult asyncResult)
            {
                if (IsTcpConnect)
                {
                    try
                    {
                        int sizeData = NetworkStream.EndRead(asyncResult);
                
                        if (sizeData < 4)
                        {
                            UNetworkLogs.ErrorReceivingTcp();
                            Close();
                            return;
                        }

                        HandleData(sizeData, ReceiveData);
                        ReceiveData = new byte[UNetworkManagerClient.Instance.receiveBufferSize];
                        NetworkStream.BeginRead(ReceiveData, 0, ReceiveData.Length, CallBackReceive, null);
                    }
                    catch (Exception e)
                    {
                        UNetworkLogs.ErrorReceivingTcp(e);
                        Close();
                    }
                }
            }
            private void HandleData(int sizeData, byte[] data)
            {
                ushort readBytes = 0;
                UNetworkIOPacket packet = new UNetworkIOPacket(data);
                while (sizeData - 1 > packet.ReadPointer)
                {
                    UNetworkReadablePacket handlerPacketTools = new UNetworkReadablePacket();
                    handlerPacketTools.Length = packet.ReadUShort();
                    handlerPacketTools.Index = packet.ReadUShort();
                    handlerPacketTools.PacketNumber = packet.ReadUShort();
                    handlerPacketTools.BufferBytes = packet.ReadBytes((ushort)((handlerPacketTools.Length + 2 + readBytes) - packet.ReadPointer));
                    
                    readBytes += (ushort)(handlerPacketTools.Length + 2);
                    
                    UNetworkUpdate.AddToQueue(() =>
                    {
                        UNetworkCore.RulesHandler.ExecuteRule(handlerPacketTools);
                    }); 
                }
            }
            public void SendData(byte[] data)
            {
                try
                {
                    if (TcpClient != null)  
                    {
                        NetworkStream.BeginWrite(data, 0, data.Length, null, null);
                    }
                }
                catch (Exception e)
                {
                    UNetworkLogs.ErrorSendingTcp(e);
                    Close();
                }
            }
        }
        public sealed class ProtocolUdp
        {
            public UdpClient UdpClient;
            public IPEndPoint EndPoint;

            public bool IsUdpConnect;

            public void Connect()
            {
                if (!IsUdpConnect)
                {
                    EndPoint = new IPEndPoint(IPAddress.Parse(UNetworkManagerClient.Instance.serverInternetProtocol), UNetworkManagerClient.Instance.serverPort);
            
                    UdpClient = new UdpClient();
                    
                    UdpClient.Client.SendBufferSize = UNetworkManagerClient.Instance.sendBufferSize;
                    UdpClient.Client.ReceiveBufferSize = UNetworkManagerClient.Instance.receiveBufferSize;
                
                    UdpClient.Connect(EndPoint);
                    UdpClient.BeginReceive(CallBackReceive, null);

                    IsUdpConnect = true;
                }
            }

            private void CallBackReceive(IAsyncResult asyncResult)
            {
                if (IsUdpConnect)
                {
                    try
                    {
                        byte[] data = UdpClient.EndReceive(asyncResult, ref EndPoint);

                        if (data.Length < 4)
                        {
                            UNetworkLogs.ErrorReceivingUdp();
                            Close();
                            return;
                        }
                        UNetworkIOPacket inputPacket = new UNetworkIOPacket(data);
                    
                        UNetworkReadablePacket readablePacketTools = new UNetworkReadablePacket
                        {
                            Length = inputPacket.ReadUShort(),
                            Index = inputPacket.ReadUShort(),
                            PacketNumber = inputPacket.ReadUShort(),
                            BufferBytes = inputPacket.ReadBytes((ushort)(inputPacket.GetLength() - inputPacket.ReadPointer)),
                        };
                        
                        HandleData(readablePacketTools);
                
                        UdpClient.BeginReceive(CallBackReceive, null);
                    }
                    catch(Exception e)
                    {
                        UNetworkLogs.ErrorReceivingUdp(e);
                        Close();
                    }
                }
            }
        
            public void HandleData(UNetworkReadablePacket packetTools)
            {
                UNetworkUpdate.AddToQueue(() =>
                {
                    UNetworkCore.RulesHandler.ExecuteRule(packetTools);
                });
            }

            public void SendData(byte[] data)
            {
                try
                {
                    if (UdpClient != null && EndPoint != null)
                    {
                        UdpClient.BeginSend(data, data.Length, null, null);
                    }
                }
                catch (Exception e)
                {
                    UNetworkLogs.ErrorSendingUdp(e);
                    Close();
                }
            }
            
        }
        public static class RulesHandler
        {
            private static Dictionary<ushort, PacketHandler> _rulesHandler = new Dictionary<ushort, PacketHandler>();
            public static Dictionary<ushort, PacketHandler> Rules { get { return _rulesHandler; }}
            
            public static void UpdateGeneralRules(UNetworkIORules.IGeneralRules generalRules) => GeneralRules = generalRules;
            public static void UpdateInputRules(UNetworkIORules.IInputRules inputRules) => InputRules = inputRules;
            public static void UpdateOutputRules(UNetworkIORules.IOutputRules outputRules) => OutputRules = outputRules;
            public static void AddNewRule(ushort packetNumber,  PacketHandler packetHandler) => _rulesHandler.Add(packetNumber, packetHandler);
            public static void ExecuteRule(UNetworkReadablePacket packetTools) => _rulesHandler[packetTools.PacketNumber](packetTools);
            public static void Clear() => _rulesHandler.Clear();

            public delegate void PacketHandler(UNetworkReadablePacket packetTools);
        }
        public static class DataHandler
        {
            public static class Tcp
            {
                public static void SendData(UNetworkIOPacket outputPacket)
                {
                    if(_tcp == null) return;
                    
                    outputPacket.Insert(ClientId);
                    outputPacket.Insert(outputPacket.GetLength());
                    _tcp.SendData(outputPacket.ToArray());
                }
            }
            
            public static class Udp
            {
                public static void SendData(UNetworkIOPacket outputPacket)
                {
                    if(_udp == null) return;
                    
                    outputPacket.Insert(ClientId);
                    outputPacket.Insert(outputPacket.GetLength());
                    _udp.SendData(outputPacket.ToArray());
                }
            }
        }

        public static async Task StartAsync()
        {
            await UNetworkManagerClient.WaitForInitialization();
            
            _tcp = new ProtocolTcp();
            _udp = new ProtocolUdp();
        }
        public static async Task ConnectAsync()
        {
            await UNetworkManagerClient.WaitForInitialization();
            
            _tcp.Connect();

            await Task.Run(() => {
                while (Tcp is {IsTcpConnect: false})
                {
                    
                }
            });
            
            _udp.Connect();
            
            await Task.Run(() => {
                while (Udp is {IsUdpConnect: false})
                {
                    
                }
            });
        }

        public static async Task WaitForConnection() => await Task.Run(() =>
        {
            while (!(Tcp is {IsTcpConnect: true}) || !( Udp is {IsUdpConnect: true}))
            {
                
            }
        });

        public static void Close()
        {
            GeneralRules.OnDisconnect();
            if (_tcp is {IsTcpConnect: true})
            {
                _tcp.IsTcpConnect = false;
                
                _tcp.TcpClient.Close();
                _tcp.NetworkStream.Close();

                _tcp.ReceiveData = null;
                
                _tcp = null;
            }

            if (_udp is {IsUdpConnect: true})
            {
                _udp.IsUdpConnect = false;
                
                _udp.UdpClient.Close();
                _udp.EndPoint = null;
                
                _udp = null;
            }
            if(RulesHandler.Rules != null) RulesHandler.Clear();
        }
    }
}
