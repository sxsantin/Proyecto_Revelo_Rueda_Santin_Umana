using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Seguridad
{
    public class SecurityHelper
    {
        // Método para generar el hash de la contraseña
        public static string HashPassword(string password) // Cambiado byte[] a string
        {
            return BCrypt.Net.BCrypt.HashPassword(password); // Añadido BCrypt.Net.BCrypt
        }

        // Método para verificar la contraseña
        public static bool VerifyPassword(string password, string hashedPassword) // Cambiado byte[] a string
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword); // Añadido BCrypt.Net.BCrypt
        }
    }
}