namespace kando_backend.Helpers
{
    public class EmailTemplates
    {
        public static string GetInvitationTemplate(string teamName, string inviterName)
        {
            return $@"
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; color: #333; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px; }}
                .header {{ text-align: center; margin-bottom: 20px; }}
                .btn {{ display: inline-block; padding: 10px 20px; background-color: #3B82F6; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                .footer {{ margin-top: 30px; font-size: 12px; color: #999; text-align: center; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <img src='https://i.ibb.co/TDmrmZ5W/kando-icon.png' alt='Kando Header' width='150'>
                </div>
                <h2 style='text-align: center;'>Has sido invitado a un equipo</h2>
                <p>Hola,</p>
                <p><strong>{inviterName}</strong> te ha invitado a colaborar en el equipo <strong>{teamName}</strong> en Kando.</p>
                
                <div style='text-align: center; margin: 30px 0;'>
                    <p>Abre la aplicación para aceptar o rechazar la invitación.</p>
                </div>

                <div class='footer'>
                    Este correo es informativo. Si no esperabas esta invitación, ignórala.
                </div>
            </div>
        </body>
        </html>";
        }
    }
}
