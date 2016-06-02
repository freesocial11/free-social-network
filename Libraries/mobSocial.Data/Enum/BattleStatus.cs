namespace mobSocial.Data.Enum
{
    public enum BattleStatus
    {
        Pending = 0, //battle is pending. videos have not been uploaded or verified by admin
        Open = 1, //battle is open
        Closed = 2, //battle is closed. no result at all
        Complete = 3, //battle is complete and we have some result
        Locked = 4 //battle is locked so it's visible to public but videos are not. we are waiting for scheduler to do the honors
    }
}
