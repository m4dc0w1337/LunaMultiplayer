﻿using LunaClient.Base;
using LunaClient.Base.Interface;
using LunaClient.Systems.VesselRemoveSys;
using LunaClient.VesselStore;
using LunaCommon.Enums;
using LunaCommon.Message.Data.Vessel;
using LunaCommon.Message.Interface;
using LunaCommon.Message.Types;
using System;
using System.Collections.Concurrent;

namespace LunaClient.Systems.VesselProtoSys
{
    public class VesselProtoMessageHandler : SubSystem<VesselProtoSystem>, IMessageHandler
    {
        public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

        public void HandleMessage(IServerMessageBase msg)
        {
            if (!(msg.Data is VesselBaseMsgData msgData)) return;

            switch (msgData.VesselMessageType)
            {
                case VesselMessageType.VesselsReply:
                    HandleVesselResponse((VesselsReplyMsgData)msgData);
                    break;
                case VesselMessageType.Proto:
                    HandleVesselProto((VesselProtoMsgData)msgData);
                    break;
                default:
                    LunaLog.LogError($"[LMP]: Cannot handle messages of type: {msgData.VesselMessageType} in VesselProtoMessageHandler");
                    break;
            }
        }

        private static void HandleVesselProto(VesselProtoMsgData messageData)
        {
            if (!SystemsContainer.Get<VesselRemoveSystem>().VesselWillBeKilled(messageData.Vessel.VesselId) && messageData.Vessel.VesselId != Guid.Empty)
            {
                VesselsProtoStore.HandleVesselProtoData(messageData.Vessel.Data, messageData.Vessel.NumBytes, messageData.Vessel.VesselId);
            }
        }

        private static void HandleVesselResponse(VesselsReplyMsgData messageData)
        {
            //We read the vessels syncronously so when we start the game we have the dictionary of all the vessels already loaded
            for (var i = 0; i < messageData.VesselsCount; i++)
            {
                if (!SystemsContainer.Get<VesselRemoveSystem>().VesselWillBeKilled(messageData.VesselsData[i].VesselId))
                    VesselsProtoStore.HandleVesselProtoData(messageData.VesselsData[i].Data, messageData.VesselsData[i].NumBytes, messageData.VesselsData[i].VesselId);
            }

            MainSystem.NetworkState = ClientState.VesselsSynced;
        }
    }
}
