namespace Note2Book.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    public string Name { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string RepeatPassword { get; set; }
}