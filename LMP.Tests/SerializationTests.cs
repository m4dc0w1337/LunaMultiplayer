﻿using Lidgren.Network;
using LunaCommon.Message;
using LunaCommon.Message.Client;
using LunaCommon.Message.Data.Chat;
using LunaCommon.Message.Data.Vessel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LMP.Tests
{
    [TestClass]
    public class SerializationTests
    {
        private static readonly ServerMessageFactory Factory = new ServerMessageFactory();
        private static readonly Random Rnd = new Random();
        private static readonly NetClient Client = new NetClient(new NetPeerConfiguration("TESTS"));

        [TestMethod]
        public void TestSerializeDeserializeVesselUpdateMsg()
        {
            var msgData = Factory.CreateNewMessageData<VesselUpdateMsgData>();
            msgData.VesselId = Guid.NewGuid();
            msgData.Name = "Name";
            msgData.Type = "Type";
            msgData.Situation = "Situation";
            msgData.Landed = true;
            msgData.LandedAt = "LandedAt";
            msgData.DisplayLandedAt = "DisplayLandedAt";
            msgData.Splashed = false;
            msgData.MissionTime = Rnd.NextDouble();
            msgData.LaunchTime = Rnd.NextDouble();
            msgData.LastUt = Rnd.NextDouble();
            msgData.Persistent = false;
            msgData.RefTransformId = (uint)Rnd.Next();

            for (var i = 0; i < 17; i++)
            {
                if (msgData.ActionGroups[i] == null)
                    msgData.ActionGroups[i] = new ActionGroup();

                msgData.ActionGroups[i].ActionGroupName = "ActionGroupName" + i;
                msgData.ActionGroups[i].State = true;
                msgData.ActionGroups[i].Time = Rnd.NextDouble();
            }

            var msg = Factory.CreateNew<VesselCliMsg>(msgData);
            
            //Serialize
            var expectedDataSize = msg.GetMessageSize();
            var lidgrenMsgSend = Client.CreateMessage(expectedDataSize);
            msg.Serialize(lidgrenMsgSend);
            var realSize = lidgrenMsgSend.LengthBytes;

            //Usually the expected size will be a bit more as Lidgren writes the size of the strings in a base128 int (so it uses less bytes)
            Assert.IsTrue(expectedDataSize >= realSize);

            //Simulate sending
            var data = lidgrenMsgSend.ReadBytes(lidgrenMsgSend.LengthBytes);
            var lidgrenMsgRecv = Client.CreateIncomingMessage(NetIncomingMessageType.Data, data);
            lidgrenMsgRecv.LengthBytes = lidgrenMsgSend.LengthBytes;

            msg.Recycle();

            //Deserialize
            var msgDes = Factory.Deserialize(lidgrenMsgRecv, Environment.TickCount);

        }

        [TestMethod]
        public void TestSerializeDeserializeChatChannelMsg()
        {
            var msgData = Factory.CreateNewMessageData<ChatChannelMsgData>();
            msgData.Channel = "C";
            msgData.SendToAll = true;
            msgData.Text = "T";

            var msg = Factory.CreateNew<ChatCliMsg>(msgData);

            //Serialize
            var expectedDataSize = msg.GetMessageSize();
            var lidgrenMsgSend = Client.CreateMessage(expectedDataSize);
            msg.Serialize(lidgrenMsgSend);
            var realSize = lidgrenMsgSend.LengthBytes;

            //Usually the expected size will be a bit more as Lidgren writes the size of the strings in a base128 int (so it uses less bytes)
            Assert.IsTrue(expectedDataSize >= realSize);

            //Simulate sending
            var data = lidgrenMsgSend.ReadBytes(lidgrenMsgSend.LengthBytes);
            var lidgrenMsgRecv = Client.CreateIncomingMessage(NetIncomingMessageType.Data, data);
            lidgrenMsgRecv.LengthBytes = lidgrenMsgSend.LengthBytes;

            msg.Recycle();

            //Deserialize
            var msgDes = Factory.Deserialize(lidgrenMsgRecv, Environment.TickCount);

        }
    }
}
