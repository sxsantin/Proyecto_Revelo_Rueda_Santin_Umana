using System;
using System.Linq;
using System.Text;

namespace Seguridad
{
    public class UserService
    {
        private readonly SecurityDBEntities _context;

        public UserService(SecurityDBEntities context)
        {
            _context = context;
        }

        // Validar la fortaleza de la contraseña
        public bool ValidatePasswordStrength(string password)
        {
            return password.Length >= 8 && password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
        }

        // Crear un nuevo usuario
        public void CreateUser(string username, string password, string email, string role)
        {
            if (!ValidatePasswordStrength(password))
                throw new Exception("La contraseña no es lo suficientemente fuerte.");

            var hashedPassword = SecurityHelper.HashPassword(password);
            var user = new Users
            {
                Username = username,
                PasswordHash = Encoding.UTF8.GetBytes(hashedPassword), // Convertir string a byte[]
                Email = email,
                Role = role // Asignar rol
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        // Recuperar un usuario por nombre de usuario
        public Users GetUserByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);
        }

        // Recuperar un usuario por ID
        public Users GetUserById(int userId)
        {
            return _context.Users.SingleOrDefault(u => u.UserId == userId);
        }

        // Validar las credenciales del usuario
        public Users AuthenticateUser(string username, string password)
        {
            var user = GetUserByUsername(username);
            if (user == null)
                throw new Exception("Usuario no encontrado.");

            var hashedPassword = Encoding.UTF8.GetString(user.PasswordHash);
            if (!SecurityHelper.VerifyPassword(password, hashedPassword))
                throw new Exception("Contraseña incorrecta.");

            return user;
        }

        // Verificar si el usuario tiene un rol específico
        public bool UserHasRole(int userId, string role)
        {
            var user = GetUserById(userId);
            return user != null && user.Role.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        // Enviar un enlace de recuperación de contraseña
        public void SendPasswordRecoveryLink(string email)
        {
            // Envía un enlace temporal para la recuperación de la contraseña
            Console.WriteLine($"Enlace de recuperación enviado a {email}");
        }
    }
}
