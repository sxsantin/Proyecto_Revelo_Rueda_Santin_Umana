    using System;
    using System.IO;

    namespace Seguridad
    {
        public static class LogService
        {
            private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppLogs.txt");

            public static void WriteLog(string message)
            {
                try
                {
                    using (var writer = new StreamWriter(LogFilePath, true))
                    {
                        writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores al escribir en el log
                    Console.WriteLine($"Error al escribir en el log: {ex.Message}");
                }
            }

            public static void LogLogin(string username, bool success)
            {
                var status = success ? "éxito" : "fallo";
                WriteLog($"Inicio de sesión de usuario '{username}' con {status}.");
            }

            public static void LogLogout(string username)
            {
                WriteLog($"Cierre de sesión de usuario '{username}'.");
            }

            public static void LogAction(string username, string action)
            {
                WriteLog($"Acción realizada por '{username}': {action}");
            }
        }
    }
