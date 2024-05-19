using Chat.Models;

namespace Chat.ViewModels;

public class ChatViewModel
{
    public ChatRoom Chat { get; set; } // Информация о чате
    public List<Message> Messages { get; set; } // Список сообщений в чате
    
    public Message NewMessage { get; set; } // Новое сообщение, которое пользователь отправит

    public ChatViewModel()
    {
    }
    
}