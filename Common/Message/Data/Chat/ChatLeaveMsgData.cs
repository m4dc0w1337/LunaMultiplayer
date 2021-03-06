﻿using Lidgren.Network;
using LunaCommon.Message.Base;
using LunaCommon.Message.Types;

namespace LunaCommon.Message.Data.Chat
{
    public class ChatLeaveMsgData : ChatBaseMsgData
    {
        /// <inheritdoc />
        internal ChatLeaveMsgData() { }
        public override ChatMessageType ChatMessageType => ChatMessageType.Leave;

        public string Channel;

        public override string ClassName { get; } = nameof(ChatLeaveMsgData);

        internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
        {
            base.InternalSerialize(lidgrenMsg);

            lidgrenMsg.Write(Channel);
        }

        internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
        {
            base.InternalDeserialize(lidgrenMsg);

            Channel = lidgrenMsg.ReadString();
        }

        internal override int InternalGetMessageSize()
        {
            return base.InternalGetMessageSize() + Channel.GetByteCount();
        }
    }
}