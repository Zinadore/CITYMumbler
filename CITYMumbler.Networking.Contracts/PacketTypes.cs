﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CITYMumbler.Networking.Contracts
{
    public enum PacketType :  byte
    {
        Connection,
		Connected,
		Disconnection,
	    GroupMessage,
	    PrivateMessage,
	    JoinGroup,
		JoinedGroup,
		DeleteGroup,
	    ChangeGroupOwner,
	    LeaveGroup,
	    LeftGroup,
	    Kick,
	    CreateGroup,
	    SendKeystroke,
	    SendGroups,
	    RequestSendGroups,
	    SendUsers,
	    RequestSendUsers
    }

    public enum LeftGroupTypes : byte
    {
        Normal,
        Kicked,
        TimeoutReached
    }

    public enum JoinGroupPermissionTypes : byte
    {
        Free,
        Password,
        Permission
    }
}