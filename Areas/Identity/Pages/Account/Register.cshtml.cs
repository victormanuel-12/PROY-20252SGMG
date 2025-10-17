// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace SGMG.Areas.Identity.Pages.Account
{
  public class RegisterModel : PageModel
  {
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;

    private readonly RoleManager<IdentityRole> _roleManager;

    // Lista de roles con GUID como Value
    public List<SelectListItem> RolesList { get; set; } = new List<SelectListItem>
{
    new SelectListItem { Value = "f3a2c1d4-1234-5678-9abc-def012345678", Text = "ADMINISTRADOR" },
    new SelectListItem { Value = "b1c2d3e4-2345-6789-abcd-ef0123456789", Text = "MEDICO" },
    new SelectListItem { Value = "c1d2e3f4-3456-789a-bcde-f01234567890", Text = "ENFERMERIA" },
    new SelectListItem { Value = "d1e2f3a4-4567-89ab-cdef-012345678901", Text = "CAJERO" }
};


    public RegisterModel(
    UserManager<IdentityUser> userManager,
    IUserStore<IdentityUser> userStore,
    SignInManager<IdentityUser> signInManager,
    RoleManager<IdentityRole> roleManager,  // <- aquí
    ILogger<RegisterModel> logger)
    {
      _userManager = userManager;
      _userStore = userStore;
      _emailStore = GetEmailStore();
      _signInManager = signInManager;
      _roleManager = roleManager;  // <- asignar
      _logger = logger;

    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }
    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
      [Required]
      [EmailAddress]
      [Display(Name = "Email")]
      public string Email { get; set; }

      [Required]
      [StringLength(100, MinimumLength = 6)]
      [DataType(DataType.Password)]
      [Display(Name = "Password")]
      public string Password { get; set; }

      [DataType(DataType.Password)]
      [Display(Name = "Confirm password")]
      [Compare("Password")]
      public string ConfirmPassword { get; set; }

      [Required(ErrorMessage = "Please select a role")]
      [Display(Name = "Role")]
      public string RoleId { get; set; } // Ahora es string porque es GUID
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
      ReturnUrl = returnUrl;
      ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
      returnUrl ??= Url.Content("~/");
      ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

      if (ModelState.IsValid)
      {
        var user = CreateUser();

        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

        // 🔹 Activar LockoutEnabled y EmailConfirmed al crear el usuario
        user.LockoutEnabled = true;
        user.EmailConfirmed = true; // ✅ Marcar como confirmado automáticamente

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
          _logger.LogInformation("User created a new account with password.");

          // 🔹 Asignar rol según el GUID seleccionado
          var roleName = RolesList.FirstOrDefault(r => r.Value == Input.RoleId)?.Text;
          if (!string.IsNullOrEmpty(roleName))
          {
            await _userManager.AddToRoleAsync(user, roleName);
          }

          // 🔹 Iniciar sesión inmediatamente
          await _signInManager.SignInAsync(user, isPersistent: false);

          // 🔹 Redirigir según el rol asignado
          return roleName switch
          {
            "ADMINISTRADOR" => LocalRedirect("/Admin/Dashboard"),
            "MEDICO" => LocalRedirect("/Medico/Home"),
            "ENFERMERIA" => LocalRedirect("/Enfermeria/Home"),
            "CAJERO" => LocalRedirect("/Cajero/Home"),
            _ => LocalRedirect(returnUrl)
          };
        }

        // Mostrar errores si falla la creación
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }

      // Si algo falla, volver a mostrar la página con errores
      return Page();
    }



    private IdentityUser CreateUser()
    {
      try
      {
        return Activator.CreateInstance<IdentityUser>();
      }
      catch
      {
        throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'.");
      }
    }

    private IUserEmailStore<IdentityUser> GetEmailStore()
    {
      if (!_userManager.SupportsUserEmail)
      {
        throw new NotSupportedException("The default UI requires a user store with email support.");
      }
      return (IUserEmailStore<IdentityUser>)_userStore;
    }
  }
}