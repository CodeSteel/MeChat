using System.Collections.Concurrent;

namespace MeChat.Controllers;

/// <summary>
/// https://stackoverflow.com/a/27848487
/// </summary>
public static class SignalRConnectionToGroupsMap
{
    private static readonly ConcurrentDictionary<string, List<string>>  Map = new ConcurrentDictionary<string, List<string>>();

    public static bool TryAddGroup(string connectionId, string groupName)
    {
        List<string> groups;

        if (!Map.TryGetValue(connectionId, out groups))
        {
            return Map.TryAdd(connectionId, new List<string>() {groupName});
        }

        if (!groups.Contains(groupName))
        {
            groups.Add(groupName);
        }

        return true;
    }

    // since for this use case we will only want to get the List of group names
    // when we're removing the mapping - we might as well remove the mapping while
    // we're grabbing the List
    public static bool TryRemoveConnection(string connectionId, out List<string> result)
    {
        return Map.TryRemove(connectionId, out result);
    }
}