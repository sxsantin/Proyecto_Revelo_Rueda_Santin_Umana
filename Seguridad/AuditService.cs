using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguridad
{
    public class AuditService
    {
        public static void LogLoginAttempt(string username, bool success)
        {
            // Registra el intento de inicio de sesión
            Console.WriteLine($"Intento de login de {username}. Éxito: {success}");
        }

        public static void SendAlert(string message)
        {
            // Enviar alertas por correo electrónico o a un sistema de monitoreo
            Console.WriteLine($"Alerta: {message}");
        }
    }
}
