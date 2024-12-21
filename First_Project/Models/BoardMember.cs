using SecureMVCApp.Models;

public enum BoardTitle
{
    CEO,
    CFO,
    CIO
}

public class BoardMember : BaseClass
{
    public BoardMember(string firstName, string lastName, string email, BoardTitle title)
        : base(firstName, lastName, email)
    {
        Title = title;
    }

    public BoardTitle Title { get; set; }

}

