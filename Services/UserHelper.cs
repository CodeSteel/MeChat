using MeChat.Models;

namespace MeChat.Services;

public static class UserHelper
{
    public static bool IsFriendsWith(this User me, User targetUser)
    {
        UserRelationship? relationship = me.UserRelationships.FirstOrDefault(x => x.TargetUser.Id == targetUser.Id);
        return relationship is { Type: UserRelationshipType.Friend };
    }

    public static bool HasSentFriendRequest(this User me, User targetUser)
    {
        UserRelationship? relationship = me.UserRelationships.FirstOrDefault(x => x.TargetUser.Id == targetUser.Id);
        return relationship is { Type: UserRelationshipType.FriendRequestSent };
    }
    
    public static bool HasFriendRequestPending(this User me, User targetUser)
    {
        UserRelationship? relationship = me.UserRelationships.FirstOrDefault(x => x.TargetUser.Id == targetUser.Id);
        return relationship is { Type: UserRelationshipType.FriendRequestReceived };
    }
}