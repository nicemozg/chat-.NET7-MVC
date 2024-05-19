using Chat.Context;
using Chat.Models;
using Chat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.Controllers;

public class ChatController : Controller
{
    private ChatContext _db;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public ChatController(ChatContext db, UserManager<User> userManager, IWebHostEnvironment hostingEnvironment)
    {
        _db = db;
        _userManager = userManager;
        _hostingEnvironment = hostingEnvironment;
    }


    // GET
    public IActionResult Index()
    {
        var chats = _db.Chats.ToList(); // Получить список всех чатов из базы данных
        return View(chats); // Передать список чатов в представление
    }


    [HttpGet]
    public async Task<IActionResult> AddChat()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> AddChat(ChatRoom chat)
    {
        if (ModelState.IsValid)
        {
            _db.Chats.Add(chat);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Chat");
        }

        return View(chat);
    }
    
    [HttpPost]
    public async Task<IActionResult> JoinChat(int chatId)
    {
        var currentChat = await _db.Chats.FindAsync(chatId);
        if (currentChat == null)
        {
            return NotFound(); 
        }
        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrEmpty(currentUser?.Id))
        {
            return RedirectToAction("Register", "Account");
        }
        ChatRoom chat = new ChatRoom();
        chat.Name = currentChat.Name;
        chat.Participants = new List<User>();
        
        chat.Participants.Add(currentUser);
        await _db.SaveChangesAsync();
        
        return RedirectToAction("Chat", new { chatId = currentChat.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Chat(int chatId)
    {
        var chat = await _db.Chats.Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
        {
            return NotFound();
        }
        var chatViewModel = new ChatViewModel
        {
            Chat = chat,
            Messages = chat.Messages,
        };
        return View(chatViewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> SendMessage(ChatViewModel chatViewModel)
    {
        if (ModelState.IsValid)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(currentUser?.Id))
            {
                return RedirectToAction("Register", "Account");
            }

            if (currentUser != null)
            {
                var newMessage = new Message
                {
                    Text = chatViewModel.NewMessage.Text,
                    UserName = currentUser.UserName,
                    Timestamp = DateTime.Now,
                    AvatarFileName = currentUser.AvatarFileName
                };

                var chat = await _db.Chats.Include(c => c.Messages)
                    .FirstOrDefaultAsync(c => c.Id == chatViewModel.Chat.Id);
            
                chat.Messages.Add(newMessage);
            
                await _db.SaveChangesAsync();
            
                chatViewModel.NewMessage.Text = string.Empty;
            
                return RedirectToAction("Chat", new { chatId = chatViewModel.Chat.Id });
            }
        }

        // Если модель недопустима, верните пользователя обратно на страницу чата с сообщением об ошибке
        return View("Chat", chatViewModel);
    }


}