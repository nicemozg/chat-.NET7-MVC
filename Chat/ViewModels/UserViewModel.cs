using System.ComponentModel.DataAnnotations;
using Chat.Validation;

namespace Chat.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 20 символов")]
    [Display(Name = "Login")]
    [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*]+$", ErrorMessage = "Логин должен содержать только латинские буквы, цифры и символы: !@#$%^&*")]
    public string UserName { get; set; }
    
    [Display(Name = "Email")]
    [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.\w+$", ErrorMessage = "НЕ валидный адрес электронной почты")]
    public string Email { get; set; }
    
    [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Максимальный размер файла - 10 МБ")]
    [Display(Name = "Загрузить изображение")]
    public IFormFile? AvatarFile { get; set; }
    
    public string? AvatarFileName { get; set; }
    
    
    [Display(Name = "Дата рождения")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime DateOfBirth { get; set; }
}