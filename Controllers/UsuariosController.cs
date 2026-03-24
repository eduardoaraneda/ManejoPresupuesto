using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace ManejoPresupuesto.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly IServicioEmail servicioEmail;

        public UsuariosController(UserManager<Usuario> userManager,
                                  SignInManager<Usuario> signInManager, IServicioEmail servicioEmail)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.servicioEmail = servicioEmail;
        }
        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new Usuario { Email = modelo.Email };

            var resultado = await userManager.CreateAsync(usuario, modelo.Password);

            if (resultado.Succeeded)
            {
                // ✅ Se pasa la clase Usuario correcta
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Transacciones");
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            var resultado = await signInManager.PasswordSignInAsync(modelo.Email,
                                                                     modelo.Password,
                                                                     modelo.Recuerdame,
                                                                     lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Transacciones");

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
                return View(modelo);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult OlvideMiPassword(string mensaje = "")
        {
            ViewBag.Mensaje = mensaje;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> OlvideMiPassword(OlvideMiPassword modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            var usuario = await userManager.FindByEmailAsync(modelo.Email);
            if (usuario is null)
            {
                return RedirectToAction(nameof(OlvideMiPassword), new { mensaje = "Se ha enviado un email si el usuario existe" });
            }
            var codigo = await userManager.GeneratePasswordResetTokenAsync(usuario);
            var codigo64 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));
            var enlace = Url.Action("ResetPassword", "Usuarios",
                                 new { codigo = codigo64 },
                                 protocol: Request.Scheme);
            await servicioEmail.EnviarEmail(modelo.Email, enlace);
            return RedirectToAction(nameof(OlvideMiPassword), new { mensaje = "Se ha enviado un email si el usuario existe" });
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string codigo = null)
        {
            if (codigo is null)
            {
                var mensaje = "Se debe proveer un código para resetear el password.";
                return RedirectToAction("OlvideMiPassword", new { mensaje });
            }
            var modelo = new RecuperarPasswordViewModel();
            modelo.codigoreseteo = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(codigo));
            return View(modelo);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(RecuperarPasswordViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            var usuario = await userManager.FindByEmailAsync(modelo.Email);
            if (usuario is null)
            {
                return RedirectToAction("PasswordCambiado");
            }
            var resultado = await userManager.ResetPasswordAsync(usuario, modelo.codigoreseteo, modelo.password);
            if (resultado.Succeeded)
            {
                return RedirectToAction("PasswordCambiado");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }
        }
        public IActionResult PasswordCambiado()
        {
            return View();
        }
    }
}
