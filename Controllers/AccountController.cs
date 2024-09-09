using MeChat.Contexts;
using MeChat.Models;
using MeChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeChat.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly ApplicationDataContext _dataContext;
    private readonly IWebHostEnvironment _environment;
    private readonly FileUploader _fileUploader;
    
    public AccountController(ILogger<AccountController> logger, ApplicationDataContext dataContext, IWebHostEnvironment environment, FileUploader fileUploader)
    {
        _logger = logger;
        _dataContext = dataContext;
        _environment = environment;
        _fileUploader = fileUploader;
    }
    
    [Authorize]
    public async Task<IActionResult> Index()
    {
        User? user = await _dataContext.Users
            .Include(x => x.Friends)
            .Include(x => x.FriendRequests)
            .Include(x => x.ProfilePicture)
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        return View(user);
    }
    
    [Authorize]
    public async Task<IActionResult> Profile(Guid userId)
    {
        User? user = await _dataContext.Users
            .Include(x => x.Friends)
            .Include(x => x.ChatGroups)
            .ThenInclude(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == userId);
        return View(user);
    }
    
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SaveAccount(User userForm)
    {
        User? user = await _dataContext.Users
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        if (user == null)
        {
            return BadRequest("User not found.");
        }
        
        _dataContext.Update(user);
        
        if (userForm.ProfilePictureUpload != null)
        {
            string fullPath =
                _fileUploader.GetFilePath(user, userForm.ProfilePictureUpload, out string databaseFilePath);
            if (_fileUploader.GetFileExists(databaseFilePath, out FileUpload? fileUpload))
            {
                user.ProfilePicture = fileUpload!;
            }
            else
            {
                FileUpload newFileUpload = await _fileUploader.UploadFile(user, userForm.ProfilePictureUpload);
                user.ProfilePicture = newFileUpload;
            }
        }

        user.DisplayName = userForm.DisplayName;
        await _dataContext.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
}