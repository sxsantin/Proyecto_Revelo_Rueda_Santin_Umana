using Seguridad;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System;

public class AccountController : Controller
{
    private readonly UserService _userService;
    private readonly SecurityService _securityService; 

    public AccountController()
    {
        _userService = new UserService(new SecurityDBEntities());
        _securityService = new SecurityService(); 
    }

    // Vista de Login
    public ActionResult Login()
    {
        return View();
    }

    // Acción para Login
    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        try
        {
            // Verificar si el usuario está bloqueado por demasiados intentos fallidos
            if (LoginAttemptService.IsLockedOut())
            {
                ViewBag.ErrorMessage = "Demasiados intentos fallidos. Intenta nuevamente más tarde.";
                LogService.LogAction(username, "Intento de inicio de sesión bloqueado por demasiados intentos fallidos.");
                return View();
            }

            // Intentar autenticar al usuario
            var user = _userService.AuthenticateUser(username, password);

            // Si la autenticación es exitosa, reiniciar los intentos fallidos
            LoginAttemptService.ResetFailedAttempts();

            // Registrar el inicio de sesión exitoso
            LogService.LogLogin(username, true);

            // Guardar el ID del usuario en la sesión
            Session["UserId"] = user.UserId;
            Session["Role"] = user.Role; // Si necesitas guardar el rol

            // Redirigir a la página principal
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            // Si ocurre un error (como una contraseña incorrecta), registrar el intento fallido
            LoginAttemptService.RecordFailedAttempt();

            // Registrar el inicio de sesión fallido
            LogService.LogLogin(username, false);

            // Mostrar el mensaje de error
            ViewBag.ErrorMessage = ex.Message;
            return View();
        }
    }


    // Vista de Registro
    public ActionResult Register()
    {
        return View();
    }

    // Acción para Registro
    [HttpPost]
    public ActionResult Register(string username, string password, string email, string role)
    {
        try
        {
            // Validar que el nombre de usuario no exista
            var existingUser = _userService.GetUserByUsername(username);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "El nombre de usuario ya está en uso.";
                return View();
            }

            // Crear el nuevo usuario
            _userService.CreateUser(username, password, email, role);

            ViewBag.SuccessMessage = "Cuenta creada exitosamente. Ahora puedes iniciar sesión.";
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View();
        }
    }

    // Acción para Logout
    public ActionResult Logout()
    {
        // Registrar el cierre de sesión antes de limpiar la sesión
        var username = Session["UserId"] != null ? _userService.GetUserById((int)Session["UserId"])?.Username : "Usuario desconocido";
        LogService.LogLogout(username);

        // Limpiar la sesión
        Session.Clear();
        return RedirectToAction("Login");
    }
}
