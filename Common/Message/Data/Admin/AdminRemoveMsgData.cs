﻿using Lidgren.Network;
using LunaCommon.Message.Base;
using LunaCommon.Message.Types;

namespace LunaCommon.Message.Data.Admin
{
    public class AdminRemoveMsgData : AdminBaseMsgData
    {
        /// <inheritdoc />
        internal AdminRemoveMsgData() { }
        public override AdminMessageType AdminMessageType => AdminMessageType.Remove;

        public string PlayerName;

        public override string ClassName { get; } = nameof(AdminRemoveMsgData);

        internal override void InternalDeserialize(NetIncomingMessage lidgrenMsg)
        {
            base.InternalDeserialize(lidgrenMsg);
            PlayerName = lidgrenMsg.ReadString();
        }

        internal override void InternalSerialize(NetOutgoingMessage lidgrenMsg)
        {
            base.InternalSerialize(lidgrenMsg);
            lidgrenMsg.Write(PlayerName);
        }

        internal override int InternalGetMessageSize()
        {
            return base.InternalGetMessageSize() + PlayerName.GetByteCount();
        }
    }
}