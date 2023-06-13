using System;
using UnityEngine;

namespace beepe0.UNetwork
{
    public abstract class UNetworkLogs
    {
        public enum ServerInfoType
        {
            Log,
            Error,
            Warning,
        }

        public static readonly string[] InfoType = new string[] { "[LOG]", "[ERROR]", "[WARNING]"};
        
        public static void ErrorReceivingTcp(Exception exception = null) => Debug.LogError(InfoType[(int)ServerInfoType.Error] + $": {DateTime.Now} Receiving TCP data. {exception}");
        public static void ErrorReceivingUdp(Exception exception = null) => Debug.LogError(InfoType[(int)ServerInfoType.Error] + $": {DateTime.Now} Receiving UDP data. {exception} ");
        public static void ErrorSendingTcp(Exception exception = null) => Debug.LogError(InfoType[(int)ServerInfoType.Error] + $": {DateTime.Now} Sending TCP data. {exception} ");
        public static void ErrorSendingUdp(Exception exception = null) => Debug.LogError(InfoType[(int)ServerInfoType.Error] + $": {DateTime.Now} Sending UDP data. {exception} ");
        public static void ErrorNullFunc(Exception exception = null) => Debug.LogError(InfoType[(int)ServerInfoType.Error] + $": {DateTime.Now} No action. {exception}");
        public static void ErrorSingletonOnce(Exception exception = null) => Debug.LogError(InfoType[(int)ServerInfoType.Error] + $": {DateTime.Now} The class is already used {exception}");
        public static void ErrorCloseConnection(Exception exception = null) => Debug.LogError(InfoType[(int)ServerInfoType.Error] + $": {DateTime.Now} NetworkMaster is null {exception}");
    }
}