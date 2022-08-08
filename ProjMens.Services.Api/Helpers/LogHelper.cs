using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using ProjMens.Services.Api.Settings;
using System.Reflection;
using System.Text;

namespace ProjMens.Services.Api.Helpers
{
    public class LogHelper
    {
        //atributo
        private readonly LogSettings _logSettings;

        //construtor
        public LogHelper(IOptions<LogSettings> logSettings)
        {
            _logSettings = logSettings.Value;
        }

        public void Create(string message, LogType logType)
        {
            //caminho relativo do projeto em execução
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


            var file = Path.Combine($"{path}\\{_logSettings.FileName}");

            //criando o arquivo caso não exista
            if (!File.Exists(file))
            {
                var stream = File.Create(file);
                stream.Close();
            }

            //gravando o conteudo do arquivo
            using (var stream = File.AppendText(file))
            {
                var builder = new StringBuilder();

                builder.Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                builder.Append(" - ");
                builder.Append(logType.ToString());
                builder.Append(" - ");
                builder.Append(message);

                stream.WriteLine(builder.ToString());
            }

        }

    }
    //tipos de registro no log
    public enum LogType
    {
        INFO, ERROR
    }
}




