using System.ComponentModel.DataAnnotations;

namespace Website.BlazorApp.ViewModels.ContactMessages;

public class UpdateMessageViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    
    public string Email { get; set; }
    
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Content of message is required.")]
    public string Content { get; set; }
}