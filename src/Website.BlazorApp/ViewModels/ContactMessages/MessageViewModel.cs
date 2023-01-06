namespace Website.BlazorApp.ViewModels.ContactMessages;

public class MessageViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? CheckedDate { get; set; }
    public bool IsChecked { get; set; }
}
