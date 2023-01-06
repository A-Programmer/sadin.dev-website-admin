namespace Website.BlazorApp.ViewModels.ContactMessages;

public class MessageAdminListItemViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Title { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? CheckedDate { get; set; }
    public bool IsChecked { get; set; }
}
